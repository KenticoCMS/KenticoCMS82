<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_System_Macros_System_MacroReport" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="System - Macros" CodeFile="System_MacroReport.aspx.cs" %>

<%@ Register Src="~/CMSModules/System/Controls/MacrosGrid.ascx" TagName="MacrosGrid" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:LocalizedLabel ID="lblReport" runat="server" ResourceString="macros.report.description" EnableViewState="False" />
    <br />
    <br />
    <cms:MacrosGrid ID="gridMacros" runat="server" />
</asp:Content>
