<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Merge_Split.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_Contact_Merge_Split"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Split merged contacts" EnableEventValidation="false"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Contact/MergeSplit.ascx"
    TagName="MergeSplit" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:MergeSplit ID="mergeSplit" runat="server" IsLiveSite="false" />
</asp:Content>
