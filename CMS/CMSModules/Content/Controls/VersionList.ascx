<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VersionList.ascx.cs" Inherits="CMSModules_Content_Controls_VersionList" %>
<%@ Register TagPrefix="cms" TagName="UniGrid" Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" %>
<table width="100%">
    <asp:PlaceHolder ID="plcLabels" runat="server">
        <tr>
            <td colspan="2">
                <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
            </td>
        </tr>
    </asp:PlaceHolder>
    <tr>
        <td>
            <cms:LocalizedHeading ID="headHistory" runat="server" Level="4" ResourceString="VersionsProperties.History"
                EnableViewState="false" />
        </td>
        <td class="TextRight">
            <cms:LocalizedButton ID="btnDestroy" runat="server" ButtonStyle="Default" OnClick="btnDestroy_Click"
                OnClientClick="return confirm(varConfirmDestroy);" ResourceString="VersionsProperties.Clear" />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <cms:UniGrid ID="gridHistory" runat="server" OrderBy="VersionHistoryID DESC" />
        </td>
    </tr>
</table>
