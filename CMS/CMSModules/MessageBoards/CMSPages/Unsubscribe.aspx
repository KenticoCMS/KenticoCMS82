<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_MessageBoards_CMSPages_Unsubscribe"
    MasterPageFile="~/CMSMasterPages/LiveSite/SimplePage.master" Theme="Default"
    CodeFile="Unsubscribe.aspx.cs" %>

<%@ Register Src="~/CMSModules/MessageBoards/Controls/Unsubscription.ascx" TagName="Unsubscription"
    TagPrefix="cms" %>
<asp:Content ID="cnt" ContentPlaceHolderID="plcContent" runat="server">
    <cms:Unsubscription ID="unsubscription" runat="server" />
</asp:Content>
