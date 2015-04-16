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

public partial class CMSWebParts_Wireframe_Components_WYSIWYG : CMSAbstractWireframeWebPart
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
    /// Sets the height of the editor toolbar. It must be specified as a CSS style value, e.g. 300px or 50%.
    /// </summary>
    public string ToolbarHeight
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("ToolbarHeight"), "65px");
        }
        set
        {
            this.SetValue("ToolbarHeight", value);
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
                pnlEditor.Style.Add("height", h);
            }

            // Width
            string w = WebPartWidth;
            if (!String.IsNullOrEmpty(w))
            {
                pnlEditor.Style.Add("width", w);
            }

            // Height
            string th = ToolbarHeight;
            if (!String.IsNullOrEmpty(th))
            {
                toolbarElem.CssStyle += "height: " + th + ";";
            }

            toolbarElem.Color = Color;

            textElem.Text = Text;

            resToolbar.ResizedElementID = toolbarElem.ClientID;
            resElem.ResizedElementID = pnlEditor.ClientID;
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