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

public partial class CMSWebParts_Wireframe_Components_Slider : CMSAbstractWireframeWebPart
{
    #region "Properties"

    /// <summary>
    /// Slider size
    /// </summary>
    public string Size
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Size"), "200px");
        }
        set
        {
            this.SetValue("Size", value);
        }
    }


    /// <summary>
    /// Slider position
    /// </summary>
    public string Position
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Position"), "");
        }
        set
        {
            this.SetValue("Position", value);
        }
    }


    /// <summary>
    /// Vertical
    /// </summary>
    public bool Vertical
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("Vertical"), false);
        }
        set
        {
            this.SetValue("Vertical", value);
        }
    }


    /// <summary>
    /// Box CSS class
    /// </summary>
    public string BoxCssClass
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("BoxCssClass"), "");
        }
        set
        {
            this.SetValue("BoxCssClass", value);
        }
    }


    /// <summary>
    /// Slider CSS class
    /// </summary>
    public string SliderCSSClass
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("SliderCSSClass"), "");
        }
        set
        {
            this.SetValue("SliderCSSClass", value);
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
            // Size
            string s = Size;
            string p = Position;

            bool vert = Vertical;
            string prop = (vert ? "height" : "width");

            if (!String.IsNullOrEmpty(s))
            {
                pnlBox.Style.Add(prop, s);
                pnlSlider.Style.Add(prop, p);
            }

            // CSS class
            string cls = this.BoxCssClass;
            if (!String.IsNullOrEmpty(cls))
            {
                pnlBox.CssClass = cls;
            }

            // Slider CSS class
            cls = this.SliderCSSClass;
            if (!String.IsNullOrEmpty(cls))
            {
                pnlSlider.CssClass = cls;
            }

            // Setup resizer
            string resImg = null;
            if (vert)
            {
                resBox.HeightPropertyName = "Size";
                resBox.VerticalOnly = true;

                resSlider.HeightPropertyName = "Position";
                resSlider.VerticalOnly = true;

                pnlBox.CssClass += "Vertical";
                pnlSlider.CssClass += "Vertical";
            }
            else
            {
                resBox.WidthPropertyName = "Size";
                resBox.HorizontalOnly = true;

                resSlider.WidthPropertyName = "Position";
                resSlider.HorizontalOnly = true;
            }

            resBox.ImageUrl = resImg;
            resBox.ResizedElementID = pnlBox.ClientID;

            resSlider.ImageUrl = resImg;
            resSlider.ResizedElementID = pnlSlider.ClientID;
            resSlider.InfoElementID = "#";
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