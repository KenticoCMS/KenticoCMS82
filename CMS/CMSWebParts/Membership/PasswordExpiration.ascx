<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/CMSWebParts/Membership/PasswordExpiration.ascx.cs"
    Inherits="CMSWebParts_Membership_PasswordExpiration" %>
<%@ Register Src="~/CMSModules/Membership/Controls/PasswordExpiration.ascx" TagPrefix="cms"
    TagName="PasswordExpiration" %>
<cms:PasswordExpiration ID="pwdExp" runat="server" EnableViewState="true" IsLiveSite="true" />
