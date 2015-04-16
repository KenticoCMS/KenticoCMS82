using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.EventLog;
using CMS.Helpers;
using CMS.Base;
using CMS.Membership;
using CMS.WorkflowEngine;
using CMS.UIControls;
using CMS.DataEngine;
using CMS.ExtendedControls;

public partial class CMSModules_Content_Controls_VersionList : VersionHistoryControl
{
    #region "Variables"

    private Label mInfoLabel;
    private Label mErrorLabel;
    private bool mDisplaySecurityMessage;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets information label.
    /// </summary>
    public Label InfoLabel
    {
        get
        {
            return mInfoLabel ?? (mInfoLabel = MessagesPlaceHolder.InfoLabel);
        }
        set
        {
            mInfoLabel = value;
        }
    }


    /// <summary>
    /// Gets or sets error label.
    /// </summary>
    public Label ErrorLabel
    {
        get
        {
            return mErrorLabel ?? (mErrorLabel = MessagesPlaceHolder.ErrorLabel);
        }
        set
        {
            mErrorLabel = value;
        }
    }


    /// <summary>
    /// Gets version list heading.
    /// </summary>
    public LocalizedHeading Heading
    {
        get
        {
            return headHistory;
        }
    }


    /// <summary>
    /// Indicates whether to display security message.
    /// </summary>
    public bool DisplaySecurityMessage
    {
        get
        {
            return mDisplaySecurityMessage;
        }
        set
        {
            mDisplaySecurityMessage = value;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            gridHistory.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override CMS.ExtendedControls.MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }

    #endregion


    #region "Delegates and events"

    public event EventHandler AfterDestroyHistory = null;

    #endregion


    #region "Page events and methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            gridHistory.StopProcessing = true;
        }
        else
        {
            SetupControl();
        }
    }


    public void SetupControl()
    {
        gridHistory.ZeroRowsText = GetString("workflowproperties.documenthasnohistory");
        gridHistory.IsLiveSite = IsLiveSite;
        if (Node != null)
        {
            // Prepare the query parameters
            WhereCondition whereCondition = new WhereCondition().WhereEquals("DocumentID", Node.DocumentID);
            gridHistory.QueryParameters = whereCondition.Parameters;
            gridHistory.WhereCondition = whereCondition.WhereCondition;

            ScriptHelper.RegisterStartupScript(this, typeof(string), "confirmDestroyMessage", ScriptHelper.GetScript("var varConfirmDestroy='" + ResHelper.GetString("VersionProperties.ConfirmDestroy") + "'; \n"));

            gridHistory.GridName = "~/CMSModules/Content/Controls/VersionHistory.xml";
            gridHistory.OnExternalDataBound += gridHistory_OnExternalDataBound;
            gridHistory.OnAction += gridHistory_OnAction;

            string viewVersionUrl;
            if (IsLiveSite)
            {
                viewVersionUrl = AuthenticationHelper.ResolveDialogUrl("~/CMSModules/Content/CMSPages/Versions/ViewVersion.aspx");
            }
            else
            {
                viewVersionUrl = ResolveUrl("~/CMSModules/Content/CMSDesk/Properties/ViewVersion.aspx");
            }

            string viewVersionScript = ScriptHelper.GetScript("function ViewVersion(versionHistoryId) {window.open('" + viewVersionUrl + "?versionHistoryId=' + versionHistoryId)}");
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "viewVersionScript", viewVersionScript);
        }
        else
        {
            gridHistory.GridName = string.Empty;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (Node != null)
        {
            btnDestroy.Visible = !gridHistory.IsEmpty;
            btnDestroy.Enabled = (CanDestroy && (!CheckedOutByAnotherUser || CanCheckIn));
            if (DisplaySecurityMessage && !CanModify)
            {
                ShowInformation(String.Format(GetString("cmsdesk.notauthorizedtoeditdocument"), Node.NodeAliasPath));
            }
            plcLabels.Visible = !(string.IsNullOrEmpty(MessagesPlaceHolder.ErrorLabel.Text) && string.IsNullOrEmpty(MessagesPlaceHolder.InfoLabel.Text));
        }
    }


    /// <summary>
    /// Reloads data in unigrid.
    /// </summary>
    public void ReloadData()
    {
        gridHistory.ReloadData();
    }

    #endregion


    #region "Grid events"

    protected object gridHistory_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        sourceName = sourceName.ToLowerCSafe();
        switch (sourceName)
        {
            case "rollback":
                {
                    CMSGridActionButton imgRollback = ((CMSGridActionButton)sender);
                    int versionId = ValidationHelper.GetInteger(imgRollback.CommandArgument, 0);
                    if (!WorkflowManager.CheckStepPermissions(Node, WorkflowActionEnum.Approve) || !CanModify || (CheckedOutByAnotherUser && !CanCheckIn) || (versionId == Node.DocumentCheckedOutVersionHistoryID))
                    {
                        imgRollback.Enabled = false;
                    }
                }
                break;

            case "allowdestroy":
                {
                    CMSGridActionButton imgDestroy = ((CMSGridActionButton)sender);
                    int versionId = ValidationHelper.GetInteger(imgDestroy.CommandArgument, 0);
                    if (!CanDestroy || (CheckedOutByAnotherUser && !CanCheckIn) || (versionId == Node.DocumentCheckedOutVersionHistoryID))
                    {
                        imgDestroy.Enabled = false;
                    }
                }
                break;

            case "modifiedwhenby":
                DataRowView data = (DataRowView)parameter;
                DateTime modifiedwhen = ValidationHelper.GetDateTime(data["ModifiedWhen"], DateTimeHelper.ZERO_TIME);
                int userId = ValidationHelper.GetInteger(data["ModifiedByUserID"], 0);

                var tr = new ObjectTransformation("cms.user", userId);
                tr.EncodeOutput = false;
                tr.Transformation = string.Format("{0} <br /> {{% Object.GetFormattedUserName()|(encode) %}}", UniGridFunctions.UserDateTimeGMT(modifiedwhen));
                return tr;
        }

        return parameter;
    }


    protected void gridHistory_OnAction(string actionName, object actionArgument)
    {
        int versionHistoryId = ValidationHelper.GetInteger(actionArgument, 0);
        switch (actionName.ToLowerCSafe())
        {
            case "rollback":
                if (versionHistoryId > 0)
                {
                    if (CheckedOutByUserID > 0)
                    {
                        // Document is checked out
                        ShowError(GetString("VersionProperties.CannotRollbackCheckedOut"));
                    }
                    else
                    {
                        // Check permissions
                        if (!WorkflowManager.CheckStepPermissions(Node, WorkflowActionEnum.Approve) || !CanModify || (CheckedOutByAnotherUser && !CanCheckIn) || (versionHistoryId == Node.DocumentCheckedOutVersionHistoryID))
                        {
                            ShowError(String.Format(GetString("cmsdesk.notauthorizedtoeditdocument"), Node.NodeAliasPath));
                        }
                        else
                        {
                            try
                            {
                                VersionManager.RollbackVersion(versionHistoryId);

                                if (!IsLiveSite)
                                {
                                    // Refresh content tree (for the case that document name has been changed)
                                    string refreshTreeScript = ScriptHelper.GetScript("if(window.RefreshTree!=null){RefreshTree(" + Node.NodeParentID + ", " + Node.NodeID + ");}");
                                    ScriptHelper.RegisterStartupScript(this, typeof(string), "refreshTree" + ClientID, refreshTreeScript);
                                }

                                // Refresh node instance
                                InvalidateNode();
                                ShowConfirmation(GetString("VersionProperties.RollbackOK"));
                            }
                            catch (Exception ex)
                            {
                                ShowError(ex.Message);
                            }
                        }
                    }
                }
                break;

            case "destroy":
                if (versionHistoryId > 0)
                {
                    if (Node != null)
                    {
                        // Check permissions
                        if (!CanDestroy || (CheckedOutByAnotherUser && !CanCheckIn) || (versionHistoryId == Node.DocumentCheckedOutVersionHistoryID))
                        {
                            ShowError(GetString("History.ErrorNotAllowedToDestroy"));
                        }
                        else
                        {
                            // Refresh node instance
                            InvalidateNode();
                            VersionManager.DestroyDocumentVersion(versionHistoryId);
                            ShowConfirmation(GetString("VersionProperties.DestroyOK"));
                        }
                    }
                }
                break;
        }
    }

    #endregion


    #region "Button handling"

    protected void btnDestroy_Click(object sender, EventArgs e)
    {
        if (Node != null)
        {
            // Check permissions
            if (!CanDestroy || (CheckedOutByAnotherUser && !CanCheckIn))
            {
                ShowError(GetString("History.ErrorNotAllowedToDestroy"));
                return;
            }
            VersionManager.ClearDocumentHistory(Node.DocumentID);
            ShowConfirmation(GetString("VersionProperties.VersionsCleared"));

            EventLogProvider.LogEvent(EventType.INFORMATION, "Content", "DESTROYHISTORY", string.Format(ResHelper.GetAPIString("contentedit.documenthistorydestroyed", "History of the page '{0}' has been destroyed."), HTMLHelper.HTMLEncode(Node.NodeAliasPath)), RequestContext.RawURL, TreeProvider.UserInfo.UserID, TreeProvider.UserInfo.UserName, Node.NodeID, Node.GetDocumentName(), RequestContext.UserHostAddress, Node.NodeSiteID);

            InvalidateNode();
            ReloadData();
            if (AfterDestroyHistory != null)
            {
                AfterDestroyHistory(sender, e);
            }
        }
    }

    #endregion
}