<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Viewers_Effects_lightbox" CodeFile="~/CMSWebParts/Viewers/Effects/lightbox.ascx.cs" %>
<%@ Register TagPrefix="cms" Namespace="CMS.Controls" Assembly="CMS.Controls" %>
<asp:Literal ID="ltlScript" runat="server" />
<cms:LightboxExtender ID="extLightbox" runat="server" />
<cms:CMSRepeater ID="repItems" runat="server" EnableViewState="true" />
<asp:Literal ID="ltlInitResponse" runat="server" />
