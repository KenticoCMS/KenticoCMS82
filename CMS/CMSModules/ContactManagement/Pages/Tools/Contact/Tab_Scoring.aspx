<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Tab_Scoring.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Contact properties - IPs" Inherits="CMSModules_ContactManagement_Pages_Tools_Contact_Tab_Scoring"
    Theme="Default" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Contact/Scoring.ascx" TagName="Scoring" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:Scoring runat="server" ID="cScoring" />
</asp:Content>
