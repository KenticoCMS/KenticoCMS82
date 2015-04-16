using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;
using CMS.Base;

public partial class CMSFormControls_LiveSelectors_InsertImageOrMedia_Tabs_Email : CMSLiveModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        bool checkUI = ValidationHelper.GetBoolean(SettingsHelper.AppSettings["CKEditor:PersonalizeToolbarOnLiveSite"], false);
        if (checkUI)
        {
            // Check UIProfile
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.WYSIWYGEditor", "InsertLink"))
            {
                RedirectToUIElementAccessDenied("CMS.WYSIWYGEditor", "InsertLink");
            }
            else if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.MediaDialog", "EmailTab"))
            {
                RedirectToUIElementAccessDenied("CMS.MediaDialog", "EmailTab");
            }
        }

        if (QueryHelper.ValidateHash("hash"))
        {
            ScriptHelper.RegisterJQuery(Page);
            CMSDialogHelper.RegisterDialogHelper(Page);
        }
        else
        {
            emailProperties.StopProcessing = true;
            emailProperties.Visible = false;
            string url = ResolveUrl("~/CMSMessages/Error.aspx?title=" + GetString("dialogs.badhashtitle") + "&text=" + GetString("dialogs.badhashtext") + "&cancel=1");
            ltlScript.Text = ScriptHelper.GetScript("if (window.parent != null) { window.parent.location = '" + url + "' }");
        }
    }
}