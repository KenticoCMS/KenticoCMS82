using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.DataEngine;

public partial class CMSModules_ContactManagement_FormControls_AccountSelectorDialog : CMSModalPage
{
    #region "Variables"

    private int siteId = -1;
    private Hashtable mParameters;
    private string where = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Stop processing flag.
    /// </summary>
    public bool StopProcessing
    {
        get
        {
            return gridElem.StopProcessing;
        }
        set
        {
            gridElem.StopProcessing = value;
        }
    }


    /// <summary>
    /// Hashtable containing dialog parameters.
    /// </summary>
    private Hashtable Parameters
    {
        get
        {
            if (mParameters == null)
            {
                string identifier = QueryHelper.GetString("params", null);
                mParameters = (Hashtable)WindowHelper.GetItem(identifier);
            }
            return mParameters;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!QueryHelper.ValidateHash("hash") || Parameters == null)
        {
            StopProcessing = true;
            return;
        }

        CurrentMaster.PanelContent.RemoveCssClass("dialog-content");

        siteId = ValidationHelper.GetInteger(Parameters["siteid"], 0);
        where = ValidationHelper.GetString(Parameters["where"], null);

        // Check read permission
        if (AccountHelper.AuthorizedReadAccount(siteId, true))
        {
            if (siteId == UniSelector.US_GLOBAL_RECORD)
            {
                PageTitle.TitleText = GetString("om.account.selectglobal");
            }
            else
            {
                PageTitle.TitleText = GetString("om.account.selectsite");
            }
            Page.Title = PageTitle.TitleText;

            // Load header actions
            InitHeaderActions();

            if (siteId > 0)
            {
                gridElem.WhereCondition = "(AccountMergedWithAccountID IS NULL AND AccountSiteID = " + siteId + ")";
            }
            else
            {
                gridElem.WhereCondition = "(AccountGlobalAccountID IS NULL AND AccountSiteID IS NULL)";
            }
            gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, "AccountID NOT IN (SELECT AccountID FROM OM_Account WHERE " + where + ")");
            gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
            gridElem.ShowActionsMenu = false;
            if (!RequestHelper.IsPostBack())
            {
                gridElem.Pager.DefaultPageSize = 10;
            }
        }
    }


    /// <summary>
    /// Unigrid external databound handler.
    /// </summary>
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "AccountName":
                LinkButton btn = new LinkButton();
                DataRowView drv = parameter as DataRowView;
                btn.Text = HTMLHelper.HTMLEncode(ValidationHelper.GetString(drv["AccountName"], null));
                btn.Click += new EventHandler(btn_Click);
                btn.CommandArgument = ValidationHelper.GetString(drv["AccountID"], null);
                return btn;
        }
        return parameter;
    }


    /// <summary>
    /// Contact status selected event handler.
    /// </summary>
    protected void btn_Click(object sender, EventArgs e)
    {
        int contactID = ValidationHelper.GetInteger(((LinkButton)sender).CommandArgument, 0);
        string script = ScriptHelper.GetScript(@"
wopener.SelectValue_" + ValidationHelper.GetString(Parameters["clientid"], string.Empty) + @"(" + contactID + @");
CloseDialog();
");

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "CloseWindow", script);
    }


    /// <summary>
    /// Initialize header actions.
    /// </summary>
    private void InitHeaderActions()
    {
        AddHeaderAction(new HeaderAction
        {
            Text = GetString("om.account.new"),
            OnClientClick = @"wopener.SelectValue_" + ValidationHelper.GetString(Parameters["clientid"], string.Empty) + @"(0); CloseDialog();"
        });
    }

    #endregion
}