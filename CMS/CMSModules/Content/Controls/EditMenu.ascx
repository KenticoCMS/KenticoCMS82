<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_EditMenu"
    CodeFile="EditMenu.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>

<asp:PlaceHolder ID="plcMenu" runat="server">
    <cms:CMSPanel ID="pnlContainer" ShortID="pC" runat="server">
        <asp:Panel ID="pnlMenu" runat="server" CssClass="header-actions-main">
            <cms:HeaderActions ID="menu" runat="server" />
        </asp:Panel>
        <asp:Label runat="server" ID="lblInfo" Visible="false" EnableViewState="false" />
        <asp:PlaceHolder ID="plcControls" runat="server">
            <asp:Panel ID="pnlRight" runat="server" CssClass="header-actions-additional-indented btn-actions control-group-inline">
                <cms:CMSCheckBox ID="chkEmails" CssClass="action-checkbox dont-check-changes" runat="server" Checked="true" />
                <asp:PlaceHolder ID="plcAdditionalControls" runat="server" Visible="false"></asp:PlaceHolder>
                <asp:PlaceHolder ID="plcDevices" runat="server" Visible="false"></asp:PlaceHolder>
            </asp:Panel>
        </asp:PlaceHolder>
        <div class="Clear">
        </div>
    </cms:CMSPanel>
    <cms:CMSDocumentPanel ID="pnlDoc" runat="server" ShortID="pD" />
    <asp:HiddenField ID="hdnParam" runat="server" EnableViewState="false" />
</asp:PlaceHolder>
