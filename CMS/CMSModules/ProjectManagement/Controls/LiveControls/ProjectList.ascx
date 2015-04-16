<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ProjectManagement_Controls_LiveControls_ProjectList" CodeFile="ProjectList.ascx.cs" %>
<%@ Register Src="~/CMSModules/ProjectManagement/Controls/UI/Project/List.ascx" TagName="ProjectList"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ProjectManagement/Controls/UI/Project/Edit.ascx" TagName="ProjectNew"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ProjectManagement/Controls/LiveControls/ProjectListEdit.ascx"
    TagName="ProjectEdit" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/BreadCrumbs.ascx" TagName="Breadcrumbs" TagPrefix="cms" %>

<asp:PlaceHolder runat="server" ID="plcProjectList">
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnlBody" runat="server" CssClass="PageBody project-list">
                <asp:PlaceHolder ID="plcList" runat="server">
                    <asp:Panel ID="pnlListActions" runat="server" CssClass="Actions">
                        <cms:HeaderActions ID="actionsElem" runat="server" />
                    </asp:Panel>
                    <asp:Panel ID="pnlListContent" runat="server">
                        <cms:LocalizedLabel runat="server" ID="lblError" CssClass="ErrorLabel" EnableViewState="false"
                            Visible="false" />
                        <cms:ProjectList ID="ucProjectList" runat="server" />
                    </asp:Panel>
                </asp:PlaceHolder>
            </asp:Panel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdateModal" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:ModalPopupDialog runat="server" ID="ucPopupDialog" BackgroundCssClass="ModalBackground"
                CssClass="ModalPopupDialog" CancelControlID="btnCancel">
                <asp:Panel ID="pnlDialogBody" runat="server" CssClass="DialogPageBody">
                    <asp:Panel ID="pnlHeader" runat="server" CssClass="PageHeader" EnableViewState="false">
                        <cms:PageTitle ID="titleElem" runat="server" />
                    </asp:Panel>
                    <asp:Panel ID="pnlContent" runat="server" CssClass="DialogPageContent">
                        <cms:ProjectNew ID="ucProjectNew" DisableOnSiteValidators="true" ShowOKButton="false"
                            ShowPageSelector="false" runat="server" IsLiveSite="true" />
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlFooterContainer">
                        <div class="PageFooterLine">
                            <asp:Panel runat="server" ID="pnlFooter" CssClass="Buttons">
                                <cms:LocalizedButton runat="server" ID="btnOK" ResourceString="general.ok" OnClick="btnOK_Click"
                                    ButtonStyle="Primary" />
                                <cms:LocalizedButton runat="server" OnClientClick="return false;" ID="btnCancel"
                                    ResourceString="general.cancel" ButtonStyle="Primary" />
                            </asp:Panel>
                        </div>
                    </asp:Panel>
                </asp:Panel>
            </cms:ModalPopupDialog>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:PlaceHolder>
<asp:PlaceHolder ID="plcEdit" runat="server">
    <asp:Panel ID="pnlEditHeader" runat="server">
        <cms:Breadcrumbs ID="ucBreadcrumbs" runat="server" HideBreadcrumbs="false" EnableViewState="false" PropagateToMainNavigation="false" />
    </asp:Panel>
    <cms:ProjectEdit ID="ucProjectEdit" runat="server" IsLiveSite="true" />
</asp:PlaceHolder>
