<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DateTimeFilter.ascx.cs"
    Inherits="CMSFormControls_Filters_DateTimeFilter" %>
<div class="control-group-inline">
    <cms:DateTimePicker ID="dtmTimeFrom" runat="server" />
    <cms:LocalizedLabel ID="lblTimeBetweenAnd" runat="server" ResourceString="general.and" CssClass="form-control-text" />
</div>
<div class="control-group-inline">
    <cms:DateTimePicker ID="dtmTimeTo" runat="server" />
</div>
