using System;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.DocumentEngine;
using CMS.Membership;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSFormControls_Selectors_RelatedDocuments : CMSModalPage
{
    #region "Variables"

    private TreeProvider mTreeProvider = null;
    private TreeNode node = null;
    private string externalControlID = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Tree provider.
    /// </summary>
    protected TreeProvider TreeProvider
    {
        get
        {
            return mTreeProvider ?? (mTreeProvider = new TreeProvider(MembershipContext.AuthenticatedUser));
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize modal page
        RegisterEscScript();
        ScriptHelper.RegisterWOpenerScript(Page);

        if (QueryHelper.ValidateHash("hash"))
        {
            externalControlID = QueryHelper.GetString("externalControlID", string.Empty);
            string title = GetString("Relationship.AddRelatedDocs");
            Page.Title = title;
            PageTitle.TitleText = title;
            Save += btnSave_Click;

            AddNoCacheTag();

            addRelatedDocument.ShowButtons = false;

            if (EditedDocument != null)
            {
                // Get the node
                node = EditedDocument;

                if (node != null)
                {
                    // Check read permissions
                    if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.Read) == AuthorizationResultEnum.Denied)
                    {
                        RedirectToAccessDenied(String.Format(GetString("cmsdesk.notauthorizedtoreaddocument"), node.NodeAliasPath));
                    }
                    else
                    {
                        lblInfo.Visible = false;
                    }

                    // Set tree node
                    addRelatedDocument.TreeNode = node;
                }
            }
        }
        else
        {
            addRelatedDocument.Visible = false;
            string url = ResolveUrl("~/CMSMessages/Error.aspx?title=" + GetString("dialogs.badhashtitle") + "&text=" + GetString("dialogs.badhashtext") + "&cancel=1");
            ltlScript.Text = ScriptHelper.GetScript("window.location = '" + url + "';");
        }
    }


    /// <summary>
    /// Save meta data of attachment.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Argument</param>
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (addRelatedDocument.SaveRelationship())
        {
            string script = "if (wopener.RefreshUpdatePanel_" + externalControlID + ") { wopener.RefreshUpdatePanel_" + externalControlID + "(); CloseDialog(); } ";
            ltlScript.Text = ScriptHelper.GetScript(script);
        }
    }

    #endregion
}