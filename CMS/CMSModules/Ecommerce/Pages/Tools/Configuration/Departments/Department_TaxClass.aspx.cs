using System;
using System.Data;

using CMS.Core;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.DataEngine;

public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_Departments_Department_TaxClass : CMSDepartmentsPage
{
    protected int mDepartmentId = 0;
    protected string mCurrentValues = string.Empty;
    protected DepartmentInfo mDepartmentInfoObj = null;


    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsMultiStoreConfiguration)
        {
            CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, "Ecommerce.GlobalDepartments.DefaultTaxClasses");
        }
        else
        {
            CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, "Configuration.Departments.TaxClasses");
        }

        mDepartmentId = QueryHelper.GetInteger("objectId", 0);
        if (mDepartmentId > 0)
        {
            mDepartmentInfoObj = DepartmentInfoProvider.GetDepartmentInfo(mDepartmentId);
            EditedObject = mDepartmentInfoObj;

            if (mDepartmentInfoObj != null)
            {
                CheckEditedObjectSiteID(mDepartmentInfoObj.DepartmentSiteID);

                // Get tax classes assigned to department
                DataSet ds = TaxClassInfoProvider.GetDepartmentTaxClasses(mDepartmentId);
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    mCurrentValues = TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], "TaxClassID"));
                }

                if (!RequestHelper.IsPostBack())
                {
                    uniSelector.Value = mCurrentValues;
                }
            }
        }

        uniSelector.OnSelectionChanged += uniSelector_OnSelectionChanged;
        uniSelector.WhereCondition = GetSelectorWhereCondition();
    }


    protected void uniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        SaveItems();
    }


    protected void SaveItems()
    {
        if (mDepartmentInfoObj == null)
        {
            return;
        }

        // Check permissions
        CheckConfigurationModification(mDepartmentInfoObj.DepartmentSiteID);

        // Remove old items
        string newValues = ValidationHelper.GetString(uniSelector.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, mCurrentValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                // Add all new items to user
                foreach (string item in newItems)
                {
                    int taxClassId = ValidationHelper.GetInteger(item, 0);
                    DepartmentTaxClassInfoProvider.RemoveTaxClassFromDepartment(taxClassId, mDepartmentId);
                }
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(mCurrentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                // Add all new items to user
                foreach (string item in newItems)
                {
                    int taxClassId = ValidationHelper.GetInteger(item, 0);
                    DepartmentTaxClassInfoProvider.AddTaxClassToDepartment(taxClassId, mDepartmentId);
                }
            }
        }

        // Show message
        ShowChangesSaved();
    }


    /// <summary>
    /// Returns where condition for uniselector. This condition filters records contained in currently selected values
    /// and site-specific records according to edited objects site ID.
    /// </summary>
    protected string GetSelectorWhereCondition()
    {
        // Select nothing
        string where = "";

        // Add records which are used by parent object
        if (!string.IsNullOrEmpty(mCurrentValues))
        {
            where = SqlHelper.AddWhereCondition(where, "TaxClassID IN (" + mCurrentValues.Replace(';', ',') + ")", "OR");
        }

        int taxSiteId = 0;
        // Add site specific records when editing site shipping option and not using global tax classes
        if ((mDepartmentInfoObj != null) && (mDepartmentInfoObj.DepartmentSiteID > 0))
        {
            if (!ECommerceSettings.UseGlobalTaxClasses(mDepartmentInfoObj.DepartmentSiteID))
            {
                taxSiteId = mDepartmentInfoObj.DepartmentSiteID;
            }
        }

        where = SqlHelper.AddWhereCondition(where, "ISNULL(TaxClassSiteID, 0) = " + taxSiteId, "OR");

        return where;
    }
}