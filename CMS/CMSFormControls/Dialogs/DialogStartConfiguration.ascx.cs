using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.UIControls;

/// <summary>
/// This form control needs other blank fields with following names to work properly:
/// dialogs_content_hide
/// dialogs_content_path
/// dialogs_content_site
/// dialogs_libraries_hide
/// dialogs_libraries_site
/// dialogs_libraries_global
/// dialogs_libraries_global_libname
/// dialogs_groups
/// dialogs_groups_name
/// dialogs_libraries_group
/// dialogs_libraries_group_libname
/// dialogs_libraries_path
/// dialogs_attachments_hide
/// dialogs_anchor_hide
/// dialogs_email_hide
/// dialogs_web_hide
/// autoresize
/// autoresize_width
/// autoresize_height
/// autoresize_maxsidesize
/// </summary>
public partial class CMSFormControls_Dialogs_DialogStartConfiguration : FormEngineUserControl
{
    #region "Variables"

    protected bool communityLoaded = false;
    protected bool mediaLoaded = false;

    #endregion


    #region "Properties"

    public override object Value
    {
        get
        {
            return true;
        }
        set
        {
            // Do nothing
        }
    }


    /// <summary>
    /// Indicates if the Autoresize settings should be available.
    /// </summary>
    public bool DisplayAutoresize
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayAutoresize"), true);
        }
        set
        {
            SetValue("DisplayAutoresize", value);
        }
    }


    /// <summary>
    /// Indicates if the E-mal tab settings should be available.
    /// </summary>
    public bool DisplayEmailTabSettings
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayEmailTabSettings"), true);
        }
        set
        {
            SetValue("DisplayEmailTabSettings", value);
        }
    }


    /// <summary>
    /// Indicates if the Anchor tab settings should be available.
    /// </summary>
    public bool DisplayAnchorTabSettings
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayAnchorTabSettings"), true);
        }
        set
        {
            SetValue("DisplayAnchorTabSettings", value);
        }
    }


    /// <summary>
    /// Indicates if the Web tab settings should be available.
    /// </summary>
    public bool DisplayWebTabSettings
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayWebTabSettings"), true);
        }
        set
        {
            SetValue("DisplayWebTabSettings", value);
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        lnkAdvacedFieldSettings.Click += lnkAdvacedFieldSettings_Click;

        communityLoaded = ModuleEntryManager.IsModuleLoaded(ModuleName.COMMUNITY);
        mediaLoaded = ModuleEntryManager.IsModuleLoaded(ModuleName.MEDIALIBRARY);

        plcMedia.Visible = mediaLoaded;
        plcGroups.Visible = communityLoaded;

        if (communityLoaded)
        {
            drpGroups.OnSelectionChanged += drpGroups_SelectedIndexChanged;
        }

        LoadSites();
        LoadSiteLibraries(null);
        LoadSiteGroups(null);
        LoadGroupLibraries(null, null);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if ((Form != null) && (Form.Data != null))
        {
            // Set Auto resize control properties
            elemAutoResize.Form = Form;
            if (ContainsColumn("autoresize"))
            {
                elemAutoResize.Value = ValidationHelper.GetString(Form.Data.GetValue("autoresize"), null);
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Display configuration settings in read-only mode
        if (!lnkAdvacedFieldSettings.IsEnabled)
        {
            plcAdvancedFieldSettings.Visible = true;
            lnkAdvacedFieldSettings.Visible = false;
            elemAutoResize.Enabled = false;
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Advanced dialog link event handler.
    /// </summary>
    protected void lnkAdvacedFieldSettings_Click(object sender, EventArgs e)
    {
        plcAdvancedFieldSettings.Visible = !plcAdvancedFieldSettings.Visible;
        if (plcAdvancedFieldSettings.Visible)
        {
            LoadValues();
        }
    }


    /// <summary>
    /// Group drop-down list event handler.
    /// </summary>
    protected void drpGroups_SelectedIndexChanged(object sender, EventArgs e)
    {
        SelectGroup();
    }


    /// <summary>
    /// Handles site selection change event.
    /// </summary>
    protected void UniSelectorMediaSites_OnSelectionChanged(object sender, EventArgs e)
    {
        string selectedSite = ValidationHelper.GetString(siteSelectorMedia.Value, String.Empty);
        SelectSite(selectedSite);
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Processes the data loading after the site is selected.
    /// </summary>
    private void SelectSite(string selectedSite)
    {
        if (communityLoaded)
        {
            LoadSiteGroups(selectedSite);
        }
        LoadSiteLibraries(selectedSite);
        SelectGroup();
    }


    /// <summary>
    /// Processes the data loading after the group is selected.
    /// </summary>
    private void SelectGroup()
    {
        bool isNone = drpGroups.DropDownSingleSelect.SelectedValue.EqualsCSafe("#none#", true);
        string selectedMediaSite = ValidationHelper.GetString(siteSelectorMedia.Value, String.Empty);
        
        drpGroupLibraries.Enabled = !isNone;
        LoadGroupLibraries(selectedMediaSite, ValidationHelper.GetString(drpGroups.Value, String.Empty), isNone); 

        if (isNone)
        {
            drpGroupLibraries.DropDownSingleSelect.SelectedIndex = 0;
        }
    }


    /// <summary>
    /// Loads the site DropDownLists.
    /// </summary>
    private void LoadSites()
    {
        // Define special fields
        SpecialFieldsDefinition specialFields = new SpecialFieldsDefinition(null, FieldInfo, ContextResolver);
        specialFields.Add(new SpecialField { Text = GetString("general.selectall"), Value = "##all##" });
        specialFields.Add(new SpecialField { Text = GetString("dialogs.config.currentsite"), Value = "##current##" });

        // Set site selector
        siteSelectorContent.DropDownSingleSelect.AutoPostBack = true;
        siteSelectorContent.AllowAll = false;
        siteSelectorContent.UseCodeNameForSelection = true;
        siteSelectorContent.UniSelector.SpecialFields = specialFields;

        siteSelectorMedia.DropDownSingleSelect.AutoPostBack = true;
        siteSelectorMedia.AllowAll = false;
        siteSelectorMedia.UseCodeNameForSelection = true;
        siteSelectorMedia.UniSelector.SpecialFields = specialFields;

        if (mediaLoaded)
        {
            siteSelectorMedia.UniSelector.OnSelectionChanged += UniSelectorMediaSites_OnSelectionChanged;
        }
    }


    /// <summary>
    /// Reloads the site groups.
    /// </summary>
    /// <param name="siteName">Name of the site</param>
    private void LoadSiteGroups(string siteName)
    {
        if (communityLoaded && mediaLoaded)
        {
            drpGroups.DropDownItems.Clear();
            drpGroups.SpecialFields.Add(new SpecialField { Text = GetString("general.selectall"), Value = String.Empty });
            drpGroups.SpecialFields.Add(new SpecialField { Text = GetString("general.selectnone"), Value = "#none#" });
            drpGroups.SpecialFields.Add(new SpecialField { Text = GetString("dialogs.config.currentgroup"), Value = "#current#" });

            if (siteName != null)
            {
                drpGroups.WhereCondition = "GroupSiteID IN (SELECT SiteID FROM CMS_Site WHERE SiteName = '" + SqlHelper.EscapeQuotes(siteName) + "')";
                drpGroups.Reload(true);
            }
        }
    }


    /// <summary>
    /// Reloads the site media libraries.
    /// </summary>
    /// <param name="siteName">Name of the site</param>
    private void LoadSiteLibraries(string siteName)
    {
        if (mediaLoaded)
        {
            drpSiteLibraries.DropDownItems.Clear();

            drpSiteLibraries.SpecialFields.Add(new SpecialField { Text = GetString("general.selectall"), Value = String.Empty });
            drpSiteLibraries.SpecialFields.Add(new SpecialField { Text = GetString("general.selectnone"), Value = "#none#" });
            drpSiteLibraries.SpecialFields.Add(new SpecialField { Text = GetString("dialogs.config.currentlibrary"), Value = "#current#" });

            if (siteName != null)
            {
                drpSiteLibraries.WhereCondition = "LibrarySiteID IN (SELECT SiteID FROM CMS_Site WHERE SiteName = '" + SqlHelper.EscapeQuotes(siteName) + "')";
                drpSiteLibraries.Reload(true);
            }
        }
    }


    /// <summary>
    /// Reloads the group media libraries.
    /// </summary>
    /// <param name="siteName">Name of the site</param>
    /// <param name="groupName">Name of the group</param>
    /// <param name="addNone">If true the (none) option is added</param>
    private void LoadGroupLibraries(string siteName, string groupName, bool addNone = false)
    {
        if (mediaLoaded && communityLoaded)
        {
            drpGroupLibraries.DropDownItems.Clear();
            
            if (addNone)
            {
                drpGroupLibraries.Value = null;
                drpGroupLibraries.SpecialFields.Add(new SpecialField { Text = GetString("general.selectnone"), Value = "#none#" });
            }

            drpGroupLibraries.SpecialFields.Add(new SpecialField { Text = GetString("general.selectall"), Value = String.Empty });
            drpGroupLibraries.SpecialFields.Add(new SpecialField { Text = GetString("dialogs.config.currentlibrary"), Value = "#current#" });

            if ((siteName != null) && (groupName != null))
            {
                drpGroupLibraries.WhereCondition = String.Format("LibraryGroupID IN (SELECT GroupID FROM Community_Group WHERE GroupSiteID IN (SELECT SiteID FROM CMS_Site WHERE SiteName = '{0}') AND GroupName = N'{1}')", SqlHelper.EscapeQuotes(siteName), SqlHelper.EscapeQuotes(groupName));
                drpGroupLibraries.Reload(true);
            }
        }
    }


    /// <summary>
    /// Selects correct item in given DDL.
    /// </summary>
    /// <param name="selector">Selector with the data</param>
    /// <param name="origKey">Key in hashtable which determines whether the value is special or specific item</param>
    /// <param name="singleItemKey">Key in hashtable for specified item</param>
    private void SelectInDDL(UniSelector selector, string origKey, string singleItemKey)
    {
        string item = ValidationHelper.GetString(Form.Data.GetValue(origKey), "").ToLowerCSafe();
        if (item == "#single#")
        {
            item = ValidationHelper.GetString(Form.Data.GetValue(singleItemKey), "");
        }

        ListItem li = selector.DropDownItems.FindByValue(item);
        if (li != null)
        {
            selector.Value = li.Value;
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Sets inner controls according to the parameters and their values included in configuration collection. Parameters collection will be passed from Field editor.
    /// </summary>
    public void LoadValues()
    {
        // Set settings configuration
        plcDisplayAnchor.Visible = DisplayAnchorTabSettings;
        plcDisplayEmail.Visible = DisplayEmailTabSettings;
        plcDisplayWeb.Visible = DisplayWebTabSettings;
        plcAutoResize.Visible = DisplayAutoresize;

        LoadOtherValues();
    }


    /// <summary>
    /// Loads the other fields values to the state of the form control
    /// </summary>
    public override void LoadOtherValues()
    {
        if ((Form == null) || (Form.Data == null))
        {
            return;
        }

        var data = Form.Data;

        if (ContainsColumn("autoresize"))
        {
            elemAutoResize.Value = ValidationHelper.GetString(data.GetValue("autoresize"), null);
        }

        // Content tab
        if (ContainsColumn("dialogs_content_hide"))
        {
            chkDisplayContentTab.Checked = !ValidationHelper.GetBoolean(data.GetValue("dialogs_content_hide"), false);
        }
        if (ContainsColumn("dialogs_content_userelativeurl"))
        {
            chkUseRelativeUrl.Checked = ValidationHelper.GetBoolean(data.GetValue("dialogs_content_userelativeurl"), false);
        }

        if (ContainsColumn("dialogs_content_path"))
        {
            selectPathElem.Value = ValidationHelper.GetString(data.GetValue("dialogs_content_path"), "");
        }

        if (ContainsColumn("dialogs_content_site"))
        {
            siteSelectorContent.Value = ValidationHelper.GetString(data.GetValue("dialogs_content_site"), null);
        }

        // Media tab
        if (mediaLoaded)
        {
            if (ContainsColumn("dialogs_libraries_hide"))
            {
                chkDisplayMediaTab.Checked = !ValidationHelper.GetBoolean(data.GetValue("dialogs_libraries_hide"), false);
            }

            // Site DDL                
            string libSites = null;
            if (ContainsColumn("dialogs_libraries_site"))
            {
                libSites = ValidationHelper.GetString(data.GetValue("dialogs_libraries_site"), null);
            }
            siteSelectorMedia.Value = libSites;
            SelectSite(libSites);

            // Site libraries DDL
            if (ContainsColumn("dialogs_libraries_global") && ContainsColumn("dialogs_libraries_global_libname"))
            {
                SelectInDDL(drpSiteLibraries, "dialogs_libraries_global", "dialogs_libraries_global_libname");
            }

            if (communityLoaded)
            {
                // Groups DDL
                if (ContainsColumn("dialogs_groups") && ContainsColumn("dialogs_groups_name"))
                {
                    SelectInDDL(drpGroups, "dialogs_groups", "dialogs_groups_name");
                }

                SelectGroup();

                // Group libraries DDL
                if (ContainsColumn("dialogs_libraries_group") && ContainsColumn("dialogs_libraries_group_libname"))
                {
                    SelectInDDL(drpGroupLibraries, "dialogs_libraries_group", "dialogs_libraries_group_libname");
                }
            }

            // Starting path
            if (ContainsColumn("dialogs_libraries_path"))
            {
                txtMediaStartPath.Text = ValidationHelper.GetString(data.GetValue("dialogs_libraries_path"), "");
            }
        }

        // Other tabs        
        if (ContainsColumn("dialogs_attachments_hide"))
        {
            chkDisplayAttachments.Checked = !ValidationHelper.GetBoolean(data.GetValue("dialogs_attachments_hide"), false);
        }
        if (ContainsColumn("dialogs_anchor_hide"))
        {
            chkDisplayAnchor.Checked = !ValidationHelper.GetBoolean(data.GetValue("dialogs_anchor_hide"), false);
        }
        if (ContainsColumn("dialogs_email_hide"))
        {
            chkDisplayEmail.Checked = !ValidationHelper.GetBoolean(data.GetValue("dialogs_email_hide"), false);
        }
        if (ContainsColumn("dialogs_web_hide"))
        {
            chkDisplayWeb.Checked = !ValidationHelper.GetBoolean(data.GetValue("dialogs_web_hide"), false);
        }
    }


    /// <summary>
    /// Returns other values related to this control.
    /// </summary>
    public override object[,] GetOtherValues()
    {
        object[,] values = new object[21, 2];
        values[0, 0] = "autoresize";
        values[1, 0] = "autoresize_width";
        values[2, 0] = "autoresize_height";
        values[3, 0] = "autoresize_maxsidesize";
        values[4, 0] = "dialogs_content_hide";
        values[5, 0] = "dialogs_content_path";
        values[6, 0] = "dialogs_content_site";
        values[7, 0] = "dialogs_libraries_hide";
        values[8, 0] = "dialogs_libraries_site";
        values[9, 0] = "dialogs_libraries_global";
        values[10, 0] = "dialogs_libraries_global_libname";
        values[11, 0] = "dialogs_groups";
        values[12, 0] = "dialogs_groups_name";
        values[13, 0] = "dialogs_libraries_group";
        values[14, 0] = "dialogs_libraries_group_libname";
        values[15, 0] = "dialogs_libraries_path";
        values[16, 0] = "dialogs_attachments_hide";
        values[17, 0] = "dialogs_anchor_hide";
        values[18, 0] = "dialogs_email_hide";
        values[19, 0] = "dialogs_web_hide";
        values[20, 0] = "dialogs_content_userelativeurl";

        // Resize control values
        values[0, 1] = elemAutoResize.Value;
        if (plcAutoResize.Visible)
        {
            var resizeValues = elemAutoResize.GetOtherValues();
            if ((resizeValues != null) && (resizeValues.Length > 3))
            {
                values[1, 1] = resizeValues[0, 1];
                values[2, 1] = resizeValues[1, 1];
                values[3, 1] = resizeValues[2, 1];
            }
        }
        else
        {
            // Set default values
            values[1, 1] = Form.GetDataValue("autoresize_width");
            values[2, 1] = Form.GetDataValue("autoresize_height");
            values[3, 1] = Form.GetDataValue("autoresize_maxsidesize");
        }

        // Content tab
        if (!chkDisplayContentTab.Checked)
        {
            values[4, 1] = true;
        }
        else
        {
            values[4, 1] = false;
        }

        values[20, 1] = chkUseRelativeUrl.Checked;


        if ((string)selectPathElem.Value != "")
        {
            values[5, 1] = selectPathElem.Value;
        }

        string selectedSite = ValidationHelper.GetString(siteSelectorContent.Value, String.Empty);
        if (selectedSite != String.Empty)
        {
            values[6, 1] = selectedSite;
        }

        // Media tab
        if (mediaLoaded)
        {
            if (!chkDisplayMediaTab.Checked)
            {
                values[7, 1] = true;
            }

            selectedSite = ValidationHelper.GetString(siteSelectorMedia.Value, String.Empty);
            if (selectedSite != String.Empty)
            {
                values[8, 1] = selectedSite;
            }

            // Site libraries DDL
            var value = ValidationHelper.GetString(drpSiteLibraries.Value, String.Empty);
            if ((value == "#none#") || (value == "#current#"))
            {
                values[9, 1] = value;
            }
            else if (value != "")
            {
                values[9, 1] = "#single#";
                values[10, 1] = value;
            }

            if (communityLoaded)
            {
                // Groups DDL
                value = ValidationHelper.GetString(drpGroups.Value, String.Empty);
                if ((value == "#none#") || (value == "#current#"))
                {
                    values[11, 1] = value;
                }
                else if (value != "")
                {
                    values[11, 1] = "#single#";
                    values[12, 1] = value;
                }

                // Group libraries DDL
                value = ValidationHelper.GetString(drpGroupLibraries.Value, String.Empty);
                if ((value == "#none#") || (value == "#current#"))
                {
                    values[13, 1] = value;
                }
                else if (value != "")
                {
                    values[13, 1] = "#single#";
                    values[14, 1] = value;
                }
            }

            // Starting path
            value = txtMediaStartPath.Text.Trim();
            if (value != "")
            {
                values[15, 1] = value;
            }
        }

        // Other tabs
        if (!chkDisplayAttachments.Checked)
        {
            values[16, 1] = true;
        }
        if (!chkDisplayAnchor.Checked)
        {
            values[17, 1] = true;
        }
        if (!chkDisplayEmail.Checked)
        {
            values[18, 1] = true;
        }
        if (!chkDisplayWeb.Checked)
        {
            values[19, 1] = true;
        }

        return values;
    }


    /// <summary>
    /// Validation of form control.
    /// </summary>
    public override bool IsValid()
    {
        bool isValid = true;

        // Check validity of autoresize element.
        if (plcAutoResize.Visible)
        {
            isValid = elemAutoResize.IsValid();
        }

        if (!ContainsColumn("dialogs_content_hide"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "dialogs_content_hide", GetString("templatedesigner.fieldtypes.boolean"));
            isValid = false;
        }
        if (!ContainsColumn("dialogs_content_path"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "dialogs_content_path", GetString("templatedesigner.fieldtypes.boolean"));
            isValid = false;
        }
        if (!ContainsColumn("dialogs_content_site"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "dialogs_content_site", GetString("general.text"));
            isValid = false;
        }
        if (!ContainsColumn("dialogs_libraries_hide"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "dialogs_libraries_hide", GetString("templatedesigner.fieldtypes.boolean"));
            isValid = false;
        }
        if (!ContainsColumn("dialogs_libraries_site"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "dialogs_libraries_site", GetString("general.text"));
            isValid = false;
        }
        if (!ContainsColumn("dialogs_libraries_global"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "dialogs_libraries_global", GetString("general.text"));
            isValid = false;
        }
        if (!ContainsColumn("dialogs_libraries_global_libname"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "dialogs_libraries_global_libname", GetString("general.text"));
            isValid = false;
        }
        if (!ContainsColumn("dialogs_groups"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "dialogs_groups", GetString("general.text"));
            isValid = false;
        }
        if (!ContainsColumn("dialogs_groups_name"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "dialogs_groups_name", GetString("general.text"));
            isValid = false;
        }
        if (!ContainsColumn("dialogs_libraries_group"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "dialogs_libraries_group", GetString("general.text"));
            isValid = false;
        }
        if (!ContainsColumn("dialogs_libraries_group_libname"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "dialogs_libraries_group_libname", GetString("general.text"));
            isValid = false;
        }
        if (!ContainsColumn("dialogs_libraries_path"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "dialogs_libraries_path", GetString("general.text"));
            isValid = false;
        }
        if (!ContainsColumn("dialogs_attachments_hide"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "dialogs_attachments_hide", GetString("templatedesigner.fieldtypes.boolean"));
            isValid = false;
        }
        if (!ContainsColumn("dialogs_anchor_hide"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "dialogs_anchor_hide", GetString("templatedesigner.fieldtypes.boolean"));
            isValid = false;
        }
        if (!ContainsColumn("dialogs_email_hide"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "dialogs_email_hide", GetString("templatedesigner.fieldtypes.boolean"));
            isValid = false;
        }
        if (!ContainsColumn("dialogs_web_hide"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "dialogs_web_hide", GetString("templatedesigner.fieldtypes.boolean"));
            isValid = false;
        }
        if (!ContainsColumn("autoresize"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "autoresize", GetString("general.text"));
            isValid = false;
        }
        if (!ContainsColumn("autoresize_width"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "autoresize_width", GetString("templatedesigner.fieldtypes.integer"));
            isValid = false;
        }
        if (!ContainsColumn("autoresize_height"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "autoresize_height", GetString("templatedesigner.fieldtypes.integer"));
            isValid = false;
        }
        if (!ContainsColumn("autoresize_maxsidesize"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "autoresize_maxsidesize", GetString("templatedesigner.fieldtypes.integer"));
            isValid = false;
        }

        return isValid;
    }

    #endregion
}