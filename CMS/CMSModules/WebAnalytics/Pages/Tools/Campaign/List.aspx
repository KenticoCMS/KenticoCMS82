<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Campaign list"
    Inherits="CMSModules_WebAnalytics_Pages_Tools_Campaign_List" Theme="Default" CodeFile="List.aspx.cs" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/UI/Campaign/List.ascx" TagName="CampaignList" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CampaignList ID="listElem" runat="server" IsLiveSite="false" />
</asp:Content>
