using System;

using CMS.UIControls;

public partial class CMSModules_OnlineMarketing_Pages_Content_ContentPersonalizationVariant_List : CMSContentPersonalizationContentPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Use local document info panel
        DocumentManager.LocalDocumentPanel = pnlDocInfo;

        // Enable split mode
        EnableSplitMode = true;
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set NodeID in order to check the access to the document
        listElem.NodeID = (Node != null) ? Node.NodeID : 0;

        // Remove MoveUp, MoveDown buttons
        listElem.Grid.GridActions.Actions.RemoveRange(2, 2);

        // Display disabled information
        ucDisabledModule.SettingsKeys = "CMSContentPersonalizationEnabled";
        ucDisabledModule.ParentPanel = pnlWarning;

        pnlContainer.Enabled = !DocumentManager.ProcessingAction;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Set selected tab
        SetPropertyTab(TAB_VARIANTS);
    }

    #endregion
}