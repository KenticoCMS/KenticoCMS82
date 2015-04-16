<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PreviewFooter.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_Preview_PreviewFooter" %>
<div class="FloatRight PageFooterLine">
    <cms:CMSButton runat="server" ButtonStyle="Primary" ID="btnSaveAndClose" OnClientClick="if (typeof(actionPerformed) =='function') actionPerformed('saveandclose');return false;" />
</div>
