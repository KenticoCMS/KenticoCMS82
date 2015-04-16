<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DocumentLibrary.ascx.cs"
    Inherits="CMSModules_DocumentLibrary_Controls_DocumentLibrary" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/DocumentLibrary/Controls/CopyDocument.ascx" TagName="CopyDocument"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/DocumentLibrary/Controls/DeleteDocument.ascx" TagName="DeleteDocument"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/DocumentLibrary/Controls/LibraryContextMenu.ascx"
    TagName="LibraryContextMenu" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/VersionList.ascx" TagName="VersionList"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Security.ascx" TagName="Security"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/UserContributions/EditForm.ascx"
    TagName="EditForm" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Attachments/DirectFileUploader/DirectFileUploader.ascx"
    TagName="DirectFileUploader" TagPrefix="cms" %>
<asp:Panel ID="pnlContent" runat="server" CssClass="DocumentLibrary cms-bootstrap">
    <cms:LocalizedLabel ID="lblError" runat="server" EnableViewState="false" CssClass="ErrorLabel"
        Visible="false" />
    <cms:CMSUpdatePanel ID="pnlUpdateHeader" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnlHeader" CssClass="DocumentLibraryHeader" runat="server">
                <cms:DirectFileUploader ID="uploadAttachment" runat="server" InsertMode="true" UploadMode="DirectSingle" />
                <cms:LocalizedButton ID="btnPermissions" runat="server" ResourceString="documentlibrary.librarypermissions"
                    OnClick="lnkPermissions_Click" EnableViewState="false" ButtonStyle="Default" />
            </asp:Panel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:CMSDocumentManager ID="docMan" runat="server" Mode="Update" IsLiveSite="true" />
            <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
            <cms:UniGrid ID="gridDocuments" runat="server" GridName="~/CMSModules/DocumentLibrary/Controls/DocumentLibrary.xml"
                EnableViewState="true" />
            <cms:LibraryContextMenu ID="arrowContextMenu" runat="server" MouseButton="Both" HorizontalPosition="Left"
                VerticalPosition="Top" OffsetY="20" ActiveItemOffset="3" />
            <cms:LibraryContextMenu ID="rowContextMenu" runat="server" MouseButton="Right" ActiveItemOffset="3" />
            <cms:ModalPopupDialog runat="server" ID="mdlLocalize" BackgroundCssClass="ModalBackground"
                CssClass="ModalPopupDialog">
                <asp:Panel ID="pnlLocalizePopup" runat="server" Visible="false">
                    <div class="DialogPageBody">
                        <div style="height: auto; min-height: 0px;">
                            <div class="PageHeader">
                                <cms:PageTitle ID="localizeTitle" runat="server" EnableViewState="false" />
                            </div>
                        </div>
                        <div class="DialogPageContent DialogScrollableContent">
                            <div class="PageBody">
                                <cms:EditForm ID="localizeElem" runat="server" AllowDelete="false" Action="edit"
                                    UseProgressScript="false" />
                            </div>
                        </div>
                        <div class="PageFooterLine">
                            <div class="Buttons FloatRight">
                                <cms:LocalizedButton ID="btnLocalizeSaveClose" runat="server" ButtonStyle="Primary"
                                    ResourceString="documentlibrary.saveandclose" OnClick="btnLocalizeSaveClose_Click" />
                                <cms:LocalizedButton ID="btnClose1" runat="server" ButtonStyle="Primary" CausesValidation="false"
                                    ResourceString="general.close" OnClick="btnClose_Click" />
                            </div>
                            <div class="ClearBoth">
                                &nbsp;
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </cms:ModalPopupDialog>
            <cms:ModalPopupDialog runat="server" ID="mdlCopy" BackgroundCssClass="ModalBackground"
                CssClass="ModalPopupDialog">
                <asp:Panel ID="pnlCopyPopup" runat="server" Visible="false">
                    <div class="DialogPageBody">
                        <div style="height: auto; min-height: 0px;">
                            <div class="PageHeader">
                                <cms:PageTitle ID="copyTitle" runat="server" EnableViewState="false" />
                            </div>
                        </div>
                        <cms:CopyDocument ID="copyElem" runat="server" />
                    </div>
                </asp:Panel>
            </cms:ModalPopupDialog>
            <cms:ModalPopupDialog runat="server" ID="mdlDelete" BackgroundCssClass="ModalBackground"
                CssClass="ModalPopupDialog">
                <asp:Panel ID="pnlDeletePopup" runat="server" Visible="false">
                    <div class="DialogPageBody">
                        <div style="height: auto; min-height: 0px;">
                            <div class="PageHeader">
                                <cms:PageTitle ID="deleteTitle" runat="server" EnableViewState="false" />
                            </div>
                        </div>
                        <cms:DeleteDocument ID="deleteElem" runat="server" />
                    </div>
                </asp:Panel>
            </cms:ModalPopupDialog>
            <cms:ModalPopupDialog runat="server" ID="mdlProperties" BackgroundCssClass="ModalBackground"
                CssClass="ModalPopupDialog">
                <asp:Panel ID="pnlPropertiesPopup" runat="server" Visible="false">
                    <div class="DialogPageBody">
                        <div style="height: auto; min-height: 0px;">
                            <div class="PageHeader">
                                <cms:PageTitle ID="propertiesTitle" runat="server" EnableViewState="false" />
                            </div>
                        </div>
                        <div class="DialogPageContent DialogScrollableContent">
                            <div class="PageBody">
                                <cms:EditForm ID="propertiesElem" runat="server" AllowDelete="false" Action="edit"
                                    UseProgressScript="false" />
                            </div>
                        </div>
                        <div class="PageFooterLine">
                            <div class="Buttons FloatRight">
                                <cms:LocalizedButton ID="btnClose2" runat="server" ButtonStyle="Primary" CausesValidation="false"
                                    ResourceString="general.close" OnClick="btnClose_Click" />
                            </div>
                            <div class="ClearBoth">
                                &nbsp;
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </cms:ModalPopupDialog>
            <cms:ModalPopupDialog runat="server" ID="mdlPermissions" BackgroundCssClass="ModalBackground"
                CssClass="ModalPopupDialog">
                <asp:Panel ID="pnlPermissionsPopup" runat="server" Visible="false">
                    <div class="DialogPageBody dl-permissions-dialog">
                        <div style="height: auto; min-height: 0px;">
                            <div class="PageHeader">
                                <cms:PageTitle ID="permissionsTitle" runat="server" EnableViewState="false" />
                            </div>
                        </div>
                        <div class="DialogPageContent DialogScrollableContent">
                            <div class="PageBody">
                                <cms:Security ID="permissionsElem" runat="server" AllowRedirection="false" DisplaySecurityMessage="true" />
                            </div>
                        </div>
                        <div class="PageFooterLine">
                            <div class="Buttons FloatRight">
                                <cms:LocalizedButton ID="btnSavePermissions" runat="server" ButtonStyle="Primary"
                                    OnClick="btnSavePermissions_Click" ResourceString="general.apply" />
                                <cms:LocalizedButton ID="btnClose3" runat="server" ButtonStyle="Primary" OnClick="btnClose_Click"
                                    ResourceString="general.close" />
                            </div>
                            <div class="ClearBoth">
                                &nbsp;
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </cms:ModalPopupDialog>
            <cms:ModalPopupDialog runat="server" ID="mdlVersions" BackgroundCssClass="ModalBackground"
                CssClass="ModalPopupDialog">
                <asp:Panel ID="pnlVersionsPopup" runat="server" Visible="false">
                    <div class="DialogPageBody">
                        <div style="height: auto; min-height: 0px;">
                            <div class="PageHeader">
                                <cms:PageTitle ID="versionsTitle" runat="server" EnableViewState="false" />
                            </div>
                        </div>
                        <div class="DialogPageContent DialogScrollableContent">
                            <div class="PageBody">
                                <cms:VersionList ID="versionsElem" runat="server" DisplaySecurityMessage="true" />
                            </div>
                        </div>
                        <div class="PageFooterLine">
                            <div class="Buttons FloatRight">
                                <cms:LocalizedButton ID="btnClose4" runat="server" ButtonStyle="Primary" CausesValidation="false"
                                    ResourceString="general.close" OnClick="btnClose_Click" />
                            </div>
                            <div class="ClearBoth">
                                &nbsp;
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </cms:ModalPopupDialog>
            <asp:HiddenField ID="hdnParameter" runat="server" />
            &nbsp;
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Panel>
