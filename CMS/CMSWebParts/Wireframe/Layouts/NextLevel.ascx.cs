using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Helpers;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.WorkflowEngine;
using CMS.DocumentEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSWebParts_Wireframe_Layouts_NextLevel : CMSAbstractWebPart
{
    /// <summary>
    /// Constructor
    /// </summary>
    public CMSWebParts_Wireframe_Layouts_NextLevel()
    {
        HideHeader = true;
    }


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
            // Do nothing
        }
        else
        {
            partPlaceholder.ID = ID;

            if (PortalContext.IsDesignMode(PortalContext.ViewMode))
            {
                pnlClass.CssClass = "WireframeNextPageLevel";

                if (PortalContext.IsDesignMode(this.ViewMode))
                {
                    pnlLevel.CssClass = "WireframeNextPageLevelWebPart";
                }
            }

            // Apply width / height
            string height = WebPartHeight;
            if (!String.IsNullOrEmpty(height))
            {
                pnlLevel.Style.Add("height", height);
            }

            string width = WebPartWidth;
            if (!String.IsNullOrEmpty(width))
            {
                pnlLevel.Style.Add("width", width);
            }

            resElem.ResizedElementID = pnlLevel.ClientID;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (partPlaceholder.PageInfo == null)
        {
            this.lblNextLevel.Visible = true;
            
            //pnlClass.CssClass = "WireframeNextPageLevel";
        }
    }
}