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

public partial class CMSWebParts_Wireframe_Components_Dialog : CMSAbstractWireframeWebPart
{
    #region "Properties"

    /// <summary>
    /// Title
    /// </summary>
    public string Title
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Title"), "Enter title");
        }
        set
        {
            this.SetValue("Title", value);
        }
    }


    /// <summary>
    /// Text
    /// </summary>
    public string Text
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Text"), "Enter dialog text");
        }
        set
        {
            this.SetValue("Text", value);
        }
    }


    /// <summary>
    /// Buttons
    /// </summary>
    public string Buttons
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Buttons"), "Yes\nNo");
        }
        set
        {
            this.SetValue("Buttons", value);
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
            ltlTitle.Text = Title;
            ltlButtons.Text = Buttons;

            ltlButtons.ItemFormat = "<li class=\"WireframeButton\">{0}</li>";

            // Height
            string h = WebPartHeight;
            if (!String.IsNullOrEmpty(h))
            {
                pnlDialog.Height = new Unit(h);
            }

            // Width
            string w = WebPartWidth;
            if (!String.IsNullOrEmpty(w))
            {
                pnlDialog.Width = new Unit(w);
            }

            resElem.ResizedElementID = pnlDialog.ClientID;
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

    #endregion
}