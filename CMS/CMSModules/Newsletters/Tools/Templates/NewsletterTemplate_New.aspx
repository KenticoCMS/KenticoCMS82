<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_Templates_NewsletterTemplate_New"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Tools - Newsletter template edit"
    CodeFile="NewsletterTemplate_New.aspx.cs" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:UIForm runat="server" ObjectType="newsletter.emailtemplate" ID="NewForm" OnOnBeforeSave="OnBeforeSave"
        RedirectUrlAfterCreate=""  OnOnAfterSave="OnAfterSave">
        <SecurityCheck DisableForm="true" Permission="managetemplates" Resource="cms.newsletter" />
    </cms:UIForm>
</asp:Content>
