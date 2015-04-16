<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Merge_Suggested.aspx.cs"
    Inherits="CMSModules_ContactManagement_Pages_Tools_Account_Merge_Suggested"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Merge suggested accounts"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Account/MergeSuggested.ascx"
    TagName="MergeSuggested" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:MergeSuggested ID="mergeSuggested" runat="server" IsLiveSite="false" />
</asp:Content>
