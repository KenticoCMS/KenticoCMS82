using System;

using CMS.Forums;
using CMS.Globalization;
using CMS.Helpers;
using CMS.SiteProvider;

public partial class CMSModules_Forums_Controls_Layouts_Flat_SubscriptionEdit : ForumViewer
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsAdHocForum)
        {
            plcHeader.Visible = false;
        }

        // Check whether subscription is for forum or post
        if (ForumContext.CurrentSubscribeThread == null)
        {
            ltrTitle.Text = GetString("ForumSubscription.SubscribeForum");
        }
        else
        {
            plcPreview.Visible = true;

            ltrTitle.Text = GetString("ForumSubscription.SubscribePost");

            ltrAvatar.Text = AvatarImage(ForumContext.CurrentSubscribeThread);
            ltrSubject.Text = HTMLHelper.HTMLEncode(ForumContext.CurrentSubscribeThread.PostSubject);
            ltrText.Text = ResolvePostText(ForumContext.CurrentSubscribeThread.PostText);
            ltrUserName.Text = HTMLHelper.HTMLEncode(ForumContext.CurrentSubscribeThread.PostUserName);
            ltrTime.Text = TimeZoneMethods.ConvertDateTime(ForumContext.CurrentSubscribeThread.PostTime, this).ToString();
        }
    }
}