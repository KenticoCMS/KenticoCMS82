<%@ Page Title="" Language="C#" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    AutoEventWireup="true" CodeFile="EditProperty.aspx.cs" Inherits="CMSModules_PortalEngine_UI_WebParts_EditProperty"
    Theme="Default" %>

<asp:Content ID="content" ContentPlaceHolderID="plcContent" runat="server">
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
        <ContentTemplate>
            <asp:PlaceHolder runat="server" ID="plcControl" />
            <asp:Button runat="server" ID="btnLoad" CssClass="HiddenButton" OnClick="btnLoad_Click" />
            <asp:HiddenField runat="server" ID="hdnValue" EnableViewState="false" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
