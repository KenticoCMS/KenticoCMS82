using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.EmailEngine;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSModules_EmailQueue_MassEmails_Recipients : CMSModalGlobalAdminPage
{
    #region "Protected variables"

    protected int emailId = 0;

    protected bool isArchive = false;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        emailId = QueryHelper.GetInteger("emailid", 0);
        isArchive = QueryHelper.GetBoolean("archive", false);

        gridElem.WhereCondition = "EmailID=" + emailId;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.OnBeforeDataReload += gridElem_OnBeforeDataReload;
        gridElem.OnAction += gridElem_OnAction;
        gridElem.Pager.ShowPageSize = false;
        gridElem.Pager.DefaultPageSize = 15;

        PageTitle.TitleText = GetString("emailqueue.sentdetails.title");
        // Header action initialization
        HeaderActions.AddAction(new HeaderAction
        {
            Text = GetString("emailqueue.queue.deleteselected"),
            OnClientClick = "if (!confirm(" + ScriptHelper.GetString(GetString("EmailQueue.DeleteSelectedRecipientConfirmation")) + ")) return false;",
            CommandName = "delete"
        });
        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }


    /// <summary>
    /// Remove selected recipients from mass e-mail.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        if (e.CommandName.EqualsCSafe("delete", true))
        {
            // Get list of selected users
            var list = gridElem.SelectedItems;
            if (list.Count > 0)
            {
                foreach (string userId in list)
                {
                    // Remove specific recipient
                    EmailUserInfoProvider.DeleteEmailUserInfo(emailId, ValidationHelper.GetInteger(userId, 0));
                }
                gridElem.ResetSelection();
                gridElem.Pager.UniPager.CurrentPage = 1;
                gridElem.ReloadData();
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Enable header action and show grid if the grid is not empty
        HeaderAction action = HeaderActions.ActionsList[0];
        if (action != null)
        {
            action.Enabled = gridElem.Visible = gridElem.GridView.Rows.Count > 0;
        }
    }

    #endregion


    #region "Grid events"

    protected void gridElem_OnBeforeDataReload()
    {
        // Hide status and last result columns in archive
        gridElem.NamedColumns["result"].Visible = gridElem.NamedColumns["status"].Visible = !isArchive;
    }


    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "userid":
                // Get user friendly name instead of id
                UserInfo ui = UserInfoProvider.GetUserInfo(ValidationHelper.GetInteger(parameter, 0));
                if (ui != null)
                {
                    return HTMLHelper.HTMLEncode(Functions.GetFormattedUserName(ui.UserName) + " (" + ui.Email + ")");
                }
                else
                {
                    return GetString("general.na");
                }
            case "result":
                return TextHelper.LimitLength(parameter.ToString(), 50);
            case "resulttooltip":
                return HTMLHelper.HTMLEncodeLineBreaks(parameter.ToString());
            case "status":
                return GetEmailStatus(parameter);
        }

        return null;
    }


    private void gridElem_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "delete":
                int userId = ValidationHelper.GetInteger(actionArgument, 0);
                if (userId > 0)
                {
                    EmailUserInfoProvider.DeleteEmailUserInfo(emailId, userId);
                }
                break;
        }
    }


    /// <summary>
    /// Gets the e-mail status.
    /// </summary>
    /// <param name="parameter">The parameter</param>
    /// <returns>E-mail status</returns>
    private string GetEmailStatus(object parameter)
    {
        switch ((EmailStatusEnum)parameter)
        {
            case EmailStatusEnum.Created:
                return GetString("emailstatus.created");

            case EmailStatusEnum.Waiting:
                return GetString("emailstatus.waiting");

            case EmailStatusEnum.Sending:
                return GetString("emailstatus.sending");

            case EmailStatusEnum.Archived:
                return GetString("general.archived");

            default:
                return string.Empty;
        }
    }

    #endregion
}