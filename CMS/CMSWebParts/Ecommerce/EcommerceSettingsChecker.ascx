<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Ecommerce_EcommerceSettingsChecker" CodeFile="~/CMSWebParts/Ecommerce/EcommerceSettingsChecker.ascx.cs" %>

<cms:LocalizedLabel runat="server" ID="lblNa" ResourceString="com.settingschecker.na" />
<cms:CMSPlaceHolder runat="server" ID="pnlError">
    <cms:LocalizedHeading runat="server" ID="headTitle" CssClass="widget-content-heading" Level="4" EnableViewState="false" ResourceString="com.settingschecker.check" DisplayColon="true"/>
    <cms:LocalizedLabel runat="server" ID="lblOk" ResourceString="com.settingschecker.ok" />
</cms:CMSPlaceHolder>
