using System;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.SocialMarketing;

[assembly: RegisterCustomClass("LinkedInPostFormExtender", typeof(LinkedInPostFormExtender))]


/// <summary>
/// Extends UI forms used for posts from Social marketing module with additional abilities.
/// </summary>
public class LinkedInPostFormExtender : ControlExtender<UIForm>
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
        var post = Control.EditedObject as LinkedInPostInfo;
        if (post == null)
        {
            return;
        }

        // if the post is newly created fill in default page
        if (post.LinkedInPostID == 0)
        {
            LinkedInAccountInfo defaultAccountInfo = LinkedInAccountInfoProvider.GetDefaultLinkedInAccount(Control.ObjectSiteID);
            if (defaultAccountInfo != null)
            {
                post.LinkedInPostLinkedInAccountID = defaultAccountInfo.LinkedInAccountID;
            }
        }
    }


    /// <summary>
    /// OnAfterDataLoad event
    /// </summary>
    private void ControlOnAfterDataLoad(object sender, EventArgs eventArgs)
    {
        LinkedInPostInfo post = Control.EditedObject as LinkedInPostInfo;
        if ((post != null) && (post.LinkedInPostID > 0))
        {
            string message = LinkedInPostInfoProvider.GetPostPublishStateMessage(post, MembershipContext.AuthenticatedUser, SiteContext.CurrentSite);
            if (post.IsFaulty)
            {
                Control.ShowError(message);
            }
            else
            {
                Control.ShowInformation(message);
            }

            // Disable control if post has already been published or is faulty
            Control.Enabled = !(post.IsPublished || post.IsFaulty);
        }
    }


    /// <summary>
    /// OnBeforeSave event.
    /// </summary>
    private void Control_OnBeforeSave(object sender, EventArgs eventArgs)
    {
        LinkedInPostInfo post = Control.EditedObject as LinkedInPostInfo;
        if (post == null)
        {
            return;
        }

        if (post.LinkedInPostID <= 0)
        {
            // The post is being created, not edited
            return;
        }

        if (!LinkedInPostInfoProvider.TryCancelScheduledPublishLinkedInPost(post))
        {
            // The post has been published during user edit. Prevent the changes to take effect
            CancelPendingSave(Control.GetString("sm.linkedin.posts.msg.editforbidden"));
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
        LinkedInPostInfo post = Control.EditedObject as LinkedInPostInfo;
        if (post == null)
        {
            return;
        }

        if (!post.LinkedInPostPostAfterDocumentPublish)
        {
            try
            {
                LinkedInPostInfoProvider.PublishLinkedInPost(post.LinkedInPostID);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogWarning("Social marketing - LinkedIn post", "PUBLISHPOST", ex, SiteContext.CurrentSiteID,
                    String.Format("An error occurred while publishing the LinkedIn post with ID {0}.", post.LinkedInPostID));
                Control.ShowError(Control.GetString("sm.linkedin.posts.msg.unknownerror"));
            }
        }

        // Invoke event to set the form's state properly
        ControlOnAfterDataLoad(sender, eventArgs);
    }

    #endregion
}
