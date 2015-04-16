using System;

using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.DataEngine;
using CMS.Membership;

public partial class CMSMessages_AccessDenied : AccessDeniedPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Try skip IIS HTTP errors
        Response.TrySkipIisCustomErrors = true;

        btnSignOut.OnClientClick = AuthenticationHelper.GetSignOutOnClickScript(this);
        
        string title = GetString("CMSDesk.AccessDenied");
        string message = GetString("CMSMessages.AccessDenied");

        GetTexts(ref message, ref title);

        lblMessage.Text = message;
        titleElem.TitleText = title;

        // Display SignOut button
        if (AuthenticationHelper.IsAuthenticated())
        {
            if (!RequestHelper.IsWindowsAuthentication())
            {
                btnSignOut.Visible = true;
            }
        }
        else
        {
            btnLogin.Visible = true;
        }
    }

    #endregion


    #region "Methods"

    protected override void PerformSignOut()
    {
        base.PerformSignOut();

        ltlScript.Text = ScriptHelper.GetScript("window.top.location.href= '../default.aspx';");
    }

    #endregion


    #region "Button handling"

    protected void btnSignOut_Click(object sender, EventArgs e)
    {
        PerformSignOut();
    }


    protected void btnLogin_Click(object sender, EventArgs e)
    {
        // Get the logon page URL
        string logonPage = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSSecuredAreasLogonPage");
        if (logonPage == string.Empty)
        {
            logonPage = "../CMSPages/Logon.aspx";
        }

        ltlScript.Text = ScriptHelper.GetScript("window.top.location.href = '" + ResolveUrl(logonPage) + "';");
    }

    #endregion
}