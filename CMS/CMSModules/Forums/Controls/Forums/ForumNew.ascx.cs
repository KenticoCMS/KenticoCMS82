using System;

using CMS.Forums;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.DataEngine;

public partial class CMSModules_Forums_Controls_Forums_ForumNew : CMSAdminEditControl
{
    #region "Variables"

    private int mGroupId;
    private int mForumId;
    private Guid mCommunityGroupGUID = Guid.Empty;
    private ForumGroupInfo mForumGroup;
    private int? mCommunityGroupID;
    private ForumInfo mForum;

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
    /// Gets or sets the ID of the group for which the new forum should be created.
    /// </summary>
    public int GroupID
    {
        get
        {
            return mGroupId;
        }
        set
        {
            mGroupId = value;
        }
    }


    public ForumGroupInfo ForumGroup
    {
        get
        {
            return mForumGroup ?? (mForumGroup = ForumGroupInfoProvider.GetForumGroupInfo(GroupID));
        }
    }

    /// <summary>
    /// Gets or sets the community group GUID.
    /// </summary>
    public Guid CommunityGroupGUID
    {
        get
        {
            return mCommunityGroupGUID;
        }
        set
        {
            mCommunityGroupGUID = value;
        }
    }


    /// <summary>
    /// Community group ID
    /// </summary>
    protected int CommunityGroupID
    {
        get
        {
            if (mCommunityGroupID == null)
            {
                BaseInfo groupInfo = null;

                if (CommunityGroupGUID != Guid.Empty)
                {
                    groupInfo = ModuleCommands.CommunityGetGroupInfoByGuid(CommunityGroupGUID);
                }
                else if ((ForumGroup != null) && (ForumGroup.GroupGroupID > 0))
                {
                    groupInfo = ModuleCommands.CommunityGetGroupInfo(ForumGroup.GroupGroupID);
                }

                mCommunityGroupID = (groupInfo != null) ? groupInfo.Generalized.ObjectID : 0;
            }

            return mCommunityGroupID.Value;
        }
    }


    /// <summary>
    /// Gets the ID of the forum which has been created using the control.
    /// </summary>
    public int ForumID
    {
        get
        {
            return mForumId;
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Forum object
    /// </summary>
    private ForumInfo Forum
    {
        get
        {
            return mForum ?? (mForum = new ForumInfo());
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Visible || StopProcessing)
        {
            EnableViewState = false;
        }

        // Code name is not editable in simple mode
        if (DisplayMode == ControlDisplayModeEnum.Simple)
        {
            plcCodeName.Visible = false;
            plcUseHtml.Visible = false;
        }

        // Set EditedObject to ensure correct breadcrumbs behavior
        EditedObject = Forum;

        txtForumDisplayName.IsLiveSite = IsLiveSite;
        txtForumDescription.IsLiveSite = IsLiveSite;

        // Control initializations
        rfvForumDisplayName.ErrorMessage = GetString("Forum_General.EmptyDisplayName");
        rfvForumName.ErrorMessage = GetString("Forum_General.EmptyCodeName");

        // Set strings for labels
        lblForumOpen.Text = GetString("Forum_Edit.ForumOpenLabel");
        lblForumLocked.Text = GetString("Forum_Edit.ForumLockedLabel");
        lblForumDisplayEmails.Text = GetString("Forum_Edit.ForumDisplayEmailsLabel");
        lblForumRequireEmail.Text = GetString("Forum_Edit.ForumRequireEmailLabel");
        lblForumDisplayName.Text = GetString("Forum_Edit.ForumDisplayNameLabel");
        lblForumName.Text = GetString("Forum_Edit.ForumNameLabel");
        lblForumModerated.Text = GetString("Forum_Edit.ForumModeratedLabel");
        lblUseHTML.Text = GetString("Forum_Edit.UseHtml");
        lblCaptcha.Text = GetString("Forum_Edit.useCaptcha");

        lblBaseUrl.Text = GetString("Forum_Edit.lblBaseUrl");
        lblUnsubscriptionUrl.Text = GetString("Forum_Edit.lblUnsubscriptionUrl");

        // Set strings for checkboxes
        chkInheritBaseUrl.Text = GetString("Forum_Edit.InheritBaseUrl");
        chkInheritUnsubscribeUrl.Text = GetString("Forum_Edit.InheritUnsupscriptionUrl");
        chkCaptcha.NotSetChoice.Text = chkForumDisplayEmails.NotSetChoice.Text =
        chkForumRequireEmail.NotSetChoice.Text = chkUseHTML.NotSetChoice.Text = GetString("Forum_Edit.InheritUnsupscriptionUrl");

        if (ForumGroup != null)
        {
            if (!IsLiveSite && !RequestHelper.IsPostBack())
            {
                ReloadData();
            }

            // Add onclick actions for 'inherit' checkboxes
            chkInheritUnsubscribeUrl.Attributes.Add("onclick", "SetInheritance('" + txtUnsubscriptionUrl.ClientID + "', '" + ForumGroup.GroupUnsubscriptionUrl + "', 'txt')");
            chkInheritBaseUrl.Attributes.Add("onclick", "SetInheritance('" + txtBaseUrl.ClientID + "','" + ForumGroup.GroupBaseUrl + "', 'txt')");
            chkCaptcha.SetDefaultValue(ForumGroup.GroupUseCAPTCHA);
            chkForumDisplayEmails.SetDefaultValue(ForumGroup.GroupDisplayEmails);
            chkForumRequireEmail.SetDefaultValue(ForumGroup.GroupRequireEmail);
            chkUseHTML.SetDefaultValue(ForumGroup.GroupHTMLEditor);

            // Create script for handle inherited values
            string script = @"
                function LoadDefault(clientId, inheritClientId)
                {
                    var objToDisable = document.getElementById(clientId);
                    var objToCheck = document.getElementById(inheritClientId);
                    if (objToDisable != null) {
                        objToDisable.disabled = true;
                        objToCheck.checked = true;
                    }
                }

                function SetInheritance(clientId, value, type)
                {
                    var obj = document.getElementById(clientId);
                    if (obj != null) {
                        if(obj.disabled)
                        {
                            obj.disabled = false;
                        }
                        else
                        {
                            obj.disabled = true;
                            if (type == 'txt') {
                                obj.value = value;
                            } else {
                                obj.checked = value;
                            }
                        }
                    }
                }";

            ltrScript.Text = ScriptHelper.GetScript(script);
        }

        // Show/hide URL textboxes
        plcBaseAndUnsubUrl.Visible = (DisplayMode != ControlDisplayModeEnum.Simple);

        // Base URL textbox enable or disable
        if (chkInheritBaseUrl.Checked)
        {
            txtBaseUrl.Text = ForumGroup.GroupBaseUrl;
            txtBaseUrl.Attributes.Add("disabled", "true");
        }
        else
        {
            txtBaseUrl.Attributes.Remove("disabled");
        }

        // Unsubscription URL textbox enable or disable
        if (chkInheritUnsubscribeUrl.Checked)
        {
            txtUnsubscriptionUrl.Text = ForumGroup.GroupUnsubscriptionUrl;
            txtUnsubscriptionUrl.Attributes.Add("disabled", "true");
        }
        else
        {
            txtUnsubscriptionUrl.Attributes.Remove("disabled");
        }
    }


    public override void ReloadData()
    {
        ClearForm();

        // Set properties
        txtUnsubscriptionUrl.Text = ForumGroup.GroupUnsubscriptionUrl;
        txtBaseUrl.Text = ForumGroup.GroupBaseUrl;
        chkUseHTML.InitFromThreeStateValue(ForumGroup,"GroupHTMLEditor");
        chkForumRequireEmail.InitFromThreeStateValue(ForumGroup,"GroupRequireEmail");
        chkForumDisplayEmails.InitFromThreeStateValue(ForumGroup,"GroupDisplayEmails");
        chkCaptcha.InitFromThreeStateValue(ForumGroup, "GroupUseCAPTCHA");

        txtBaseUrl.Attributes.Add("disabled", "true");
        txtUnsubscriptionUrl.Attributes.Add("disabled", "true");

        chkInheritBaseUrl.Checked = true;
        chkInheritUnsubscribeUrl.Checked = true;
    }


    /// <summary>
    /// Clears the form fields to default values.
    /// </summary>
    public override void ClearForm()
    {
        // Clears all textboxes
        txtBaseUrl.Text = "";
        txtForumDescription.Text = "";
        txtForumDisplayName.Text = "";
        txtForumName.Text = "";
        txtUnsubscriptionUrl.Text = "";

        // Uncheck all checkboxes
        chkCaptcha.Value = false;
        chkForumDisplayEmails.Value = false;
        chkForumModerated.Checked = false;
        chkForumOpen.Checked = true;
        chkForumLocked.Checked = false;
        chkForumRequireEmail.Value = false;
        chkInheritBaseUrl.Checked = true;
        chkInheritUnsubscribeUrl.Checked = true;
        chkUseHTML.Value = false;
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check MODIFY permission for forums
        if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
        {
            return;
        }

        string codeName = txtForumName.Text.Trim();

        // Get safe code name for simple display mode
        if (DisplayMode == ControlDisplayModeEnum.Simple)
        {
            codeName = ValidationHelper.GetCodeName(txtForumDisplayName.Text.Trim(), 50) + "_group_" + CommunityGroupGUID;
        }

        // Check required fields
        string errorMessage = new Validator().NotEmpty(txtForumDisplayName.Text, GetString("Forum_General.EmptyDisplayName")).NotEmpty(codeName, GetString("Forum_General.EmptyCodeName")).Result;

        if (errorMessage == String.Empty && !ValidationHelper.IsCodeName(codeName))
        {
            errorMessage = GetString("general.errorcodenameinidentifierformat");
        }

        if (String.IsNullOrEmpty(errorMessage))
        {
            if (SiteContext.CurrentSite != null)
            {
                // If forum with given name already exists show error message
                if (ForumInfoProvider.GetForumInfo(codeName, SiteContext.CurrentSiteID, CommunityGroupID) != null)
                {
                    ShowError(GetString("Forum_Edit.ForumAlreadyExists"));
                    return;
                }

                // Set properties to newly created object
                Forum.ForumSiteID = SiteContext.CurrentSite.SiteID;
                Forum.ForumIsLocked = chkForumLocked.Checked;
                Forum.ForumOpen = chkForumOpen.Checked;
                chkForumDisplayEmails.SetThreeStateValue(Forum,"ForumDisplayEmails");
                Forum.ForumDescription = txtForumDescription.Text.Trim();
                chkForumRequireEmail.SetThreeStateValue(Forum, "ForumRequireEmail");
                Forum.ForumDisplayName = txtForumDisplayName.Text.Trim();
                Forum.ForumName = codeName;
                Forum.ForumGroupID = mGroupId;
                Forum.ForumModerated = chkForumModerated.Checked;
                Forum.ForumAccess = 40000;
                Forum.ForumPosts = 0;
                Forum.ForumThreads = 0;
                Forum.ForumPostsAbsolute = 0;
                Forum.ForumThreadsAbsolute = 0;
                Forum.ForumOrder = 0;
                chkCaptcha.SetThreeStateValue(Forum,"ForumUseCAPTCHA");
                Forum.ForumCommunityGroupID = CommunityGroupID;

                // For simple display mode skip some properties
                if (DisplayMode != ControlDisplayModeEnum.Simple)
                {
                    Forum.ForumBaseUrl = txtBaseUrl.Text.Trim();
                    Forum.ForumUnsubscriptionUrl = txtUnsubscriptionUrl.Text.Trim();
                    chkUseHTML.SetThreeStateValue(Forum, "ForumHTMLEditor");

                    if (chkInheritBaseUrl.Checked)
                    {
                        Forum.ForumBaseUrl = null;
                    }

                    if (chkInheritUnsubscribeUrl.Checked)
                    {
                        Forum.ForumUnsubscriptionUrl = null;
                    }
                }

                // Check license
                if (ForumInfoProvider.LicenseVersionCheck(RequestContext.CurrentDomain, FeatureEnum.Forums, ObjectActionEnum.Insert))
                {
                    ForumInfoProvider.SetForumInfo(Forum);
                    mForumId = Forum.ForumID;
                    RaiseOnSaved();
                }
                else
                {
                    ShowError(GetString("LicenseVersionCheck.Forums"));
                }
            }
        }
        else
        {
            ShowError(errorMessage);
        }
    }
}