using System;
using System.Data;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_CssStylesheets_Pages_CssStylesheet_General : CMSDeskPage
{
    #region "Variables"

    protected int cssStylesheetId = 0;
    private CssStylesheetInfo si = null;
    private SiteInfo mSite = null;
    private bool isDialog = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Site info object, used for test whether css sheet belongs to site
    /// </summary>
    private SiteInfo CurrentSite
    {
        get
        {
            if (mSite == null)
            {
                int siteId = QueryHelper.GetInteger("siteid", 0);
                if (siteId > 0)
                {
                    mSite = SiteInfoProvider.GetSiteInfo(siteId);
                }
                if (mSite == null)
                {
                    mSite = SiteContext.CurrentSite;
                }
            }
            return mSite;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnPreInit(EventArgs e)
    {
        RequireSite = false;

        CurrentUserInfo currentUser = MembershipContext.AuthenticatedUser;

        // Check for UI permissions
        if (!currentUser.IsAuthorizedPerUIElement("CMS.Content", new string[] { "Properties", "Properties.General", "General.Design", "Design.EditCSSStylesheets" }, SiteContext.CurrentSiteName))
        {
            RedirectToUIElementAccessDenied("CMS.Content", "Properties;Properties.General;General.Design;Design.EditCSSStylesheets");
        }

        // Page has been opened in CMSDesk and only stylesheet style editing is allowed
        isDialog = (QueryHelper.GetBoolean("dialog", false) || QueryHelper.GetBoolean("isindialog", false));

        // Prevent replacing of the master page with dialog master page
        RequiresDialog = false;

        if (isDialog)
        {
            // Check hash
            if (!QueryHelper.ValidateHash("hash", "objectid", null, true))
            {
                URLHelper.Redirect(ResolveUrl(string.Format("~/CMSMessages/Error.aspx?title={0}&text={1}", ResHelper.GetString("dialogs.badhashtitle"), ResHelper.GetString("dialogs.badhashtext"))));
            }

            // Check 'Design Web site' permission 
            if (!currentUser.IsAuthorizedPerResource("CMS.Design", "Design"))
            {
                RedirectToAccessDenied("CMS.Design", "Design");
            }
        }
        else
        {
            CheckGlobalAdministrator();
        }

        string stylesheet = QueryHelper.GetString("objectid", "0");

        // If default stylesheet defined and selected, choose it
        if (stylesheet == "default")
        {
            si = PortalContext.CurrentSiteStylesheet;
        }

        // Default stylesheet not selected try to find stylesheet selected
        if (si != null)
        {
            cssStylesheetId = si.StylesheetID;
        }
        else
        {
            cssStylesheetId = ValidationHelper.GetInteger(stylesheet, 0);
            if (cssStylesheetId > 0)
            {
                // Get the stylesheet
                si = CssStylesheetInfoProvider.GetCssStylesheetInfo(cssStylesheetId);
            }
        }

        SetEditedObject(si, null);

        // Check site association in case that user is not admin
        var checkSiteAssociation = !currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin);

        if ((si != null) && isDialog && checkSiteAssociation)
        {
            // Check if stylesheet is under current site
            int siteId = (CurrentSite != null) ? CurrentSite.SiteID : 0;
            DataSet ds = CssStylesheetSiteInfoProvider.GetCssStylesheetSites()
                .WhereEquals("SiteID", siteId)
                .WhereEquals("StylesheetID", si.StylesheetID)
                .TopN(1);

            if (DataHelper.DataSourceIsEmpty(ds))
            {
                URLHelper.Redirect(ResolveUrl(string.Format("~/CMSMessages/Error.aspx?title={0}&text={1}", ResHelper.GetString("cssstylesheet.errorediting"), ResHelper.GetString("cssstylesheet.notallowedtoedit"))));
            }
        }

        base.OnPreInit(e);
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Register client ID for autoresize of codemirror
        ucHierarchy.RegisterEnvelopeClientID();
    }


    protected override void CreateChildControls()
    {
        ScriptHelper.RegisterWOpenerScript(Page);

        if (si != null)
        {
            UIContext.EditedObject = si;
            ucHierarchy.PreviewObjectName = si.StylesheetName;
        }

        ucHierarchy.IgnoreSessionValues = isDialog;
        ucHierarchy.ShowPanelSeparator = true;
        ucHierarchy.StorePreviewScrollPosition = true;
        ucHierarchy.DefaultPreviewPath = "/";

        // Prevent displaying footer in dialog mode
        ucHierarchy.DialogMode = isDialog;

        base.CreateChildControls();
    }

    #endregion
}
