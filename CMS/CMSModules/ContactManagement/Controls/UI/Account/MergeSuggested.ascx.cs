using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;

using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.DataEngine;

public partial class CMSModules_ContactManagement_Controls_UI_Account_MergeSuggested : CMSAdminListControl
{
    #region "Variables"

    protected int mSiteId = -1;
    protected AccountInfo ai;

    /// <summary>
    /// URL of collision dialog.
    /// </summary>
    private const string MERGE_DIALOG = "~/CMSModules/ContactManagement/Pages/Tools/Account/CollisionDialog.aspx";

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
            filter.IsLiveSite = value;
            plcMess.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets the site id.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteId;
        }
        set
        {
            mSiteId = value;
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

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get edited object
        if (Account != null)
        {
            SiteID = ai.AccountSiteID;
            gridElem.WhereCondition = filter.GetWhereCondition();
            gridElem.ZeroRowsText = GetString("om.account.noaccountsfound");
            gridElem.OnBeforeDataReload += new OnBeforeDataReload(gridElem_OnBeforeDataReload);
            btnMergeSelected.Click += new EventHandler(btnMerge_Click);
            btnMergeAll.Click += new EventHandler(btnMergeAll_Click);

            if (QueryHelper.GetBoolean("saved", false))
            {
                ShowConfirmation(GetString("om.account.merging"));
            }
        }
        else
        {
            StopProcessing = true;
            Visible = false;
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        pnlFooter.Visible = !DataHelper.DataSourceIsEmpty(gridElem.GridView.DataSource);
    }


    private void gridElem_OnBeforeDataReload()
    {
        // Hide address columns
        if (!filter.AddressChecked)
        {
            gridElem.NamedColumns["address1"].Visible
                = gridElem.NamedColumns["address2"].Visible
                  = gridElem.NamedColumns["city"].Visible
                    = gridElem.NamedColumns["zip"].Visible = false;
        }
        // Hide web column
        gridElem.NamedColumns["website"].Visible = filter.URLChecked;

        // Hide phone & fax columns
        if (!filter.PhonesChecked)
        {
            gridElem.NamedColumns["phone"].Visible
                = gridElem.NamedColumns["fax"].Visible = false;
        }
        // Hide email column
        gridElem.NamedColumns["email"].Visible = filter.EmailChecked;

        // Hide contact columns
        if (!filter.ContactsChecked)
        {
            gridElem.NamedColumns["primarycontactfullname"].Visible
                = gridElem.NamedColumns["secondarycontactfullname"].Visible = false;
        }

        // Hide site name column
        gridElem.NamedColumns["sitename"].Visible = ((filter.SelectedSiteID < 0) && (filter.SelectedSiteID != UniSelector.US_GLOBAL_RECORD));
    }


    /// <summary>
    /// Button merge click.
    /// </summary>
    private void btnMerge_Click(object sender, EventArgs e)
    {
        if (AccountHelper.AuthorizedModifyAccount(SiteID, true))
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
    /// Button merge click.
    /// </summary>
    private void btnMergeAll_Click(object sender, EventArgs e)
    {
        if (AccountHelper.AuthorizedModifyAccount(SiteID, true))
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
    /// Sets the dialog parameters to the context.
    /// </summary>
    private void SetDialogParameters(bool mergeAll)
    {
        Hashtable parameters = new Hashtable();

        DataSet ds;

        if (mergeAll)
        {
            ds = new AccountListInfo().Generalized.GetData(null, gridElem.WhereCondition, null, -1, null, false);
        }
        else
        {
            ds = new AccountListInfo().Generalized.GetData(null, SqlHelper.GetWhereCondition("AccountID", gridElem.SelectedItems), null, -1, null, false);
        }

        parameters["MergedAccounts"] = ds;
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
        script.Append(@"modalDialog('" + ResolveUrl(url) + @"', 'mergeDialog', 700, 700, null, null, true);");
        ScriptHelper.RegisterStartupScript(this, typeof(string), "MergeDialog" + ClientID, ScriptHelper.GetScript(script.ToString()));
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshPageScript", ScriptHelper.GetScript("function RefreshPage() { window.location.replace('" + URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "saved", "true") + "'); }"));
    }

    #endregion
}