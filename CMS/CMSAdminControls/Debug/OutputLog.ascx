<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Debug_OutputLog" CodeFile="OutputLog.ascx.cs" %>
<div class="DebugLog">
    <cms:CMSPanel runat="server" EnableViewState="false" ID="pnlHeading" Visible="False" CssClass="LogInfo">
        <cms:LocalizedLabel runat="server" ID="lblHeading" ResourceString="OutputLog.Info" EnableViewState="False" />
    </cms:CMSPanel>
    <cms:CMSPanel runat="server" EnableViewState="false" ID="pnlOutputInfo" CssClass="OutputInfo">
        <cms:LocalizedLabel runat="server" ID="lblSizeCaption" AssociatedControlID="lblSize" ResourceString="OutputLog.Info" EnableViewState="false" />
        <cms:LocalizedLabel runat="server" ID="lblSize" EnableViewState="false" />
    </cms:CMSPanel>
    <cms:CMSTextArea runat="server" ID="txtOutput" ReadOnly="True" Rows="10" EnableViewState="false" CssClass="Output" Wrap="true" />
</div>
