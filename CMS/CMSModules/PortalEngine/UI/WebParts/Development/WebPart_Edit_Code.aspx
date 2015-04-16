<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Inherits="CMSModules_PortalEngine_UI_WebParts_Development_WebPart_Edit_Code"
    Theme="Default" EnableEventValidation="false" CodeFile="WebPart_Edit_Code.aspx.cs" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <div class="form-group">
        <strong>
            <cms:LocalizedLabel runat="server" ID="lblBaseControl" ResourceString="WebPartCode.BaseControl" /></strong>
        <cms:CMSTextBox runat="server" ID="txtBaseControl" EnableViewState="false" />
    </div>
    <div class="form-group">
        <strong>
            <cms:LocalizedLabel runat="server" ID="lblASCX" ResourceString="WebPartCode.ASCX" /></strong><br />
        <cms:ExtendedTextArea ID="txtASCX" runat="server" EnableViewState="false" ReadOnly="true"
            EditorMode="Advanced" Width="95%" Height="50px" ShowToolbar="false" />
    </div>
    <div class="form-group">
        <strong>
            <cms:LocalizedLabel runat="server" ID="lblCode" ResourceString="WebPartCode.Code" /></strong><br />
        <cms:ExtendedTextArea ID="txtCS" runat="server" EnableViewState="false" ReadOnly="true"
            EditorMode="Advanced" Language="CSharp" Width="95%" Height="565px" />
    </div>
</asp:Content>
