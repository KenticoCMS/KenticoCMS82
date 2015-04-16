using System;

using CMS.Core;
using CMS.UIControls;
using CMS.Ecommerce;

[Title("com.reportsui.datagenerator")]
[UIElement(ModuleName.ECOMMERCE, "SampleDataGenerator")]
public partial class CMSModules_Ecommerce_Pages_Tools_Reports_SampleData_Generator : CMSEcommerceReportsPage
{
    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check permissions
        if (!CurrentUser.IsGlobalAdministrator)
        {
            RedirectToAccessDenied(GetString("security.accesspage.onlyglobaladmin"));
        }

        // Generate JavaScript prompt
        btnGenerate.OnClientClick = GenerateConfirmation("com.generator.generateconfirm");
        btnDelete.OnClientClick = GenerateConfirmation("com.generator.deleteconfirm");

        // Display info message
        ShowInformation(GetString("com.generator.mainpericope"));
    }

    #endregion


    #region "Form Events"

    protected void btnGenerateClick(object sender, EventArgs e)
    {
        if (OrderInfoProvider.GenerateData())
        {
            ShowConfirmation(GetString("com.reports.datagenerated"));
        }
        else
        {
            ShowError(GetString("com.reports.operationFailed"));
        }
    }


    protected void btnDeleteClick(object sender, EventArgs e)
    {
        if (OrderInfoProvider.DeleteData())
        {
            ShowConfirmation(GetString("com.reports.datadeleted"));
        }
        else
        {
            ShowError(GetString("com.reports.operationFailed"));
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Generates JavaScript confirm pop-up.
    /// </summary>
    /// <param name="resourceString">Resource String to be shown</param>
    private string GenerateConfirmation(string resourceString)
    {
        return "return confirm(\"" + GetString(resourceString) + "\")";
    }

    #endregion
}