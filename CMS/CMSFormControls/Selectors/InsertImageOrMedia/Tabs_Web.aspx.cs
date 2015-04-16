using System;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSFormControls_Selectors_InsertImageOrMedia_Tabs_Web : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check UIProfile
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.WYSIWYGEditor", "InsertImageOrMedia"))
        {
            RedirectToUIElementAccessDenied("CMS.WYSIWYGEditor", "InsertImageOrMedia");
        }
        else if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.MediaDialog", "WebTab"))
        {
            RedirectToUIElementAccessDenied("CMS.MediaDialog", "WebTab");
        }

		// CKEditor's plugin filebrowser add custom params to url. 
		// This ensures that custom params aren't validated
		if (QueryHelper.ValidateHash("hash", "CKEditor;CKEditorFuncNum;langCode", validateWithoutExcludedParameters: true))
        {
            ScriptHelper.RegisterJQuery(Page);
            CMSDialogHelper.RegisterDialogHelper(Page);
        }
        else
        {
            webContentSelector.StopProcessing = true;
            webContentSelector.Visible = false;
            string url = ResolveUrl("~/CMSMessages/Error.aspx?title=" + GetString("dialogs.badhashtitle") + "&text=" + GetString("dialogs.badhashtext") + "&cancel=1");
            ltlScript.Text = ScriptHelper.GetScript("if (window.parent != null) { window.parent.location = '" + url + "' }");
        }
    }
}