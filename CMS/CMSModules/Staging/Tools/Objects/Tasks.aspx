<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Staging_Tools_Objects_Tasks" Theme="Default" Title="Staging - Tasks"
    CodeFile="Tasks.aspx.cs" %>

<%@ Register Src="~/CMSModules/Staging/FormControls/ServerSelector.ascx" TagName="ServerSelector"
    TagPrefix="cms" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>

<%@ Register Src="~/CMSAdminControls/AsyncLogDialog.ascx" TagName="AsyncLog" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>

<asp:Content ID="cntHeader" runat="server" ContentPlaceHolderID="plcActions">
    <div class="form-horizontal form-filter server-selector">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblServers" runat="server" EnableViewState="false" ResourceString="Tasks.SelectServer" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:ServerSelector ID="selectorElem" runat="server" IsLiveSite="false" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel runat="server" ID="pnlLog" Visible="false">
                <cms:AsyncLog ID="ctlAsyncLog" runat="server" />
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlNotLogged">
                <cms:DisabledModule runat="server" ID="ucDisabledModule" />
            </asp:Panel>
            <asp:PlaceHolder ID="plcContent" runat="server">
                <cms:UniGrid ID="gridTasks" runat="server" GridName="~/CMSModules/Staging/Tools/Objects/Tasks.xml"
                    IsLiveSite="false" OrderBy="TaskTime, TaskID" DelayedReload="false" ExportFileName="staging_task" />
                <br />
                <asp:Panel ID="pnlFooter" runat="server" Style="clear: both;">
                    <table style="width: 100%;">
                        <tr>
                            <td>
                                <cms:LocalizedButton runat="server" ID="btnSyncSelected" ButtonStyle="Default" OnClick="btnSyncSelected_Click"
                                    ResourceString="Tasks.SyncSelected" EnableViewState="false" />
                                <cms:LocalizedButton runat="server" ID="btnSyncAll" ButtonStyle="Default" OnClick="btnSyncAll_Click"
                                    ResourceString="Tasks.SyncAll" EnableViewState="false" />
                            </td>
                            <td class="TextRight">
                                <cms:LocalizedButton runat="server" ID="btnDeleteSelected" ButtonStyle="Default"
                                    OnClick="btnDeleteSelected_Click" ResourceString="Tasks.DeleteSelected" EnableViewState="false" />
                                <cms:LocalizedButton runat="server" ID="btnDeleteAll" ButtonStyle="Default" OnClick="btnDeleteAll_Click"
                                    ResourceString="Tasks.DeleteAll" EnableViewState="false" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </asp:PlaceHolder>
            <script type="text/javascript">
                //<![CDATA[
                var currentNodeId = '',
                    currentSiteId = -1;

                function ChangeServer(value) {
                    currentServerId = value;
                }

                function SelectNode(serverId, nodeId, siteId) {
                    currentServerId = serverId;
                    currentNodeId = nodeId;
                    currentSiteId = siteId;
                    document.location = 'Tasks.aspx?serverId=' + currentServerId + '&objecttype=' + nodeId + '&siteid=' + siteId;
                }
                //]]>
            </script>
            <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
