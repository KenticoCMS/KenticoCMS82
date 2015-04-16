using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;

using CMS.EventLog;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.StrandsRecommender;
using CMS.UIControls;

using HtmlAgilityPack;


/// <summary>
/// Helper page used for asynchronous web method calls.
/// </summary>
public partial class CMSModules_StrandsRecommender_Pages_WebMethods : CMSPage
{
    #region "Public methods"

    /// <summary>
    /// Gets names, IDs and types of all web templates from the Strands API grouped by the template type (placement) 
    /// and ordered by the template number (a number at the end of the template name).
    /// </summary>
    /// <exception cref="Exception">StrandsException thrown in this method is always converted to Exception so it can be serialized and returned to the caller</exception>
    [WebMethod]
    public static Dictionary<string, IEnumerable<StrandsWebTemplateData>> LoadAllWebTemplates()
    {
        try
        {
            StrandsApiClient strandsApiClient = new StrandsApiClient(StrandsSettings.GetApiID(SiteContext.CurrentSiteName));

            var groups = from webTemplate in strandsApiClient.GetAvailableWebTemplates()
                         group webTemplate by webTemplate.Type
                         into webTemplateGroups
                         orderby webTemplateGroups.Key
                         select webTemplateGroups;

            return groups.ToDictionary(
                g => g.Key.ToStringRepresentation(),
                g => g.OrderBy(GetTemplateOrder).Select(EncodeWebTemplate));
        }
        catch (StrandsException ex)
        {
            EventLogProvider.LogException("Strands Recommender", "LOADALLWEBTEMPLATES", ex);

            // Since it is not possible to send exception with custom properties via AJAX when request failed,
            // throw regular exception and provide nice UI message as its base message
            throw new Exception(ex.UIMessage, ex);
        }
    }


    /// <summary>
    /// Loads one specific email template from Strands API.
    /// </summary>
    /// <param name="name">Identifier of desired email template</param>
    /// <returns>HTML content of email template</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is null</exception>
    [WebMethod]
    public static string LoadSpecificEmailTemplate(string name)
    {
        try
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            var provider = new StrandsApiClient(StrandsSettings.GetApiID(SiteContext.CurrentSiteName));
            string template = provider.GetEmailTemplate(name);

            template = AddTitleParameter(template);

            return template;
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Strands Recommender", "LOADSPECIFISEMAILTEMPLATE", ex);
            throw;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Adds title parameter to hyperlinks in Strands recommendation widget.
    /// Text from title is then automatically transfered to description in Link tracking window (newsletters link tracking dialog).
    /// </summary>
    /// <param name="input">HTML code with hyperlinks</param>
    /// <returns>Edited HTML code with title parameters in hyperlinks</returns>
    private static string AddTitleParameter(string input)
    {
        var document = new HtmlDocument();
        document.LoadHtml(input);

        var anchors = document.DocumentNode.SelectNodes("//a");
        if (anchors == null)
        {
            return input;
        }

        foreach (var anchor in anchors)
        {
            var hrefParameter = anchor.Attributes["href"];
            if (hrefParameter != null)
            {
                string indexValue = HttpUtility.ParseQueryString(hrefParameter.Value).Get("index");

                if (!string.IsNullOrEmpty(indexValue))
                {
                    anchor.Attributes.Add("title", String.Format(ResHelper.GetString("strands.newsletterlinkdescriptiontext"), indexValue));
                }
            }
        }

        return document.DocumentNode.OuterHtml;
    }


    /// <summary>
    /// Gets integer value from the right side of the ID of the provided Strands template.
    /// </summary>
    /// <param name="webTemplate">Strands web template. Its ID will be parsed to get order</param>
    /// <example>For template ID PROD-4 returns 4, for template ID home_8 returns 8 etc.</example>
    /// <returns>Integer value of template ID</returns>
    private static int GetTemplateOrder(StrandsWebTemplateData webTemplate)
    {
        var splittedText = Regex.Split(webTemplate.ID, "[-_]");
        if (splittedText.Length == 2)
        {
            return ValidationHelper.GetInteger(splittedText[1], 0);
        }

        return 0;
    }


    /// <summary>
    /// HTML encodes templates ID and Title fields, so it can be safely rendered.
    /// </summary>
    /// <param name="template">Template whose fields will be encoded.</param>
    /// <returns>The same instance as was passed in <paramref name="template"/>, but with encoded properties</returns>
    private static StrandsWebTemplateData EncodeWebTemplate(StrandsWebTemplateData template)
    {
        template.ID = HTMLHelper.HTMLEncode(template.ID);
        template.Title = HTMLHelper.HTMLEncode(template.Title);

        return template;
    }

    #endregion
}