<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Merge_Choose.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_Account_Merge_Choose"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Choose accounts to merge" EnableEventValidation="false"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Account/MergeChoose.ascx"
    TagName="MergeChoose" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:MergeChoose ID="mergeChoose" runat="server" IsLiveSite="false" />
</asp:Content>
