using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using CMS;
using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

[assembly: RegisterCustomClass("CultureSitesExtender", typeof(CultureSitesExtender))]

/// <summary>
/// Culture list extender
/// </summary>
public class CultureSitesExtender : ControlExtender<UniSelector>
{
    public override void OnInit()
    {
        Control.OnSelectionChanged += Control_OnSelectionChanged;
    }


    private void Control_OnSelectionChanged(object sender, EventArgs e)
    {
        int cultureId = QueryHelper.GetInteger("objectid", 0);
        CultureInfo culture = GetSafeCulture(cultureId);
        
        // Get the current sites
        string currentValues = GetCultureSites(cultureId);
        // Get sites from selector
        string newValues = ValidationHelper.GetString(Control.Value, null);

        bool somethingChanged = false;
        bool hasErrors = false;
        int siteId;
        SiteInfo si;
        DataSet nodes;
        string[] newItems;

        // Remove old items
        string items = DataHelper.GetNewItemsInList(newValues, currentValues);
        if (!string.IsNullOrEmpty(items))
        {
            newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                TreeProvider tree = new TreeProvider(Control.CurrentUser);
                // Add all new items to site
                foreach (string item in newItems)
                {
                    siteId = ValidationHelper.GetInteger(item, 0);

                    si = SiteInfoProvider.GetSiteInfo(siteId);
                    if ((si != null) && (culture != null))
                    {
                        // Check if site does not contain document from this culture
                        nodes = tree.SelectNodes(si.SiteName, "/%", culture.CultureCode, false, null, null, null, -1, false, 1, "NodeID");
                        if (DataHelper.DataSourceIsEmpty(nodes))
                        {
                            CultureSiteInfoProvider.RemoveCultureFromSite(culture.CultureID, siteId);
                            somethingChanged = true;
                        }
                        else
                        {
                            hasErrors = true;
                            Control.ShowError(string.Format(Control.GetString("culture.ErrorRemoveSiteFromCulture"), si.DisplayName));
                            continue;
                        }
                    }
                }
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(currentValues, newValues);
        if (!string.IsNullOrEmpty(items))
        {
            newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                // Add all new items to site
                foreach (string item in newItems)
                {
                    siteId = ValidationHelper.GetInteger(item, 0);

                    // Add cullture to site
                    si = SiteInfoProvider.GetSiteInfo(siteId);
                    if (si != null)
                    {
                        if (CultureSiteInfoProvider.LicenseVersionCheck(si.DomainName, FeatureEnum.Multilingual, ObjectActionEnum.Insert))
                        {
                            CultureSiteInfoProvider.AddCultureToSite(culture.CultureID, siteId);
                            somethingChanged = true;
                        }
                        else
                        {
                            hasErrors = true;
                            Control.ShowError(Control.GetString("licenselimitation.siteculturesexceeded"));
                            break;
                        }
                    }
                }
            }
        }

        // If there were some errors, reload uniselector
        if (hasErrors)
        {
            Control.Value = GetCultureSites(cultureId);
            Control.Reload(true);
        }

        if (somethingChanged)
        {
            Control.ShowChangesSaved();
        }
    }


    private static CultureInfo GetSafeCulture(int cultureId)
    {
        if (cultureId <= 0)
        {
            return null;
        }

        try
        {
            return CultureInfoProvider.GetCultureInfo(cultureId);
        }
        catch (Exception)
        {
            return null;
        }
    }


    /// <summary>
    /// Returns string with culture sites.
    /// </summary>    
    private string GetCultureSites(int cultureId)
    {
        DataSet cultures = CultureSiteInfoProvider.GetCultureSites("SiteID", "CultureID = " + cultureId, null, 0);
        if (!DataHelper.DataSourceIsEmpty(cultures))
        {
            return TextHelper.Join(";", DataHelper.GetStringValues(cultures.Tables[0], "SiteID"));
        }

        return null;
    }
}