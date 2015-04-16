<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StrandsCatalogUploadFrequency.ascx.cs" Inherits="CMSModules_StrandsRecommender_FormControls_StrandsCatalogUploadFrequency" EnableViewState="False" %>

<cms:CMSUpdatePanel runat="server" UpdateMode="Always">
    <ContentTemplate>
        <cms:CMSDropDownList ID="ddlMainFrequency" runat="server" IsLiveSite="false" AutoPostBack="True" />
        
        <span style="width: 50px; text-align: center; display: inline-block;">
            <cms:LocalizedLiteral ID="litExtendedFrequencySpecifier" runat="server" />
        </span>

        <cms:CMSDropDownList ID="ddlHourlyExtendedFrequency" runat="server" IsLiveSite="false" />
        <cms:CMSDropDownList ID="ddlDailyExtendedFrequency" runat="server" IsLiveSite="false" />
        <cms:CMSDropDownList ID="ddlWeeklyExtendedFrequency" runat="server" IsLiveSite="false" />
        
        <asp:Label ID="lblPSTTimeZone" runat="server" Text="PST" />
    </ContentTemplate>
</cms:CMSUpdatePanel>

<div style="margin: 10px 0 0 4px;">
    <cms:LocalizedLabel ID="lblCatalogUploadededInfoMessage" runat="server" ResourceString="strands.uploadfrequency.aftersaveupload" />
</div>