using System;
using System.Text;
using System.Web.UI.WebControls;
using CMS.Core;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalControls;
using CMS.Base;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSWebParts_OnlineMarketing_MyContacts : CMSAbstractWebPart
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
        var user = MembershipContext.AuthenticatedUser;
        bool siteContactsAllowed = user.IsAuthorizedPerResource(ModuleName.CONTACTMANAGEMENT, "ReadContacts");
        bool globalContactsAllowed = user.IsAuthorizedPerResource(ModuleName.CONTACTMANAGEMENT, "ReadGlobalContacts") && SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSCMGlobalContacts");
        if (!siteContactsAllowed && !globalContactsAllowed)
        {
            lblInfo.Visible = true;
            lblInfo.Text = GetString("om.myaccounts.notallowedtoreadcontacts");
            return;
        }

        // Create additional restriction if only site or global objects are allowed
        string where = null;
        if (!globalContactsAllowed)
        {
            where = SqlHelper.AddWhereCondition(where, "ContactSiteID IS NOT NULL");
        }
        if (!siteContactsAllowed)
        {
            where = SqlHelper.AddWhereCondition(where, "ContactSiteID IS NULL");
        }

        // Display accounts on current site or global site (if one of those shouldn't be displayed, it's filtered above)
        where = SqlHelper.AddWhereCondition(where, "ContactSiteID = " + SiteContext.CurrentSiteID + " OR ContactSiteID IS NULL");

        gridElem.Visible = true;
        gridElem.WhereCondition = SqlHelper.AddWhereCondition("ContactOwnerUserID=" + user.UserID + " AND ContactMergedWithContactID IS NULL", where);
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
        // Hide unwanted columns
        for (int i = gridElem.GridColumns.Columns.Count - 1; i >= 0; i--)
        {
            string colName = null;
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


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "edit":
                CMSGridActionButton btn = ((CMSGridActionButton)sender);
                string contactURL = UIContextHelper.GetElementDialogUrl(ModuleName.ONLINEMARKETING, "EditContact", btn.CommandArgument.ToInteger(0));
                // Add modal dialog script to onClick action
                btn.OnClientClick = ScriptHelper.GetModalDialogScript(contactURL, "ContactDetail");
                break;

            case "website":
                string url = ValidationHelper.GetString(parameter, null);
                if (url != null)
                {
                    return "<a target=\"_blank\" href=\"" + URLHelper.URLEncode(url) + "\" \">" + HTMLHelper.HTMLEncode(url) + "</a>";
                }

                return GetString("general.na");
        }
        return parameter;
    }

    #endregion
}