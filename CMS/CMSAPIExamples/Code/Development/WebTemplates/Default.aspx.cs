using System;
using System.Data;

using CMS.CMSImportExport;
using CMS.Helpers;
using CMS.ImportExport;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSAPIExamples_Code_Development_WebTemplates_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Web template
        apiCreateWebTemplate.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateWebTemplate);
        apiGetAndUpdateWebTemplate.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateWebTemplate);
        apiGetAndMoveWebTemplateDown.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndMoveWebTemplateDown);
        apiGetAndMoveWebTemplateUp.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndMoveWebTemplateUp);
        apiGetAndBulkUpdateWebTemplates.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateWebTemplates);
        apiDeleteWebTemplate.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteWebTemplate);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Web template
        apiCreateWebTemplate.Run();
        apiGetAndUpdateWebTemplate.Run();
        apiGetAndMoveWebTemplateUp.Run();
        apiGetAndMoveWebTemplateDown.Run();
        apiGetAndBulkUpdateWebTemplates.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Web template
        apiDeleteWebTemplate.Run();
    }

    #endregion


    #region "API examples - Web template"

    /// <summary>
    /// Creates web template. Called when the "Create template" button is pressed.
    /// </summary>
    private bool CreateWebTemplate()
    {
        // Create new web template object
        WebTemplateInfo newTemplate = new WebTemplateInfo();

        // Set the properties
        newTemplate.WebTemplateDisplayName = "My new template";
        newTemplate.WebTemplateName = "MyNewTemplate";
        newTemplate.WebTemplateDescription = "This is web template created by API Exapmle";
        newTemplate.WebTemplateFileName = "~\\App_Data\\Templates\\MyNewTemplate";
        newTemplate.WebTemplateLicenses = "F;S;B;N;C;P;R;E;U;";
        newTemplate.WebTemplatePackages = "ECM;SCN;ADV;DOC;";

        // Set the web template order
        DataSet webTemplates = WebTemplateInfoProvider.GetWebTemplates(null, null, 0, "WebTemplateID", false);
        if (!DataHelper.DataSourceIsEmpty(webTemplates))
        {
            newTemplate.WebTemplateOrder = webTemplates.Tables[0].Rows.Count + 1;
        }
        else
        {
            newTemplate.WebTemplateOrder = 1;
        }

        // Save the web template
        WebTemplateInfoProvider.SetWebTemplateInfo(newTemplate);

        return true;
    }


    /// <summary>
    /// Gets and updates web template. Called when the "Get and update template" button is pressed.
    /// Expects the CreateWebTemplate method to be run first.
    /// </summary>
    private bool GetAndUpdateWebTemplate()
    {
        // Get the web template
        WebTemplateInfo updateTemplate = WebTemplateInfoProvider.GetWebTemplateInfo("MyNewTemplate");
        if (updateTemplate != null)
        {
            // Update the properties
            updateTemplate.WebTemplateDisplayName = updateTemplate.WebTemplateDisplayName.ToLower();

            // Save the changes
            WebTemplateInfoProvider.SetWebTemplateInfo(updateTemplate);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and moves web template down. Called when the "Get and move template down" button is pressed.
    /// Expects the CreateWebTemplate method to be run first.
    /// </summary>
    private bool GetAndMoveWebTemplateDown()
    {
        // Get the web template
        WebTemplateInfo moveDownTemplate = WebTemplateInfoProvider.GetWebTemplateInfo("MyNewTemplate");
        if (moveDownTemplate != null)
        {
            // Move template down
            WebTemplateInfoProvider.MoveTemplateDown(moveDownTemplate.WebTemplateId);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and moves web template up. Called when the "Get and move template up" button is pressed.
    /// Expects the CreateWebTemplate method to be run first.
    /// </summary>
    private bool GetAndMoveWebTemplateUp()
    {
        // Get the web template
        WebTemplateInfo moveUpTemplate = WebTemplateInfoProvider.GetWebTemplateInfo("MyNewTemplate");
        if (moveUpTemplate != null)
        {
            // Move template up
            WebTemplateInfoProvider.MoveTemplateUp(moveUpTemplate.WebTemplateId);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates web templates. Called when the "Get and bulk update templates" button is pressed.
    /// Expects the CreateWebTemplate method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateWebTemplates()
    {
        // Prepare the parameters
        string where = "WebTemplateName LIKE N'MyNewTemplate%'";

        // Get the data
        DataSet templates = WebTemplateInfoProvider.GetWebTemplates(where, null);
        if (!DataHelper.DataSourceIsEmpty(templates))
        {
            // Loop through the individual items
            foreach (DataRow templateDr in templates.Tables[0].Rows)
            {
                // Create object from DataRow
                WebTemplateInfo modifyTemplate = new WebTemplateInfo(templateDr);

                // Update the properties
                modifyTemplate.WebTemplateDisplayName = modifyTemplate.WebTemplateDisplayName.ToUpper();

                // Save the changes
                WebTemplateInfoProvider.SetWebTemplateInfo(modifyTemplate);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes web template. Called when the "Delete template" button is pressed.
    /// Expects the CreateWebTemplate method to be run first.
    /// </summary>
    private bool DeleteWebTemplate()
    {
        // Get the web template
        WebTemplateInfo deleteTemplate = WebTemplateInfoProvider.GetWebTemplateInfo("MyNewTemplate");

        // Delete the web template
        WebTemplateInfoProvider.DeleteWebTemplateInfo(deleteTemplate);

        return (deleteTemplate != null);
    }

    #endregion
}