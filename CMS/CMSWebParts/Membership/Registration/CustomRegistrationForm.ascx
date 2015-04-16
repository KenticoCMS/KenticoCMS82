<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Membership_Registration_CustomRegistrationForm" CodeFile="~/CMSWebParts/Membership/Registration/CustomRegistrationForm.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/Captcha/SecurityCode.ascx" TagName="SecurityCode" TagPrefix="uc1" %>
<asp:Label ID="lblError" runat="server" EnableViewState="false" Visible="false" />
<asp:Label ID="lblInfo" runat="server" EnableViewState="false" Visible="false" />
<asp:Panel ID="pnlRegForm" runat="server" DefaultButton="btnRegister">
    <cms:DataForm ID="formUser" runat="server" IsLiveSite="true" DefaultFormLayout="SingleTable" />
    <asp:PlaceHolder runat="server" ID="plcCaptcha">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblCaptcha" ResourceString="webparts_membership_registrationform.captcha" />
                </div>
                <div class="editing-form-value-cell">
                    <uc1:SecurityCode ID="captchaElem" runat="server" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
    <cms:CMSButton ID="btnRegister" runat="server" CssClass="RegisterButton" ButtonStyle="Default" />
</asp:Panel>
