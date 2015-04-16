<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_Edit"
    ValidateRequest="false" EnableEventValidation="false" Theme="Default" CodeFile="Newsletter_Issue_Edit.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSModules/Newsletters/Controls/EditIssue.ascx" TagPrefix="cms"
    TagName="EditIssue" %>
<%@ Register Src="~/CMSModules/Newsletters/Controls/VariantSlider.ascx" TagPrefix="cms"
    TagName="VariantSlider" %>
<%@ Register Src="~/CMSModules/Newsletters/Controls/VariantDialog.ascx" TagPrefix="cms"
    TagName="VariantDialog" %>

<asp:Content ID="cntControls" runat="server" ContentPlaceHolderID="plcControls">
    <cms:VariantSlider ID="ucVariantSlider" runat="server" ShortID="vs" />
</asp:Content>
<asp:Content ID="cntBeforeContent" runat="server" ContentPlaceHolderID="plcBeforeContent">
    <cms:VariantDialog ID="ucVariantDialog" runat="server" ShortID="vd" />
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <%-- Edit issue dialog --%>
    <cms:EditIssue ID="editElem" runat="server" ShortID="e" />
</asp:Content>