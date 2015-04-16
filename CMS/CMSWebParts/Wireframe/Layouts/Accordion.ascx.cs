using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using AjaxControlToolkit;

using CMS.Controls;
using CMS.Helpers;
using CMS.PortalControls;
using CMS.PortalEngine;

public partial class CMSWebParts_Wireframe_Layouts_Accordion : CMSAbstractLayoutWebPart
{
    #region "Properties"

    /// <summary>
    /// Number of panes
    /// </summary>
    public int Panes
    {
        get
        {
            return ValidationHelper.GetInteger(this.GetValue("Panes"), 2);
        }
        set
        {
            this.SetValue("Panes", value);
        }
    }


    /// <summary>
    /// Pane headers
    /// </summary>
    public string PaneHeaders
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("PaneHeaders"), "");
        }
        set
        {
            this.SetValue("PaneHeaders", value);
        }
    }


    /// <summary>
    /// Active pane index
    /// </summary>
    public int ActivePaneIndex
    {
        get
        {
            return ValidationHelper.GetInteger(this.GetValue("ActivePaneIndex"), 0);
        }
        set
        {
            this.SetValue("ActivePaneIndex", value);
        }
    }


    /// <summary>
    /// Require opened pane
    /// </summary>
    public bool RequireOpenedPane
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("RequireOpenedPane"), true);
        }
        set
        {
            this.SetValue("RequireOpenedPane", value);
        }
    }


    /// <summary>
    /// Width
    /// </summary>
    public string Width
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Width"), "");
        }
        set
        {
            this.SetValue("Width", value);
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

        // Pane headers
        string[] headers = TextHelper.EnsureLineEndings(PaneHeaders, "\n").Split('\n');

        for (int i = 1; i <= Panes; i++)
        {
            // Create new pane
            AccordionPane pane = new AccordionPane();
            pane.ID = "pane" + i;

            // Prepare the header
            string header = null;
            if (headers.Length >= i)
            {
                header = ResHelper.LocalizeString(headers[i - 1]);
            }
            if (String.IsNullOrEmpty(header))
            {
                header = "Pane " + i;
            }

            string title = header;

            if (IsDesign)
            {
                header = EditableWebPartProperty.GetHTMLCode(null, this, "PaneHeaders", i, EditablePropertyTypeEnum.TextBox, header, null, null, null, true);
            }
            else
            {
                header = EditableWebPartProperty.ApplyReplacements(HttpUtility.HtmlEncode(header), false);
            }

            pane.Header = new TextTransformationTemplate(header);
            acc.Panes.Add(pane);

            var zone = AddZone(ID + "_" + i, title, pane.ContentContainer);
            zone.Wireframe = true;
        }

        // Setup the accordion
        if ((ActivePaneIndex >= 1) && (ActivePaneIndex <= acc.Panes.Count))
        {
            acc.SelectedIndex = ActivePaneIndex - 1;
        }

        // If no active pane is selected and doesn't require opened one, do not preselect any
        if (!acc.RequireOpenedPane && (ActivePaneIndex < 0))
        {
            acc.SelectedIndex = -1;
        }

        // Wireframe design
        acc.CssClass = "WireframeAccordion";
        acc.ContentCssClass = "WireframeAccordionContent";
        acc.HeaderCssClass = "WireframeAccordionHeader";
        acc.HeaderSelectedCssClass = "WireframeAccordionSelectedHeader";

        acc.RequireOpenedPane = RequireOpenedPane;
        acc.AutoSize = AutoSize.None;

        // Set width / height
        string width = Width;
        if (!String.IsNullOrEmpty(width))
        {
            acc.Width = new Unit(width);
        }

        if (IsDesign && AllowDesignMode)
        {
            // Pane actions
            if (Panes > 1)
            {
                AppendRemoveAction(GetString("Layout.RemoveLastPane"), "Panes", "icon-times", null);
                Append(" ");
            }

            AppendAddAction(GetString("Layout.AddPane"), "Panes", "icon-plus", null);

            resElem.ResizedElementID = acc.ClientID;
        }

        // Render the actions
        string actions = FinishLayout(false);
        if (!String.IsNullOrEmpty(actions))
        {
            pnlActions.Visible = true;
            ltlActions.Text = actions;
        }
    }

    #endregion
}