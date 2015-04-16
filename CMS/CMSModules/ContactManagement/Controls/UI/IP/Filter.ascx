<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ContactManagement_Controls_UI_IP_Filter"
    CodeFile="Filter.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TimeSimpleFilter.ascx" TagName="TimeSimpleFilter"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TextSimpleFilter.ascx" TagName="TextSimpleFilter"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<asp:Panel ID="pnl" runat="server" DefaultButton="btnSearch">
    <div class="form-horizontal form-filter">
        <asp:PlaceHolder runat="server" ID="plcSite" Visible="false">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblSite" runat="server" EnableViewState="false" ResourceString="general.site"
                        DisplayColon="true" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plcCon" Visible="false">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblContact" runat="server" EnableViewState="false" ResourceString="om.activity.contactname"
                        DisplayColon="true" />
                </div>
                <cms:TextSimpleFilter ID="fltContact" runat="server" Column="ContactFullName" />
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblIP" runat="server" ResourceString="om.contact.ipaddress"
                    DisplayColon="true" />
            </div>
            <cms:TextSimpleFilter ID="fltIP" runat="server" Column="IPAddress" />
        </div>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblFromTo" runat="server" ResourceString="om.contact.ipdatebetween"
                    DisplayColon="true" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:TimeSimpleFilter ID="dtFromTo" runat="server" Column="IPCreated" />
            </div>
        </div>
        <div class="form-group form-group-buttons">
            <div class="filter-form-buttons-cell-wide">
                <cms:LocalizedButton ID="btnReset" runat="server" ButtonStyle="Default" EnableViewState="false" />
                <cms:LocalizedButton ID="btnSearch" runat="server" ButtonStyle="Primary" ResourceString="general.filter" />
            </div>
        </div>
    </div>
</asp:Panel>
