<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_LiveSelectors_TagSelector"
    Theme="Default" MasterPageFile="~/CMSMasterPages/LiveSite/Dialogs/ModalDialogPage.master"
    Title="Metadata - Select tags" CodeFile="TagSelector.aspx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" tagname="UniGrid" tagprefix="cms" %>


<asp:Content ContentPlaceHolderID="plcContent" runat="Server">
    <asp:Panel ID="pnlTags" runat="server">
        <asp:Panel ID="pnlContent" CssClass="PageContent" runat="server">
            <strong>
                <cms:LocalizedLabel ID="lblInfo" runat="server" CssClass="InfoLabel" ResourceString="tags.tagselector.listold" />
            </strong>
            <cms:UniGrid ID="gridElem" runat="server" IsLiveSite="true" GridName="~/CMSFormControls/Selectors/TagSelector.xml" OrderBy="TagName" ShowActionsMenu="false" />
            <asp:HiddenField ID="hdnValues" runat="server" />
            <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
        </asp:Panel>
    </asp:Panel>
</asp:Content>
<asp:Content ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnOk" runat="server" ButtonStyle="Primary" ResourceString="general.ok"
            EnableViewState="false" /><cms:LocalizedButton ID="btnCancel" runat="server" ButtonStyle="Primary"
                ResourceString="general.cancel" EnableViewState="false" OnClientClick="return CloseDialog();" />
    </div>
</asp:Content>
