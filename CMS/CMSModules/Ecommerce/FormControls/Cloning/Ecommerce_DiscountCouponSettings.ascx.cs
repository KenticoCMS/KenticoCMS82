using System;
using System.Collections;

using CMS.FormControls;
using CMS.Base;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_FormControls_Cloning_Ecommerce_DiscountCouponSettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Gets properties hashtable.
    /// </summary>
    public override Hashtable CustomParameters
    {
        get
        {
            return GetProperties();
        }
    }


    /// <summary>
    /// Hide code name.
    /// </summary>
    public override bool HideCodeName
    {
        get
        {
            return true;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            txtCode.Text = InfoToClone.Generalized.GetUniqueCodeName();
        }
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[DiscountCouponInfo.OBJECT_TYPE + ".code"] = txtCode.Text;
        return result;
    }

    #endregion
}