using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using CMS.Helpers;
using CMS.PortalControls;
using CMS.Base;


public partial class CMSWebParts_Wireframe_WireframeListComponent : CMSAbstractWireframeWebPart
{
    #region "Properties"

    /// <summary>
    /// Text
    /// </summary>
    public string Items
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Items"), String.Empty);
        }
        set
        {
            this.SetValue("Items", value);
            ltlText.Text = value;
        }
    }


    /// <summary>
    /// List type
    /// </summary>
    public string Type
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Type"), "");
        }
        set
        {
            this.SetValue("Type", value);
        }
    }


    /// <summary>
    /// Selected item index
    /// </summary>
    public string SelectedItem
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("SelectedItem"), "");
        }
        set
        {
            this.SetValue("SelectedItem", value);
        }
    }


    /// <summary>
    /// Data
    /// </summary>
    public string Data
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Data"), "");
        }
        set
        {
            this.SetValue("Data", value);
        }
    }

    #endregion


    #region "MenuBar properties"

    /// <summary>
    /// ItemCssClass
    /// </summary>
    public string ItemCssClass
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("ItemCssClass"), "");
        }
        set
        {
            this.SetValue("ItemCssClass", value);
        }
    }


    /// <summary>
    /// Selected item CSS class
    /// </summary>
    public string SelectedItemCssClass
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("SelectedItemCssClass"), "");
        }
        set
        {
            this.SetValue("SelectedItemCssClass", value);
        }
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
                case "linkbar":
                    ProcessLinkBar();
                    break;

                case "breadcrumbs":
                    ProcessBreadcrumbs();
                    break;

                case "list":
                    ProcessList();
                    break;

                case "grid":
                    ProcessGrid();
                    break;

                case "dropdown":
                    ProcessDropDown();
                    break;

                case "tagcloud":
                    ProcessTagCloud();
                    break;

                case "menubar":
                    ProcessMenuBar();
                    break;
            }
        }
    }


    /// <summary>
    /// Ensures breadcrumbs
    /// </summary>
    private void ProcessLinkBar()
    {
        ltlText.ItemFormat = "<span class=\"WireframeLink\">{0}</span>";
        ltlText.ItemSeparator = " | ";
        ltlText.Text = Items;
    }


    /// <summary>
    /// Ensures breadcrumbs
    /// </summary>
    private void ProcessBreadcrumbs()
    {
        ltlText.ItemFormat = "> <span class=\"WireframeLink\">{0}</span>";
        ltlText.Text = Items;
    }


    /// <summary>
    /// Ensures menu bar
    /// </summary>
    private void ProcessMenuBar()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("<li class=\"", ItemCssClass, " {1}\">{0}</li>");

        ltlText.ItemFormat = sb.ToString();
        ltlText.SelectedItemText = SelectedItemCssClass;
        ltlText.RenderAsTag = "ul";

        ltlText.Text = Items;
        ltlText.SelectedItems = SelectedItem;

        pnlEnvelope.RenderChildrenOnly = false;
        pnlEnvelope.CssClass = "WireframeMenuBar";
    }


    /// <summary>
    /// Ensures tag cloud
    /// </summary>
    private void ProcessTagCloud()
    {
        ltlText.ItemFormat = "<span class=\"WireframeTag\" style=\"font-size: {10-20}px\">{0}</span> ";
        ltlText.Text = Items;
        ltlText.CssClass = "WireframeTagCloud";

        EnsureDimensions(true, true);

        resElem.ResizedElementID = ltlText.ClientID;
    }


    /// <summary>
    /// Ensures grid behavior
    /// </summary>
    private void ProcessGrid()
    {
        ltlText.Text = Data;

        ltlText.ItemFormat = "<tr class=\"{2}\">{0}</tr>";
        ltlText.Attributes += " cellspacing=\"0\"";
        ltlText.SubItemFormat = "<td>{0}</td>";
        ltlText.RenderAsTag = "table";
        ltlText.PropertyName = "Data";
        ltlText.CssClass = "WireframeGrid";

        EnsureDimensions(true, true);

        resElem.ResizedElementID = ltlText.ClientID;
    }


    /// <summary>
    /// Ensures dropdown behavior
    /// </summary>
    private void ProcessDropDown()
    {
        ltlText.ItemFormat = "<option value=\"{3}\">{4}</option>";
        ltlText.Text = Items;
        ltlText.CssClass = "WireframeDropdown";
        ltlText.RenderAsTag = "select";

        EnsureDimensions(true, false);

        if (IsDesign)
        {
            ltlText.Attributes += String.Format("onchange=\"SetWebPartProperty('{0}', 'SelectedItem', this.selectedIndex + '')\"", this.ShortClientID);
        }

        resElem.ResizedElementID = ltlText.ClientID;
        resElem.HorizontalOnly = true;

        string script = "document.getElementById('" + ltlText.ClientID + "').selectedIndex = " + ValidationHelper.GetInteger(this.SelectedItem, 0);
        ScriptHelper.RegisterStartupScript(this, typeof(string), ltlText.ClientID + "_Init", ScriptHelper.GetScript(script));
    }


    /// <summary>
    /// Ensures list behavior
    /// </summary>
    private void ProcessList()
    {
        ltlText.ItemFormat = "<li>{0}</li>";
        ltlText.RenderAsTag = "ul";

        string type = this.Type;
        if (!String.IsNullOrEmpty(type))
        {
            ltlText.RenderAsTag = "ol";

            if (type != "1")
            {
                ltlText.Attributes += " type=\"" + type + "\"";
            }
        }

        ltlText.Text = Items;
    }


    /// <summary>
    /// Ensures width and height
    /// </summary>
    private void EnsureDimensions(bool width, bool height)
    {
        if (height)
        {
            // Height
            string h = WebPartHeight;
            if (!String.IsNullOrEmpty(h))
            {
                ltlText.CssStyle += "height: " + h + ";";
            }
        }

        if (width)
        {
            // Width
            string w = WebPartWidth;
            if (!String.IsNullOrEmpty(w))
            {
                ltlText.CssStyle += "width: " + w + ";";
            }
        }
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
