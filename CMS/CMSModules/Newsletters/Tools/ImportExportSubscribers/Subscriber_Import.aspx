<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_ImportExportSubscribers_Subscriber_Import"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Tools - Newsletter subscribers"
    CodeFile="Subscriber_Import.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>

<%@ Register Src="~/CMSAdminControls/AsyncLogDialog.ascx" TagName="AsyncLog"
    TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcBeforeBody" runat="server" ID="cntBeforeBody">
    <asp:Panel runat="server" ID="pnlLog" Visible="false">
        <cms:AsyncLog ID="ctlAsyncLog" runat="server" />
    </asp:Panel>
</asp:Content>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <asp:Panel runat="server" ID="pnlContent" EnableViewState="false">
        <cms:MessagesPlaceHolder ID="plcMessages" runat="server" IsLiveSite="false" />
        <div class="content-block-50">
            <cms:LocalizedHeading runat="server" ID="headActions" Level="4" ResourceString="Subscriber_Import.lblActions" EnableViewState="false" />
            <div class="radio-list-vertical">
                <cms:CMSRadioButton ID="radSubscribe" runat="server" GroupName="ImportSubscribers"
                    ResourceString="Subscriber_Import.SubscribeImported" Checked="true" />
                <div class="checkbox-list-vertical selector-subitem">
                    <cms:CMSCheckBox ID="chkDoNotSubscribe" runat="server" ResourceString="Subscriber_Import.DoNotSubscribe"
                        CssClass="UnderRadioContent" />
                </div>
                <cms:CMSRadioButton ID="radUnsubscribe" runat="server" GroupName="ImportSubscribers"
                    ResourceString="Subscriber_Import.UnsubscribeImported" />
                <cms:CMSRadioButton ID="radDelete" runat="server" GroupName="ImportSubscribers"
                    ResourceString="Subscriber_Import.DeleteImported" />
            </div>
        </div>
        <div class="content-block-50">
            <cms:LocalizedHeading runat="server" ID="headSubscribers" Level="4" ResourceString="Subscriber_Import.lblImportedSub" EnableViewState="false" />
            <div class="content-block-25">
                <cms:LocalizedLabel ID="lblNote" runat="server" EnableViewState="false"
                    ResourceString="Subscriber_Import.lblNote" />
            </div>
            <cms:CMSTextArea ID="txtImportSub" runat="server" Rows="11" />
        </div>
        <div class="content-block-50">
            <cms:LocalizedHeading runat="server" ID="headNewsletters" Level="4" ResourceString="Subscriber_Import.lblSelectSub" EnableViewState="false" />
            <cms:UniSelector ID="usNewsletters" runat="server" IsLiveSite="false" ObjectType="Newsletter.Newsletter"
                SelectionMode="Multiple" ResourcePrefix="newsletterselect" />
        </div>
        <div class="content-block-50">
            <div class="checkbox-list-vertical">
                <cms:CMSCheckBox ID="chkSendConfirmation" runat="server" ResourceString="Subscriber_Edit.SendConfirmation" />
                <cms:CMSCheckBox ID="chkRequireOptIn" runat="server" ResourceString="newsletter.requireoptin" />
            </div>
        </div>
        <div class="content-block-50">
            <cms:LocalizedButton ID="btnImport" runat="server" ButtonStyle="Primary" EnableViewState="false"
                ResourceString="general.start" OnClick="btnImport_Click" />
        </div>
    </asp:Panel>
</asp:Content>
