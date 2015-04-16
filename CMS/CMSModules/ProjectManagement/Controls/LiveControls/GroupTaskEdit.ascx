<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_ProjectManagement_Controls_LiveControls_GroupTaskEdit" CodeFile="GroupTaskEdit.ascx.cs" %>
<%@ Register Src="~/CMSModules/ProjectManagement/Controls/UI/ProjectTask/List.ascx"
    TagPrefix="cms" TagName="TaskList" %>
<%@ Register Src="~/CMSModules/ProjectManagement/Controls/UI/ProjectTask/Edit.ascx"
    TagPrefix="cms" TagName="TaskEdit" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<%@ Register TagPrefix="cms" Namespace="CMS.ExtendedControls" Assembly="CMS.ExtendedControls" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/BreadCrumbs.ascx" TagName="Breadcrumbs" TagPrefix="cms" %>

<asp:Panel ID="plcList" runat="server">
    <asp:Panel ID="pnlListActions" runat="server">
        <cms:HeaderActions ID="actionsElem" runat="server" />
    </asp:Panel>
    <cms:TaskList IgnoreCommunityGroup="true" ID="ucTaskList" runat="server" IsLiveSite="true" />
</asp:Panel>
<asp:Panel ID="pnlEdit" runat="server">
    <asp:Panel ID="pnlEditHeader" runat="server" CssClass="PageHeaderLine">
        <cms:Breadcrumbs ID="ucBreadcrumbs" runat="server" HideBreadcrumbs="false" EnableViewState="false" PropagateToMainNavigation="false" />
        <asp:LinkButton ID="lnkBackHidden" runat="server" CausesValidation="false" EnableViewState="false" />
    </asp:Panel>
    <cms:TaskEdit ID="ucTaskEdit" runat="server" DelayedReload="true" IsLiveSite="true" />
</asp:Panel>

