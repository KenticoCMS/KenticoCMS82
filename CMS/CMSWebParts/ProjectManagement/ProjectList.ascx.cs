using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.PortalControls;
using CMS.UIControls;

public partial class CMSWebParts_ProjectManagement_ProjectList : CMSAbstractWebPart
{
    #region "Variables"

    private bool displayErrorMessage = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the permission for creating new project.
    /// </summary>
    public string ProjectAccess
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ProjectAccess"), ucProjectList.ProjectAccess);
        }
        set
        {
            SetValue("ProjectAccess", value);
            ucProjectList.ProjectAccess = value;
        }
    }


    /// <summary>
    /// Gest or sets the role names separated by semicolon which are authorized to create new project.
    /// </summary>
    public string AuthorizedRoles
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AuthorizedRoles"), ucProjectList.AuthorizedRoles);
        }
        set
        {
            SetValue("AuthorizedRoles", value);
            ucProjectList.AuthorizedRoles = value;
        }
    }


    /// <summary>
    /// Show finished projects too.
    /// </summary>
    public bool ShowFinishedProjects
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowFinishedProjects"), ucProjectList.ShowFinishedProjects);
        }
        set
        {
            SetValue("ShowFinishedProjects", value);
            ucProjectList.ShowFinishedProjects = value;
        }
    }


    /// <summary>
    /// If true records are paged.
    /// </summary>
    public bool EnablePaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnablePaging"), ucProjectList.EnablePaging);
        }
        set
        {
            SetValue("EnablePaging", value);
            ucProjectList.EnablePaging = value;
        }
    }


    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), ucProjectList.PageSize);
        }
        set
        {
            SetValue("PageSize", value);
            ucProjectList.PageSize = value;
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


    /// <summary>
    /// Setup control.
    /// </summary>
    protected void SetupControl()
    {
        // Do nothing if stop processing is enabled
        if (StopProcessing)
        {
            Visible = false;
            return;
        }

        ucProjectList.DisplayMode = ControlDisplayModeEnum.Simple;
        ucProjectList.ShowFinishedProjects = ShowFinishedProjects;
        ucProjectList.EnablePaging = EnablePaging;
        ucProjectList.PageSize = PageSize;
        ucProjectList.IsLiveSite = true;
        ucProjectList.ProjectAccess = ProjectAccess;
        ucProjectList.AuthorizedRoles = AuthorizedRoles;
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Render override.
    /// </summary>
    /// <param name="writer">Writer</param>
    protected override void Render(HtmlTextWriter writer)
    {
        if (!displayErrorMessage)
        {
            messageElem.Visible = false;
        }

        base.Render(writer);
    }

    #endregion
}