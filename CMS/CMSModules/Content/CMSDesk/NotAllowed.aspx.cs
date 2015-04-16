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

public partial class CMSModules_Content_CMSDesk_NotAllowed : CMSContentPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get action from quesrystring
        string action = QueryHelper.GetString("action", string.Empty).ToLowerCSafe();
        string errorMessage = "";

        // Setup page title text and image
        PageTitle.TitleText = GetString("Content.NewTitle");
        switch (action)
        {
            case "child":
                errorMessage = GetString("Content.ChildClassNotAllowed");
                break;

            case "new":
                errorMessage = GetString("accessdenied.notallowedtocreatedocument");
                break;

            case "newculture":
                errorMessage = GetString("accessdenied.notallowedtocreatenewcultureversion");
                break;
        }

        ShowError(errorMessage);
    }
}