﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Membership;
using CMS.UIControls;
using CMS.Helpers;
using CMS.Forums;
using CMS.ExtendedControls;

public partial class CMSModules_Forums_CMSPages_LiveUserUnsubscribe : CMSLiveModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup the modal dialog
        SetCulture();
        RegisterEscScript();

        // Get post ID
        int postId = QueryHelper.GetInteger("postid", 0);

        if (CheckSubscription(postId))
        {
            // Set mode
            PostApproveFooter.Mode = PostApprove.Mode = "subscription";

            // Set the post ID
            PostApprove.PostID = PostApproveFooter.PostID = postId;
            PostApprove.UserID = PostApproveFooter.UserID = MembershipContext.AuthenticatedUser.UserID;

            // Page title
            PageTitle.TitleText = GetString("forums_forumnewpost_header.preview");
        }
        else
        {
            // Redirect to access denied
            RedirectToAccessDenied(ResHelper.GetString("forumpost.notsubscribed"));
        }
    }


    /// <summary>
    /// Checks if current user has subscribed given post.
    /// </summary>
    /// <param name="postId">ID of forum post</param>
    /// <returns>true if user has subscriptions for given post, false if not</returns>
    private bool CheckSubscription(int postId)
    {
        DataSet ds = ForumSubscriptionInfoProvider.GetSubscriptions("(SubscriptionUserID = " + MembershipContext.AuthenticatedUser.UserID + ") AND (SubscriptionPostID = " + postId + ")", null, 0, "SubscriptionID");

        if (DataHelper.DataSourceIsEmpty(ds))
        {
            return false;
        }

        return true;
    }


    /// <summary>
    /// Raises the <see cref="E:PreRender"/> event.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Setup the modal dialog
        RegisterModalPageScripts();
    }

}
