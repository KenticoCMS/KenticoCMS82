using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_Widgets_LiveDialogs_WidgetProperties_Buttons : CMSWidgetPropertiesLivePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.BodyClass += " Buttons";

        // set button text
        btnOk.Text = GetString("general.ok");
        btnApply.Text = GetString("general.apply");
        btnCancel.Text = GetString("general.cancel");

        btnCancel.OnClientClick = FramesManager.GetCancelScript();
        btnApply.OnClientClick = FramesManager.GetApplyScript();
        btnOk.OnClientClick = FramesManager.GetOKScript();

        if (inline)
        {
            btnApply.Visible = false;
        }
    }
}