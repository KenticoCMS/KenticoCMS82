using System;

using CMS.UIControls;
using CMS.Helpers;

public partial class CMSModules_Messaging_CMSPages_MessageUserSelector_Frameset : CMSLiveModalPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        if (QueryHelper.HashEnabled)
        {
            QueryHelper.ValidateHash("hash");
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Add styles
        RegisterDialogCSSLink();
    }
}