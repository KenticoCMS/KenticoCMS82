using System;
using System.Linq;

using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.Base;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

[Title("newsletters.templates")]
[Action(0, "NewsletterTemplate_List.NewItemCaption", "NewsletterTemplate_New.aspx")]
[UIElement(ModuleName.NEWSLETTER, "Templates")]
public partial class CMSModules_Newsletters_Tools_Templates_NewsletterTemplate_List : CMSNewsletterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        UniGrid.OnAction += uniGrid_OnAction;
        UniGrid.WhereCondition = "(TemplateSiteID = " + SiteContext.CurrentSiteID + ")";
        UniGrid.OnExternalDataBound += UniGrid_OnExternalDataBound;
    }


    protected object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            // Initialize template type column
            case "templatetype":
                switch (parameter.ToString().ToLowerCSafe())
                {
                    case "u":
                        return GetString("NewsletterTemplate_List.Unsubscription");

                    case "s":
                        return GetString("NewsletterTemplate_List.Subscription");

                    case "d":
                        return GetString("NewsletterTemplate_List.OptIn");

                    default:
                        return GetString("NewsletterTemplate_List.Issue");
                }
        }
        return parameter;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        string templateId = actionArgument.ToString();

        switch (actionName.ToLowerCSafe())
        {
            // Edit the template
            case "edit":

                string url = UIContextHelper.GetElementUrl("cms.newsletter", "TemplateProperties", false, templateId.ToInteger(0));
                URLHelper.Redirect(url);

                break;

            // Delete the template
            case "delete":
                // Check 'Manage templates' permission
                var template = EmailTemplateInfoProvider.GetEmailTemplateInfo(ValidationHelper.GetInteger(actionArgument, 0));
                if (template == null)
                {
                    RedirectToAccessDenied(GetString("general.invalidparameters"));
                }

                if (!template.CheckPermissions(PermissionsEnum.Delete, CurrentSiteName, CurrentUser))
                {
                    RedirectToAccessDenied("cms.newsletter", "managetemplates");
                }
                
                // Check if the template is used in a newsletter
                var newsByEmailtempl = NewsletterInfoProvider
                                            .GetNewsletters()
                                            .WhereEquals("NewsletterTemplateID", templateId)
                                            .Or()
                                            .WhereEquals("NewsletterSubscriptionTemplateID", templateId)
                                            .Or()
                                            .WhereEquals("NewsletterUnsubscriptionTemplateID", templateId)
                                            .Or()
                                            .WhereEquals("NewsletterOptInTemplateID", templateId)
                                            .Column("NewsletterID")
                                            .TopN(1);

                if (!newsByEmailtempl.Any())
                {
                    // Check if the template is used in an issue
                    var newsletterIssuesIDs = IssueInfoProvider.GetIssues().WhereEquals("IssueTemplateID", templateId).TopN(1).Column("IssueID");
                    if (!newsletterIssuesIDs.Any())
                    {
                        // Delete EmailTemplate object from database
                        EmailTemplateInfoProvider.DeleteEmailTemplateInfo(ValidationHelper.GetInteger(templateId, 0));
                    }
                    else
                    {
                        ShowError(GetString("NewsletterTemplate_List.TemplateInUseByNewsletterIssue"));
                    }
                }
                else
                {
                    ShowError(GetString("NewsletterTemplate_List.TemplateInUseByNewsletter"));
                }
                break;
        }
    }
}