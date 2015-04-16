<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" Title="MassEmails - Recipients"
    Inherits="CMSModules_EmailQueue_MassEmails_Recipients" Theme="Default" CodeFile="MassEmails_Recipients.aspx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" tagname="UniGrid" tagprefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="UniSelectorDialogGridArea">
        <cms:UniGrid ID="gridElem" runat="server" ShortID="g" ObjectType="cms.emailuser" Columns="UserID,LastSendResult,Status" OrderBy="UserID" IsLiveSite="false" ShowActionsMenu="true">
            <GridActions>
                <ug:Action Name="delete" ExternalSourceName="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$general.confirmdelete$" />
            </GridActions>
            <GridColumns>
                <ug:Column Source="UserID" ExternalSourceName="userid" Caption="$emailqueue.recipient$" Wrap="false" CssClass="main-column-100" />
                <ug:Column Source="LastSendResult" ExternalSourceName="result" Name="result" IsText="true" Caption="$EmailQueue.LastSendResult$" Wrap="false">
                    <Tooltip Source="LastSendResult" ExternalSourceName="resulttooltip" />
                </ug:Column>
                <ug:Column Source="Status" ExternalSourceName="status" Name="status" Caption="$EmailQueue.Status$" Wrap="false" />
            </GridColumns>
            <GridOptions DisplayFilter="false" ShowSelection="true" SelectionColumn="UserID" />
        </cms:UniGrid>
    </div>
</asp:Content>