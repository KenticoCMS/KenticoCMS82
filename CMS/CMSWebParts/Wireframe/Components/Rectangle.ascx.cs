using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.PortalControls;

public partial class CMSWebParts_Wireframe_Components_Rectangle : CMSAbstractWireframeWebPart
{
    #region "Properties"

    /// <summary>
    /// Text
    /// </summary>
    public string Text
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Text"), "");
        }
        set
        {
            this.SetValue("Text", value);
        }
    }


    /// <summary>
    /// Color
    /// </summary>
    public string Color
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Color"), "#fd0");
        }
        set
        {
            this.SetValue("Color", value);
        }
    }


    /// <summary>
    /// Allow resize width
    /// </summary>
    public bool AllowResizeWidth
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("AllowResizeWidth"), true);
        }
        set
        {
            this.SetValue("AllowResizeWidth", value);
        }
    }


    /// <summary>
    /// Allow resize height
    /// </summary>
    public bool AllowResizeHeight
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("AllowResizeHeight"), true);
        }
        set
        {
            this.SetValue("AllowResizeHeight", value);
        }
    }


    /// <summary>
    /// Background image
    /// </summary>
    public string BackgroundImage
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("BackgroundImage"), "");
        }
        set
        {
            this.SetValue("BackgroundImage", value);
        }
    }

    #endregion


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do not process
        }
        else
        {
            // Height
            string h = WebPartHeight;
            if (!String.IsNullOrEmpty(h))
            {
                pnlImage.Style.Add("height", h);
            }

            // Width
            string w = WebPartWidth;
            if (!String.IsNullOrEmpty(w))
            {
                pnlImage.Style.Add("width",  w);
            }

            colElem.Color = Color;

            // CSS class
            string cls = this.CssClass;
            if (!String.IsNullOrEmpty(cls))
            {
                colElem.CssClass += " " + this.CssClass;
            }

            // Setup resizer
            if (!AllowResizeHeight)
            {
                resElem.HeightPropertyName = null;
                resElem.ImageUrl = "CMSModules/CMS_PortalEngine/ResizeHorz.png";
            }
            if (!AllowResizeWidth)
            {
                resElem.WidthPropertyName = null;
                resElem.ImageUrl = "CMSModules/CMS_PortalEngine/ResizeVert.png";
            }

            // Background image
            string image = this.BackgroundImage;
            if (!String.IsNullOrEmpty(image))
            {
                pnlImage.Style.Add("background", "url(" + UIHelper.GetImageUrl(this.Page, image) + ") no-repeat rgba(226, 226, 226, 0.6);");
            }

            resElem.ResizedElementID = pnlImage.ClientID;
        }
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }
}