using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Web.UI;

using CMS.Base;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_ContactManagement_Controls_UI_Account_MergeChoose : CMSAdminListControl
{
    #region "Variables and constants"

    /// <summary>
    /// URL of collision dialog.
    /// </summary>
    private const string MERGE_DIALOG = "~/CMSModules/ContactManagement/Pages/Tools/Account/CollisionDialog.aspx";

    private CMSModules_ContactManagement_Controls_UI_Account_Filter mFilter;
    private AccountInfo mAccountInfo;

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
            plcMess.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Current contact.
    /// </summary>
    public AccountInfo Account
    {
        get
        {
            if (mAccountInfo == null)
            {
                if (UIContext.EditedObject != null)
                {
                    mAccountInfo = (AccountInfo)UIContext.EditedObject;
                }
            }
            return mAccountInfo;
        }
    }


    /// <summary>
    /// Modal dialog identifier.
    /// </summary>
    private Guid Identifier
    {
        get
        {
            // Try to load data from control View State
            Guid identifier = hdnIdentifier.Value.ToGuid(Guid.NewGuid());
            hdnIdentifier.Value = identifier.ToString();

            return identifier;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        gridElem.OnFilterFieldCreated += gridElem_OnFilterFieldCreated;
        gridElem.LoadGridDefinition();
        base.OnInit(e);
    }


    private void gridElem_OnFilterFieldCreated(string columnName, UniGridFilterField filterDefinition)
    {
        mFilter = filterDefinition.ValueControl as CMSModules_ContactManagement_Controls_UI_Account_Filter;
        if (mFilter != null)
        {
            mFilter.HideMergedFilter = true;
            mFilter.NotMerged = true;
            mFilter.IsLiveSite = IsLiveSite;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (Account != null)
        {
            // Current account is global object
            if (Account.AccountSiteID == 0)
            {
                mFilter.SiteID = SiteContext.CurrentSiteID;
                // Display site selector in site manager
                if (ContactHelper.IsSiteManager)
                {
                    mFilter.DisplaySiteSelector = true;
                    mFilter.SiteID = UniSelector.US_GLOBAL_RECORD;
                }
                    // Display 'site or global' selector in CMS desk for global objects
                else if (AccountHelper.AuthorizedModifyAccount(SiteContext.CurrentSiteID, false) && AccountHelper.AuthorizedReadAccount(SiteContext.CurrentSiteID, false))
                {
                    mFilter.DisplayGlobalOrSiteSelector = true;
                }
                mFilter.HideMergedIntoGlobal = true;
            }
            else
            {
                mFilter.SiteID = Account.AccountSiteID;
            }
            mFilter.ShowGlobalStatuses =
                ConfigurationHelper.AuthorizedReadConfiguration(UniSelector.US_GLOBAL_RECORD, false) &&
                (SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSCMGlobalConfiguration") || ContactHelper.IsSiteManager);
            gridElem.WhereCondition = mFilter.WhereCondition;
            gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, "AccountID <> " + Account.AccountID);
            gridElem.ZeroRowsText = GetString("om.account.noaccountsfound");

            btnMergeSelected.Click += btnMergeSelected_Click;
            btnMergeAll.Click += btnMergeAll_Click;

            if (Request[Page.postEventArgumentID] == "saved")
            {
                ShowConfirmation(GetString("om.account.merging"));

                // Clear selected items
                gridElem.ResetSelection();
            }
        }
        else
        {
            StopProcessing = true;
            Visible = false;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        pnlButton.Visible = !DataHelper.DataSourceIsEmpty(gridElem.GridView.DataSource);
        gridElem.NamedColumns["sitename"].Visible = ((mFilter.SelectedSiteID < 0) && (mFilter.SelectedSiteID != UniSelector.US_GLOBAL_RECORD));
    }


    /// <summary>
    /// Button merge all click
    /// </summary>
    private void btnMergeAll_Click(object sender, EventArgs e)
    {
        if (AccountHelper.AuthorizedModifyAccount(Account.AccountSiteID, true))
        {
            if (!DataHelper.DataSourceIsEmpty(gridElem.GridView.DataSource))
            {
                SetDialogParameters(true);
                OpenWindow();
            }
            else
            {
                ShowError(GetString("om.account.noaccounts"));
            }
        }
    }


    /// <summary>
    /// Button merge click.
    /// </summary>
    private void btnMergeSelected_Click(object sender, EventArgs e)
    {
        if (AccountHelper.AuthorizedModifyAccount(Account.AccountSiteID, true))
        {
            if (gridElem.SelectedItems.Count > 0)
            {
                SetDialogParameters(false);
                OpenWindow();
            }
            else
            {
                ShowError(GetString("om.account.selectaccounts"));
            }
        }
    }


    /// <summary>
    /// Sets the dialog parameters to the context.
    /// </summary>
    private void SetDialogParameters(bool allData)
    {
        var selectedItems = gridElem.SelectedItems.Select(item => item.ToInteger(0)).Where(id => id != 0).ToList();

        string condition = allData ? SqlHelper.AddWhereCondition(gridElem.WhereCondition, gridElem.WhereClause) : SqlHelper.GetWhereCondition("AccountID", selectedItems);

        Hashtable parameters = new Hashtable();
        parameters["MergedAccounts"] = new AccountListInfo().Generalized.GetData(null, condition, null, -1, null, false);
        parameters["ParentAccount"] = Account;
        parameters["issitemanager"] = ContactHelper.IsSiteManager;

        WindowHelper.Add(Identifier.ToString(), parameters);
    }


    /// <summary>
    /// Registers JS for opening window.
    /// </summary>
    private void OpenWindow()
    {
        ScriptHelper.RegisterDialogScript(Page);
        string url = MERGE_DIALOG + "?params=" + Identifier;
        url += "&hash=" + QueryHelper.GetHash(url, false);
        StringBuilder script = new StringBuilder();
        script.Append(@"modalDialog('" + ResolveUrl(url) + @"', 'mergeDialog', 850, 700, null, null, true);");
        ScriptHelper.RegisterStartupScript(this, typeof (string), "MergeDialog" + ClientID, ScriptHelper.GetScript(script.ToString()));
        ScriptHelper.RegisterClientScriptBlock(this, typeof (string), "RefreshPageScript", ScriptHelper.GetScript("function RefreshPage() { __doPostBack('" + ClientID + @"', 'saved'); }"));
    }

    #endregion
}