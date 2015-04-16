using System;
using System.Collections.Generic;

using CMS.UIControls;

public partial class CMSModules_Ecommerce_Controls_UI_QuickNavigation : CMSAdminControl
{
    #region "Variables"

    private IDictionary<string, string> mNavigationItems; // private member mItems already exists in parent (CMSUserControl)

    #endregion


    #region "Properties"

    public IDictionary<string, string> Items
    {
        get
        {
            return mNavigationItems ?? (mNavigationItems = new Dictionary<string, string>());
        }
        set
        {
            mNavigationItems = value;
        }
    }


    public string CssClass
    {
        get
        {
            return pnlWrapper.CssClass;
        }
        set
        {
            pnlWrapper.CssClass = value;
        }
    }

    #endregion


    #region "Lifecycle methods"

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        lstNavigationItems.DataSource = Items;
        lstNavigationItems.DataBind();
    }

    #endregion
}
