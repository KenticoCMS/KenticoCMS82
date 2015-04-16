using System;

using CMS.Helpers;
using CMS.SocialMedia;


public partial class CMSWebParts_SocialMedia_YouTubeVideo : SocialMediaAbstractWebPart
{
    #region "Private fiels"

    private bool mHide;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Indicates whether to hide content of the WebPart
    /// </summary>
    public override bool HideContent
    {
        get
        {
            return mHide;
        }
        set
        {
            mHide = value;
            ltlPlaceholder.Visible = !value;
            ltlScript.Visible = !value;
        }
    }



    /// <summary>
    ///  Gets or sets the URL of YouTube video to be displayed.
    /// </summary>
    public string VideoURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("VideoURL"), "");
        }
        set
        {
            SetValue("VideoURL", value);
        }
    }


    /// <summary>
    /// Gets or sets the video width.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), 425);
        }
        set
        {
            SetValue("Width", value);
        }
    }


    /// <summary>
    /// Gets or sets the video height.
    /// </summary>
    public int Height
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Height"), 355);
        }
        set
        {
            SetValue("Height", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether video start immediately after webpart load.
    /// </summary>
    public bool AutoPlay
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AutoPlay"), false);
        }
        set
        {
            SetValue("AutoPlay", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether video should loops after playback stops.
    /// </summary>
    [Obsolete("'Loop' parameter is no longer supported.")]
    public bool Loop
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value that indicates whether player should display related videos after playback stops.
    /// </summary>
    public bool Rel
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Rel"), false);
        }
        set
        {
            SetValue("Rel", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether video should support full screen playback.
    /// </summary>
    public bool FullScreen
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("FullScreen"), false);
        }
        set
        {
            SetValue("FullScreen", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected override void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            YouTubeVideoParameters ytParams = new YouTubeVideoParameters();
            ytParams.Url = VideoURL;

            // If no wmode is set, append 'transparent' wmode. Fix IE issue with widget (widgets buttons are beyond youtube video)
            if (String.IsNullOrEmpty(URLHelper.GetQueryValue(ytParams.Url, "wmode")))
            {
                ytParams.Url = URLHelper.UpdateParameterInUrl(ytParams.Url, "wmode", "transparent");
            }

            ytParams.FullScreen = FullScreen;
            ytParams.AutoPlay = AutoPlay;
            ytParams.RelatedVideos = Rel;
            ytParams.Width = Width;
            ytParams.Height = Height;

            ltlPlaceholder.Text = MediaHelper.GetYouTubeVideo(ytParams);
        }
    }

    #endregion
}