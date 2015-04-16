<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Edit.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Account status edit" Inherits="CMSModules_ContactManagement_Pages_Tools_Configuration_AccountStatus_Edit"
    Theme="Default" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UIForm runat="server" ID="EditForm" ObjectType="om.accountstatus" RedirectUrlAfterSave="Edit.aspx?statusid={%EditedObject.ID%}&saved=1" />
</asp:Content>
