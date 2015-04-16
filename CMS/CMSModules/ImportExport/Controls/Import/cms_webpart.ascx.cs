using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.UIControls;

public partial class CMSModules_ImportExport_Controls_Import_cms_webpart : ImportExportControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (CheckVersion())
        {
            pnlWarning.Visible = true;
            lblWarning.Text = GetString("ImportObjects.WarningObjectVersion");
        }
        else if (CheckHotfixVersion())
        {
            pnlWarning.Visible = true;
            lblWarning.Text = GetString("ImportObjects.WarningObjectHotfixVersion");
        }
    }
}