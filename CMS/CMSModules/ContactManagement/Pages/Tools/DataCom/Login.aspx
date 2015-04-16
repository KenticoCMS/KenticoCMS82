<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_DataCom_Login"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/DataCom/ErrorSummary.ascx" TagName="ErrorSummary" TagPrefix="cms" %>

<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ErrorSummary ID="ErrorSummary" runat="server" EnableViewState="false" MessagesEnabled="true" />
    <asp:Panel ID="pnlGeneral" runat="server" CssClass="PageContent">
        <cms:LocalizedHeading ResourceString="datacom.datacomaccount" runat="server" ID="headTitle" Level="4" CssClass="listing-title" EnableViewState="false" />
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblEmail" runat="server" EnableViewState="false" ResourceString="general.emailaddress"
                        DisplayColon="true" AssociatedControlID="txtEmail" ShowRequiredMark="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtEmail" runat="server" EnableViewState="false" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblPassword" runat="server" EnableViewState="false" ResourceString="general.password"
                        DisplayColon="true" AssociatedControlID="txtPassword" ShowRequiredMark="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtPassword" runat="server" EnableViewState="false" TextMode="Password" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-value-cell editing-form-value-cell-offset">
                    <cms:CMSButton ID="btnLogin" runat="server" EnableViewState="false" ButtonStyle="Primary" OnClick="btnLogin_Click" />
                    <span class="form-control-text">
                        <cms:LocalizedLabel ID="lblSignUp" runat="server" EnableViewState="false" ResourceString="datacom.needaccount" />
                        <cms:LocalizedHyperlink ID="linkSignUpHere" runat="server" EnableViewState="false" Target="_blank" ResourceString="datacom.register" />
                    </span>
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
