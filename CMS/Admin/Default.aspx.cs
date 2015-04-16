using System;
using System.Web.UI;

using CMS.Base;
using CMS.Helpers;


public partial class Admin_Default : Page
{
    protected override void OnPreInit(EventArgs e)
    {
        string customAdminPath = UIHelper.GetCustomAdministrationPath();
        if (String.IsNullOrEmpty(customAdminPath) || customAdminPath.EqualsCSafe(UIHelper.DEFAULT_ADMINISTRATION_PATH, true))
        {
            URLHelper.Redirect("~/Admin/CMSAdministration.aspx" + RequestContext.URL.Query);
        }
        else
        {
            RequestHelper.Respond404();
        }
        base.OnPreInit(e);
    }
}