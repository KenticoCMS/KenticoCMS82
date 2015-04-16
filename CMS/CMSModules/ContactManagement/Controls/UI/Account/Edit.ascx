<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ContactManagement_Controls_UI_Account_Edit"
    CodeFile="Edit.ascx.cs" %>
<%@ Register TagPrefix="cms" TagName="AnchorDropup" Src="~/CMSAdminControls/UI/PageElements/AnchorDropup.ascx" %>

<asp:Panel ID="panelMergedAccountDetails" runat="server" CssClass="contactmanagement-accountdetail-accountmergedinto">
    <cms:LocalizedHeading runat="server" ID="headingMergedInto" Level="5" DisplayColon="True" />
    <asp:Label ID="lblMergedIntoAccountName" runat="server" />
    <cms:CMSAccessibleButton ID="btnMergedAccount" runat="server" IconOnly="true" />
</asp:Panel>
<cms:CMSUpdatePanel runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
  <ContentTemplate>
    <cms:UIForm runat="server" ID="EditForm" ObjectType="OM.Account"
        OnOnAfterDataLoad="EditForm_OnAfterDataLoad" OnOnAfterSave="EditForm_OnAfterSave" IsLiveSite="false">
    </cms:UIForm>
  </ContentTemplate>
</cms:CMSUpdatePanel>

<cms:AnchorDropup runat="server" ID="anchorDropup" MinimalAnchors="2" IsOpened="False" />
