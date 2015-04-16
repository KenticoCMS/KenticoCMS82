<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_MultiFileUploader_MultiFileUploader"
    CodeFile="MultiFileUploader.ascx.cs" %>
<asp:Panel ID="pnlUpload" class="uploader-overlay-div" runat="server">
    <cms:CMSFileUpload ID="uploadFile" type="file" runat="server" />
</asp:Panel>
