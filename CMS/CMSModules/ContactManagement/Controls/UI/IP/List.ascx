<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_Ip_List" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/IP/Filter.ascx" TagPrefix="cms" TagName="IPFilter" %>
<cms:UniGrid runat="server" ID="gridElem" ObjectType="om.ip" OrderBy="IPCreated DESC"
    Columns="IPID,IPAddress,IPCreated,ContactFullName,ContactSiteID,ContactMergedWithContactID"
    IsLiveSite="false" EditActionUrl="Frameset.aspx?ipId={0}" ShowObjectMenu="false" HideFilterButton="true" RememberStateByParam="issitemanager">
    <GridActions Parameters="IPID">
        <ug:Action Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$"
            ExternalSourceName="delete" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="IPAddress" Caption="$om.ip.address$" Wrap="false" />
        <ug:Column Source="IPCreated" Caption="$om.ip.created$" Wrap="false" />
        <ug:Column Source="ContactFullName" Caption="$om.contact.name$" Wrap="false" Name="ContactFullName" />
        <ug:Column Source="ContactSiteID" AllowSorting="false" Caption="$general.sitename$"
            ExternalSourceName="#sitenameorglobal" Name="ContactSiteID" Wrap="false" />
        <ug:Column Source="ContactGUID" Visible="false" />
        <ug:Column CssClass="filling-column" />
    </GridColumns>
    <GridOptions DisplayFilter="true" FilterPath="~/CMSModules/ContactManagement/Controls/UI/IP/Filter.ascx" />
</cms:UniGrid>
