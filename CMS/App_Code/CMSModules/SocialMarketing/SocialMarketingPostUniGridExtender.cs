using System;
using System.Data;
using System.Linq;

using CMS;
using CMS.DataEngine;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.SocialMarketing;
using CMS.UIControls;
using CMS.PortalEngine;
using CMS.Core;

using TreeNode = CMS.DocumentEngine.TreeNode;

[assembly: RegisterCustomClass("SocialMarketingPostUniGridExtender", typeof(SocialMarketingPostUniGridExtender))]

/// <summary>
/// Extends Unigrids used for posts from Social marketing module with additional abilities.
/// </summary>
public class SocialMarketingPostUniGridExtender : ControlExtender<UniGrid>
{

    #region "Private variables"

    private readonly string mFacebookPostDetailsUrlFormat = URLHelper.GetAbsoluteUrl("~/CMSModules/SocialMarketing/Pages/FacebookPostDetailDialog.aspx") + "?postid={0}";

    private readonly string mPostDocumentUrlFormat = URLHelper.GetAbsoluteUrl("~/Admin/cmsadministration.aspx?action=edit&mode=editform") + "&nodeId={0}&culture={1}" + UIContextHelper.GetApplicationHash("cms.content", "content");

    #endregion


    #region "life-cycle methods and event handlers"

    /// <summary>
    /// OnInit page event.
    /// </summary>
    public override void OnInit()
    {
        Control.OnAction += Control_OnAction;
        Control.OnExternalDataBound += Control_OnExternalDataBound;

        ScriptHelper.RegisterDialogScript(Control.Page);
    }


    /// <summary>
    /// Control OnExternalDataBound event.
    /// </summary>
    private object Control_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "documentguid":
            {
                TreeNode document;
                if(GetDocument(parameter, out document))
                {
                    if (document == null)
                    {
                        return ResHelper.GetString("sm.posts.msg.documentnotavailable");
                    }
                    string linkUrl = String.Format(mPostDocumentUrlFormat, document.NodeID, document.DocumentCulture);

                    return String.Format("<a href=\"{0}\" title=\"{1}\" target=\"_blank\">{2}</a>", 
                        linkUrl, 
                        String.Format(ResHelper.GetString("sm.posts.document.tooltip"), HTMLHelper.HTMLEncode(document.NodeAliasPath)),
                        HTMLHelper.HTMLEncode(TextHelper.LimitLength(document.DocumentName, 50)));
                }
                break;
            }

            case "percentage":
                double percentage = ValidationHelper.GetDouble(parameter, -1);
                return percentage >= 0 ? String.Format("{0:0.00} %", percentage) : "";

            case "state":
                return GetPostState(ValidationHelper.GetInteger(parameter, 0));

            case "facebookpostdetail":
                return GetFacebookPostDetailLink(ValidationHelper.GetInteger(parameter, 0));
        }

        return null;
    }


    /// <summary>
    /// Gets document
    /// </summary>
    /// <param name="parameter">Row parameter</param>
    /// <param name="document">Document received based on parameters</param>
    private bool GetDocument(object parameter, out TreeNode document)
    {
        var view = (DataRowView)parameter;

        string guidColumnName = null;
        string siteColumnName = null;

        switch (Control.ObjectType)
        {
            // Facebook post
            case FacebookPostInfo.OBJECT_TYPE:
                guidColumnName = "FacebookPostDocumentGUID";
                siteColumnName = "FacebookPostSiteID";
                break;

            // LinkedIn post
            case LinkedInPostInfo.OBJECT_TYPE:
                guidColumnName = "LinkedInPostDocumentGUID";
                siteColumnName = "LinkedInPostSiteID";
                break;

            // Twitter post
            case TwitterPostInfo.OBJECT_TYPE:
                guidColumnName = "TwitterPostDocumentGUID";
                siteColumnName = "TwitterPostSiteID";
                break;
        }

        var documentGuid = ValidationHelper.GetGuid(DataHelper.GetDataRowViewValue(view, guidColumnName), Guid.Empty);
        var siteId = ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(view, siteColumnName), 0);
        if (documentGuid == Guid.Empty)
        {
            document = null;
            return false;
        }

        document = new ObjectQuery<TreeNode>().WithGuid(documentGuid).OnSite(siteId).FirstOrDefault();
        return true;
    }


    /// <summary>
    /// Gets a link control that opens post details dialog when clicked. Post's content is used as link text.
    /// </summary>
    /// <param name="postId">Facebook post identifier.</param>
    private object GetFacebookPostDetailLink(int postId)
    {
        FacebookPostInfo post = FacebookPostInfoProvider.GetFacebookPostInfo(postId);
        if (LicenseKeyInfoProvider.IsFeatureAvailable(FeatureEnum.SocialMarketingInsights))
        {
            string dialogUrl = String.Format(mFacebookPostDetailsUrlFormat, post.FacebookPostID);
            return String.Format("<a href=\"{0}\" onclick=\"modalDialog('{0}', 'PostDetails', 480, 570); return false;\" title=\"{1}\">{2}</a>",
                dialogUrl,
                ResHelper.GetString("sm.facebook.posts.detail.tooltip"),
                HTMLHelper.HTMLEncode(TextHelper.LimitLength(post.FacebookPostText, 50))
                );
        }
        return HTMLHelper.HTMLEncode(TextHelper.LimitLength(post.FacebookPostText, 50));
    }


    /// <summary>
    /// Control OnAction event.
    /// </summary>
    private void Control_OnAction(string actionName, object actionArgument)
    {
        switch (Control.ObjectType)
        {
            // Facebook post
            case FacebookPostInfo.OBJECT_TYPE:
                OnAction_Facebook(actionName, actionArgument);
                break;

            // LinkedIn post
            case LinkedInPostInfo.OBJECT_TYPE:
                OnAction_LinkedIn(actionName, actionArgument);
                break;

            // Twitter post
            case TwitterPostInfo.OBJECT_TYPE:
                OnAction_Twitter(actionName, actionArgument);
                break;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// OnAction for Facebook post object type.
    /// </summary>
    private void OnAction_Facebook(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "delete":    
                try
                {
                    int postId = ValidationHelper.GetInteger(actionArgument, 0);
                    var postInfo = FacebookPostInfoProvider.GetFacebookPostInfo(postId);

                    if (VerifyPermissionsAndObjectAvailability(postInfo))
                    {
                        FacebookPostInfoProvider.DeleteFacebookPostInfo(postInfo);
                    }
                }
                catch (Exception ex)
                {
                    EventLogProvider.LogWarning("Social marketing - Facebook post", "DELETEPOST", ex, SiteContext.CurrentSiteID, "Facebook post could not be deleted.");
                    Control.ShowError(Control.GetString("sm.facebook.posts.msg.deleteerror"));
                }
                break;
        }
    }


    /// <summary>
    /// OnAction for LinkedIn post object type.
    /// </summary>
    private void OnAction_LinkedIn(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "delete":
                try
                {
                    int postId = ValidationHelper.GetInteger(actionArgument, 0);
                    var postInfo = LinkedInPostInfoProvider.GetLinkedInPostInfo(postId);

                    if (VerifyPermissionsAndObjectAvailability(postInfo))
                    {
                        LinkedInPostInfoProvider.DeleteLinkedInPostInfo(postInfo);
                    }
                    Control.ShowConfirmation(Control.GetString("sm.linkedin.posts.msg.completedelete"));
                }
                catch (LinkedInPartialDeleteException)
                {
                    Control.ShowWarning(Control.GetString("sm.linkedin.posts.msg.partialdelete"));
                }
                catch (Exception ex)
                {
                    EventLogProvider.LogWarning("Social marketing - LinkedIn post", "DELETEPOST", ex, SiteContext.CurrentSiteID, "LinkedIn post could not be deleted.");
                    Control.ShowError(Control.GetString("sm.linkedin.posts.msg.deleteerror"));
                }
                break;
        }
    }


    /// <summary>
    /// OnAction for Twitter post object type.
    /// </summary>
    /// <param name="actionName"></param>
    /// <param name="actionArgument">Integer ID as a string.</param>
    private void OnAction_Twitter(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "delete":
                try
                {
                    int postId = ValidationHelper.GetInteger(actionArgument, 0);
                    var postInfo = TwitterPostInfoProvider.GetTwitterPostInfo(postId);

                    if (VerifyPermissionsAndObjectAvailability(postInfo))
                    {
                        TwitterPostInfoProvider.DeleteTwitterPostInfo(postInfo);
                    }
                }
                catch (Exception ex)
                {
                    EventLogProvider.LogWarning("Social marketing - Twitter post", "DELETEPOST", ex, SiteContext.CurrentSiteID, "Twitter post could not be deleted.");
                    Control.ShowError(Control.GetString("sm.twitter.posts.msg.deleteerror"));
                }
                break;
        }
    }


    /// <summary>
    /// Verifies permissons to modify given BaseInfo and current user.
    /// Redirects to Information Page with "Object doesn't exist." message
    /// or Access denied page.
    /// </summary>
    /// <param name="info">Info object for which the permissions should be verified.</param>
    /// <returns></returns>
    private bool VerifyPermissionsAndObjectAvailability(BaseInfo info)
    {
        if (info == null)
        {
            CMSPage.RedirectToInformation("editedobject.notexists");

            return false;
        }

        if (!info.CheckPermissions(PermissionsEnum.Modify, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
        {
            CMSPage.RedirectToAccessDenied(ModuleName.SOCIALMARKETING, "Modify");

            return false;
        }

        return true;
    }


    /// <summary>
    /// Gets localized message describing post state.
    /// </summary>
    /// <param name="postId">Post id.</param>
    private object GetPostState(int postId)
    {
        switch (Control.ObjectType)
        {
            // Facebook post
            case FacebookPostInfo.OBJECT_TYPE:
                FacebookPostInfo facebookPost = FacebookPostInfoProvider.GetFacebookPostInfo(postId);
                return FacebookPostInfoProvider.GetPostPublishStateMessage(facebookPost, MembershipContext.AuthenticatedUser, SiteContext.CurrentSite, true);

            // LinkedIn post
            case LinkedInPostInfo.OBJECT_TYPE:
                LinkedInPostInfo linkedInPost = LinkedInPostInfoProvider.GetLinkedInPostInfo(postId);
                return LinkedInPostInfoProvider.GetPostPublishStateMessage(linkedInPost, MembershipContext.AuthenticatedUser, SiteContext.CurrentSite, true);

            // Twitter post
            case TwitterPostInfo.OBJECT_TYPE:
                TwitterPostInfo twitterPost = TwitterPostInfoProvider.GetTwitterPostInfo(postId);
                return TwitterPostInfoProvider.GetPostPublishStateMessage(twitterPost, MembershipContext.AuthenticatedUser, SiteContext.CurrentSite, true);
        }

        return String.Empty;
    }

    #endregion
}