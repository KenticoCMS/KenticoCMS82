<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CMSAPIExamples_Code_Tools_Polls_Default" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Poll --%>
    <cms:LocalizedHeading ID="headCreatePoll" runat="server" Texta="Poll" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreatePoll" runat="server" ButtonText="Create poll" InfoMessage="Poll 'My new poll' was created." />
    <cms:APIExample ID="apiGetAndUpdatePoll" runat="server" ButtonText="Get and update poll" APIExampleType="ManageAdditional" InfoMessage="Poll 'My new poll' was updated." ErrorMessage="Poll 'My new poll' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdatePolls" runat="server" ButtonText="Get and bulk update polls" APIExampleType="ManageAdditional" InfoMessage="All polls matching the condition were updated." ErrorMessage="Polls matching the condition were not found." />
    <%-- Answer --%>
    <cms:LocalizedHeading ID="headCreateAnswer" runat="server" Text="Answer" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateAnswer" runat="server" ButtonText="Create answer" InfoMessage="Answer 'My new answer' was created." ErrorMessage="Poll 'My new poll' was not found." />
    <cms:APIExample ID="apiGetAndUpdateAnswer" runat="server" ButtonText="Get and update answer" APIExampleType="ManageAdditional" InfoMessage="Answer 'My new answer' was updated." ErrorMessage="Answer 'My new answer' or poll 'My new poll' were not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateAnswers" runat="server" ButtonText="Get and bulk update answers" APIExampleType="ManageAdditional" InfoMessage="All answers matching the condition were updated." ErrorMessage="Answers matching the condition or poll 'My new poll' were not found." />
    <%-- Poll on site --%>
    <cms:LocalizedHeading ID="headAddPollToSite" runat="server" Text="Poll on site" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiAddPollToSite" runat="server" ButtonText="Add poll to site" InfoMessage="Poll 'My new poll' was added to current site." ErrorMessage="Poll 'My new poll' was not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Poll on site --%>
    <cms:LocalizedHeading ID="headRemovePollFromSite" runat="server" Text="Poll on site" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiRemovePollFromSite" runat="server" ButtonText="Remove poll from site" APIExampleType="CleanUpMain" InfoMessage="Poll 'My new poll' was removed from current site." ErrorMessage="Poll 'My new poll' was not found." />
    <%-- Answer --%>
    <cms:LocalizedHeading ID="headDeleteAnswer" runat="server" Text="Answer" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteAnswer" runat="server" ButtonText="Delete answer" APIExampleType="CleanUpMain" InfoMessage="Answer 'My new answer' and all its dependencies were deleted." ErrorMessage="Answer 'My new answer' or poll 'My new poll' were not found." />
    <%-- Poll --%>
    <cms:LocalizedHeading ID="headDeletePoll" runat="server" Text="Poll" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeletePoll" runat="server" ButtonText="Delete poll" APIExampleType="CleanUpMain" InfoMessage="Poll 'My new poll' and all its dependencies were deleted." ErrorMessage="Poll 'My new poll' was not found." />
</asp:Content>
