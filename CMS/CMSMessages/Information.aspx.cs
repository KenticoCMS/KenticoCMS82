using System;

using CMS.Helpers;
using CMS.UIControls;

public partial class CMSMessages_Information : MessagePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get localization string from URL
        string msgResString = QueryHelper.GetText("message", QueryHelper.GetText("resstring", ""));

        lblMessage.Text = GetString(msgResString);

        Title = "CMS - Information";

        // Initialize title
        hdnPermission.Text = PageTitle.TitleText = GetString("CMSDesk.Information");
        PageTitle.HideTitle = false;
    }
}