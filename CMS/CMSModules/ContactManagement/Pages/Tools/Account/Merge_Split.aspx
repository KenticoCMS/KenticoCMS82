<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Merge_Split.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_Account_Merge_Split"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Split merged accounts" EnableEventValidation="false"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Account/MergeSplit.ascx"
    TagName="MergeSplit" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:MergeSplit ID="mergeSplit" runat="server" IsLiveSite="false" />
</asp:Content>
