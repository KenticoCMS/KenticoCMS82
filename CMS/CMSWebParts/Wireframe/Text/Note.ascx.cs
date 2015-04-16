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

using CMS.Base;

public partial class CMSWebParts_Wireframe_Text_Note : CMSAbstractWireframeWebPart
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
    /// Mark text
    /// </summary>
    public string MarkText
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("MarkText"), "1");
        }
        set
        {
            this.SetValue("MarkText", value);
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
            ltlText.Text = GetWireframeText(Text, false);
            ltlText.Style += String.Format("background-color: {0}", Color);

            colElem.Color = Color;
            colElem.TargetElementID = ltlText.ClientID;

            // Note mark
            if (ControlType.EqualsCSafe("notemark", true))
            {
                ltlMark.Visible = false;
                resElem.Visible = false;
            }
            // Note
            else
            {
                ltlMark.Text = MarkText;

                // Height
                string h = WebPartHeight;
                if (!String.IsNullOrEmpty(h))
                {
                    ltlText.Style += String.Format("height: {0};", h);
                }

                // Width
                string w = WebPartWidth;
                if (!String.IsNullOrEmpty(w))
                {
                    ltlText.Style += String.Format("width: {0};", w);
                }

                resElem.ResizedElementID = ltlText.ClientID;
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