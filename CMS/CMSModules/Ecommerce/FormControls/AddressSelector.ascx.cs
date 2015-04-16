using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Base;

public partial class CMSModules_Ecommerce_FormControls_AddressSelector : FormEngineUserControl
{
    #region "Variables"

    private bool mShowBilling = true;
    private bool mShowShipping = true;
    private bool mShowCompany = true;
    private bool mRenderInline = true;
    private int mCustomerId = 0;
    private string mCustomerIdColumnName = string.Empty;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the ID of the customer the addresses of which should be displayed.
    /// </summary>
    public int CustomerID
    {
        get
        {
            // Get customer ID from form
            if ((mCustomerId <= 0) && !string.IsNullOrEmpty(CustomerIDColumnName) && Form.Data.ContainsColumn(CustomerIDColumnName))
            {
                int customerId = ValidationHelper.GetInteger(Form.Data.GetValue(CustomerIDColumnName), 0);
                if (customerId > 0)
                {
                    mCustomerId = customerId;
                }
            }

            return mCustomerId;
        }
        set
        {
            mCustomerId = value;
        }
    }


    /// <summary>
    /// Gets a column name for customer ID.
    /// </summary>
    public string CustomerIDColumnName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CustomerIDColumnName"), null);
        }
        set
        {
            SetValue("CustomerIDColumnName", value);
        }
    }


    /// <summary>
    /// Indicates whether to render update panel in inline mode.
    /// </summary>
    public bool RenderInline
    {
        get
        {
            return mRenderInline;
        }
        set
        {
            mRenderInline = value;
        }
    }


    /// <summary>
    /// Indicates whether to display all addresses - if true all other settings (ShowOnlyEnabled, ShowBilling, ShowShipping) are ignored.
    /// </summary>
    public bool ShowAll
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates, whether to show button for adding new address. Default value is true.
    /// </summary>
    public bool ShowNewButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowNewButton"), true);
        }
        set
        {
            SetValue("ShowNewButton", value);
        }
    }


    /// <summary>
    /// Indicates, whether to show button for editing selected address. Default value is true.
    /// </summary>
    public bool ShowEditButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowEditButton"), true);
        }
        set
        {
            SetValue("ShowEditButton", value);
        }
    }


    /// <summary>
    /// Indicates whether to display only enabled addresses.
    /// </summary>
    public bool DisplayOnlyEnabled
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayOnlyEnabled"), true);
        }
        set
        {
            SetValue("DisplayOnlyEnabled", value);
        }
    }


    /// <summary>
    /// Indicates whether to display billing addresses.
    /// </summary>
    public bool ShowBilling
    {
        get
        {
            return mShowBilling;
        }
        set
        {
            mShowBilling = value;
        }
    }

    /// <summary>
    /// Indicates whether to shipping billing addresses.
    /// </summary>
    public bool ShowShipping
    {
        get
        {
            return mShowShipping;
        }
        set
        {
            mShowShipping = value;
        }
    }


    /// <summary>
    /// Indicates whether to display company addresses.
    /// </summary>
    public bool ShowCompany
    {
        get
        {
            return mShowCompany;
        }
        set
        {
            mShowCompany = value;
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return AddressID;
        }
        set
        {
            AddressID = ValidationHelper.GetInteger(value, 0);
        }
    }


    /// <summary>
    /// Gets or sets the Address ID.
    /// </summary>
    public int AddressID
    {
        get
        {
            return ValidationHelper.GetInteger(uniSelector.Value, 0);
        }
        set
        {
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }
            uniSelector.Value = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether to add new item record to the dropdown list.
    /// </summary>
    public bool AddNewRecord
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AddNewRecord"), false);
        }
        set
        {
            SetValue("AddNewRecord", value);
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether to add none item record to the dropdown list.
    /// </summary>
    public bool AddNoneRecord
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AddNoneRecord"), true);
        }
        set
        {
            SetValue("AddNoneRecord", value);
        }
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            if (uniSelector != null)
            {
                uniSelector.Enabled = value;
            }
        }
    }


    /// <summary>
    /// Returns ClientID of the dropdown list.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return uniSelector.DropDownSingleSelect.ClientID;
        }
    }


    /// <summary>
    /// Returns inner DropDown control.
    /// </summary>
    public CMSDropDownList DropDownSingleSelect
    {
        get
        {
            return uniSelector.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Set typeId for new address after "new" button clicked. 0 = All (default), 1 = Shipping, 2 = Billing, 3 = Company
    /// </summary> 
    public int NewTypeId
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            uniSelector.StopProcessing = true;
        }
        else
        {
            ReloadData();
        }

        DropDownSingleSelect.AutoPostBack = true;

        // Show New and Edit buttons if user has permission
        if (ECommerceContext.IsUserAuthorizedForPermission("ModifyCustomers"))
        {
            string dialogUrl = ResolveUrl("~/CMSModules/Ecommerce/FormControls/AddressSelectorDialog.aspx?typeId=" + NewTypeId + "&customerId=" + CustomerID);
            uniSelector.NewItemPageUrl = dialogUrl;
            uniSelector.ButtonDropDownNew.ButtonStyle = ButtonStyle.Default;
            uniSelector.EditItemPageUrl = dialogUrl + "&addressId=##ITEMID##";
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        if (RenderInline)
        {
            pnlUpdate.RenderMode = UpdatePanelRenderMode.Inline;
        }

        uniSelector.IsLiveSite = IsLiveSite;
        uniSelector.AllowEmpty = AddNoneRecord;

        string where = "AddressCustomerID = " + CustomerID;

        if (!ShowAll)
        {
            string typeWhere = "";

            if (ShowBilling)
            {
                typeWhere = SqlHelper.AddWhereCondition(typeWhere, "AddressIsBilling = 1", "OR");
            }
            if (ShowShipping)
            {
                typeWhere = SqlHelper.AddWhereCondition(typeWhere, "AddressIsShipping = 1", "OR");
            }
            if (ShowCompany)
            {
                typeWhere = SqlHelper.AddWhereCondition(typeWhere, "AddressIsCompany = 1", "OR");
            }

            // Select nothing when no address types requested
            if (string.IsNullOrEmpty(typeWhere))
            {
                typeWhere = "(0=1)";
            }

            // Apply type where
            where = SqlHelper.AddWhereCondition(where, typeWhere);

            if (DisplayOnlyEnabled)
            {
                where = SqlHelper.AddWhereCondition(where, "AddressEnabled = 1");
            }
        }

        // Include selected value
        if (AddressID > 0)
        {
            where = SqlHelper.AddWhereCondition(where, "AddressID = " + AddressID, "OR");
        }

        uniSelector.WhereCondition = where;

        if (AddNewRecord)
        {
            uniSelector.SpecialFields.Add(new SpecialField() { Text = GetString("shoppingcartorderaddresses.newaddress"), Value = "0" });
        }
    }

    #endregion
}