using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

using CMS.Core;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSModules_ContactManagement_Pages_Tools_Account_Add_Contact_Dialog : CMSModalPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        RequireSite = false;
    }


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
            int siteID = ValidationHelper.GetInteger(parameters["SiteID"], -1);
            if (siteID != -1)
            {
                // Check permissions
                ContactHelper.AuthorizedReadContact(siteID, true);
                if (AccountHelper.AuthorizedModifyAccount(siteID, false) || ContactHelper.AuthorizedModifyContact(siteID, false))
                {
                    contactRoleSelector.SiteID = siteID;
                    contactRoleSelector.IsLiveSite = false;
                    contactRoleSelector.UniSelector.DialogWindowName = "SelectContactRole";
                    contactRoleSelector.IsSiteManager = ValidationHelper.GetBoolean(parameters["IsSiteManager"], false);

                    selectionDialog.LocalizeItems = QueryHelper.GetBoolean("localize", true);

                    // Load resource prefix
                    string resourcePrefix = ValidationHelper.GetString(parameters["ResourcePrefix"], "general");

                    // Set the page title
                    string titleText = GetString(resourcePrefix + ".selectitem|general.selectitem");

                    // Validity group text
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
        SetSaveJavascript(@"var role = $cmsj('#" + contactRoleSelector.DropDownList.ClientID + @"').val();
    if (wopener.setRole) wopener.setRole(role);
    return US_Submit();
");
        base.OnPreRender(e);
    }


    protected override void OnPreRenderComplete(EventArgs e)
    {
        pnlRole.Visible = !selectionDialog.UniGrid.IsEmpty;

        base.OnPreRenderComplete(e);
    }
}