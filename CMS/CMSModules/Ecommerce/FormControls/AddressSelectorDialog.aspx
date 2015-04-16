<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddressSelectorDialog.aspx.cs" Inherits="CMSModules_Ecommerce_FormControls_AddressSelectorDialog"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" Title="Order - edit adresses" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UIForm runat="server" FormButtonCssClass="Hidden" ID="EditForm" OnOnAfterDataLoad="EditForm_OnAfterDataLoad" ObjectType="ecommerce.address" AlternativeFormName="OrderPropertiesAddress" RedirectUrlAfterCreate="AddressSelectorDialog.aspx?typeId={?typeId?}&customerId={%EditedObject.AddressCustomerID%}&addressId={%EditedObject.AddressID%}&saved=1&selectorid={?selectorid?}" />
</asp:Content>