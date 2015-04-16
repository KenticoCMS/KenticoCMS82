using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Localization;
using CMS.UIControls;
using CMS.Helpers;
using CMS.DocumentEngine;
using CMS.SiteProvider;
using CMS.Membership;

using MenuItem = CMS.UIControls.UniMenuConfig.Item;
using SubMenuItem = CMS.UIControls.UniMenuConfig.SubItem;

using CMS.DataEngine;

public partial class CMSAdminControls_UI_UniMenu_OnSiteEdit_CultureMenu : CMSUserControl
{
    #region "Private variables"

    private Dictionary<string, string> mNodeCultures = null;
    private InfoDataSet<CultureInfo> mSiteCultures = null;
    private PageInfo pi = null;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets all the cultures of the current node.
    /// </summary>
    private Dictionary<string, string> NodeCultures
    {
        get
        {
            if (mNodeCultures == null)
            {
                // Get all language versions
                TreeNodeDataSet culturesDs = GetNodeCultures();
                mNodeCultures = new Dictionary<string, string>();

                // Create culture/UrlPath collection
                if (!DataHelper.DataSourceIsEmpty(culturesDs))
                {
                    foreach (DataRow dr in culturesDs.Tables[0].Rows)
                    {
                        string docCulture = ValidationHelper.GetString(dr["DocumentCulture"], String.Empty);
                        string urlPath = ValidationHelper.GetString(dr["DocumentUrlPath"], String.Empty);
                        mNodeCultures.Add(docCulture, urlPath);
                    }
                }
            }

            return mNodeCultures;
        }
    }


    /// <summary>
    /// Gets all the site cultures.
    /// </summary>
    private InfoDataSet<CultureInfo> SiteCultures
    {
        get
        {
            if (mSiteCultures == null)
            {
                mSiteCultures = CultureSiteInfoProvider.GetSiteCultures(SiteContext.CurrentSiteName);
            }

            return mSiteCultures;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        string prefferedCultureCode = LocalizationContext.PreferredCultureCode;
        string defaultString = GetString("general.default");

        InfoDataSet<CultureInfo> siteCultures = CultureSiteInfoProvider.GetSiteCultures(SiteContext.CurrentSiteName);
        pi = DocumentContext.CurrentPageInfo ?? DocumentContext.CurrentCultureInvariantPageInfo ?? new PageInfo();

        // Cultures button
        MenuItem cultureItem = new MenuItem();
        cultureItem.CssClass = "BigButton";
        cultureItem.ImageAlign = ImageAlign.Top;
        cultureItem.ImagePath = URLHelper.UnResolveUrl(UIHelper.GetFlagIconUrl(Page, prefferedCultureCode, "16x16"), SystemContext.ApplicationPath);
        cultureItem.Text = GetString("general.cultures");
        cultureItem.Tooltip = GetString("onsiteedit.languageselector");
        cultureItem.ImageAltText = GetString("general.cultures");

        // Add all cultures to the sub menu
        foreach (CultureInfo culture in siteCultures)
        {
            string iconUrl = UIHelper.GetFlagIconUrl(Page, culture.CultureCode, "16x16");
            string cultureName = culture.CultureName;
            string cultureCode = culture.CultureCode;
            string cultureShortName = culture.CultureShortName;

            if (cultureCode != prefferedCultureCode)
            {
                SubMenuItem menuItem = new SubMenuItem()
                {
                    Text = cultureName,
                    Tooltip = cultureName,
                    ImagePath = iconUrl,
                    ImageAltText = cultureName
                };

                // Build the web part image html
                bool translationExists = NodeCultures.ContainsKey(cultureCode);

                if (translationExists)
                {
                    // Assign click action which changes the document culture
                    menuItem.OnClientClick = "document.location.replace(" + ScriptHelper.GetString(URLHelper.UpdateParameterInUrl(RequestContext.CurrentURL, URLHelper.LanguageParameterName, cultureCode)) + ");";
                }
                else
                {
                    // Display the "Not translated" image
                    menuItem.RightImageIconClass = "icon-ban-sign";
                    menuItem.RightImageAltText = GetString("onsitedit.culturenotavailable");

                    // Assign click action -> Create new document culture
                    menuItem.OnClientClick = "NewDocumentCulture(" + pi.NodeID + ",'" + cultureCode + "');";
                }

                cultureItem.SubItems.Add(menuItem);
            }
            else
            {
                // Current culture
                cultureItem.Text = culture.CultureShortName;
                cultureItem.Tooltip = cultureName;
                cultureItem.ImagePath = iconUrl;
                cultureItem.ImageAltText = cultureName;
            }
        }

        btnCulture.Buttons.Add(cultureItem);
    }


    /// <summary>
    /// Returns all culture nodes
    /// </summary>
    protected TreeNodeDataSet GetNodeCultures()
    {
        // Get all language versions
        TreeProvider tp = new TreeProvider(MembershipContext.AuthenticatedUser);
        return tp.SelectNodes(SiteContext.CurrentSiteName, "/%", TreeProvider.ALL_CULTURES, false, null, "NodeID = " + pi.NodeID, "DocumentCulture", -1, false, 0, "DocumentCulture, DocumentUrlPath");
    }

    #endregion
}
