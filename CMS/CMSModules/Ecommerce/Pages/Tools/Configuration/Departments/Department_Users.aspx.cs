using System;
using System.Linq;

using CMS.Core;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DataEngine;

public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_Departments_Department_Users : CMSDepartmentsPage
{
    protected int mDepartmentId = 0;
    protected string mCurrentValues = string.Empty;
    protected DepartmentInfo mDepartmentInfoObj = null;


    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsMultiStoreConfiguration)
        {
            CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, "Ecommerce.GlobalDepartments.Users");
        }
        else
        {
            CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, "Configuration.Departments.Users");
        }

        mDepartmentId = QueryHelper.GetInteger("objectId", 0);
        if (mDepartmentId > 0)
        {
            mDepartmentInfoObj = DepartmentInfoProvider.GetDepartmentInfo(mDepartmentId);
            EditedObject = mDepartmentInfoObj;

            if (mDepartmentInfoObj != null)
            {
                CheckEditedObjectSiteID(mDepartmentInfoObj.DepartmentSiteID);

                // Get the active users
                string where = "UserID IN (SELECT UserID FROM COM_UserDepartment WHERE DepartmentID = " + mDepartmentId + ")";
                var data = UserInfoProvider.GetUsers().Where(where).Columns("UserID");
                if (data.Any())
                {
                    mCurrentValues = TextHelper.Join(";", DataHelper.GetStringValues(data.Tables[0], "UserID"));
                }

                if (!RequestHelper.IsPostBack())
                {
                    uniSelector.Value = mCurrentValues;
                }
            }
        }

        uniSelector.OnSelectionChanged += uniSelector_OnSelectionChanged;
        uniSelector.WhereCondition = GetWhereCondition();
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
                    int userId = ValidationHelper.GetInteger(item, 0);
                    UserDepartmentInfoProvider.RemoveUserFromDepartment(mDepartmentId, userId);
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
                    int userId = ValidationHelper.GetInteger(item, 0);
                    UserDepartmentInfoProvider.AddUserToDepartment(mDepartmentId, userId);
                }
            }
        }

        // Show message
        ShowChangesSaved();
    }


    /// <summary>
    /// Creates where conditin for user selector.
    /// </summary>
    protected string GetWhereCondition()
    {
        string where = "UserID IN (SELECT UserID FROM CMS_UserSite WHERE SiteID = " + SiteContext.CurrentSiteID + ")";

        where = SqlHelper.AddWhereCondition(where, UserInfoProvider.USER_ENABLED_WHERE_CONDITION);

        // Include selected values
        if (!string.IsNullOrEmpty(mCurrentValues))
        {
            string[] usersIds = mCurrentValues.Split(';');
            int[] intUsersIds = ValidationHelper.GetIntegers(usersIds, 0);

            where = SqlHelper.AddWhereCondition(where, SqlHelper.GetWhereCondition("UserID", intUsersIds), "OR");
        }

        return where;
    }
}