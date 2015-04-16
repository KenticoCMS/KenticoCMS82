using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Blogs;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_Blogs_CMSPages_Unsubscribe : LivePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle title = PageTitle;
        title.TitleText = HTMLHelper.HTMLEncode(string.Format(GetString("blog.unsubscribe"), (unsubscription.SubscriptionSubject != null) ? ScriptHelper.GetString(unsubscription.SubscriptionSubject.DocumentNamePath) : null)); ;
    }
}