<%@ Page Title="" Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    AutoEventWireup="false" CodeFile="PageLayouts.aspx.cs" Inherits="CMSModules_DeviceProfile_Pages_Development_PageLayouts"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/DeviceProfile/Controls/PageLayouts.ascx" TagPrefix="cms" TagName="PageLayouts" %>

<asp:Content ID="Content" ContentPlaceHolderID="plcContent" runat="server">
    <cms:PageLayouts runat="server" ID="PageLayouts" />
</asp:Content>
