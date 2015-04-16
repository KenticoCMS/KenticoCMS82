<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_ImportExportSubscribers_Subscriber_Export"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Tools - Newsletter subscribers"
    CodeFile="Subscriber_Export.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:MessagesPlaceHolder ID="plcMessages" runat="server" IsLiveSite="false" />
            <div class="form-group">
                <cms:LocalizedHeading runat="server" ID="headSelectedSubscribes" Level="4" ResourceString="Subscriber_Export.lblSelectSub" EnableViewState="false" />
                <cms:UniSelector ID="usNewsletters" runat="server" IsLiveSite="false" ObjectType="Newsletter.Newsletter"
                    SelectionMode="Multiple" ResourcePrefix="newsletterselect" />
            </div>
            <div class="form-group">
                <cms:LocalizedHeading runat="server" ID="headSelectedExport" Level="4" ResourceString="newsletter.exportsubscribers" DisplayColon="true" EnableViewState="false" />
                <cms:CMSRadioButtonList ID="rblExportList" runat="server" RepeatDirection="Vertical">
                    <asp:ListItem Value="0" />
                    <asp:ListItem Value="1" />
                    <asp:ListItem Value="2" />
                </cms:CMSRadioButtonList>
            </div>
            <div class="form-group">
                <cms:LocalizedButton ID="btnExport" runat="server" ButtonStyle="Primary" EnableViewState="false"
                    ResourceString="Subscriber_Export.btnExport" OnClick="btnExport_Click" />
            </div>
            <cms:LocalizedHeading runat="server" ID="headSubscribers" Level="4" ResourceString="Subscriber_Export.lblExportedSub" EnableViewState="false" />
            <cms:CMSTextArea ID="txtExportSub" runat="server" Rows="19" Enabled="false" EnableViewState="false" />
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnExport" EventName="Click" />
        </Triggers>
    </cms:CMSUpdatePanel>
</asp:Content>
