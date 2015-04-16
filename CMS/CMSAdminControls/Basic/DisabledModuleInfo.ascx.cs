using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.UIControls;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.Helpers;
using CMS.DataEngine;
using CMS.Modules;

public partial class CMSAdminControls_Basic_DisabledModuleInfo : CMSUserControl
{
    #region "Variables"

    private String mSettingsKeys = String.Empty;
    private String mConfigKeys = String.Empty;

    private List<String> mInfoTexts = new List<string>();
    private Panel mParentPanel;
    private String mInfoText = String.Empty;

    private bool settingsChecked = false;
    private bool mShowButtons = true;

    private String mSiteObjects = String.Empty;
    private String mGlobalObjects = String.Empty;
    private DisabledModuleScope mKeyScope = DisabledModuleScope.Both;
    private bool mReloadUIWhenModuleEnabled = true;


    public CMSAdminControls_Basic_DisabledModuleInfo()
    {
        AtLeastOne = false;
        SettingsEnabled = false;
        SiteOrGlobal = false;
    }

    #endregion


    #region "Properties"

    /// <summary>
    /// Info text to show
    /// </summary>
    public String InfoText
    {
        get
        {
            return mInfoText;
        }
        set
        {
            mInfoText = value;
        }
    }


    /// <summary>
    /// Scope of key check. It's used only for 'SettingsKeys' property. Do not affect 'GlobalObjects' and 'SiteObjects' properties.
    /// </summary>
    public DisabledModuleScope KeyScope
    {
        get
        {
            return mKeyScope;
        }
        set
        {
            mKeyScope = value;
        }
    }


    /// <summary>
    /// Collection of texts, used for multiple info labels for multiple setting keys
    /// </summary>
    public List<String> InfoTexts
    {
        get
        {
            return mInfoTexts;
        }
        set
        {
            mInfoTexts = value;
        }
    }


    /// <summary>
    /// Get/set sitename. Store sitename to viewstate - in case of save, before sitename is known.
    /// </summary>
    public String SiteName
    {
        get
        {
            string msiteName = ValidationHelper.GetString(ViewState["SiteName"], null);

            return msiteName ?? SiteContext.CurrentSiteName;
        }
        set
        {
            ViewState["SiteName"] = value;
        }
    }


    /// <summary>
    /// Settings keys to check delimited by ';'
    /// </summary>
    public String SettingsKeys
    {
        get
        {
            return mSettingsKeys;
        }
        set
        {
            mSettingsKeys = value;
        }
    }


    /// <summary>
    /// Web.config keys to check delimited by ';'. If enabled through web.config keys, does not check the settings and considered enabled.
    /// </summary>
    public String ConfigKeys
    {
        get
        {
            return mConfigKeys;
        }
        set
        {
            mConfigKeys = value;
        }
    }


    /// <summary>
    /// Parent panel, used for hiding info row if no module is disabled. Is automatically hidden when no module is disabled.
    /// </summary>
    public Panel ParentPanel
    {
        get
        {
            return mParentPanel;
        }
        set
        {
            mParentPanel = value;
        }
    }


    /// <summary>
    /// If true, global or site settings are check separately
    /// </summary>
    public bool SiteOrGlobal
    {
        get;
        set;
    }


    /// <summary>
    /// This value contains result of settings checking
    /// </summary>
    public bool SettingsEnabled
    {
        get;
        protected set;
    }


    /// <summary>
    /// Indicates whether show "site" and "global" buttons
    /// </summary>
    public bool ShowButtons
    {
        get
        {
            return mShowButtons;
        }
        set
        {
            mShowButtons = value;
        }
    }


    /// <summary>
    /// If true, settings are considered as checked if at least one setting is checked. This is used only for displaying general information.
    /// </summary>
    public bool AtLeastOne
    {
        get;
        set;
    }


    /// <summary>
    /// List of keys (delimited by ';') - indicates which settings will be tested (and checked) only for site settings
    /// </summary>
    public String SiteObjects
    {
        get
        {
            return mSiteObjects;
        }
        set
        {
            mSiteObjects = value;
        }
    }


    /// <summary>
    /// List of keys (delimited by ';') - indicates which settings will be tested (and checked) only for global settings
    /// </summary>
    public String GlobalObjects
    {
        get
        {
            return mGlobalObjects;
        }
        set
        {
            mGlobalObjects = value;
        }
    }


    /// <summary>
    /// Indicates if UI should be refreshed by reload after enabling module
    /// </summary>
    public bool ReloadUIWhenModuleEnabled
    {
        get
        {
            return mReloadUIWhenModuleEnabled;
        }
        set
        {
            mReloadUIWhenModuleEnabled = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        Visible = true;

        // Set text for link
        btnGlobal.Text = GetString("module.allowglobal");
        btnSite.Text = GetString("module.allowsite");
    }


    /// <summary>
    /// Generate info text for given setting key
    /// </summary>
    /// <param name="ski">Setting key object</param>
    private String GenerateInfoText(SettingsKeyInfo ski)
    {
        // Get setting's group
        SettingsCategoryInfo sci = SettingsCategoryInfoProvider.GetSettingsCategoryInfo(ski.KeyCategoryID);

        // Get resource name from group
        ResourceInfo ri = ResourceInfoProvider.GetResourceInfo(sci.CategoryResourceID);

        string resourceName = ResHelper.LocalizeString(ri.ResourceDisplayName);
        string path = string.Join(" -> ", GetCategoryPath(sci).Reverse().Select(s => ResHelper.LocalizeString(s.CategoryDisplayName)));

        return String.Format(GetString("ui.moduledisabled.general"), resourceName, path, ResHelper.GetString(ski.KeyDisplayName));
    }


    /// <summary>
    /// Get path to the given settings category.
    /// </summary>
    /// <param name="settingsKeyCategoryInfo">The key that path is generated for</param>
    /// <returns>Path to the given settings category</returns>
    private IEnumerable<SettingsCategoryInfo> GetCategoryPath(SettingsCategoryInfo settingsKeyCategoryInfo)
    {
        var sci = SettingsCategoryInfoProvider.GetSettingsCategoryInfo(settingsKeyCategoryInfo.CategoryParentID);
        while (sci != null)
        {
            yield return sci;
            sci = SettingsCategoryInfoProvider.GetSettingsCategoryInfo(sci.CategoryParentID);
        }
    }


    /// <summary>
    /// Displays info label, if any module is disabled
    /// </summary>
    private bool DisplayErrorText()
    {
        GlobalObjects = ";" + GlobalObjects + ";";
        SiteObjects = ";" + SiteObjects + ";";

        bool keyDisabled = false;
        bool isAnyKeySite = false;
        bool settingChecked = false;
        bool showSite = false;
        bool showGlobal = false;

        // Check config keys - stronger
        if (!String.IsNullOrEmpty(ConfigKeys))
        {
            var keys = ConfigKeys.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string key in keys)
            {
                if (!ValidationHelper.GetBoolean(SettingsHelper.AppSettings[key], false))
                {
                    if (!AtLeastOne)
                    {
                        settingChecked = false;
                        break;
                    }
                }
                else
                {
                    settingChecked = true;
                }
            }
        }

        // Check settings
        if (!settingChecked && !String.IsNullOrEmpty(SettingsKeys))
        {
            int i = 0;

            var keys = SettingsKeys.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string key in keys)
            {
                String objectKey = ";" + key + ";";
                bool globalObject = GlobalObjects.Contains(objectKey);
                bool siteObject = SiteObjects.Contains(objectKey);
                String siteKeyName = SiteName + "." + key;
                String keyName = (SiteOrGlobal || globalObject || KeyScope == DisabledModuleScope.Global) ? key : siteKeyName;

                // If module disabled
                if (!SettingsKeyInfoProvider.GetBoolValue(keyName) || (siteObject && !SettingsKeyInfoProvider.GetBoolValue(siteKeyName)))
                {
                    // For site or global settings check site setting separately
                    if (SiteOrGlobal && SettingsKeyInfoProvider.GetBoolValue(siteKeyName))
                    {
                        settingChecked = true;
                        i++;
                        continue;
                    }

                    // If at least one is checked, info error is set later
                    if (!AtLeastOne)
                    {
                        // If setting is global - hide site button
                        var ski = SettingsKeyInfoProvider.GetSettingsKeyInfo(key);
                        if ((ski != null) && (!ski.KeyIsGlobal))
                        {
                            isAnyKeySite = true;
                        }

                        // Get text (either from collection of text or from single text property)
                        String text = (InfoTexts.Count != 0 && InfoTexts.Count > i) ? InfoTexts[i] : InfoText;

                        if (String.IsNullOrEmpty(text) && (ski != null))
                        {
                            text = GenerateInfoText(ski);
                        }

                        if (lblText.Text != "")
                        {
                            lblText.Text += "<br />";
                        }

                        // Add new text to label
                        lblText.Text += text;

                        // Add this key to collection of disabled keys

                        // Make this info label visible
                        keyDisabled = !String.IsNullOrEmpty(text);

                        if (!siteObject && !globalObject)
                        {
                            showSite = (KeyScope != DisabledModuleScope.Global);
                            showGlobal = (KeyScope != DisabledModuleScope.Site);
                        }
                        else
                        {
                            showSite |= siteObject;
                            showGlobal |= globalObject;
                        }
                    }
                }
                else
                {
                    settingChecked = true;
                }

                i++;
            }
        }

        // If atleastone is set, check if any setting is checked. If no, display warning message
        if (AtLeastOne && !settingChecked)
        {
            keyDisabled = true;
            lblText.Text = InfoText;
        }

        // If parent panel is set, show(hide) it 
        if (ParentPanel != null)
        {
            ParentPanel.Visible = keyDisabled;
        }

        // Show/hide this control if module disabled
        Visible = keyDisabled;

        // Show site button only if any key is site
        btnSite.Visible = isAnyKeySite;

        // Set result to property
        SettingsEnabled = !keyDisabled;

        btnSite.Visible &= showSite;
        btnGlobal.Visible &= showGlobal;

        return !keyDisabled;
    }


    protected void btnGlobal_clicked(object sender, EventArgs ea)
    {
        Save(false);
    }


    protected void btnSiteOnly_clicked(object sender, EventArgs ea)
    {
        Save(true);
    }


    /// <summary>
    /// Check settings and return result.
    /// </summary>    
    public bool Check()
    {
        settingsChecked = true;
        return DisplayErrorText();
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (!settingsChecked)
        {
            // Display text if some setting is disabled
            DisplayErrorText();
        }

        if (!MembershipContext.AuthenticatedUser.IsGlobalAdministrator || !ShowButtons)
        {
            btnGlobal.Visible = false;
            btnSite.Visible = false;
        }

        if (btnGlobal.Visible && btnSite.Visible)
        {
            btnGlobal.CssClass = "DisabledModuleButtons";
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Saves the changed settings 
    /// </summary>
    /// <param name="isSite">Indicates whether changed settings is global or site</param>
    private void Save(bool isSite)
    {
        // This action is permitted only for global administrators
        if (MembershipContext.AuthenticatedUser.IsGlobalAdministrator)
        {
            if (!String.IsNullOrEmpty(SettingsKeys))
            {
                String[] keys = SettingsKeys.Split(';');
                foreach (String key in keys)
                {
                    if (key != String.Empty)
                    {
                        String objectKey = ";" + key + ";";
                        String siteKeyName = SiteName + "." + key;
                        bool globalObject = (GlobalObjects.Contains(objectKey) || KeyScope == DisabledModuleScope.Global);
                        bool siteObject = SiteObjects.Contains(objectKey);

                        // If setting is global or site (or both), set global(site) settings no matter what button (site or global) was clicked
                        if (globalObject || siteObject)
                        {
                            if (globalObject)
                            {
                                if (!SettingsKeyInfoProvider.GetBoolValue(key))
                                {
                                    SettingsKeyInfoProvider.SetValue(key, true);
                                }
                            }

                            if (siteObject)
                            {
                                if (!SettingsKeyInfoProvider.GetBoolValue(siteKeyName))
                                {
                                    SettingsKeyInfoProvider.SetValue(siteKeyName, true);
                                }
                            }

                            continue;
                        }

                        // Test first if settings is disabled
                        if (!SettingsKeyInfoProvider.GetBoolValue(siteKeyName))
                        {
                            String keyName = isSite ? siteKeyName : key;
                            try
                            {
                                SettingsKeyInfoProvider.SetValue(keyName, true);
                            }
                            catch (Exception)
                            {
                                if (isSite)
                                {
                                    // Site settings does not exists. Save as global then
                                    SettingsKeyInfoProvider.SetValue(key, true);
                                }
                            }

                            // If global enabled and site still disabled - enable it also
                            if (!isSite && (KeyScope != DisabledModuleScope.Global))
                            {
                                // If settings not enabled, inherit from global
                                if (!SettingsKeyInfoProvider.GetBoolValue(siteKeyName))
                                {
                                    SettingsKeyInfoProvider.SetValue(siteKeyName, null);
                                }
                            }
                        }
                    }
                }
            }

            // Reload UI if necessary
            if (ReloadUIWhenModuleEnabled)
            {
                URLHelper.Redirect(RequestContext.CurrentURL);
            }
        }
    }

    #endregion
}


#region "DisabledModuleScopeEnum"

/// <summary>
/// Scope of disabled module key check
/// </summary>
public enum DisabledModuleScope : int
{
    /// <summary>
    /// Both site and global settings are checked
    /// </summary>
    [EnumStringRepresentation("Both")]
    Both = 0,

    /// <summary>
    /// Only site keys are checked
    /// </summary>
    [EnumStringRepresentation("Site")]
    Site = 1,

    /// <summary>
    /// Only global keys are checked
    /// </summary>
    [EnumStringRepresentation("Global")]
    Global = 2,
}

#endregion
