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

public partial class CMSWebParts_Wireframe_Components_Image : CMSAbstractWireframeWebPart
{
    #region "Properties"

    /// <summary>
    /// Image URL
    /// </summary>
    public string ImageUrl
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("ImageUrl"), "");
        }
        set
        {
            this.SetValue("ImageUrl", value);
        }
    }


    /// <summary>
    /// Use bounding box
    /// </summary>
    public bool UseBoundingBox
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("UseBoundingBox"), false);
        }
        set
        {
            this.SetValue("UseBoundingBox", value);
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
            imgElem.ImageUrl = UIHelper.GetImageUrl(this.Page, ImageUrl);

            if (UseBoundingBox)
            {
                // Height
                string h = WebPartHeight;
                if (!String.IsNullOrEmpty(h))
                {
                    pnlBox.Height = new Unit(h);
                }

                // Width
                string w = WebPartWidth;
                if (!String.IsNullOrEmpty(w))
                {
                    pnlBox.Width = new Unit(w);
                }

                pnlBox.RenderChildrenOnly = false;

                string boxCss = this.BoxCssClass;
                if (!String.IsNullOrEmpty(boxCss))
                {
                    pnlBox.CssClass += " " + boxCss;
                }
                
                resElem.ResizedElementID = pnlBox.ClientID;
                resElem.RenderEnvelope = true;
            }
            else
            {
                // Height
                string h = WebPartHeight;
                if (!String.IsNullOrEmpty(h))
                {
                    imgElem.Height = new Unit(h);
                }

                // Width
                string w = WebPartWidth;
                if (!String.IsNullOrEmpty(w))
                {
                    imgElem.Width = new Unit(w);
                }

                // Resize image directly
                resElem.ResizedElementID = imgElem.ClientID;
            }

            mRenderWebPartClass = false;
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