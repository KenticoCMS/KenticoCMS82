<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_Subscribers_Subscriber_New"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Tools - Newsletter subscriber edit" CodeFile="Subscriber_New.aspx.cs" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:UIForm runat="server" ID="NewForm" ObjectType="newsletter.subscriber" AlternativeFormName="New" OnOnBeforeSave="OnBeforeSave" RedirectUrlAfterCreate="" OnOnAfterSave="OnAfterSave">
        <SecurityCheck DisableForm="true" Permission="managesubscribers" Resource="cms.newsletter" />
    </cms:UIForm>
</asp:Content>
