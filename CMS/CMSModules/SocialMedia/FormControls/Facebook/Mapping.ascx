<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Mapping.ascx.cs" Inherits="CMSModules_SocialMedia_FormControls_Facebook_Mapping" %>
<%@ Register TagPrefix="cms" TagName="Mapping" Src="~/CMSModules/SocialMedia/Controls/Facebook/Mapping.ascx" %>
<%@ Register TagPrefix="cms" TagName="Error" Src="~/CMSModules/SocialMedia/Controls/Facebook/Error.ascx" %>
<cms:CMSUpdatePanel ID="MainUpdatePanel" runat="server">
    <ContentTemplate>
        <asp:HiddenField ID="MappingHiddenField" runat="server" EnableViewState="false" />
        <cms:Error ID="ErrorControl" runat="server" EnableViewState="false" />
        <asp:Panel ID="MappingPanel" runat="server" EnableViewState="false">
            <cms:Mapping ID="MappingControl" runat="server"></cms:Mapping>
        </asp:Panel>
        <p id="MessageLabel" runat="server" enableviewstate="false" visible="false"></p>
        <cms:LocalizedButton ID="EditMappingButton" runat="server" EnableViewState="false" ResourceString="general.edit" ButtonStyle="Default"></cms:LocalizedButton>
    </ContentTemplate>
</cms:CMSUpdatePanel>