<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Inherits="CMSModules_ContactManagement_Controls_UI_ActivityDetails_BizFormDetails" Theme="Default"
    CodeFile="BizFormDetails.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:BizForm ID="bizRecord" runat="server" IsLiveSite="false" DefaultFormLayout="Divs" />
</asp:Content>