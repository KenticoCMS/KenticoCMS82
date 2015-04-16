<%@ Page Language="C#" AutoEventWireup="true"
    Title="Approve friendship" Inherits="CMSModules_Friends_Dialogs_Friends_Approve"
    Theme="default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalSimplePage.master" CodeFile="Friends_Approve.aspx.cs" %>

<%@ Register Src="~/CMSModules/Friends/Controls/Friends_Approve.ascx" TagName="FriendsApprove"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="PageContent">
        <cms:FriendsApprove ID="FriendsApprove" runat="server" IsLiveSite="false" Visible="true" />
    </div>
</asp:Content>
