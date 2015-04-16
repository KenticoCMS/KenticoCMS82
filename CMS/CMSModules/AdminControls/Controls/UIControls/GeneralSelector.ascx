<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/CMSModules/AdminControls/Controls/UIControls/GeneralSelector.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_UIControls_GeneralSelector" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:LocalizedLabel runat="server" ID="lblText" DisplayColon="true" />
<cms:UniSelector runat="server" ID="selectorElem" />
