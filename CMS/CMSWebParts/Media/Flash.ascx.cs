using System;

using CMS.Helpers;
using CMS.PortalControls;
using CMS.Base;

public partial class CMSWebParts_Media_Flash : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the URL of the flash to be displayed.
    /// </summary>
    public string FlashURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FlashURL"), "");
        }
        set
        {
            SetValue("FlashURL", value);
        }
    }


    /// <summary>
    /// Gets or sets additional parameters for player.
    /// </summary>
    public string AdditionalParameters
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AdditionalParameters"), "");
        }
        set
        {
            SetValue("AdditionalParameters", value);
        }
    }


    /// <summary>
    /// Gets or sets the width of the flash.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), 200);
        }
        set
        {
            SetValue("Width", value);
        }
    }


    /// <summary>
    /// Gets or sets the height of the flash.
    /// </summary>
    public int Height
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Height"), 150);
        }
        set
        {
            SetValue("Height", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether fullscreen mode is allowed or not.
    /// </summary>
    public bool AllowFullScreen
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowFullScreen"), false);
        }
        set
        {
            SetValue("AllowFullScreen", value);
        }
    }


    /// <summary>
    /// Gets or sets the quality of the flash.
    /// </summary>
    public string Quality
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Quality"), "best");
        }
        set
        {
            SetValue("Quality", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether flash is started automatically.
    /// </summary>
    public bool AutoPlay
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AutoPlay"), true);
        }
        set
        {
            SetValue("AutoPlay", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether flash after the end is automatically started again.
    /// </summary>
    public bool Loop
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Loop"), true);
        }
        set
        {
            SetValue("Loop", value);
        }
    }


    /// <summary>
    /// Gets or sets the scale of the flash.
    /// </summary>
    public string Scale
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Scale"), "default");
        }
        set
        {
            SetValue("Scale", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether flash is automatically activated.
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
    /// Gets or sets the 'Not supported' text. 
    /// </summary>
    public string NotSupportedText
    {
        get
        {
            return GetString(ValidationHelper.GetString(GetValue("NotSupportedText"), "Flash.NotSupported"));
        }
        set
        {
            SetValue("NotSupportedText", value);
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
            string additionalParams = string.Empty;
            if (!String.IsNullOrEmpty(AdditionalParameters))
            {
                additionalParams = AdditionalParameters.Trim() + "\n";
            }

            if (AutoActivation)
            {
                ltlPlaceholder.Text = "<div class=\"VideoLikeContent\" id=\"FlashPlaceholder_" + ltlScript.ClientID + "\" ></div>";

                // Register external script
                ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Media/Flash_files/flash.js");

                // Call function for flash object insertion               
                ltlScript.Text = BuildScriptBlock(additionalParams);
            }
            else
            {
                // Create flash
                ltlPlaceholder.Text = "<div class=\"VideoLikeContent\" ><object type=\"application/x-shockwave-flash\" width=\"" + Width + "\" height=\"" + Height + "\" data=\"" + HTMLHelper.HTMLEncode(ResolveUrl(FlashURL)) + "\">\n" +
                                      "<param name=\"classid\" value=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" />\n" +
                                      "<param name=\"codebase\" value=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,40,0\" />\n" +
                                      "<param name=\"movie\" value=\"" + HTMLHelper.HTMLEncode(ResolveUrl(FlashURL)) + "\" />\n" +
                                      "<param name=\"quality\" value=\"" + HTMLHelper.HTMLEncode(Quality) + "\" />\n" +
                                      "<param name=\"scale\" value=\"" + HTMLHelper.HTMLEncode(Scale) + "\" />\n" +
                                      "<param name=\"allowFullScreen\" value=\"" + AllowFullScreen + "\" />\n" +
                                      "<param name=\"play\" value=\"" + AutoPlay + "\" />\n" +
                                      "<param name=\"loop\" value=\"" + Loop + "\" />\n" +
                                      "<param name=\"pluginurl\" value=\"http://www.adobe.com/go/getflashplayer\" />\n" +
                                      "<param name=\"wmode\" value=\"transparent\" />\n" +
                                      additionalParams +
                                      NotSupportedText + "\n" +
                                      "</object></div>";
            }
        }
    }


    /// <summary>
    /// Creates a script block which loads a Flash object at runtime.
    /// </summary>
    /// <param name="additionalParams">Additional parameters for the script</param>
    /// <returns>Script block that will load a Flash object</returns>
    private string BuildScriptBlock(string additionalParams)
    {
        string scriptBlock = string.Format(@"LoadFlash('FlashPlaceholder_{0}', '{1}', {2}, {3}, {4}, '{5}', '{6}', {7}, {8}, {9}, {10})",
                                           ltlScript.ClientID,
                                           HTMLHelper.HTMLEncode(ResolveUrl(FlashURL)),
                                           Width,
                                           Height,
                                           AllowFullScreen.ToString().ToLowerCSafe(),
                                           HTMLHelper.HTMLEncode(Quality),
                                           HTMLHelper.HTMLEncode(Scale),
                                           AutoPlay.ToString().ToLowerCSafe(),
                                           Loop.ToString().ToLowerCSafe(),
                                           ScriptHelper.GetString(NotSupportedText),
                                           string.IsNullOrEmpty(additionalParams) ? "''" : additionalParams);

        return ScriptHelper.GetScript(scriptBlock);
    }

    #endregion
}