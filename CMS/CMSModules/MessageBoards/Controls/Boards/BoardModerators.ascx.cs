using System;
using System.Data;

using CMS.Helpers;
using CMS.Membership;
using CMS.MessageBoards;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.DataEngine;

public partial class CMSModules_MessageBoards_Controls_Boards_BoardModerators : CMSAdminEditControl
{
    #region "Variables"

    protected int mBoardID = 0;
    protected BoardInfo mBoard = null;
    private string currentValues = String.Empty;
    private bool canModify = false;

    private bool mShouldReloadData = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
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
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// ID of the current message board.
    /// </summary>
    public int BoardID
    {
        get
        {
            if (mBoard != null)
            {
                return mBoard.BoardID;
            }

            return mBoardID;
        }
        set
        {
            mBoardID = value;

            mBoard = null;
        }
    }


    /// <summary>
    /// Current message board info object.
    /// </summary>
    public BoardInfo Board
    {
        get
        {
            return mBoard ?? (mBoard = BoardInfoProvider.GetBoardInfo(BoardID));
        }
        set
        {
            mBoard = value;

            mBoardID = 0;
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Indicates whether the data should be reloaded on PreRender.
    /// </summary>
    private bool ShouldReloadData
    {
        get
        {
            return mShouldReloadData;
        }
        set
        {
            mShouldReloadData = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script for pendingCallbacks repair
        ScriptHelper.FixPendingCallbacks(Page);

        // Initializes the controls
        SetupControls();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Reload data if necessary
        if (ShouldReloadData || (!URLHelper.IsPostback() && !IsLiveSite))
        {
            currentValues = "";
            userSelector.CurrentValues = GetModerators();

            ReloadData();
        }

        if (Board != null)
        {
            userSelector.Enabled = Board.BoardModerated && canModify;
            chkBoardModerated.Enabled = canModify;
        }
    }


    private void SetupControls()
    {
        // Get resource strings
        userSelector.CurrentSelector.OnSelectionChanged += new EventHandler(CurrentSelector_OnSelectionChanged);

        if (BoardID > 0)
        {
            EditedObject = Board;
        }

        if (Board != null)
        {
            // Check permissions
            if (Board.BoardGroupID > 0)
            {
                canModify = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.groups", CMSAdminControl.PERMISSION_MANAGE);
            }
            else
            {
                canModify = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.messageboards", CMSAdminControl.PERMISSION_MODIFY);
            }

            userSelector.BoardID = BoardID;
            userSelector.GroupID = Board.BoardGroupID;
            userSelector.CurrentSelector.SelectionMode = SelectionModeEnum.Multiple;
            userSelector.ShowSiteFilter = false;
            userSelector.SiteID = SiteContext.CurrentSiteID;
            userSelector.CurrentValues = GetModerators();
            userSelector.IsLiveSite = IsLiveSite;
        }
    }


    /// <summary>
    /// Reloads form data.
    /// </summary>
    public override void ReloadData()
    {
        ReloadData(true);
    }


    /// <summary>
    /// Reloads form data.
    /// </summary>
    public override void ReloadData(bool forceReload)
    {
        base.ReloadData(forceReload);

        // Get board info
        if (Board != null)
        {
            chkBoardModerated.Checked = Board.BoardModerated;
            if (forceReload)
            {
                if (!String.IsNullOrEmpty(currentValues))
                {
                    string where = SqlHelper.AddWhereCondition(userSelector.CurrentSelector.WhereCondition, "UserID IN (" + currentValues.Replace(';', ',') + ")", "OR");
                    userSelector.CurrentSelector.WhereCondition = where;
                }

                userSelector.CurrentSelector.Value = GetModerators();
                userSelector.ReloadData();
            }
        }
    }


    /// <summary>
    /// Returns ID of users who are moderators to this board.
    /// </summary>
    protected string GetModerators()
    {
        if (String.IsNullOrEmpty(currentValues))
        {
            // Get all message board moderators
            DataSet ds = BoardModeratorInfoProvider.GetBoardModerators(BoardID, "UserID", null, null, 0);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                currentValues = TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], "UserID"));
            }
        }

        return currentValues;
    }


    /// <summary>
    /// Board moderated checkbox change.
    /// </summary>
    protected void chkBoardModerated_CheckedChanged(object sender, EventArgs e)
    {
        if (!canModify)
        {
            return;
        }

        if (Board != null)
        {
            Board.BoardModerated = chkBoardModerated.Checked;
            BoardInfoProvider.SetBoardInfo(Board);

            ShowChangesSaved();

            ShouldReloadData = true;
        }
    }


    private void CurrentSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        ShouldReloadData = true;
    }
}