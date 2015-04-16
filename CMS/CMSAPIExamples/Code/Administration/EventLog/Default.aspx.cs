using System;
using System.Data;

using CMS.EventLog;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSAPIExamples_Code_Administration_EventLog_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Event log
        apiLogEvent.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(LogEvent);
        apiGetAndUpdateEvent.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateEvent);
        apiGetAndBulkUpdateEvents.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateEvents);
        apiClearLog.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(ClearLog);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Event log
        apiLogEvent.Run();
        apiGetAndUpdateEvent.Run();
        apiGetAndBulkUpdateEvents.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Event log
        apiClearLog.Run();
    }

    #endregion


    #region "API examples - Event log"

    /// <summary>
    /// Log event. Called when the "Log event" button is pressed.
    /// </summary>
    private bool LogEvent()
    {
        EventLogProvider.LogEvent(EventType.INFORMATION, "API Example", "APIEXAMPLE", eventDescription: "My new logged event.");

        return true;
    }


    /// <summary>
    /// Gets and updates abuse report. Called when the "Get and update report" button is pressed.
    /// Expects the LogEvent method to be run first.
    /// </summary>
    private bool GetAndUpdateEvent()
    {
        // Get top 1 event matching the where condition
        string where = "EventCode = 'APIEXAMPLE'";
        int topN = 1;
        DataSet events = EventLogProvider.GetAllEvents(where, null, topN, null);

        if (!DataHelper.DataSourceIsEmpty(events))
        {
            // Create the object from DataRow
            EventLogInfo updateEvent = new EventLogInfo(events.Tables[0].Rows[0]);

            // Update the properties
            updateEvent.EventDescription = updateEvent.EventDescription.ToLower();

            // Save the changes
            EventLogProvider.SetEventLogInfo(updateEvent);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates events. Called when the "Get and bulk update events" button is pressed.
    /// Expects the LogEvent method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateEvents()
    {
        // Get events matching the where condition
        string where = "EventCode = 'APIEXAMPLE'";
        DataSet events = EventLogProvider.GetAllEvents(where, null);

        if (!DataHelper.DataSourceIsEmpty(events))
        {
            // Loop through the individual items
            foreach (DataRow eventDr in events.Tables[0].Rows)
            {
                // Create the object from DataRow
                EventLogInfo updateEvent = new EventLogInfo(eventDr);

                // Update the properties
                updateEvent.EventDescription = updateEvent.EventDescription.ToUpper();

                // Save the changes
                EventLogProvider.SetEventLogInfo(updateEvent);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Clears event log for current site. Called when the "Clear event log" button is pressed.
    /// Expects the CreateAbuseReport method to be run first.
    /// </summary>
    private bool ClearLog()
    {
        // Clear event log for current site
        EventLogProvider.ClearEventLog(MembershipContext.AuthenticatedUser.UserID, MembershipContext.AuthenticatedUser.UserName, RequestContext.UserHostAddress, SiteContext.CurrentSiteID);

        return true;
    }

    #endregion
}