using System;
using System.Linq;

using CMS.Base;
using CMS.DataEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.SocialMarketing;


public partial class CMSAPIExamples_Code_SocialMarketing_Facebook_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {

        // Facebook application
        apiCreateFacebookApplication.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateFacebookApplication);
        apiGetAndUpdateFacebookApplication.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateFacebookApplication);
        apiDeleteFacebookApplication.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteFacebookApplication);

        // Facebook page
        apiCreateFacebookPage.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateFacebookPage);
        apiGetAndUpdateFacebookPage.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateFacebookPage);
        apiDeleteFacebookPage.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteFacebookPage);

        // Facebook post
        apiCreateFacebookPost.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateFacebookPost);
        apiGetAndUpdateFacebookPost.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateFacebookPost);
        apiDeleteFacebookPosts.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteFacebookPosts);
        apiPublishPostToFacebook.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(PublishPostToFacebook);

    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Facebook application
        apiCreateFacebookApplication.Run();
        apiGetAndUpdateFacebookApplication.Run();

        // Facebook page
        apiCreateFacebookPage.Run();
        apiGetAndUpdateFacebookPage.Run();

        // Facebook post
        apiCreateFacebookPost.Run();
        apiGetAndUpdateFacebookPost.Run();
        apiPublishPostToFacebook.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Facebook post
        apiDeleteFacebookPosts.Run();

        // Facebook page
        apiDeleteFacebookPage.Run();

        // Facebook application
        apiDeleteFacebookApplication.Run();
    }

    #endregion


    #region "API examples - Facebook application"

    /// <summary>
    /// Creates Facebook application. Called when the "Create application" button is pressed.
    /// </summary>
    private bool CreateFacebookApplication()
    {
        // Verify the app's credentials have been provided.
        if (String.IsNullOrWhiteSpace(txtApiKey.Text) || String.IsNullOrWhiteSpace(txtApiSecret.Text))
        {
            throw new Exception("[FacebookApiExamples.CreateFacebookApplication]: Empty values for ApiKey and ApiSecret are not allowed. Please provide your app's credentials.");
        }

        // Create new Facebook application object
        FacebookApplicationInfo newApplication = new FacebookApplicationInfo();

        // Set the properties
        newApplication.FacebookApplicationDisplayName = "My new application";
        newApplication.FacebookApplicationName = "MyNewApplication";
        newApplication.FacebookApplicationSiteID = SiteContext.CurrentSiteID;
        newApplication.FacebookApplicationConsumerKey = txtApiKey.Text;
        newApplication.FacebookApplicationConsumerSecret = txtApiSecret.Text;

        // Save the Facebook application into DB
        FacebookApplicationInfoProvider.SetFacebookApplicationInfo(newApplication);

        return true;
    }


    /// <summary>
    /// Gets and updates Facebook application. Called when the "Get and update application" button is pressed.
    /// Expects the CreateFacebookApplication method to be run first.
    /// </summary>
    private bool GetAndUpdateFacebookApplication()
    {
        // Get the Facebook application from DB
        FacebookApplicationInfo app = FacebookApplicationInfoProvider.GetFacebookApplicationInfo("MyNewApplication", SiteContext.CurrentSiteID);
        if (app != null)
        {
            // Update the properties
            app.FacebookApplicationDisplayName = app.FacebookApplicationDisplayName.ToLowerCSafe();

            // Save the changes into DB
            FacebookApplicationInfoProvider.SetFacebookApplicationInfo(app);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes Facebook application. Called when the "Delete application" button is pressed.
    /// Expects the CreateFacebookApplication method to be run first.
    /// </summary>
    private bool DeleteFacebookApplication()
    {
        // Get the Facebook application from DB
        FacebookApplicationInfo app = FacebookApplicationInfoProvider.GetFacebookApplicationInfo("MyNewApplication", SiteContext.CurrentSiteID);

        // Delete the Facebook application from DB
        FacebookApplicationInfoProvider.DeleteFacebookApplicationInfo(app);

        return (app != null);
    }

    #endregion


    #region "API examples - Facebook page"

    /// <summary>
    /// Creates Facebook page. Called when the "Create page" button is pressed.
    /// </summary>
    private bool CreateFacebookPage()
    {
        // Check if all the fields have been properly set
        if (String.IsNullOrWhiteSpace(txtPageUrl.Text) || String.IsNullOrWhiteSpace(txtFacebookPageAccessToken.Text))
        {
            throw new Exception("[FacebookApiExamples.CreateFacebookPage]: Empty values for 'Facebook page URL' and 'Access token' are not allowed. Please provide your page's credentials.");
        }

        // Get the application ID you want to associate the page with first.
        FacebookApplicationInfo app = FacebookApplicationInfoProvider.GetFacebookApplicationInfo("MyNewApplication", SiteContext.CurrentSiteID);

        if (app == null)
        {
            throw new Exception("[FacebookApiExamples.CreateFacebookPage]: Application 'MyNewApplication' was not found.");
        }
        int appId = app.FacebookApplicationID;

        // Create new Facebook page object
        FacebookAccountInfo page = new FacebookAccountInfo();

        // Set the properties
        page.FacebookAccountDisplayName = "My new page";
        page.FacebookAccountName = "MyNewPage";
        page.FacebookAccountSiteID = SiteContext.CurrentSiteID;
        page.FacebookAccountFacebookApplicationID = appId;

        // Use FacebookHelper to get the ID that facebook has given the page. It's used to identify the page later when posting on its wall. Throw exception if failed.
        string myFacebookPageId;
        if (!FacebookHelper.TryGetFacebookPageId(txtPageUrl.Text, out myFacebookPageId))
        {
            throw new Exception("[FacebookApiExamples.CreateFacebookPage]: Failed to get PageID from Facebook. The 'Page Url', you have entered is not a valid Facebook Page Url.");
        }
        // see https://developers.Facebook.com/docs/Facebook-login/access-tokens/ or http://Facebooksdk.net/docs/web/ or use https://developers.Facebook.com/tools/explorer/ to get access token now
        page.FacebookPageIdentity = new FacebookPageIdentityData(txtPageUrl.Text, myFacebookPageId);
        page.FacebookPageAccessToken = new FacebookPageAccessTokenData(txtFacebookPageAccessToken.Text, null);

        // Save the Facebook page into DB
        FacebookAccountInfoProvider.SetFacebookAccountInfo(page);

        return true;
    }


    /// <summary>
    /// Gets and updates Facebook page. Called when the "Get and update page" button is pressed.
    /// Expects the CreateFacebookPage method to be run first.
    /// </summary>
    private bool GetAndUpdateFacebookPage()
    {
        // Get the Facebook page from DB
        FacebookAccountInfo page = FacebookAccountInfoProvider.GetFacebookAccountInfo("MyNewPage", SiteContext.CurrentSiteID);
        if (page != null)
        {
            // Update the properties
            page.FacebookAccountDisplayName = page.FacebookAccountDisplayName.ToLowerCSafe();

            // Save the changes into DB
            FacebookAccountInfoProvider.SetFacebookAccountInfo(page);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes Facebook page. Called when the "Delete page" button is pressed.
    /// Expects the CreateFacebookPage method to be run first.
    /// </summary>
    private bool DeleteFacebookPage()
    {
        // Get the Facebook page from DB
        FacebookAccountInfo page = FacebookAccountInfoProvider.GetFacebookAccountInfo("MyNewPage", SiteContext.CurrentSiteID);

        // Delete the Facebook page from DB
        FacebookAccountInfoProvider.DeleteFacebookAccountInfo(page);

        return (page != null);
    }

    #endregion


    #region "API examples - Facebook post"

    /// <summary>
    /// Creates Facebook post. Called when the "Create post" button is pressed.
    /// </summary>
    private bool CreateFacebookPost()
    {
        // Get an page to which the post is tied
        FacebookAccountInfo page = FacebookAccountInfoProvider.GetFacebookAccountInfo("MyNewPage", SiteContext.CurrentSiteID);

        if (page == null)
        {
            throw new Exception("[FacebookApiExamples.CreateFacebookPost]: Page 'My new page' wasn't found.");
        }

        // Create new Facebook post object
        FacebookPostInfo post = new FacebookPostInfo();

        // Set the properties
        post.FacebookPostFacebookAccountID = page.FacebookAccountID;
        post.FacebookPostSiteID = SiteContext.CurrentSiteID;
        post.FacebookPostText = "Sample post text.";

        // Should the post be scheduled instead of directly posted?
        post.FacebookPostScheduledPublishDateTime = DateTime.Now + TimeSpan.FromMinutes(5);

        // Is the post tied to a document?
        post.FacebookPostDocumentGUID = null;

        // Save the Facebook post into DB
        FacebookPostInfoProvider.SetFacebookPostInfo(post);

        return true;
    }


    /// <summary>
    /// Gets and updates Facebook post. Called when the "Get and update post" button is pressed.
    /// Expects the CreateFacebookPost method to be run first.
    /// </summary>
    private bool GetAndUpdateFacebookPost()
    {
        // Get an page to which the post is tied
        FacebookAccountInfo page = FacebookAccountInfoProvider.GetFacebookAccountInfo("MyNewPage", SiteContext.CurrentSiteID);

        if (page == null)
        {
            throw new Exception("[FacebookApiExamples.GetAndUpdateFacebookPost]: Page 'My new page' wasn't found.");
        }

        // Get the Facebook post from DB
        FacebookPostInfo post = FacebookPostInfoProvider.GetFacebookPostInfosByAccountId(page.FacebookAccountID).FirstOrDefault();
        if (post != null)
        {
            // Update the properties
            post.FacebookPostText = post.FacebookPostText + " Edited.";

            // Save the changes into DB
            FacebookPostInfoProvider.SetFacebookPostInfo(post);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes Facebook post. Called when the "Delete post" button is pressed.
    /// Expects the CreateFacebookPost method to be run first.
    /// </summary>
    private bool DeleteFacebookPosts()
    {
        // Get an page to which the post is tied
        FacebookAccountInfo page = FacebookAccountInfoProvider.GetFacebookAccountInfo("MyNewPage", SiteContext.CurrentSiteID);

        if (page == null)
        {
            throw new Exception("[FacebookApiExamples.DeleteFacebookPosts]: Page 'My new page' wasn't found.");
        }

        // Get the Facebook post from DB
        ObjectQuery<FacebookPostInfo> post = FacebookPostInfoProvider.GetFacebookPostInfosByAccountId(page.FacebookAccountID);

        // Delete the Facebook post from CMS and from Facebook
        foreach (FacebookPostInfo deletePost in post)
        {
            FacebookPostInfoProvider.DeleteFacebookPostInfo(deletePost);
        }

        return (post.Count != 0);
    }


    /// <summary>
    /// Publishes a facebook post.
    /// </summary>
    private bool PublishPostToFacebook()
    {
        // Get an page to which the post is tied
        FacebookAccountInfo page = FacebookAccountInfoProvider.GetFacebookAccountInfo("MyNewPage", SiteContext.CurrentSiteID);

        if (page == null)
        {
            throw new Exception("[FacebookApiExamples.PublishPostToFacebook]: Page 'My new page' wasn't found.");
        }

        // Get the Facebook post from DB
        FacebookPostInfo post = FacebookPostInfoProvider.GetFacebookPostInfosByAccountId(page.FacebookAccountID).FirstOrDefault();
        if (post == null)
        {
            throw new Exception("[FacebookApiExamples.PublishPostToFacebook]: No post has been created via these api Examples, or they have been deleted.");
        }

        // Publish the post. The post is scheduled to be published if its FacebookPostScheduledPublishDateTime is set in the future.
        FacebookPostInfoProvider.PublishFacebookPost(post.FacebookPostID);

        return true;
    }

    #endregion
}
