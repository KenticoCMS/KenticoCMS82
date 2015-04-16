<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MergeChoose.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_Contact_MergeChoose" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Contact/Filter.ascx" TagPrefix="cms" TagName="ContactFilter" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<cms:LocalizedHeading ID="headTitle" runat="server" ResourceString="om.contact.choosemerge" Level="4"
    EnableViewState="false" CssClass="listing-title" DisplayColon="true" />
<cms:UniGrid runat="server" ID="gridElem" ObjectType="om.contactlist" OrderBy="ContactLastName"
    Columns="ContactID,ContactLastName,ContactFirstName,ContactEmail,ContactStatusID,ContactCountryID,ContactSiteID"
    IsLiveSite="false" ShowObjectMenu="false" HideFilterButton="true" RememberState="false">
    <GridColumns>
        <ug:Column Source="ContactFirstName" Caption="$om.contact.firstname$" Wrap="false">
        </ug:Column>
        <ug:Column Source="ContactLastName" Caption="$om.contact.lastname$" Wrap="false">
        </ug:Column>
        <ug:Column Source="ContactEmail" Caption="$general.emailaddress$" Wrap="false">
        </ug:Column>
        <ug:Column Source="ContactStatusID" AllowSorting="false" ExternalSourceName="#transform: om.contactstatus.contactstatusdisplayname"
            Caption="$om.contactstatus$" Wrap="false">
        </ug:Column>
        <ug:Column Source="ContactCountryID" AllowSorting="false" ExternalSourceName="#transform: cms.country.countrydisplayname"
            Caption="$general.country$" Wrap="false">
        </ug:Column>
        <ug:Column Source="ContactSiteID" Name="sitename" AllowSorting="false" ExternalSourceName="#sitenameorglobal"
            Caption="$general.sitename$" Wrap="false" Localize="true">
        </ug:Column>
        <ug:Column Source="ContactGUID" Visible="false" />
        <ug:Column CssClass="filling-column" />
    </GridColumns>
    <GridOptions DisplayFilter="true" ShowSelection="true" FilterPath="~/CMSModules/ContactManagement/Controls/UI/Contact/Filter.ascx" />
</cms:UniGrid>
<asp:Panel ID="pnlButton" runat="server" class="form-horizontal mass-action">
    <div class="form-group">
        <div class="mass-action-value-cell">
            <cms:LocalizedButton ID="btnMergeSelected" runat="server" ButtonStyle="Primary"
                ResourceString="om.contact.mergeselected" />
            <cms:LocalizedButton ID="btnMergeAll" runat="server" ButtonStyle="Default"
                ResourceString="om.contact.mergeall" />
        </div>
    </div>
</asp:Panel>
<asp:HiddenField ID="hdnIdentifier" runat="server" EnableViewState="false" />