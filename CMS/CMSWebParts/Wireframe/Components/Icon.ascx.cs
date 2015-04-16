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

public partial class CMSWebParts_Wireframe_Components_Icon : CMSAbstractWireframeWebPart
{
    #region "Properties"

    /// <summary>
    /// Image
    /// </summary>
    public string ImageUrl
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("ImageUrl"), "CMSModules/CMS_PortalEngine/Wireframes/icon.png");
        }
        set
        {
            this.SetValue("ImageUrl", value);
        }
    }


    /// <summary>
    /// Icon size
    /// </summary>
    public string Size
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Size"), "M");
        }
        set
        {
            this.SetValue("Size", value);
        }
    }


    /// <summary>
    /// Icon label
    /// </summary>
    public string Label
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Label"), "");
        }
        set
        {
            this.SetValue("Label", value);
        }
    }


    /// <summary>
    /// Label position
    /// </summary>
    public string LabelPosition
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("LabelPosition"), "");
        }
        set
        {
            this.SetValue("LabelPosition", value);
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
            imgElem.Attributes.Add("ondragstart", "return false");

            imgElem.CssClass = this.Size;

            ltlText.Text = this.Label;

            if (!this.LabelPosition.Equals("right", StringComparison.InvariantCultureIgnoreCase))
            {
                ltlText.CssClass += " Block";
            }
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