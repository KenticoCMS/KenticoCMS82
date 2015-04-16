<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CompanyFilter.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_DataCom_CompanyFilter" %>
<div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="NameLabel" runat="server" AssociatedControlID="NameTextBox" EnableViewState="false" ResourceString="datacom.company.name" DisplayColon="true" />
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSTextBox ID="NameTextBox" runat="server" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group form-group-buttons">
            <div class="filter-form-buttons-cell">
                <cms:LocalizedButton ID="SearchButton" runat="server" ButtonStyle="Primary" EnableViewState="False" ResourceString="general.filter" />
            </div>
        </div>
</div>