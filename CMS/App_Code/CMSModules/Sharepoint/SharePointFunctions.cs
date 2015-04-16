using System;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.PortalEngine;
using CMS.Base;
using CMS.SiteProvider;


/// <summary>
/// SharePoint functions.
/// </summary>
public static class SharePointFunctions
{
    /// <summary>
    /// Returns URL link to get specified document or picture from.
    /// </summary>
    /// <param name="server">SharePoint server URL</param>
    /// <param name="name">Path and name of document to download</param>    
    public static string GetSPFileUrl(string server, string name)
    {
        return GetSharePointFileUrl(server, name);
    }


    /// <summary>
    /// Returns URL link to get specified document or picture from.
    /// </summary>
    /// <param name="server">SharePoint server URL</param>
    /// <param name="name">Path and name of document to download</param>    
    public static string GetSharePointFileUrl(string server, string name)
    {
        return URLHelper.ResolveUrl("~/CMSModules/SharePoint/CMSPages/GetSharePointFile.aspx?server=" + server + "&name=" + name);
    }


    /// <summary>
    /// Helper function for splitting combined fields returned by SharePoint. Eg. ows_LastModified="1;#2009-03-17T10:32:17Z".
    /// </summary>
    /// <param name="value">Original combined value</param>
    /// <param name="index">Index of the returned part</param>
    /// <returns>Returns specified part of field</returns>
    public static string SplitSPField(string value, int index)
    {
        return SplitSharePointField(value, index);
    }


    /// <summary>
    /// Helper function for splitting combined fields returned by SharePoint. Eg. ows_LastModified="1;#2009-03-17T10:32:17Z".
    /// </summary>
    /// <param name="value">Original combined value</param>
    /// <param name="index">Index of the returned part</param>
    /// <returns>Returns specified part of field</returns>
    public static string SplitSharePointField(string value, int index)
    {
        if (value != null)
        {
            string[] values = value.Split('#');
            if (values.Length > index)
            {
                return values[index];
            }
        }

        return null;
    }


    /// <summary>
    /// Returns ICredential object for SharePoint authentication based on CMS settings.
    /// </summary>    
    public static ICredentials GetSharePointCredetials()
    {
        ICredentials credentials = null;

        // Check settings
        string siteName = SiteContext.CurrentSiteName;
        bool useDefaultCred = SettingsKeyInfoProvider.GetBoolValue(siteName + ".CMSSharePointDefaultCredentials");

        if (useDefaultCred)
        {
            // Use default credentials is checked
            credentials = CredentialCache.DefaultCredentials;
        }
        else
        {
            // Use settings username and password
            string username = SettingsKeyInfoProvider.GetValue(siteName + ".CMSSharePointUserName");
            string pass = EncryptionHelper.DecryptData(SettingsKeyInfoProvider.GetValue(siteName + ".CMSSharePointPassword"));
            credentials = GetSharePointCredetials(username, pass, false);
        }

        return credentials;
    }


    /// <summary>
    /// Returns ICredential object for SharePoint authentication based on given username and password.
    /// </summary>
    public static ICredentials GetSharePointCredetials(string username, string password, bool base64)
    {
        ICredentials credentials = null;

        string pass = password;

        // If password is in base64 encoding, convert it back
        if (base64)
        {
            pass = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(pass));
        }

        // Handle username like DOMAIN\username
        int domainIndex = username.LastIndexOfCSafe("\\");
        if (domainIndex > 0)
        {
            string domain = username.Substring(0, domainIndex);
            string user = username.Substring(domainIndex + 1);

            credentials = new NetworkCredential(user, pass, domain);
        }
        else
        {
            credentials = new NetworkCredential(username, pass);
        }

        return credentials;
    }


    /// <summary>
    /// Transformes CAML xml fragment trough XSL stylesheet.
    /// </summary>
    /// <param name="caml">Source xml = CAML fragment</param>
    /// <param name="ti">Transformation</param>
    /// <returns>Transformed text</returns>
    public static string TransformCAML(XmlNode caml, TransformationInfo ti)
    {
        XslCompiledTransform xslTransform = new XslCompiledTransform();

        // Read stylesheet code from transformation
        StringReader stream = new StringReader(ti.TransformationCode);
        XmlTextReader xmlReader = new XmlTextReader(stream);
        xslTransform.Load(xmlReader);

        // Create XMLDocument from CAML fragment
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(caml.InnerXml);

        StringWriter transformed = new StringWriter();
        xslTransform.Transform(xmlDoc, null, transformed);

        return transformed.ToString();
    }
}