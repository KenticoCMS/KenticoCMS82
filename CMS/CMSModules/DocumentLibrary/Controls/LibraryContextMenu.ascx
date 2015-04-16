<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LibraryContextMenu.ascx.cs"
    Inherits="CMSModules_DocumentLibrary_Controls_LibraryContextMenu" %>
<%@ Register Assembly="CMS.UIControls" Namespace="CMS.UIControls" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Attachments/DirectFileUploader/DirectFileUploader.ascx"
    TagName="DirectFileUploader" TagPrefix="cms" %>
<cms:ContextMenu ID="libraryMenuElem" runat="server" Dynamic="true">
    <asp:Panel runat="server" ID="pnlLibraryMenu" CssClass="LibraryContextMenu" EnableViewState="false">
        <asp:Panel runat="server" ID="pnlEdit" CssClass="Item" Visible="false">
            <asp:Panel runat="server" ID="pnlEditPadding" CssClass="webdav-edit-item">
            </asp:Panel>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlUpload" CssClass="Item" Visible="false">
            <asp:Panel runat="server" ID="pnlUploadPadding">
                <cms:DirectFileUploader ID="updateAttachment" runat="server" InsertMode="false" UploadMode="DirectSingle" EnableAdvancedUploader="false" />
            </asp:Panel>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlLocalize" CssClass="Item" Visible="false">
            <asp:Panel runat="server" ID="pnlLocalizePadding" CssClass="ItemPadding">
                <cms:LocalizedLabel runat="server" ID="lblLocalize" CssClass="Name" EnableViewState="false" ResourceString="LibraryContextMenu.Localize" />
            </asp:Panel>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlSep1" CssClass="Separator" />
        <asp:Panel runat="server" ID="pnlCopy" CssClass="Item" Visible="false">
            <asp:Panel runat="server" ID="pnlCopyPadding" CssClass="ItemPadding">
                <cms:LocalizedLabel runat="server" ID="lblCopy" CssClass="Name" EnableViewState="false" ResourceString="general.copy" />
            </asp:Panel>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlDelete" CssClass="Item" Visible="false">
            <asp:Panel runat="server" ID="pnlDeletePadding" CssClass="ItemPadding">
                <cms:LocalizedLabel runat="server" ID="lblDelete" CssClass="Name" EnableViewState="false" ResourceString="general.delete" />
            </asp:Panel>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlOpen" CssClass="Item" Visible="false">
            <asp:Panel runat="server" ID="pnlOpenPadding" CssClass="ItemPadding">
                <cms:LocalizedLabel runat="server" ID="lblOpen" CssClass="Name" EnableViewState="false" ResourceString="general.open" />
            </asp:Panel>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlSep2" CssClass="Separator" />
        <asp:Panel runat="server" ID="pnlProperties" CssClass="Item" Visible="false">
            <asp:Panel runat="server" ID="pnlPropertiesPadding" CssClass="ItemPadding">
                <cms:LocalizedLabel runat="server" ID="lblProperties" CssClass="Name" EnableViewState="false" ResourceString="general.properties" />
            </asp:Panel>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlPermissions" CssClass="Item" Visible="false">
            <asp:Panel runat="server" ID="pnlPermissionsPadding" CssClass="ItemPadding">
                <cms:LocalizedLabel runat="server" ID="lblPermissions" CssClass="Name" EnableViewState="false" ResourceString="general.permissions" />
            </asp:Panel>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlVersionHistory" CssClass="Item" Visible="false">
            <asp:Panel runat="server" ID="pnlVersionHistoryPadding" CssClass="ItemPadding">
                <cms:LocalizedLabel runat="server" ID="lblVersionHistory" CssClass="Name" EnableViewState="false" ResourceString="LibraryContextMenu.VersionHistory" />
            </asp:Panel>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlSep3" CssClass="Separator" />
        <asp:Panel runat="server" ID="pnlCheckOut" CssClass="Item" Visible="false">
            <asp:Panel runat="server" ID="pnlCheckOutPadding" CssClass="ItemPadding">
                <cms:LocalizedLabel runat="server" ID="lblCheckOut" CssClass="Name" EnableViewState="false" ResourceString="general.checkout" />
            </asp:Panel>
        </asp:Panel>
        <cms:ContextMenuContainer runat="server" ID="cmcCheck">
            <asp:Panel runat="server" ID="pnlCheckIn" CssClass="Item" Visible="false">
                <asp:Panel runat="server" ID="pnlCheckInPadding" CssClass="ItemPadding">
                    <cms:LocalizedLabel runat="server" ID="lblCheckIn" CssClass="Name" EnableViewState="false" ResourceString="general.checkin" />
                </asp:Panel>
            </asp:Panel>
        </cms:ContextMenuContainer>
        <asp:Panel runat="server" ID="pnlUndoCheckout" CssClass="Item" Visible="false">
            <asp:Panel runat="server" ID="pnlUndoCheckoutPadding" CssClass="ItemPadding">
                <cms:LocalizedLabel runat="server" ID="lblUndoCheckout" CssClass="Name" EnableViewState="false" ResourceString="general.undocheckout" />
            </asp:Panel>
        </asp:Panel>
        <cms:ContextMenuContainer runat="server" ID="cmcApp">
            <asp:Panel runat="server" ID="pnlSubmitToApproval" CssClass="Item" Visible="false">
                <asp:Panel runat="server" ID="pnlSubmitToApprovalPadding" CssClass="ItemPadding">
                    <cms:LocalizedLabel runat="server" ID="lblSubmitToApproval" CssClass="Name" EnableViewState="false" ResourceString="LibraryContextMenu.SubmitToApproval" />
                </asp:Panel>
            </asp:Panel>
        </cms:ContextMenuContainer>
        <cms:ContextMenuContainer runat="server" ID="cmcRej">
            <asp:Panel runat="server" ID="pnlReject" CssClass="Item" Visible="false">
                <asp:Panel runat="server" ID="pnlRejectPadding" CssClass="ItemPadding">
                    <cms:LocalizedLabel runat="server" ID="lblReject" CssClass="Name" EnableViewState="false" ResourceString="general.reject" />
                </asp:Panel>
            </asp:Panel>
        </cms:ContextMenuContainer>
        <cms:ContextMenuContainer runat="server" ID="cmcArch">
            <asp:Panel runat="server" ID="pnlArchive" CssClass="item-last" Visible="false">
                <asp:Panel runat="server" ID="pnlArchivePadding" CssClass="ItemPadding">
                    <cms:LocalizedLabel runat="server" ID="lblArchive" CssClass="Name" EnableViewState="false" ResourceString="general.archive" />
                </asp:Panel>
            </asp:Panel>
        </cms:ContextMenuContainer>
        <asp:Panel runat="server" ID="pnlNoAction" CssClass="item-last" Visible="false">
            <asp:Panel runat="server" ID="pnlNoActionPadding" CssClass="ItemPadding">
                <cms:LocalizedLabel runat="server" ID="lblNoAction" CssClass="Name" EnableViewState="false"
                    ResourceString="documentlibrary.noaction" />
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</cms:ContextMenu>
<cms:ContextMenu runat="server" ID="menuComm" VerticalPosition="Bottom"
    HorizontalPosition="Left" ActiveItemCssClass="ItemSelected" MenuLevel="1"
    ShowMenuOnMouseOver="true" Dynamic="true">
    <asp:Panel runat="server" ID="pnlCommMenu" CssClass="LibraryContextMenu" EnableViewState="false">
        <asp:Panel runat="server" ID="pnlComment" CssClass="item-last">
            <asp:Panel runat="server" ID="pnlCommentPadding" CssClass="ItemPadding">
                <cms:LocalizedLabel runat="server" ID="lblComment" CssClass="Name" EnableViewState="false" />
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</cms:ContextMenu>