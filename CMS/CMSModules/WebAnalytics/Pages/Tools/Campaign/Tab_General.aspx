<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Campaign properties - General" Inherits="CMSModules_WebAnalytics_Pages_Tools_Campaign_Tab_General"
    Theme="Default" CodeFile="Tab_General.aspx.cs" %>

<%@ Register Src="~/CMSModules/WebAnalytics/Controls/UI/Campaign/Edit.ascx" TagName="CampaignEdit"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel ID="pnlContent" runat="server">
        <cms:CampaignEdit ID="editElem" runat="server" IsLiveSite="false" />
    </asp:Panel>
</asp:Content>