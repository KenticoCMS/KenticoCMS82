using System;

using CMS.FormControls;
using CMS.Helpers;
using CMS.Membership;
using CMS.Base;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSAdminControls_UI_Selectors_SelectDocumentGroup : CMSUserControl
{
    private int mSiteId;
    private int mNodeId;
    private int mGroupId;

    private FormEngineUserControl ctrl;

    private const string controlPath = "~/CMSModules/Groups/FormControls/CommunityGroupSelector.ascx";


    protected void Page_Load(object sender, EventArgs e)
    {
        if (FileHelper.FileExists(controlPath))
        {
            // Load the group selector
            ctrl = LoadUserControl(controlPath) as FormEngineUserControl;
            ctrl.ID = "groupSelector";

            plcGroupSelector.Controls.Add(ctrl);

            UniSelector usGroups = ctrl.GetValue("CurrentSelector") as UniSelector;
            if (usGroups != null)
            {
                usGroups.ReturnColumnName = "GroupID";
            }

            usGroups.UseUniSelectorAutocomplete = false;

            mSiteId = QueryHelper.GetInteger("siteid", 0);
            mNodeId = QueryHelper.GetInteger("nodeid", 0);
            mGroupId = QueryHelper.GetInteger("groupid", 0);

            if (!RequestHelper.IsPostBack())
            {
                ctrl.Value = mGroupId;
            }

            ctrl.IsLiveSite = false;
            ctrl.SetValue("DisplayCurrentGroup", false);
            ctrl.SetValue("DisplayNoneValue", true);
            ctrl.SetValue("SiteID", mSiteId);
            ctrl.SetValue("CurrentSelector", usGroups);
        }
    }


    public void ProcessAction()
    {
        if (ctrl != null)
        {
            // Get the node
            TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

            TreeNode node = tree.SelectSingleNode(mNodeId);
            int groupId = ValidationHelper.GetInteger(ctrl.Value, 0);

            // Check inherited documents
            if (chkInherit.Checked)
            {
                tree.ChangeCommunityGroup(node.NodeAliasPath, groupId, mSiteId, true);
            }

            using (new CMSActionContext { LogEvents = false })
            {
                // Update the document node
                node.SetIntegerValue("NodeGroupID", groupId, false);
                node.Update();
            }

            // Log synchronization
            DocumentSynchronizationHelper.LogDocumentChange(node, TaskTypeEnum.UpdateDocument, tree);
        }

        ltlScript.Text = ScriptHelper.GetScript("wopener.ReloadOwner(); CloseDialog();");
    }
}