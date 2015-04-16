using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.Helpers;

public partial class CMSModules_Blogs_CMSPages_SubscriptionApproval : LivePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle title = PageTitle;
        title.TitleText = HTMLHelper.HTMLEncode(string.Format(GetString("blog.subscriptionconfirmation"), (subscriptionApproval.SubscriptionSubject != null) ? ScriptHelper.GetString(subscriptionApproval.SubscriptionSubject.DocumentNamePath) : null)); ;
    }
}
