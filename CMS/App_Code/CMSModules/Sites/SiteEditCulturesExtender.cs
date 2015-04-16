using System;
using System.Data;
using System.Web.UI;

using CMS;
using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.UIControls;

[assembly: RegisterCustomClass("SiteEditCulturesExtender", typeof(SiteEditCulturesExtender))]

/// <summary>
/// Site edit culture extender.
/// </summary>
public class SiteEditCulturesExtender : ControlExtender<UniSelector>
{
    #region "Variables"

    private SiteInfo siteInfo;
    private string currentValues;
    private const string ASSIGN_ARGUMENT_NAME = "assign";
    bool reloadData = false;

    #endregion


    #region "Methods"

    /// <summary>
    /// Extender initialization.
    /// </summary>
    public override void OnInit()
    {
        Control.OnSelectionChanged += Control_OnSelectionChanged;
        Control.Page.Load += Page_Load;
        Control.Page.PreRender += Page_PreRender;

        siteInfo = Control.UIContext.EditedObject as SiteInfo;
    }


    /// <summary>
    /// Page pre-render event handling.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (siteInfo == null)
        {
            return;
        }

        if (reloadData)
        {
            Control.Visible = true;
            Control.Reload(true);
        }

        // Check if site hasn't assigned more cultures than license approve
        if (!CultureSiteInfoProvider.LicenseVersionCheck(siteInfo.DomainName, FeatureEnum.Multilingual, ObjectActionEnum.Insert))
        {
            Control.ButtonAddItems.Enabled = false;
        }
        else if (!CultureSiteInfoProvider.LicenseVersionCheck(siteInfo.DomainName, FeatureEnum.Multilingual, ObjectActionEnum.Edit))
        {
            Control.ShowError(ResHelper.GetString("licenselimitation.siteculturesexceeded"));
            Control.ButtonAddItems.Enabled = false;
        }
    }


    /// <summary>
    /// Page load event handling.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (siteInfo == null)
        {
            return;
        }

        HandleReAssigningCulture();

        Control.Visible = true;
        bool multilingual = LicenseHelper.CheckFeature(URLHelper.GetDomainName(siteInfo.DomainName), FeatureEnum.Multilingual);
        bool cultureOnSite = CultureSiteInfoProvider.IsCultureOnSite(CultureHelper.GetDefaultCultureCode(siteInfo.SiteName), siteInfo.SiteName);
        if (!multilingual && !cultureOnSite)
        {
            Control.Visible = false;

            // Add link that assign the default content culture to the site
            LocalizedHyperlink linkButton = new LocalizedHyperlink()
            {
                ResourceString = "sitecultures.assigntodefault",
                NavigateUrl = "javascript:" + ControlsHelper.GetPostBackEventReference(Control.Page, ASSIGN_ARGUMENT_NAME) + ";"
            };

            Control.Parent.Controls.Add(linkButton);
        }
        else
        {
            // Redirect only if cultures not exceeded => to be able to unassign
            if (!CultureSiteInfoProvider.LicenseVersionCheck(siteInfo.DomainName, FeatureEnum.Multilingual, ObjectActionEnum.Edit))
            {
                LicenseHelper.CheckFeatureAndRedirect(URLHelper.GetDomainName(siteInfo.DomainName), FeatureEnum.Multilingual);
            }
        }

        // Get the active cultures from DB
        DataSet ds = CultureInfoProvider.GetCultures("CultureID IN (SELECT CultureID FROM CMS_SiteCulture WHERE SiteID = " + siteInfo.SiteID + ")", null, 0, "CultureID");
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            currentValues = TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], "CultureID"));
        }  
    }


    /// <summary>
    /// Handles OnSelectionChanged event of the UniSelector.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void Control_OnSelectionChanged(object sender, EventArgs e)
    {
        if (siteInfo == null)
        {
            return;
        }

        bool reloadNeeded = false;

        // Remove old items
        string newValues = ValidationHelper.GetString(Control.Value, String.Empty);
        string removedCultures = DataHelper.GetNewItemsInList(newValues, currentValues);
        if (!String.IsNullOrEmpty(removedCultures))
        {
            string[] removedCultureIDs = removedCultures.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (removedCultureIDs != null)
            {
                // Initialize tree provider
                TreeProvider tree = new TreeProvider();

                // Add all new items to site
                foreach (string cultureID in removedCultureIDs)
                {
                    CultureInfo ci = CultureInfoProvider.GetCultureInfo(ValidationHelper.GetInteger(cultureID, 0));
                    if (ci != null)
                    {
                        // Get the documents assigned to the culture being removed
                        DataSet nodes = tree.SelectNodes(siteInfo.SiteName, "/%", ci.CultureCode, false, null, null, null, -1, false);
                        if (DataHelper.DataSourceIsEmpty(nodes))
                        {
                            CultureSiteInfoProvider.RemoveCultureFromSite(ci.CultureCode, siteInfo.SiteName);
                        }
                        else
                        {
                            reloadNeeded = true;
                            Control.AddError(String.Format(ResHelper.GetString("site_edit_cultures.errorremoveculturefromsite"), ci.CultureCode) + '\n', null);
                        }
                    }
                }
            }
        }

        // Catch license limitations Exception  
        try
        {
            // Add new items
            string newCultures = DataHelper.GetNewItemsInList(currentValues, newValues);
            if (!String.IsNullOrEmpty(newCultures))
            {
                string[] newCultureIDs = newCultures.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (newCultureIDs != null)
                {
                    // Add all new items to site
                    foreach (string cultureID in newCultureIDs)
                    {
                        int id = ValidationHelper.GetInteger(cultureID, 0);
                        CultureSiteInfoProvider.AddCultureToSite(id, siteInfo.SiteID);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            reloadNeeded = true;
            Control.ShowError(ex.Message);
        }

        if (reloadNeeded)
        {
            // Get the active cultures from DB
            DataSet ds = CultureInfoProvider.GetCultures("CultureID IN (SELECT CultureID FROM CMS_SiteCulture WHERE SiteID = " + siteInfo.SiteID + ")", null, 0, "CultureID");
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                currentValues = TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], "CultureID"));
                Control.Value = currentValues;
                Control.Reload(true);
            }
        }
    }


    /// <summary>
    /// Assign the culture that is set as default content culture to the current site.
    /// </summary>
    private void HandleReAssigningCulture()
    {
        if (RequestHelper.IsPostBack())
        {
            string arg = ValidationHelper.GetString(Control.Page.Request[Page.postEventArgumentID], String.Empty);
            if (arg.EqualsCSafe(ASSIGN_ARGUMENT_NAME))
            {
                string culture = CultureHelper.GetDefaultCultureCode(siteInfo.SiteName);

                // Only default content culture is allowed to be assigned to the site in case there is no multilingual license
                CultureSiteInfoProvider.RemoveSiteCultures(siteInfo.SiteName);
                CultureSiteInfoProvider.AddCultureToSite(culture, siteInfo.SiteName);

                // Get info object of the default content culture to set value of the UniSelector
                CultureInfo ci = CultureInfoProvider.GetCultureInfoForCulture(culture);
                if (ci != null)
                {
                    Control.Value = Convert.ToString(ci.CultureID);
                    reloadData = true;
                }

                Control.ShowChangesSaved();
            }
        }
    }

    #endregion
}
