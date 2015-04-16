using System;
using System.Web;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Collections;

using CMS.Core;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Base;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.Taxonomy;
using CMS.UIControls;

public partial class CMSModules_Objects_Controls_CloneObject : CMSUserControl
{
    #region "Variables"

    private string mCloseScript = null;
    private CloneSettingsControl customProperties = null;
    private ObjectTypeInfo typeInfo = null;

    private List<string> excludedChildren = new List<string>();
    private List<string> excludedBindings = new List<string>();
    private List<string> excludedOtherBindings = new List<string>();

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns script which should be run when cloning is successfully finished.
    /// </summary>
    public string CloseScript
    {
        get
        {
            if (!string.IsNullOrEmpty(mCloseScript))
            {
                return mCloseScript;
            }
            return "RefreshContent(); CloseDialog();";
        }
    }


    /// <summary>
    /// Gets or sets BaseInfo object to be clonned.
    /// </summary>
    public BaseInfo InfoToClone
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if user chosed to use transaction to clone object.
    /// </summary>
    public bool UseTransaction
    {
        get
        {
            return chkUseTransaction.Checked;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (InfoToClone != null)
        {
            ScriptHelper.RegisterJQuery(this.Page);

            typeInfo = InfoToClone.TypeInfo;

            siteElem.AllowGlobal = typeInfo.SupportsGlobalObjects;

            SetLabel(lblDisplayName, "displaynamelabel", "clonning.newdisplayname");
            SetLabel(lblCodeName, "codenamelabel", "clonning.newcodename");

            lblKeepFieldsTranslated.ToolTip = GetString("clonning.settings.keepfieldstranslated.tooltip");
            lblCloneUnderSite.ToolTip = GetString("clonning.settings.cloneundersite.tooltip");
            lblMetafiles.ToolTip = GetString("clonning.settings.metafiles.tooltip");
            lblMaxRelativeLevel.ToolTip = GetString("clonning.settings.maxrelativelevel.tooltip");


            plcCodeName.Visible = (typeInfo.CodeNameColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN);
            plcDisplayName.Visible = (typeInfo.DisplayNameColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN) && !typeInfo.DisplayNameColumn.EqualsCSafe(typeInfo.CodeNameColumn, true);

            // Try to load Custom properties
            customProperties = LoadCustomProperties(typeInfo.ObjectType);
            if ((customProperties == null) && (typeInfo.ObjectType != typeInfo.OriginalObjectType))
            {
                // Try get original object type settings control
                customProperties = LoadCustomProperties(typeInfo.OriginalObjectType);
            }

            if (customProperties != null)
            {
                headCustom.Text = GetCustomParametersTitle();
                customProperties.ID = "customProperties";
                customProperties.InfoToClone = InfoToClone;

                plcCustomParameters.Controls.Add(customProperties);
                plcCustomParametersBox.Visible = customProperties.DisplayControl;

                if (customProperties.HideDisplayName)
                {
                    plcDisplayName.Visible = false;
                }
                if (customProperties.HideCodeName)
                {
                    plcCodeName.Visible = false;
                }

                if (!RequestHelper.IsPostBack())
                {
                    TransferExcludedTypes();
                }
            }

            // Show site DDL only for Global Admin and for controls which have SiteID (and are not under group or any other parent) and are not from E-Commerce/Forums module
            int sitesCount = SiteInfoProvider.GetSitesCount();
            plcCloneUnderSite.Visible = typeInfo.SupportsCloneToOtherSite
                && (typeInfo.SiteIDColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN)
                && (MembershipContext.AuthenticatedUser != null)
                && (MembershipContext.AuthenticatedUser.IsGlobalAdministrator)
                && ((typeInfo.SupportsGlobalObjects && (sitesCount > 0)) || (sitesCount > 1))
                && (InfoToClone.Generalized.ObjectGroupID == 0)
                && (InfoToClone.Generalized.ObjectParentID == 0)
                && !typeInfo.ModuleName.EqualsCSafe(ModuleName.ECOMMERCE, true)
                && !typeInfo.ModuleName.EqualsCSafe(ModuleName.FORUMS, true)
                && (typeInfo.OriginalObjectType != CategoryInfo.OBJECT_TYPE);

            if (((typeInfo.BindingObjectTypes != null) && (typeInfo.BindingObjectTypes.Count > 0)) || ((typeInfo.OtherBindingObjectTypes != null) && (typeInfo.OtherBindingObjectTypes.Count > 0)))
            {
                // Remove site binding from bindings if exists
                List<string> bindings = new List<string>();
                if (typeInfo.BindingObjectTypes != null)
                {
                    bindings.AddRange(typeInfo.BindingObjectTypes);
                }
                if (typeInfo.OtherBindingObjectTypes != null)
                {
                    bindings.AddRange(typeInfo.OtherBindingObjectTypes);
                }
                if (!string.IsNullOrEmpty(typeInfo.SiteBinding))
                {
                    if (bindings.Contains(typeInfo.SiteBinding))
                    {
                        bindings.Remove(typeInfo.SiteBinding);
                    }
                }
                if (bindings.Count > 0)
                {
                    List<string> excludedTypes = new List<string>();
                    excludedTypes.AddRange(excludedBindings);
                    excludedTypes.AddRange(excludedOtherBindings);

                    int itemNumber = 0;
                    lblBindings.ToolTip = GetCloneHelpText(bindings, excludedTypes, out itemNumber);

                    if (itemNumber == 1)
                    {
                        lblBindings.Text = lblBindings.ToolTip;
                        lblBindings.ToolTip = "";
                    }
                    else
                    {
                        SetLabel(lblBindings, "bindingslabel", "clonning.settings.bindings");
                    }

                    plcBindings.Visible = itemNumber > 0;
                }
            }

            if ((typeInfo.ChildObjectTypes != null) && (typeInfo.ChildObjectTypes.Count > 0))
            {
                int itemNumber = 0;
                lblChildren.ToolTip = GetCloneHelpText(typeInfo.ChildObjectTypes, excludedChildren, out itemNumber);

                if (itemNumber == 1)
                {
                    lblChildren.Text = lblChildren.ToolTip;
                    lblChildren.ToolTip = "";
                }
                else
                {
                    lblChildren.Text = GetString("clonning.settings.children");
                }

                plcChildren.Visible = itemNumber > 0;
                plcChildrenLevel.Visible = ShowChildrenLevel(excludedChildren);
            }

            if (!string.IsNullOrEmpty(typeInfo.SiteBinding) && (InfoToClone.Generalized.ObjectGroupID == 0))
            {
                // For objects with SiteID column allow site bindings only for global versions of the object (for example polls)
                if ((typeInfo.SiteIDColumn == ObjectTypeInfo.COLUMN_NAME_UNKNOWN) || (InfoToClone.Generalized.ObjectSiteID == 0))
                {
                    lblAssignToCurrentSite.ToolTip = GetString("clonning.settings.assigntocurrentsite.tooltip");
                    plcAssignToCurrentSite.Visible = true;

                    lblSiteBindings.ToolTip = GetCloneHelpText(new List<string>() { typeInfo.SiteBinding });

                    plcSiteBindings.Visible = true;
                }
            }

            if ((InfoToClone.MetaFiles != null) && (InfoToClone.MetaFiles.Count > 0))
            {
                plcMetafiles.Visible = true;
            }

            // Preselect site of the object as a "clone under site" option
            if (plcCloneUnderSite.Visible && !RequestHelper.IsPostBack())
            {
                siteElem.SiteName = InfoToClone.Generalized.ObjectSiteName;
            }

            if (!RequestHelper.IsPostBack())
            {
                if (plcCodeName.Visible)
                {
                    txtCodeName.Text = InfoToClone.Generalized.GetUniqueCodeName();
                }
                if (plcDisplayName.Visible)
                {
                    txtDisplayName.Text = InfoToClone.Generalized.GetUniqueDisplayName();
                }

                // Exception for cultures for assigning to current site (for cultures the default value should be false)
                if (typeInfo.ObjectType == CultureInfo.OBJECT_TYPE)
                {
                    chkAssignToCurrentSite.Checked = false;
                }
            }

            if (plcChildren.Visible)
            {
                LoadMaxRelativeLevel();
            }
        }
    }

    /// <summary>
    /// Loads custom object type properties control
    /// </summary>
    /// <param name="objectType">Object type of current cloned object</param>
    private CloneSettingsControl LoadCustomProperties(string objectType)
    {
        string fileName = TranslationHelper.GetSafeClassName(objectType) + "Settings.ascx";
        string generalControlFile = "~/CMSModules/Objects/FormControls/Cloning/" + fileName;
        string moduleControlFile = ((typeInfo.ModuleInfo == null) || string.IsNullOrEmpty(typeInfo.ModuleInfo.ModuleRootPath) ? generalControlFile : typeInfo.ModuleInfo.ModuleRootPath.TrimEnd('/') + "/FormControls/Cloning/" + fileName);

        if (customProperties == null)
        {
            try
            {
                customProperties = this.LoadUserControl(moduleControlFile) as CloneSettingsControl;
            }
            catch { }
        }

        if (customProperties == null)
        {
            try
            {
                customProperties = this.LoadUserControl(generalControlFile) as CloneSettingsControl;
            }
            catch { }
        }

        return customProperties;
    }


    private void SetLabel(LocalizedLabel label, string suffix, string defaultString)
    {
        string stringPrefixName = "cloning.settings." + TranslationHelper.GetSafeClassName(typeInfo.ObjectType) + ".";
        string newString = stringPrefixName + suffix;

        if (GetString(newString) != newString)
        {
            label.ResourceString = newString;
        }
        else
        {
            label.ResourceString = defaultString;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Determines whether the children objects have their own children
    /// </summary>
    /// <param name="excludedTypes">Excluded child types</param>
    public bool ShowChildrenLevel(List<string> excludedTypes)
    {
        ObjectTypeInfo typeInfo = InfoToClone.TypeInfo;
        if (typeInfo.ChildObjectTypes == null)
        {
            return false;
        }

        string[] objTypes = typeInfo.ChildObjectTypes.ToArray();
        for (int i = 0; i < objTypes.Length; i++)
        {
            bool allowed = (excludedTypes == null) || !excludedTypes.Contains(objTypes[i]);
            if (allowed)
            {
                ObjectTypeInfo typeInfoChild = ModuleManager.GetReadOnlyObject(objTypes[i]).TypeInfo;
                if ((typeInfoChild.ChildObjectTypes != null) && (typeInfoChild.ChildObjectTypes.Count > 0))
                {
                    return true;
                }
            }
        }

        return false;
    }


    /// <summary>
    /// Indicates if any setting is relevant (and therefore visible) for the given object.
    /// </summary>
    public bool HasNoSettings()
    {
        return !(plcMetafiles.Visible || plcCloneUnderSite.Visible || plcCodeName.Visible || plcCustomParameters.Visible || plcDisplayName.Visible || plcChildren.Visible || plcSiteBindings.Visible);
    }


    /// <summary>
    /// Creates tooltip for given list of object types.
    /// </summary>
    /// <param name="objectTypes">Object types list</param>
    private string GetCloneHelpText(List<string> objectTypes)
    {
        int itemNumber = 0;
        return GetCloneHelpText(objectTypes, null, out itemNumber);
    }


    /// <summary>
    /// Creates tooltip for given list of object types.
    /// </summary>
    /// <param name="objTypes">Object types list</param>
    /// <param name="excludedTypes">Object types which whould be excluded</param>
    /// <param name="itemNumber">Number of items</param>
    private string GetCloneHelpText(List<string> objTypes, List<string> excludedTypes, out int itemNumber)
    {
        List<string> types = new List<string>();
        for (int i = 0; i < objTypes.Count; i++)
        {
            bool allowed = (excludedTypes == null) || !excludedTypes.Contains(objTypes[i]);
            if (allowed)
            {
                types.Add(GetString("objecttype." + TranslationHelper.GetSafeClassName(objTypes[i])));
            }
        }
        itemNumber = types.Count;
        if (itemNumber == 1)
        {
            string baseName = types[0];
            if (baseName.Length > 2)
            {
                if (Char.IsUpper(baseName[0]) && !Char.IsUpper(baseName[1]))
                {
                    baseName = Char.ToLowerInvariant(baseName[0]) + baseName.Substring(1);
                }
            }
            return string.Format(GetString("clonning.settings.oneitemhelp"), baseName.Trim());
        }
        else
        {
            return string.Format(GetString("clonning.settings.tooltiphelp"), string.Join(", ", types.ToArray()));
        }
    }


    /// <summary>
    /// Load dropdown with MaxRelativeLevel.
    /// </summary>
    private void LoadMaxRelativeLevel()
    {
        if (drpMaxRelativeLevel.Items.Count == 0)
        {
            drpMaxRelativeLevel.Items.Add(new ListItem(GetString("clonning.settings.level.all"), "-1"));
            drpMaxRelativeLevel.Items.Add(new ListItem(GetString("clonning.settings.level.1"), "1"));
            drpMaxRelativeLevel.Items.Add(new ListItem(GetString("clonning.settings.level.2"), "2"));
            drpMaxRelativeLevel.Items.Add(new ListItem(GetString("clonning.settings.level.3"), "3"));
        }
    }


    /// <summary>
    /// Clones the object to the DB according to provided settings.
    /// </summary>
    public CloneResult CloneObject()
    {
        if (InfoToClone != null)
        {
            TransferExcludedTypes();

            // Check code name
            if (plcCodeName.Visible)
            {
                bool checkCodeName = true;
                if (customProperties != null)
                {
                    checkCodeName = customProperties.ValidateCodeName;
                }

                if (checkCodeName && !ValidationHelper.IsCodeName(txtCodeName.Text))
                {
                    ShowError(GetString("general.invalidcodename"));
                    return null;
                }
            }

            // Check permissions
            string targetSiteName = SiteContext.CurrentSiteName;
            if (plcCloneUnderSite.Visible && siteElem.Visible)
            {
                int targetSiteId = siteElem.SiteID;
                if (targetSiteId > 0)
                {
                    targetSiteName = SiteInfoProvider.GetSiteName(targetSiteId);
                }
            }

            // Check object permissions (Create & Modify)
            try
            {
                InfoToClone.CheckPermissions(PermissionsEnum.Create, targetSiteName, CurrentUser, true);
                InfoToClone.CheckPermissions(PermissionsEnum.Modify, targetSiteName, CurrentUser, true);
            }
            catch (PermissionCheckException ex)
            {
                RedirectToAccessDenied(ex.ModuleName, ex.PermissionFailed);
            }

            CloneSettings settings = new CloneSettings();
            settings.KeepFieldsTranslated = chkKeepFieldsTranslated.Checked;
            settings.CloneBase = InfoToClone;
            settings.CodeName = txtCodeName.Text;
            settings.DisplayName = txtDisplayName.Text;
            settings.IncludeBindings = chkBindings.Checked;
            settings.IncludeOtherBindings = chkBindings.Checked;
            settings.IncludeChildren = chkChildren.Checked;
            settings.IncludeMetafiles = chkMetafiles.Checked;
            settings.IncludeSiteBindings = chkSiteBindings.Checked;
            if (plcAssignToCurrentSite.Visible)
            {
                settings.AssignToSiteID = (chkAssignToCurrentSite.Checked ? SiteContext.CurrentSiteID : 0);
            }
            settings.MaxRelativeLevel = ValidationHelper.GetInteger(drpMaxRelativeLevel.SelectedValue, -1);
            if (plcCloneUnderSite.Visible && siteElem.Visible)
            {
                settings.CloneToSiteID = siteElem.SiteID;
            }
            else
            {
                settings.CloneToSiteID = InfoToClone.Generalized.ObjectSiteID;
            }
            if (customProperties != null)
            {
                if (customProperties.IsValid(settings))
                {
                    Hashtable p = customProperties.CustomParameters;
                    if (p != null)
                    {
                        settings.CustomParameters = p;
                    }

                    settings.ExcludedChildTypes.AddRange(excludedChildren);
                    settings.ExcludedBindingTypes.AddRange(excludedBindings);
                    settings.ExcludedOtherBindingTypes.AddRange(excludedOtherBindings);
                }
                else
                {
                    return null;
                }
            }
            if (InfoToClone.Parent != null)
            {
                settings.ParentID = InfoToClone.Parent.Generalized.ObjectID;
            }

            CloneResult result = new CloneResult();
            BaseInfo clone = null;

            if (chkUseTransaction.Checked)
            {
                using (var transaction = new CMSTransactionScope())
                {
                    clone = InfoToClone.Generalized.InsertAsClone(settings, result);
                    transaction.Commit();
                }
            }
            else
            {
                clone = InfoToClone.Generalized.InsertAsClone(settings, result);
            }

            if (customProperties != null)
            {
                string script = customProperties.CloseScript;
                if (!string.IsNullOrEmpty(script))
                {
                    mCloseScript = script.Replace("{0}", clone.Generalized.ObjectID.ToString());
                }
            }

            return result;
        }
        return null;
    }


    private void TransferExcludedTypes()
    {
        if (customProperties != null)
        {
            string children = customProperties.ExcludedChildTypes;
            string bindings = customProperties.ExcludedBindingTypes;
            string otherBindings = customProperties.ExcludedOtherBindingTypes;
            char[] sep = new char[] { ';' };
            if (!string.IsNullOrEmpty(children))
            {
                excludedChildren = new List<string>(children.Split(sep, StringSplitOptions.None));
            }
            if (!string.IsNullOrEmpty(bindings))
            {
                excludedBindings = new List<string>(bindings.Split(sep, StringSplitOptions.None));
            }
            if (!string.IsNullOrEmpty(otherBindings))
            {
                excludedOtherBindings = new List<string>(otherBindings.Split(sep, StringSplitOptions.None));
            }
        }
    }


    protected string GetCustomParametersTitle()
    {
        if (InfoToClone != null)
        {
            return string.Format(GetString("clonning.settings.customparameters"), GetString("objecttype." + TranslationHelper.GetSafeClassName(InfoToClone.TypeInfo.ObjectType)));
        }

        return "";
    }

    #endregion
}