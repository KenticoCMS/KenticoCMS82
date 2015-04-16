<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RecycleBin_Documents.aspx.cs"
    Inherits="CMSModules_RecycleBin_Pages_RecycleBin_Documents" Theme="Default" EnableEventValidation="true"
    MaintainScrollPositionOnPostback="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Pages recycle bin" %>

<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/RecycleBin/Controls/RecycleBin.ascx" TagName="RecycleBin"
    TagPrefix="cms" %>
<asp:Content ID="cntBefore" ContentPlaceHolderID="plcBeforeContent" runat="server">
    <asp:Panel runat="server" ID="pnlBody">
        <asp:Panel ID="pnlSiteSelector" runat="server" CssClass="cms-edit-menu">
            <div class="form-horizontal form-filter selector-right">
                <div>
                    <div class="filter-form-label-cell">
                        <cms:LocalizedLabel ID="lblSite" runat="server" EnableViewState="false" ResourceString="Administration-RecycleBin.Site" CssClass="control-label" />
                    </div>
                    <div class="filter-form-value-cell-wide">
                        <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" />
                    </div>
                </div>
            </div>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <cms:RecycleBin ID="recycleBin" runat="server" IsSingleSite="false" IsLiveSite="false" DisplayDateTimeFilter="true" />
</asp:Content>
