<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ContactFilter.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_DataCom_ContactFilter" %>
<div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="FirstNameLabel" runat="server" AssociatedControlID="FirstNameTextBox" EnableViewState="false" ResourceString="datacom.contact.firstname" DisplayColon="true"></cms:LocalizedLabel>
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSTextBox ID="FirstNameTextBox" runat="server" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="LastNameLabel" runat="server" AssociatedControlID="LastNameTextBox" EnableViewState="false" ResourceString="datacom.contact.lastname" DisplayColon="true"></cms:LocalizedLabel>
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSTextBox ID="LastNameTextBox" runat="server" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="EmailLabel" runat="server" AssociatedControlID="EmailTextBox" EnableViewState="false" ResourceString="datacom.contact.email" DisplayColon="true"></cms:LocalizedLabel>
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSTextBox ID="EmailTextBox" runat="server" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="CompanyNameLabel" runat="server" AssociatedControlID="CompanyNameTextBox" EnableViewState="false" ResourceString="datacom.contact.companyname" DisplayColon="true"></cms:LocalizedLabel>
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSTextBox ID="CompanyNameTextBox" runat="server" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group form-group-buttons">
            <div class="filter-form-buttons-cell">
                <cms:LocalizedButton ID="SearchButton" runat="server" ButtonStyle="Primary" EnableViewState="False" ResourceString="general.filter" />
            </div>
        </div>
</div>