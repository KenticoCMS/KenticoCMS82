using System;

using CMS.Ecommerce;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.FormEngine;
using CMS.DataEngine;

public partial class CMSModules_Ecommerce_FormControls_DepartmentSelector : SiteSeparatedObjectSelector
{
    #region "Variables and constants"

    private const int ALL_MY_DEPARTMENTS = -6;
    private const int WITHOUT_DEPARTMENT = -5;

    private bool mAddAllMyRecord;
    private string mAllMyRecordValue = string.Empty;
    private int mUserId;
    private bool mReflectGlobalProductsUse;
    private bool mDropDownListMode = true;
    private bool mAddWithoutDepartmentRecord;
    private bool mShowAllSites;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the ID of the user the departments of which should be displayed. 0 means all departments are displayed.
    /// </summary>
    public int UserID
    {
        get
        {
            if (mUserId == 0)
            {
                return MembershipContext.AuthenticatedUser.UserID;
            }
            return mUserId;
        }
        set
        {
            mUserId = value;
        }
    }


    /// <summary>
    /// Allows to access uniselector object
    /// </summary>
    public override UniSelector UniSelector
    {
        get
        {
            return uniSelector;
        }
    }


    /// <summary>
    ///  If true, selected value is DepartmentName, if false, selected value is DepartmentID.
    /// </summary>
    public override bool UseNameForSelection
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseDepartmentNameForSelection"), base.UseNameForSelection);
        }
        set
        {
            SetValue("UseDepartmentNameForSelection", value);
            base.UseNameForSelection = value;
        }
    }


    /// <summary>
    /// Returns ClientID of the dropdown list.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            if (DropDownListMode)
            {
                return uniSelector.DropDownSingleSelect.ClientID;
            }

            return uniSelector.TextBoxSelect.ClientID;
        }
    }


    /// <summary>
    /// Indicates whether global items are to be offered.
    /// </summary>
    public override bool DisplayGlobalItems
    {
        get
        {
            return base.DisplayGlobalItems || (ReflectGlobalProductsUse && ECommerceSettings.AllowGlobalProducts(SiteID));
        }
        set
        {
            base.DisplayGlobalItems = value;
        }
    }


    /// <summary>
    /// Gets or sets a value that indicates if the global items should be displayed when the global products are used on the site.
    /// </summary>
    public bool ReflectGlobalProductsUse
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ReflectGlobalProductsUse"), mReflectGlobalProductsUse);
        }
        set
        {
            SetValue("ReflectGlobalProductsUse", value);
            mReflectGlobalProductsUse = value;
        }
    }


    /// <summary>
    /// Indicates if drop down list mode is used. Default value is true.
    /// </summary>
    public bool DropDownListMode
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DropDownListMode"), mDropDownListMode);
        }
        set
        {
            SetValue("DropDownListMode", value);
            mDropDownListMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether to add all my departments item record to the dropdown list.
    /// </summary>
    public bool AddAllMyRecord
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AddAllMyRecord"), mAddAllMyRecord);
        }
        set
        {
            SetValue("AddAllMyRecord", value);
            mAddAllMyRecord = value;
        }
    }


    /// <summary>
    /// Gets the value of 'All my departments' record.
    /// </summary>
    public int AllMyRecordValue
    {
        get
        {
            return ALL_MY_DEPARTMENTS;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether to add 'without department' item record to the dropdown list.
    /// </summary>
    public bool AddWithoutDepartmentRecord
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AddWithoutDepartmentRecord"), mAddWithoutDepartmentRecord);
        }
        set
        {
            SetValue("AddWithoutDepartmentRecord", value);
            mAddWithoutDepartmentRecord = value;
        }
    }


    /// <summary>
    /// Gets the value of 'Without department' record.
    /// </summary>
    public int WithoutDepartmentRecordValue
    {
        get
        {
            return WITHOUT_DEPARTMENT;
        }
    }


    /// <summary>
    /// Indicates whether departments from all sites are to be shown. Default value is false.
    /// </summary>
    public virtual bool ShowAllSites
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowAllSites"), mShowAllSites);
        }
        set
        {
            mShowAllSites = value;
            SetValue("ShowAllSites", value);
        }
    }

    #endregion


    #region "Lifecycle"

    protected override void OnInit(EventArgs e)
    {
        uniSelector.SelectionMode = (DropDownListMode ? SelectionModeEnum.SingleDropDownList : SelectionModeEnum.SingleTextBox);

        base.OnInit(e);
    }


    protected override void OnLoad(EventArgs e)
    {
        TryInitByForm();

        base.OnLoad(e);
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (RequestHelper.IsPostBack() && DependsOnAnotherField)
        {
            InitSelector();
        }

        uniSelector.Reload(true);

        base.OnPreRender(e);
    }

    #endregion


    #region "Initialization"

    protected override void InitSelector()
    {
        // Add special records
        if (AddAllMyRecord)
        {
            uniSelector.SpecialFields.Add(new SpecialField { Text = GetString("product_list.allmydepartments"), Value = AllMyRecordValue.ToString() });
        }

        if (AddWithoutDepartmentRecord)
        {
            uniSelector.SpecialFields.Add(new SpecialField { Text = GetString("general.empty"), Value = WithoutDepartmentRecordValue.ToString() });
        }

        base.InitSelector();

        if (ShowAllSites)
        {
            uniSelector.FilterControl = "~/CMSFormControls/Filters/SiteFilter.ascx";
            uniSelector.SetValue("FilterMode", "department");
        }

        if (UseNameForSelection)
        {
            uniSelector.AllRecordValue = "";
            uniSelector.NoneRecordValue = "";
        }
    }


    /// <summary>
    /// Convert given department name to its ID for specified site.
    /// </summary>
    /// <param name="name">Name of the department to be converted.</param>
    /// <param name="siteName">Name of the site of the department.</param>
    protected override int GetID(string name, string siteName)
    {
        DepartmentInfo dept;

        if (ShowAllSites)
        {
            // Take any department
            dept = DepartmentInfoProvider.GetDepartments()
                       .TopN(1)
                       .WithCodeName(name)
                       .OrderByAscending("DepartmentSiteID");
        }
        else
        {
            dept = DepartmentInfoProvider.GetDepartmentInfo(name, siteName);
        }

        return (dept != null) ? dept.DepartmentID : 0;
    }


    /// <summary>
    /// Appends where condition filtering only users departments to given condition.
    /// </summary>
    /// <param name="where">Condition to be enriched.</param>
    protected override string AppendExclusiveWhere(string where)
    {
        where = base.AppendExclusiveWhere(where);

        // Get only departments of the given user if he is not authorized for all departments
        if (!ShowAllSites && (UserID > 0) && !UserInfoProvider.IsAuthorizedPerResource("CMS.Ecommerce", "AccessAllDepartments", SiteContext.CurrentSiteName, UserInfoProvider.GetUserInfo(UserID)))
        {
            where = SqlHelper.AddWhereCondition(where, "DepartmentID IN (SELECT DepartmentID FROM COM_UserDepartment WHERE UserID = " + UserID + ")");
        }

        return where;
    }


    /// <summary>
    /// Appends site where to given where condition.
    /// </summary>
    /// <param name="where">Original where condition to append site where to.</param>
    protected override string AppendSiteWhere(string where)
    {
        // Do not filter by site when showing all sites departments
        if (ShowAllSites)
        {
            return where;
        }

        return base.AppendSiteWhere(where);
    }


    private void TryInitByForm()
    {
        if ((Form == null) || !Form.AdditionalData.ContainsKey("DataClassID"))
        {
            return;
        }

        var dataClassId = ValidationHelper.GetInteger(Form.AdditionalData["DataClassID"], 0);
        var dataClass = DataClassInfoProvider.GetDataClassInfo(dataClassId);
        if (dataClass != null)
        {
            DepartmentInfo department = DepartmentInfoProvider.GetDepartmentInfo(dataClass.ClassSKUDefaultDepartmentName, SiteInfoProvider.GetSiteName(SiteID));

            // Do not preselect department if user is not authorized for this department
            if ((department == null) || !ECommerceContext.IsUserAuthorizedForDepartment(department.DepartmentID))
            {
                return;
            }

            Value = (UseNameForSelection) ? dataClass.ClassSKUDefaultDepartmentName : department.DepartmentID.ToString();
        }
    }

    #endregion
}