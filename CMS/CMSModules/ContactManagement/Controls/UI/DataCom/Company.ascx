<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Company.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_DataCom_Company" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/DataCom/ErrorSummary.ascx" TagName="ErrorSummary" TagPrefix="cms" %>
<cms:ErrorSummary ID="ErrorSummary" runat="server" EnableViewState="false" />
<div class="form-horizontal">
    <asp:Repeater ID="AttributeRepeater" runat="server">
        <ItemTemplate>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <span class="control-label"><%# HTMLHelper.HTMLEncode(((RepeaterDataItem)Container.DataItem).AttributeName) %></span>
                </div>
                <div class="editing-form-value-cell">
                    <span class="form-control-text"><%# HTMLHelper.HTMLEncode(((RepeaterDataItem)Container.DataItem).AttributeValue) %></span>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
