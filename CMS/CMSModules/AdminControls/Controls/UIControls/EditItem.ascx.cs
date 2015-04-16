using System;
using System.Linq;

using CMS.Base;
using CMS.UIControls;
using CMS.Helpers;
using CMS.DataEngine;
using CMS.Modules;
using CMS.PortalEngine;
using CMS.FormEngine;
using CMS.FormControls;


public partial class CMSModules_AdminControls_Controls_UIControls_EditItem : CMSAbstractUIWebpart
{
    #region "Variables"

    private bool newItem;
    private bool itemChanged;
    private string mReturnHandler;
    private bool dnChanged;

    #endregion


    #region "Properties"

    /// <summary>
    /// Script to launch, after object is saved
    /// </summary>
    public String AfterSaveScript
    {
        get
        {
            return GetStringContextValue("AfterSaveScript");
        }
        set
        {
            SetValue("AfterSaveScript", value);
        }
    }


    /// <summary>
    /// Returns true if the control processing should be stopped
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            EditForm.StopProcessing = value;
        }
    }


    /// <summary>
    /// Alternative form name.
    /// </summary>
    public String AlternativeFormName
    {
        get
        {
            return GetStringContextValue("AlternativeFormName");
        }
        set
        {
            SetValue("AlternativeFormName", value);
        }
    }


    /// <summary>
    /// Collection of properties (delimited by ';') that refresh parent tab when changed.
    /// </summary>
    public String ParentRefreshProperties
    {
        get
        {
            return GetStringContextValue("ParentRefreshProperties");
        }
        set
        {
            SetValue("ParentRefreshProperties", value);
        }
    }


    /// <summary>
    /// Form layout
    /// </summary>
    public FormLayoutEnum DefaultFormLayout
    {
        get
        {
            return GetStringContextValue("DefaultFormLayout").ToEnum<FormLayoutEnum>();
        }
        set
        {
            SetValue("DefaultFormLayout", value);
        }
    }


    /// <summary>
    /// If true, modify check is made on save action not on load
    /// </summary>
    public bool DelayedModifyCheck
    {
        get
        {
            return UIContext["DelayedModifyCheck"].ToBoolean(false);
        }
    }


    /// <summary>
    /// Default field layout
    /// </summary>
    public FieldLayoutEnum DefaultFieldLayout
    {
        get
        {
            return GetStringContextValue("DefaultFieldLayout", "twocolumns").ToEnum<FieldLayoutEnum>();
        }
        set
        {
            SetValue("DefaultFieldLayout", value);
        }
    }


    /// <summary>
    /// UIForm CSS class.
    /// </summary>
    public override string CssClass
    {
        get
        {
            return GetStringContextValue("CssClass");
        }
        set
        {
            SetValue("CssClass", value);
        }
    }


    /// <summary>
    /// URL to which page is redirected, after object is saved. 
    /// </summary>
    public String RedirectURL
    {
        get
        {
            return GetStringContextValue("RedirectURL", String.Empty, true);
        }
        set
        {
            SetValue("RedirectURL", value);
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets the return handler javascript function.
    /// </summary>
    private string ReturnHandler
    {
        get
        {
            return mReturnHandler ?? (mReturnHandler = QueryHelper.GetString("returnhandler", string.Empty));
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Init event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        if (StopProcessing)
        {
            // No actions if processing is stopped
        }
        else
        {
            EditForm.ContextResolver.SetNamedSourceData("UIContext", UIContext);

            bool saved = ValidationHelper.GetBoolean(UIContext["saved"], false);
            if (saved && !RequestHelper.IsPostBack())
            {
                ShowChangesSaved();
            }
        }

        base.OnInit(e);
    }


    void EditForm_OnAfterDataLoad(object sender, EventArgs e)
    {
        BaseInfo bi = EditForm.EditedObject;

        // Set object ID for new items, set site ID directly, because of check permission. Dont use UIForm's object site ID
        if (newItem && (bi.Generalized.ObjectSiteID <= 0) && (bi.TypeInfo.SiteIDColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN))
        {
            bi.Generalized.ObjectSiteID = GetSiteID(bi);
        }

        // Check view permission
        if (!CheckViewPermissions(bi))
        {
            EditForm.StopProcessing = true;
            EditForm.Visible = false;
            return;
        }

        // Check edit permissions
        if (!(DelayedModifyCheck && newItem) && !CheckEditPermissions(bi))
        {
            EditForm.Enabled = false;
            ShowError(GetString("ui.notauthorizemodified"));
            EditForm.StopProcessing = true;
        }
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            // Raise return handler on the first load when this page is a part of a layout template (i.e. horizontal tabs). Solves redirection issues from "New" to "Edit" pages.
            bool raiseReturnHandler = (IsDialog && !string.IsNullOrEmpty(ReturnHandler) && (UIContext.UIElement != null) && (RootElementID != UIContext.UIElement.ElementID));

            if (QueryHelper.GetBoolean("saved", false) || raiseReturnHandler)
            {
                // Raise javascript "after save" hander
                RaiseReturnHandler();
            }
        }

        if (dnChanged)
        {
            if (UIContext.UIElement != null)
            {
                // Don't call refresh for top dialog element
                bool topDialog = (IsDialog && (UIContext.UIElement.ElementID == UIContext.RootElementID));
                if (!topDialog)
                {
                    // Check for layout in parent tab
                    var ui = UIElementInfoProvider.GetUIElementInfo(UIContext.UIElement.ElementParentID);
                    if (UIContextHelper.ElementIsLayout(ui))
                    {
                        var name = UIContextHelper.GetObjectBreadcrumbsText(UIContext, EditForm.EditedObject);

                        ScriptHelper.RefreshTabHeader(Page, name);
                    }
                }
            }
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Handles the Create event of the EditForm control.
    /// </summary>
    protected void EditForm_Create(object sender, EventArgs ea)
    {
        var form = EditForm;
        
        // Hides save button if set
        form.SubmitButton.Visible = !ValidationHelper.GetBoolean(UIContext["HideSaveButton"], false);

        form.OnAfterDataLoad += EditForm_OnAfterDataLoad;

        form.ObjectType = UIContextHelper.GetObjectType(UIContext);

        if (!String.IsNullOrEmpty(AlternativeFormName))
        {
            form.AlternativeFormName = AlternativeFormName;
        }

        form.DefaultFormLayout = DefaultFormLayout;
        form.CssClass = CssClass;
        form.DefaultFieldLayout = DefaultFieldLayout;

        // Add default category name for field sets and tables design
        if ((DefaultFormLayout == FormLayoutEnum.FieldSets) || (DefaultFormLayout == FormLayoutEnum.TablePerCategory))
        {
            form.DefaultCategoryName = "{$general.general$}";
        }

        // Set new item property
        newItem = (UIContext.EditedObject == null) || ((BaseInfo)UIContext.EditedObject).Generalized.ObjectID <= 0;
    }


    /// <summary>
    /// Handles the OnBeforeSave event of the EditForm control.
    /// </summary>
    protected void EditForm_OnBeforeSave(object sender, EventArgs ea)
    {
        var ctx = UIContext;

        // If edited object is empty, but action is edit and object ID is set -> modifying non existence object -> redirect
        if (((ctx.EditedObject == null) || (((BaseInfo)ctx.EditedObject).Generalized.ObjectID <= 0))
            && (ctx.ObjectID != 0) && ctx["Action"].ToString("") == "edit")
        {
            ctx.EditedObject = null;
        }

        BaseInfo bi = EditForm.EditedObject;

        dnChanged = bi.ItemChanged(bi.Generalized.DisplayNameColumn) || ResHelper.ContainsLocalizationMacro(bi.Generalized.ObjectDisplayName);

        // Modify check
        bool allowed = EditForm.Enabled;
        if (DelayedModifyCheck && newItem)
        {
            allowed = CheckEditPermissions(bi);
        }

        if (!allowed)
        {
            ShowError(GetString("ui.notauthorizemodified"));
            EditForm.StopProcessing = true;
        }

        if (newItem)
        {
            if (!bi.Generalized.CheckLicense(ObjectActionEnum.Insert))
            {
                EditForm.StopProcessing = true;
                ShowError(GetString("licenseversion.createobject"));
                return;
            }

            // If parent column is specified manually, add parent object ID to it (f.e. category hierarchy leveling, parent is not set in TYPEINFO)
            String parentColumn = ValidationHelper.GetString(ctx["parentcolumn"], String.Empty);
            if (parentColumn != String.Empty)
            {
                bi.SetValue(parentColumn, ctx["parentobjectid"]);
            }
        }

        // Refresh header properties check
        foreach (String str in ParentRefreshProperties.Split(';'))
        {
            if (EditForm.EditedObject.ItemChanged(str))
            {
                itemChanged = true;
                break;
            }
        }
    }


    /// <summary>
    /// Handles the OnAfterSave event of the EditForm control.
    /// </summary>
    protected void EditForm_OnAfterSave(object sender, EventArgs ea)
    {
        String saveScript = AfterSaveScript;

        if (saveScript != String.Empty)
        {
            ScriptHelper.RegisterClientScriptBlock(this, typeof(String), "aftersavescript", ScriptHelper.GetScript(saveScript));
        }
        else
        {
            if (!String.IsNullOrEmpty(RedirectURL))
            {
                URLHelper.Redirect(RedirectURL);
            }

            if (newItem)
            {
                String url = RedirectURL;
                if (String.IsNullOrEmpty(RedirectURL))
                {
                    // Find element representing new object
                    UIElementInfo uiPar = UIElementInfoProvider.GetUIElementInfo(UIContext.UIElement.ElementParentID);
                    if (uiPar != null)
                    {
                        // Append parameters
                        url = UIContextHelper.GetElementUrl(uiPar.GetEditElement(), UIContext);
                        url = URLHelper.AppendQuery(url, "objectid=" + EditForm.EditedObject.Generalized.ObjectID + "&saved=1");

                        // Ensure that the redirected edit page will behave as a top dialog page (hide breadcrumbs, uses dialog page title...) 
                        url = URLHelper.RemoveParameterFromUrl(url, "rootelementid");

                        // Append action edit parameter
                        url = URLHelper.AddParameterToUrl(url, "action", "edit");
                    }
                }

                // Ensure hash for dialog mode
                url = UIContextHelper.AppendDialogHash(UIContext, url);

                EditForm.RedirectUrlAfterSave = url;
            }
        }

        // Raise the javascript return handler function
        RaiseReturnHandler();

        if (itemChanged)
        {
            String refreshWopener = String.Empty;

            // Refresh opener for dialog page.
            if (IsDialog)
            {
                refreshWopener = "if(wopener!=null) {if (wopener.Refresh != null) {wopener.Refresh()} else if (wopener.RefreshPage != null) wopener.RefreshPage();}";
            }

            ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "RefreshParent", ScriptHelper.GetScript(refreshWopener + "if (parent.refreshPage) {parent.refreshPage()} else parent.location = parent.location;"));
        }
    }


    /// <summary>
    /// Raises the JavaScript function which is defined in the url.
    /// </summary>
    private void RaiseReturnHandler()
    {
        if (!string.IsNullOrEmpty(ReturnHandler))
        {
            string wopenerReturnType = QueryHelper.GetString("returntype", string.Empty).ToLowerCSafe();

            var bi = EditForm.EditedObject;
            if (bi != null)
            {
                string returnColumnName;

                var ti = bi.TypeInfo;

                // Get the return column name
                switch (wopenerReturnType)
                {
                    case "codename":
                        returnColumnName = ti.CodeNameColumn;
                        break;

                    case "displayname":
                        returnColumnName = ti.DisplayNameColumn;
                        break;

                    case "guid":
                        returnColumnName = ti.GUIDColumn;
                        break;

                    default:
                        returnColumnName = ti.IDColumn;
                        break;
                }

                string returnHandler = ScriptHelper.GetString(ReturnHandler, false);
                string returnValue = ScriptHelper.GetString((bi.GetValue(returnColumnName) ?? string.Empty).ToString());
                string wopenerScript = "if (wopener && wopener." + returnHandler + ") { wopener." + returnHandler + "(" + returnValue + "); }";

                // Empty redirect url after create, redirect manually after update script are launched
                if (!String.IsNullOrEmpty(EditForm.RedirectUrlAfterSave))
                {
                    wopenerScript += "window.location='" + ResolveUrl(EditForm.RedirectUrlAfterSave) + "';";
                    EditForm.RedirectUrlAfterSave = "";
                }

                // Register the handler script to the page
                ScriptHelper.RegisterWOpenerScript(Page);
                ScriptHelper.RegisterClientScriptBlock(Page, GetType(), "UpdateWOpener", wopenerScript, true);
            }
        }
    }

    #endregion
}
