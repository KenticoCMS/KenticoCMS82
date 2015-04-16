<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AbuseReport.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_ActivityDetails_AbuseReport" %>
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblDoc" ResourceString="om.activitydetails.documenturl"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizedLabel  ID="lblDocIDVal" CssClass="form-control-text" runat="server" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblComment" ResourceString="om.activitydetails.abusecomment"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextArea runat="server" ID="txtComment" Rows="10" ReadOnly="true" />
        </div>
    </div>
</div>