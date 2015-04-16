using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_Groups_Tools_Forums_Posts_ForumPost_LeftBorder : CMSGroupForumPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.GetBoolean("changemaster", false))
        {
            pnlInner.Visible = false;
        }
    }
}