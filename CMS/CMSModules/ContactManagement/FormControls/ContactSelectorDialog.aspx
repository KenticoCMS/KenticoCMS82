<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ContactSelectorDialog.aspx.cs"
    Inherits="CMSModules_ContactManagement_FormControls_ContactSelectorDialog" Title="Select contact"
    EnableEventValidation="false" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Theme="Default" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ID="content" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:Panel ID="pnlBody" runat="server" CssClass="UniSelectorDialogBody">
        <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="UniSelectorDialogGridArea">
                    <div class="UniSelectorDialogGridPadding">
                        <cms:LocalizedLabel runat="server" ID="lblInfo" CssClass="InfoLabel" EnableViewState="false" ResourceString="om.contact.selectparent" />
                        <cms:UniGrid runat="server" ID="gridElem" ObjectType="om.contactlist" OrderBy="ContactLastName"
                            Columns="ContactID,ContactFullNameJoined" IsLiveSite="false">
                            <GridColumns>
                                <ug:Column ExternalSourceName="ContactFullNameJoined" Source="##ALL##" Caption="$om.contact.name$"
                                    Wrap="false">
                                    <Filter Type="text" Size="100" Source="ContactFullNameJoined" />
                                </ug:Column>
                                <ug:Column CssClass="filling-column" />
                            </GridColumns>
                            <GridOptions DisplayFilter="true" ShowSelection="false" FilterLimit="10" />
                        </cms:UniGrid>
                        <div class="ClearBoth">
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:Panel>
</asp:Content>
