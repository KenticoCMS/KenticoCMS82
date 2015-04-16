using System;

using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_AdminControls_Controls_Class_FieldEditor_CategoryEdit : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets form category info.
    /// </summary>
    public FormCategoryInfo CategoryInfo
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets macroresolver name used in macro editor.
    /// </summary>
    public string ResolverName
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        txtCategoryName.IsLiveSite = IsLiveSite;
        chkCollapsible.IsLiveSite = IsLiveSite;
        chkCollapsedByDefault.IsLiveSite = IsLiveSite;
        chkVisible.IsLiveSite = IsLiveSite;
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        FormEngineUserControl categoryNameControl = (FormEngineUserControl)txtCategoryName.NestedControl;
        if (categoryNameControl != null)
        {
            // Disable autosave on LocalizableTextBox controls
            categoryNameControl.SetValue("AutoSave", false);
        }

        txtCategoryName.ResolverName = ResolverName;
        chkCollapsible.ResolverName = ResolverName;
        chkCollapsedByDefault.ResolverName = ResolverName;
        chkVisible.ResolverName = ResolverName;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        lblCategoryName.AssociatedControlClientID = EditingFormControl.GetInputClientID(txtCategoryName.NestedControl.Controls);
        lblCollapsible.AssociatedControlClientID = EditingFormControl.GetInputClientID(chkCollapsible.NestedControl.Controls);
        lblCollapsedByDefault.AssociatedControlClientID = EditingFormControl.GetInputClientID(chkCollapsedByDefault.NestedControl.Controls);
        lblVisible.AssociatedControlClientID = EditingFormControl.GetInputClientID(chkVisible.NestedControl.Controls);
    }


    /// <summary>
    /// Reloads control.
    /// </summary>
    public void Reload()
    {
        if (CategoryInfo != null)
        {
            bool isMacro;
            txtCategoryName.SetValue(CategoryInfo.GetPropertyValue(FormCategoryPropertyEnum.Caption, out isMacro), isMacro);
            chkCollapsible.SetValue(CategoryInfo.GetPropertyValue(FormCategoryPropertyEnum.Collapsible, out isMacro), isMacro);
            chkCollapsedByDefault.SetValue(CategoryInfo.GetPropertyValue(FormCategoryPropertyEnum.CollapsedByDefault, out isMacro), isMacro);
            chkVisible.SetValue(CategoryInfo.GetPropertyValue(FormCategoryPropertyEnum.Visible, out isMacro), isMacro);
        }
        else
        {
            txtCategoryName.SetValue(null);
            chkCollapsible.SetValue("false");
            chkCollapsedByDefault.SetValue("false");
            chkVisible.SetValue("true");
        }
    }


    /// <summary>
    /// Save the properties to form field info.
    /// </summary>
    /// <returns>True if success</returns>
    public bool Save()
    {
        if (CategoryInfo != null)
        {
            CategoryInfo.SetPropertyValue(FormCategoryPropertyEnum.Caption, ValidationHelper.GetString(txtCategoryName.Value, String.Empty).Replace("'", string.Empty), txtCategoryName.IsMacro);
            CategoryInfo.SetPropertyValue(FormCategoryPropertyEnum.Collapsible, Convert.ToString(chkCollapsible.Value), chkCollapsible.IsMacro);
            CategoryInfo.SetPropertyValue(FormCategoryPropertyEnum.CollapsedByDefault, Convert.ToString(chkCollapsedByDefault.Value), chkCollapsedByDefault.IsMacro);
            CategoryInfo.SetPropertyValue(FormCategoryPropertyEnum.Visible, Convert.ToString(chkVisible.Value), chkVisible.IsMacro);
            return true;
        }
        return false;
    }

    #endregion
}