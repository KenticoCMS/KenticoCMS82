using System;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSModules_Content_CMSDesk_Properties_CreateWireframe : CMSContentPage
{
    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        DocumentManager.OnSaveData += DocumentManager_OnSaveData;
        DocumentManager.HandleWorkflow = false;
    }


    protected override void OnInit(EventArgs e)
    {
        // Load the root category of the selector
        DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo("cms.wireframe");
        if (dci != null)
        {
            selTemplate.RootCategory = dci.ClassPageTemplateCategoryID;
            selTemplate.SetDefaultTemplate(dci.ClassDefaultPageTemplateID);
        }

        selTemplate.IsWireframe = true;
        menuElem.ShowSpellCheck = false;

        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script files
        ScriptHelper.RegisterLoader(this);
        ScriptHelper.RegisterEditScript(Page, false);

        // Hide error label
        lblError.Style.Add("display", "none");
    }

    #endregion


    #region "Other events"

    protected void DocumentManager_OnSaveData(object sender, DocumentManagerEventArgs e)
    {
        TreeNode node = e.Node;

        // Wireframe
        string errorMessage = null;

        PageTemplateInfo pti = selTemplate.EnsureTemplate(node.DocumentName, node.NodeGUID, ref errorMessage);
        
        if (String.IsNullOrEmpty(errorMessage))
        {
            if (pti != null)
            {
                node.NodeWireframeTemplateID = pti.PageTemplateId;
            }

            // Wireframe mode for wireframes
            PortalContext.ViewMode = ViewModeEnum.Wireframe;

            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "Refresh", ScriptHelper.GetScript(String.Format(
                "RefreshTree({0}, {0}); SelectNode({0});",
                node.NodeID
            )));
        }
        else
        {
            e.IsValid = false;
            e.ErrorMessage = errorMessage;
        }
    }

    #endregion
}
