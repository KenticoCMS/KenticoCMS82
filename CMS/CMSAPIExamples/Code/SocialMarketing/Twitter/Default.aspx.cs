using System;
using System.Linq;

using CMS.Base;
using CMS.DataEngine;
using CMS.SiteProvider;
using CMS.SocialMarketing;
using CMS.UIControls;


public partial class CMSAPIExamples_Code_SocialMarketing_Twitter_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {

        // Twitter App
        apiCreateTwitterApp.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateTwitterApp);
        apiGetAndUpdateTwitterApp.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateTwitterApp);
        apiDeleteTwitterApp.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteTwitterApp);

        // Twitter channel
        apiCreateTwitterChannel.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateTwitterChannel);
        apiGetAndUpdateTwitterChannel.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateTwitterChannel);
        apiDeleteTwitterChannel.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteTwitterChannel);

        // Twitter post
        apiCreateTwitterPost.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateTwitterPost);
        apiGetAndUpdateTwitterPost.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateTwitterPost);
        apiPublishPostToTwitter.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(PublishTweetToTwitter);
        apiDeleteTwitterPosts.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteTwitterPosts);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Twitter App
        apiCreateTwitterApp.Run();
        apiGetAndUpdateTwitterApp.Run();

        // Twitter channel
        apiCreateTwitterChannel.Run();
        apiGetAndUpdateTwitterChannel.Run();

        // Twitter post
        apiCreateTwitterPost.Run();
        apiGetAndUpdateTwitterPost.Run();
        apiPublishPostToTwitter.Run();

    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Twitter post
        apiDeleteTwitterPosts.Run();

        // Twitter channel
        apiDeleteTwitterChannel.Run();

        // Twitter App
        apiDeleteTwitterApp.Run();
    }

    #endregion


    #region "API examples - Twitter app"

    /// <summary>
    /// Creates a Twitter App based on the credentials provided in text boxes.
    /// </summary>
    private bool CreateTwitterApp()
    {
        // Verify that app's credentials have been set
        if (string.IsNullOrEmpty(txtConsumerKey.Text) || string.IsNullOrEmpty(txtConsumerSecret.Text))
        {
            throw new Exception("[ApiExamples.CreateTwitterApp]: Empty values for 'Twitter consumer key' and 'Twitter consumer secret' are not allowed. Please provide your app's credentials.");
        }

        // Create new Twitter app object
        TwitterApplicationInfo app = new TwitterApplicationInfo();

        // Set the roperties
        app.TwitterApplicationDisplayName = "My new Twitter app";
        app.TwitterApplicationName = "MyNewTwitterApp";

        app.TwitterApplicationConsumerKey = txtConsumerKey.Text;
        app.TwitterApplicationConsumerSecret = txtConsumerSecret.Text;

        app.TwitterApplicationSiteID = SiteContext.CurrentSiteID;

        // Save the Twitter app into DB
        TwitterApplicationInfoProvider.SetTwitterApplicationInfo(app);

        return true;
    }


    /// <summary>
    /// Gets a Twitter app from the database and modifies it.
    /// </summary>
    private bool GetAndUpdateTwitterApp()
    {
        // Get the app from DB
        TwitterApplicationInfo app = TwitterApplicationInfoProvider.GetTwitterApplicationInfo("MyNewTwitterApp", SiteContext.CurrentSiteName);

        if (app != null)
        {
            // Update the app
            app.TwitterApplicationDisplayName = app.TwitterApplicationDisplayName.ToLowerCSafe();

            //Save the changes into DB
            TwitterApplicationInfoProvider.SetTwitterApplicationInfo(app);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes an existing Twitter app.
    /// </summary>
    private bool DeleteTwitterApp()
    {
        //Get the app from DB
        TwitterApplicationInfo app = TwitterApplicationInfoProvider.GetTwitterApplicationInfo("MyNewTwitterApp", SiteContext.CurrentSiteName);

        if (app != null)
        {
            // Delete the app from DB
            TwitterApplicationInfoProvider.DeleteTwitterApplicationInfo(app);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Twitter channel"

    /// <summary>
    /// Creates a Twitter channel.
    /// </summary>
    private bool CreateTwitterChannel()
    {
        if (string.IsNullOrEmpty(txtAccessToken.Text) || string.IsNullOrEmpty(txtAccessTokenSecret.Text))
        {
            throw new Exception("[ApiExamples.CreateTwitterChannel]: Empty values for 'Channel access token' and 'Channel access token secret' are not allowed. Please provide your channel's credentials.");
        }

        // Get the app the channel is tied to.
        TwitterApplicationInfo app = TwitterApplicationInfoProvider.GetTwitterApplicationInfo("MyNewTwitterApp", SiteContext.CurrentSiteName);

        if (app == null)
        {
            throw new Exception("[ApiExamples.CreateTwitterChannel]: Application 'MyNewTwitterApp' was not found.");
        }

        // Create new channel object
        TwitterAccountInfo channel = new TwitterAccountInfo();

        // Set the properties
        channel.TwitterAccountDisplayName = "My new Twitter channel";
        channel.TwitterAccountName = "MyNewTwitterChannel";

        channel.TwitterAccountAccessToken = txtAccessToken.Text;
        channel.TwitterAccountAccessTokenSecret = txtAccessTokenSecret.Text;
        channel.TwitterAccountSiteID = SiteContext.CurrentSiteID;
        channel.TwitterAccountTwitterApplicationID = app.TwitterApplicationID;

        // Save the channel into DB
        TwitterAccountInfoProvider.SetTwitterAccountInfo(channel);

        return true;
    }


    /// <summary>
    /// Gets a Twitter channel from the database and modifies it.
    /// </summary>
    private bool GetAndUpdateTwitterChannel()
    {
        // Get the channel from DB
        TwitterAccountInfo channel = TwitterAccountInfoProvider.GetTwitterAccountInfo("MyNewTwitterChannel", SiteContext.CurrentSiteID);

        if (channel == null)
        {
            return false;
        }

        // Update the properties
        channel.TwitterAccountDisplayName = channel.TwitterAccountDisplayName.ToLowerCSafe();

        // Save the changes into DB
        TwitterAccountInfoProvider.SetTwitterAccountInfo(channel);

        return true;
    }


    /// <summary>
    /// Deletes a channel from DB
    /// </summary>
    private bool DeleteTwitterChannel()
    {
        // Get the channel from DB
        TwitterAccountInfo channel = TwitterAccountInfoProvider.GetTwitterAccountInfo("MyNewTwitterChannel", SiteContext.CurrentSiteID);

        if (channel == null)
        {
            return false;
        }

        // Delete channel from DB
        TwitterAccountInfoProvider.DeleteTwitterAccountInfo(channel);

        return true;
    }

    #endregion


    #region "API examples - Twitter post"

    /// <summary>
    /// Creates a new tweet. Requires Create Twitter app and Create Twitter channel to be run first.
    /// </summary>
    private bool CreateTwitterPost()
    {
        // Get the channel the tweet is tied to
        TwitterAccountInfo channel = TwitterAccountInfoProvider.GetTwitterAccountInfo("MyNewTwitterChannel", SiteContext.CurrentSiteID);

        if (channel == null)
        {
            throw new Exception("[ApiExamples.CreateTwitterPost]: Account 'MyNewTwitterChannel' has not been found.");
        }

        // Create new post object
        TwitterPostInfo tweet = new TwitterPostInfo();

        // Set the roperties
        tweet.TwitterPostTwitterAccountID = channel.TwitterAccountID;
        tweet.TwitterPostSiteID = SiteContext.CurrentSiteID;
        tweet.TwitterPostText = "Sample tweet text.";

        // Should the tweet be scheduled instead of directly posted?
        tweet.TwitterPostScheduledPublishDateTime = DateTime.Now + TimeSpan.FromMinutes(5);

        // Save the tweet into DB
        TwitterPostInfoProvider.SetTwitterPostInfo(tweet);

        return true;
    }


    /// <summary>
    /// Gets and updates a tweet
    /// </summary>
    private bool GetAndUpdateTwitterPost()
    {
        // Get the channel the tweet is tied to
        TwitterAccountInfo channel = TwitterAccountInfoProvider.GetTwitterAccountInfo("MyNewTwitterChannel", SiteContext.CurrentSiteID);

        if (channel == null)
        {
            throw new Exception("[ApiExamples.GetAndUpdateTwitterPost]: Account 'MyNewTwitterChannel' has not been found.");
        }

        // Get a post tied to the channel
        TwitterPostInfo tweet = TwitterPostInfoProvider.GetTwitterPostInfoByAccountId(channel.TwitterAccountID).FirstOrDefault();
        if (tweet == null)
        {
            throw new Exception("[ApiExamples.GetAndUpdateTwitterPost]: No post has been created via these api-examples. (There is no post tied to 'MyNewTwitterChannel'.)");
        }

        // Update the properties
        tweet.TwitterPostText = tweet.TwitterPostText + " Edited.";

        // Save the changes into DB
        TwitterPostInfoProvider.SetTwitterPostInfo(tweet);

        return true;
    }


    /// <summary>
    /// Deletes all tweets tied to channel 'MyNewTwitterChannel'.
    /// </summary>
    private bool DeleteTwitterPosts()
    {
        // Get the channel the tweet is tied to
        TwitterAccountInfo channel = TwitterAccountInfoProvider.GetTwitterAccountInfo("MyNewTwitterChannel", SiteContext.CurrentSiteID);

        if (channel == null)
        {
            throw new Exception("[ApiExamples.DeleteTwitterPosts]: Account 'MyNewTwitterChannel' has not been found.");
        }

        // Get all posts tied to the account
        ObjectQuery<TwitterPostInfo> tweets = TwitterPostInfoProvider.GetTwitterPostInfoByAccountId(channel.TwitterAccountID);

        // Delete these posts from CMS and from Twitter
        foreach (TwitterPostInfo tweet in tweets)
        {
            TwitterPostInfoProvider.DeleteTwitterPostInfo(tweet);
        }

        return tweets.Count != 0;
    }


    /// <summary>
    /// Publishes a tweet.
    /// </summary>
    private bool PublishTweetToTwitter()
    {
        // Get the channel the tweet is tied to
        TwitterAccountInfo channel = TwitterAccountInfoProvider.GetTwitterAccountInfo("MyNewTwitterChannel", SiteContext.CurrentSiteID);

        if (channel == null)
        {
            throw new Exception("[ApiExamples.PublishTweetToTwitter]: Account 'MyNewTwitterChannel' has not been found.");
        }

        // Get a post tied to the channel
        TwitterPostInfo tweet = TwitterPostInfoProvider.GetTwitterPostInfoByAccountId(channel.TwitterAccountID).FirstOrDefault();
        if (tweet == null)
        {
            throw new Exception("[ApiExamples.PublishTweetToTwitter]: No post has been created via these api-examples. (There is no post tied to 'MyNewTwitterChannel'.)");
        }

        // Publish the post. The Tweet is scheduled for publishing if its FacebookPostScheduledPublishDateTime is set in the future.
        TwitterPostInfoProvider.PublishTwitterPost(tweet.TwitterPostID);

        return true;
    }

    #endregion
}
