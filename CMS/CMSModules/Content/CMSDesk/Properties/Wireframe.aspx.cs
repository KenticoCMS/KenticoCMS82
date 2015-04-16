using System;

using CMS.Helpers;
using CMS.UIControls;

[Security(Resource = "CMS.Content", UIElements = "Properties.Wireframe")]
public partial class CMSModules_Content_CMSDesk_Properties_Wireframe : CMSPropertiesPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Register the scripts
        ScriptHelper.RegisterLoader(Page);

        // Set tab mode
        SetPropertyTab(TAB_WIREFRAME);

        wfElem.EditMenu = menuElem;
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        pnlContent.Enabled = !DocumentManager.ProcessingAction;
    }

    #endregion
}