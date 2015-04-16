using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Groups_Tools_Group_Edit_General : CMSGroupPage
{
    protected int groupId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get groupid
        groupId = QueryHelper.GetInteger("groupid", 0);

        CheckGroupPermissions(groupId, CMSAdminControl.PERMISSION_READ);

        // Initialize editing control
        groupEditElem.GroupID = groupId;
        groupEditElem.DisplayAdvanceOptions = true;
        groupEditElem.IsLiveSite = false;

        if (SiteContext.CurrentSite != null)
        {
            groupEditElem.SiteID = SiteContext.CurrentSite.SiteID;
        }
    }
}