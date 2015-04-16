<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" EnableEventValidation="false" Theme="Default" CodeFile="BuyContact.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_DataCom_BuyContact" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/DataCom/ErrorSummary.ascx" TagName="ErrorSummary" TagPrefix="cms" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:HiddenField ID="ContactHiddenField" runat="server" />
    <asp:Panel runat="server" ID="TopPanel">
        <cms:ErrorSummary ID="ErrorSummary" runat="server" EnableViewState="false" MessagesEnabled="true"></cms:ErrorSummary>
        <p>
            <cms:LocalizedLiteral ID="IntroductionLiteral" runat="server" ResourceString="datacom.buycontact.introduction"></cms:LocalizedLiteral>
        </p>
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="FirstNameLabel" runat="server" AssociatedControlID="FirstNameValue" DisplayColon="true" ResourceString="datacom.contact.firstname"></cms:LocalizedLabel>
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label ID="FirstNameValue" CssClass="form-control-text" runat="server"></asp:Label>
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="LastNameLabel" runat="server" AssociatedControlID="LastNameValue" DisplayColon="true" ResourceString="datacom.contact.lastname"></cms:LocalizedLabel>
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label ID="LastNameValue" CssClass="form-control-text" runat="server"></asp:Label>
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="CompanyNameLabel" runat="server" AssociatedControlID="CompanyNameValue" DisplayColon="true" ResourceString="datacom.contact.companyname"></cms:LocalizedLabel>
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label ID="CompanyNameValue" CssClass="form-control-text" runat="server"></asp:Label>
                </div>
            </div>
        </div>
        <p>
            <asp:Literal ID="AccountPointsLiteral" runat="server"></asp:Literal>
            <cms:LocalizedHyperlink ID="PurchasePointsLink" runat="server" Target="_blank" ResourceString="datacom.buycontact.purchasepoints" Visible="false"></cms:LocalizedHyperlink>
        </p>
    </asp:Panel>
    <asp:LinkButton ID="DummyLinkButton" runat="server"></asp:LinkButton>
    <script type="text/javascript">

        $cmsj(document).ready(function ($) {
            var element = document.getElementById('<%= ContactHiddenField.ClientID %>');
            if (element != null && element.value != null && element.value != "") {
                wopener.DataCom_SetContact(element.value);
                CloseDialog();
            }
        });

    </script>
</asp:Content>
<asp:Content ID="FooterContent" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight" id="FooterPanel" runat="server">
        <cms:LocalizedButton ID="BuyButton" runat="server" ButtonStyle="Primary" EnableViewState="False" ResourceString="datacom.buycontact.buy" OnClick="BuyButton_Click" />
    </div>
</asp:Content>
