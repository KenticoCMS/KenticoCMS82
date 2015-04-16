using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_AdminControls_Controls_UIControls_MessagesPlaceholder : CMSAbstractUIWebpart
{
    protected override void OnInit(EventArgs e)
    {
        // Set control's placeholder and actions for use of non-children controls
        ICMSPage page = Page as ICMSPage;
        if (page != null)
        {
            page.MessagesPlaceHolder = plcMess;
        }

        ManageTexts();

        base.OnInit(e);
    }
}
