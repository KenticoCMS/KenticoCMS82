using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.FormControls;
using CMS.Controls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.PortalEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;
using System.Data;

public partial class CMSModules_Content_Controls_Wireframe : CMSUserControl
{
    #region "Protected variables"

    protected bool hasModifyPermission = true;

    TreeNode node = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Edit menu
    /// </summary>
    public EditMenu EditMenu
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether the asynchronous post back occurs on the page.
    /// </summary>
    private bool IsAsyncPostback
    {
        get
        {
            return ScriptManager.GetCurrent(Page).IsInAsyncPostBack;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        DocumentManager.OnSaveData += DocumentManager_OnSaveData;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register the scripts
        ScriptHelper.RegisterLoader(Page);

        // Init node
        EditMenu.HandleWorkflow = false;
        DocumentManager.HandleWorkflow = false;
        node = DocumentManager.Node;

        // Setup child controls
        inheritElem.Node = node;

        if (node != null)
        {
            if (node.NodeWireframeTemplateID > 0)
            {
                EditMenu.ShowRemoveWireframe = true;
            }
            else
            {
                // Document does not have the wireframe
                ShowInformation(GetString("Wireframe.NotSet"));

                EditMenu.ShowSave = false;
                EditMenu.ShowCreateWireframe = true;

                pnlForm.Visible = false;
            }
        }

        if (!RequestHelper.IsPostBack())
        {
            ReloadData();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Enable form
        bool enable = hasModifyPermission && DocumentManager.AllowSave;
        pnlForm.Enabled = enable;

        base.OnPreRender(e);
    }

    #endregion


    #region "Control handling"

    protected void DocumentManager_OnSaveData(object sender, DocumentManagerEventArgs e)
    {
        node.SetValue("NodeWireframeComment", txtComment.Text);
        node.SetValue("NodeWireframeInheritPageLevels", inheritElem.Value);
    }

    #endregion


    #region "Methods"

    private void ReloadData()
    {
        if (node != null)
        {
            if (node.IsRoot())
            {
                // Hide inheritance options for root node
                pnlInherits.Visible = false;
            }
            else
            {
                inheritElem.Value = node.NodeWireframeInheritPageLevels;
            }

            var currentUser = MembershipContext.AuthenticatedUser;

            // Check read permissions
            if (currentUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.Read) == AuthorizationResultEnum.Denied)
            {
                RedirectToAccessDenied(String.Format(GetString("cmsdesk.notauthorizedtoreaddocument"), node.NodeAliasPath));
            }
            // Check modify permissions
            else if (!currentUser.IsAuthorizedPerResource("CMS.Design", "Wireframing") || (currentUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied))
            {
                hasModifyPermission = false;
                txtComment.Enabled = false;
                EditMenu.Enabled = false;
            }

            txtComment.Text = node.GetStringValue("NodeWireframeComment", "");
        }
    }

    #endregion
}
