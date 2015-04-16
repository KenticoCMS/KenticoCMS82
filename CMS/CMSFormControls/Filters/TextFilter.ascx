<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TextFilter.ascx.cs" Inherits="CMSFormControls_Filters_TextFilter" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <div class="form-condition-cell-generated">
            <cms:CMSDropDownList ID="drpOperator" AutoPostBack="true" runat="server" CssClass="ExtraSmallDropDown" />
        </div>
        <div class="form-value-cell-generated">
            <cms:CMSTextBox ID="txtText" runat="server" />
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
