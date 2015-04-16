<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_Filters_DocTypeFilter"
    CodeFile="DocTypeFilter.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<div class="form-horizontal form-filter">
    <div class="form-group">
        <div class="filter-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblClassType" runat="server" ResourceString="queryselection.lblclasstype"
                EnableViewState="false" />
        </div>
        <div class="filter-form-value-cell-wide">
            <cms:CMSDropDownList ID="drpClassType" runat="server" AutoPostBack="True"
                OnSelectedIndexChanged="drpClassType_SelectedIndexChanged" />
        </div>
    </div>
    <div class="form-group">
        <div class="filter-form-label-cell">
            <asp:Label CssClass="control-label" ID="lblDocType" runat="server" />
        </div>
        <div class="filter-form-value-cell-wide">
            <cms:UniSelector ID="uniSelector" runat="server" />
        </div>
    </div>
</div>
