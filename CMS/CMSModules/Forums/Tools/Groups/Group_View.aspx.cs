using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Forums;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_Forums_Tools_Groups_Group_View : CMSForumsPage
{
    protected int groupId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "Forum - Group view";

        groupId = QueryHelper.GetInteger("objectid", 0);
        ForumContext.CheckSite(groupId, 0, 0);

        ForumGroupInfo group = ForumGroupInfoProvider.GetForumGroupInfo(groupId);
        if (group != null)
        {
            Forum1.GroupName = group.GroupName;
        }
    }
}