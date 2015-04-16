using System;

using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.MessageBoards;
using CMS.PortalControls;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DataEngine;

public partial class CMSWebParts_MessageBoards_MessageBoard : CMSAbstractWebPart
{
    #region "Private fields"

    private string mWebPartName = "";
    private BoardInfo mBoardObj = null;
    private UserInfo mCurrentUser = null;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Current web part name.
    /// </summary>
    private string WebPartName
    {
        get
        {
            if (mWebPartName == "")
            {
                mWebPartName = ValidationHelper.GetString(GetValue("WebPartControlID"), "");
                if (mWebPartName == "")
                {
                    // Fix for ASPX templates
                    mWebPartName = ID;
                }
            }

            return mWebPartName;
        }
    }


    /// <summary>
    /// Current user from site context.
    /// </summary>
    public new UserInfo CurrentUser
    {
        get
        {
            if (mCurrentUser == null)
            {
                mCurrentUser = MembershipContext.CurrentUserProfile;
            }

            return mCurrentUser;
        }
    }


    /// <summary>
    /// Current message board.
    /// </summary>
    private BoardInfo BoardObj
    {
        get
        {
            if (mBoardObj == null)
            {
                switch (BoardOwner.ToLowerCSafe())
                {
                    case "user":
                        // Get the current user info and obtain the related board info
                        if (CurrentUser != null)
                        {
                            mBoardObj = BoardInfoProvider.GetBoardInfoForUser(GetBoardName(WebPartName, "user"), CurrentUser.UserID);
                        }
                        break;

                    default:
                        // If the 'BoardType' is of 'document' type or isn't specified use the default way to get the board info
                        mBoardObj = BoardInfoProvider.GetBoardInfo(GetBoardName(WebPartName, ""), DocumentContext.CurrentPageInfo.DocumentID);
                        if (mBoardObj == null)
                        {
                            // Backward compatibility
                            mBoardObj = BoardInfoProvider.GetBoardInfo(WebPartName + "_doc_" + DocumentContext.CurrentPageInfo.NodeGUID, DocumentContext.CurrentPageInfo.DocumentID);
                        }
                        break;
                }
            }
            return mBoardObj;
        }
    }

    #endregion


    #region "Public properties - Messages"

    /// <summary>
    /// Transformation used to display board message text.
    /// </summary>
    public string MessageTransformation
    {
        get
        {
            return ValidationHelper.GetString(GetValue("MessageTransformation"), "");
        }
        set
        {
            SetValue("MessageTransformation", value);
        }
    }


    /// <summary>
    /// OrderBy clause used for sorting messages.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderBy"), "MessageInserted DESC");
        }
        set
        {
            msgBoard.OrderBy = value;
        }
    }


    /// <summary>
    /// No messages text.
    /// </summary>
    public string NoMessagesText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NoMessagesText"), "");
        }
        set
        {
            SetValue("NoMessagesText", value);
        }
    }

    #endregion


    #region "Public properties - Messages management"

   /// <summary>
    /// Indicates whether the EDIT button should be displayed.
    /// </summary>
    public bool ShowEdit
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowEdit"), false);
        }
        set
        {
            SetValue("ShowEdit", value);
        }
    }


    /// <summary>
    /// Indicates whether the DELETE button should be displayed.
    /// </summary>
    public bool ShowDelete
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowDelete"), false);
        }
        set
        {
            SetValue("ShowDelete", value);
        }
    }


    /// <summary>
    /// Indicates whether the APPROVE button should be displayed.
    /// </summary>
    public bool ShowApprove
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowApprove"), false);
        }
        set
        {
            SetValue("ShowApprove", value);
        }
    }


    /// <summary>
    /// Indicates whether the REJECT button should be displayed.
    /// </summary>
    public bool ShowReject
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowReject"), false);
        }
        set
        {
            SetValue("ShowReject", value);
        }
    }

    #endregion


    #region "Public properties - Form fields"

    /// <summary>
    /// Indicates if input field for entering user's name should be displayed.
    /// </summary>
    public bool ShowNameField
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("ShowNameField"), true);
        }
        set
        {
            this.SetValue("ShowNameField", value);
        }
    }


    /// <summary>
    /// Indicates if input field for entering user's URL should be displayed.
    /// </summary>
    public bool ShowURLField
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("ShowURLField"), true);
        }
        set
        {
            this.SetValue("ShowURLField", value);
        }
    }


    /// <summary>
    /// Indicates if input field for entering user's e-mail address should be displayed.
    /// </summary>
    public bool ShowEmailField
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("ShowEmailField"), true);
        }
        set
        {
            this.SetValue("ShowEmailField", value);
        }
    } 

    #endregion


    #region "Public properties - New board settings"

    /// <summary>
    /// Default board name.
    /// </summary>
    public string BoardDisplayName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BoardDisplayName"), "");
        }
        set
        {
            SetValue("BoardDisplayName", value);
        }
    }


    /// <summary>
    /// Default board moderators.
    /// </summary>
    public string BoardModerators
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BoardModerators"), "");
        }
        set
        {
            SetValue("BoardModerators", value);
        }
    }


    /// <summary>
    /// Default board authorized roles.
    /// </summary>
    public string BoardRoles
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BoardRoles"), "");
        }
        set
        {
            SetValue("BoardRoles", value);
        }
    }


    /// <summary>
    /// Indicates whether the subscriptions should be enabled.
    /// </summary>
    public bool BoardEnableSubscriptions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("BoardEnableSubscriptions"), false);
        }
        set
        {
            SetValue("BoardEnableSubscriptions", value);
        }
    }


    /// <summary>
    /// Board unsubscription URL.
    /// </summary>
    public string BoardUnsubscriptionUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BoardUnsubscriptionUrl"), "");
        }
        set
        {
            SetValue("BoardUnsubscriptionUrl", value);
        }
    }


    /// <summary>
    /// Board base URL.
    /// </summary>
    public string BoardBaseUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BoardBaseUrl"), "");
        }
        set
        {
            SetValue("BoardBaseUrl", value);
        }
    }


    /// <summary>
    /// Indicates whether board is opened.
    /// </summary>
    public bool BoardOpened
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("BoardOpened"), false);
        }
        set
        {
            SetValue("BoardOpened", value);
        }
    }


    /// <summary>
    /// Indicates type of board access.
    /// </summary>
    public SecurityAccessEnum BoardAccess
    {
        get
        {
            return (SecurityAccessEnum)ValidationHelper.GetInteger(GetValue("BoardAccess"), 0);
        }
        set
        {
            SetValue("BoardAccess", value);
        }
    }


    /// <summary>
    /// Indicates the board message post requires e-mail.
    /// </summary>
    public bool BoardRequireEmails
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("BoardRequireEmails"), false);
        }
        set
        {
            SetValue("BoardRequireEmails", value);
        }
    }


    /// <summary>
    /// Indicates whether the board is moderated.
    /// </summary>
    public bool BoardModerated
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("BoardModerated"), false);
        }
        set
        {
            SetValue("BoardModerated", value);
        }
    }


    /// <summary>
    /// Indicates whether the CAPTCHA should be used.
    /// </summary>
    public bool BoardUseCaptcha
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("BoardUseCaptcha"), false);
        }
        set
        {
            SetValue("BoardUseCaptcha", value);
        }
    }


    /// <summary>
    /// Board opened from.
    /// </summary>
    public DateTime BoardOpenedFrom
    {
        get
        {
            return ValidationHelper.GetDateTimeSystem(GetValue("BoardOpenedFrom"), DateTimeHelper.ZERO_TIME);
        }
        set
        {
            SetValue("BoardOpenedFrom", value);
        }
    }


    /// <summary>
    /// Board opened to.
    /// </summary>
    public DateTime BoardOpenedTo
    {
        get
        {
            return ValidationHelper.GetDateTimeSystem(GetValue("BoardOpenedTo"), DateTimeHelper.ZERO_TIME);
        }
        set
        {
            SetValue("BoardOpenedTo", value);
        }
    }


    /// <summary>
    /// Type of the message board indicating what kind of system object is allowed for posting messages.
    /// </summary>
    public string BoardOwner
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BoardOwner"), "");
        }
        set
        {
            SetValue("BoardOwner", value);
        }
    }

    #endregion


    #region "Public properties - Content rating"

    /// <summary>
    /// Enables/disables content rating
    /// </summary>
    public bool EnableContentRating
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableContentRating"), false);
        }
        set
        {
            SetValue("EnableContentRating", value);
        }
    }


    /// <summary>
    /// Gets or sets type of content rating scale.
    /// </summary>
    public string RatingType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RatingType"), "Stars");
        }
        set
        {
            SetValue("RatingType", value);
        }
    }


    /// <summary>
    /// Gets or sets max value/length of content rating scale
    /// </summary>
    public int MaxRatingValue
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRatingValue"), 10);
        }
        set
        {
            SetValue("MaxRatingValue", value);
        }
    }


    /// <summary>
    /// Indicates if it is allowed to add message without rating.
    /// </summary>
    public bool AllowEmptyRating
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("AllowEmptyRating"), true);
        }
        set
        {
            this.SetValue("AllowEmptyRating", value);
        }
    }


    /// <summary>
    /// If checked, users can rate only once here.
    /// </summary>
    public bool CheckIfUserRated
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("CheckIfUserRated"), false);
        }
        set
        {
            this.SetValue("CheckIfUserRated", value);
        }
    }


    #endregion


    #region "Public properties - Others"

    /// <summary>
    /// Indicates whether the permissions should be checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), false);
        }
        set
        {
            SetValue("CheckPermissions", value);
            msgBoard.BoardProperties.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Indicates whether the existing messages should be displayed to anonymous user.
    /// </summary>
    public bool EnableAnonymousRead
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableAnonymousRead"), false);
        }
        set
        {
            SetValue("EnableAnonymousRead", value);
        }
    }  


    /// <summary>
    /// Indicates whether logging activity is performed.
    /// </summary>
    public bool LogActivity
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("LogActivity"), false);
        }
        set
        {
            SetValue("LogActivity", value);
        }
    } 


    /// <summary>
    /// Gest or sest the cache item name.
    /// </summary>
    public override string CacheItemName
    {
        get
        {
            return base.CacheItemName;
        }
        set
        {
            base.CacheItemName = value;
            msgBoard.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, msgBoard.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            msgBoard.CacheDependencies = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache minutes.
    /// </summary>
    public override int CacheMinutes
    {
        get
        {
            return base.CacheMinutes;
        }
        set
        {
            base.CacheMinutes = value;
            msgBoard.CacheMinutes = value;
        }
    }

    #endregion


    protected void Page_Init(object sender, EventArgs e)
    {
        // Initialize the controls
        SetupControls();
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Ensure visibility of message board
        Visible = msgBoard.Visible;
    }


    #region "Private methods"

    /// <summary>
    /// Initializes the controls.
    /// </summary>
    private void SetupControls()
    {
        // If the web part shouldn't proceed further
        if (StopProcessing)
        {
            msgBoard.BoardProperties.StopProcessing = true;
            Visible = false;
        }
        else
        {
            // Set the message board transformation
            msgBoard.MessageTransformation = MessageTransformation;

            // Set sorting
            msgBoard.OrderBy = String.IsNullOrEmpty(OrderBy) ? "MessageInserted DESC" : OrderBy;

            // Set buttons
            msgBoard.BoardProperties.ShowApproveButton = ShowApprove;
            msgBoard.BoardProperties.ShowDeleteButton = ShowDelete;
            msgBoard.BoardProperties.ShowEditButton = ShowEdit;
            msgBoard.BoardProperties.ShowRejectButton = ShowReject;

            // Set fields
            msgBoard.BoardProperties.ShowNameField = ShowNameField;
            msgBoard.BoardProperties.ShowEmailField = ShowEmailField;
            msgBoard.BoardProperties.ShowURLField = ShowURLField;
            
            // Set rating
            msgBoard.BoardProperties.EnableContentRating = EnableContentRating;            
            msgBoard.BoardProperties.RatingType = RatingType;
            msgBoard.BoardProperties.MaxRatingValue = MaxRatingValue;
            msgBoard.BoardProperties.AllowEmptyRating = AllowEmptyRating;
            msgBoard.BoardProperties.CheckIfUserRated = CheckIfUserRated;

            // Set caching
            msgBoard.CacheItemName = CacheItemName;
            msgBoard.CacheMinutes = CacheMinutes;
            msgBoard.CacheDependencies = CacheDependencies;

            // Set web part only properties
            msgBoard.BoardProperties.BoardEnableAnonymousRead = EnableAnonymousRead;
            msgBoard.BoardProperties.CheckPermissions = CheckPermissions;
            msgBoard.NoMessagesText = NoMessagesText;

            // Use board properties
            if (BoardObj != null)
            {
                msgBoard.BoardProperties.BoardAccess = BoardObj.BoardAccess;
                msgBoard.BoardProperties.BoardName = BoardObj.BoardName;
                msgBoard.BoardProperties.BoardDisplayName = BoardObj.BoardDisplayName;

                msgBoard.BoardProperties.BoardUnsubscriptionUrl = BoardInfoProvider.GetUnsubscriptionUrl(BoardObj.BoardUnsubscriptionURL, SiteContext.CurrentSiteName);
                msgBoard.BoardProperties.BoardBaseUrl = (string.IsNullOrEmpty(BoardObj.BoardBaseURL)) ? ValidationHelper.GetString(SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSBoardBaseURL"), "") : BoardObj.BoardBaseURL;
                msgBoard.BoardProperties.BoardEnableSubscriptions = BoardObj.BoardEnableSubscriptions;
                msgBoard.BoardProperties.BoardOpened = BoardObj.BoardOpened;
                msgBoard.BoardProperties.BoardRequireEmails = BoardObj.BoardRequireEmails;
                msgBoard.BoardProperties.BoardModerated = BoardObj.BoardModerated;
                msgBoard.BoardProperties.BoardUseCaptcha = BoardObj.BoardUseCaptcha;
                msgBoard.BoardProperties.BoardOpenedFrom = BoardObj.BoardOpenedFrom;
                msgBoard.BoardProperties.BoardOpenedTo = BoardObj.BoardOpenedTo;
                msgBoard.MessageBoardID = BoardObj.BoardID;
            }
            // Use default properties
            else
            {
                // If the board is user and information on current user wasn't supplied hide the web part
                if (((BoardOwner == "user") && (CurrentUser == null)))
                {
                    if (!String.IsNullOrEmpty(NoMessagesText))
                    {
                        msgBoard.NoMessagesText = NoMessagesText;
                    }
                    Visible = false;
                }
                else
                {
                    // Default board- document related continue
                    msgBoard.BoardProperties.BoardAccess = BoardAccess;
                    msgBoard.BoardProperties.BoardOwner = BoardOwner;
                    msgBoard.BoardProperties.BoardName = GetBoardName(WebPartName, BoardOwner);

                    string boardDisplayName = null;
                    if (!String.IsNullOrEmpty(BoardDisplayName))
                    {
                        boardDisplayName = BoardDisplayName;
                    }
                    // Use predefined display name format
                    else
                    {
                        boardDisplayName = DocumentContext.CurrentPageInfo.GetDocumentName() + " (" + DocumentContext.CurrentPageInfo.DocumentNamePath + ")";
                    }
                    // Limit display name length
                    msgBoard.BoardProperties.BoardDisplayName = TextHelper.LimitLength(boardDisplayName, 250, "");

                    msgBoard.BoardProperties.BoardUnsubscriptionUrl = BoardInfoProvider.GetUnsubscriptionUrl(BoardUnsubscriptionUrl, SiteContext.CurrentSiteName);
                    msgBoard.BoardProperties.BoardBaseUrl = (string.IsNullOrEmpty(BoardBaseUrl)) ? ValidationHelper.GetString(SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSBoardBaseURL"), "") : BoardBaseUrl;
                    msgBoard.BoardProperties.BoardEnableSubscriptions = BoardEnableSubscriptions;
                    msgBoard.BoardProperties.BoardOpened = BoardOpened;
                    msgBoard.BoardProperties.BoardRequireEmails = BoardRequireEmails;
                    msgBoard.BoardProperties.BoardModerated = BoardModerated;
                    msgBoard.BoardProperties.BoardRoles = BoardRoles;
                    msgBoard.BoardProperties.BoardModerators = BoardModerators;
                    msgBoard.BoardProperties.BoardUseCaptcha = BoardUseCaptcha;
                    msgBoard.BoardProperties.BoardOpenedFrom = BoardOpenedFrom;
                    msgBoard.BoardProperties.BoardOpenedTo = BoardOpenedTo;
                    msgBoard.BoardProperties.BoardLogActivity = LogActivity;                              
                    msgBoard.MessageBoardID = 0;
                }
            }
        }
    }


    /// <summary>
    /// Returns board name according board type.
    /// </summary>
    /// <param name="webPartName">Name of the web part</param>
    /// <param name="boardOwner">Owner of the board</param>    
    private string GetBoardName(string webPartName, string boardOwner)
    {
        // Get type
        BoardOwnerTypeEnum type = BoardInfoProvider.GetBoardOwnerTypeEnum(boardOwner);

        // Get name
        switch (boardOwner.ToLowerCSafe())
        {
            case "user":
                return BoardInfoProvider.GetMessageBoardName(webPartName, type, CurrentUser.UserGUID.ToString());

            default:
                return BoardInfoProvider.GetMessageBoardName(webPartName, type, "");
        }
    }


    public override void ReloadData()
    {
        base.ReloadData();
        SetupControls();
    }

    #endregion
}