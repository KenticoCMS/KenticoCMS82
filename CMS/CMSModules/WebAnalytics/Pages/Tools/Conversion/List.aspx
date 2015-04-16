<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Conversion list"
    Inherits="CMSModules_WebAnalytics_Pages_Tools_Conversion_List" Theme="Default" CodeFile="List.aspx.cs" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/UI/Conversion/List.ascx" TagName="ConversionList" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ConversionList ID="listElem" runat="server" IsLiveSite="false" />
</asp:Content>
