<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MassActions.ascx.cs" Inherits="CMSAdminControls_UI_UniGrid_Controls_MassActions" %>

<div class="form-horizontal mass-action">
    <div class="form-group hidden" runat="server" id="divMessages">
    </div>
    <div class="form-group dont-check-changes">
        <div class="mass-action-value-cell">
            <cms:LocalizedLabel runat="server" AssociatedControlID="drpAction" CssClass="sr-only" ResourceString="general.scope" />
            <cms:CMSDropDownList ID="drpScope" runat="server" />
            <cms:LocalizedLabel runat="server" AssociatedControlID="drpScope" CssClass="sr-only" ResourceString="general.action" />
            <cms:CMSDropDownList ID="drpAction" runat="server" EnableViewState="False" />
            <button id="btnOk" runat="server" class="btn-primary btn" type="button"><%: GetString("general.ok") %></button>
        </div>
    </div>
</div>