using System;
using System.Text;
using System.Web.UI.WebControls;
using CMS.Base;
using CMS.Core;
using CMS.ExtendedControls.ActionsConfig;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Modules;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSAdminControls_UI_UIProfiles_UIElementEdit : CMSUserControl
{
    #region "Variables"

    private ResourceInfo mCurrentResourceInfo;
    private UIElementInfo elemInfo = null;
    private bool refreshTree = false;
    private bool isNew = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the current resource info
    /// </summary>
    private ResourceInfo CurrentResourceInfo
    {
        get
        {
            if (mCurrentResourceInfo == null)
            {
                mCurrentResourceInfo = ResourceInfoProvider.GetResourceInfo(ResourceID);
            }
            return mCurrentResourceInfo;
        }
    }


    /// <summary>
    /// Current element id.
    /// </summary>
    public int ElementID
    {
        get;
        set;
    }


    /// <summary>
    /// Current resource id.
    /// </summary>
    public int ResourceID
    {
        get;
        set;
    }


    /// <summary>
    /// Current parent id.
    /// </summary>
    public int ParentID
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Remove javascript prefix from target URL if type is javascript
    /// </summary>
    private void FixJS()
    {
        if (elemInfo != null)
        {
            String value = ValidationHelper.GetString(EditForm.FieldControls["ElementTargetURL"].Value, String.Empty);
            if ((elemInfo.ElementType == UIElementTypeEnum.Javascript) && value.StartsWithCSafe("javascript:", true))
            {
                EditForm.FieldControls["ElementTargetURL"].Value = value.Substring(11);
            }
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        bool displayNone = false;
        bool currentIsCustomizedOfInstalled = false;

        if ((UIContext.EditedObject != null) && ((UIElementInfo)UIContext.EditedObject).ElementID > 0)
        {
            elemInfo = (UIElementInfo)UIContext.EditedObject;
            ParentID = elemInfo.ElementParentID;

            EditForm.FieldControls["ElementPageTemplateID"].SetValue("ItemGuid", elemInfo.ElementGUID);
            EditForm.FieldControls["ElementPageTemplateID"].SetValue("ItemName", elemInfo.ElementDisplayName);

            // Exclude current element and children from dropdown list
            EditForm.FieldControls["ElementParentID"].SetValue("WhereCondition", "ElementIDPath NOT LIKE N'" + elemInfo.ElementIDPath + "%'");

            // Enable editing only for current module. Disable for root
            EditForm.Enabled = ((!UIElementInfoProvider.AllowEditOnlyCurrentModule || (ResourceID == elemInfo.ElementResourceID) && elemInfo.ElementIsCustom)
                               && (elemInfo.ElementParentID != 0));

            // Allow global application checkbox only for applications
            if (!elemInfo.IsApplication && !elemInfo.ElementIsGlobalApplication)
            {
                EditForm.FieldsToHide.Add("ElementIsGlobalApplication");
            }

            // Show info for customized elements
            ResourceInfo ri = ResourceInfoProvider.GetResourceInfo(elemInfo.ElementResourceID);
            if (ri != null)
            {
                if (elemInfo.ElementIsCustom && !ri.ResourceIsInDevelopment)
                {
                    currentIsCustomizedOfInstalled = true;
                    ShowInformation(GetString("module.customeleminfo"));
                }
            }
        }
        // New item
        else
        {
            isNew = true;
            if (!RequestHelper.IsPostBack())
            {
                EditForm.FieldControls["ElementFromVersion"].Value = CMSVersion.GetVersion(true, true, false, false);
            }

            // Predefine current resource if is in development
            ResourceInfo ri = CurrentResourceInfo;
            if ((ri != null) && ri.ResourceIsInDevelopment)
            {
                EditForm.FieldControls["ElementResourceID"].Value = ResourceID;
            }
            // Display none if is under not-development resource
            else
            {
                displayNone = true;
            }

            EditForm.FieldControls["ElementParentID"].Value = ParentID;
            EditForm.FieldsToHide.Add("ElementParentID");

            var parent = UIElementInfoProvider.GetUIElementInfo(ParentID);
            if ((parent == null) || (parent.ElementLevel != 2) || !parent.IsInAdministrationScope)
            {
                EditForm.FieldsToHide.Add("ElementIsGlobalApplication");
            }
        }

        if (!SystemContext.DevelopmentMode)
        {
            EditForm.FieldsToHide.Add("ElementFromVersion");
        }

        EditForm.OnAfterSave += EditForm_OnAfterSave;
        EditForm.OnBeforeSave += EditForm_OnBeforeSave;
        EditForm.OnItemValidation += EditForm_OnItemValidation;

        // Allow only modules in development mode
        FormEngineUserControl resourceCtrl = EditForm.FieldControls["ElementResourceID"];
        if (resourceCtrl != null)
        {
            resourceCtrl.SetValue("DisplayOnlyModulesInDevelopmentMode", true);
        }

        // Disable form and add customize button if element is not custom and not development mode
        if (!SystemContext.DevelopmentMode && (elemInfo != null) && (ResourceID == elemInfo.ElementResourceID) && !elemInfo.ElementIsCustom && (elemInfo.ElementParentID != 0))
        {
            ICMSMasterPage master = Page.Master as ICMSMasterPage;
            if (master != null)
            {
                master.HeaderActions.AddAction(new HeaderAction
                {
                    Text = GetString("general.customize"),
                    CommandName = "customize",
                    OnClientClick = "if (!confirm("+ ScriptHelper.GetString(ResHelper.GetString("module.customizeconfirm"))+")) { return false; }"
                });

                master.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
            }

            EditForm.Enabled = false;
        }

        if (resourceCtrl != null)
        {
            // Display all modules in disabled UI
            if (!EditForm.Enabled)
            {
                resourceCtrl.SetValue("DisplayOnlyModulesInDevelopmentMode", false);
            }
            // Display all modules in customized element but do not allow change the module
            else if (currentIsCustomizedOfInstalled)
            {
                resourceCtrl.SetValue("DisplayOnlyModulesInDevelopmentMode", false);
                resourceCtrl.Enabled = false;
            }

            // Display none if needed
            resourceCtrl.SetValue("DisplayNone", displayNone);
        }
    }


    /// <summary>
    /// Checks whether module is selected and is not in development mode
    /// </summary>
    void EditForm_OnItemValidation(object sender, ref string errorMessage)
    {
        FormEngineUserControl ctrl = sender as FormEngineUserControl;

        if ((ctrl != null) && (ctrl.FieldInfo.Name == "ElementResourceID"))
        {
            int resourceId = ValidationHelper.GetInteger(ctrl.Value, 0);
            if (resourceId > 0)
            {
                ResourceInfo ri = ResourceInfoProvider.GetResourceInfo(resourceId);
                if ((ri != null) && (ri.ResourceIsInDevelopment || ((elemInfo != null) && elemInfo.ElementIsCustom)))
                {
                    return;
                }
            }
            errorMessage = ResHelper.GetString("module.noneordevelopedmoduleselected");
        }
    }


    void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        if (e.CommandName == "customize")
        {
            if (elemInfo != null)
            {
                // Allow editing, set is custom flag, hide customize button
                elemInfo.ElementIsCustom = true;
                elemInfo.Update();

                // Redirect to set all properties right (may be resolved in OnInit)
                URLHelper.Redirect(RequestContext.CurrentURL);
            }
        }
    }


    void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        elemInfo = (UIElementInfo)EditForm.EditedObject;

        // Clear icon field based on icon type
        int type = EditForm.FieldControls["ElementIconType"].Value.ToInteger(0);
        if (type == 0)
        {
            elemInfo.ElementIconClass = "";
        }
        else
        {
            elemInfo.ElementIconPath = "";
        }

        // Check unique code name
        UIElementInfo testUI = UIElementInfoProvider.GetUIElementInfo(elemInfo.ElementResourceID, elemInfo.ElementName);
        if ((testUI != null) && (testUI.ElementID != elemInfo.ElementID))
        {
            ShowError(GetString("ui.element.alreadyexists"));
            EditForm.StopProcessing = true;
            return;
        }

        UIElementInfo oldItem = UIContext.EditedObject as UIElementInfo;

        // If new element or display name has changed or parent changed, refresh tree and recalculate order
        if ((oldItem != null) && ((oldItem.ElementParentID != elemInfo.ElementParentID) || (oldItem.ElementDisplayName != elemInfo.ElementDisplayName)) || (ElementID == 0))
        {
            // If element is new or changed parent, put him in the end of the order, otherwise it stays on it's place
            if ((ElementID == 0) || (oldItem.ElementParentID != elemInfo.ElementParentID))
            {
                elemInfo.ElementOrder = UIElementInfoProvider.GetLastElementOrder(elemInfo.ElementParentID) + 1;
            }
            refreshTree = true;
        }

        // Clear target URL for page template type
        if (elemInfo.ElementType == UIElementTypeEnum.PageTemplate)
        {
            elemInfo.ElementTargetURL = null;
        }
        else
        {
            // Empty Page Template ID for non page template type
            elemInfo.ElementPageTemplateID = 0;
        }

        // Add javascript prefix for TargetURL if javascript type is selected
        if ((elemInfo.ElementType == UIElementTypeEnum.Javascript) && !elemInfo.ElementTargetURL.StartsWithCSafe("javascript", true))
        {
            elemInfo.ElementTargetURL = "javascript:" + elemInfo.ElementTargetURL;
        }

        if (isNew)
        {
            elemInfo.ElementIsCustom = !SystemContext.DevelopmentMode;
        }

        // For new elements or when template is changed, create new element's properties based on default values
        bool templateChanged = ValidationHelper.GetBoolean(EditForm.FieldControls["ElementPageTemplateID"].GetValue("TemplateChanged"), false);
        if (isNew || templateChanged)
        {
            // Get page template if any template is selected
            PageTemplateInfo pti = (elemInfo.ElementPageTemplateID > 0) ? PageTemplateInfoProvider.GetPageTemplateInfo(elemInfo.ElementPageTemplateID) : null;

            // Create form info based on either template combined with default general data (XML file) or default general data only (if no template is selected)
            FormInfo fi = (pti != null) ? pti.PageTemplatePropertiesForm : PortalFormHelper.GetUIElementDefaultPropertiesForm(UIElementPropertiesPosition.Both);

            // Create XMLData collection for current element (this make sense only for template change). New elements obviously have no properties.
            XmlData data = new XmlData();
            data.LoadData(elemInfo.ElementProperties);

            // Apply default data do element's properties, but only it has not it's own data (based by column name)
            foreach (FormFieldInfo ffi in fi.GetFields(true, true))
            {
                if (!data.ColumnNames.Contains(ffi.Name) && !String.IsNullOrEmpty(ffi.DefaultValue))
                {
                    data[ffi.Name] = ffi.DefaultValue;
                }
            }

            elemInfo.ElementProperties = data.GetData();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Page template indicates new template was created, save the form
        if (ValidationHelper.GetBoolean(EditForm.FieldControls["ElementPageTemplateID"].GetValue("Save"), false))
        {
            // If not valid data store at least page template directly, cloned template must be assigned.
            if (EditForm.ValidateData())
            {
                EditForm.SaveData(String.Empty);
                ShowChangesSaved();
            }
            else
            {
                elemInfo.ElementPageTemplateID = ValidationHelper.GetInteger(EditForm.FieldControls["ElementPageTemplateID"].Value, 0);
                elemInfo.Update();
            }
        }

        FixJS();

        base.OnPreRender(e);
    }


    private void RefreshTree()
    {
        // Reload header and content after save
        var sb = new StringBuilder();

        sb.Append(
@"
var p = window.parent;
if (p != null) 
{
");

        if (ElementID == 0)
        {
            var elementUrl = URLHelper.AppendQuery(UIContextHelper.GetElementUrl(ModuleName.CMS, "Modules.UserInterface.Edit", false, elemInfo.ElementID), "elementId=" + elemInfo.ElementID + "&moduleid=" + ResourceID + "&objectParentId=" + ResourceID + "&saved=1");

            sb.AppendFormat(
@"
    var elUrl = '{0}';
    var fr = p.parent.frames['uicontent'];
    if (fr) {{
        fr.location = elUrl;
    }}
    fr = p.frames['uicontent'];
    if (fr) {{
        fr.location = elUrl;
    }}
",
            elementUrl
            );
        }

        var treeUrl = ResolveUrl("~/CMSModules/Modules/Pages/Module/UserInterface/Tree.aspx") + "?moduleID=" + ResourceID + "&elementId=" + elemInfo.ElementID;

        sb.AppendFormat(
@"
    var tUrl = '{0}';
    fr = p.parent.frames['tree'];
    if (fr) {{
        fr.location = tUrl;
    }}
    fr = p.frames['tree'];
    if (fr) {{
        fr.location = tUrl;
    }}
}}",
        treeUrl
        );

        ltlScript.Text = ScriptHelper.GetScript(sb.ToString());
    }


    void EditForm_OnAfterSave(object sender, EventArgs e)
    {
        elemInfo = (UIElementInfo)EditForm.EditedObject;

        if (refreshTree)
        {
            RefreshTree();
        }

        // When template was changed, delete all adhoc templates assigned to current UI element (if adhoc template is not assigned).
        bool templateChanged = ValidationHelper.GetBoolean(EditForm.FieldControls["ElementPageTemplateID"].GetValue("TemplateChanged"), false);
        if (templateChanged)
        {
            bool delete = true;
            if (elemInfo.ElementType == UIElementTypeEnum.PageTemplate)
            {
                PageTemplateInfo pti = PageTemplateInfoProvider.GetPageTemplateInfo(elemInfo.ElementPageTemplateID);

                // Ad hoc template is assigned, do not delete
                if ((pti != null) && !pti.IsReusable)
                {
                    delete = false;
                }
            }

            if (delete)
            {
                PageTemplateInfoProvider.DeleteAdHocTemplates(elemInfo.ElementGUID);
            }
        }
    }

    #endregion
}