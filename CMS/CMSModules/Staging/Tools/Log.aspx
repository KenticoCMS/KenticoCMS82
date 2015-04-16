<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Staging_Tools_Log"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Staging - Synchronization log" CodeFile="Log.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Content ID="cntControls" runat="server" ContentPlaceHolderID="plcControls">
    <cms:LocalizedHeading runat="server" ID="lblInfo" Level="4" EnableViewState="false" />
    <cms:LocalizedButton runat="server" ID="btnClear" ButtonStyle="Default" OnClick="btnClear_Click"
        EnableViewState="false" ResourceString="Task.LogClear" />
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid ID="gridLog" runat="server" GridName="SyncLog.xml" OrderBy="SyncLogTime DESC"
        IsLiveSite="false" />
</asp:Content>
