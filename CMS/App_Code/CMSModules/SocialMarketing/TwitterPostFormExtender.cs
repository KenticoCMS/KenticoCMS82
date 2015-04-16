using System;

using CMS;
using CMS.Base;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.SocialMarketing;

[assembly: RegisterCustomClass("TwitterPostFormExtender", typeof(TwitterPostFormExtender))]

/// <summary>
/// Extends UI forms used for posts from Social marketing module with additional abilities.
/// </summary>
public class TwitterPostFormExtender : ControlExtender<UIForm>
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
        var twitterPost = Control.EditedObject as TwitterPostInfo;
        if (twitterPost == null)
        {
            return;
        }

        // if the post is newly created fill in default page
        if (twitterPost.TwitterPostID == 0)
        {
            TwitterAccountInfo defaultTwitterAccountInfo = TwitterAccountInfoProvider.GetDefaultTwitterAccount(Control.ObjectSiteID);
            if (defaultTwitterAccountInfo != null)
            {
                twitterPost.TwitterPostTwitterAccountID = defaultTwitterAccountInfo.TwitterAccountID;
            }
        }
    }


    /// <summary>
    /// OnAfterDataLoad event
    /// </summary>
    private void ControlOnAfterDataLoad(object sender, EventArgs eventArgs)
    {
        TwitterPostInfo twitterPost = Control.EditedObject as TwitterPostInfo;
        if ((twitterPost != null) && (twitterPost.TwitterPostID > 0))
        {
            string message = TwitterPostInfoProvider.GetPostPublishStateMessage(twitterPost, MembershipContext.AuthenticatedUser, SiteContext.CurrentSite);
            if (twitterPost.IsFaulty)
            {
                Control.ShowError(message);
            }
            else
            {
                Control.ShowInformation(message);
            }

            // Disable control if post has already been published or is faulty
            Control.Enabled = !(twitterPost.IsPublished || twitterPost.IsFaulty);
        }
    }


    /// <summary>
    /// OnBeforeSave event.
    /// </summary>
    private void Control_OnBeforeSave(object sender, EventArgs eventArgs)
    {
        TwitterPostInfo twitterPost = Control.EditedObject as TwitterPostInfo;
        if (twitterPost == null)
        {
            return;
        }

        if (twitterPost.TwitterPostID <= 0)
        {
            // The post is being created, not edited
            return;
        }

        if (!TwitterPostInfoProvider.TryCancelScheduledPublishTwitterPost(twitterPost))
        {
            // The post has been published during user edit. Prevent the changes to take effect
            CancelPendingSave(Control.GetString("sm.twitter.posts.msg.editforbidden"));
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
        TwitterPostInfo twitterPost = Control.EditedObject as TwitterPostInfo;
        if (twitterPost == null)
        {
            return;
        }

        if (!twitterPost.TwitterPostPostAfterDocumentPublish)
        {
            try
            {
                TwitterPostInfoProvider.PublishTwitterPost(twitterPost.TwitterPostID);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogWarning("Social marketing - Twitter post", "PUBLISHPOST", ex, SiteContext.CurrentSiteID,
                    String.Format("An error occurred while publishing the Twitter post with ID {0}.", twitterPost.TwitterPostID));
                Control.ShowError(Control.GetString("sm.twitter.posts.msg.unknownerror"));
            }
        }

        // Invoke event to set the form's state properly
        ControlOnAfterDataLoad(sender, eventArgs);
    }

    #endregion

}