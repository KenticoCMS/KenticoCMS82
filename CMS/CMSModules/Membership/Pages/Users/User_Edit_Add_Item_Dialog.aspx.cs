using System;
using System.Text;
using System.Collections;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSModules_Membership_Pages_Users_User_Edit_Add_Item_Dialog : CMSModalPage
{
    #region "Variables"

    /// <summary>
    /// Indicates if send notification field is used.
    /// </summary>
    private bool useSendNotification;

    #endregion


    protected override void OnPreInit(EventArgs e)
    {
        String identifier = QueryHelper.GetString("params", null);
        Hashtable parameters = (Hashtable)WindowHelper.GetItem(identifier);

        useSendNotification = (QueryHelper.GetInteger("UseSendNotification", 0) == 1);

        if (parameters != null)
        {
            // Take only first column
            parameters["AdditionalColumns"] = null;
        }

        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Users", "Read"))
        {
            RedirectToAccessDenied("CMS.Users", "Read");
        }

        ScriptHelper.RegisterWOpenerScript(Page);
        ScriptHelper.RegisterJQuery(Page);


        // Try to get parameters...
        string identifier = QueryHelper.GetString("params", null);
        Hashtable parameters = (Hashtable)WindowHelper.GetItem(identifier);

        // ... and validate hash
        if ((QueryHelper.ValidateHash("hash", "selectedvalue")) && (parameters != null))
        {
            // Take only first column
            parameters["AdditionalColumns"] = null;

            selectionDialog.LocalizeItems = QueryHelper.GetBoolean("localize", true);

            // Load resource prefix
            string resourcePrefix = ValidationHelper.GetString(parameters["ResourcePrefix"], "general");

            // Set the page title
            string titleText = GetString(resourcePrefix + ".selectitem|general.selectitem");

            // Validity group text
            pnlDateTimeHeading.ResourceString = resourcePrefix + ".bindingproperties";

            PageTitle.TitleText = titleText;
            Page.Title = titleText;

            // Remove additional dialog padding
            CurrentMaster.PanelContent.RemoveCssClass("dialog-content");
        }
        else
        {
            // Redirect to error page
            URLHelper.Redirect(ResolveUrl("~/CMSMessages/Error.aspx?title=" + ResHelper.GetString("dialogs.badhashtitle") + "&text=" + ResHelper.GetString("dialogs.badhashtext")));
        }

        ((ICMSModalMasterPage)CurrentMaster).ShowSaveAndCloseButton();
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Show/hide send notification field
        pnlSendNotification.Visible = useSendNotification;

        base.OnPreRender(e);
    }


    protected override void OnPreRenderComplete(EventArgs e)
    {
        base.OnPreRenderComplete(e);

        String okScript;

        if (selectionDialog.UniGrid.IsEmpty)
        {
            pnlDateTime.Visible = false;
            okScript = "return US_Cancel()";
        }
        else
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("function okScript() {");
            sb.AppendFormat("    if ((typeof(isDateTimeValid) != 'undefined') && (!isDateTimeValid('{0}'))) {{", ucDateTime.DateTimeTextBox.ClientID);
            sb.NewLine();
            sb.AppendFormat("        var lblErr = $cmsj('#{0}');", lblError.ClientID);
            sb.NewLine();
            sb.AppendFormat("        lblErr.text ('{0}');", GetString("basicform.errorinvaliddatetimerange"));
            sb.NewLine();
            sb.AppendLine("        lblErr.show();");
            sb.AppendLine("    } else {");
            sb.AppendFormat("        var date = $cmsj('#{0}').val();", ucDateTime.DateTimeTextBox.ClientID);
            sb.NewLine();
            sb.AppendFormat("        var sendNotification = ($cmsj('#{0}').attr('checked') == 'checked') ? true : false;", chkSendNotification.ClientID);
            sb.NewLine();
            sb.AppendLine("        if (wopener.setNewDateTime != null) { wopener.setNewDateTime(date); }");
            sb.AppendLine("        if (wopener.setNewSendNotification != null) { wopener.setNewSendNotification(sendNotification); }");
            sb.AppendLine("        US_Submit();");
            sb.AppendLine("    }");
            sb.AppendLine("    return false;");
            sb.AppendLine("}");

            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "ok", ScriptHelper.GetScript(sb.ToString()));

            okScript = "return okScript();";
        }

        SetSaveJavascript(okScript);
    }
}