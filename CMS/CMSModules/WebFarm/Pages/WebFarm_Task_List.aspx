<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_WebFarm_Pages_WebFarm_Task_List" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Web farm server - List" Theme="Default" CodeFile="WebFarm_Task_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Content ID="cntBeforeCnt" runat="server" ContentPlaceHolderID="plcBeforeActions">
    <div class="FloatRight">
        <div class="form-horizontal form-filter">
            <div>
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel runat="server" ID="lblServer" CssClass="control-label" AssociatedControlID="uniSelector" ResourceString="WebFarmTasks_List.ServerLabel" EnableViewState="false" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:UniSelector ID="uniSelector" runat="server" ObjectType="cms.webfarmserver" ResourcePrefix="webfarmserver"
                        AllowEmpty="false" IsLiveSite="false" OrderBy="ServerDisplayName" ReturnColumnName="ServerName" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:MessagesPlaceHolder runat="server" ID="plcMess" IsLiveSite="false" />
            <cms:UniGrid runat="server" ID="UniGrid" GridName="WebFarm_Task_List.xml" OrderBy="ServerDisplayName" IsLiveSite="false" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
