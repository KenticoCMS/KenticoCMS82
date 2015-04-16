using System;
using System.Collections;

using CMS.Core;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.UIControls;

public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Add_Account_Dialog : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Ensure, that it is going to be rendered
        pnlRole.Visible = true;

        ScriptHelper.RegisterWOpenerScript(Page);
        ScriptHelper.RegisterJQuery(Page);

        // Try to get parameters
        string identifier = QueryHelper.GetString("params", null);
        Hashtable parameters = (Hashtable)WindowHelper.GetItem(identifier);

        // Validate hash
        if ((QueryHelper.ValidateHash("hash", "selectedvalue")) && (parameters != null))
        {
            int siteid = ValidationHelper.GetInteger(parameters["SiteID"], -1);
            if (siteid != -1)
            {
                AccountHelper.AuthorizedReadAccount(siteid, true);
                if (AccountHelper.AuthorizedModifyAccount(siteid, false) || ContactHelper.AuthorizedModifyContact(siteid, false))
                {
                    contactRoleSelector.SiteID = siteid;
                    contactRoleSelector.IsLiveSite = ValidationHelper.GetBoolean(parameters["IsLiveSite"], false);
                    contactRoleSelector.UniSelector.DialogWindowName = "SelectContactRole";
                    contactRoleSelector.IsSiteManager = ValidationHelper.GetBoolean(parameters["IsSiteManager"], false);

                    selectionDialog.LocalizeItems = QueryHelper.GetBoolean("localize", true);

                    // Load resource prefix
                    string resourcePrefix = ValidationHelper.GetString(parameters["ResourcePrefix"], "general");

                    // Set the page title
                    string titleText = GetString(resourcePrefix + ".selectitem|general.selectitem");

                    lblAddAccounts.ResourceString = resourcePrefix + ".contactsrole";
                    pnlRoleHeading.Visible = true;

                    PageTitle.TitleText = titleText;
                    Page.Title = titleText;
                }
                // No permission modify
                else
                {
                    RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "ModifyAccount");
                }
            }
            else
            {
                // Redirect to error page
                URLHelper.Redirect(ResolveUrl("~/CMSMessages/Error.aspx?title=" + ResHelper.GetString("dialogs.badhashtitle") + "&text=" + ResHelper.GetString("dialogs.badhashtext")));
            }
        }

        CurrentMaster.PanelContent.RemoveCssClass("dialog-content");
    }


    protected override void OnPreRender(EventArgs e)
    {
        SetSaveJavascript("var role = $cmsj('#" + contactRoleSelector.DropDownList.ClientID + @"').val();
if (wopener.setRole) wopener.setRole(role);
return US_Submit();");

        base.OnPreRender(e);
    }


    protected override void OnPreRenderComplete(EventArgs e)
    {
        pnlRole.Visible = !selectionDialog.UniGrid.IsEmpty;

        base.OnPreRenderComplete(e);
    }
}