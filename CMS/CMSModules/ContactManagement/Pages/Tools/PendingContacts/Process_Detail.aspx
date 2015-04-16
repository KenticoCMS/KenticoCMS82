<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Process_Detail.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_PendingContacts_Process_Detail"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" %>

<%@ Register Src="~/CMSModules/Automation/Controls/Process/Edit.ascx" TagName="Detail"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Automation/Controls/AutomationMenu.ascx" TagName="AutoMenu"
    TagPrefix="cms" %>
<asp:Content ID="cntMenu" ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:AutoMenu ID="menuElem" runat="server" IsLiveSite="false" />
    <cms:CMSAutomationManager ID="autoMan" runat="server" ShortID="aM" />
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUp" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnlContainer" runat="server">
                <cms:Detail runat="server" ID="detailElem" IsLiveSite="false" />
            </asp:Panel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
