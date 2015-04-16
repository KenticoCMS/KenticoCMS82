<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Membership_Logon_LogonForm"
    CodeFile="~/CMSWebParts/Membership/Logon/LogonForm.ascx.cs" %>
<asp:Panel ID="pnlBody" runat="server" CssClass="LogonPageBackground">
    <table class="DialogPosition">
        <tr style="vertical-align: middle;">
            <td>
                <asp:Login ID="Login1" runat="server" DestinationPageUrl="~/Default.aspx">
                    <LayoutTemplate>
                        <asp:Panel runat="server" ID="pnlLogin" DefaultButton="LoginButton">
                            <table style="border: none;">
                                <tr>
                                    <td class="TopLeftCorner"></td>
                                    <td class="TopMiddleBorder"></td>
                                    <td class="TopRightCorner"></td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <cms:LocalizedLabel ID="lblTokenInfo" runat="server" EnableViewState="False" Visible="false" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3" class="LogonDialog">
                                        <table style="border: none; width: 100%; border-collapse: separate;">
                                            <asp:PlaceHolder runat="server" ID="plcTokenInfo" Visible="false">
                                                <tr>
                                                    <td class="logon-label">
                                                        <cms:LocalizedLabel ID="lblTokenIDlabel" runat="server" CssClass="control-label" ResourceString="mfauthentication.label.token" />
                                                    </td>
                                                    <td class="token-label">
                                                        <cms:LocalizedLabel ID="lblTokenID" runat="server" />
                                                    </td>
                                                </tr>
                                            </asp:PlaceHolder>

                                            <asp:PlaceHolder runat="server" ID="plcLoginInputs">
                                                <tr>
                                                    <td style="white-space: nowrap;">
                                                        <cms:LocalizedLabel ID="lblUserName" runat="server" AssociatedControlID="UserName" CssClass="control-label" />
                                                    </td>
                                                    <td style="white-space: nowrap;">
                                                        <cms:CMSTextBox ID="UserName" runat="server" MaxLength="100" />
                                                        <cms:CMSRequiredFieldValidator ID="rfvUserNameRequired" runat="server" ControlToValidate="UserName"
                                                            ValidationGroup="Login1" Display="Dynamic" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="white-space: nowrap;">
                                                        <cms:LocalizedLabel ID="lblPassword" runat="server" AssociatedControlID="Password" CssClass="control-label"/>
                                                    </td>
                                                    <td>
                                                        <cms:CMSTextBox ID="Password" runat="server" TextMode="Password" MaxLength="110" />
                                                    </td>
                                                </tr>
                                            </asp:PlaceHolder>

                                            <asp:PlaceHolder runat="server" ID="plcPasscodeBox" Visible="false">
                                                <tr>
                                                    <td style="white-space: nowrap;">
                                                        <cms:LocalizedLabel ID="lblPasscode" runat="server" AssociatedControlID="txtPasscode" CssClass="control-label" ResourceString="mfauthentication.label.passcode" />
                                                    </td>
                                                    <td>
                                                        <cms:CMSTextBox ID="txtPasscode" runat="server" MaxLength="110" />
                                                    </td>
                                                </tr>
                                            </asp:PlaceHolder>
                                            <tr>
                                                <td></td>
                                                <td style="text-align: left; white-space: nowrap;">
                                                    <cms:CMSCheckBox ID="chkRememberMe" runat="server" ResourceString="LogonForm.RememberMe" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <cms:LocalizedLabel ID="FailureText" runat="server" EnableViewState="False" CssClass="ErrorLabel" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td style="text-align: left;">
                                                    <cms:LocalizedButton ID="LoginButton" runat="server" ButtonStyle="Primary" CommandName="Login" EnableViewState="false" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </LayoutTemplate>
                </asp:Login>
            </td>
        </tr>
        <tr>
            <td>
                <cms:CMSUpdatePanel runat="server" ID="pnlUpdatePasswordRetrievalLink" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:LinkButton ID="lnkPasswdRetrieval" runat="server" EnableViewState="false"
                            OnClick="lnkPasswdRetrieval_Click" />
                    </ContentTemplate>
                </cms:CMSUpdatePanel>
            </td>
        </tr>
        <tr>
            <td>
                <cms:CMSUpdatePanel runat="server" ID="pnlUpdatePasswordRetrieval" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="pnlPasswdRetrieval" runat="server" CssClass="LoginPanelPasswordRetrieval"
                            DefaultButton="btnPasswdRetrieval" Visible="False">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblPasswdRetrieval" runat="server" EnableViewState="false" AssociatedControlID="txtPasswordRetrieval" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <cms:CMSTextBox ID="txtPasswordRetrieval" runat="server" />
                                        <cms:CMSButton ID="btnPasswdRetrieval" runat="server" EnableViewState="false" ButtonStyle="Default" /><br />
                                        <cms:CMSRequiredFieldValidator ID="rqValue" runat="server" ControlToValidate="txtPasswordRetrieval"
                                            EnableViewState="false" />
                                    </td>
                                </tr>
                            </table>
                            <asp:Label ID="lblResult" runat="server" Visible="false" EnableViewState="false" />
                        </asp:Panel>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger EventName="Click" ControlID="lnkPasswdRetrieval" />
                    </Triggers>
                </cms:CMSUpdatePanel>
            </td>
        </tr>
    </table>
</asp:Panel>
