<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_MediaLibrary_Controls_MediaLibrary_ImageEditor_Control" CodeFile="ImageEditor.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/ImageEditor/BaseImageEditor.ascx" TagName="BaseImageEditor"
    TagPrefix="cms" %>
<asp:PlaceHolder ID="plcContent" runat="server">
    <cms:BaseImageEditor ID="baseImageEditor" runat="server" />
</asp:PlaceHolder>
