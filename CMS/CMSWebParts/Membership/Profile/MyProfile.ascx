<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Membership_Profile_MyProfile" CodeFile="~/CMSWebParts/Membership/Profile/MyProfile.ascx.cs" %>
<%@ Register Src="~/CMSModules/Membership/Controls/MyProfile.ascx" TagName="MyProfile" TagPrefix="cms" %>

<asp:Label ID="lblError" CssClass="ErrorLabel" runat="server" Visible="false" EnableViewState="false" />
<asp:PlaceHolder id="plcContent" runat="server">
    <cms:MyProfile ID="myProfile" runat="server" />
</asp:PlaceHolder>
