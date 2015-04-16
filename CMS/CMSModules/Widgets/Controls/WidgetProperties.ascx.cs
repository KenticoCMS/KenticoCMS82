using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.Base;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DataEngine;
using CMS.Synchronization;

using MessageTypeEnum = CMS.ExtendedControls.MessageTypeEnum;
using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_Widgets_Controls_WidgetProperties : CMSUserControl
{
    #region "Variables"

    private VariantModeEnum mVariantMode = VariantModeEnum.None;
    private WidgetZoneTypeEnum mZoneType = WidgetZoneTypeEnum.None;
    private bool isValidWidget = true;
    private int xmlVersion = 0;

    // Web part info.
    private WebPartInfo wpi = null;

    // Current widget (alias web part instance).
    private WebPartInstance widgetInstance = null;

    // Current page template.
    private PageTemplateInstance templateInstance = null;

    // Zone instance.
    private WebPartZoneInstance zone = null;

    // Tree provider.
    private readonly TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

    // Result of transformation (loaded in init).
    private List<FormFieldInfo> mFields;

    // Widget info object
    private WidgetInfo wi = null;

    // Preferred culture code to use along with alias path.
    private string mCultureCode = null;

    #endregion


    #region "Events"

    /// <summary>
    /// On not allowed - security check.
    /// </summary>
    public event EventHandler OnNotAllowed;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Current page info.
    /// </summary>
    public new PageInfo CurrentPageInfo
    {
        get;
        set;
    }


    /// <summary>
    /// Zone type.
    /// </summary>
    public WidgetZoneTypeEnum ZoneType
    {
        get
        {
            return mZoneType;
        }
        set
        {
            mZoneType = value;
        }
    }


    /// <summary>
    /// Indicated whether the widget is inline.
    /// </summary>
    public bool IsInline
    {
        get;
        set;
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
    public int PageTemplateId
    {
        get;
        set;
    }


    /// <summary>
    /// Zone ID.
    /// </summary>
    public string ZoneId
    {
        get;
        set;
    }


    /// <summary>
    /// Widget identifier.
    /// </summary>
    public string WidgetId
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


    /// <summary>
    /// True if the web part ID has changed.
    /// </summary>
    public bool WidgetIdChanged
    {
        get;
        private set;
    }


    /// <summary>
    /// Before form definition.
    /// </summary>
    public string BeforeFormDefinition
    {
        get;
        set;
    }


    /// <summary>
    /// After form definition.
    /// </summary>
    public string AfterFormDefinition
    {
        get;
        set;
    }


    /// <summary>
    /// Whether is widget new or not.
    /// </summary>
    public bool IsNewWidget
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
    /// Gets or sets the actual widget variant ID.
    /// </summary>
    public int VariantID
    {
        get;
        set;
    }


    /// <summary>
    /// Ensure portal view mode for dashboards.
    /// </summary>
    public void EnsureDashboard()
    {
        if (PageTemplateId > 0)
        {
            PageTemplateInfo pti = PageTemplateInfoProvider.GetPageTemplateInfo(PageTemplateId);
            if ((pti != null) && (pti.PageTemplateType == PageTemplateTypeEnum.Dashboard))
            {
                PortalContext.SetRequestViewMode(ViewModeEnum.DashboardWidgets);
                PortalContext.DashboardName = QueryHelper.GetString("dashboard", String.Empty);
                PortalContext.DashboardSiteName = QueryHelper.GetString("sitename", String.Empty);
                formCustom.DisplayContext = FormInfo.DISPLAY_CONTEXT_DASHBOARD;
            }
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        LoadForm();

        // Override BasicStyles flag to ensure correct styles in widget properties on the dashboard
        MessagesPlaceHolder.BasicStyles = false;
        MessagesPlaceHolder.WrapperControlClientID = formCustom.ClientID;

        ScriptHelper.RegisterEditScript(Page, false);

        // Register css file with editor classes
        CSSHelper.RegisterCSSLink(Page, "~/CMSModules/Widgets/CSS/editor.css");

        string definition = ValidationHelper.GetString(SessionHelper.GetValue("WidgetDefinition"), string.Empty);
        if (String.IsNullOrEmpty(definition))
        {
            hdnWidgetDefinition.Value = definition;
            SessionHelper.Remove("WidgetDefinition");
        }

        DocumentManager.RegisterSaveChangesScript = false;
    }


    /// <summary>
    /// Control ID validation.
    /// </summary>
    protected void formElem_OnItemValidation(object sender, ref string errorMessage)
    {
        Control ctrl = (Control)sender;
        if (CMSString.Compare(ctrl.ID, "widgetcontrolid", true) == 0)
        {
            TextBox ctrlTextbox = (TextBox)ctrl;
            string newId = ctrlTextbox.Text;

            var pti = CMSPortalManager.GetTemplateInstanceForEditing(CurrentPageInfo);

            // Validate unique ID
            WebPartInstance existingPart = pti.GetWebPart(newId);
            if ((existingPart != null) && (existingPart != widgetInstance) && (existingPart.InstanceGUID != widgetInstance.InstanceGUID))
            {
                // Error - duplicated IDs
                errorMessage = GetString("Widgets.Properties.ErrorUniqueID");
            }
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Saves the widget properties and closes the window.
    /// </summary>
    public bool OnSave()
    {
        // Save widget properties
        if (IsInline)
        {
            // Is valid widget
            if (!isValidWidget)
            {
                ltlScript.Text += ScriptHelper.GetScript("CloseDialog(false);");
                return false;
            }

            // Register HTMLEditor.js script file
            ScriptHelper.RegisterScriptFile(Page, "~/CMSScripts/Dialogs/HTMLEditor.js");

            string widgetString = SaveInline();

            // Validate the data input
            if (String.IsNullOrEmpty(widgetString))
            {
                return false;
            }

            SessionHelper.Remove("WidgetDefinition");
            ltlScript.Text += ScriptHelper.GetScript(widgetString + "CloseDialog(false);");

            return true;
        }
        else
        {
            // Save the widget
            if (Save())
            {
                if (!IsLiveSite)
                {
                    StringBuilder sb = new StringBuilder();

                    if (IsNewVariant && (widgetInstance != null))
                    {
                        // Select the new variant
                        sb.Append("SendEvent('updatevariantposition', true, { itemCode: 'Variant_WP_", widgetInstance.InstanceGUID.ToString("N"), "', variantId: -1 }); ");
                    }
                    else
                    {
                        // Inform the parent page that the content has changed
                        sb.Append("SendEvent('setcontentchanged'); ");
                    }

                    ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "CustomScripts", sb.ToString(), true);
                }

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Saves the widget properties.
    /// </summary>
    public bool OnApply()
    {
        if (Save())
        {
            hdnIsNewWebPart.Value = "false";
            hdnInstanceGUID.Value = widgetInstance.InstanceGUID.ToString();

            if (WidgetIdChanged)
            {
                ltlScript.Text += ScriptHelper.GetScript("SendEvent('changewidget', true, { zoneId: '" + ZoneId + "', widgetId: '" + WidgetId + "', aliasPath: '" + AliasPath + "'}); ");
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Overrides the AbstractUserControl.ShowMessage method. Ensures that the message will use the standard rich CSS styles on live site as well.
    /// </summary>
    /// <param name="type">Message type</param>
    /// <param name="text">Message text</param>
    /// <param name="description">Additional description</param>
    /// <param name="tooltipText">Tooltip text</param>
    /// <param name="persistent">Indicates if the message is persistent</param>
    public override void ShowMessage(MessageTypeEnum type, string text, string description, string tooltipText, bool persistent)
    {
        ShowMessage(type, text, description, tooltipText, persistent, false);
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes the form.
    /// </summary>
    /// <param name="form">Form</param>
    /// <param name="dr">Datarow with the data</param>
    /// <param name="fi">Form info</param>
    private void InitForm(BasicForm form, DataRow dr, FormInfo fi)
    {
        form.DataRow = dr;
        form.MacroTable = widgetInstance != null ? widgetInstance.MacroTable : new Hashtable(StringComparer.InvariantCultureIgnoreCase);

        form.SubmitButton.Visible = false;
        form.SiteName = SiteContext.CurrentSiteName;
        form.FormInformation = fi;
        form.ShowPrivateFields = true;
        form.OnItemValidation += formElem_OnItemValidation;
        form.Mode = IsNewWidget ? FormModeEnum.Insert : FormModeEnum.Update;

        form.ReloadData();
    }


    /// <summary>
    /// Loads the widget form.
    /// </summary>
    private void LoadForm()
    {
        // Setup basic form on live site
        formCustom.AllowMacroEditing = false;
        formCustom.IsLiveSite = IsLiveSite;

        // Load settings
        if (!String.IsNullOrEmpty(Request.Form[hdnIsNewWebPart.UniqueID]))
        {
            IsNewWidget = ValidationHelper.GetBoolean(Request.Form[hdnIsNewWebPart.UniqueID], false);
        }
        if (!String.IsNullOrEmpty(Request.Form[hdnInstanceGUID.UniqueID]))
        {
            InstanceGUID = ValidationHelper.GetGuid(Request.Form[hdnInstanceGUID.UniqueID], Guid.Empty);
        }

        // Try to find the widget variant in the database and set its VariantID
        if (IsNewVariant)
        {
            Hashtable properties = WindowHelper.GetItem("variantProperties") as Hashtable;
            if (properties != null)
            {
                // Get the variant code name from the WindowHelper
                string variantName = ValidationHelper.GetString(properties["codename"], string.Empty);

                // Check if the variant exists in the database
                int variantIdFromDB = VariantHelper.GetVariantID(VariantMode, PageTemplateId, variantName, false);

                // Set the variant id from the database
                if (variantIdFromDB > 0)
                {
                    VariantID = variantIdFromDB;
                    IsNewVariant = false;
                }
            }
        }

        EnsureDashboard();

        if (!String.IsNullOrEmpty(WidgetId) && !IsInline)
        {
            if (CurrentPageInfo == null)
            {
                ShowError(GetString("Widgets.Properties.aliasnotfound"));
                pnlFormArea.Visible = false;
                return;
            }

            // Get template instance
            templateInstance = CMSPortalManager.GetTemplateInstanceForEditing(CurrentPageInfo);

            if (!IsNewWidget)
            {
                // Get the instance of widget
                widgetInstance = templateInstance.GetWebPart(InstanceGUID, WidgetId);
                if (widgetInstance == null)
                {
                    ShowError(GetString("Widgets.Properties.WidgetNotFound"));
                    pnlFormArea.Visible = false;
                    return;
                }
                
                if ((VariantID > 0) && (widgetInstance != null) && (widgetInstance.PartInstanceVariants != null))
                {
                    // Check OnlineMarketing permissions.
                    if (CheckPermissions("Read"))
                    {
                        widgetInstance = CurrentPageInfo.DocumentTemplateInstance.GetWebPart(InstanceGUID, WidgetId);
                        widgetInstance = widgetInstance.PartInstanceVariants.Find(v => v.VariantID.Equals(VariantID));
                        // Set the widget variant mode
                        if (widgetInstance != null)
                        {
                            VariantMode = widgetInstance.VariantMode;
                        }
                    }
                    else
                    {
                        // Not authorized for OnlineMarketing - Manage.
                        RedirectToInformation(String.Format(GetString("general.permissionresource"), "Read", (VariantMode == VariantModeEnum.ContentPersonalization) ? "CMS.ContentPersonalization" : "CMS.MVTest"));
                    }
                }

                // Get widget info by widget name(widget type)
                wi = WidgetInfoProvider.GetWidgetInfo(widgetInstance.WebPartType);
            }
            // Widget instance hasn't created yet
            else
            {
                wi = WidgetInfoProvider.GetWidgetInfo(ValidationHelper.GetInteger(WidgetId, 0));
            }

            // Keep xml version
            if (widgetInstance != null)
            {
                xmlVersion = widgetInstance.XMLVersion;
            }

            UIContext.EditedObject = wi;

            // Get the zone to which it inserts
            WebPartZoneInstance zone = templateInstance.GetZone(ZoneId);
            if ((ZoneType == WidgetZoneTypeEnum.None) && (zone != null))
            {
                ZoneType = zone.WidgetZoneType;
            }

            // Check security
            var currentUser = MembershipContext.AuthenticatedUser;

            switch (ZoneType)
            {
                // Group zone => Only group widgets and group admin
                case WidgetZoneTypeEnum.Group:
                    // Should always be, only group widget are allowed in group zone
                    if (!wi.WidgetForGroup || (!currentUser.IsGroupAdministrator(CurrentPageInfo.NodeGroupID) && ((PortalContext.ViewMode != ViewModeEnum.Design) || ((PortalContext.ViewMode == ViewModeEnum.Design) && (!currentUser.IsAuthorizedPerResource("CMS.Design", "Design"))))))
                    {
                        if (OnNotAllowed != null)
                        {
                            OnNotAllowed(this, null);
                        }
                    }
                    break;

                // Widget must be allowed for editor zones
                case WidgetZoneTypeEnum.Editor:
                    if (!wi.WidgetForEditor)
                    {
                        if (OnNotAllowed != null)
                        {
                            OnNotAllowed(this, null);
                        }
                    }
                    break;

                // Widget must be allowed for user zones
                case WidgetZoneTypeEnum.User:
                    if (!wi.WidgetForUser)
                    {
                        if (OnNotAllowed != null)
                        {
                            OnNotAllowed(this, null);
                        }
                    }
                    break;

                // Widget must be allowed for dashboard zones
                case WidgetZoneTypeEnum.Dashboard:
                    if (!wi.WidgetForDashboard)
                    {
                        if (OnNotAllowed != null)
                        {
                            OnNotAllowed(this, null);
                        }
                    }
                    break;
            }

            // Check security
            if ((ZoneType != WidgetZoneTypeEnum.Group) && !WidgetRoleInfoProvider.IsWidgetAllowed(wi, currentUser.UserID, AuthenticationHelper.IsAuthenticated()))
            {
                if (OnNotAllowed != null)
                {
                    OnNotAllowed(this, null);
                }
            }

            // Get form schemas
            wpi = WebPartInfoProvider.GetWebPartInfo(wi.WidgetWebPartID);
            FormInfo zoneTypeDefinition = PortalFormHelper.GetPositionFormInfo(ZoneType);
            string widgetProperties = FormHelper.MergeFormDefinitions(wpi.WebPartProperties, wi.WidgetProperties);
            FormInfo fi = PortalFormHelper.GetWidgetFormInfo(wi.WidgetName, Enum.GetName(typeof(WidgetZoneTypeEnum), ZoneType), widgetProperties, zoneTypeDefinition, true, wi.WidgetDefaultValues);

            if (fi != null)
            {

                fi.ContextResolver.Settings.RelatedObject = templateInstance;

                // Check if there are some editable properties
                var ffi = fi.GetFields(true, false).ToList<FormFieldInfo>();
                if ((ffi == null) || (ffi.Count == 0))
                {
                    ShowInformation(GetString("widgets.emptyproperties"));
                }

                DataRow dr = fi.GetDataRow();

                // Load overridden values for new widget
                if (IsNewWidget || (xmlVersion > 0))
                {
                    fi.LoadDefaultValues(dr, FormResolveTypeEnum.WidgetVisible);
                }

                if (IsNewWidget)
                {
                    // Override default value and set title as widget display name
                    DataHelper.SetDataRowValue(dr, "WidgetTitle", ResHelper.LocalizeString(wi.WidgetDisplayName));
                }

                // Load values from existing widget
                LoadDataRowFromWidget(dr, fi);

                // Init HTML toolbar if exists                
                InitHTMLToobar(fi);

                // Init the form
                InitForm(formCustom, dr, fi);

                // Set the context name
                formCustom.ControlContext.ContextName = CMS.ExtendedControls.ControlContext.WIDGET_PROPERTIES;
            }
        }

        if (IsInline)
        {
            // Load text definition from session
            string definition = ValidationHelper.GetString(SessionHelper.GetValue("WidgetDefinition"), string.Empty);
            if (String.IsNullOrEmpty(definition))
            {
                definition = Request.Form[hdnWidgetDefinition.UniqueID];
            }
            else
            {
                hdnWidgetDefinition.Value = definition;
            }

            Hashtable parameters = null;
            string widgetName = string.Empty;



            if (IsNewWidget)
            {
                // New widget - load widget info by id
                if (!String.IsNullOrEmpty(WidgetId))
                {
                    wi = WidgetInfoProvider.GetWidgetInfo(ValidationHelper.GetInteger(WidgetId, 0));
                }
                else
                {
                    // Try to get widget from codename
                    widgetName = QueryHelper.GetString("WidgetName", String.Empty);
                    wi = WidgetInfoProvider.GetWidgetInfo(widgetName);
                }
            }
            else
            {
                if (definition == null)
                {
                    DisplayError("widget.failedtoload");
                    return;
                }

                // Parse definition 
                parameters = CMSDialogHelper.GetHashTableFromString(definition);

                // Trim control name
                if (parameters["name"] != null)
                {
                    widgetName = parameters["name"].ToString();
                }

                wi = WidgetInfoProvider.GetWidgetInfo(widgetName);
            }
            if (wi == null)
            {
                DisplayError("widget.failedtoload");
                return;
            }

            // If widget cant be used as inline
            if (!wi.WidgetForInline)
            {
                DisplayError("widget.cantbeusedasinline");
                return;
            }


            // Test permission for user
            var currentUser = MembershipContext.AuthenticatedUser;
            if (!WidgetRoleInfoProvider.IsWidgetAllowed(wi, currentUser.UserID, AuthenticationHelper.IsAuthenticated()))
            {
                isValidWidget = false;
                OnNotAllowed(this, null);
            }

            // If user is editor, more properties are shown
            WidgetZoneTypeEnum zoneType = WidgetZoneTypeEnum.User;
            if (currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Editor, SiteContext.CurrentSiteName))
            {
                zoneType = WidgetZoneTypeEnum.Editor;
            }

            WebPartInfo wpi = WebPartInfoProvider.GetWebPartInfo(wi.WidgetWebPartID);
            string widgetProperties = FormHelper.MergeFormDefinitions(wpi.WebPartProperties, wi.WidgetProperties);
            FormInfo zoneTypeDefinition = PortalFormHelper.GetPositionFormInfo(zoneType);
            FormInfo fi = PortalFormHelper.GetWidgetFormInfo(wi.WidgetName, Enum.GetName(typeof(WidgetZoneTypeEnum), zoneType), widgetProperties, zoneTypeDefinition, true, wi.WidgetDefaultValues);
            if (fi != null)
            {
                // Check if there are some editable properties
                mFields = fi.GetFields(true, true);
                if ((mFields == null) || !mFields.Any())
                {
                    ShowInformation(GetString("widgets.emptyproperties"));
                }

                // Get datarows with required columns
                DataRow dr = PortalHelper.CombineWithDefaultValues(fi, wi);

                if (IsNewWidget)
                {
                    // Load default values for new widget
                    fi.LoadDefaultValues(dr, FormResolveTypeEnum.WidgetVisible);
                }
                else
                {
                    foreach (string key in parameters.Keys)
                    {
                        object value = parameters[key];
                        // Test if given property exists
                        if (dr.Table.Columns.Contains(key) && (value != null))
                        {
                            try
                            {
                                dr[key] = DataHelper.ConvertValue(value, dr.Table.Columns[key].DataType);
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                // Override default value and set title as widget display name
                DataHelper.SetDataRowValue(dr, "WidgetTitle", wi.WidgetDisplayName);

                // Init HTML toolbar if exists
                InitHTMLToobar(fi);

                // Init the form
                InitForm(formCustom, dr, fi);

                // Set the context name
                formCustom.ControlContext.ContextName = CMS.ExtendedControls.ControlContext.WIDGET_PROPERTIES;
            }
        }
    }


    /// <summary>
    /// Saves the given form.
    /// </summary>
    /// <param name="form">Form to save</param>
    private static bool SaveForm(BasicForm form)
    {
        if (form.Visible)
        {
            return form.SaveData("");
        }

        return true;
    }


    /// <summary>
    /// Saves widget properties.
    /// </summary>
    public bool Save()
    {
        if (VariantID > 0)
        {
            // Check MVT/CP security
            if (!CheckPermissions("Manage"))
            {
                DisplayError("general.modifynotallowed");
                return false;
            }
        }

        // Save the data
        if ((CurrentPageInfo != null) && (templateInstance != null) && SaveForm(formCustom))
        {
            ViewModeEnum viewMode = PortalContext.ViewMode;

            // Check manage permission for non-livesite version
            if (!viewMode.IsLiveSite() && (viewMode != ViewModeEnum.DashboardWidgets))
            {
                if (CurrentUser.IsAuthorizedPerDocument(CurrentPageInfo.NodeID, CurrentPageInfo.ClassName, NodePermissionsEnum.Modify) != AuthorizationResultEnum.Allowed)
                {
                    DisplayError("general.modifynotallowed");
                    return false;
                }

                // Check design permissions
                if (PortalContext.IsDesignMode(viewMode, false) && !PortalContext.CurrentUserIsDesigner)
                {
                    RedirectToAccessDenied("CMS.Design", "Design");
                }
            }

            PageTemplateInfo pti = templateInstance.ParentPageTemplate;
            if (PortalContext.IsDesignMode(viewMode) && SynchronizationHelper.IsCheckedOutByOtherUser(pti))
            {
                string userName = null;
                UserInfo ui = UserInfoProvider.GetUserInfo(pti.Generalized.IsCheckedOutByUserID);
                if (ui != null)
                {
                    userName = HTMLHelper.HTMLEncode(ui.GetFormattedUserName(IsLiveSite));
                }

                DisplayError(string.Format(GetString("ObjectEditMenu.CheckedOutByAnotherUser"), pti.TypeInfo.ObjectType, pti.DisplayName, userName));
                return false;
            }

            // Get the zone
            zone = templateInstance.EnsureZone(ZoneId);

            if (zone != null)
            {
                zone.WidgetZoneType = ZoneType;

                // Add new widget
                if (IsNewWidget)
                {
                    bool isLayoutZone = (QueryHelper.GetBoolean("layoutzone", false));
                    int widgetID = ValidationHelper.GetInteger(WidgetId, 0);

                    // Create new widget instance
                    widgetInstance = PortalHelper.AddNewWidget(widgetID, ZoneId, ZoneType, isLayoutZone, templateInstance, null);
                }

                // Ensure handling of the currently edited object (if not exists -> redirect)
                UIContext.EditedObject = widgetInstance;

                widgetInstance.XMLVersion = 1;
                if (IsNewVariant)
                {
                    widgetInstance = widgetInstance.Clone();

                    // Check whether the editor widgets have been already customized
                    if (CurrentPageInfo.DocumentTemplateInstance.WebPartZones.Count == 0)
                    {
                        // There are no customized editor widgets yet => copy the default editor widgets from the page template under the document (to enable customization)

                        // Save to the document as editor admin changes
                        TreeNode node = DocumentHelper.GetDocument(CurrentPageInfo.DocumentID, tree);

                        // Extract and set the document web parts
                        node.SetValue("DocumentWebParts", templateInstance.GetZonesXML(WidgetZoneTypeEnum.Editor));

                        // Save the document
                        DocumentHelper.UpdateDocument(node, tree);
                    }
                }

                bool isLayoutWidget = ((wpi != null) && ((WebPartTypeEnum)wpi.WebPartType == WebPartTypeEnum.Layout));

                // Get basicform's datarow and update widget            
                SaveFormToWidget(formCustom, templateInstance, isLayoutWidget);

                // Ensure unique id for new widget variant or layout widget
                if (IsNewVariant || (isLayoutWidget && IsNewWidget)) 
                {
                    string controlId = GetUniqueWidgetId(wi.WidgetName);

                    if (!string.IsNullOrEmpty(controlId))
                    {
                        widgetInstance.ControlID = controlId;
                    }
                    else
                    {
                        DisplayError("Unable to generate unique widget id.");
                        return false;
                    }
                }

                // Allow set dashboard in design mode
                if ((ZoneType == WidgetZoneTypeEnum.Dashboard) && String.IsNullOrEmpty(PortalContext.DashboardName))
                {
                    viewMode = ViewModeEnum.Design;
                    PortalContext.SetViewMode(ViewModeEnum.Design);
                }

                bool isWidgetVariant = (VariantID > 0) || IsNewVariant;
                if (!isWidgetVariant)
                {
                    // Save the changes
                    if ((viewMode.IsEdit(true) || viewMode.IsEditLive()) && (ZoneType == WidgetZoneTypeEnum.Editor))
                    {
                        if (DocumentManager.AllowSave)
                        {
                            // Store the editor widgets in the temporary interlayer
                            PortalContext.SaveEditorWidgets(CurrentPageInfo.DocumentID, templateInstance.GetZonesXML(WidgetZoneTypeEnum.Editor));
                        }
                    }
                    else
                    {
                        // Save the changes  
                        CMSPortalManager.SaveTemplateChanges(CurrentPageInfo, templateInstance, ZoneType, viewMode, tree);
                    }
                }
                else if ((viewMode.IsEdit()) && (ZoneType == WidgetZoneTypeEnum.Editor))
                {
                    Hashtable properties = WindowHelper.GetItem("variantProperties") as Hashtable;

                    VariantHelper.SaveWebPartVariantChanges(widgetInstance, VariantID, 0, VariantMode, properties);

                    // Clear the document template
                    templateInstance.ParentPageTemplate.ParentPageInfo.DocumentTemplateInstance = null;

                    // Log widget variant synchronization
                    TreeNode node = DocumentHelper.GetDocument(CurrentPageInfo.DocumentID, tree);
                    DocumentSynchronizationHelper.LogDocumentChange(node, TaskTypeEnum.UpdateDocument, tree);
                }
            }

            // Reload the form (because of macro values set only by JS)            
            formCustom.ReloadData();

            // Display info message
            ShowChangesSaved();

            // Clear the cached web part
            if (InstanceGUID != null)
            {
                CacheHelper.TouchKey("webpartinstance|" + InstanceGUID.ToString().ToLowerCSafe());
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Saves the widget data and create string for inline widget.
    /// </summary>
    private string SaveInline()
    {
        // Validate data
        if (!SaveForm(formCustom) ||
            (wi == null) ||
            (mFields == null))
        {
            return String.Empty;
        }

        DataRow dr = formCustom.DataRow;

        string script = PortalHelper.GetAddInlineWidgetScript(wi, dr, mFields);

        if (!string.IsNullOrEmpty(script))
        {
            // Add to recently used widgets collection
            MembershipContext.AuthenticatedUser.UserSettings.UpdateRecentlyUsedWidget(wi.WidgetName);
            return script;
        }

        return string.Empty;
    }


    /// <summary>
    /// Saves the given DataRow data to the web part properties.
    /// </summary>
    /// <param name="form">Form to save</param>
    /// <param name="pti">Page template instance</param>
    /// <param name="isLayoutWidget">Indicates whether the edited widget is a layout widget</param>
    private void SaveFormToWidget(BasicForm form, PageTemplateInstance pti, bool isLayoutWidget)
    {
        if (form.Visible && (widgetInstance != null))
        {
            // Keep the old ID to check the change of the ID
            string oldId = widgetInstance.ControlID.ToLowerCSafe();

            DataRow dr = form.DataRow;

            foreach (DataColumn column in dr.Table.Columns)
            {
                widgetInstance.MacroTable[column.ColumnName.ToLowerCSafe()] = form.MacroTable[column.ColumnName.ToLowerCSafe()];
                widgetInstance.SetValue(column.ColumnName, dr[column]);

                // If name changed, move the content
                // (This can happen when a user overrides the WidgetControlID property to be visible in the widget properties dialog)
                if (CMSString.Compare(column.ColumnName, "widgetcontrolid", true) == 0)
                {
                    try
                    {
                        string newId = ValidationHelper.GetString(dr[column], "").ToLowerCSafe();

                        // Name changed
                        if (!String.IsNullOrEmpty(newId) && (CMSString.Compare(newId, oldId, false) != 0))
                        {
                            WidgetIdChanged = true;
                            WidgetId = newId;

                            // Move the document content if present
                            string currentContent = CurrentPageInfo.EditableWebParts[oldId];
                            if (currentContent != null)
                            {
                                TreeNode node = DocumentHelper.GetDocument(CurrentPageInfo.DocumentID, tree);

                                // Move the content in the page info
                                CurrentPageInfo.EditableWebParts[oldId] = null;
                                CurrentPageInfo.EditableWebParts[newId] = currentContent;

                                // Update the document
                                node.SetValue("DocumentContent", CurrentPageInfo.GetContentXml());
                                DocumentHelper.UpdateDocument(node, tree);
                            }

                            // Change the underlying zone names if layout widget
                            if (isLayoutWidget)
                            {
                                string prefix = oldId + "_";

                                foreach (WebPartZoneInstance zone in pti.WebPartZones)
                                {
                                    if (zone.ZoneID.StartsWithCSafe(prefix, true))
                                    {
                                        // Change the zone prefix to the new one
                                        zone.ZoneID = String.Format("{0}_{1}", newId, zone.ZoneID.Substring(prefix.Length));
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        EventLogProvider.LogException("Content", "CHANGEWIDGET", ex);
                    }
                }
            }
        }
    }


    /// <summary>
    /// Loads the data row data from given web part instance.
    /// </summary>
    /// <param name="dr">DataRow to fill</param>
    private void LoadDataRowFromWidget(DataRow dr, FormInfo fi)
    {
        if (widgetInstance != null)
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
                            load = widgetInstance.Properties.Contains(column.ColumnName.ToLowerCSafe()) || column.ColumnName.EqualsCSafe("webpartcontrolid", true);
                            break;
                        // Version 0
                        default:
                            // Load default value for Boolean type in old XML version
                            if ((column.DataType == typeof(bool)) && !widgetInstance.Properties.Contains(column.ColumnName.ToLowerCSafe()))
                            {
                                FormFieldInfo ffi = fi.GetFormField(column.ColumnName);
                                if (ffi != null)
                                {
                                    widgetInstance.SetValue(column.ColumnName, ffi.GetPropertyValue(FormFieldPropertyEnum.DefaultValue));
                                }
                            }
                            break;
                    }

                    if (load)
                    {
                        object value = widgetInstance.GetValue(column.ColumnName);

                        // Convert value into default format
                        if ((value != null) && (value.ToString() != ""))
                        {
                            if (column.DataType == typeof(decimal))
                            {
                                value = ValidationHelper.GetDouble(value, 0, "en-us");
                            }

                            if (column.DataType == typeof(DateTime))
                            {
                                value = ValidationHelper.GetDateTime(value, DateTime.Now, "en-us");
                            }
                        }

                        DataHelper.SetDataRowValue(dr, column.ColumnName, value);
                    }
                }
                catch
                {
                }
            }
        }
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
    /// Show error label.
    /// </summary>
    /// <param name="error">Error message to show</param>
    private void DisplayError(string error)
    {
        isValidWidget = false;
        ShowError(GetString(error));
    }


    /// <summary>
    /// Initializes the HTML toolbar.
    /// </summary>
    /// <param name="form">Form information</param>
    private void InitHTMLToobar(FormInfo form)
    {
        // Display / hide the HTML editor toolbar area
        if (form.UsesHtmlArea())
        {
            plcToolbar.Visible = true;
        }
    }


    /// <summary>
    /// Gets a unique widget id. Returned widget id is unique among both document widgets (Page tab) and default widgets (Design tab).
    /// </summary>
    /// <param name="baseId">The base id.</param>
    private string GetUniqueWidgetId(string baseId)
    {
        int counter = 0;
        string controlId = null;

        if ((CurrentPageInfo != null)
            && CurrentPageInfo.UsedPageTemplateInfo != null)
        {
            // Get the document widgets template instance
            PageTemplateInstance documentTemplateInstance = zone.ParentTemplateInstance;
            // Get the default widgets template instance
            PageTemplateInstance templateInstance = CurrentPageInfo.UsedPageTemplateInfo.TemplateInstance;

            // Limit the search 
            while (counter < 100)
            {
                // Try to find the first available controlId for both templates
                controlId = WebPartZoneInstance.GetUniqueWebPartId(baseId, documentTemplateInstance, counter);

                if (controlId == WebPartZoneInstance.GetUniqueWebPartId(baseId, templateInstance, counter))
                {
                    break;
                }

                counter++;
            }
        }

        return controlId;
    }

    #endregion
}