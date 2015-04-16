<%@ Page Title="Automation" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master" Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_OnlineMarketing_Automation_Default" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Temporary objects --%>
    <cms:LocalizedHeading ID="headCreateTemporaryObjects" runat="server" Text="Preparation" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateTemporaryObjects" runat="server" ButtonText="Create temporary objects" APIExampleType="ManageMain" InfoMessage="Temporary objects were created." ErrorMessage="Temporary objects were not created." />
    <%-- Processes --%>
    <cms:LocalizedHeading ID="headCreateProcess" runat="server" Text="Process" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateProcess" runat="server" ButtonText="Create process" APIExampleType="ManageMain" InfoMessage="Process 'My new process' was created." ErrorMessage="Process 'My new process' was not created." />
    <cms:APIExample ID="apiGetAndUpdateProcess" runat="server" ButtonText="Get and update process" APIExampleType="ManageAdditional" InfoMessage="Process 'My new process' was updated." ErrorMessage="Process 'My new process' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateProcesses" runat="server" ButtonText="Get and bulk update processes" APIExampleType="ManageAdditional" InfoMessage="All processes matching the condition were updated." ErrorMessage="Processes matching the condition were not found." />
    <%-- Steps --%>
    <cms:LocalizedHeading ID="headCreateStep" runat="server" Text="Step" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateProcessStep" runat="server" ButtonText="Create step" InfoMessage="Step 'My new step' was created." ErrorMessage="Process 'My new process' was not found." />
    <cms:APIExample ID="apiGetAndUpdateProcessStep" runat="server" ButtonText="Get and update step" APIExampleType="ManageAdditional" InfoMessage="Step 'My new step' was updated." ErrorMessage="Process 'My new process' or step 'My new step' were not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateProcessSteps" runat="server" ButtonText="Get and bulk update steps" APIExampleType="ManageAdditional" InfoMessage="All steps matching the condition were updated." ErrorMessage="Process 'My new process' or steps matching the condition were not found." />
    <%-- Transitions --%>
    <cms:LocalizedHeading ID="headCreateTransitions" runat="server" Text="Transitions" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateProcessTransitions" runat="server" ButtonText="Create transitions" InfoMessage="Transitions were created." ErrorMessage="Process 'My new process' or step 'My new step' were not found." />
    <%-- Triggers --%>
    <cms:LocalizedHeading ID="headCreateTrigger" runat="server" Text="Trigger" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateProcessTrigger" runat="server" ButtonText="Create trigger" InfoMessage="Trigger 'My new trigger' was created." ErrorMessage="Process 'My new process' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateProcessTriggers" runat="server" ButtonText="Get and update trigger(s)" APIExampleType="ManageAdditional" InfoMessage="All triggers matching the condition were updated." ErrorMessage="Triggers matching the condition were not found." />
    <%-- Automation states --%>
    <cms:LocalizedHeading ID="headManageProcess" runat="server" Text="Manage process" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateAutomationState" runat="server" ButtonText="Start process" APIExampleType="ManageMain" InfoMessage="Process 'My new process' was started." ErrorMessage="Process 'My new process' or contact 'My New Contact' were not found." />
    <cms:APIExample ID="apiMoveContactToNextStep" runat="server" ButtonText="Move to next step" APIExampleType="ManageAdditional" InfoMessage="Contact 'My New Contact' was moved to next step." ErrorMessage="Process 'My new process' or contact 'My New Contact' were not found or process was not started." />
    <cms:APIExample ID="apiMoveContactToPreviousStep" runat="server" ButtonText="Move to previous step" APIExampleType="ManageAdditional" InfoMessage="Contact 'My New Contact' was moved to previous step." ErrorMessage="Process 'My new process' or contact 'My New Contact' were not found or process was not started." />
    <cms:APIExample ID="apiMoveContactToSpecificStep" runat="server" ButtonText="Move to specific step" APIExampleType="ManageAdditional" InfoMessage="Contact 'My New Contact' was moved to specific step." ErrorMessage="Process 'My new process' or contact 'My New Contact' or step 'finished' were not found or process was not started." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Processes --%>
    <cms:LocalizedHeading ID="headDeleteProcess" runat="server" Text="Process" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveContactFromProcess" runat="server" ButtonText="Remove contact from process" APIExampleType="CleanUpMain" InfoMessage="Contact 'My New Contact' was removed from process." ErrorMessage="Process 'My new process' or contact 'My New Contact' were not found or process was not started." />
    <cms:APIExample ID="apiDeleteProcess" runat="server" ButtonText="Delete process" APIExampleType="CleanUpMain" InfoMessage="Process 'My new process' and all its dependencies were deleted." ErrorMessage="Process 'My new process' was not found." />
    <%-- Temporary objects --%>
    <cms:LocalizedHeading ID="headDeleteTemporaryObjects" runat="server" Text="Temporary objects" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteTemporaryObjects" runat="server" ButtonText="Delete temporary objects" APIExampleType="CleanUpMain" InfoMessage="Temporary objects were deleted." ErrorMessage="Temporary objects were not found." />
</asp:Content>
