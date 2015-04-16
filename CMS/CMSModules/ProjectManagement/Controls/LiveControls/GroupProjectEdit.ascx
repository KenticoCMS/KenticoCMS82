<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ProjectManagement_Controls_LiveControls_GroupProjectEdit" CodeFile="GroupProjectEdit.ascx.cs" %>
<%@ Register TagPrefix="cms" Namespace="CMS.UIControls" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/ProjectManagement/Controls/UI/Project/Edit.ascx" TagName="ProjectEdit"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ProjectManagement/Controls/LiveControls/GroupTaskEdit.ascx"
    TagPrefix="cms" TagName="TaskProperties" %>
<%@ Register Src="~/CMSModules/ProjectManagement/Controls/UI/Project/Security.ascx"
    TagName="Security" TagPrefix="cms" %>

<cms:BasicTabControl ID="tabControlElem" runat="server" TabControlLayout="Horizontal"
    UseClientScript="true" UsePostback="true" />
<div class="TabBody">
    <asp:Panel ID="pnlTasks" runat="server" CssClass="ProjectManagementEdit">
        <cms:TaskProperties runat="server" ID="ucTaskProperties" IsLiveSite="true" />
    </asp:Panel>
    <asp:Panel ID="pnlProjects" runat="server" CssClass="ProjectManagementEdit">
        <cms:ProjectEdit runat="server" ID="ucProjectEdit" IsLiveSite="true" />
    </asp:Panel>
    <asp:Panel ID="pnlSecurity" runat="server" CssClass="ProjectManagementEdit">
        <cms:Security runat="server" ID="ucSecurity" />
    </asp:Panel>
</div>
<asp:HiddenField runat="server" ID="hdnProjId" />
