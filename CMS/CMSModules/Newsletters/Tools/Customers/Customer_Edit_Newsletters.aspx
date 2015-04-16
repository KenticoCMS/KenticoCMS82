<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Newsletters_Tools_Customers_Customer_Edit_Newsletters" Theme="Default"
    Title="Customer newsletters" CodeFile="Customer_Edit_Newsletters.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcSiteSelector" ID="siteSelector" runat="server">
    <asp:Panel runat="server" ID="pnlSiteSelector" Visible="false" CssClass="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel runat="server" ID="lblSite" EnableViewState="false" DisplayColon="true"
                    ResourceString="General.Site" CssClass="control-label" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" AllowAll="false" />
            </div>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="Customer_Edit_Newsletters.Title"
                CssClass="listing-title" EnableViewState="false" />
            <cms:UniSelector ID="usNewsletters" runat="server" ObjectType="Newsletter.Newsletter"
                SelectionMode="Multiple" ResourcePrefix="newsletterselect" IsLiveSite="false" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
