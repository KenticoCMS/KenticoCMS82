<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_Newsletters_Tab_Templates"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Newsletter edit - Templates"
    CodeFile="Tab_Templates.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="newslettertemplate.assignedtemplates"
        CssClass="listing-title" DisplayColon="true" EnableViewState="false" />
    <cms:CMSUpdatePanel runat="server" ID="pnlAvailability">
        <ContentTemplate>
            <cms:UniSelector ID="usTemplates" runat="server" IsLiveSite="false" ObjectType="newsletter.emailtemplate"
                SelectionMode="Multiple" ResourcePrefix="templatesselect" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
