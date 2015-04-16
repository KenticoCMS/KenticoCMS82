using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalControls;
using CMS.ProjectManagement;
using CMS.Base;
using CMS.SiteProvider;

public partial class CMSWebParts_ProjectManagement_MyProjects : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Show finished projects too.
    /// </summary>
    public bool ShowFinishedProjects
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowFinishedProjects"), false);
        }
        set
        {
            SetValue("ShowFinishedProjects", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether paging should be used.
    /// </summary>
    public bool EnablePaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnablePaging"), false);
        }
        set
        {
            SetValue("EnablePaging", value);
        }
    }


    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), 10);
        }
        set
        {
            SetValue("PageSize", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// On content loaded.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Setup control.
    /// </summary>
    protected void SetupControl()
    {
        if (!StopProcessing)
        {
            // Ignore community groups
            ucProjectList.IgnoreCommunityGroup = true;

            // Build condition event handler
            ucProjectList.BuildCondition += new CMSModules_ProjectManagement_Controls_UI_Project_List.BuildConditionEvent(ucProjectList_BuildCondition);

            ucProjectList.RegisterEditScript = false;
            ucProjectList.Grid.GridName = "~/CMSModules/ProjectManagement/Controls/UI/Project/MyProjects.xml";
            ucProjectList.IsLiveSite = true;

            ucProjectList.EnablePaging = EnablePaging;
            ucProjectList.PageSize = PageSize;
        }
        else
        {
            ucProjectList.StopProcessing = true;
            ucProjectList.Visible = false;
        }
    }


    /// <summary>
    /// Builds where condition.
    /// </summary>
    private string ucProjectList_BuildCondition(object sender, string whereCondition)
    {
        // Do not display projects without relation to document
        whereCondition = SqlHelper.AddWhereCondition(whereCondition, "ProjectNodeID IS NOT NULL");

        // Finished projects
        if (!ShowFinishedProjects)
        {
            whereCondition = SqlHelper.AddWhereCondition(whereCondition, "StatusIsFinished=0");
        }

        // Security condition
        whereCondition = ProjectInfoProvider.CombineSecurityWhereCondition(whereCondition, MembershipContext.AuthenticatedUser, SiteContext.CurrentSiteName);

        return whereCondition;
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }

    #endregion
}