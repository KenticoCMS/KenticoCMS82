using System;
using System.Data;
using CMS.Helpers;
using CMS.Globalization;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using TimeZoneInfo = CMS.Globalization.TimeZoneInfo;

public partial class CMSAPIExamples_Code_Development_TimeZones_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Timezone
        apiCreateTimezone.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateTimezone);
        apiGetAndUpdateTimezone.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateTimezone);
        apiGetAndBulkUpdateTimezones.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateTimezones);
        apiDeleteTimezone.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteTimezone);
        apiConvertTime.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(ConvertTime);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Timezone
        apiCreateTimezone.Run();
        apiGetAndUpdateTimezone.Run();
        apiGetAndBulkUpdateTimezones.Run();
        apiConvertTime.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Timezone
        apiDeleteTimezone.Run();
    }

    #endregion


    #region "API examples - Timezone"

    /// <summary>
    /// Creates timezone. Called when the "Create timezone" button is pressed.
    /// </summary>
    private bool CreateTimezone()
    {
        // Create new timezone object
        TimeZoneInfo newTimezone = new TimeZoneInfo();

        // Set the properties
        newTimezone.TimeZoneDisplayName = "My new timezone";
        newTimezone.TimeZoneName = "MyNewTimezone";
        newTimezone.TimeZoneGMT = -12;
        newTimezone.TimeZoneDaylight = true;
        newTimezone.TimeZoneRuleStartRule = "MAR|SUN|1|LAST|3|0|1";
        newTimezone.TimeZoneRuleEndRule = "OCT|SUN|1|LAST|3|0|0";

        // Save the timezone
        TimeZoneInfoProvider.SetTimeZoneInfo(newTimezone);

        return true;
    }


    /// <summary>
    /// Gets and updates timezone. Called when the "Get and update timezone" button is pressed.
    /// Expects the CreateTimezone method to be run first.
    /// </summary>
    private bool GetAndUpdateTimezone()
    {
        // Get the timezone
        TimeZoneInfo updateTimezone = TimeZoneInfoProvider.GetTimeZoneInfo("MyNewTimezone");
        if (updateTimezone != null)
        {
            // Update the properties
            updateTimezone.TimeZoneDisplayName = updateTimezone.TimeZoneDisplayName.ToLower();

            // Save the changes
            TimeZoneInfoProvider.SetTimeZoneInfo(updateTimezone);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates timezones. Called when the "Get and bulk update timezones" button is pressed.
    /// Expects the CreateTimezone method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateTimezones()
    {
        // Prepare the parameters
        string where = "TimeZoneName LIKE N'MyNewTimezone%'";

        // Get the data
        DataSet timezones = TimeZoneInfoProvider.GetTimeZones(where, null);
        if (!DataHelper.DataSourceIsEmpty(timezones))
        {
            // Loop through the individual items
            foreach (DataRow timezoneDr in timezones.Tables[0].Rows)
            {
                // Create object from DataRow
                TimeZoneInfo modifyTimezone = new TimeZoneInfo(timezoneDr);

                // Update the properties
                modifyTimezone.TimeZoneDisplayName = modifyTimezone.TimeZoneDisplayName.ToUpper();

                // Save the changes
                TimeZoneInfoProvider.SetTimeZoneInfo(modifyTimezone);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes timezone. Called when the "Delete timezone" button is pressed.
    /// Expects the CreateTimezone method to be run first.
    /// </summary>
    private bool DeleteTimezone()
    {
        // Get the timezone
        TimeZoneInfo deleteTimezone = TimeZoneInfoProvider.GetTimeZoneInfo("MyNewTimezone");

        // Delete the timezone
        TimeZoneInfoProvider.DeleteTimeZoneInfo(deleteTimezone);

        return (deleteTimezone != null);
    }


    /// <summary>
    /// Converts time by user timezone. Called when the "Convert time" button is pressed.
    /// </summary>
    private bool ConvertTime()
    {
        // Get user
        UserInfo user = UserInfoProvider.GetFullUserInfo(MembershipContext.AuthenticatedUser.UserID);

        // If user exist
        if (user != null)
        {
            // Get converted time
            DateTime convertedTime = TimeZoneHelper.ConvertToUserDateTime(DateTime.Now, user);

            return true;
        }
        return false;
    }

    #endregion
}