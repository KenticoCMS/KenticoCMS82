using System.Web;

using CMS.Helpers;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.Base;

public partial class CMSWebParts_Media_QuickTime : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether video is automatically activated.
    /// </summary>
    public bool AutoActivation
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AutoActivation"), false);
        }
        set
        {
            SetValue("AutoActivation", value);
        }
    }


    /// <summary>
    /// Gets or sets the URL of video to be displayed.
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
    /// Gets or sets the width of video.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), 400);
        }
        set
        {
            SetValue("Width", value);
        }
    }


    /// <summary>
    /// Gets or sets the height of video.
    /// </summary>
    public int Height
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Height"), 300);
        }
        set
        {
            SetValue("Height", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the video is automatically started.
    /// </summary>
    public bool Autostart
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Autostart"), false);
        }
        set
        {
            SetValue("Autostart", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether video controller is displayed.
    /// </summary>
    public bool ShowControls
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowControls"), true);
        }
        set
        {
            SetValue("ShowControls", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether video after the end is automatically started again.
    /// </summary>
    public bool Loop
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Loop"), false);
        }
        set
        {
            SetValue("Loop", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Reloads data.
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
            if (PortalContext.IsDesignMode(PortalContext.ViewMode))
            {
                ltlPlaceholder.Text = "<table style=\"border: 1px solid Gray;background-color:#eee;width:" + Width + "px;height:" + Height + "px\"><tr><td style=\"vertical-align:middle;text-align:center;color:Gray;\">" + GetString("global.herecomesvideo").ToUpperCSafe() + "</td></tr></table>";
            }
            else
            {
                // Auto activation hack
                if (AutoActivation)
                {
                    ltlPlaceholder.Text = "<div class=\"VideoLikeContent\" id=\"QTPlaceholder_" + ltlScript.ClientID + "\" ></div>";

                    // Register external script
                    ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Media/QuickTime_files/video.js");

                    // Call function for video object insertion
                    ltlScript.Text = BuildScriptBlock();
                }
                else
                {
                    ltlPlaceholder.Text = "<div class=\"VideoLikeContent\" ><object classid=\"clsid:02BF25D5-8C17-4B23-BC80-D3488ABDDC6B\" codebase=\"http://www.apple.com/qtactivex/qtplugin.cab\" width=\"" + Width + "\" height=\"" + Height + "\">" +
                                          "<param name=\"src\" value=\"" + HTMLHelper.HTMLEncode(URLHelper.ResolveUrl(VideoURL)) + "\" />" +
                                          "<param name=\"controller\" value=\"" + (ShowControls ? "true" : "false") + "\" />" +
                                          "<param name=\"autoplay\" value=\"" + (Autostart ? "true" : "false") + "\" />" +
                                          //"<param name=\"wmode\" value=\"transparent\" />" +
                                          "<param name=\"loop\" value=\"" + (Loop ? "true" : "false") + "\" />" +
                                          "<param name=\"scale\" value=\"tofit\" />" +
                                          "<!--[if !IE]>-->" +
                                          "<object type=\"video/quicktime\" data=\"" + HTMLHelper.HTMLEncode(URLHelper.ResolveUrl(VideoURL)) + "\" width=\"" + Width + "\" height=\"" + Height + "\" >" +
                                          "<param name=\"autoplay\" value=\"" + (Autostart ? "true" : "false") + "\" />" +
                                          "<param name=\"controller\" value=\"" + (ShowControls ? "true" : "false") + "\" />" +
                                          "<param name=\"loop\" value=\"" + (Loop ? "true" : "false") + "\" />" +
                                          "<param name=\"scale\" value=\"tofit\" />" +
                                          //"<param name=\"wmode\" value=\"transparent\" />" +
                                          GetString("Media.NotSupported") + "\n" +
                                          "</object>" +
                                          "<!--<![endif]-->" +
                                          "</object></div>";
                }
            }
        }
    }


    /// <summary>
    /// Creates a script block which loads a QuickTime video at runtime.
    /// </summary>    
    /// <returns>Script block that will load a QuickTime video</returns>
    private string BuildScriptBlock()
    {
        string scriptBlock = string.Format("LoadQuickTime('QTPlaceholder_{0}', '{1}', {2}, {3}, '{4}', '{5}', '{6}', {7});",
                                           ltlScript.ClientID,
                                           HTMLHelper.HTMLEncode(URLHelper.ResolveUrl(VideoURL)),
                                           Width,
                                           Height,
                                           ShowControls.ToString().ToLowerCSafe(),
                                           Autostart.ToString().ToLowerCSafe(),
                                           Loop.ToString().ToLowerCSafe(),
                                           ScriptHelper.GetString(GetString("Media.NotSupported")));

        return ScriptHelper.GetScript(scriptBlock);
    }

    #endregion
}