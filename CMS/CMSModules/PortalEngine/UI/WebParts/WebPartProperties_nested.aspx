<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_UI_WebParts_WebPartProperties_nested"
    Theme="default" EnableEventValidation="false" ValidateRequest="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    CodeFile="WebPartProperties_nested.aspx.cs" %>

<%@ Register Src="~/CMSModules/PortalEngine/Controls/WebParts/WebpartProperties.ascx"
    TagName="WebpartProperties" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/PortalEngine/Controls/WebParts/SelectWebpart.ascx" TagPrefix="cms" TagName="SelectWebpart" %>

<asp:Content ContentPlaceHolderID="plcSiteSelector" ID="cntSite" runat="server">
    <table>
        <tr>
            <td>
                <strong><cms:localizedlabel runat="server" id="lblNested" displaycolon="True" /></strong>
            </td>
            <td>
                <cms:selectwebpart runat="server" id="selWebPart" DisplayNone="True" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="pnlContent" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Panel runat="server" ID="pnlBody">
        <cms:webpartproperties id="webPartProperties" runat="server" islivesite="false" />
    </asp:Panel>
</asp:Content>
