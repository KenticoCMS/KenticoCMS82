<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Translation submission properties" Inherits="CMSModules_Translations_Pages_Tools_TranslationSubmission_Edit"
    Theme="Default" CodeFile="Edit.aspx.cs" %>

<%@ Register Src="~/CMSModules/Translations/Controls/UI/TranslationSubmission/Edit.ascx"
    TagName="TranslationSubmissionEdit" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:TranslationSubmissionEdit ID="editElem" runat="server" IsLiveSite="false" />
    <asp:Button runat="server" ID="btnShowMessage" EnableViewState="false" CssClass="HiddenButton" />
    <cms:AsyncControl ID="ctlAsync" runat="server" AttachToRunningThread="True" ProcessGUID="f805daba-8df7-4e86-b1dd-84173cb3d6ef" />
</asp:Content>
