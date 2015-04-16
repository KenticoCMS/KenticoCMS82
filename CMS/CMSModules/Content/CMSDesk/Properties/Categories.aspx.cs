using System;

using CMS.Helpers;
using CMS.Membership;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSModules_Content_CMSDesk_Properties_Categories : CMSPropertiesPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", "Properties.Categories"))
        {
            RedirectToUIElementAccessDenied("CMS.Content", "Properties.Categories");
        }

        // Enable split mode
        EnableSplitMode = true;

        // Non-versioned data are edited on this page
        DocumentManager.UseDocumentHelper = false;
        DocumentManager.HandleWorkflow = false;

        DocumentManager.RegisterSaveChangesScript = false;

        SetPropertyTab(TAB_CATEGORIES);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (Node != null)
        {
            // Check modify permissions
            if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
            {
                // Disable selector
                categoriesElem.Enabled = false;
            }

            // Display all global categories in administration UI
            categoriesElem.UserID = MembershipContext.AuthenticatedUser.UserID;
            categoriesElem.Node = Node;
        }

        // UI settings
        categoriesElem.DisplaySavedMessage = false;
        categoriesElem.OnAfterSave += categoriesElem_OnAfterSave;
        categoriesElem.UniSelector.OnSelectionChanged += categoriesElem_OnSelectionChanged;

        // Register the scripts
        ScriptHelper.RegisterLoader(Page);

        pnlContent.Enabled = !DocumentManager.ProcessingAction;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Display document information
        DocumentManager.ShowDocumentInfo(false);

        // Display 'The changes were saved' message
        if (QueryHelper.GetBoolean("saved", false))
        {
            ShowChangesSaved();
        }
    }

    #endregion


    #region "Handlers"

    private void categoriesElem_OnAfterSave()
    {
        ShowChangesSaved();
    }


    private void categoriesElem_OnSelectionChanged(object sender, EventArgs e)
    {
        if (DocumentManager.AllowSave)
        {
            categoriesElem.Save();
        }
    }

    #endregion
}