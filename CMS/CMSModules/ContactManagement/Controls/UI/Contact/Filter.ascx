<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Filter.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_Contact_Filter" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TextSimpleFilter.ascx" TagName="TextSimpleFilter"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ContactManagement/FormControls/ContactStatusSelector.ascx"
    TagName="ContactStatusSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TimeSimpleFilter.ascx" TagName="TimeSimpleFilter"
    TagPrefix="cms" %>
<asp:Panel ID="pnlFilter" runat="server" DefaultButton="btnSearch">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblName" runat="server" ResourceString="general.firstname"
                    DisplayColon="true" EnableViewState="false" />
            </div>
            <cms:TextSimpleFilter ID="fltFirstName" runat="server" Column="ContactFirstName" />
        </div>
        <asp:PlaceHolder ID="plcMiddle" runat="server" Visible="false">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblMiddleName" runat="server" ResourceString="general.middlename"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <cms:TextSimpleFilter ID="fltMiddleName" runat="server" Column="ContactMiddleName" />
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblLastName" runat="server" ResourceString="general.lastname"
                    DisplayColon="true" EnableViewState="false" />
            </div>
            <cms:TextSimpleFilter ID="fltLastName" runat="server" Column="ContactLastName" />
        </div>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblEmail" runat="server" ResourceString="general.emailaddress" DisplayColon="true"
                    EnableViewState="false" />
            </div>
            <cms:TextSimpleFilter ID="fltEmail" runat="server" Column="ContactEmail" />
        </div>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblContactStatus" runat="server" ResourceString="om.contactstatus"
                    DisplayColon="true" EnableViewState="false" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:ContactStatusSelector ID="fltContactStatus" runat="server" IsLiveSite="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblMonitored" runat="server" ResourceString="general.search"
                    DisplayColon="true" EnableViewState="false" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:CMSRadioButtonList ID="radMonitored" runat="server" RepeatDirection="Horizontal" />
            </div>
        </div>
        <asp:PlaceHolder ID="plcSite" runat="server" Visible="false">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblSite" runat="server" ResourceString="general.site" DisplayColon="true"
                        EnableViewState="false" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" AllowGlobal="true"
                        Visible="false" />
                    <cms:SiteOrGlobalSelector ID="siteOrGlobalSelector" runat="server" IsLiveSite="false"
                        Visible="false" AutoPostBack="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcAdvancedSearch" runat="server" Visible="false">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblPhone" runat="server" ResourceString="general.phone" DisplayColon="true"
                        EnableViewState="false" />
                </div>
                <cms:TextSimpleFilter ID="fltPhone" runat="server" />
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblOwner" runat="server" ResourceString="om.contact.owner"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <cms:TextSimpleFilter ID="fltOwner" runat="server" Column="FullName" />
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblCountry" runat="server" ResourceString="general.country"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <cms:TextSimpleFilter ID="fltCountry" runat="server" Column="CountryDisplayName" />
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblState" runat="server" ResourceString="general.state" DisplayColon="true"
                        EnableViewState="false" />
                </div>
                <cms:TextSimpleFilter ID="fltState" runat="server" Column="StateDisplayName" />
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblCity" runat="server" ResourceString="general.city" DisplayColon="true"
                        EnableViewState="false" />
                </div>
                <cms:TextSimpleFilter ID="fltCity" runat="server" Column="ContactCity" />
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblSalesForceLeadReplicationStatus" runat="server" ResourceString="general.search"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:CMSRadioButtonList ID="radSalesForceLeadReplicationStatus" runat="server" RepeatDirection="Horizontal" />
                </div>
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblIP" runat="server" ResourceString="eventloglist.ipaddress"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:CMSTextBox ID="txtIP" runat="server" CssClass="LongFilterTextBox" MaxLength="15" />
                </div>
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblCreated" runat="server" ResourceString="filter.createdbetween"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:TimeSimpleFilter runat="server" ID="fltCreated" Column="ContactCreated" />
                </div>
            </div>
            <asp:PlaceHolder ID="plcMerged" runat="server">
                <div class="form-group">
                    <div class="filter-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblMerged" runat="server" ResourceString="om.contact.mergedcontact"
                            DisplayColon="true" EnableViewState="false" AssociatedControlID="chkMerged" />
                    </div>
                    <div class="filter-form-value-cell-wide">
                        <cms:CMSCheckBox ID="chkMerged" runat="server" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcChildren" runat="server" Visible="false">
                <div class="form-group">
                    <div class="filter-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblChildren" runat="server" ResourceString="om.contact.listchildrencheck"
                            DisplayColon="true" EnableViewState="false" AssociatedControlID="chkChildren" />
                    </div>
                    <div class="filter-form-value-cell-wide">
                        <cms:CMSCheckBox ID="chkChildren" runat="server" />
                    </div>
                </div>
            </asp:PlaceHolder>
        </asp:PlaceHolder>
        <div class="form-group form-group-buttons">
            <div class="filter-form-label-cell">
                <asp:Panel ID="pnlAdvanced" runat="server" Visible="true">
                    <asp:LinkButton ID="lnkShowSimpleFilter" runat="server" OnClick="lnkShowSimpleFilter_Click" CssClass="simple-advanced-link" />
                </asp:Panel>
                <asp:Panel ID="pnlSimple" runat="server" Visible="false">
                    <asp:LinkButton ID="lnkShowAdvancedFilter" runat="server" OnClick="lnkShowAdvancedFilter_Click" CssClass="simple-advanced-link" />
                </asp:Panel>
            </div>
            <div class="filter-form-buttons-cell-wide-with-link">
                <cms:LocalizedButton ID="btnReset" runat="server" ButtonStyle="Default" EnableViewState="false" />
                <cms:LocalizedButton ID="btnSearch" runat="server" ButtonStyle="Primary" ResourceString="general.filter" />
            </div>
        </div>
    </div>
</asp:Panel>