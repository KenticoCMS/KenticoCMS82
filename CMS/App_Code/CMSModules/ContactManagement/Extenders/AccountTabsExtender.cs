using System;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.FormEngine;
using CMS.OnlineMarketing;
using CMS.UIControls;

[assembly: RegisterCustomClass("AccountTabsExtender", typeof(AccountTabsExtender))]

/// <summary>
/// Extender for account detail tabs UIElement.
/// </summary>
public class AccountTabsExtender : UITabsExtender
{
    private AccountInfo Account
    {
        get
        {
            return Control.UIContext.EditedObject as AccountInfo;
        }
    }


    public override void OnInit()
    {
        base.OnInit();
        if (Account == null)
        {
            return;
        }

        Control.Page.Load += Page_Load;
    }


    /// <summary>
    /// Initialization of tabs.
    /// </summary>
    public override void OnInitTabs()
    {
        if (Account == null)
        {
            return;
        }

        Control.OnTabCreated += OnTabCreated;
    }


    private void Page_Load(object sender, EventArgs e)
    {
        // Check permission read
        AccountHelper.AuthorizedReadAccount(Account.AccountSiteID, true);
    }


    private void OnTabCreated(object sender, TabCreatedEventArgs e)
    {
        if (e.Tab == null)
        {
            return;
        }

        switch (e.Tab.TabName.ToLowerCSafe())
        {
            case "account.customfields":
                // Check if contact has any custom fields
                var formInfo = FormHelper.GetFormInfo("OM.Account", false);
                if (!formInfo.GetFields(true, false, false).Any())
                {
                    e.Tab = null;
                }
                break;

            case "account.contacts":
                // Display contacts tab only if user is authorized to read contacts
                if (!ContactHelper.AuthorizedReadContact(Account.AccountSiteID, false) && !AccountHelper.AuthorizedReadAccount(Account.AccountSiteID, false))
                {
                    e.Tab = null;
                }
                break;

            case "account.merge":
            case "account.subsidiaries":
                if (Account.AccountMergedWithAccountID != 0)
                {
                    e.Tab = null;
                }
                break;
        }
    }
}
