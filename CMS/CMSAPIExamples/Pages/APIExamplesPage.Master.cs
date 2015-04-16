using System;

using CMS.Helpers;
using CMS.UIControls;
using CMS.Base;

public partial class CMSAPIExamples_Pages_APIExamplesPage : CMSMasterPage
{
    #region "Properties"

    public override PageTitle Title
    {
        get
        {
            return titleElem;
        }
    }

    #endregion


    #region "Page methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblCreateInfo.Text = "This section provides API examples for creating, getting and updating objects. It is recommended to follow the API examples order.";
        lblCleanInfo.Text = "This section provides API examples for deleting objects and their dependencies. The order of the cleanup examples is usually reversed.";
     
        if (!RequestHelper.IsPostBack())
        {
            if (SystemContext.IsRunningOnAzure)
            {
                txtCode.Text = @"
//
// Viewing of the source code is disabled on Microsoft Azure.
//";
            }
            else
            {
                txtCode.Text = @"
//
// Source code of the example will be displayed after clicking the 'View code' button.
//";
            }
        }
    }

    #endregion


    #region Events

    /// <summary>
    /// Runs all create and update examples on the page.
    /// </summary>
    protected void btnRunAll_Click(object sender, EventArgs e)
    {
        CMSAPIExamplePage examplePage = Page as CMSAPIExamplePage;
        if (examplePage != null)
        {
            examplePage.RunAll();
        }
    }


    /// <summary>
    /// Runs all cleanup examples on the page.
    /// </summary>
    protected void btnCleanAll_Click(object sender, EventArgs e)
    {
        CMSAPIExamplePage examplePage = Page as CMSAPIExamplePage;
        if (examplePage != null)
        {
            examplePage.CleanUpAll();
        }
    }

    #endregion
}