<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CMSAPIExamples_Code_Tools_Events_Default" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Event (booking system) page --%>
    <cms:LocalizedHeading ID="headCreateEvent" runat="server" Text="Event" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateEvent" runat="server" ButtonText="Create event" InfoMessage="Event 'My new event' was created." />
    <cms:APIExample ID="apiGetAndUpdateEvent" runat="server" ButtonText="Get and update event" APIExampleType="ManageAdditional" InfoMessage="Event 'My new event' was updated." ErrorMessage="Event 'My new event' was not found." />

    <%-- Event attendee --%>
    <cms:LocalizedHeading ID="headCreateAttendee" runat="server" Text="Event attendee" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateAttendee" runat="server" ButtonText="Create attendee" InfoMessage="Attendee 'My new attendee' was created." ErrorMessage="Event 'My new event' was not found." />
    <cms:APIExample ID="apiGetAndUpdateAttendee" runat="server" ButtonText="Get and update attendee" APIExampleType="ManageAdditional" InfoMessage="Attendee 'My new attendee' was updated." ErrorMessage="Attendee 'My new attendee' or event 'My new event' were not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateAttendees" runat="server" ButtonText="Get and bulk update attendees" APIExampleType="ManageAdditional" InfoMessage="All attendees matching the condition were updated." ErrorMessage="Attendees matching the condition or event 'My new event' were not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Event attendee --%>
    <cms:LocalizedHeading ID="headDeleteAttendee" runat="server" Text="Event attendee" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteAttendee" runat="server" ButtonText="Delete attendee" APIExampleType="CleanUpMain" InfoMessage="Attendee 'My new attendee' and all its dependencies were deleted." ErrorMessage="Attendee 'My new attendee' or event 'My new event' were not found." />

    <%-- Event (booking system) page --%>
    <cms:LocalizedHeading ID="headDeleteEvent" runat="server" Text="Event" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteEvent" runat="server" ButtonText="Delete event" APIExampleType="CleanUpMain" InfoMessage="Event 'My new event' and all its dependencies were deleted." ErrorMessage="Event 'My new event' was not found." />
</asp:Content>
