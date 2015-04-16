<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_Activity_List" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Activity/Filter.ascx" TagPrefix="cms" TagName="ActivityFilter" %>

<asp:Panel ID="pnlUpdate" runat="server">
    <cms:UniGrid runat="server" ID="gridElem" ObjectType="om.activitylist" Columns="ActivityID,ContactFullNameJoined,ActivityTitle,ActivityType,ActivityCreated,ActivityIPAddress,ActivitySiteID"
        IsLiveSite="false" HideFilterButton="true" RememberStateByParam="issitemanager">
        <GridActions Parameters="ActivityID">
            <ug:Action Name="view" ExternalSourceName="view" Caption="$General.View$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="#delete" ExternalSourceName="delete" CommandArgument="ActivityID"
                Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$"
                ModuleName="CMS.ContactManagement" Permissions="ManageActivities" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="ActivityTitle" Caption="$om.activity.gridtitle$" Wrap="false" />
            <ug:Column Source="ActivityType" ExternalSourceName="acttype" Caption="$general.type$"
                Wrap="false">
                <Tooltip Source="ActivityType" ExternalSourceName="acttypedesc" />
            </ug:Column>
            <ug:Column Source="ContactFullNameJoined" Caption="$om.activity.contactname$" Wrap="false"
                Name="contactname">
            </ug:Column>
            <ug:Column Source="ActivityIPAddress" Caption="$om.activity.ipaddress$" Wrap="false"
                Name="ipaddress" />
            <ug:Column Source="ActivityCreated" Caption="$om.activity.activitytime$" Wrap="false" />
            <ug:Column Source="ActivitySiteID" ExternalSourceName="#sitenameorglobal" AllowSorting="false"
                Caption="$general.sitename$" Wrap="false" Name="sitename" Localize="true">
            </ug:Column>
            <ug:Column Source="ActivityGUID" Visible="false" />
            <ug:Column CssClass="filling-column" />
        </GridColumns>
        <GridOptions ShowSelection="true" DisplayFilter="true" FilterPath="~/CMSModules/ContactManagement/Controls/UI/Activity/Filter.ascx" />
    </cms:UniGrid>
</asp:Panel>
<asp:Panel ID="pnlFooter" runat="server" Visible="false" EnableViewState="false" CssClass="form-horizontal mass-action">
    <div class="form-group">
        <div class="mass-action-value-cell">
            <cms:LocalizedLabel runat="server" AssociatedControlID="drpWhat" CssClass="sr-only" ResourceString="general.scope" />
            <cms:CMSDropDownList ID="drpWhat" runat="server" />
            <cms:LocalizedLabel runat="server" AssociatedControlID="drpAction" CssClass="sr-only" ResourceString="general.action" />
            <cms:CMSDropDownList ID="drpAction" runat="server" />
            <cms:LocalizedButton ID="btnOk" runat="server" ResourceString="general.ok" ButtonStyle="Primary"
                EnableViewState="false" />
        </div>
    </div>
    <asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" EnableViewState="false" />
</asp:Panel>
<asp:HiddenField ID="hdnIdentifier" runat="server" EnableViewState="false" />