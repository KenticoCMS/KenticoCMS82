using System;
using System.Data;

using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.Newsletters;
using CMS.DataEngine;

// Set edited object
[EditedObject("newsletter.emailtemplate", "objectid")]
[Security(Resource = "CMS.Newsletter", UIElements = "Templates.Newsletters")]
[Security(Resource = "CMS.Newsletter", UIElements = "TemplateProperties")]
[UIElement("CMS.Newsletter", "Templates.Newsletters")]
public partial class CMSModules_Newsletters_Tools_Templates_Tab_Newsletters : CMSNewsletterPage
{
    #region "Variables"

    private EmailTemplateInfo emailTemplateInfo;
    private DataSet templateNewsletters;
    private string currentValues;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        emailTemplateInfo = EditedObject as EmailTemplateInfo;

        if (emailTemplateInfo == null)
        {
            pnlAvailability.Visible = false;
            return;
        }

        // Initialize newsletter selector
        var where = new WhereCondition()
            .WhereEquals("NewsletterType", NewsletterType.TemplateBased)
            .WhereEquals("NewsletterSiteID", SiteContext.CurrentSiteID)
            .WhereNotEquals("NewsletterTemplateID", emailTemplateInfo.TemplateID);
        usNewsletters.WhereCondition = where.ToString(expand: true);

        if (!RequestHelper.IsPostBack())
        {
            LoadSiteBindings();
        }

        usNewsletters.OnSelectionChanged += usSites_OnSelectionChanged;
    }

    #endregion


    #region "Control event handlers"

    /// <summary>
    /// Uniselector event handler.
    /// </summary>
    protected void usSites_OnSelectionChanged(object sender, EventArgs e)
    {
        SaveSiteBindings();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Load control.
    /// </summary>
    private void LoadSiteBindings()
    {
        GetCurrentNewsletters();
        usNewsletters.Value = currentValues;
        usNewsletters.Reload(true);
    }


    /// <summary>
    /// Loads current newsletters from DB.
    /// </summary>
    private void GetCurrentNewsletters()
    {
        templateNewsletters = EmailTemplateNewsletterInfoProvider
                                    .GetEmailTemplateNewsletters()
                                    .WhereEquals("TemplateID", emailTemplateInfo.TemplateID)
                                    .Column("NewsletterID");

        if (!DataHelper.DataSourceIsEmpty(templateNewsletters))
        {
            currentValues = TextHelper.Join(";", DataHelper.GetStringValues(templateNewsletters.Tables[0], "NewsletterID"));
        }
        else
        {
            currentValues = string.Empty;
        }
    }


    /// <summary>
    /// Save changes.
    /// </summary>
    private void SaveSiteBindings()
    {
        // Check 'Manage templates' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.newsletter", "managetemplates"))
        {
            RedirectToAccessDenied("cms.newsletter", "managetemplates");
        }

        if (RequestHelper.IsPostBack())
        {
            GetCurrentNewsletters();
        }

        string newValues = ValidationHelper.GetString(usNewsletters.Value, null);
        RemoveOldRecords(newValues, currentValues);
        AddNewRecords(newValues, currentValues);
        currentValues = newValues;
    }


    /// <summary>
    /// Remove newsletters from template.
    /// </summary>
    private void RemoveOldRecords(string newValues, string currentNewsletters)
    {
        string items = DataHelper.GetNewItemsInList(newValues, currentNewsletters);
        if (!String.IsNullOrEmpty(items))
        {
            var modifiedItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in modifiedItems)
            {
                EmailTemplateNewsletterInfoProvider.RemoveNewsletterFromTemplate(emailTemplateInfo.TemplateID, ValidationHelper.GetInteger(item, 0));
            }
        }
    }


    /// <summary>
    /// Add newsletters to template.
    /// </summary>
    private void AddNewRecords(string newValues, string currentNewsletters)
    {
        string items = DataHelper.GetNewItemsInList(currentNewsletters, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            var modifiedItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in modifiedItems)
            {
                EmailTemplateNewsletterInfoProvider.AddNewsletterToTemplate(emailTemplateInfo.TemplateID, ValidationHelper.GetInteger(item, 0));
            }
        }
    }

    #endregion
}