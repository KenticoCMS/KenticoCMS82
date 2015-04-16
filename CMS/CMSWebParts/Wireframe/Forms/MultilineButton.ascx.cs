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

public partial class CMSWebParts_Wireframe_Forms_MultilineButton : CMSAbstractWireframeWebPart
{
    #region "Properties"

    /// <summary>
    /// Main text
    /// </summary>
    public string MainText
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("MainText"), "Main text");
        }
        set
        {
            this.SetValue("MainText", value);
        }
    }


    /// <summary>
    /// Text
    /// </summary>
    public string Text
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Text"), "Info text");
        }
        set
        {
            this.SetValue("Text", value);
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
            ltlText.Text = Text;
            ltlMainText.Text = MainText;

            // Width
            string w = WebPartWidth;
            if (!String.IsNullOrEmpty(w))
            {
                pnlButton.Style.Add("width", w);
            }

            // Height
            string h = WebPartHeight;
            if (!String.IsNullOrEmpty(h))
            {
                pnlButton.Style.Add("height", h);
            }

            resElem.ResizedElementID = pnlButton.ClientID;
            resElem.RenderEnvelope = true;
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