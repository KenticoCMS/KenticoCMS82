using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Core;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;

[UIElementAttribute(ModuleName.CMS, "Settings")]
public partial class CMSModules_Settings_Pages_Default : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (CultureHelper.IsUICultureRTL())
        {
            ControlsHelper.ReverseFrames(colsFrameset);
        }

        keysFrame.Attributes.Add("src", UIContextHelper.GetElementUrl(ModuleName.CMS, "Settings.Keys", false));
    }
}