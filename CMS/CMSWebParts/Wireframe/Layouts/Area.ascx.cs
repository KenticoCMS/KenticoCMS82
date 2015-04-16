using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using AjaxControlToolkit;

using CMS.Helpers;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.Base;

public partial class CMSWebParts_Wireframe_Layouts_Area : CMSAbstractLayoutWebPart
{
    #region "Variables"

    private CMSWebPartZone zone = null;

    #endregion
    

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
    
    #endregion


    #region "Methods"

    /// <summary>
    /// Constructor
    /// </summary>
    public CMSWebParts_Wireframe_Layouts_Area()
    {
        SimpleRender = true;
    }


    /// <summary>
    /// Prepares the layout of the web part.
    /// </summary>
    protected override void PrepareLayout()
    {
        StartLayout(true);
                              
        // Add the zone
        zone = AddZone(ID + "_zone", ID, pnlZone);

        zone.LayoutType = ZoneLayoutTypeEnum.Free;
        zone.CssClass += " WireframeArea";

        zone.ZoneWidth = Width;
        zone.ZoneHeight = Height;

        FinishLayout(false);
    }


    /// <summary>
    /// Content loaded event handler
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties
    /// </summary>
    protected void SetupControl()
    {
        if (this.StopProcessing)
        {
            // Do not process
        }
        else
        {
            if (this.IsDesign)
            {
                pnlZone.CssClass = "WebPartZoneContent";

                if (ViewMode != ViewModeEnum.DesignDisabled)
                {
                    pnlActions.Visible = true;
                }
            }

            resElem.ResizedElementID = zone.ContainerClientID;
        }
    }

    #endregion
}