<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_SmartSearch_SearchIndex_General"
    Title="Search Index - General" ValidateRequest="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Theme="Default" CodeFile="SearchIndex_General.aspx.cs" EnableEventValidation="false" %>

<%@ Register Src="~/CMSModules/SmartSearch/Controls/UI/SearchIndex_General.ascx" TagName="SearchIndexEdit" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/SmartSearch/Controls/IndexInfo.ascx" TagName="IndexInfo" TagPrefix="cms" %>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <cms:SearchIndexEdit ID="ucSearchIndexEdit" runat="server" IsLiveSite="false" />
    <cms:CMSUpdatePanel runat="server" ID="pnlIndexInfo" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Timer ID="timIndexInfoRefresh" runat="server" Interval="3000" EnableViewState="false" />
            <cms:IndexInfo ID="ucIndexInfo" runat="server" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
