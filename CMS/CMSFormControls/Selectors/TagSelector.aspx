<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_Selectors_TagSelector"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Metadata - Select tags" CodeFile="TagSelector.aspx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" tagname="UniGrid" tagprefix="cms" %>


<asp:Content ContentPlaceHolderID="plcContent" runat="Server">
    <asp:Panel ID="pnlTags" runat="server">
        <asp:Panel ID="pnlContent" runat="server">
            <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="tags.tagselector.listold" CssClass="listing-title" EnableViewState="false" />  
            <cms:UniGrid ID="gridElem" runat="server" GridName="TagSelector.xml" IsLiveSite="false" OrderBy="TagName" ShowActionsMenu="false" />
            <asp:HiddenField ID="hdnValues" runat="server" />
            <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
        </asp:Panel>
    </asp:Panel>
</asp:Content>
