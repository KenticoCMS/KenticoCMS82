using System;

using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.FormEngine;

public partial class CMSModules_RecycleBin_Pages_RecycleBin_Documents : GlobalAdminPage
{
    #region "Page events"
    
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get site ID       
        int selectedSite = QueryHelper.GetInteger("toSite", 0);
        if (selectedSite == 0)
        {
            // Set site selector
            siteSelector.DropDownSingleSelect.AutoPostBack = true;
            siteSelector.OnlyRunningSites = false;
            siteSelector.AllowAll = false;

            siteSelector.UniSelector.SpecialFields.Add(new SpecialField()
            {
                Text = GetString("RecycleBin.AllSites"),
                Value = "0"
            });
               
            siteSelector.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;

            if (!RequestHelper.IsPostBack())
            {
                selectedSite = 0;
                siteSelector.Value = selectedSite;
            }
        }

        SiteInfo si = SiteInfoProvider.GetSiteInfo(ValidationHelper.GetInteger(siteSelector.Value, 0));
        if (si != null)
        {
            recycleBin.SiteName = si.SiteName;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Hide site selector if there are no sites
        if (!siteSelector.UniSelector.HasData)
        {
            pnlSiteSelector.Visible = false;
        }
        base.OnPreRender(e);
    }

    #endregion


    #region "Control events"

    /// <summary>
    /// Handles site selection change event.
    /// </summary>
    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        recycleBin.ReloadData();
    }

    #endregion
}