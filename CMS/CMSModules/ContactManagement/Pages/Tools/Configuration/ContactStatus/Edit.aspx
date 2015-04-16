<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Edit.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Contact status edit" Inherits="CMSModules_ContactManagement_Pages_Tools_Configuration_ContactStatus_Edit"
    Theme="Default" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UIForm runat="server" ID="EditForm" ObjectType="om.contactstatus" RedirectUrlAfterSave="Edit.aspx?statusid={%EditedObject.ID%}&saved=1" />
</asp:Content>
