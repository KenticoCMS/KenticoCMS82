<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Inherits="CMSModules_Newsletters_Tools_Subscribers_Subscriber_General"
    Theme="Default" Title="Tools - Newsletter subscriber edit" CodeFile="Subscriber_General.aspx.cs" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:UIForm ID="EditForm" runat="server" ClassName="newsletter.subscriber"
        IsLiveSite="false" ObjectType="newsletter.subscriber" OnOnAfterValidate="EditForm_OnAfterValidate" 
        OnOnBeforeSave="EditForm_OnBeforeSave" >
        <SecurityCheck Resource="cms.newsletter" Permission="managesubscribers" DisableForm="true" />
    </cms:UIForm>
</asp:Content>
