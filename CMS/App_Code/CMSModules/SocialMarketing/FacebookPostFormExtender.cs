using System;

using CMS;
using CMS.Base;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.SocialMarketing;

[assembly: RegisterCustomClass("FacebookPostFormExtender", typeof(FacebookPostFormExtender))]

/// <summary>
/// Extends UI forms used for posts from Social marketing module with additional abilities.
/// </summary>
public class FacebookPostFormExtender : ControlExtender<UIForm>
{

    #region "Public methods"

    public override void OnInit()
    {
        Control.OnBeforeSave += Control_OnBeforeSave;
        Control.OnAfterSave += ControlOnAfterSave;
        Control.OnAfterDataLoad += ControlOnAfterDataLoad;
        Control.OnBeforeDataLoad += Control_OnBeforeDataLoad;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// OnBeforeDataLoad event - Ensures default account is pre-selected.
    /// </summary>
    private void Control_OnBeforeDataLoad(object sender, EventArgs e)
    {
        var facebookPost = Control.EditedObject as FacebookPostInfo;
        if (facebookPost == null)
        {
            return;
        }

        // if the post is newly created fill in default page
        if (facebookPost.FacebookPostID == 0)
        {
            FacebookAccountInfo defaultFacebookAccountInfo = FacebookAccountInfoProvider.GetDefaultFacebookAccount(Control.ObjectSiteID);
            if (defaultFacebookAccountInfo != null)
            {
                facebookPost.FacebookPostFacebookAccountID = defaultFacebookAccountInfo.FacebookAccountID;
            }
        }
    }


    /// <summary>
    /// OnAfterDataLoad event
    /// </summary>
    private void ControlOnAfterDataLoad(object sender, EventArgs eventArgs)
    {
        FacebookPostInfo facebookPost = Control.EditedObject as FacebookPostInfo;
        if ((facebookPost != null) && (facebookPost.FacebookPostID > 0))
        {
            string message = FacebookPostInfoProvider.GetPostPublishStateMessage(facebookPost, MembershipContext.AuthenticatedUser, SiteContext.CurrentSite);
            if (facebookPost.IsFaulty)
            {
                Control.ShowError(message);
            }
            else
            {
                Control.ShowInformation(message);
            }

            // Disable control if post has already been published or is faulty
            Control.Enabled = !(facebookPost.IsPublished || facebookPost.IsFaulty);
        }
    }


    /// <summary>
    /// OnBeforeSave event.
    /// </summary>
    private void Control_OnBeforeSave(object sender, EventArgs eventArgs)
    {
        FacebookPostInfo facebookPost = Control.EditedObject as FacebookPostInfo;
        if (facebookPost == null)
        {
            return;
        }

        if (facebookPost.FacebookPostID <= 0)
        {
            // The post is being created, not edited
            return;
        }

        if (!FacebookPostInfoProvider.TryCancelScheduledPublishFacebookPost(facebookPost))
        {
            // The post has been published during user edit. Prevent the changes to take effect
            CancelPendingSave(Control.GetString("sm.facebook.posts.msg.editforbidden"));
        }
    }


    /// <summary>
    /// Cancels and disables save and displays given error message (if any).
    /// </summary>
    /// <param name="errorMessage">Error message to be displayed.</param>
    private void CancelPendingSave(string errorMessage)
    {
        Control.StopProcessing = true;
        Control.SubmitButton.Enabled = false;
        if (!String.IsNullOrEmpty(errorMessage))
        {
            Control.ShowError(errorMessage);
        }
    }


    /// <summary>
    /// OnAfterSave event.
    /// </summary>
    private void ControlOnAfterSave(object sender, EventArgs eventArgs)
    {
        FacebookPostInfo facebookPost = Control.EditedObject as FacebookPostInfo;
        if (facebookPost == null)
        {
            return;
        }

        if (!facebookPost.FacebookPostPostAfterDocumentPublish)
        {
            try
            {
                FacebookPostInfoProvider.PublishFacebookPost(facebookPost.FacebookPostID);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogWarning("Social marketing - Facebook post", "PUBLISHPOST", ex, SiteContext.CurrentSiteID,
                    String.Format("An error occurred while publishing the Facebook post with ID {0}.", facebookPost.FacebookPostID));
                Control.ShowError(Control.GetString("sm.facebook.posts.msg.unknownerror"));
            }
        }

        // Invoke event to set the form's state properly
        ControlOnAfterDataLoad(sender, eventArgs);
    }

    #endregion

}