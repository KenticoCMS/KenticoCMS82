<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Tab_ContactGroups.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Contact properties - Contact Groups"
    Inherits="CMSModules_ContactManagement_Pages_Tools_Contact_Tab_ContactGroups"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Contact/ContactGroups.ascx"
    TagName="ContactGroups" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ContactManagement/FormControls/ContactGroupSelector.ascx"
    TagName="GroupSelector" TagPrefix="cms" %>
<asp:Content ID="contentControls" ContentPlaceHolderID="plcActions" runat="server">
    <div class="PageHeaderItem">
        <cms:GroupSelector runat="server" ID="selectGroup" />
    </div>
    <div class="ClearBoth">
        &nbsp;
    </div>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:ContactGroups runat="server" ID="contactGroups" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
