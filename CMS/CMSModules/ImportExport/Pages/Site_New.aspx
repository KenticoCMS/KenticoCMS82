<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_ImportExport_Pages_Site_New"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="New Site" CodeFile="Site_New.aspx.cs" %>

<%@ Register Src="~/CMSModules/ImportExport/Controls/NewSiteWizard.ascx" TagName="NewSiteWizard" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">

     <asp:Panel ID="PanelBody" runat="server" CssClass="PageBody">
        <asp:Panel ID="PanelNewSite" runat="server" CssClass="PageContent">
            <cms:NewSiteWizard ID="NewSiteWizard" runat="server" />
        </asp:Panel>
    </asp:Panel>

</asp:Content>