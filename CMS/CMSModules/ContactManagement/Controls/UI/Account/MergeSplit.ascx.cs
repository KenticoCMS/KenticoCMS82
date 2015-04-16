using System;
using System.Text;
using System.Web.UI;
using System.Collections;
using System.Linq;

using CMS.Core;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.DataEngine;

public partial class CMSModules_ContactManagement_Controls_UI_Account_MergeSplit : CMSAdminListControl, ICallbackEventHandler
{
    #region "Constant"
    
    private CMSModules_ContactManagement_Controls_UI_Account_Filter filter;

    #endregion


    #region "Variables"

    private AccountInfo ai;
    private Hashtable mParameters;

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            gridElem.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            gridElem.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Current account.
    /// </summary>
    public AccountInfo Account
    {
        get
        {
            if (ai == null)
            {
                if (UIContext.EditedObject != null)
                {
                    ai = (AccountInfo)UIContext.EditedObject;
                }
            }
            return ai;
        }
    }


    /// <summary>
    /// True if "select all children" should be visible.
    /// </summary>
    public bool ShowChildrenOption
    {
        get;
        set;
    }


    /// <summary>
    /// Dialog control identifier.
    /// </summary>
    private string Identifier
    {
        get
        {
            string identifier = hdnValue.Value;
            if (string.IsNullOrEmpty(identifier))
            {
                identifier = Guid.NewGuid().ToString();
                hdnValue.Value = identifier;
            }

            return identifier;
        }
    }


    /// <summary>
    /// Gets or sets the callback argument.
    /// </summary>
    private string CallbackArgument
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        gridElem.OnFilterFieldCreated += gridElem_OnFilterFieldCreated;
        gridElem.LoadGridDefinition();
        base.OnInit(e);
    }


    void gridElem_OnFilterFieldCreated(string columnName, UniGridFilterField filterDefinition)
    {
        filter = filterDefinition.ValueControl as CMSModules_ContactManagement_Controls_UI_Account_Filter;
        if (filter != null)
        {
            filter.HideMergedFilter = true;
            filter.IsLiveSite = IsLiveSite;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        bool isGlobal = Account.AccountSiteID == 0;
        // Current contact is global object
        if (isGlobal)
        {
            filter.SiteID = SiteContext.CurrentSiteID;
            // Display site selector in site manager
            if (ContactHelper.IsSiteManager)
            {
                filter.SiteID = UniSelector.US_GLOBAL_RECORD;
                filter.DisplaySiteSelector = true;
            }
            // Display 'site or global' selector in CMS desk for global objects
            else if (AccountHelper.AuthorizedModifyAccount(SiteContext.CurrentSiteID, false) && AccountHelper.AuthorizedReadAccount(SiteContext.CurrentSiteID, false))
            {
                filter.DisplayGlobalOrSiteSelector = true;
            }
        }
        else
        {
            filter.SiteID = Account.AccountSiteID;
        }
        filter.ShowGlobalStatuses =
                ConfigurationHelper.AuthorizedReadConfiguration(UniSelector.US_GLOBAL_RECORD, false) &&
                (SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSCMGlobalConfiguration") || ContactHelper.IsSiteManager);

        filter.ShowChildren = ShowChildrenOption;

        string where = filter.WhereCondition;

        if (!filter.ChildrenSelected)
        {
            // Display only direct children ("first level")
            where = SqlHelper.AddWhereCondition(where, "AccountGlobalAccountID = " + Account.AccountID + " OR AccountMergedWithAccountID = " + Account.AccountID);
        }
        else if (isGlobal)
        {
            // Get children for global contact
            where = SqlHelper.AddWhereCondition(where, "AccountID IN (SELECT * FROM Func_OM_Account_GetChildren_Global(" + Account.AccountID + ", 0))");
        }
        else
        {
            // Get children for site contact
            where = SqlHelper.AddWhereCondition(where, "AccountID IN (SELECT * FROM Func_OM_Account_GetChildren(" + Account.AccountID + ", 0))");
        }
        gridElem.WhereCondition = where;
        gridElem.ZeroRowsText = GetString("om.account.noaccountsfound");
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        btnSplit.Click += btnSplit_Click;

        // Register JS scripts
        RegisterScripts();
    }


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "edit":
                {
                    var btn = ((CMSGridActionButton)sender);

                    // Ensure accountID parameter value;
                    var objectID = ValidationHelper.GetInteger(btn.CommandArgument, 0);

                    // Account detail URL
                    string accountURL = UIContextHelper.GetElementDialogUrl(ModuleName.ONLINEMARKETING, "EditAccount", objectID, "isSiteManager=" + ContactHelper.IsSiteManager);

                    // Add modal dialog script to onClick action
                    btn.OnClientClick = ScriptHelper.GetModalDialogScript(accountURL, "AccountDetail");
                }
                break;
        }

        return null;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        pnlFooter.Visible = !DataHelper.DataSourceIsEmpty(gridElem.GridView.DataSource);
        gridElem.NamedColumns["sitename"].Visible = ((filter.SelectedSiteID < 0) && (filter.SelectedSiteID != UniSelector.US_GLOBAL_RECORD));
    }


    private void btnSplit_Click(object sender, EventArgs e)
    {
        if (AccountHelper.AuthorizedModifyAccount(Account.AccountSiteID, true))
        {
            var items = gridElem.SelectedItems;
            if (items.Count > 0)
            {
                AccountHelper.Split(Account, items.ToList(), chkCopyMissingFields.Checked, chkRemoveContacts.Checked, chkRemoveContactGroups.Checked);
                gridElem.ReloadData();
                gridElem.ResetSelection();
                ShowConfirmation(GetString("om.account.splitting"));
                pnlUpdate.Update();
            }
            else
            {
                ShowError(GetString("om.account.selectaccountssplit"));
            }
        }
    }


    /// <summary>
    /// Registers JS.
    /// </summary>
    private void RegisterScripts()
    {
        ScriptHelper.RegisterDialogScript(Page);
        StringBuilder script = new StringBuilder();

        // Register script to open dialogs for role selection and for account editing
        script.Append(@"
var dialogParams_" + ClientID + @" = '';
");

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "Actions", ScriptHelper.GetScript(script.ToString()));
    }

    #endregion


    #region "ICallbackEventHandler Members"

    /// <summary>
    /// Gets callback result.
    /// </summary>
    public string GetCallbackResult()
    {
        string queryString = string.Empty;

        if (!string.IsNullOrEmpty(CallbackArgument))
        {
            // Prepare parameters...
            mParameters = new Hashtable();
            // for unigrid action
            mParameters["accountcontactid"] = CallbackArgument;
            mParameters["allownone"] = "1";

            WindowHelper.Add(Identifier, mParameters);

            queryString = "?params=" + Identifier;
            queryString = URLHelper.AddParameterToUrl(queryString, "hash", QueryHelper.GetHash(queryString));
        }

        return queryString;
    }


    /// <summary>
    /// Raise callback method.
    /// </summary>
    public void RaiseCallbackEvent(string eventArgument)
    {
        CallbackArgument = eventArgument;
    }

    #endregion
}