using System;
using System.Linq;
using System.Data;
using System.Text;

using CMS.UIControls;
using CMS.Helpers;
using CMS.Base;
using CMS.DocumentEngine;
using CMS.SiteProvider;
using CMS.Membership;

using TreeNode = CMS.DocumentEngine.TreeNode;
using CMS.DataEngine;
using CMS.Modules;

public partial class CMSModules_Content_CMSDesk_Properties_Alias_AliasList : CMSModalPage
{
    protected override void OnInit(EventArgs e)
    {
        // Check site availability
        if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.Content", SiteContext.CurrentSiteName))
        {
            RedirectToResourceNotAvailableOnSite("CMS.Content");
        }

        // Check security for content and properties tab
        CMSPropertiesPage.CheckPropertiesSecurity();
        CMSContentPage.CheckSecurity();

        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Init unigrid
        ugAlias.WhereCondition = SqlHelper.AddWhereCondition(ugAlias.WhereCondition, "AliasSiteID =" + SiteContext.CurrentSiteID);
        ugAlias.Columns = "AliasID,NodeName,SiteName,AliasCulture AS DocumentCulture,AliasUrlPath,ClassName,ClassIconClass,AliasExtensions,NodeACLID,NodeSiteID,NodeOwner,NodeAliasPath,CampaignDisplayName,NodeParentID";
        ugAlias.Query = "CMS.DocumentAlias.SelectAllNodes";
        ugAlias.OnAction += UniGridAlias_OnAction;
        ugAlias.OnExternalDataBound += UniGridAlias_OnExternalDataBound;
        ugAlias.OnAfterRetrieveData += ugAlias_OnAfterRetrieveData;

        PageTitle.TitleText = GetString("content.ui.urlsaliases");
        // Register tooltip script
        ScriptHelper.RegisterTooltip(Page);
    }


    DataSet ugAlias_OnAfterRetrieveData(DataSet ds)
    {
        // Filter dataset - show only aliases of documents that are visible for current user
        ds = TreeSecurityProvider.FilterDataSetByPermissions(ds, NodePermissionsEnum.Read, MembershipContext.AuthenticatedUser);
        return ds;
    }


    private void UniGridAlias_OnAction(string actionName, object actionArgument)
    {
        // Manage edit and delete
        int aliasID = ValidationHelper.GetInteger(actionArgument, 0);
        string action = DataHelper.GetNotEmpty(actionName, String.Empty).ToLowerCSafe();
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
        DocumentAliasInfo dai = DocumentAliasInfoProvider.GetDocumentAliasInfo(aliasID);

        if (dai != null)
        {
            // Find node 
            TreeNode node = tree.SelectSingleNode(dai.AliasNodeID);

            if (node != null)
            {
                // Check modify permissions
                if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
                {
                    return;
                }

                // Edit only if node exists
                if (action == "edit")
                {
                    URLHelper.Redirect("Alias_Edit.aspx?nodeid=" + node.NodeID + "&aliasid=" + aliasID + "&defaultNodeID=" + NodeID + "&dialog=1");
                }
            }

            // Delete even if node does not exist
            if (action == "delete")
            {
                if (aliasID > 0)
                {
                    // Delete
                    DocumentAliasInfoProvider.DeleteDocumentAliasInfo(aliasID);

                    // Log synchronization
                    DocumentSynchronizationHelper.LogDocumentChange(node, TaskTypeEnum.UpdateDocument, tree);
                }
            }
        }
    }


    protected object UniGridAlias_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        sourceName = sourceName.ToLowerCSafe();
        switch (sourceName)
        {
            case "culture":
                return UniGridFunctions.CultureDisplayName(parameter);

            case "documentname":
                {
                    DataRowView data = (DataRowView)parameter;

                    // Use class name for icon
                    string className = ValidationHelper.GetString(data["ClassName"], string.Empty);

                    // Document name
                    string name = ValidationHelper.GetString(data["NodeName"], string.Empty);

                    // Document alias path
                    string aliasPath = ValidationHelper.GetString(DataHelper.GetDataRowViewValue(data, "NodeAliasPath"), String.Empty);

                    // Create tooltip string
                    string tooltip = "<strong>" + HTMLHelper.HTMLEncode(name) + "</strong>" + (String.IsNullOrEmpty(aliasPath) ? "" : "<br />" + ResHelper.GetString("general.path") + ":&nbsp;" + HTMLHelper.HTMLEncode(aliasPath));

                    // Page type icon
                    var iconClass = ValidationHelper.GetString(data["ClassIconClass"], String.Empty);
                    string icon = UIHelper.GetDocumentTypeIcon(this, className, iconClass);

                    var sb = new StringBuilder();
                    sb.Append(
                        "<span style=\"cursor: help \" onmouseout=\"UnTip()\" onmouseover=\"Tip('", tooltip, "')\" >" + icon + "<span>",
                        HTMLHelper.HTMLEncode(TextHelper.LimitLength(name, 50)),
                        "</span></span>"
                        );

                    return sb.ToString();
                }
        }

        return parameter;
    }

    public string DataHelperClass { get; set; }
}
