<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Newsletters_UnsubscriptionRequest" CodeFile="~/CMSWebParts/Newsletters/UnsubscriptionRequest.ascx.cs" %>
<asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" Visible="false" EnableViewState="false" />
<div class="unsubcription-request">
    <asp:Label ID="lblInfo" runat="server" AssociatedControlID="txtEmail" CssClass="InfoLabel" Visible="false" EnableViewState="false" />
    <cms:CMSTextBox ID="txtEmail" runat="server" CssClass="UnsubscriptionEmail" />
    <cms:CMSButton ID="btnSubmit" runat="server" EnableViewState="false" ButtonStyle="Default" />
</div>
