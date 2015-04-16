using System;

using CMS.Core;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

[Breadcrumbs]
[Breadcrumb(0, ResourceString = "NewsletterTemplate_Edit.ItemListLink", TargetUrl = "~/CMSModules/Newsletters/Tools/Templates/NewsletterTemplate_List.aspx")]
[Breadcrumb(1, ResourceString = "NewsletterTemplate_Edit.NewItemCaption")]
[EditedObject(EmailTemplateInfo.OBJECT_TYPE, "objectid")]
[Title("newsletters.templates")]
[UIElement(ModuleName.NEWSLETTER, "Templates")]
public partial class CMSModules_Newsletters_Tools_Templates_NewsletterTemplate_New : CMSNewsletterPage
{
    protected EmailTemplateInfo TypedEditedObject 
    {
        get
        {
            return EditedObject as EmailTemplateInfo;
        }
    }


    #region "Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CurrentUser.IsAuthorizedPerResource(ModuleName.NEWSLETTER, "ManageTemplates"))
        {
            RedirectToAccessDenied(ModuleName.NEWSLETTER, "ManageTemplates");
        }
    }


    protected void OnBeforeSave(object sender, EventArgs e)
    {
        NewForm.Data.SetValue("TemplateBody", string.Empty);
        NewForm.Data.SetValue("TemplateHeader", "<html>\n<head>\n</head>\n<body>");
        NewForm.Data.SetValue("TemplateFooter", "</body>\n</html>");
        NewForm.Data.SetValue("TemplateSiteID", SiteContext.CurrentSiteID);
    }


    protected void OnAfterSave(object sender, EventArgs e)
    {
        if (TypedEditedObject != null)
        {
            string url = UIContextHelper.GetElementUrl("cms.newsletter", "TemplateProperties", false, TypedEditedObject.TemplateID);
            url = URLHelper.AddParameterToUrl(url, "saved", "1");
            
            URLHelper.Redirect(url);
        }
    }

    #endregion
}