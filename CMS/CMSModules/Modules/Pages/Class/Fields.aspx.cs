using System;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.ExtendedControls.ActionsConfig;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;

[UIElement(ModuleName.CMS, "Fields")]
public partial class CMSModules_Modules_Pages_Class_Fields : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SetupControls();

        if (!URLHelper.IsPostback() && QueryHelper.GetBoolean("gen", false))
        {
            fieldEditor.ShowInformation(GetString("EditTemplateFields.FormDefinitionGenerated"));
        }

        ScriptHelper.HideVerticalTabs(this);
    }


    /// <summary>
    /// Initializes the controls.
    /// </summary>
    private void SetupControls()
    {
        // Get info on the class
        DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(QueryHelper.GetInteger("classid", 0));
        if (dci != null)
        {
            if (dci.ClassIsDocumentType && !dci.ClassIsCoupledClass)
            {
                ShowError(GetString("EditTemplateFields.ErrorIsNotCoupled"));
            }
            else
            {
                fieldEditor.Visible = true;
                fieldEditor.ClassName = dci.ClassName;

                ResourceInfo resource = ResourceInfoProvider.GetResourceInfo(QueryHelper.GetInteger("moduleid", 0));
                bool devMode = SystemContext.DevelopmentMode;
                bool isEditable = ((resource != null) && resource.IsEditable) || dci.ClassShowAsSystemTable || devMode;

                // Allow development mode only for non-system tables
                fieldEditor.DevelopmentMode = !dci.ClassShowAsSystemTable || devMode;
                fieldEditor.Enabled = isEditable;
                fieldEditor.Mode = dci.ClassShowAsSystemTable ? FieldEditorModeEnum.SystemTable : FieldEditorModeEnum.ClassFormDefinition;

                if (devMode)
                {
                    // Add header action for generating default form definition
                    fieldEditor.HeaderActions.AddAction(new HeaderAction()
                    {
                        Text = GetString("EditTemplateFields.GenerateFormDefinition"),
                        Tooltip = GetString("EditTemplateFields.GenerateFormDefinition"),
                        OnClientClick = "if (!confirm('" + GetString("EditTemplateFields.GenerateFormDefConfirmation") + "')) {{ return false; }}",
                        Visible = !dci.ClassIsDocumentType,
                        CommandName = "gendefinition",
                        Enabled = isEditable
                    });

                    fieldEditor.HeaderActions.ActionPerformed += (s, ea) => { if (ea.CommandName == "gendefinition") GenerateDefinition(); };
                }
            }
        }
    }


    /// <summary>
    /// Generates default form definition.
    /// </summary>
    private void GenerateDefinition()
    {
        // Get info on the class
        DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(QueryHelper.GetInteger("classid", 0));
        if (dci != null)
        {
            TableManager tm = new TableManager(dci.ClassConnectionString);

            // Get the XML schema
            dci.ClassXmlSchema = tm.GetXmlSchema(dci.ClassTableName);
            dci.ClassFormDefinition = FormHelper.GetXmlFormDefinitionFromXmlSchema(dci.ClassXmlSchema, true);
            DataClassInfoProvider.SetDataClassInfo(dci);

            URLHelper.Redirect(URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "gen", "1"));
        }
    }
}