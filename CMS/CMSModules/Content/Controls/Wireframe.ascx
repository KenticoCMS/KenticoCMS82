<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Wireframe.ascx.cs" Inherits="CMSModules_Content_Controls_Wireframe" %>
<%@ Register Src="~/CMSModules/PortalEngine/FormControls/PageTemplates/PageTemplateLevels.ascx"
    TagName="PageTemplateLevel" TagPrefix="cms" %>
<asp:Panel ID="pnlForm" runat="server">
    <cms:CMSUpdatePanel ID="ucPanel" runat="server">
        <ContentTemplate>
            <div class="form-horizontal">
                <cms:LocalizedHeading ID="lblComment" runat="server" EnableViewState="false" ResourceString="Wireframe.Comment" Level="4"/>
                <cms:CMSTextArea ID="txtComment" runat="server" Rows="17" />
                <cms:LocalizedHeading ID="LocalizedHeading1" runat="server" Level="4" ResourceString="PageProperties.InheritLevels"></cms:LocalizedHeading>
                <asp:Panel runat="server" ID="pnlInherits" CssClass="NodePermissions">
                    <cms:PageTemplateLevel runat="server" ID="inheritElem" IsWireframeTemplate="true" />
                </asp:Panel>
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Panel>
