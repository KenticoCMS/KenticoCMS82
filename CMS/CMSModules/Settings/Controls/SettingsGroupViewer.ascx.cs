using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS;
using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.EmailEngine;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.HealthMonitoring;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.PortalEngine;
using CMS.Scheduler;
using CMS.SiteProvider;
using CMS.TranslationServices;
using CMS.UIControls;
using CMS.WinServiceEngine;

public partial class CMSModules_Settings_Controls_SettingsGroupViewer : SettingsGroupViewerControl
{
    #region "Private variables"

    // Settings
    private int mCategoryId;
    private string mCategoryName;
    private SettingsCategoryInfo mSettingsCategoryInfo;
    private readonly List<SettingsKeyItem> mKeyItems = new List<SettingsKeyItem>();

    // Site
    private int mSiteId;
    private string mSiteName = string.Empty;
    private SiteInfo mSiteInfo;

    private bool mAllowGlobalInfoMessage = true;

    // Search
    private string mSearchText = "";
    private bool mSearchDescription = true;
    private const int mSearchLimit = 2;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the settings category ID.
    /// </summary>
    public int CategoryID
    {
        get
        {
            if ((mCategoryId == 0) && (SettingsCategoryInfo != null))
            {
                mCategoryId = SettingsCategoryInfo.CategoryID;
            }
            return mCategoryId;
        }
        set
        {
            mCategoryId = value;
            mCategoryName = null;
            mSettingsCategoryInfo = null;
        }
    }


    /// <summary>
    /// Gets or sets the settings category name.
    /// </summary>
    public string CategoryName
    {
        get
        {
            if ((mCategoryName == null) && (SettingsCategoryInfo != null))
            {
                mCategoryName = SettingsCategoryInfo.CategoryName;
            }
            return mCategoryName;
        }
        set
        {
            mCategoryName = value;
            mCategoryId = 0;
            mSettingsCategoryInfo = null;
        }
    }


    /// <summary>
    /// Gets the SettingsCategoryInfo object for the specified CategoryID or CategoryName respectively.
    /// </summary>
    public SettingsCategoryInfo SettingsCategoryInfo
    {
        get
        {
            if (mSettingsCategoryInfo == null)
            {
                if (mCategoryId > 0)
                {
                    mSettingsCategoryInfo = SettingsCategoryInfoProvider.GetSettingsCategoryInfo(mCategoryId);
                }
                else
                {
                    if (mCategoryName != null)
                    {
                        mSettingsCategoryInfo = SettingsCategoryInfoProvider.GetSettingsCategoryInfoByName(mCategoryName);
                    }
                }
            }
            return mSettingsCategoryInfo;
        }
    }


    /// <summary>
    /// Gets the settings keys list for the current category.
    /// </summary>
    public List<SettingsKeyItem> KeyItems
    {
        get
        {
            return mKeyItems;
        }
    }


    /// <summary>
    /// ID of the site.
    /// </summary>
    public int SiteID
    {
        get
        {
            if ((mSiteId == 0) && SiteInfo != null)
            {
                mSiteId = SiteInfo.SiteID;
            }
            return mSiteId;
        }
        set
        {
            mSiteId = value;
            mSiteName = string.Empty;
            mSiteInfo = null;
        }
    }


    /// <summary>
    /// Code name of the site.
    /// </summary>
    public string SiteName
    {
        get
        {
            if (string.IsNullOrEmpty(mSiteName) && SiteInfo != null)
            {
                mSiteName = SiteInfo.SiteName;
            }
            return mSiteName;
        }
        set
        {
            mSiteName = value;
            mSiteId = 0;
            mSiteInfo = null;
        }
    }


    /// <summary>
    /// Gets the site info object for the configured site.
    /// </summary>
    public SiteInfo SiteInfo
    {
        get
        {
            if (mSiteInfo == null)
            {
                if (mSiteId != 0)
                {
                    mSiteInfo = SiteInfoProvider.GetSiteInfo(mSiteId);
                }
                else
                {
                    if (!string.IsNullOrEmpty(mSiteName))
                    {
                        mSiteInfo = SiteInfoProvider.GetSiteInfo(mSiteName);
                    }
                }
            }

            return mSiteInfo;
        }
    }


    /// <summary>
    /// Gets or sets the where condition used to filter settings groups.
    /// All groups will be selected if not set.
    /// </summary>
    public string Where
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value that indicates if "these settings are global ..." message can shown.
    /// Is true by default.
    /// </summary>
    public bool AllowGlobalInfoMessage
    {
        get
        {
            return mAllowGlobalInfoMessage;
        }
        set
        {
            mAllowGlobalInfoMessage = value;
        }
    }


    /// <summary>
    /// Gets a value that indicates if a valid search text is specified.
    /// </summary>
    public bool IsSearchTextValid
    {
        get
        {
            return !string.IsNullOrEmpty(mSearchText) && (mSearchText.Length >= mSearchLimit);
        }
    }

    #endregion


    #region "Lifecycle"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Get search parameters
        mSearchText = QueryHelper.GetString("search", "").Trim();
        mSearchDescription = QueryHelper.GetBoolean("description", false);
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (SettingsCategoryInfo == null)
        {
            plcContent.Append(GetString("settings.keys.nocategoryselected"));
            StopProcessing = true;
            return;
        }

        ScriptHelper.RegisterTooltip(Page);
        ScriptHelper.RegisterBootstrapTooltip(Page, ".info-icon > i");

        // Loop through all the groups in the category
        int groupCount = 0;
        bool hasOnlyGlobalKeys = true;
        var groups = GetGroups(SettingsCategoryInfo);

        foreach (var group in groups)
        {
            // Get keys
            var keys = GetKeys(group.CategoryID).ToArray();

            // Skip empty group
            if (!keys.Any())
            {
                continue;
            }

            groupCount++;

            // Add category panel for the group
            var pnlGroup = GetCategoryPanel(group, groupCount);
            plcContent.Append(pnlGroup);

            // Loop through all the keys in the group
            int keyCount = 0;
            foreach (var keyInfo in keys)
            {
                // Increase key number for unique control identification
                keyCount++;

                // Update flag when non-global-only key exists
                if (!keyInfo.KeyIsGlobal)
                {
                    hasOnlyGlobalKeys = false;
                }

                // Create key item
                var keyItem = new SettingsKeyItem
                {
                    ParentCategoryPanel = pnlGroup,
                    KeyName = keyInfo.KeyName,
                    KeyType = keyInfo.KeyType,
                    ValidationRegexPattern = keyInfo.KeyValidation,
                    CategoryName = group.CategoryName,
                    ExplanationText = ResHelper.LocalizeString(keyInfo.KeyExplanationText)
                };


                Panel pnlRow = new Panel
                {
                    CssClass = "form-group"
                };
                pnlGroup.Controls.Add(pnlRow);

                // Add label cell to the beginning of the row
                var pnlLabelCell = new Panel
                {
                    CssClass = "editing-form-label-cell"
                };
                pnlRow.Controls.AddAt(0, pnlLabelCell);

                // Continue with the value cell
                pnlRow.Controls.Add(new LiteralControl(@"<div class=""editing-form-value-cell"">"));

                // Create placeholder for the editing control that may end up in an update panel
                var pnlValueCell = new Panel
                {
                    CssClass = "settings-group-inline keep-white-space-fixed"
                };
                var pnlValue = new Panel
                {
                    CssClass = "editing-form-control-nested-control keep-white-space-fixed"
                };
                pnlValueCell.Controls.Add(pnlValue);
                var pnlIcons = new Panel
                {
                    CssClass = "settings-info-group keep-white-space-fixed"
                };
                pnlValueCell.Controls.Add(pnlIcons);

                Label helpIcon = GetIcon("icon-question-circle", MacroResolver.Resolve(ResHelper.LocalizeString(keyInfo.KeyDescription)));
                pnlIcons.Controls.Add(helpIcon);

                CMSCheckBox chkInherit = null;
                if (mSiteId > 0)
                {
                    // Wrap in update panel for inherit checkbox postback
                    var pnlValueUpdate = new UpdatePanel
                    {
                        ID = string.Format("pnlValueUpdate{0}{1}", groupCount, keyCount),
                        UpdateMode = UpdatePanelUpdateMode.Conditional,
                    };
                    pnlRow.Controls.Add(pnlValueUpdate);

                    // Add inherit checkbox
                    chkInherit = GetInheritCheckBox(groupCount, keyCount);
                    keyItem.InheritCheckBox = chkInherit;

                    pnlValueUpdate.ContentTemplateContainer.Controls.Add(chkInherit);

                    pnlValueUpdate.ContentTemplateContainer.Controls.Add(pnlValueCell);
                }
                else
                {
                    pnlRow.Controls.Add(pnlValueCell);

                    // Add "current site does not inherit the global value" warning for global settings
                    if (SiteContext.CurrentSite != null)
                    {
                        var isCurrentSiteValueInherited = SettingsKeyInfoProvider.IsValueInherited(keyInfo.KeyName, SiteContext.CurrentSiteID);
                        if (!isCurrentSiteValueInherited)
                        {
                            string inheritWarningText = String.Format(GetString("settings.currentsitedoesnotinherit"), ResHelper.LocalizeString(SiteContext.CurrentSite.DisplayName));
                            Label inheritWarningImage = GetIcon("icon-exclamation-triangle warning-icon", HTMLHelper.HTMLEncode(inheritWarningText));

                            pnlIcons.Controls.Add(inheritWarningImage);
                        }
                    }
                }

                // Add explanation text
                if (!String.IsNullOrWhiteSpace(keyItem.ExplanationText))
                {
                    Panel pnlExplanationText = new Panel
                    {
                        CssClass = "explanation-text"
                    };
                    LocalizedLiteral explanationText = new LocalizedLiteral
                    {
                        Text = keyItem.ExplanationText
                    };
                    pnlExplanationText.Controls.Add(explanationText);
                    pnlRow.Controls.Add(pnlExplanationText);
                }

                pnlRow.Controls.Add(new LiteralControl(@"</div>"));

                // Get current values
                keyItem.KeyIsInherited = SettingsKeyInfoProvider.IsValueInherited(keyInfo.KeyName, SiteName);
                keyItem.KeyValue = SettingsKeyInfoProvider.GetValue(keyInfo.KeyName, SiteName);

                // Get value
                string keyValue;
                bool isInherited;
                if (RequestHelper.IsPostBack() && (chkInherit != null))
                {
                    isInherited = Request.Form[chkInherit.UniqueID] != null;
                    keyValue = isInherited ? SettingsKeyInfoProvider.GetValue(keyInfo.KeyName) : SettingsKeyInfoProvider.GetValue(keyInfo.KeyName, SiteName);
                }
                else
                {
                    isInherited = keyItem.KeyIsInherited;
                    keyValue = keyItem.KeyValue;

                    // Set the inherit checkbox state
                    if (!RequestHelper.IsPostBack() && chkInherit != null)
                    {
                        chkInherit.Checked = isInherited;
                    }
                }

                // Add value editing control
                var enabled = !isInherited;
                FormEngineUserControl control = GetFormEngineUserControl(keyInfo, groupCount, keyCount);
                if (control != null)
                {
                    // Add form engine value editing control
                    control.Value = keyValue;
                    pnlValue.Controls.Add(control);

                    // Set form control enabled value, does not work when moved before plcControl.Controls.Add(control)
                    control.Enabled = enabled;

                    keyItem.ValueControl = control;

                    if (chkInherit != null)
                    {
                        chkInherit.CheckedChanged += (sender, args) =>
                        {
                            control.Value = keyValue;
                        };
                    }
                }
                else
                {
                    // Add simple value editing control
                    switch (keyInfo.KeyType.ToLowerCSafe())
                    {
                        case "boolean":
                            // Add checkbox value editing control
                            var @checked = ValidationHelper.GetBoolean(keyValue, false);
                            CMSCheckBox chkValue = GetValueCheckBox(groupCount, keyCount, @checked, enabled);
                            pnlValue.Controls.Add(chkValue);

                            keyItem.ValueControl = chkValue;

                            if (chkInherit != null)
                            {
                                chkInherit.CheckedChanged += (sender, args) =>
                                {
                                    chkValue.Checked = @checked;
                                };
                            }
                            break;

                        case "longtext":
                            // Add text area value editing control
                            var longText = keyValue;
                            var txtValueTextArea = GetValueTextArea(groupCount, keyCount, longText, enabled);
                            if (txtValueTextArea != null)
                            {
                                // Text area control was loaded successfully
                                pnlValue.Controls.Add(txtValueTextArea);
                                keyItem.ValueControl = txtValueTextArea;
                                if (chkInherit != null)
                                {
                                    chkInherit.CheckedChanged += (sender, args) =>
                                    {
                                        txtValueTextArea.Text = longText;
                                    };
                                }
                            }
                            else
                            {
                                // Text area control was not loaded successfully
                                var errorLabel = new FormControlError
                                {
                                    ErrorTitle = "[Error loading the editing control, check the event log for more details]",
                                };
                                pnlValue.Controls.Add(errorLabel);
                            }
                            break;

                        default:
                            // Add textbox value editing control
                            var text = keyValue;
                            TextBox txtValue = GetValueTextBox(groupCount, keyCount, text, enabled);
                            pnlValue.Controls.Add(txtValue);

                            keyItem.ValueControl = txtValue;

                            if (chkInherit != null)
                            {
                                chkInherit.CheckedChanged += (sender, args) =>
                                {
                                    txtValue.Text = text;
                                };
                            }
                            break;
                    }
                }

                // Add label to the label cell when associated control has been resolved
                pnlLabelCell.Controls.Add(GetLabel(keyInfo, keyItem.ValueControl, groupCount, keyCount));

                // Add error label if KeyType is integer or validation expression defined or FormControl is used
                if ((keyInfo.KeyType == "int") || (keyInfo.KeyType == "double") || (keyItem.ValidationRegexPattern != null) || (control != null))
                {
                    Label lblError = GetLabelError(groupCount, keyCount);
                    pnlIcons.Controls.Add(lblError);
                    keyItem.ErrorLabel = lblError;
                }

                mKeyItems.Add(keyItem);
            }
        }

        // Show info message when other than global-only global keys are displayed
        if ((mSiteId <= 0) && (CategoryID > 0) && !hasOnlyGlobalKeys && AllowGlobalInfoMessage)
        {
            ShowInformation(GetString("settings.keys.globalsettingsnote"));
        }

        // Display export and reset links only if some groups were found.
        if (groupCount > 0)
        {
            // Add reset link if required
            if (!RequestHelper.IsPostBack() && QueryHelper.GetInteger("resettodefault", 0) == 1)
            {
                ShowInformation(GetString("Settings-Keys.ValuesWereResetToDefault"));
            }
        }
        else
        {
            // Hide "These settings are global ..." message if no setting found in this group
            if (!string.IsNullOrEmpty(mSearchText))
            {
                var ltrScript = new Literal
                {
                    Text = ScriptHelper.GetScript("DisableHeaderActions();")
                };
                plcContent.Append(ltrScript);
                lblNoData.Visible = true;
            }
        }
    }

    #endregion


    #region "Save methods"

    /// <summary>
    /// Validates the settings values and returns true if all are valid.
    /// </summary>
    private bool IsValid()
    {
        // Loop through all settings items
        for (int i = 0; i < mKeyItems.Count; i++)
        {
            SettingsKeyItem item = mKeyItems[i];

            var keyChanged = false;

            if (item.ValueControl is TextBox)
            {
                var tb = (TextBox)item.ValueControl;
                tb.Text = tb.Text.Trim();
                keyChanged = (tb.Text != item.KeyValue);
                item.KeyValue = tb.Text;
            }
            else if (item.ValueControl is CMSCheckBox)
            {
                var cb = (CMSCheckBox)item.ValueControl;
                keyChanged = (cb.Checked.ToString() != item.KeyValue);
                item.KeyValue = cb.Checked.ToString();
            }
            else if (item.ValueControl is FormEngineUserControl)
            {
                var control = (FormEngineUserControl)item.ValueControl;
                if (control.IsValid())
                {
                    keyChanged = Convert.ToString(control.Value) != item.KeyValue;
                    item.KeyValue = Convert.ToString(control.Value);
                }
                else
                {
                    item.ErrorLabel.Text = String.IsNullOrEmpty(control.ErrorMessage) ? GetString("Settings.ValidationError") : control.ErrorMessage;
                    item.ErrorLabel.Visible = !String.IsNullOrEmpty(item.ErrorLabel.Text);
                    ShowError(GetString("general.saveerror"));
                    return false;
                }
            }

            if (item.InheritCheckBox != null)
            {
                var inheritanceChanged = item.InheritCheckBox.Checked != item.KeyIsInherited;
                keyChanged = inheritanceChanged || !item.KeyIsInherited && keyChanged;
                item.KeyIsInherited = item.InheritCheckBox.Checked;
            }

            item.KeyChanged = keyChanged;
            if (!keyChanged)
            {
                continue;
            }

            // Validation result
            string result = string.Empty;

            // Validation using regular expression if there is any
            if (!string.IsNullOrEmpty(item.ValidationRegexPattern) && (item.ValidationRegexPattern.Trim() != string.Empty))
            {
                result = new Validator().IsRegularExp(item.KeyValue, item.ValidationRegexPattern, GetString("Settings.ValidationRegExError")).Result;
            }

            // Validation according to the value type (validate only nonempty values)
            if (string.IsNullOrEmpty(result) && !string.IsNullOrEmpty(item.KeyValue))
            {
                switch (item.KeyType.ToLowerCSafe())
                {
                    case "int":
                        result = new Validator().IsInteger(item.KeyValue, GetString("Settings.ValidationIntError")).Result;
                        break;

                    case "double":
                        result = new Validator().IsDouble(item.KeyValue, GetString("Settings.ValidationDoubleError")).Result;
                        break;
                }
            }

            if (!string.IsNullOrEmpty(result))
            {
                item.ErrorLabel.Text = result;
                item.ErrorLabel.Visible = !String.IsNullOrEmpty(result);
                return false;
            }
            else
            {
                // Update changes
                mKeyItems[i] = item;
            }
        }

        return true;
    }


    /// <summary>
    /// Clears the cache to apply the settings to the web site.
    /// </summary>
    private void ClearCache(string keyName, object keyValue)
    {
        var clearCache = false;
        var clearOutputCache = false;
        var clearCSSCache = false;
        var clearPartialCache = false;

        string serviceBaseName = null;
        var serviceEnabled = false;

        // Clear the cached items
        switch (keyName.ToLowerCSafe())
        {
            case "cmsemailsenabled":
                if ((mSiteId <= 0) && (keyValue.ToString().EqualsCSafe("false", true)))
                {
                    // Stop current sending of e-mails and newsletters if e-mails are disabled in global settings
                    EmailHelper.Queue.CancelSending();
                    ModuleCommands.CancelNewsletterSending();
                }
                break;

            case "cmslogsize":
                // Log size changed
                EventLogProvider.Clear();
                break;

            case "cmscacheminutes":
            case "cmscachepageinfo":
            case "cmscacheimages":
            case "cmsmaxcachefilesize":
            case "cmsdefaultaliaspath":
            case "cmsdefaultculturecode":
            case "cmscombinewithdefaultculture":
                // Clear cache upon change
                clearCache = true;
                break;

            case "cmspagekeywordsprefix":
            case "cmspagedescriptionprefix":
            case "cmspagetitleformat":
            case "cmspagetitleprefix":
            case "cmscontrolelement":
            case "cmsenableoutputcache":
            case "cmsfilesystemoutputcacheminutes":
                // Clear output cache upon change
                clearOutputCache = true;
                break;

            case "cmsenablepartialcache":
                // Clear output cache upon change
                clearPartialCache = true;
                break;

            case "cmsresourcecompressionenabled":
            case "cmsstylesheetminificationenabled":
            case "cmsresolvemacrosincss":
                // Clear the CSS styles
                clearCSSCache = true;
                break;

            case "cmsuseexternalservice":
            case "cmsservicehealthmonitoringinterval":
            case "cmsenablehealthmonitoring":
                // Restart Health Monitoring service
                {
                    serviceBaseName = WinServiceHelper.HM_SERVICE_BASENAME;
                    serviceEnabled = HealthMonitoringHelper.UseExternalService;

                    // Clear status of health monitoring
                    HealthMonitoringHelper.Clear();
                }
                break;

            case "cmsprogressivecaching":
                CacheHelper.ProgressiveCaching = ValidationHelper.GetBoolean(keyValue, false);
                break;

            case "cmsscheduleruseexternalservice":
            case "cmsschedulerserviceinterval":
            case "cmsschedulertasksenabled":
                // Restart Scheduler service
                serviceBaseName = WinServiceHelper.SCHEDULER_SERVICE_BASENAME;
                serviceEnabled = SchedulingHelper.UseExternalService;
                break;

            case "cmsresizeimagestodevice":
                CacheHelper.TouchKey(DeviceProfileInfoProvider.DEVICE_IMAGE_CACHE_KEY);
                break;

            case "cmstranslationscomurl":
            case "cmstranslationscomusername":
            case "cmstranslationscompassword":
            case "cmstranslationscomprojectcode":
                AbstractHumanTranslationService.ClearHashtables();
                break;

            case "cmsuseeventloglistener":
                if (ValidationHelper.GetBoolean(keyValue, false))
                {
                    EventLogSourceHelper.RegisterDefaultEventLogListener();
                }
                else
                {
                    EventLogSourceHelper.UnregisterDefaultEventLogListener();
                }

                break;
        }

        // Clear the cache to apply the settings to the web site
        if (clearCache)
        {
            CacheHelper.ClearCache(null);
        }
        // Restart windows service
        else if (serviceBaseName != null)
        {
            try
            {
                WinServiceItem def = WinServiceHelper.GetServiceDefinition(serviceBaseName);
                if (def != null)
                {
                    if (serviceEnabled)
                    {
                        WinServiceHelper.RestartService(def.GetServiceName());
                    }
                    else
                    {
                        WinServiceHelper.DeleteServiceFile(def.GetServiceName());
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("Settings", "RestartService", ex);
            }
        }
        else
        {
            // Clear only cache portions
            if (clearOutputCache)
            {
                CacheHelper.ClearFullPageCache();
            }
            if (clearCSSCache)
            {
                CacheHelper.ClearCSSCache();
            }
            if (clearPartialCache)
            {
                CacheHelper.ClearPartialCache();
            }
        }
    }


    /// <summary>
    /// Saves changes made to settings keys into the database.
    /// </summary>
    public void SaveChanges()
    {
        // Validate values
        var isValid = IsValid();
        if (!isValid)
        {
            ShowError(GetString("general.saveerror"));
            return;
        }

        bool logSynchronization = (mSettingsCategoryInfo.CategoryName.ToLowerCSafe() != "cms.staging");

        using (var h = SettingsSave.StartEvent())
        {
            if (h.CanContinue())
            {
                // Update changes in database and hashtables
                foreach (SettingsKeyItem tmpItem in mKeyItems)
                {
                    // Save only changed settings
                    if (!tmpItem.KeyChanged)
                    {
                        continue;
                    }

                    string keyName = tmpItem.KeyName;

                    object keyValue = tmpItem.KeyValue;
                    if (tmpItem.KeyIsInherited)
                    {
                        keyValue = DBNull.Value;
                    }

                    if (keyName.EqualsCSafe("CMSDBObjectOwner", true))
                    {
                        logSynchronization = false;
                    }

                    SettingsKeyInfoProvider.SetValue(keyName, SiteName, keyValue, logSynchronization);

                    ClearCache(keyName, keyValue);
                }

                // Show message
                ShowChangesSaved();
            }

            h.FinishEvent();
        }
    }


    /// <summary>
    /// Resets all keys in the current category to the default value.
    /// </summary>
    public void ResetToDefault()
    {
        if (SettingsCategoryInfo == null)
        {
            return;
        }

        // Get keys
        IEnumerable<SettingsKeyInfo> keys = GetGroups(SettingsCategoryInfo).SelectMany(g => GetKeys(g.CategoryID));

        // Set default values
        foreach (var key in keys)
        {
            SettingsKeyInfoProvider.SetValue(key.KeyName, SiteName, key.KeyDefaultValue);
        }
    }

    #endregion


    #region "Controls methods"

    /// <summary>
    /// Gets FormEngineUserControl instance for the input SettingsKeyInfo object.
    /// </summary>
    /// <param name="key">SettingsKeyInfo</param>
    /// <param name="groupNo">Number representing index of the processing settings group</param>
    /// <param name="keyNo">Number representing index of the processing SettingsKeyInfo</param>
    private FormEngineUserControl GetFormEngineUserControl(SettingsKeyInfo key, int groupNo, int keyNo)
    {
        string controlNameOrPath = key.KeyEditingControlPath;
        if (string.IsNullOrEmpty(controlNameOrPath))
        {
            return null;
        }

        // Try to get form control by its name
        FormEngineUserControl control = null;
        var formUserControl = FormUserControlInfoProvider.GetFormUserControlInfo(controlNameOrPath);
        if (formUserControl != null)
        {
            var formProperties = formUserControl.UserControlMergedParameters;

            if (formUserControl.UserControlParentID > 0)
            {
                // Get parent user control
                var parentFormUserControl = FormUserControlInfoProvider.GetFormUserControlInfo(formUserControl.UserControlParentID);
                if (parentFormUserControl != null)
                {
                    formUserControl = parentFormUserControl;
                }
            }

            // Create FormInfo and load control
            control = Page.LoadUserControl(FormUserControlInfoProvider.GetFormUserControlUrl(formUserControl)) as FormEngineUserControl;
            if (control != null)
            {
                FormInfo fi = FormHelper.GetFormControlParameters(controlNameOrPath, formProperties, false);
                control.LoadDefaultProperties(fi);

                if (!string.IsNullOrEmpty(key.KeyFormControlSettings))
                {
                    control.FieldInfo = FormHelper.GetFormControlSettingsFromXML(key.KeyFormControlSettings);
                    control.LoadControlFromFFI();
                }
            }
        }
        else
        {
            // Try to load the control
            try
            {
                control = Page.LoadUserControl(controlNameOrPath) as FormEngineUserControl;
            }
            catch
            {
            }
        }

        if (control == null)
        {
            return null;
        }

        control.ID = string.Format(@"key{0}{1}", groupNo, keyNo);
        control.IsLiveSite = false;

        return control;
    }


    /// <summary>
    /// Gets <c>CategoryPanel</c> instance for the input settings group.
    /// </summary>
    /// <param name="group"><c>SettingsCategoryInfo</c> instance representing settings group</param>
    /// <param name="groupNo">Number representing index of the processing settings group</param>
    private CategoryPanel GetCategoryPanel(SettingsCategoryInfo group, int groupNo)
    {
        string title = null;
        if (IsSearchTextValid)
        {
            var categories = SettingsCategoryInfoProvider.GetCategoriesOnPath(group.CategoryIDPath);
            var categoryNames = categories.Cast<SettingsCategoryInfo>().Select(c =>
            {
                var displayName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(c.CategoryDisplayName));
                if (c.CategoryIsGroup)
                {
                    return displayName;
                }

                var url = string.Format("~/CMSModules/Settings/Pages/Categories.aspx?selectedCategoryId={0}&selectedSiteId={1}", c.CategoryID, SiteID);
                url = ResolveUrl(url);

                var name = string.Format("<a href=\"\" onclick=\"selectCategory('{0}');\">{1}</a>", url, displayName);
                return name;
            });
            title = categoryNames.Join(" > ");
        }
        else
        {
            title = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(group.CategoryDisplayName));
        }

        var panel = new CategoryPanel
        {
            ID = string.Format(@"CategoryPanel{0}", groupNo),
            DisplayRightPanel = false,
            AllowCollapsing = false,
            Text = title,
            RenderAs = HtmlTextWriterTag.Div
        };

        return panel;
    }


    /// <summary>
    /// Gets <c>CheckBox</c> control used for key value editing.
    /// </summary>
    /// <param name="groupNo">Number representing index of the processing settings group</param>
    /// <param name="keyNo">Number representing index of the processing SettingsKeyInfo</param>
    /// <param name="checked">Checked</param>
    /// <param name="enabled">Enabled</param>
    private CMSCheckBox GetValueCheckBox(int groupNo, int keyNo, bool @checked, bool enabled)
    {
        var chkValue = new CMSCheckBox
        {
            ID = string.Format("chkKey{0}{1}", groupNo, keyNo),
            EnableViewState = false,
            Checked = @checked,
            Enabled = enabled,
            CssClass = "checkbox-no-label"
        };

        return chkValue;
    }


    /// <summary>
    /// Gets <c>TextBox</c> control used for key value editing.
    /// </summary>
    /// <param name="groupNo">Number representing index of the processing settings group</param>
    /// <param name="keyNo">Number representing index of the processing SettingsKeyInfo</param>
    /// <param name="text">Text</param>
    /// <param name="enabled">Enabled</param>
    private TextBox GetValueTextBox(int groupNo, int keyNo, string text, bool enabled)
    {
        var txtValue = new CMSTextBox
        {
            ID = string.Format("txtKey{0}{1}", groupNo, keyNo),
            EnableViewState = false,
            Text = text,
            Enabled = enabled,
        };

        return txtValue;
    }


    /// <summary>
    /// Gets the text area form engine user control used for key value editing.
    /// </summary>
    /// <param name="groupNo">Number representing index of the processing settings group</param>
    /// <param name="keyNo">Number representing index of the processing SettingsKeyInfo</param>
    /// <param name="text">Text</param>
    /// <param name="enabled">Enabled</param>
    private FormEngineUserControl GetValueTextArea(int groupNo, int keyNo, string text, bool enabled)
    {
        try
        {
            var txtValue = (FormEngineUserControl)LoadControl("~/CMSFormControls/Inputs/LargeTextArea.ascx");
            txtValue.ID = string.Format("txtKey{0}{1}", groupNo, keyNo);
            txtValue.EnableViewState = false;
            txtValue.Enabled = enabled;
            txtValue.Value = text;
            return txtValue;
        }
        catch (Exception ex)
        {
            CoreServices.EventLog.LogException("Settings", "LOADCONTROL", ex);
            return null;
        }
    }


    /// <summary>
    /// Gets inherit <c>CheckBox</c> instance for the input <c>SettingsKeyInfo</c> object.
    /// </summary>
    /// <param name="groupNo">Number representing index of the processing settings group</param>
    /// <param name="keyNo">Number representing index of the processing SettingsKeyInfo</param>
    private CMSCheckBox GetInheritCheckBox(int groupNo, int keyNo)
    {
        var chkInherit = new CMSCheckBox
        {
            ID = string.Format(@"chkInherit{0}{1}", groupNo, keyNo),
            Text = GetString("settings.keys.checkboxinheritglobal"),
            EnableViewState = true,
            AutoPostBack = true,
            CssClass = "field-value-override-checkbox"
        };

        return chkInherit;
    }


    /// <summary>
    /// Gets <c>Label</c> instance for the input <c>SettingsKeyInfo</c> object.
    /// </summary>
    /// <param name="groupNo">Number representing index of the processing settings group</param>
    /// <param name="keyNo">Number representing index of the processing SettingsKeyInfo</param>
    private Label GetLabelError(int groupNo, int keyNo)
    {
        var label = new Label
        {
            ID = string.Format(@"lblError{0}{1}", groupNo, keyNo),
            EnableViewState = false,
            CssClass = "form-control-error",
            Visible = false
        };

        return label;
    }


    /// <summary>
    /// Gets <c>Label</c> instance for the input <c>SettingsKeyInfo</c> object.
    /// </summary>
    /// <param name="settingsKey"><c>SettingsKeyInfo</c> instance</param>
    /// <param name="inputControl">Input control associated to the label</param>
    /// <param name="groupNo">Number representing index of the processing settings group</param>
    /// <param name="keyNo">Number representing index of the processing SettingsKeyInfo</param>
    private Label GetLabel(SettingsKeyInfo settingsKey, Control inputControl, int groupNo, int keyNo)
    {
        LocalizedLabel label = new LocalizedLabel
        {
            EnableViewState = false,
            ID = string.Format(@"lblDispName{0}{1}", groupNo, keyNo),
            CssClass = "control-label editing-form-label",
            Text = settingsKey.KeyDisplayName,
            DisplayColon = true
        };
        if (inputControl != null)
        {
            label.AssociatedControlID = inputControl.ID;
        }

        ScriptHelper.AppendTooltip(label, MacroResolver.Resolve(ResHelper.LocalizeString(settingsKey.KeyDescription)), null);

        return label;
    }


    /// <summary>
    /// Returns Label control that displays font icon specified by ccs class
    /// </summary>
    /// <param name="cssClass">Font icon css class</param>
    /// <param name="toolTip">Icon tooltip</param>
    private Label GetIcon(string cssClass, string toolTip)
    {
        Label iconWrapper = new Label
        {
            CssClass = "info-icon"
        };

	    toolTip = ScriptHelper.FormatTooltipString(toolTip, false, false);

        CMSIcon helpIcon = new CMSIcon
        {
            CssClass = cssClass,
            ToolTip = toolTip
        };

        // Enable HTML formating in tooltip
        helpIcon.Attributes.Add("data-html", "true");

        iconWrapper.Controls.Add(helpIcon);

        return iconWrapper;
    }

    #endregion


    #region "Settings methods"

    private IEnumerable<SettingsCategoryInfo> GetGroups(SettingsCategoryInfo category)
    {
        if (IsSearchTextValid)
        {
            var groups = SettingsCategoryInfoProvider.GetSettingsCategories("CategoryIsGroup = 1", "CategoryName");
            return groups;
        }
        else
        {
            var groups = SettingsCategoryInfoProvider.GetChildSettingsCategories(category.CategoryName, Where);
            return groups.Cast<SettingsCategoryInfo>().Where(c => c.CategoryIsGroup);
        }
    }


    private IEnumerable<SettingsKeyInfo> GetKeys(int groupId)
    {
        IEnumerable<SettingsKeyInfo> keys = SettingsKeyInfoProvider.GetSettingsKeys(groupId)
            .Where(new WhereCondition().WhereFalse("KeyIsHidden").Or().WhereNull("KeyIsHidden"))
            .OrderBy("KeyOrder", "KeyDisplayName");

        if (IsSearchTextValid)
        {
            keys = keys.Where(k => SettingsKeyInfoProvider.SearchSettingsKey(k, mSearchText, mSearchDescription));
        }

        if (SiteID > 0)
        {
            return keys.Where(k => !k.KeyIsGlobal);
        }

        return keys;
    }

    #endregion


    #region "Types"

    public struct SettingsKeyItem
    {
        // Settings key
        public string KeyName;
        public string KeyType;
        public string KeyValue;
        public bool KeyIsInherited;
        public bool KeyChanged;
        public string CategoryName;
        public string ExplanationText;

        public string ValidationRegexPattern;

        // Related controls
        public Control ValueControl;
        public CMSCheckBox InheritCheckBox;
        public Label ErrorLabel;
        public CategoryPanel ParentCategoryPanel;
    }

    #endregion
}
