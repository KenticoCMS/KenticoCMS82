using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.ProjectManagement;
using CMS.UIControls;
using CMS.Membership;


[assembly: RegisterCustomClass("ProjectUniGridExtender", typeof(ProjectUniGridExtender))]

/// <summary>
/// Extends Unigrids used for projects from Project management module with additional abilities.
/// </summary>
public class ProjectUniGridExtender : ControlExtender<UniGrid>
{
    #region "Variables"

    private string rowColor = null;
    private bool isGroupAdministrator;
    private bool hasGroupManagePermission;
    private int? mCommunityGroupId;

    #endregion


    #region "Properties"

    /// <summary>
    /// Community group ID.
    /// </summary>
    private int CommunityGroupID
    {
        get
        {
            if (!mCommunityGroupId.HasValue)
            {
                mCommunityGroupId = QueryHelper.GetInteger("parentobjectid", 0);
            }
            return mCommunityGroupId.Value;
        }
    }

    #endregion


    #region "life-cycle methods and event handlers"

    /// <summary>
    /// OnInit page event.
    /// </summary>
    public override void OnInit()
    {
        Control.OnExternalDataBound += Control_OnExternalDataBound;
        Control.GridView.RowDataBound += GridView_RowDataBound;

        if (CommunityGroupID != 0)
        {
            // Prepare permissions for external data bound event
            isGroupAdministrator = MembershipContext.AuthenticatedUser.IsGroupAdministrator(CommunityGroupID);
            hasGroupManagePermission = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.groups", "Manage");

            // Group project has special permission check - user has to have Read permission or be a group admin
            // The default permission check is suppressed in UIElement (using custom attribute)
            if (!isGroupAdministrator && !MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.groups", "Read"))
            {
                Control.StopProcessing = true;
                CMSPage.RedirectToAccessDenied("cms.groups", "Read");
            }
        }
    }


    /// <summary>
    /// Handles the RowDataBound event of the GridView control.
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">The <see cref="GridViewRowEventArgs"/>Instance containing the event data</param>
    private void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!String.IsNullOrEmpty(rowColor))
            {
                e.Row.Attributes.Add("style", "background-color: " + rowColor);
            }
        }
    }


    /// <summary>
    /// Control OnExternalDataBound event.
    /// </summary>
    private object Control_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView row = null;
        switch (sourceName.ToLowerCSafe())
        {
            case "delete":
            case "#delete":
                {
                    if (CommunityGroupID != 0)
                    {
                        // Group project has special permission check - user has to have Manage permission or be a group admin
                        // The default permission check is suppressed in UIElement (using custom attribute)
                        if (!isGroupAdministrator && !hasGroupManagePermission)
                        {
                            CMSGridActionButton newPostButton = ((CMSGridActionButton)sender);
                            newPostButton.Enabled = false;
                        }
                    }
                }
                break;
                
            case "projectprogress":
                row = (DataRowView)parameter;
                int progress = ValidationHelper.GetInteger(row["ProjectProgress"], 0);

                return ProjectTaskInfoProvider.GenerateProgressHtml(progress, true);

            case "statusicon":
                row = (DataRowView)parameter;
                string iconUrl = ValidationHelper.GetString(row["ProjectStatusIcon"], "");
                // Get row color
                rowColor = ValidationHelper.GetString(row["ProjectStatusColor"], "");

                if (!String.IsNullOrEmpty(iconUrl))
                {
                    string statusText = ValidationHelper.GetString(row["ProjectStatus"], "");
                    statusText = HTMLHelper.HTMLEncode(statusText);

                    return "<div class=\"ProjectStatusIcon\"><img alt=\"" + statusText + "\" title=\"" + statusText + "\" src=\"" + HTMLHelper.HTMLEncode(Control.GetImageUrl(iconUrl)) + "\" class=\"ProjectStatusIcon\"  /></div>";
                }
                return "";

        }

        return null;
    }

    #endregion

}
