<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Staging_Tools_Data_Tree"
    EnableEventValidation="false" Theme="Default" CodeFile="tree.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/Trees/ObjectTree.ascx" TagName="ObjectTree" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/Trees/TreeBorder.ascx" TagName="TreeBorder" TagPrefix="cms" %>
<!DOCTYPE html>
<html>
<head id="Head1" runat="server" enableviewstate="false">
    <title>Content - Tree</title>
</head>
<body class="TreeBody <%=mBodyClass%>">
    <form id="form1" runat="server">
        <asp:Panel runat="server" ID="pnlBody" CssClass="ContentTree">
            <cms:TreeBorder ID="borderElem" runat="server" FramesetName="colsFrameset" />
            <div class="TreeArea">
                <div class="TreeAreaTree">
                    <asp:TreeView ID="objectTree" runat="server" ShowLines="true" ShowExpandCollapse="true"
                        CssClass="ContentTree" EnableViewState="false" />
                </div>
            </div>
        </asp:Panel>
        <input type="hidden" id="selectedObjectType" name="selectedObjectType" value="" />

        <script type="text/javascript">
            //<![CDATA[
            var currentNode = document.getElementById('treeSelectedNode');

            // Refresh node action
            function RefreshNode(nodeId, selectNodeId, selectSiteId) {
                if (selectNodeId == null) {
                    selectNodeId = currentNodeId;
                }
                if (selectSiteId == null) {
                    selectSiteId = currentSiteId;
                }
                document.location.replace(treeUrl + "?objecttype=" + selectNodeId + "&siteid=" + currentSiteId);
            }

            function SelectNode(objectType, nodeElem) {
                if ((currentNode != null) && (nodeElem != null)) {
                    currentNode.className = 'ContentTreeItem';
                }

                parent.frames['tasksContent'].SelectNode(parent.frames['tasksContent'].currentServerId, objectType);
                document.getElementById('selectedObjectType').value = objectType;

                if (nodeElem != null) {
                    currentNode = nodeElem;
                    currentNode.className = 'ContentTreeSelectedItem';
                }
            }

            //]]>
        </script>

    </form>
</body>
</html>
