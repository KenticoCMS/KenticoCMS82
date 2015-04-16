using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

using CMS.DataEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.StrandsRecommender;
using CMS.UIControls;

using MessageTypeEnum = CMS.ExtendedControls.MessageTypeEnum;


[Title("strands.insertemailrecommendation.title")]
[CheckLicence(FeatureEnum.ContactManagement)] // Email recommendations needs Contact Management in order to work, so inserting recommendation to the email is not permitted if Contact Management is not available
public partial class CMSModules_StrandsRecommender_Pages_InsertEmailWidget : CMSModalPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Register custom style sheet file
        CSSHelper.RegisterCSSLink(Page, "~/CMSModules/StrandsRecommender/StyleSheets/InsertEmailWidget.css");

        List<string> emailTemplates = null;
        
        // If Strands is disabled (no API key present in settings), show common warning message
        if (StrandsSettings.IsStrandsEnabled(SiteContext.CurrentSiteName))
        {
            emailTemplates = LoadEmailTemplateNames();

            // If there is no template available, show warning message
            if ((emailTemplates != null) && !emailTemplates.Any()) 
            {
                AddWarning(GetString("strands.notemplates"));
            }
        }
        else
        {
            AddWarning(GetString("strands.notoken"));
        }

        RegisterScripts(emailTemplates);
    }


    /// <summary>
    /// Loads names of all available email templates from Strands API.
    /// </summary>
    /// <returns>Collection containing names of all available email templates</returns>
    private List<string> LoadEmailTemplateNames()
    {
        var provider = new StrandsApiClient(StrandsSettings.GetApiID(SiteContext.CurrentSiteName));

        try
        {
            // Return collection if everything is alright
            return provider.GetAvailableEmailTemplates().Select(HTMLHelper.HTMLEncode).ToList();
        }
        catch (StrandsException ex)
        {
            ProcessErrorMessage(ex, ex.UIMessage ?? ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            // Usually an exception thrown when there is a problem when communicating with Strands
            ProcessErrorMessage(ex, GetString("strands.exception"));
            return null;
        }
    }


    /// <summary>
    /// Logs exception and shows error message specified by given message key.
    /// </summary>
    /// <param name="exception">Exception to be logged</param>
    /// <param name="errorMessage">Error message</param>
    private void ProcessErrorMessage(Exception exception, string errorMessage)
    {
        EventLogProvider.LogException("Strands Recommender", "INSERTEMAILRECOMMENDATION", exception);
        AddError(errorMessage);
    }


    /// <summary>
    /// Registers all necessary scripts.
    /// </summary>
    /// <param name="emailTemplateNames">Names of the loaded email templates. null if Strands is disabled</param>
    private void RegisterScripts(IEnumerable<string> emailTemplateNames)
    {
        // Ensure jQuery is loaded
        ScriptHelper.RegisterJQuery(Page);

        // Ensure application path is available in global namespace
        ScriptHelper.RegisterApplicationConstants(Page);

        // File with all common Strands module methods 
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/StrandsRecommender/Scripts/StrandsModule.js");

        // File with all JavaScript logic related with this widget
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/StrandsRecommender/Scripts/InsertEmailWidget.js");

        string loadingGifUrl = GetImageUrl("CMSModules/CMS_StrandsRecommender/loading.gif");

        string serializedEmailTemplateNames = new JavaScriptSerializer().Serialize(emailTemplateNames);

        // Call init method
        ScriptHelper.RegisterStartupScript(this, typeof (string), "STRANDS.initInsertEmailWidget", ScriptHelper.GetScript(string.Format("STRANDS.initInsertEmailWidget({0}, {1});", serializedEmailTemplateNames, ScriptHelper.GetString(loadingGifUrl))));
    }


    /// <summary>
    /// Adds text to existing message on the page. If type of message is Error or Warning, hides info message telling user to select template, because there are no templates.
    /// </summary>
    /// <param name="type">Message type</param>
    /// <param name="text">Information message</param>
    /// <param name="separator">Separator</param>
    public override void AddMessage(MessageTypeEnum type, string text, string separator = null)
    {
        base.AddMessage(type, text, separator);
        if ((type == MessageTypeEnum.Warning) || (type == MessageTypeEnum.Error))
        {
            litSelectWidget.Visible = false;
        }
    }

    #endregion
}