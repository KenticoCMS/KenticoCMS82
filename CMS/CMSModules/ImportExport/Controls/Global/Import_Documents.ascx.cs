using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.UIControls;

public partial class CMSModules_ImportExport_Controls_Global_Import_Documents : CMSUserControl
{
    /// <summary>
    /// If the document should be imported.
    /// </summary>
    public bool ImportDocuments
    {
        get
        {
            return radImportDoc.Checked;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        radImportDoc.Text = GetString("ImportSite.wzdStepDocuments.radImportDoc");
        radDoNotImportDoc.Text = GetString("ImportSite.wzdStepDocuments.radDoNotImportDoc");
    }
}