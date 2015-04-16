<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ContactManagement_Controls_UI_Contact_Edit"
    CodeFile="Edit.ascx.cs" %>
<%@ Register TagPrefix="cms" TagName="AnchorDropup" Src="~/CMSAdminControls/UI/PageElements/AnchorDropup.ascx" %>

<asp:Panel ID="panelMergedContactDetails" runat="server" CssClass="contactmanagement-contactdetail-contactmergedinto">
    <cms:LocalizedHeading runat="server" ID="headingMergedInto" Level="5" DisplayColon="True" />
    <asp:Label ID="lblMergedIntoContactName" runat="server" />
    <cms:CMSAccessibleButton ID="btnMergedContact" runat="server" IconOnly="true" />
</asp:Panel>

<cms:UIForm runat="server" ID="EditForm" ObjectType="OM.Contact" OnOnAfterSave="EditForm_OnAfterSave"
    OnOnBeforeDataLoad="EditForm_OnBeforeDataLoad" OnOnAfterDataLoad="EditForm_OnAfterDataLoad" IsLiveSite="false">
</cms:UIForm>
<cms:AnchorDropup runat="server" ID="anchorDropup" MinimalAnchors="2" IsOpened="False" />