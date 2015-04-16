<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Variant properties" Inherits="CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABVariant_Edit"
    Theme="Default" CodeFile="Edit.aspx.cs" %>

<%@ Register Src="~/CMSModules/OnlineMarketing/Controls/UI/ABVariant/Edit.ascx" TagName="VariantEdit"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<asp:Content ID="cntHeader" runat="server" ContentPlaceHolderID="plcBeforeContent">
    <asp:Panel runat="server" ID="pnlDisabled" CssClass="header-panel">
        <cms:DisabledModule runat="server" ID="ucDisabledModule" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:VariantEdit ID="editElem" runat="server" IsLiveSite="false" />
</asp:Content>
