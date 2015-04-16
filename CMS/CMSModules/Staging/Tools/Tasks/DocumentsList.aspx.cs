using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.Membership;
using CMS.Base;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.DataEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;

[UIElement("CMS.Staging", "Documents")]
public partial class CMSModules_Staging_Tools_Tasks_DocumentsList : CMSStagingPage
{
    #region "Variables"

    private int nodeId = 0;
    private int serverId = 0;
    private TreeProvider mTreeProvider = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Tree provider used for current page.
    /// </summary>
    public TreeProvider TreeProvider
    {
        get
        {
            return mTreeProvider ?? (mTreeProvider = new TreeProvider(MembershipContext.AuthenticatedUser));
        }
        set
        {
            mTreeProvider = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        uniGrid.OnDataReload += uniGrid_OnDataReload;
        uniGrid.OnExternalDataBound += uniGrid_OnExternalDataBound;
        uniGrid.ShowActionsMenu = true;
        uniGrid.Columns = "NodeID, DocumentName, DocumentNamePath, DocumentCulture, DocumentModifiedWhen, ClassDisplayName, NodeHasChildren";
        uniGrid.OnBeforeDataReload += uniGrid_OnBeforeDataReload;
        uniGrid.AllColumns = SqlHelper.JoinColumnList(ObjectTypeManager.GetColumnNames(PredefinedObjectType.NODE, PredefinedObjectType.DOCUMENTLOCALIZATION));

        nodeId = QueryHelper.GetInteger("stagingnodeid", 0);
        serverId = QueryHelper.GetInteger("serverid", 0);
        if (nodeId > 0)
        {
            TreeNode node = TreeProvider.SelectSingleNode(nodeId);
            if (node != null)
            {
                string closeLink = "<a href=\"#\"><span class=\"ListingClose\" style=\"cursor: pointer;\" " +
                                   "onclick=\"parent.frames['tasksHeader'].selectDocuments = false; window.location.href='" +
                                   ResolveUrl("~/CMSModules/Staging/Tools/Tasks/Tasks.aspx?serverid=") + serverId +
                                   "&stagingnodeid=" + nodeId + "';" +
                                   "var completeObj = parent.frames['tasksHeader'].document.getElementById('pnlComplete');" +
                                   "if (completeObj != null){ completeObj.style.display = 'block'; }" +
                                   "return false;\">" + GetString("general.close") +
                                   "</span></a>";
                string docNamePath = "<span class=\"ListingPath\">" + node.DocumentNamePath + "</span>";

                lblListingInfo.Text = String.Format(GetString("synchronization.listinginfo"), docNamePath, closeLink);
            }
        }
    }

    #endregion


    #region "Grid event handlers"

    protected object uniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView data = null;
        switch (sourceName.ToLowerCSafe())
        {
            case "select":
                CMSGridActionButton btnImg = (CMSGridActionButton)sender;
                btnImg.OnClientClick = "parent.frames['tasksHeader'].selectDocuments = false;parent.frames['tasksTree'].SelectTree(" + btnImg.CommandArgument + ");window.location.href='" + ResolveUrl("~/CMSModules/Staging/Tools/Tasks/Tasks.aspx?serverid=") + serverId + "&stagingnodeid=" + btnImg.CommandArgument + "'; return false;";
                return btnImg;

            case "showsubdocuments":
                CMSGridActionButton btnSubImg = (CMSGridActionButton)sender;
                bool hasChildren = ValidationHelper.GetBoolean(((DataRowView)((GridViewRow)parameter).DataItem).Row["NodeHasChildren"], false);
                if (hasChildren)
                {
                    btnSubImg.OnClientClick = "parent.frames['tasksTree'].RefreshNode(" + btnSubImg.CommandArgument + "," + btnSubImg.CommandArgument + ");window.location.href='" + ResolveUrl("~/CMSModules/Staging/Tools/Tasks/DocumentsList.aspx?serverid=") + serverId + "&stagingnodeid=" + btnSubImg.CommandArgument + "'; return false;";
                }
                else
                {
                    string noSubDocuments = GetString("synchronization.nosubdocuments");
                    btnSubImg.ToolTip = noSubDocuments;
                    btnSubImg.Enabled = false;
                    btnSubImg.OnClientClick = "return false;";
                }
                return btnSubImg;

            case "documentname":
                data = (DataRowView)parameter;
                string name = ValidationHelper.GetString(data["DocumentName"], string.Empty);
                return "<span>" + HTMLHelper.HTMLEncode(name) + "</span>";

            case "documentnametooltip":
                data = (DataRowView)parameter;
                return UniGridFunctions.DocumentNameTooltip(data);
        }
        return parameter;
    }


    protected void uniGrid_OnBeforeDataReload()
    {
        string searchText = txtSearch.Text.Trim();
        if (!String.IsNullOrEmpty(searchText))
        {
            searchText = SqlHelper.EscapeQuotes(searchText);
            searchText = SqlHelper.EscapeLikeText(searchText);
            uniGrid.WhereClause = "(DocumentName LIKE N'%" + searchText + "%')";
        }
        else
        {
            uniGrid.WhereClause = null;
        }
    }


    protected DataSet uniGrid_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        string searchText = txtSearch.Text.Trim();
        int parentNodeID = nodeId;

        string where = "(NodeParentID = " + parentNodeID + ")";

        where = SqlHelper.AddWhereCondition(where, completeWhere);

        DataSet nodes = TreeProvider.SelectNodes(TreeProvider.ALL_SITES, "/%", null, true, null, where, null, TreeProvider.ALL_LEVELS, false, currentTopN, columns);
        if (DataHelper.DataSourceIsEmpty(nodes))
        {
            if (String.IsNullOrEmpty(searchText))
            {
                pnlSearch.Visible = false;
                ShowInformation(GetString("synchronization.nochilddocuments"));
            }
            else
            {
                ShowInformation(GetString("synchronization.nodocumentsfound"));
            }
            pnlUniGrid.Visible = false;
        }
        else
        {
            pnlUniGrid.Visible = true;
        }
        totalRecords = DataHelper.GetItemsCount(nodes);
        return nodes;
    }

    #endregion
}