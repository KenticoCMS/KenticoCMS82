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

public partial class CMSModules_Forums_Tools_Forums_Forum_Moderators : CMSForumsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int forumID = QueryHelper.GetInteger("forumid", 0);
        ForumContext.CheckSite(0, forumID, 0);

        forumModerators.ForumID = forumID;
        forumModerators.IsLiveSite = false;
    }
}