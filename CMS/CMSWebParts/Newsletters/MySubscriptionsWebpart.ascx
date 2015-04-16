<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Newsletters_MySubscriptionsWebpart" CodeFile="~/CMSWebParts/Newsletters/MySubscriptionsWebpart.ascx.cs" %>
<%@ Register Src="~/CMSModules/Newsletters/Controls/MySubscriptions.ascx" TagName="MySubscriptions" TagPrefix="cms" %>
<cms:MySubscriptions id="ucMySubsriptions" runat="server" IsLiveSite="true" />
