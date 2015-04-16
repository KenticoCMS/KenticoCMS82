<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditImage.aspx.cs" Inherits="CMSModules_PortalEngine_UI_OnSiteEdit_EditImage"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" %>
<%@ Register Src="~/CMSModules/Content/Controls/EditMenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<%@ Register src="~/CMSModules/PortalEngine/Controls/Editable/EditableImage.ascx" tagname="EditableImage" tagprefix="cms" %>


<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <cms:EditableImage ID="ucEditableImage" runat="server" DisplaySelectorTextBox="false" />
</asp:Content>
<asp:Content ID="pnlFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <cms:editmenu ID="menuElem" ShortID="m" runat="server" ShowProperties="false" IsLiveSite="false" />
</asp:Content>