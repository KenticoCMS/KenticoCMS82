using System.Web;

using CMS.Helpers;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.Base;

public partial class CMSWebParts_Media_RealMedia : CMSAbstractWebPart
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
    /// Gets or sets the controls panel height.
    /// </summary>
    public int ControlsHeight
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ControlsHeight"), 60);
        }
        set
        {
            SetValue("ControlsHeight", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicate whether video is automatically started.
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
    /// Gets or sets the value that indicates whether vide controller is displayed.
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
                    ltlPlaceholder.Text = "<div class=\"VideoLikeContent\" id=\"RMPlaceholder_" + ltlScript.ClientID + "\" ></div>";

                    // Register external script
                    ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Media/RealMedia_files/video.js");

                    // Call function for video object insertion
                    ltlScript.Text = BuildScriptBlock();
                }
                else
                {
                    // Movie
                    ltlPlaceholder.Text = "<div class=\"VideoLikeContent\" ><div class=\"Video\"><object classid=\"clsid:CFCDAA03-8BE4-11CF-B84B-0020AFBBCCFA\" width=\"" + Width + "\" height=\"" + Height + "\">" +
                                          "<param name=\"src\" value=\"" + URLHelper.ResolveUrl(VideoURL) + "\" />" +
                                          "<param name=\"autostart\" value=\"" + Autostart.ToString().ToLowerCSafe() + "\" />" +
                                          "<param name=\"wmode\" value=\"transparent\" />" +
                                          "<param name=\"loop\" value=\"" + Loop + "\" />" +
                                          "<param name=\"logo\" value=\"false\" />" +
                                          "<param name=\"controls\" value=\"ImageWindow\" />" +
                                          "<param name=\"console\" value=\"one\" />" +
                                          "<!--[if !IE]>-->" +
                                          "<embed height=\"" + Height + "\" loop=\"" + Loop + "\" wmode=\"transparent\" src=\"" + HTMLHelper.HTMLEncode(URLHelper.ResolveUrl(VideoURL)) + "\" type=\"audio/x-pn-realaudio-plugin\" width=\"" + Width + "\" controls=\"ImageWindow\" autostart=\"" + Autostart + "\" console=\"one\" logo=\"false\" />" +
                                          "<!--<![endif]-->" +
                                          "<noembed>" + GetString("RealMedia.NotSupported") + "</noembed>" + "\n" +
                                          "</object></div>";

                    // Control panel
                    if (ShowControls)
                    {
                        ltlPlaceholder.Text += "<div class=\"Controls\"><object classid=\"clsid:CFCDAA03-8BE4-11CF-B84B-0020AFBBCCFA\" width=\"" + Width + "\" height=\"" + ControlsHeight + "\">" +
                                               "<param name=\"src\" value=\"" + HTMLHelper.HTMLEncode(URLHelper.ResolveUrl(VideoURL)) + "\" />" +
                                               "<param name=\"autostart\" value=\"" + Autostart.ToString().ToLowerCSafe() + "\" />" +
                                               "<param name=\"wmode\" value=\"transparent\" />" +
                                               "<param name=\"loop\" value=\"" + Loop + "\" />" +
                                               "<param name=\"controls\" value=\"ControlPanel\" />" +
                                               "<param name=\"logo\" value=\"false\" />" +
                                               "<param name=\"console\" value=\"one\" />" +
                                               "<!--[if !IE]>-->" +
                                               "<embed height=\"" + ControlsHeight + "\" loop=\"" + Loop + "\" wmode=\"transparent\" src=\"" + HTMLHelper.HTMLEncode(URLHelper.ResolveUrl(VideoURL)) + "\" type=\"audio/x-pn-realaudio-plugin\" width=\"" + Width + "\" controls=\"ControlPanel\" autostart=\"" + Autostart + "\" console=\"one\" logo=\"false\" />" +
                                               "<!--<![endif]-->" +
                                               "<noembed>" + GetString("RealMedia.NotSupported") + "</noembed>" +
                                               "</object></div>";
                    }

                    // End div
                    ltlPlaceholder.Text += "</div>";
                }
            }
        }
    }


    /// <summary>
    /// Creates a script block which loads a RealMedia object at runtime.
    /// </summary>    
    /// <returns>Script block that will load a RealMedia object</returns>
    private string BuildScriptBlock()
    {
        string scriptBlock = string.Format("LoadRealMedia('RMPlaceholder_{0}', '{1}', {2}, {3}, '{4}', '{5}', '{6}', {7}, {8});",
                                           ltlScript.ClientID,
                                           HTMLHelper.HTMLEncode(URLHelper.ResolveUrl(VideoURL)),
                                           Width,
                                           Height,
                                           ShowControls.ToString().ToLowerCSafe(),
                                           Autostart.ToString().ToLowerCSafe(),
                                           Loop.ToString().ToLowerCSafe(),
                                           ControlsHeight,
                                           ScriptHelper.GetString(GetString("RealMedia.NotSupported")));

        return ScriptHelper.GetScript(scriptBlock);
    }

    #endregion
}