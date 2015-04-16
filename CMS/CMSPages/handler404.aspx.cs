using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Helpers;
using CMS.UIControls;

public partial class CMSPages_handler404 : MessagePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set preferred content culture
        SetLiveCulture();
        titleElem.TitleText = GetString("404.Header");
        lblInfo.Text = String.Format(GetString("404.Info"), QueryHelper.GetText("aspxerrorpath", String.Empty));

        lnkBack.Text = GetString("404.Back");
        lnkBack.NavigateUrl = "~/";
    }


    /// <summary>
    /// Disable handler base tag.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        UseBaseTagForHandlerPage = false;
        base.OnInit(e);
    }
}