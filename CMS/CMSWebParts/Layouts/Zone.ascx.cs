using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using AjaxControlToolkit;

using CMS.Helpers;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.Base;

public partial class CMSWebParts_Layouts_Zone : CMSAbstractLayoutWebPart
{
    #region "Properties"

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
    /// Height.
    /// </summary>
    public string Height
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Height"), "");
        }
        set
        {
            SetValue("Height", value);
        }
    }


    /// <summary>
    /// Zone CSS class.
    /// </summary>
    public string ZoneCSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ZoneCSSClass"), "");
        }
        set
        {
            SetValue("ZoneCSSClass", value);
        }
    }


    /// <summary>
    /// Location.
    /// </summary>
    public string Location
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Location"), "");
        }
        set
        {
            SetValue("Location", value);
        }
    }


    /// <summary>
    /// Vertical offset.
    /// </summary>
    public int VerticalOffset
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("VerticalOffset"), 0);
        }
        set
        {
            SetValue("VerticalOffset", value);
        }
    }


    /// <summary>
    /// Horizontal offset.
    /// </summary>
    public int HorizontalOffset
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("HorizontalOffset"), 0);
        }
        set
        {
            SetValue("HorizontalOffset", value);
        }
    }


    /// <summary>
    /// Scroll effect duration (ms).
    /// </summary>
    public int ScrollEffectDuration
    {
        get
        {
            int result = ValidationHelper.GetInteger(GetValue("ScrollEffectDuration"), 100);
            if (result <= 0)
            {
                result = 100;
            }

            return result;
        }
        set
        {
            SetValue("ScrollEffectDuration", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Prepares the layout of the web part.
    /// </summary>
    protected override void PrepareLayout()
    {
        string location = Location;

        bool alwaysVisible = !String.IsNullOrEmpty(location);

        StartLayout();

        if (IsDesign)
        {
            Append("<table class=\"LayoutTable\" cellspacing=\"0\">");

            if (ViewModeIsDesign())
            {
                Append("<tr><td class=\"LayoutHeader\" colspan=\"2\">");

                // Add header container
                AddHeaderContainer();

                Append("</td></tr>");
            }

            Append("<tr><td>");
        }

        string style = null;

        // Width
        string width = Width;
        if (!String.IsNullOrEmpty(width))
        {
            style += " width: " + width + ";";
        }

        // Height
        string height = Height;
        if (!String.IsNullOrEmpty(height))
        {
            style += " height: " + height + ";";
        }

        string cssclass = ZoneCSSClass;

        // Render the envelope if needed
        bool renderEnvelope = IsDesign || !String.IsNullOrEmpty(style) || !String.IsNullOrEmpty(cssclass);
        if (renderEnvelope)
        {
            Append("<div");

            if (IsDesign)
            {
                Append(" id=\"", ShortClientID, "_env\"");
            }

            if (!String.IsNullOrEmpty(style))
            {
                Append(" style=\"", style, "\"");
            }

            if (!String.IsNullOrEmpty(cssclass))
            {
                Append(" class=\"", cssclass, "\"");
            }

            Append(">");
        }

        if (alwaysVisible)
        {
            // Add the extender
            AlwaysVisibleControlExtender av = new AlwaysVisibleControlExtender();
            av.TargetControlID = "pnlEx";
            av.ID = "avExt";

            // Horizontal location
            if (location.EndsWithCSafe("left", true))
            {
                av.HorizontalSide = HorizontalSide.Left;
            }
            else if (location.EndsWithCSafe("center", true))
            {
                av.HorizontalSide = HorizontalSide.Center;
            }
            else if (location.EndsWithCSafe("right", true))
            {
                av.HorizontalSide = HorizontalSide.Right;
            }

            // Horizontal location
            if (location.StartsWithCSafe("top", true))
            {
                av.VerticalSide = VerticalSide.Top;
            }
            else if (location.StartsWithCSafe("middle", true))
            {
                av.VerticalSide = VerticalSide.Middle;
            }
            else if (location.StartsWithCSafe("bottom", true))
            {
                av.VerticalSide = VerticalSide.Bottom;
            }

            // Offsets
            av.HorizontalOffset = HorizontalOffset;
            av.VerticalOffset = VerticalOffset;

            av.ScrollEffectDuration = ScrollEffectDuration / 1000f;

            // Add the extender
            Controls.Add(av);
        }

        // Add the zone
        CMSWebPartZone zone = AddZone(ID + "_zone", ID);

        if (renderEnvelope)
        {
            Append("</div>");
        }

        if (IsDesign)
        {
            Append("</td>");

            // Resizers
            if (AllowDesignMode)
            {
                // Vertical resizer
                Append("<td class=\"HorizontalResizer\" onmousedown=\"", GetHorizontalResizerScript("env", "Width", false, null), " return false;\">&nbsp;</td></tr><tr>");

                // Horizontal resizer
                Append("<td class=\"VerticalResizer\" onmousedown=\"", GetVerticalResizerScript("env", "Height"), " return false;\">&nbsp;</td>");
                Append("<td class=\"BothResizer\" onmousedown=\"", GetBothResizerScript("env", "Width", "Height"), " return false;\">&nbsp;</td>");
            }

            Append("</tr></table>");
        }

        // Panel for extender
        PlaceHolder pnlEx = new PlaceHolder();
        pnlEx.ID = "pnlEx";
        //pnlEx.Visible = false;

        AddControl(pnlEx);


        FinishLayout();
    }


    protected override void Render(HtmlTextWriter writer)
    {
        if (!String.IsNullOrEmpty(Location))
        {
            // Ensure the envelope for placing elsewhere
            writer.Write("<div id=\"" + ClientID + "_pnlEx\" style=\"z-index: 9901;\">");

            base.Render(writer);

            writer.Write("</div>");
        }
        else
        {
            // Standard rendering of single zone
            base.Render(writer);
        }
    }

    #endregion
}