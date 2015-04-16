using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Sites : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // gets classID from querystring
        int classId = QueryHelper.GetInteger("objectid", 0);
        if (classId > 0)
        {
            classSites.TitleString = GetString("DocumentType_Edit_Sites.Info");
            classSites.ClassId = classId;
            classSites.CheckLicense = false;
        }
    }
}