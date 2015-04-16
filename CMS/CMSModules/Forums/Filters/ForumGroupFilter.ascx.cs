using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.Controls;
using CMS.Forums;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_Forums_Filters_ForumGroupFilter : CMSAbstractBaseFilterControl
{
    #region "Variables"

    private CMSUserControl filteredControl;

    #endregion


    #region "Properties"

    /// <summary>
    /// Where condition.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            int selectedGroup = ValidationHelper.GetInteger(forumGroupSelector.Value, 0);
            base.WhereCondition = GenerateWhereCondition(selectedGroup);
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }


    /// <summary>
    /// Filter value.
    /// </summary>
    public override object Value
    {
        get
        {
            return forumGroupSelector.Value;
        }
        set
        {
            forumGroupSelector.Value = value;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        filteredControl = FilteredControl as CMSUserControl;
        forumGroupSelector.UniSelector.ReturnColumnName = "GroupID";
        forumGroupSelector.UniSelector.OnSelectionChanged += new EventHandler(UniSelector_OnSelectionChanged);
        forumGroupSelector.DropDownSingleSelect.AutoPostBack = true;

        // Set group selector dropdow to show only forumgroups of the site
        int siteId = ValidationHelper.GetInteger(filteredControl.GetValue("SiteID"), 0);
        forumGroupSelector.SiteId = siteId;

        // Get first forum group to be able to filter uni selector
        if (!RequestHelper.IsPostBack())
        {
            if (siteId > 0)
            {
                DataSet defaultGroup = ForumGroupInfoProvider.GetGroups("GroupGroupID IS NULL AND GroupName NOT LIKE 'AdHoc%' AND  GroupSiteID=" + siteId, "GroupDisplayName", 1, "GroupID");
                if (!DataHelper.DataSourceIsEmpty(defaultGroup))
                {
                    int defaultGroupId = ValidationHelper.GetInteger(defaultGroup.Tables[0].Rows[0]["GroupID"], 0);
                    if (defaultGroupId > 0)
                    {
                        forumGroupSelector.UniSelector.Value = defaultGroupId;
                        WhereCondition = GenerateWhereCondition(defaultGroupId);
                    }
                }
                ;
            }
        }
    }


    private void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        RaiseOnFilterChanged();
    }


    /// <summary>
    /// Generates where condition.
    /// </summary>
    /// <param name="forumGroupId">Selected forum group id</param>
    protected string GenerateWhereCondition(int forumGroupId)
    {
        return "ForumGroupID IN (SELECT GroupID FROM Forums_ForumGroup WHERE forumGroupId = " + forumGroupId + ")";
    }

    #endregion
}