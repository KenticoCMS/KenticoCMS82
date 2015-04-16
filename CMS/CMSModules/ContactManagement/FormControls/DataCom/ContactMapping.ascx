<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ContactMapping.ascx.cs" Inherits="CMSModules_ContactManagement_FormControls_DataCom_ContactMapping" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/DataCom/ContactMapping.ascx" TagName="ContactMapping" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/DataCom/ErrorSummary.ascx" TagName="ErrorSummary" TagPrefix="cms" %>

<cms:CMSUpdatePanel ID="MainUpdatePanel" runat="server">
    <ContentTemplate>
        <asp:HiddenField ID="MappingHiddenField" runat="server" EnableViewState="false"></asp:HiddenField>
        <cms:ErrorSummary ID="ErrorSummary" runat="server" EnableViewState="false" />
        <asp:Panel ID="MappingPanel" runat="server" CssClass="datacom-fields-mapping">
            <cms:ContactMapping ID="ContactMappingControl" runat="server"></cms:ContactMapping>
        </asp:Panel>
        <cms:LocalizedButton ID="EditMappingButton" runat="server" EnableViewState="false" ResourceString="datacom.mappingeditor.edit" ButtonStyle="Default"></cms:LocalizedButton>
    </ContentTemplate>
</cms:CMSUpdatePanel>