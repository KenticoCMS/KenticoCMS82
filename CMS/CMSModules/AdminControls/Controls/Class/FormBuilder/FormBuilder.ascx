<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_AdminControls_Controls_Class_FormBuilder_FormBuilder"
    CodeFile="FormBuilder.ascx.cs" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FormBuilder/Settings.ascx" TagName="Settings"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FormBuilder/FormComponents.ascx" TagName="FormComponents"
    TagPrefix="cms" %>

<asp:Panel ID="pnlFormBuilder" runat="server" DefaultButton="hdnFormBuilderUpdate" CssClass="form-builder">
    <cms:FormComponents runat="server" ID="pnlFormComponents" />
    <cms:CMSUpdatePanel ID="pnlUpdateForm" UpdateMode="Conditional" ChildrenAsTriggers="false" runat="server">
        <ContentTemplate>
            <div class="form-builder-form">
                <cms:MessagesPlaceHolder ID="plcMessagesHolder" runat="server" ShortID="m" UseRelativePlaceHolder="false" />
                <asp:Button ID="hdnFormBuilderUpdate" ClientIDMode="Static" runat="server" CssClass="HiddenButton" />
                <cms:LocalizedLabel ID="lblEmptyFormPlaceholder" runat="server" ResourceString="FormBuilder.EmptyFormPlaceholder" CssClass="empty-form-placeholder" Style="display: none" />
                <cms:BasicForm runat="server" ID="formElem" IsLiveSite="false" IsDesignMode="true" EnableViewState="false"
                    DefaultFieldLayout="TwoColumns" MarkRequiredFields="true" AutomaticLabelWidth="true" FormButtonCssClass="btn btn-primary" />
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <cms:Settings runat="server" ID="pnlSettings" />
</asp:Panel>
