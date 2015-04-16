using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using AjaxControlToolkit;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.Base;

public partial class CMSWebParts_Wireframe_Layouts_Tabs : CMSAbstractLayoutWebPart
{
    #region "Public properties"

    /// <summary>
    /// Number of tabs.
    /// </summary>
    public int Tabs
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Tabs"), 2);
        }
        set
        {
            SetValue("Tabs", value);
        }
    }


    /// <summary>
    /// Tab headers.
    /// </summary>
    public string TabHeaders
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TabHeaders"), "");
        }
        set
        {
            SetValue("TabHeaders", value);
        }
    }


    /// <summary>
    /// Active tab index.
    /// </summary>
    public int ActiveTabIndex
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ActiveTabIndex"), 0);
        }
        set
        {
            SetValue("ActiveTabIndex", value);
        }
    }
    

    /// <summary>
    /// Tab strip placement.
    /// </summary>
    public string TabStripPlacement
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TabStripPlacement"), "top");
        }
        set
        {
            SetValue("TabStripPlacement", value);
        }
    }


    /// <summary>
    /// Width.
    /// </summary>
    public string Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Width"), "");
        }
        set
        {
            SetValue("Width", value);
        }
    }
    
    /// <summary>
    /// Tabs CSS class.
    /// </summary>
    public string TabsCSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TabsCSSClass"), "");
        }
        set
        {
            SetValue("TabsCSSClass", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Prepares the layout of the web part.
    /// </summary>
    protected override void PrepareLayout()
    {
        StartLayout(true);

        // Tab headers
        string[] headers = TextHelper.EnsureLineEndings(TabHeaders, "\n").Split('\n');

        if ((ActiveTabIndex >= 1) && (ActiveTabIndex <= Tabs))
        {
            tabs.ActiveTabIndex = ActiveTabIndex - 1;
        }

        for (int i = 1; i <= Tabs; i++)
        {
            // Create new tab
            TabPanel tab = new TabPanel();
            tab.ID = "tab" + i;

            // Prepare the header
            string header = null;
            if (headers.Length >= i)
            {
                header = ResHelper.LocalizeString(headers[i - 1]);
            }
            if (String.IsNullOrEmpty(header))
            {
                header = "Tab " + i;
            }

            tabs.Tabs.Add(tab);

            AddZone(ID + "_" + i, header, tab);

            if (IsDesign)
            {
                header = EditableWebPartProperty.GetHTMLCode(null, this, "TabHeaders", i, EditablePropertyTypeEnum.TextBox, header, null, null, null, true);
            }
            else
            {
                header = EditableWebPartProperty.ApplyReplacements(HttpUtility.HtmlEncode(header), false);
            }

            tab.HeaderText = header;
        }

        // Wireframe design
        tabs.TabStripPlacement = GetTabStripPlacement(TabStripPlacement);
        tabs.CssClass = "WireframeTabs";

        // Set width / height
        string width = Width;
        if (!String.IsNullOrEmpty(width))
        {
            tabs.Width = new Unit(width);
        }
        
        if (IsDesign && AllowDesignMode)
        {
            // Pane actions
            if (Tabs > 1)
            {
                AppendRemoveAction(GetString("Layout.RemoveTab"), "Tabs", "icon-times", null);
                Append(" ");
            }

            AppendAddAction(GetString("Layout.AddTab"), "Tabs", "icon-plus", null);
            
            resElem.ResizedElementID = tabs.ClientID;
        }

        // Render the actions
        string actions = FinishLayout(false);
        if (!String.IsNullOrEmpty(actions))
        {
            pnlActions.Visible = true;
            ltlActions.Text = actions;
        }
    }

    
    /// <summary>
    /// Gets the tab strip placement based on the string representation
    /// </summary>
    /// <param name="placement">Placement</param>
    protected TabStripPlacement GetTabStripPlacement(string placement)
    {
        switch (placement.ToLowerCSafe())
        {
            case "bottom":
                return AjaxControlToolkit.TabStripPlacement.Bottom;

            case "bottomright":
                return AjaxControlToolkit.TabStripPlacement.BottomRight;

            case "topright":
                return AjaxControlToolkit.TabStripPlacement.TopRight;

            default:
                return AjaxControlToolkit.TabStripPlacement.Top;
        }
    }

    #endregion
}