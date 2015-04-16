<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditFromSettings.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Chat support canned response properties" Inherits="CMSModules_Chat_Pages_Tools_ChatSupportCannedResponse_EditFromSettings" Theme="Default" %>
<%@ Register Src="~/CMSModules/Chat/Controls/UI/ChatSupportCannedResponse/Edit.ascx"
    TagName="ChatSupportCannedResponseEdit" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlCannedResponseList">
        <cms:ChatSupportCannedResponseEdit ID="editElem" runat="server" IsLiveSite="false" />
    </asp:Panel>
</asp:Content>
<asp:Content runat="server" ID="cntFooter" ContentPlaceHolderID="plcFooter">
    <cms:LocalizedHyperlink runat="server" NavigateUrl="~/CMSModules/Chat/Pages/ChatSupportSettings.aspx" ResourceString="general.cancel" CssClass="btn btn-default"></cms:LocalizedHyperlink>
</asp:Content>