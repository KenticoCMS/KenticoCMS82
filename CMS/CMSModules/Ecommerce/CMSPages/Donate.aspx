<%@ Page Title="Donate" Language="C#" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    AutoEventWireup="true" CodeFile="Donate.aspx.cs" Inherits="CMSModules_Ecommerce_CMSPages_Donate"
    Theme="Default" %>

<%@ Register TagPrefix="cms" TagName="DonationProperties" Src="~/CMSModules/Ecommerce/Controls/ProductOptions/DonationProperties.ascx" %>
<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <%-- SKU name --%>
    <asp:Label runat="server" ID="lblSKUName" EnableViewState="false" CssClass="BoldInfoLabel" />
    <%-- Description --%>
    <cms:LocalizedLabel ID="lblDescription" runat="server" CssClass="InfoLabel" ResourceString="com.donatedialog.pleasedonate"
        EnableViewState="false" />
    <%-- Donation amount information --%>
    <cms:LocalizedLabel ID="lblAmount" runat="server" CssClass="InfoLabel" EnableViewState="false" />
    <asp:PlaceHolder runat="server" ID="plcMinMaxLabels">
        <cms:LocalizedLabel ID="lblMinimumAmount" runat="server" CssClass="InfoLabel" EnableViewState="false" />
        <cms:LocalizedLabel ID="lblMaximumAmount" runat="server" CssClass="InfoLabel" EnableViewState="false" />
    </asp:PlaceHolder>
    <%-- Error --%>
    <cms:LocalizedLabel ID="lblError" runat="server" CssClass="ErrorLabel" EnableViewState="false"
        Visible="false" />
    <br />
    <%-- Donation properties --%>
    <cms:DonationProperties ID="donationPropertiesElem" runat="server" />
</asp:Content>
<asp:Content ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <%-- Donate --%>
        <cms:LocalizedButton ID="btnDonate" runat="server" ButtonStyle="Primary" ResourceString="com.donatedialog.donate"
            EnableViewState="false" />
    </div>
</asp:Content>
