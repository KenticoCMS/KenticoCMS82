<%@ Control Language="C#" AutoEventWireup="true" CodeFile="community_group.ascx.cs" Inherits="CMSModules_ImportExport_Controls_Import_Site_community_group" %>
<%@ Register Src="~/CMSModules/ImportExport/Controls/Import/Site/media_library.ascx" TagName="MediaLibrarySettings"
    TagPrefix="cms" %>
<cms:MediaLibrarySettings runat="server" ID="mlSettings" />
<asp:Panel runat="server" ID="pnlCheck" CssClass="wizard-section">
    <cms:CMSCheckBox ID="chkObject" runat="server" Visible="false" />
</asp:Panel>
