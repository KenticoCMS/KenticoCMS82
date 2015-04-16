using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.SiteProvider;

public partial class CMSInlineControls_MediaControl : InlineUserControl
{
    #region "Properties"

    /// <summary>
    /// Url of media file.
    /// </summary>
    public string Url
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Url"), null);
        }
        set
        {
            SetValue("Url", value);
        }
    }


    /// <summary>
    /// Type of media file.
    /// </summary>
    public string Type
    {
        get
        {
            string type = ValidationHelper.GetString(GetValue("Type"), null);
            if (type == null)
            {
                type = ValidationHelper.GetString(GetValue("Ext"), null);
            }
            if (type == null)
            {
                type = URLHelper.GetUrlParameter(Url, "ext");
            }
            return type;
        }
        set
        {
            SetValue("Type", value);
        }
    }


    /// <summary>
    /// Width of media or flash player.
    /// </summary>
    public int Width
    {
        get
        {
            int width = ValidationHelper.GetInteger(GetValue("Width"), -1);
            if (width == -1)
            {
                width = ValidationHelper.GetInteger(URLHelper.GetUrlParameter(Url, "width"), -1);
            }
            return width;
        }
        set
        {
            SetValue("Width", value);
        }
    }


    /// <summary>
    /// Height of media or flash player.
    /// </summary>
    public int Height
    {
        get
        {
            int height = ValidationHelper.GetInteger(GetValue("Height"), -1);
            if (height == -1)
            {
                height = ValidationHelper.GetInteger(URLHelper.GetUrlParameter(Url, "height"), -1);
            }
            return height;
        }
        set
        {
            SetValue("Height", value);
        }
    }


    /// <summary>
    /// Auto play media or flash.
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
    /// Loop media or flash.
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


    /// <summary>
    /// Show media player controls.
    /// </summary>
    public bool AVControls
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Controls"), true);
        }
        set
        {
            SetValue("Controls", value);
        }
    }


    /// <summary>
    /// Automatically active media player.
    /// </summary>
    public bool AutoActive
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AutoActive"), false);
        }
        set
        {
            SetValue("AutoActive", value);
        }
    }


    /// <summary>
    /// Enable flash control context menu.
    /// </summary>
    public bool Menu
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Menu"), false);
        }
        set
        {
            SetValue("Menu", value);
        }
    }


    /// <summary>
    /// Scale of flash control.
    /// </summary>
    public string Scale
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Scale"), null);
        }
        set
        {
            SetValue("Scale", value);
        }
    }


    /// <summary>
    /// Flash control id.
    /// </summary>
    public string Id
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Id"), null);
        }
        set
        {
            SetValue("Id", value);
        }
    }


    /// <summary>
    /// Title of flash player control.
    /// </summary>
    public string Title
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Title"), null);
        }
        set
        {
            SetValue("Title", value);
        }
    }


    /// <summary>
    /// Flash control css style class.
    /// </summary>
    public string Class
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Class"), null);
        }
        set
        {
            SetValue("Class", value);
        }
    }


    /// <summary>
    /// Flash control inline style.
    /// </summary>
    public string Style
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Style"), null);
        }
        set
        {
            SetValue("Style", value);
        }
    }


    /// <summary>
    /// Flash control variables.
    /// </summary>
    public string FlashVars
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FlashVars"), null);
        }
        set
        {
            SetValue("FlashVars", value);
        }
    }


    /// <summary>
    /// Control parameter.
    /// </summary>
    public override string Parameter
    {
        get
        {
            return Url;
        }
        set
        {
            Url = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (MediaHelper.IsFlash(Type))
        {
            CreateFlash();
        }
        else if (ImageHelper.IsImage(Type))
        {
            CreateImage();
        }
        else
        {
            CreateMedia();
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Creates the flash object
    /// </summary>
    private void CreateFlash()
    {
        FlashParameters flParams = new FlashParameters();
        flParams.Url = URLHelper.GetAbsoluteUrl(Url);
        flParams.Extension = Type;
        flParams.Width = Width;
        flParams.Height = Height;
        flParams.Autoplay = AutoPlay;
        flParams.Loop = Loop;
        flParams.Menu = Menu;
        flParams.Scale = Scale;
        flParams.Id = HttpUtility.UrlDecode(Id);
        flParams.Title = HttpUtility.UrlDecode(Title);
        flParams.Class = HttpUtility.UrlDecode(Class);
        flParams.Style = HttpUtility.UrlDecode(Style);
        flParams.FlashVars = HttpUtility.UrlDecode(FlashVars);

        ltlMedia.Text = MediaHelper.GetFlash(flParams);
    }


    /// <summary>
    /// Creates the media (audio / video) object
    /// </summary>
    private void CreateMedia()
    {
        AudioVideoParameters avParams = new AudioVideoParameters();
        if (Url != null)
        {
            avParams.SiteName = SiteContext.CurrentSiteName;
            avParams.Url = URLHelper.GetAbsoluteUrl(Url);
            avParams.Extension = Type;
            avParams.Width = Width;
            avParams.Height = Height;
            avParams.AutoPlay = AutoPlay;
            avParams.Loop = Loop;
            avParams.Controls = AVControls;
        }

        ltlMedia.Text = MediaHelper.GetAudioVideo(avParams);
    }


    /// <summary>
    /// Creates the image object
    /// </summary>
    private void CreateImage()
    {
        ImageParameters imgParams = new ImageParameters();
        if (Url != null)
        {
            imgParams.Url = URLHelper.GetAbsoluteUrl(Url);
            imgParams.Extension = Type;
            imgParams.Width = Width;
            imgParams.Height = Height;
            imgParams.Id = HttpUtility.UrlDecode(Id);
            imgParams.Tooltip = HttpUtility.UrlDecode(Title);
            imgParams.Class = HttpUtility.UrlDecode(Class);
            imgParams.Style = HttpUtility.UrlDecode(Style);
        }
        ltlMedia.Text = MediaHelper.GetImage(imgParams);
    }

    #endregion
}