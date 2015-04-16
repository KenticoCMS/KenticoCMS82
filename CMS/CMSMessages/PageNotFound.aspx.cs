using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;

public partial class CMSMessages_PageNotFound : MessagePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Try skip IIS http errors
        Response.TrySkipIisCustomErrors = true;
        // Set page not found state
        Response.StatusCode = 404;

        // Set preferred content culture
        SetLiveCulture();

        titleElem.TitleText = GetString("404.Header");
        lblInfo.Text = String.Format(GetString("404.Info"), QueryHelper.GetText("aspxerrorpath", String.Empty));

        lnkBack.Text = GetString("404.Back");
        lnkBack.NavigateUrl = URLHelper.ResolveUrl("~/");
    }
}