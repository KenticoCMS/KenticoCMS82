<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Tab_IPs.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Contact properties - IPs" Inherits="CMSModules_ContactManagement_Pages_Tools_Contact_Tab_IPs"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Ip/List.ascx" TagName="IpList" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/IP/Filter.ascx" TagName="IpFilter" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:IpList ID="listElem" runat="server" IsLiveSite="false" />
</asp:Content>
