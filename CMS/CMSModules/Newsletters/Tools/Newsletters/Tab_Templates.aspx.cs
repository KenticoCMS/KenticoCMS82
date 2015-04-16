using System;
using System.Data;

using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;
using CMS.Newsletters;

[UIElement(ModuleName.NEWSLETTER, "Newsletter.Templates")]
[EditedObject(NewsletterInfo.OBJECT_TYPE, "objectid")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Tab_Templates : CMSNewsletterPage
{
    #region "Variables"

    private NewsletterInfo mNewsletter;
    private DataSet mTemplateNewsletters;
    private string mCurrentValues;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        mNewsletter = EditedObject as NewsletterInfo;
        if (mNewsletter == null)
        {
            pnlAvailability.Visible = false;
            return;
        }
        
        if (!mNewsletter.CheckPermissions(PermissionsEnum.Read, CurrentSiteName, CurrentUser))
        {
            RedirectToAccessDenied(mNewsletter.TypeInfo.ModuleName, "ManageTemplates");
        }

        if (!RequestHelper.IsPostBack())
        {
            LoadBindings();
        }

        // Show all issue templates from current site
        var where = new WhereCondition()
            .WhereEquals("TemplateType", EmailTemplateType.Issue)
            .WhereEquals("TemplateSiteID", mNewsletter.NewsletterSiteID)
            .WhereNotEquals("TemplateID", mNewsletter.NewsletterTemplateID);
        usTemplates.WhereCondition = where.ToString(expand: true);
        usTemplates.OnSelectionChanged += usTemplates_OnSelectionChanged;
    }

    #endregion


    #region "Control event handlers"

    /// <summary>
    /// Uniselector event handler.
    /// </summary>
    protected void usTemplates_OnSelectionChanged(object sender, EventArgs e)
    {
        SaveBindings();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Load control.
    /// </summary>
    private void LoadBindings()
    {
        GetCurrentNewsletters();
        usTemplates.Value = mCurrentValues;
        usTemplates.Reload(true);
    }


    /// <summary>
    /// Loads current templates from DB.
    /// </summary>
    private void GetCurrentNewsletters()
    {
        mTemplateNewsletters = EmailTemplateNewsletterInfoProvider
                                    .GetEmailTemplateNewsletters()
                                    .WhereEquals("NewsletterID", mNewsletter.NewsletterID)
                                    .Column("TemplateID");

        if (!DataHelper.DataSourceIsEmpty(mTemplateNewsletters))
        {
            mCurrentValues = TextHelper.Join(";", DataHelper.GetStringValues(mTemplateNewsletters.Tables[0], "TemplateID"));
        }
        else
        {
            mCurrentValues = String.Empty;
        }
    }


    /// <summary>
    /// Save changes.
    /// </summary>
    private void SaveBindings()
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

        string newValues = ValidationHelper.GetString(usTemplates.Value, null);
        RemoveOldRecords(newValues, mCurrentValues);
        AddNewRecords(newValues, mCurrentValues);
        mCurrentValues = newValues;
    }


    /// <summary>
    /// Remove templates from newsletter.
    /// </summary>
    private void RemoveOldRecords(string newValues, string currentRecords)
    {
        string items = DataHelper.GetNewItemsInList(newValues, currentRecords);
        if (!String.IsNullOrEmpty(items))
        {
            var modifiedItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in modifiedItems)
            {
                EmailTemplateNewsletterInfoProvider.RemoveNewsletterFromTemplate(ValidationHelper.GetInteger(item, 0), mNewsletter.NewsletterID);
            }
        }
    }


    /// <summary>
    /// Add templates to newsletter.
    /// </summary>
    private void AddNewRecords(string newValues, string currentRecords)
    {
        string items = DataHelper.GetNewItemsInList(currentRecords, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            var modifiedItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in modifiedItems)
            {
                EmailTemplateNewsletterInfoProvider.AddNewsletterToTemplate(ValidationHelper.GetInteger(item, 0), mNewsletter.NewsletterID);
            }
        }
    }

    #endregion
}