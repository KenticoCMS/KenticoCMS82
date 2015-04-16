<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ReCaptcha.ascx.cs" Inherits="CMSFormControls_Captcha_ReCaptcha" %>
<%@ Register TagPrefix="cms" Namespace="CMS.UIControls" Assembly="CMS.UIControls" %>

<asp:Panel ID="pnlCaptchaWrap" runat="server">
    <div id="cbCaptcha" style="display: none;"></div>
    <cms:RecaptchaControl ID="captcha" runat="server" />
</asp:Panel>
