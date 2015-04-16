<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Newsletters_Controls_MySubscriptions" CodeFile="MySubscriptions.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<asp:PlaceHolder runat="server" ID="plcMain">
    <cms:MessagesPlaceHolder runat="server" ID="plcMess" LiveSiteOnly="true" />
    <asp:Label runat="server" ID="lblText" EnableViewState="false" CssClass="InfoLabel" />
    <cms:UniSelector ID="usNewsletters" runat="server" ObjectType="Newsletter.Newsletter"
        SelectionMode="Multiple" ResourcePrefix="MySubscriptions.Newsletterselect" />
    <asp:HiddenField ID="hdnValue" runat="server" EnableViewState="false" />
</asp:PlaceHolder>
