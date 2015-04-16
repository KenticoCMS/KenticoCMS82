using System;
using System.Data;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.MessageBoards;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_MessageBoards_Controls_Boards_BoardSubscription : CMSAdminEditControl
{
    #region "Private variables"

    private BoardSubscriptionInfo mCurrentSubscription;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Currently edited subscriber.
    /// </summary>
    private BoardSubscriptionInfo CurrentSubscription
    {
        get
        {
            return mCurrentSubscription ?? (mCurrentSubscription = (SubscriptionID > 0) ? BoardSubscriptionInfoProvider.GetBoardSubscriptionInfo(SubscriptionID) : new BoardSubscriptionInfo());
        }
    }

    #endregion


    #region "Public properties"

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
    /// ID of the current board.
    /// </summary>
    public int BoardID
    {
        get;
        set;
    }


    /// <summary>
    /// ID of the current subscription.
    /// </summary>
    public int SubscriptionID
    {
        get;
        set;
    }


    /// <summary>
    /// ID of the current subscription.
    /// </summary>
    public int GroupID
    {
        get;
        set;
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // If control should be hidden save view state memory
        if (StopProcessing || !Visible)
        {
            EnableViewState = false;
        }

        // Initializes the controls
        SetupControls();

        // Reload data if necessary
        if (!URLHelper.IsPostback() && !IsLiveSite)
        {
            ReloadData();
        }

        if (!RequestHelper.IsPostBack())
        {
            chkSendConfirmationEmail.Checked = true;
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Reload data if is live site 
        if (!URLHelper.IsPostback() && IsLiveSite)
        {
            ReloadData();
        }
    }


    #region "General methods"

    /// <summary>
    /// Initializes the controls on the page.
    /// </summary>
    private void SetupControls()
    {
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "UpdateForm", ScriptHelper.GetScript("function UpdateForm() {return;}"));

        userSelector.SiteID = SiteContext.CurrentSiteID;
        userSelector.ShowSiteFilter = false;
        userSelector.IsLiveSite = IsLiveSite;

        int groupId = QueryHelper.GetInteger("groupid", 0);
        if (groupId > 0)
        {
            userSelector.GroupID = groupId;
        }
        else
        {
            userSelector.GroupID = GroupID;
        }

        // Initialize the labels
        radAnonymousSubscription.Text = GetString("board.subscription.anonymous");
        radRegisteredSubscription.Text = GetString("board.subscription.registered");

        lblEmailAnonymous.Text = GetString("general.email") + ResHelper.Colon;
        lblEmailRegistered.Text = GetString("general.email") + ResHelper.Colon;
        lblUserRegistered.Text = GetString("general.username") + ResHelper.Colon;
        rfvEmailAnonymous.ErrorMessage = GetString("board.subscription.noemail");
        rfvEmailRegistered.ErrorMessage = GetString("board.subscription.noemail");

        radRegisteredSubscription.CheckedChanged += radRegisteredSubscription_CheckedChanged;
        radAnonymousSubscription.CheckedChanged += radAnonymousSubscription_CheckedChanged;

        userSelector.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;

        ProcessDisabling(radAnonymousSubscription.Checked);
    }


    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        int userId = ValidationHelper.GetInteger(userSelector.Value, 0);
        if (userId > 0)
        {
            // Show users email
            UserInfo ui = UserInfoProvider.GetUserInfo(userId);
            if (ui != null)
            {
                txtEmailRegistered.Text = ui.Email;
            }
        }
    }


    public override void ReloadData()
    {
        ClearForm();

        // Set edited object
        EditedObject = CurrentSubscription;

        // Load existing subscription data
        if (CurrentSubscription.SubscriptionID > 0)
        {
            // If the subscription is related to the registered user
            if (CurrentSubscription.SubscriptionUserID > 0)
            {
                // Load data
                userSelector.Value = CurrentSubscription.SubscriptionUserID;
                txtEmailRegistered.Text = CurrentSubscription.SubscriptionEmail;

                radRegisteredSubscription.Checked = true;
                radAnonymousSubscription.Checked = false;

                ProcessDisabling(false);
            }
            else
            {
                // Load data
                txtEmailAnonymous.Text = CurrentSubscription.SubscriptionEmail;

                radAnonymousSubscription.Checked = true;
                radRegisteredSubscription.Checked = false;

                ProcessDisabling(true);
            }

            chkSendConfirmationEmail.Checked = false;
        }
        else
        {
            radAnonymousSubscription.Checked = true;
            radRegisteredSubscription.Checked = false;

            ProcessDisabling(true);
        }

        if (QueryHelper.GetBoolean("saved", false))
        {
            // Display info on success if subscription is edited
            ShowChangesSaved();
        }
    }


    /// <summary>
    /// Clears the form entries.
    /// </summary>
    public override void ClearForm()
    {
        radAnonymousSubscription.Checked = true;
        userSelector.Value = "";
        txtEmailAnonymous.Text = "";
        txtEmailRegistered.Text = "";
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// 
    /// </summary>
    private string ValidateForm()
    {
        string errMsg;
        string where;

        // Check if the entered e-mail is non-empty string and valid e-mail address
        if (radAnonymousSubscription.Checked)
        {
            errMsg = new Validator().NotEmpty(txtEmailAnonymous.Text, GetString("board.subscription.emailnotvalid")).IsEmail(txtEmailAnonymous.Text, GetString("board.subscription.emailnotvalid")).Result;
            where = "SubscriptionEmail ='" + SqlHelper.GetSafeQueryString(txtEmailAnonymous.Text, false) + "'";
        }
        else
        {
            errMsg = new Validator().NotEmpty(txtEmailRegistered.Text, GetString("board.subscription.emailnotvalid")).IsEmail(txtEmailRegistered.Text, GetString("board.subscription.emailnotvalid")).NotEmpty(userSelector.Value, GetString("board.subscription.emptyuser")).Result;
            where = "SubscriptionEmail ='" + SqlHelper.GetSafeQueryString(txtEmailRegistered.Text, false) + "'";
        }

        // Check if there is not the subscription for specified e-mail yet
        if (string.IsNullOrEmpty(errMsg))
        {
            DataSet ds = BoardSubscriptionInfoProvider.GetSubscriptions(where, null);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                // If existing subscription is the current one
                if ((ValidationHelper.GetInteger(ds.Tables[0].Rows[0]["SubscriptionID"], 0) != SubscriptionID) &&
                    (ValidationHelper.GetInteger(ds.Tables[0].Rows[0]["SubscriptionBoardID"], 0) == BoardID))
                {
                    errMsg = GetString("board.subscription.emailexists");
                }
            }
        }

        return errMsg;
    }


    /// <summary>
    /// Handles enabling/disabling appropriate controls
    /// </summary>
    private void ProcessDisabling(bool isTrue)
    {
        // Process registered user subscription displaying            
        lblEmailAnonymous.Enabled = isTrue;
        txtEmailAnonymous.Enabled = isTrue;
        rfvEmailAnonymous.Enabled = isTrue;

        lblUserRegistered.Enabled = !isTrue;
        lblEmailRegistered.Enabled = !isTrue;
        txtEmailRegistered.Enabled = !isTrue;
        rfvEmailRegistered.Enabled = !isTrue;
        userSelector.Enabled = !isTrue;
    }

    #endregion


    #region "Event handling"

    protected void radRegisteredSubscription_CheckedChanged(object sender, EventArgs e)
    {
        ProcessDisabling(!radRegisteredSubscription.Checked);
    }


    protected void radAnonymousSubscription_CheckedChanged(object sender, EventArgs e)
    {
        ProcessDisabling(radAnonymousSubscription.Checked);
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        if (!CheckPermissions("cms.messageboards", PERMISSION_MODIFY))
        {
            return;
        }

        string errMsg = ValidateForm();

        // If entered form was validated successfully
        if (string.IsNullOrEmpty(errMsg))
        {
            // Get data according the selected type
            if (radAnonymousSubscription.Checked)
            {
                CurrentSubscription.SubscriptionEmail = txtEmailAnonymous.Text;
                CurrentSubscription.SubscriptionUserID = 0;
            }
            else
            {
                CurrentSubscription.SubscriptionEmail = txtEmailRegistered.Text;
                CurrentSubscription.SubscriptionUserID = ValidationHelper.GetInteger(userSelector.Value, 0);
            }

            CurrentSubscription.SubscriptionBoardID = BoardID;

            // Save information on user
            BoardSubscriptionInfoProvider.SetBoardSubscriptionInfo(CurrentSubscription);
            if (chkSendConfirmationEmail.Checked)
            {
                BoardSubscriptionInfoProvider.SendConfirmationEmail(CurrentSubscription, true);
            }

            SubscriptionID = CurrentSubscription.SubscriptionID;

            RaiseOnSaved();

            // Display info on success if subscription is edited
            ShowChangesSaved();
        }
        else
        {
            // Inform user on error
            ShowError(errMsg);
        }
    }

    #endregion
}