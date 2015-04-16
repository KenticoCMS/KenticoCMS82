<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_Pages_Users_General_User_Reject"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" CodeFile="User_Reject.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Literal ID="ltlScript1" runat="server" />
    <cms:LocalizedLabel ID="lblReason" runat="server" DisplayColon="true" ResourceString="administration.users.reason" /><br />
    <cms:CMSTextArea ID="txtReason" runat="server" Rows="19" MaxLength="1000" />
    <br />
    <div>
        <cms:CMSCheckBox ID="chkSendEmail" runat="server" Checked="true" ResourceString="administration.users.email" />
    </div>
    <br />
    <cms:LocalizedButton ID="btnReject" runat="server" ButtonStyle="Default" ResourceString="general.reject" />
    <cms:LocalizedButton ID="btnCancel" runat="server" ButtonStyle="Default" ResourceString="general.cancel" />
    <asp:Literal ID="ltlScript" runat="server" />
</asp:Content>
