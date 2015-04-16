using System;
using System.Text;
using System.Web.UI.WebControls;
using System.Data;

using CMS.Core;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.PortalControls;
using CMS.Base;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.DataEngine;
using CMS.ExtendedControls;

public partial class CMSWebParts_OnlineMarketing_MyAccounts : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets list of visible fields (columns).
    /// </summary>
    public string VisibleFields
    {
        get
        {
            return ValidationHelper.GetString(GetValue("VisibleFields"), "");
        }
        set
        {
            SetValue("VisibleFields", value);
        }
    }


    /// <summary>
    /// Gets or sets page size.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), 0);
        }
        set
        {
            SetValue("PageSize", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    protected void SetupControl()
    {
        // Check permissions
        var currentUser = MembershipContext.AuthenticatedUser;
        bool siteAccountsAllowed = currentUser.IsAuthorizedPerResource(ModuleName.CONTACTMANAGEMENT, "ReadAccounts");
        bool globalAccountsAllowed = currentUser.IsAuthorizedPerResource(ModuleName.CONTACTMANAGEMENT, "ReadGlobalAccounts") && SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSCMGlobalAccounts");
        if (!siteAccountsAllowed && !globalAccountsAllowed)
        {
            lblInfo.Visible = true;
            lblInfo.Text = GetString("om.myaccounts.notallowedtoreadaccounts");
            return;
        }

        // Create additional restriction if only site or global objects are allowed
        string where = null;
        if (!globalAccountsAllowed)
        {
            where = SqlHelper.AddWhereCondition(null, "AccountSiteID IS NOT NULL");
        }
        if (!siteAccountsAllowed)
        {
            where = SqlHelper.AddWhereCondition(null, "AccountSiteID IS NULL");
        }

        // Display accounts on current site or global site (if one of those shouldn't be displayed, it's filtered above)
        where = SqlHelper.AddWhereCondition(where, "AccountSiteID = " + SiteContext.CurrentSiteID + " OR AccountSiteID IS NULL");

        gridElem.Visible = true;
        gridElem.WhereCondition = SqlHelper.AddWhereCondition("AccountOwnerUserID=" + currentUser.UserID + " AND AccountMergedWithAccountID IS NULL", where);
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.Pager.DefaultPageSize = PageSize;

        ScriptHelper.RegisterDialogScript(Page);

        SetVisibleColumns();
    }


    /// <summary>
    /// Hide unwanted columns.
    /// </summary>
    protected void SetVisibleColumns()
    {
        string visibleCols = "|" + VisibleFields.Trim('|') + "|";
        string colName;
        // Hide unwanted columns
        for (int i = gridElem.GridColumns.Columns.Count - 1; i >= 0; i--)
        {
            if (!String.IsNullOrEmpty(colName = gridElem.GridColumns.Columns[i].Name))
            {
                if (visibleCols.IndexOfCSafe("|" + colName + "|", StringComparison.Ordinal) == -1)
                {
                    gridElem.GridColumns.Columns[i].Visible = false;
                    gridElem.GridColumns.Columns[i].Filter = null;
                }
                else
                {
                    gridElem.GridColumns.Columns[i].Visible = true;
                }
            }
        }
    }


    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "edit":
                var btn = ((CMSGridActionButton)sender);
                // Ensure accountID parameter value;
                var objectID = ValidationHelper.GetInteger(btn.CommandArgument, 0);
                // Account detail URL
                string accountURL = UIContextHelper.GetElementDialogUrl(ModuleName.ONLINEMARKETING, "EditAccount", objectID, "isSiteManager=" + ContactHelper.IsSiteManager);
                // Add modal dialog script to onClick action
                btn.OnClientClick = ScriptHelper.GetModalDialogScript(accountURL, "AccountDetail");
                break;

            case "primary":
                DataRowView drv = (DataRowView)parameter;
                int contactId = ValidationHelper.GetInteger(drv["AccountPrimaryContactID"], 0);
                string fullName = ValidationHelper.GetString(drv["PrimaryContactFullName"], null);

                string contactURL = UIContextHelper.GetElementDialogUrl(ModuleName.ONLINEMARKETING, "EditContact", contactId);
                // Add modal dialog script to onClick action
                var script = ScriptHelper.GetModalDialogScript(contactURL, "ContactDetail");
                return "<a href=\"#\" onclick=\"" + script + "\">" + HTMLHelper.HTMLEncode(fullName) + "</a>";

            case "website":
                string url = ValidationHelper.GetString(parameter, null);
                if (url != null)
                {
                    return "<a target=\"_blank\" href=\"" + HTMLHelper.HTMLEncode(url) + "\" \">" + HTMLHelper.HTMLEncode(url) + "</a>";
                }

                return GetString("general.na");
        }

        return parameter;
    }

    #endregion
}