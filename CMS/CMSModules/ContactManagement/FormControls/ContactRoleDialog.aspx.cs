using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSModules_ContactManagement_FormControls_ContactRoleDialog : CMSModalPage
{
    #region "Variables"

    private int siteId = -1;
    protected AccountContactInfo aci = null;
    protected bool isMassAction = false;
    protected Hashtable mParameters;

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

        PageTitle.TitleText = GetString("om.contactrole.select");
        Page.Title = PageTitle.TitleText;

        // Check if the dialog was opened from mass actions
        isMassAction = Parameters.ContainsKey("ismassaction");
        if (isMassAction)
        {
            siteId = ValidationHelper.GetInteger(Parameters["siteid"], 0);
        }
        else
        {
            int accountContactId = ValidationHelper.GetInteger(Parameters["accountcontactid"], 0);
            aci = AccountContactInfoProvider.GetAccountContactInfo(accountContactId);
            if (aci != null)
            {
                AccountInfo ai = AccountInfoProvider.GetAccountInfo(aci.AccountID);
                if (ai != null)
                {
                    siteId = ai.AccountSiteID;
                }
            }
        }

        // Show all global configuration to authorized users ..
        bool allowGlobal = ConfigurationHelper.AuthorizedReadConfiguration(UniSelector.US_GLOBAL_RECORD, false);

        // .. but just in SiteManager - fake it in CMSDesk so that even Global Admin sees user configuration
        // as Site Admins (depending on settings).
        bool isSiteManager = ValidationHelper.GetBoolean(Parameters["issitemanager"], false);
        allowGlobal &= (isSiteManager || SettingsKeyInfoProvider.GetBoolValue(SiteInfoProvider.GetSiteName(siteId) + ".cmscmglobalconfiguration"));

        bool allowSite;
        if (siteId > 0)
        {
            allowSite = ConfigurationHelper.AuthorizedReadConfiguration(siteId, false);
        }
        else
        {
            allowSite = ConfigurationHelper.AuthorizedReadConfiguration(SiteContext.CurrentSiteID, false);
        }

        // Check read permission
        if ((siteId > 0) && !allowSite && !allowGlobal)
        {
            RedirectToAccessDenied("cms.contactmanagement", "ReadConfiguration");
            return;
        }
        else if ((siteId == 0) && !allowGlobal)
        {
            RedirectToAccessDenied("cms.contactmanagement", "ReadGlobalConfiguration");
            return;
        }

        if (siteId > 0)
        {
            if (allowSite)
            {
                gridElem.WhereCondition = "ContactRoleSiteID = " + siteId;
            }

            // Check if global config is allowed for the site
            if (allowGlobal)
            {
                // Add contact roles from global configuration
                gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, "ContactRoleSiteID IS NULL", "OR");
            }
        }
        else if ((siteId == 0) && allowGlobal)
        {
            gridElem.WhereCondition = "ContactRoleSiteID IS NULL";
        }

        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.Pager.DefaultPageSize = 10;

        // Display 'Reset' button when 'none' role is allowed
        if (ValidationHelper.GetBoolean(Parameters["allownone"], false))
        {
            btnReset.Visible = true;
            btnReset.Click += btn_Click;
            btnReset.CommandArgument = "0";
        }
    }


    /// <summary>
    /// Unigrid after data reload handler.
    /// </summary>
    protected void gridElem_OnAfterDataReload()
    {
        if (!gridElem.IsEmpty)
        {
            // Add '(none)' record so it is possible to remove contact role
            DataSet data = gridElem.GridView.DataSource as DataSet;
            DataRow noneRow = data.Tables[0].NewRow();
            noneRow["ContactRoleID"] = 0;
            noneRow["ContactRoleDisplayName"] = GetString("general.selectnone");
            noneRow["ContactRoleSiteID"] = siteId;
            data.Tables[0].Rows.InsertAt(noneRow, 0);
        }
    }


    /// <summary>
    /// Unigrid external databound handler.
    /// </summary>
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "contactroledisplayname":
                LinkButton btn = new LinkButton();
                DataRowView drv = parameter as DataRowView;
                btn.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ValidationHelper.GetString(drv["ContactRoleDisplayName"], null)));
                btn.Click += new EventHandler(btn_Click);
                btn.CommandArgument = ValidationHelper.GetString(drv["ContactRoleID"], null);
                btn.ToolTip = HTMLHelper.HTMLEncode(ValidationHelper.GetString(drv.Row["ContactRoleDescription"], null));
                return btn;
        }
        return parameter;
    }


    /// <summary>
    /// Contact role selected event handler.
    /// </summary>
    protected void btn_Click(object sender, EventArgs e)
    {
        int roleId = ValidationHelper.GetInteger(((IButtonControl)sender).CommandArgument, 0);
        string script = null;
        if (!isMassAction)
        {
            // Set contact role to specified account-contact relation
            aci.ContactRoleID = roleId;
            AccountContactInfoProvider.SetAccountContactInfo(aci);

            script = ScriptHelper.GetScript(@"
if (wopener.Refresh) {wopener.Refresh();}
setTimeout('CloseDialog()',200);
");
        }
        else
        {
            script = ScriptHelper.GetScript(@"
wopener.AssignContactRole_" + ValidationHelper.GetString(Parameters["clientid"], string.Empty) + @"(" + roleId + @");
setTimeout('CloseDialog()',200);
");
        }

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "CloseWindow", script);
    }

    #endregion
}