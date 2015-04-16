using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_Membership_Pages_Users_General_User_Reject : CMSUsersPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Pagetitle
        Title = GetString("administration.users.rejectusers");

        // Set the master page header
        PageTitle.TitleText = GetString("administration.users.rejectusers");
        // Initialize other properties        
        txtReason.Text = GetString("administration.user.reasondefault");
        btnCancel.Attributes.Add("onclick", "return CloseDialog();");

        // Register scripts
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "CloseAndRefresh",
                                               ScriptHelper.GetScript(
                                                   "function CloseAndRefresh()\n" +
                                                   "{\n" +
                                                   "var txtReason = document.getElementById('" + txtReason.ClientID + "').value;\n" +
                                                   "var chkSendEmail = document.getElementById('" + chkSendEmail.ClientID + "').checked;\n" +
                                                   "wopener.SetRejectParam(txtReason, chkSendEmail, 'true');\n" +
                                                   "CloseDialog();\n" +
                                                   "}\n"));

        txtReason.Focus();

        btnReject.OnClientClick = "CloseAndRefresh(); return false;";

        // Register modal page scripts
        RegisterEscScript();
        RegisterModalPageScripts();
    }
}