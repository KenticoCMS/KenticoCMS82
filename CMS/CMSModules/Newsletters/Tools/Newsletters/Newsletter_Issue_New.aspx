<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Newsletter_Issue_New.aspx.cs"
    Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_New" Theme="Default"
    EnableEventValidation="false" ValidateRequest="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Newsletter - New issue" %>

<%@ Register Src="~/CMSModules/Newsletters/Controls/EditIssue.ascx" TagPrefix="cms" TagName="EditIssue" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdateProgress ID="up" runat="server" HandlePostback="true" />
    <cms:CMSUpdatePanel runat="server" UpdateMode="Always">
        <ContentTemplate>
            <cms:MessagesPlaceHolder ID="plcMessages" runat="server" UseRelativePlaceHolder="false" IsLiveSite="false" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
   
    <cms:EditIssue ID="editElem" runat="server" ShortID="e" Enabled="true" />
</asp:Content>