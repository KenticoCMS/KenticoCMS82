using System;
using System.Globalization;
using System.Threading;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.Base;
using CMS.UIControls;
using CMS.DocumentEngine;
using CMS.ExtendedControls;

public partial class CMSModules_Widgets_Controls_Dashboard : CMSUserControl
{
    #region "Variables"

    private bool mHighlightDropableAreas = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the dashboard site name.
    /// </summary>
    public string DashboardSiteName
    {
        get
        {
            return PortalContext.DashboardSiteName;
        }
        set
        {
            PortalContext.DashboardSiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets the portal page instance.
    /// </summary>
    public PortalPage PortalPageInstance
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the literal control.
    /// </summary>
    public Literal TagsLiteral
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the element name.
    /// </summary>
    public string ResourceName
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the element name.
    /// </summary>
    public string ElementName
    {
        get;
        set;
    }


    /// <summary>
    /// Droppable areas are highlighted when widget dragged.
    /// </summary>
    public bool HighlightDropableAreas
    {
        get
        {
            return mHighlightDropableAreas;
        }
        set
        {
            mHighlightDropableAreas = value;
        }
    }


    /// <summary>
    /// If true zone border can be activated (+add widget button).
    /// </summary>
    public bool ActivateZoneBorder
    {
        get;
        set;
    }

    #endregion


    #region "Page events"
      
    /// <summary>
    /// Check security.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(ResourceName) && !String.IsNullOrEmpty(ElementName))
        {
            // Check UIProfile
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement(ResourceName, ElementName))
            {
                URLHelper.Redirect(UIHelper.GetInformationUrl("uiprofile.uinotavailable"));
            }
        }

        if (!CheckHashCode())
        {
            RedirectToAccessDenied(GetString("dashboard.invalidparameters"));
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Init the header tags
        TagsLiteral.Text = PortalPageInstance.HeaderTags;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Ensures dashboard initialization.
    /// </summary>
    public void SetupDashboard()
    {
        // Register placeholder for context menu
        ICMSPage page = Page as ICMSPage;
        if (page != null)
        {
            page.ContextMenuContainer = plcCtx;
        }

        if (PortalPageInstance == null)
        {
            throw new Exception("[DashboardControl.SetupDashboard] Portal page instance must be set.");
        }

        // Default settings for drag and drop for dashboard
        manPortal.HighlightDropableAreas = HighlightDropableAreas;
        manPortal.ActivateZoneBorder = ActivateZoneBorder;

        string dashboardName = QueryHelper.GetString("DashboardName", PersonalizationInfoProvider.UNDEFINEDDASHBOARD);

        // Set culture
        CultureInfo ci = CultureHelper.PreferredUICultureInfo;
        Thread.CurrentThread.CurrentCulture = ci;
        Thread.CurrentThread.CurrentUICulture = ci;

        // Init the page components
        PortalPageInstance.PageManager = manPortal;
        manPortal.SetMainPagePlaceholder(plc);

        string templateName = QueryHelper.GetString("templatename", String.Empty);

        PageTemplateInfo pti = PageTemplateInfoProvider.GetPageTemplateInfo(templateName);
        if (pti != null)
        {
            if (pti.PageTemplateType != PageTemplateTypeEnum.Dashboard)
            {
                RedirectToAccessDenied("dashboard.invalidpagetemplate");
            }

            // Prepare virtual page info
            PageInfo pi = PageInfoProvider.GetVirtualPageInfo(pti.PageTemplateId);
            pi.DocumentNamePath = "/" + templateName;
            
            DocumentContext.CurrentPageInfo = pi;

            // Set the design mode
            PortalContext.SetRequestViewMode(ViewModeEnum.DashboardWidgets);
            PortalContext.DashboardName = dashboardName;

            PortalPageInstance.ManagersContainer = plcManagers;
            PortalPageInstance.ScriptManagerControl = manScript;
        }
        else
        {
            RedirectToInformation(GetString("dashboard.notemplate"));
        }
    }


    /// <summary>
    /// Checks whether url parameters are valid.
    /// </summary>
    protected bool CheckHashCode()
    {
        // Get hashcode from querystring
        string hash = QueryHelper.GetString("hash", String.Empty);

        // Check whether url contains all reuired values
        if (QueryHelper.Contains("dashboardname") && !String.IsNullOrEmpty(hash))
        {
            // Try get custom hash values
            string hashValues = QueryHelper.GetString("hashvalues", String.Empty);

            string hashString = String.Empty;
            // Use default hash values
            if (String.IsNullOrEmpty(hashValues))
            {
                hashString = QueryHelper.GetString("dashboardname", String.Empty) + "|" + QueryHelper.GetString("templatename", String.Empty);
            }
            // Use custom hash values
            else
            {
                string[] values = hashValues.Split(';');
                foreach (string value in values)
                {
                    hashString += QueryHelper.GetString(value, String.Empty) + "|";
                }

                hashString = hashString.TrimEnd('|');
            }

            // Compare url hash with current hash
            return ((CMSString.Compare(hash, ValidationHelper.GetHashString(hashString), false) == 0));
        }
        return false;
    }

    #endregion
}