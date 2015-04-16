using System;
using System.Web;
using System.Web.UI;
using CMS.Community;
using CMS.EmailEngine;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.WebAnalytics;
using CMS.DataEngine;

public partial class CMSWebParts_Community_Membership_GroupInvitation : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether to send default group e-mails.
    /// </summary>
    public bool SendDefaultGroupEmails
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SendDefaultGroupEmails"), false);
        }
        set
        {
            SetValue("SendDefaultGroupEmails", value);
        }
    }


    ///// <summary>
    ///// Gets or sets the value that indicates whether to send e-mail to inviter.
    ///// </summary>
    public bool SendEmailToInviter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SendEmailToInviter"), false);
        }
        set
        {
            SetValue("SendEmailToInviter", value);
        }
    }


    /// <summary>
    /// Gets or sets logon page URL.
    /// </summary>
    public string LoginURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LoginURL"), string.Empty);
        }
        set
        {
            SetValue("LoginURL", value);
        }
    }


    #region "Caption properties"

    public string InvitationNoLongerExists
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("InvitationNoLongerExists"), GetString("group.invitationnolongerexists"));
        }
    }


    public string InvitationIsNotValid
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("InvitationIsNotValid"), GetString("group.invitationisnotvalid"));
        }
    }


    public string GroupNoLongerExists
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("GroupNoLongerExists"), GetString("group.nolongerexists"));
        }
    }


    public string MemberJoined
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("MemberJoined"), GetString("groups.memberjoined"));
        }
    }


    public string MemberWaiting
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("MemberWaiting"), GetString("groups.memberjoinedwaiting"));
        }
    }


    public string UserIsAlreadyMember
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("UserIsAlreadyMember"), GetString("groups.userisalreadymember"));
        }
    }

    #endregion


    #endregion


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
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
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            if (!RequestHelper.IsPostBack())
            {
                // If user is public
                if (MembershipContext.AuthenticatedUser.IsPublic())
                {
                    // Get logon URL
                    string logonUrl = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSSecuredAreasLogonPage");
                    logonUrl = DataHelper.GetNotEmpty(LoginURL, logonUrl);

                    // Create redirect URL
                    logonUrl = URLHelper.ResolveUrl(logonUrl) + "?ReturnURL=" + HttpUtility.UrlEncode(RequestContext.CurrentURL);
                    URLHelper.Redirect(logonUrl);
                }
                else
                {
                    // Get invitation by GUID
                    Guid invitationGuid = QueryHelper.GetGuid("invitationguid", Guid.Empty);
                    if (invitationGuid != Guid.Empty)
                    {
                        InvitationInfo invitation = InvitationInfoProvider.GetInvitationInfo(invitationGuid);
                        if (invitation != null)
                        {
                            // Check if invitation is valid
                            if ((invitation.InvitationValidTo == DateTimeHelper.ZERO_TIME) ||
                                (invitation.InvitationValidTo >= DateTime.Now))
                            {
                                GroupInfo group = GroupInfoProvider.GetGroupInfo(invitation.InvitationGroupID);

                                if (group != null)
                                {
                                    // Check whether current user is the user who should be invited
                                    if ((invitation.InvitedUserID > 0) && (invitation.InvitedUserID != MembershipContext.AuthenticatedUser.UserID))
                                    {
                                        lblInfo.CssClass = "InvitationErrorLabel";
                                        lblInfo.Text = InvitationIsNotValid;
                                        lblInfo.Visible = true;
                                        return;
                                    }

                                    // If user was invited by e-mail
                                    if (invitation.InvitedUserID == 0)
                                    {
                                        invitation.InvitedUserID = MembershipContext.AuthenticatedUser.UserID;
                                    }

                                    if (!GroupMemberInfoProvider.IsMemberOfGroup(invitation.InvitedUserID, invitation.InvitationGroupID))
                                    {
                                        // Create group member info object
                                        GroupMemberInfo groupMember = new GroupMemberInfo();
                                        groupMember.MemberInvitedByUserID = invitation.InvitedByUserID;
                                        groupMember.MemberUserID = MembershipContext.AuthenticatedUser.UserID;
                                        groupMember.MemberGroupID = invitation.InvitationGroupID;
                                        groupMember.MemberJoined = DateTime.Now;

                                        // Set proper status depending on grouo settings
                                        switch (group.GroupApproveMembers)
                                        {
                                                // Only approved members can join
                                            case GroupApproveMembersEnum.ApprovedCanJoin:
                                                groupMember.MemberStatus = GroupMemberStatus.WaitingForApproval;
                                                lblInfo.Text = MemberWaiting.Replace("##GROUPNAME##", HTMLHelper.HTMLEncode(group.GroupDisplayName));
                                                break;
                                                // Only invited members
                                            case GroupApproveMembersEnum.InvitedWithoutApproval:
                                                // Any site members can join
                                            case GroupApproveMembersEnum.AnyoneCanJoin:
                                                groupMember.MemberApprovedWhen = DateTime.Now;
                                                groupMember.MemberStatus = GroupMemberStatus.Approved;
                                                lblInfo.Text = MemberJoined.Replace("##GROUPNAME##", HTMLHelper.HTMLEncode(group.GroupDisplayName));
                                                break;
                                        }
                                        // Store info object to database
                                        GroupMemberInfoProvider.SetGroupMemberInfo(groupMember);

                                        // Handle sending e-mails
                                        if (SendEmailToInviter || SendDefaultGroupEmails)
                                        {
                                            UserInfo sender = UserInfoProvider.GetFullUserInfo(groupMember.MemberUserID);
                                            UserInfo recipient = UserInfoProvider.GetFullUserInfo(groupMember.MemberInvitedByUserID);

                                            if (SendEmailToInviter)
                                            {
                                                EmailTemplateInfo template = EmailTemplateProvider.GetEmailTemplate("Groups.MemberAcceptedInvitation", SiteContext.CurrentSiteName);

                                                // Resolve macros
                                                MacroResolver resolver = MacroContext.CurrentResolver;
                                                resolver.SetAnonymousSourceData(sender, recipient, group, groupMember);
                                                resolver.SetNamedSourceData("Sender", sender);
                                                resolver.SetNamedSourceData("Recipient", recipient);
                                                resolver.SetNamedSourceData("Group", group);
                                                resolver.SetNamedSourceData("GroupMember", groupMember);

                                                if (!String.IsNullOrEmpty(recipient.Email) && !String.IsNullOrEmpty(sender.Email))
                                                {
                                                    // Send e-mail
                                                    EmailMessage message = new EmailMessage();
                                                    message.Recipients = recipient.Email;
                                                    message.From = EmailHelper.GetSender(template, SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSNoreplyEmailAddress"));
                                                    message.Subject = resolver.ResolveMacros(template.TemplateSubject);
                                                    message.PlainTextBody = resolver.ResolveMacros(template.TemplatePlainText);

                                                    // Enable macro encoding for body
                                                    resolver.Settings.EncodeResolvedValues = true;
                                                    message.Body = resolver.ResolveMacros(template.TemplateText);
                                                    message.CcRecipients = template.TemplateCc;
                                                    message.BccRecipients = template.TemplateBcc;
                                                    message.EmailFormat = EmailFormatEnum.Default;

                                                    EmailHelper.ResolveMetaFileImages(message, template.TemplateID, EmailTemplateInfo.OBJECT_TYPE, ObjectAttachmentsCategories.TEMPLATE);
                                                    EmailSender.SendEmail(SiteContext.CurrentSiteName, message);
                                                }
                                            }

                                            if (SendDefaultGroupEmails)
                                            {
                                                // Send join or leave notification
                                                if (group.GroupSendJoinLeaveNotification &&
                                                    (groupMember.MemberStatus == GroupMemberStatus.Approved))
                                                {
                                                    GroupMemberInfoProvider.SendNotificationMail("Groups.MemberJoin", SiteContext.CurrentSiteName, groupMember, true);
                                                    GroupMemberInfoProvider.SendNotificationMail("Groups.MemberJoinedConfirmation", SiteContext.CurrentSiteName, groupMember, false);
                                                }

                                                // Send 'waiting for approval' notification
                                                if (group.GroupSendWaitingForApprovalNotification && (groupMember.MemberStatus == GroupMemberStatus.WaitingForApproval))
                                                {
                                                    GroupMemberInfoProvider.SendNotificationMail("Groups.MemberWaitingForApproval", SiteContext.CurrentSiteName, groupMember, true);
                                                    GroupMemberInfoProvider.SendNotificationMail("Groups.MemberJoinedWaitingForApproval", SiteContext.CurrentSiteName, groupMember, false);
                                                }
                                            }
                                        }

                                        // Delete all invitations to specified group for specified user (based on e-mail or userId)
                                        string whereCondition = "InvitationGroupID = " + invitation.InvitationGroupID + " AND (InvitedUserID=" + MembershipContext.AuthenticatedUser.UserID + " OR InvitationUserEmail = N'" + SqlHelper.GetSafeQueryString(MembershipContext.AuthenticatedUser.Email, false) + "')";
                                        InvitationInfoProvider.DeleteInvitations(whereCondition);

                                        // Log activity
                                        LogJoinActivity(groupMember, group);
                                    }
                                    else
                                    {
                                        lblInfo.Text = UserIsAlreadyMember.Replace("##GROUPNAME##", HTMLHelper.HTMLEncode(group.GroupDisplayName));
                                        lblInfo.CssClass = "InvitationErrorLabel";

                                        // Delete this invitation
                                        InvitationInfoProvider.DeleteInvitationInfo(invitation);
                                    }
                                }
                                else
                                {
                                    lblInfo.Text = GroupNoLongerExists;
                                    lblInfo.CssClass = "InvitationErrorLabel";
                                    // Delete this invitation
                                    InvitationInfoProvider.DeleteInvitationInfo(invitation);
                                }
                            }
                            else
                            {
                                lblInfo.Text = InvitationIsNotValid;
                                lblInfo.CssClass = "InvitationErrorLabel";
                                // Delete this invitation
                                InvitationInfoProvider.DeleteInvitationInfo(invitation);
                            }
                        }
                        else
                        {
                            lblInfo.Text = InvitationNoLongerExists;
                            lblInfo.CssClass = "InvitationErrorLabel";
                        }
                        lblInfo.Visible = true;
                    }
                    else
                    {
                        // Hide control if invitation GUID isn't set
                        Visible = false;
                    }
                }
            }
        }
    }


    /// <summary>
    /// Log activity
    /// </summary>
    /// <param name="gmi">Member info</param>
    /// <param name="group">Group info</param>
    private void LogJoinActivity(GroupMemberInfo gmi, GroupInfo group)
    {
        Activity activity = new ActivityJoinGroup(gmi, group, AnalyticsContext.ActivityEnvironmentVariables);
        activity.Log();
    }
}