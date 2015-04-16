using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Controls;
using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;

public partial class CMSModules_Ecommerce_Controls_Filters_CustomerTypeFilter : CMSAbstractDataFilterControl
{
    #region "Constants"

    protected const string CUSTOMERS_ALL = "all";
    protected const string CUSTOMERS_ANONYMOUS = "ano";
    protected const string CUSTOMERS_REGISTERED = "reg";

    #endregion


    #region "Properties"

    public override string WhereCondition
    {
        get
        {
            return GetWhereCondition();
        }
    }

    #endregion


    #region "Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Fill customer type selector
        ddlCustomerType.Items.Add(new ListItem(ResHelper.GetString("general.selectall"), CUSTOMERS_ALL));
        ddlCustomerType.Items.Add(new ListItem(ResHelper.GetString("general.yes"), CUSTOMERS_REGISTERED));
        ddlCustomerType.Items.Add(new ListItem(ResHelper.GetString("general.no"), CUSTOMERS_ANONYMOUS));
    }


    protected void Page_Load(object sender, EventArgs e)
    {
    }

    #endregion


    #region "Methods"

    public string GetWhereCondition()
    {
        switch (ddlCustomerType.SelectedValue)
        {
            case CUSTOMERS_ALL:
                return string.Empty;

            case CUSTOMERS_ANONYMOUS:
                return "(CustomerUserID IS NULL) AND CustomerSiteID = " + SiteContext.CurrentSiteID;

            case CUSTOMERS_REGISTERED:
                return string.Format("(CustomerUserID IS NOT NULL) AND (CustomerUserID IN (SELECT UserID FROM CMS_UserSite WHERE SiteID = {0}))", SiteContext.CurrentSiteID);
        }

        return "(1=0)";
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        ddlCustomerType.SelectedValue = CUSTOMERS_ALL;
    }

    #endregion
}