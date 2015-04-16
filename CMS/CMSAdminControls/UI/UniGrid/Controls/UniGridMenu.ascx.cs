using System;

using CMS.ExtendedControls;
using CMS.Helpers;

public partial class CMSAdminControls_UI_UniGrid_Controls_UniGridMenu : CMSContextMenuControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string menuId = ContextMenu.MenuID;
        string parentElemId = ContextMenu.ParentElementClientID;

        string parameterScript = "GetContextMenuParameter('" + menuId + "')";

        string actionPattern = "UG_Export_" + parentElemId + "('{0}', " + parameterScript + ");";

        // Initialize menu
        lblExcel.Text = ResHelper.GetString("export.exporttoexcel");
        pnlExcel.Attributes.Add("onclick", ScriptHelper.GetDisableProgressScript() + String.Format(actionPattern, DataExportFormatEnum.XLSX));

        lblCSV.Text = ResHelper.GetString("export.exporttocsv");
        pnlCSV.Attributes.Add("onclick", ScriptHelper.GetDisableProgressScript() + String.Format(actionPattern, DataExportFormatEnum.CSV));

        lblXML.Text = ResHelper.GetString("export.exporttoxml");
        pnlXML.Attributes.Add("onclick", ScriptHelper.GetDisableProgressScript() + String.Format(actionPattern, DataExportFormatEnum.XML));

        lblAdvancedExport.Text = ResHelper.GetString("export.advancedexport");
        pnlAdvancedExport.Attributes.Add("onclick", string.Format(actionPattern, "advancedexport"));
    }
}