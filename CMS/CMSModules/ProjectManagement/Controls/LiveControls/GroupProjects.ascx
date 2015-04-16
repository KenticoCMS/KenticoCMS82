<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ProjectManagement_Controls_LiveControls_GroupProjects" CodeFile="GroupProjects.ascx.cs" %>
<%@ Register Src="~/CMSModules/ProjectManagement/Controls/UI/Project/List.ascx" TagName="ProjectList"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ProjectManagement/Controls/LiveControls/GroupProjectEdit.ascx"
    TagName="ProjectEdit" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ProjectManagement/Controls/UI/Project/Edit.ascx" TagName="ProjectNew"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/BreadCrumbs.ascx" TagName="Breadcrumbs" TagPrefix="cms" %>

<asp:Panel ID="pnlBody" runat="server" CssClass="PageBody">
    <asp:PlaceHolder ID="plcList" runat="server">
        <asp:Panel ID="pnlListActions" runat="server">
            <cms:HeaderActions ID="actionsElem" runat="server" />
        </asp:Panel>
        <asp:Panel ID="pnlListContent" runat="server">
            <cms:ProjectList ID="ucProjectList" runat="server" IsLiveSite="true" />
        </asp:Panel>
    </asp:PlaceHolder>
</asp:Panel>
<asp:Panel ID="plcBreadcrumbs" runat="server" CssClass="PageHeaderLine">
    <cms:Breadcrumbs ID="ucBreadcrumbs" runat="server" HideBreadcrumbs="false" EnableViewState="false" PropagateToMainNavigation="false" />
    <asp:LinkButton ID="lnkBackHidden" runat="server" CausesValidation="false" EnableViewState="false" />
</asp:Panel>
<asp:PlaceHolder ID="plcEdit" runat="server">
    <cms:ProjectEdit ID="ucProjectEdit" runat="server" />
</asp:PlaceHolder>
<asp:PlaceHolder ID="plcNew" runat="server">
    <cms:ProjectNew ID="ucProjectNew" runat="server" IsLiveSite="true" />
</asp:PlaceHolder>
