using System;
using System.Web;
using System.Data;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading;

using CMS.Core;
using CMS.EmailEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.LicenseProvider;
using CMS.Base;
using CMS.SiteProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.DataEngine;

[Title("SubmitIssue.Title")]
[UIElementAttribute(ModuleName.CMS, "SubmitIssue")]
public partial class CMSModules_Support_Pages_SubmitIssue : GlobalAdminPage
{
    #region "Variables"

    private bool dialogMode = false;

    #endregion


    #region "Methods"

    protected override void OnPreInit(EventArgs e)
    {
        // Swap the master pages before Title attribute is processed in base.OnPreInit.
        dialogMode = QueryHelper.Contains("eventid");
        if (dialogMode)
        {
            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master";
            var master = CurrentMaster as ICMSModalMasterPage;
            if (master != null)
            {
                master.Save += btnSend_Click;
                master.ShowSaveAndCloseButton();
                master.SetSaveResourceString("general.send");
            }
        }
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (dialogMode)
        {
            pnlButtons.Visible = false;
        }
        rfvEmail.Text = GetString("Support.SubmiIssue.EmailEmpty");
        rfvSubject.Text = GetString("Support.SubmiIssue.SubjectEmpty");

        htmlTemplateBody.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        htmlTemplateBody.EditorAreaCSS = string.Empty;

        if (!RequestHelper.IsPostBack())
        {
            // Initialize system info
            txtSysInfo.Text = GetSystemInformation();

            ShowInformation(GetString("support.checksmtp"));
        }

        if (dialogMode)
        {
            ScriptHelper.RegisterWOpenerScript(Page);
        }
    }


    /// <summary>
    /// Send e-mail to support.
    /// </summary>
    protected void btnSend_Click(object sender, EventArgs e)
    {
        string result = new Validator()
            .NotEmpty(txtSubject.Text.Trim(), GetString("Support.SubmiIssue.SubjectEmpty"))
            .NotEmpty(txtEmail.Text.Trim(), GetString("Support.SubmiIssue.EmailEmpty"))
            .IsEmail(txtEmail.Text.Trim(), GetString("Support.SubmiIssue.EmailFormat"))
            .Result;

        if (!string.IsNullOrEmpty(result))
        {
            ShowError(result);
            return;
        }

        EmailMessage message = new EmailMessage
        {
            EmailFormat = EmailFormatEnum.Html,
            From = txtEmail.Text.Trim(),
            Subject = txtSubject.Text.Trim(),
            Recipients = "info@kenticoasia.com"
        };

        StringBuilder sb = new StringBuilder();

        sb.Append("<html><head></head><body>");
        sb.Append(htmlTemplateBody.ResolvedValue);
        sb.Append("<br /><div>=============== System information ==================<br />");
        sb.Append(txtSysInfo.Text.Replace("\r", string.Empty).Replace("\n", "<br />"));
        sb.Append("<br />==============================================</div><br />");
        sb.Append("<div>================ Template type ==================<br />");
        sb.Append(GetEngineInfo());
        sb.Append("<br />");
        sb.Append("=============================================</div>");
        sb.Append("</body></html>");

        message.Body = sb.ToString();

        // Add settings attachment
        if (chkSettings.Checked)
        {
            // Get settings data
            string settings = GetSettingsInfo();
            if (!string.IsNullOrEmpty(settings))
            {
                Stream stream = MemoryStream.New();

                // Put the file content in the stream
                StreamWriter writer = StreamWriter.New(stream, UnicodeEncoding.UTF8);
                writer.Write(settings);
                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                message.Attachments.Add(new Attachment(stream.SystemStream, "settings.txt", MediaTypeNames.Application.Octet));
            }
        }
        // Add uploaded attachment
        if (fileUpload.HasFile)
        {
            Attachment at = new Attachment(fileUpload.PostedFile.InputStream, fileUpload.FileName, MediaTypeNames.Application.Octet);
            if (at != null)
            {
                message.Attachments.Add(at);
            }
        }

        EmailSender.SendEmail(null, message, true);
        ShowInformation(GetString("Support.Success"));

        ClearForm();
    }


    /// <summary>
    /// Clears the form.
    /// </summary>
    protected void ClearForm()
    {
        txtEmail.Text = string.Empty;
        txtSubject.Text = string.Empty;
        txtSysInfo.Text = GetSystemInformation();

        htmlTemplateBody.ResolvedValue = string.Empty;
        chkSettings.Checked = true;
        radDontKnow.Checked = true;
        radAspx.Checked = radMix.Checked = radPortal.Checked = false;
    }


    /// <summary>
    /// Gets data from the global and current site Settings.
    /// </summary>
    private static string GetSettingsInfo()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("----------Settings---------");
        sb.AppendLine();
        sb.AppendLine();

        // Get global settings data
        sb.Append(GetSettingsString(0, null));

        int siteId = SiteContext.CurrentSiteID;
        if (siteId > 0)
        {
            // Get current site settings data
            sb.Append(GetSettingsString(siteId, SiteContext.CurrentSiteName));
        }

        return sb.ToString();
    }


    /// <summary>
    /// Returns string with Settings information for specified site.
    /// </summary>
    /// <param name="siteId">Site ID or 0 for global Settings</param>
    /// <param name="siteName">Site name or null for global Settings</param>
    private static string GetSettingsString(int siteId, string siteName)
    {
        var categories = SettingsCategoryInfoProvider.GetSettingsCategories()
            .OrderBy("CategoryName", "CategoryOrder")
            .Columns("CategoryName", "CategoryID");

        // Get global setting categories
        if (siteId > 0)
        {
            categories.Where("CategoryID IN (SELECT KeyCategoryID FROM CMS_SettingsKey WHERE SiteID = " + siteId + ")");
        }

        var sb = new StringBuilder();

        string site = siteId > 0 ? siteName : "GLOBAL SETTINGS";

        sb.Append(" - ");
        sb.Append(site);
        sb.AppendLine();

        // Loop through all setting categories
        foreach (DataRow catDr in categories.Tables[0].Rows)
        {
            // Get settings keys for specific category
            var categoryId = ValidationHelper.GetInteger(catDr["CategoryID"], 0);

            DataSet keys = SettingsKeyInfoProvider.GetSettingsKeys(categoryId, siteId)
                .Columns("KeyName", "KeyValue", "KeyType");

            if (!DataHelper.DataSourceIsEmpty(keys))
            {
                // Display only not empty categories
                sb.Append("\r\n\t - ");

                var categoryName = ValidationHelper.GetString(catDr["CategoryName"], "");
                sb.Append(ResHelper.LocalizeString(categoryName));

                sb.AppendLine();

                // Display keys for category
                foreach (DataRow keyDr in keys.Tables[0].Rows)
                {
                    var value = keyDr["KeyValue"];
                    if (value != DBNull.Value)
                    {
                        var name = ValidationHelper.GetString(keyDr["KeyName"], string.Empty);
                        var stringValue = ValidationHelper.GetString(value, string.Empty);
                        var type = ValidationHelper.GetString(keyDr["KeyType"], string.Empty);

                        sb.AppendFormat("\t\t - {0} '{1}' ({2})", name, stringValue, type);

                        sb.AppendLine();
                    }
                }
            }
        }

        sb.AppendLine();

        return sb.ToString();
    }


    /// <summary>
    /// Returns system information.
    /// </summary>
    private static string GetSystemInformation()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendFormat("CMS version: {0} Build: {1}",
                        CMSVersion.MainVersion,
                        CMSVersion.GetVersion(true, true,true, true));
        sb.AppendLine();

        sb.AppendFormat("OS version: {0}", Environment.OSVersion);
        sb.AppendLine();

        LicenseKeyInfo licenseKey = null;
        if (SiteContext.CurrentSite != null)
        {
            licenseKey = LicenseKeyInfoProvider.GetLicenseKeyInfo(SiteContext.CurrentSite.DomainName);
        }

        if (licenseKey != null)
        {
            sb.AppendFormat("License info: {0}, {1}, {2}, {3}",
                            licenseKey.Domain,
                            licenseKey.Edition,
                            licenseKey.ExpirationDateReal.ToString(DateTimeHelper.DefaultIFormatProvider),
                            licenseKey.Version);

            string packages = ValidationHelper.GetString(licenseKey.GetValue("LicensePackages"), string.Empty);
            if (!string.IsNullOrEmpty(packages))
            {
                sb.AppendFormat(", {0}", packages);
            }
        }

        int eventId = QueryHelper.GetInteger("eventid", 0);
        if (eventId > 0)
        {
            EventLogInfo ev = EventLogProvider.GetEventLogInfo(eventId);
            if (ev != null)
            {
                sb.AppendLine();
                sb.Append(HttpUtility.HtmlDecode(EventLogHelper.GetEventText(ev)));
            }
        }

        return sb.ToString();
    }


    private string GetEngineInfo()
    {
        if (radAspx.Checked)
        {
            return "ASPX Templates";
        }

        if (radPortal.Checked)
        {
            return "Portal engine";
        }

        if (radMix.Checked)
        {
            return "Both";
        }

        if (radDontKnow.Checked)
        {
            return "I don't know";
        }

        return string.Empty;
    }

    #endregion
}