using System;
using System.Collections;
using System.Data;
using System.Web.UI;

using CMS.Base;
using CMS.DocumentEngine;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_PortalEngine_Controls_WebParts_WebpartProperties : CMSUserControl
{
    #region "Variables"

    protected int xmlVersion = 0;
    protected VariantModeEnum mVariantMode = VariantModeEnum.None;


    /// <summary>
    /// Current page info.
    /// </summary>
    private PageInfo pi = null;

    /// <summary>
    /// Page template info.
    /// </summary>
    private PageTemplateInfo pti = null;

    /// <summary>
    /// Currently edited web part.
    /// </summary>
    private WebPartInstance webPartInstance = null;

    /// <summary>
    /// Main web part instance
    /// </summary>
    private WebPartInstance mainWebPartInstance = null;

    /// <summary>
    /// Current page template.
    /// </summary>
    private PageTemplateInstance templateInstance = null;

    /// <summary>
    /// Tree provider.
    /// </summary>
    private TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

    /// <summary>
    /// Gets web part from instance.
    /// </summary>
    private WebPartInfo wpi = null;

    /// <summary>
    /// Indicates whether the new variant should be chosen when closing this dialog
    /// </summary>
    private bool selectNewVariant = false;

    /// <summary>
    /// Preferred culture code to use along with alias path.
    /// </summary>
    private string mCultureCode = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Page alias path.
    /// </summary>
    public string AliasPath
    {
        get;
        set;
    }


    /// <summary>
    /// Preferred culture code to use along with alias path.
    /// </summary>
    public string CultureCode
    {
        get
        {
            if (string.IsNullOrEmpty(mCultureCode))
            {
                mCultureCode = LocalizationContext.PreferredCultureCode;
            }
            return mCultureCode;
        }
        set
        {
            mCultureCode = value;
        }
    }


    /// <summary>
    /// Page template ID.
    /// </summary>
    public int PageTemplateID
    {
        get;
        set;
    }


    /// <summary>
    /// Zone ID.
    /// </summary>
    public string ZoneID
    {
        get;
        set;
    }


    /// <summary>
    /// Web part ID.
    /// </summary>
    public string WebPartID
    {
        get;
        set;
    }


    /// <summary>
    /// ID of the data source web part. If set, the editor edits the nested data source of the web part
    /// </summary>
    public int NestedWebPartID
    {
        get;
        set;
    }


    /// <summary>
    /// Key to the nested web part in the collection of the nested web parts
    /// </summary>
    public string NestedWebPartKey
    {
        get;
        set;
    }


    /// <summary>
    /// Instance GUID.
    /// </summary>
    public Guid InstanceGUID
    {
        get;
        set;
    }


    /// <summary>
    /// True if the web part ID has changed.
    /// </summary>
    public bool WebPartIDChanged
    {
        get;
        private set;
    }


    /// <summary>
    /// Indicates whether the web part is new (inserting) or not (updating).
    /// </summary>
    public bool IsNewWebPart
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the position of the inserted web part.
    /// </summary>
    public int Position
    {
        get;
        set;
    }



    /// <summary>
    /// Relative position of the web part from the left
    /// </summary>
    public int PositionLeft
    {
        get;
        set;
    }


    /// <summary>
    /// Relative position of the web part from the top
    /// </summary>
    public int PositionTop
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether is a new variant.
    /// </summary>
    public bool IsNewVariant
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the actual web part variant ID.
    /// </summary>
    public int VariantID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the web part zone variant ID.
    /// This property is set when adding a new webpart into the zone variant, in all other cases is set to 0.
    /// </summary>
    public int ZoneVariantID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets the variant mode. Indicates whether there are MVT/ContentPersonalization/None variants active.
    /// </summary>
    public VariantModeEnum VariantMode
    {
        get
        {
            return mVariantMode;
        }
        set
        {
            mVariantMode = value;
        }
    }

    #endregion


    #region "Methods"

    public void LoadData()
    {
        if (!StopProcessing)
        {
            LoadForm();

            // Hide ID editing in case the instance is default configuration of the web part
            if ((webPartInstance != null) && (webPartInstance.InstanceGUID == WebPartInfo.DEFAULT_CONFIG_INSTANCEGUID))
            {
                form.FieldsToHide.Add("WebPartControlID");
            }

            // Setup info/error message placeholder
            if (MessagesPlaceHolder != null)
            {
                MessagesPlaceHolder.UseRelativePlaceHolder = false;
                form.EnsureMessagesPlaceholder(MessagesPlaceHolder);
            }

            ScriptHelper.RegisterEditScript(Page, false);
            ScriptHelper.RegisterJQuery(Page);
        }
    }


    protected void lnkLoadDefaults_Click(object sender, EventArgs e)
    {
        // Get the web part form info
        FormInfo fi = GetWebPartFormInfo();
        if (fi != null)
        {
            // Create DataRow with default data
            var dr = fi.GetDataRow();

            // Load default values
            fi.LoadDefaultValues(dr);

            // Load to the form
            form.LoadData(dr);
        }
    }


    /// <summary>
    /// Loads the web part form.
    /// </summary>
    protected void LoadForm()
    {
        // Load settings
        if (!string.IsNullOrEmpty(Request.Form[hdnIsNewWebPart.UniqueID]))
        {
            IsNewWebPart = ValidationHelper.GetBoolean(Request.Form[hdnIsNewWebPart.UniqueID], false);
        }
        if (!string.IsNullOrEmpty(Request.Form[hdnInstanceGUID.UniqueID]))
        {
            InstanceGUID = ValidationHelper.GetGuid(Request.Form[hdnInstanceGUID.UniqueID], Guid.Empty);
        }

        // Indicates whether the new variant should be chosen when closing this dialog
        selectNewVariant = IsNewVariant;

        // Try to find the web part variant in the database and set its VariantID
        if (IsNewVariant)
        {
            Hashtable varProperties = WindowHelper.GetItem("variantProperties") as Hashtable;
            if (varProperties != null)
            {
                // Get the variant code name from the WindowHelper
                string variantName = ValidationHelper.GetString(varProperties["codename"], string.Empty);

                // Check if the variant exists in the database
                int variantIdFromDB = VariantHelper.GetVariantID(VariantMode, PageTemplateID, variantName, true);

                // Set the variant id from the database
                if (variantIdFromDB > 0)
                {
                    VariantID = variantIdFromDB;
                    IsNewVariant = false;
                }
            }
        }

        if (!String.IsNullOrEmpty(WebPartID))
        {
            // Get the page info
            pi = CMSWebPartPropertiesPage.GetPageInfo(AliasPath, PageTemplateID, CultureCode);
            if (pi != null)
            {
                // Get template
                pti = pi.UsedPageTemplateInfo;

                // Get template instance
                templateInstance = pti.TemplateInstance;

                if (!IsNewWebPart)
                {
                    // Standard zone
                    webPartInstance = templateInstance.GetWebPart(InstanceGUID, WebPartID);

                    // If the web part not found, try to find it among the MVT/CP variants
                    if (webPartInstance == null)
                    {
                        // MVT/CP variant
                        templateInstance.LoadVariants(false, VariantModeEnum.None);
                        webPartInstance = templateInstance.GetWebPart(InstanceGUID, -1, 0);

                        // Set the VariantMode according to the selected web part/zone variant
                        if ((webPartInstance != null) && (webPartInstance.ParentZone != null))
                        {
                            VariantMode = (webPartInstance.VariantMode != VariantModeEnum.None) ? webPartInstance.VariantMode : webPartInstance.ParentZone.VariantMode;
                        }
                        else
                        {
                            VariantMode = VariantModeEnum.None;
                        }
                    }
                    else
                    {
                        // Ensure that the ZoneVariantID is not set when the web part was found in a regular zone
                        ZoneVariantID = 0;
                    }

                    if ((VariantID > 0) && (webPartInstance != null) && (webPartInstance.PartInstanceVariants != null))
                    {
                        // Check OnlineMarketing permissions.
                        if (CheckPermissions("Read"))
                        {
                            webPartInstance = webPartInstance.FindVariant(VariantID);
                        }
                        else
                        {
                            // Not authorized for OnlineMarketing - Manage.
                            RedirectToInformation(String.Format(GetString("general.permissionresource"), "Read", (VariantMode == VariantModeEnum.ContentPersonalization) ? "CMS.ContentPersonalization" : "CMS.MVTest"));
                        }
                    }

                    if (webPartInstance == null)
                    {
                        UIContext.EditedObject = null;
                        return;
                    }
                }

                mainWebPartInstance = webPartInstance;

                // Keep xml version
                if (webPartInstance != null)
                {
                    xmlVersion = webPartInstance.XMLVersion;

                    // If data source ID set, edit the data source
                    if (NestedWebPartID > 0)
                    {
                        webPartInstance = webPartInstance.NestedWebParts[NestedWebPartKey] ?? new WebPartInstance() { InstanceGUID = Guid.NewGuid() };
                    }
                }

                // Get the form info
                FormInfo fi = GetWebPartFormInfo();

                // Get the form definition
                if (fi != null)
                {
                    fi.ContextResolver.Settings.RelatedObject = templateInstance;
                    form.AllowMacroEditing = ((WebPartTypeEnum)wpi.WebPartType != WebPartTypeEnum.Wireframe);

                    // Get data row with required columns
                    DataRow dr = fi.GetDataRow();

                    if (IsNewWebPart || (xmlVersion > 0))
                    {
                        fi.LoadDefaultValues(dr);
                    }

                    // Load values from existing web part
                    LoadDataRowFromWebPart(dr, webPartInstance, fi);

                    // Set a unique WebPartControlID for the new variant
                    if (IsNewVariant || IsNewWebPart)
                    {
                        // Set control ID
                        string webPartControlId = ValidationHelper.GetCodeName(wpi.WebPartDisplayName);
                        dr["WebPartControlID"] = WebPartZoneInstance.GetUniqueWebPartId(webPartControlId, templateInstance);
                    }

                    // Init the form
                    InitForm(form, dr, fi);

                    AddExportLink();
                }
                else
                {
                    UIContext.EditedObject = null;
                }
            }
        }
    }


    /// <summary>
    /// Loads the web part info
    /// </summary>
    private void EnsureWebPartInfo()
    {
        if (wpi != null)
        {
            return;
        }

        if (NestedWebPartID > 0)
        {
            // Setup the form for data source
            wpi = WebPartInfoProvider.GetWebPartInfo(NestedWebPartID);
            form.Mode = FormModeEnum.Update;
            return;
        }

        if (!IsNewWebPart)
        {
            // Get web part by code name
            wpi = WebPartInfoProvider.GetWebPartInfo(webPartInstance.WebPartType);
            form.Mode = FormModeEnum.Update;
        }
        else
        {
            // Web part instance wasn't created yet, get by web part ID
            wpi = WebPartInfoProvider.GetWebPartInfo(ValidationHelper.GetInteger(WebPartID, 0));
            form.Mode = FormModeEnum.Insert;
        }
    }


    /// <summary>
    /// Gets the form info for the given web part
    /// </summary>
    private FormInfo GetWebPartFormInfo()
    {
        EnsureWebPartInfo();

        if (wpi == null)
        {
            return null;
        }

        return wpi.GetWebPartFormInfo();
    }


    /// <summary>
    /// Checks permissions (depends on variant mode) 
    /// </summary>
    /// <param name="permissionName">Name of permission to test</param>
    private bool CheckPermissions(string permissionName)
    {
        var cui = MembershipContext.AuthenticatedUser;
        switch (VariantMode)
        {
            case VariantModeEnum.MVT:
                return cui.IsAuthorizedPerResource("cms.mvtest", permissionName);

            case VariantModeEnum.ContentPersonalization:
                return cui.IsAuthorizedPerResource("cms.contentpersonalization", permissionName);

            case VariantModeEnum.Conflicted:
            case VariantModeEnum.None:
                return cui.IsAuthorizedPerResource("cms.mvtest", permissionName) || cui.IsAuthorizedPerResource("cms.contentpersonalization", permissionName);
        }

        return true;
    }


    /// <summary>
    /// Loads the data row data from given web part instance.
    /// </summary>
    /// <param name="dr">DataRow to fill</param>
    /// <param name="webPart">Source web part</param>
    /// <param name="formInfo">Web part form info</param>
    private void LoadDataRowFromWebPart(DataRow dr, WebPartInstance webPart, FormInfo formInfo)
    {
        if (webPart != null)
        {
            foreach (DataColumn column in dr.Table.Columns)
            {
                try
                {
                    bool load = true;
                    // switch by xml version
                    switch (xmlVersion)
                    {
                        case 1:
                            load = webPart.Properties.Contains(column.ColumnName.ToLowerCSafe()) || column.ColumnName.EqualsCSafe("webpartcontrolid", true);
                            break;
                        // Version 0
                        default:
                            // Load default value for Boolean type in old XML version
                            if ((column.DataType == typeof(bool)) && !webPart.Properties.Contains(column.ColumnName.ToLowerCSafe()))
                            {
                                FormFieldInfo ffi = formInfo.GetFormField(column.ColumnName);
                                if (ffi != null)
                                {
                                    webPart.SetValue(column.ColumnName, ffi.GetPropertyValue(FormFieldPropertyEnum.DefaultValue));
                                }
                            }
                            break;
                    }

                    if (load)
                    {
                        var value = webPart.GetValue(column.ColumnName);

                        // Convert value into default format
                        if ((value != null) && (ValidationHelper.GetString(value, String.Empty) != String.Empty))
                        {
                            if (column.DataType == typeof(decimal))
                            {
                                value = ValidationHelper.GetDouble(value, 0, "en-us");
                            }

                            if (column.DataType == typeof(DateTime))
                            {
                                value = ValidationHelper.GetDateTime(value, DateTime.MinValue, "en-us");
                            }
                        }

                        DataHelper.SetDataRowValue(dr, column.ColumnName, value);
                    }
                }
                catch (Exception ex)
                {
                    EventLogProvider.LogException("WebPartProperties", "LOADDATAROW", ex);
                }
            }
        }
    }


    /// <summary>
    /// Initializes the form.
    /// </summary>
    /// <param name="basicForm">Form</param>
    /// <param name="dr">Data row with the data</param>
    /// <param name="fi">Form info</param>
    private void InitForm(BasicForm basicForm, DataRow dr, FormInfo fi)
    {
        if (basicForm != null)
        {
            basicForm.DataRow = dr;
            if (webPartInstance != null)
            {
                basicForm.MacroTable = webPartInstance.MacroTable;
            }
            else
            {
                basicForm.MacroTable = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
            }

            if (!RequestHelper.IsPostBack() && (webPartInstance != null))
            {
                fi = new FormInfo(fi.GetXmlDefinition());

                // Load the collapsed/un-collapsed state of categories
                var categories = fi.GetCategoryNames();
                foreach (string category in categories)
                {
                    FormCategoryInfo fci = fi.GetFormCategory(category);
                    if (ValidationHelper.GetBoolean(fci.GetPropertyValue(FormCategoryPropertyEnum.Collapsible, basicForm.ContextResolver), false) && ValidationHelper.GetBoolean(fci.GetPropertyValue(FormCategoryPropertyEnum.CollapsedByDefault, basicForm.ContextResolver), false) && ValidationHelper.GetBoolean(webPartInstance.GetValue("cat_open_" + category), false))
                    {
                        fci.SetPropertyValue(FormCategoryPropertyEnum.CollapsedByDefault, "false");
                    }
                }
            }

            basicForm.SubmitButton.Visible = false;
            basicForm.SiteName = SiteContext.CurrentSiteName;
            basicForm.FormInformation = fi;
            basicForm.ShowPrivateFields = true;
            basicForm.OnItemValidation += formElem_OnItemValidation;
            basicForm.ReloadData();
        }
    }


    /// <summary>
    /// Adds the export link.
    /// </summary>
    private void AddExportLink()
    {
        if (webPartInstance != null)
        {
            lnkExport.OnClientClick = "window.open('GetWebPartProperties.aspx?webpartid=" + webPartInstance.ControlID + "&webpartguid=" + webPartInstance.InstanceGUID + "&aliaspath=" + AliasPath + "&zoneid=" + ZoneID + "&templateid="+ PageTemplateID + "'); return false;";
        }
    }

    #endregion


    #region "Save methods"

    /// <summary>
    /// Raised when the Save action is required.
    /// </summary>
    public bool OnSave()
    {
        if (Save())
        {
            hdnIsNewWebPart.Value = "false";

            if (webPartInstance != null)
            {
                hdnInstanceGUID.Value = webPartInstance.InstanceGUID.ToString();

                if (selectNewVariant)
                {
                    // Select the new variant
                    string script = "SendEvent('updatevariantposition', true, { itemCode: 'Variant_WP_" + webPartInstance.InstanceGUID.ToString("N") + "', variantId: -1 }); ";

                    ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "CustomScripts", script, true);
                }
            }

            AddExportLink();

            return true;
        }

        return false;
    }


    /// <summary>
    /// Saves the given form.
    /// </summary>
    /// <param name="form">Form to save</param>
    private static bool SaveForm(BasicForm form)
    {
        if ((form != null) && form.Visible)
        {
            return form.SaveData("");
        }

        return true;
    }


    /// <summary>
    /// Control ID validation.
    /// </summary>
    private void formElem_OnItemValidation(object sender, ref string errorMessage)
    {
        Control ctrl = (Control)sender;
        if (ctrl.ID.ToLowerCSafe() == "webpartcontrolid")
        {
            FormEngineUserControl ctrlTextbox = (FormEngineUserControl)ctrl;
            
            // New web part control id 
            string newControlId = ValidationHelper.GetString(ctrlTextbox.Value, null);

            // Load the web part variants if not loaded yet
            if ((PortalContext.MVTVariantsEnabled && !templateInstance.MVTVariantsLoaded) ||
                (PortalContext.ContentPersonalizationEnabled && !templateInstance.ContentPersonalizationVariantsLoaded))
            {
                templateInstance.LoadVariants(false, VariantModeEnum.None);
            }

            // New or changed web part control id
            bool checkIdUniqueness = IsNewWebPart || IsNewVariant || (webPartInstance == null) || (webPartInstance.ControlID != newControlId);

            // Try to find a web part with the same web part control id amongst all the web parts and their variants
            if (checkIdUniqueness
                && (templateInstance.GetWebPart(newControlId, true) != null))
            {
                // Error - duplicity IDs
                errorMessage = GetString("WebPartProperties.ErrorUniqueID");
            }
            else
            {
                string uniqueId = WebPartZoneInstance.GetUniqueWebPartId(newControlId, pi.TemplateInstance);
                if (!uniqueId.EqualsCSafe(newControlId, true))
                {
                    // Check if there is already a widget with the same id in the page
                    WebPartInstance foundWidget = pi.TemplateInstance.GetWebPart(newControlId);
                    if ((foundWidget != null) && foundWidget.IsWidget)
                    {
                        // Error - the ID collide with another widget which is already in the page
                        errorMessage = ResHelper.GetString("WidgetProperties.ErrorUniqueID");
                    }
                }
            }
        }
    }


    /// <summary>
    /// Saves webpart properties.
    /// </summary>
    public bool Save()
    {
        // Check MVT/CP security
        if (VariantID > 0)
        {
            // Check OnlineMarketing permissions.
            if (!CheckPermissions("Manage"))
            {
                ShowError("general.modifynotallowed");
                return false;
            }
        }

        // Save the data
        if ((pi != null) && (pti != null) && (templateInstance != null) && SaveForm(form))
        {
            if (SynchronizationHelper.IsCheckedOutByOtherUser(pti))
            {
                string userName = null;
                UserInfo ui = UserInfoProvider.GetUserInfo(pti.Generalized.IsCheckedOutByUserID);
                if (ui != null)
                {
                    userName = HTMLHelper.HTMLEncode(ui.GetFormattedUserName(IsLiveSite));
                }

                ShowError(string.Format(GetString("ObjectEditMenu.CheckedOutByAnotherUser"), pti.TypeInfo.ObjectType, pti.DisplayName, userName));
                return false;
            }

            // Add web part if new
            if (IsNewWebPart)
            {
                int webpartId = ValidationHelper.GetInteger(WebPartID, 0);

                // Ensure layout zone flag
                if (QueryHelper.GetBoolean("layoutzone", false))
                {
                    WebPartZoneInstance zone = pti.TemplateInstance.EnsureZone(ZoneID);
                    zone.LayoutZone = true;
                }

                webPartInstance = PortalHelper.AddNewWebPart(webpartId, ZoneID, false, ZoneVariantID, Position, templateInstance);

                // Set default layout
                if (wpi.WebPartParentID > 0)
                {
                    WebPartLayoutInfo wpli = WebPartLayoutInfoProvider.GetDefaultLayout(wpi.WebPartID);
                    if (wpli != null)
                    {
                        webPartInstance.SetValue("WebPartLayout", wpli.WebPartLayoutCodeName);
                    }
                }
            }

            webPartInstance.XMLVersion = 1;
            if (IsNewVariant)
            {
                webPartInstance = webPartInstance.Clone();
                webPartInstance.VariantMode = VariantModeFunctions.GetVariantModeEnum(QueryHelper.GetString("variantmode", String.Empty).ToLowerCSafe());
            }

            // Get basic form's data row and update web part
            SaveFormToWebPart(form);

            // Set new position if set
            if (PositionLeft > 0)
            {
                webPartInstance.SetValue("PositionLeft", PositionLeft);
            }
            if (PositionTop > 0)
            {
                webPartInstance.SetValue("PositionTop", PositionTop);
            }

            // Ensure the data source web part instance in the main web part
            if (NestedWebPartID > 0)
            {
                webPartInstance.WebPartType = wpi.WebPartName;
                mainWebPartInstance.NestedWebParts[NestedWebPartKey] = webPartInstance;
            }

            bool isWebPartVariant = (VariantID > 0) || (ZoneVariantID > 0) || IsNewVariant;
            if (!isWebPartVariant)
            {
                // Save the changes  
                CMSPortalManager.SaveTemplateChanges(pi, templateInstance, WidgetZoneTypeEnum.None, ViewModeEnum.Design, tree);
            }
            else
            {
                Hashtable varProperties = WindowHelper.GetItem("variantProperties") as Hashtable;
                // Save changes to the web part variant
                VariantHelper.SaveWebPartVariantChanges(webPartInstance, VariantID, ZoneVariantID, VariantMode, varProperties);
            }

            // Reload the form (because of macro values set only by JS)
            form.ReloadData();

            // Clear the cached web part
            CacheHelper.TouchKey("webpartinstance|" + InstanceGUID.ToString().ToLowerCSafe());

            ShowChangesSaved();

            return true;
        }
        else if ((mainWebPartInstance != null) && (mainWebPartInstance.ParentZone != null) && (mainWebPartInstance.ParentZone.ParentTemplateInstance != null))
        {
            // Reload the zone/web part variants when saving of the form fails
            mainWebPartInstance.ParentZone.ParentTemplateInstance.LoadVariants(true, VariantModeEnum.None);
        }

        return false;
    }


    /// <summary>
    /// Saves the given DataRow data to the web part properties.
    /// </summary>
    /// <param name="basicForm">Form to save</param>
    private void SaveFormToWebPart(BasicForm basicForm)
    {
        if (basicForm.Visible && (webPartInstance != null))
        {
            // Keep the old ID to check the change of the ID
            string oldId = webPartInstance.ControlID.ToLowerCSafe();

            DataRow dr = basicForm.DataRow;
            foreach (DataColumn column in dr.Table.Columns)
            {
                webPartInstance.MacroTable[column.ColumnName.ToLowerCSafe()] = basicForm.MacroTable[column.ColumnName.ToLowerCSafe()];
                webPartInstance.SetValue(column.ColumnName, dr[column]);

                // If name changed, move the content
                if (column.ColumnName.ToLowerCSafe() == "webpartcontrolid")
                {
                    try
                    {
                        string newId = null;
                        if (!IsNewVariant)
                        {
                            newId = ValidationHelper.GetString(dr[column], "").ToLowerCSafe();
                        }

                        // Name changed
                        if ((!string.IsNullOrEmpty(newId)) && (newId != oldId))
                        {
                            if (!IsNewWebPart && !IsNewVariant)
                            {
                                WebPartIDChanged = true;
                            }
                            WebPartID = newId;

                            // Move the document content if present
                            ChangeEditableContentIDs(oldId, newId);

                            // Change the underlying zone names if layout web part
                            if ((wpi != null) && ((WebPartTypeEnum)wpi.WebPartType == WebPartTypeEnum.Layout))
                            {
                                ChangeLayoutZoneIDs(oldId, newId);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        EventLogProvider.LogException("Content", "CHANGEWEBPART", ex);
                    }
                }
            }

            SaveCategoryState(basicForm);
        }
    }


    /// <summary>
    /// Changes the IDs of the editable content
    /// </summary>
    /// <param name="oldId">Old web part ID</param>
    /// <param name="newId">New web part ID</param>
    private void ChangeEditableContentIDs(string oldId, string newId)
    {
        string currentContent = pi.EditableWebParts[oldId];
        if (currentContent != null)
        {
            TreeNode node = DocumentHelper.GetDocument(pi.DocumentID, tree);

            // Move the content in the page info
            pi.EditableWebParts[oldId] = null;
            pi.EditableWebParts[newId] = currentContent;

            // Update the document
            node.SetValue("DocumentContent", pi.GetContentXml());
            DocumentHelper.UpdateDocument(node, tree);
        }
    }


    /// <summary>
    /// Saves collapsed/un-collapsed states of the categories
    /// </summary>
    /// <param name="basicForm">Form</param>
    private void SaveCategoryState(BasicForm basicForm)
    {
        // Save the collapsed/un-collapsed state of categories
        FormInfo fi = GetWebPartFormInfo();

        var categories = fi.GetCategoryNames();
        foreach (string category in categories)
        {
            FormCategoryInfo fci = fi.GetFormCategory(category);
            if (ValidationHelper.GetBoolean(fci.GetPropertyValue(FormCategoryPropertyEnum.Collapsible, basicForm.ContextResolver), false) &&
                ValidationHelper.GetBoolean(fci.GetPropertyValue(FormCategoryPropertyEnum.CollapsedByDefault, basicForm.ContextResolver), false))
            {
                if (basicForm.IsCategoryCollapsed(category))
                {
                    webPartInstance.SetValue("cat_open_" + category, null);
                }
                else
                {
                    webPartInstance.SetValue("cat_open_" + category, true);
                }
            }
        }
    }


    /// <summary>
    /// Changes the layout zone IDs based on the change of the web part ID
    /// </summary>
    /// <param name="oldId">Old web part ID</param>
    /// <param name="newId">New web part ID</param>
    private void ChangeLayoutZoneIDs(string oldId, string newId)
    {
        string prefix = oldId + "_";

        foreach (WebPartZoneInstance zone in pti.WebPartZones)
        {
            if (zone.ZoneID.StartsWithCSafe(prefix, true))
            {
                // Change the zone prefix to the new one
                zone.ZoneID = newId + "_" + zone.ZoneID.Substring(prefix.Length);
            }
        }
    }

    #endregion
}
