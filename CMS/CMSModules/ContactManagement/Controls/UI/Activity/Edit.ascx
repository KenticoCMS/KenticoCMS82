<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ContactManagement_Controls_UI_Activity_Edit"
    CodeFile="Edit.ascx.cs" %>
<cms:UIForm runat="server" ObjectType="om.activity" IsLiveSite="false" ID="EditForm" AlternativeFormName="CustomActivityForm"
    RedirectUrlAfterCreate="~/CMSModules/ContactManagement/Pages/Tools/Activities/Activity/List.aspx?saved=1&siteid={?siteid?}"
    OnOnBeforeSave="EditForm_OnBeforeSave" OnOnAfterValidate="EditForm_OnAfterValidate"> 
    <SecurityCheck Resource="CMS.ContactManagement" Permission="ManageActivities"/>
</cms:UIForm>