<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SystemDevelopment_DevMenu" CodeFile="DevMenu.ascx.cs" EnableViewState="false" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<div class="cms-edit-menu">
    <div class="FloatRight">
        <cms:HeaderActions ID="menu" ShortID="m" runat="server" IsLiveSite="false" />
    </div>
</div>
