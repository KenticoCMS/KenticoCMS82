<%@ Page Language="C#" AutoEventWireup="true" CodeFile="List.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Activity list"
    Inherits="CMSModules_ContactManagement_Pages_Tools_Activities_Activity_List" Theme="Default" EnableEventValidation="false" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Activity/List.ascx" TagName="ActivityList" TagPrefix="cms" %>
<%@ Register TagPrefix="cms" TagName="HeaderActions" Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<asp:Content ID="cntBefore" runat="server" ContentPlaceHolderID="plcBeforeContent">
    <asp:Panel runat="server" ID="pnlDis" CssClass="header-panel" Visible="false">
        <cms:DisabledModule runat="server" ID="ucDisabledModule" SettingsKeys="CMSPersonalizeUserInterface" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntActions" runat="server" ContentPlaceHolderID="plcActions">
    <cms:CMSUpdatePanel ID="pnlActons" runat="server">
        <ContentTemplate>
            <div class="control-group-inline">
                <cms:HeaderActions ID="hdrActions" runat="server" IsLiveSite="false" />
                <cms:LocalizedLabel ID="lblWarnNew" runat="server" ResourceString="om.choosesite"
                    EnableViewState="false" Visible="false" CssClass="button-explanation-text" />
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ActivityList ID="listElem" runat="server" IsLiveSite="false" ShowContactNameColumn="true" ShowRemoveButton="true" />
</asp:Content>
