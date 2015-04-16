using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Helpers;
using CMS.PortalControls;

public partial class CMSWebParts_Wireframe_WireframeComponent : CMSAbstractWireframeWebPart
{
    #region "Variables"

    WebControl envelopeControl = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Text
    /// </summary>
    public string Text
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Text"), "Sample text");
        }
        set
        {
            this.SetValue("Text", value);
        }
    }


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
    /// Gets or sets the Editable web part property envelope CSS class
    /// </summary>
    private string PropertyEnvelopeCssClass
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the Editable web part image envelope CSS class
    /// </summary>
    public string ImageEnvelopeCssClass
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

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

            switch (ControlType.ToLowerCSafe())
            {
                case "button":
                    EnsureButton();
                    break;

                case "textbox":
                    EnsureTextBox();
                    break;

                case "textarea":
                    EnsureTextArea();
                    break;

                case "label":
                    EnsureLabel();
                    break;

                case "multilinetext":
                    EnsureMultiLineText();
                    break;

                case "fieldset":
                    EnsureFieldSet();
                    break;
            }

            // Bound client id
            resElem.ResizedElementID = ltlText.ClientID;

            #region "Editable web part property"

            ltlText.Text = GetWireframeText(Text, false);

            if (!String.IsNullOrEmpty(this.CssClass))
            {
                ltlText.CssClass = this.CssClass;
            }

            if (!String.IsNullOrEmpty(PropertyEnvelopeCssClass))
            {
                pnlProperty.RenderChildrenOnly = false;
                pnlProperty.CssClass = PropertyEnvelopeCssClass;
                resElem.ResizedElementID = pnlProperty.ClientID;
                envelopeControl = pnlProperty;
            }

            #endregion


            #region "Editable image"

            // Image URL
            string imgUrl = ImageUrl;
            if (!String.IsNullOrEmpty(imgUrl))
            {
                imgElem.ImageUrl = UIHelper.GetImageUrl(this.Page, imgUrl);
                imgElem.Visible = true;

                if (!String.IsNullOrEmpty(ImageEnvelopeCssClass))
                {
                    pnlImage.RenderChildrenOnly = false;
                    pnlImage.CssClass = ImageEnvelopeCssClass;
                }
            }

            #endregion


            #region "Dimensions"

            // Width
            string w = WebPartWidth;
            if (!String.IsNullOrEmpty(w))
            {
                if (envelopeControl == null)
                {
                    ltlText.Style += String.Format("width: {0};", w);
                }
                else
                {
                    envelopeControl.Width = new Unit(w);
                }
            }

            // Height
            string h = WebPartHeight;
            if (!String.IsNullOrEmpty(h))
            {
                if (envelopeControl == null)
                {
                    ltlText.Style += String.Format("height: {0};", h);
                }
                else
                {
                    envelopeControl.Height = new Unit(h);
                }
            }

            #endregion
        }
    }

    /// <summary>
    /// Ensures field set
    /// </summary>
    private void EnsureFieldSet()
    {
        ltlText.CssClass = "WireframeFieldsetLegend";
        PropertyEnvelopeCssClass = "WireframeFieldset";
    }


    /// <summary>
    /// Ensures multi-line text
    /// </summary>
    private void EnsureMultiLineText()
    {
        ltlText.CssClass = "WireframeText NoWrap";
        ltlText.Type = EditablePropertyTypeEnum.TextArea;
        resElem.Visible = false;
    }


    /// <summary>
    /// Ensures label
    /// </summary>
    private void EnsureLabel()
    {
        ltlText.CssClass = "WireframeText NoWrap";
        resElem.Visible = false;
    }


    /// <summary>
    /// Ensures text box
    /// </summary>
    private void EnsureTextArea()
    {
        ltlText.CssClass = "WireframeTextArea";
        ltlText.Type = EditablePropertyTypeEnum.TextArea;
    }


    /// <summary>
    /// Ensures text box
    /// </summary>
    private void EnsureTextBox()
    {
        ltlText.CssClass = "WireframeTextBox";
        imgElem.CssClass = "WireframeTextBoxIcon";
        ImageEnvelopeCssClass = "WireframePositionEnvelope";
        resElem.HorizontalOnly = true;
    }

    /// <summary>
    /// Ensure button properties
    /// </summary>
    private void EnsureButton()
    {
        ltlText.CssClass = "WireframeButton";
    }


    /// <summary>
    /// OnInit
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        SetupControl();
    }


    /// <summary>
    /// Reload data
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }

    #endregion
}
