using System;

using CMS.Base;
using CMS.DocumentEngine;
using CMS.EventManager;
using CMS.Globalization;
using CMS.Helpers;
using CMS.PortalControls;
using CMS.Protection;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.WebAnalytics;

using TimeZoneInfo = CMS.Globalization.TimeZoneInfo;

public partial class CMSWebParts_EventManager_EventManager : CMSAbstractWebPart
{
    protected DateTime eventDate = DateTimeHelper.ZERO_TIME;
    protected DateTime openFrom = DateTimeHelper.ZERO_TIME;
    protected DateTime openTo = DateTimeHelper.ZERO_TIME;
    protected int capacity = 0;
    protected bool allowRegistrationOverCapacity = false;
    protected bool errorOccurs = false;
    protected TreeNode mEventNode = null;

    protected TreeNode EventNode
    {
        get
        {
            return mEventNode;
        }
        set
        {
            mEventNode = value;
        }
    }


    #region "Public properties"

    /// <summary>
    ///  Gets or sets the value that indicates whether first and last user name are required.
    /// </summary>
    public bool RequireName
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RequireName"), true);
        }
        set
        {
            SetValue("RequireName", value);
        }
    }


    /// <summary>
    ///  Gets or sets the value that indicates whether phone number is required for registration.
    /// </summary>
    public bool RequirePhone
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RequirePhone"), true);
        }
        set
        {
            SetValue("RequirePhone", value);
        }
    }


    /// <summary>
    ///  Gets or sets the value that indicates whether link to *.ics file will be available after registration.
    /// </summary>
    public bool AllowExportToOutlook
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowExportToOutlook"), true);
        }
        set
        {
            SetValue("AllowExportToOutlook", value);
        }
    }


    /// <summary>
    ///  Gets or sets the value that indicates whether public users are allowed to register.
    /// </summary>
    public bool AllowAnonymousRegistration
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowAnonymousRegistration"), true);
        }
        set
        {
            SetValue("AllowAnonymousRegistration", value);
        }
    }


    /// <summary>
    ///  Gets or sets the registration title.
    /// </summary>
    public string RegistrationTitle
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RegistrationTitle"), null);
        }
        set
        {
            SetValue("RegistrationTitle", value);
        }
    }

    #endregion


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing || DocumentContext.CurrentDocument == null || DocumentContext.CurrentDocument.NodeClassName.ToLowerCSafe() != "cms.bookingevent")
        {
            // Do nothing
            Visible = false;
        }
        else
        {
            // Get current event document
            EventNode = DocumentContext.CurrentDocument;

            // Get event date, open from/to, capacity and possibility to register over capacity information
            eventDate = ValidationHelper.GetDateTimeSystem(EventNode.GetValue("EventDate"), DateTimeHelper.ZERO_TIME);
            openFrom = ValidationHelper.GetDateTimeSystem(EventNode.GetValue("EventOpenFrom"), DateTimeHelper.ZERO_TIME);
            openTo = ValidationHelper.GetDateTimeSystem(EventNode.GetValue("EventOpenTo"), DateTimeHelper.ZERO_TIME);
            capacity = ValidationHelper.GetInteger(EventNode.GetValue("EventCapacity"), 0);
            allowRegistrationOverCapacity = ValidationHelper.GetBoolean(EventNode.GetValue("EventAllowRegistrationOverCapacity"), false);

            // Display registration section
            DisplayRegistration();

            // Display link to iCalendar file which adds this event to users Outlook
            if (AllowExportToOutlook)
            {
                lnkOutlook.NavigateUrl = "~/CMSModules/EventManager/CMSPages/AddToOutlook.aspx?eventid=" + EventNode.NodeID;
                lnkOutlook.Target = "_blank";
                lnkOutlook.Text = GetString("eventmanager.exporttooutlook");
                lnkOutlook.Visible = true;
            }
        }
    }


    /// <summary>
    /// Displays registration section depending on situation.
    /// </summary>
    protected void DisplayRegistration()
    {
        if (!string.IsNullOrEmpty(RegistrationTitle))
        {
            lblRegTitle.Text = RegistrationTitle;
            lblRegTitle.Visible = true;
        }

        var userInfo = MembershipContext.AuthenticatedUser;
        // Display registration form to anonymous user only if this is allowed
        if ((AllowAnonymousRegistration || (userInfo != null && AuthenticationHelper.IsAuthenticated())) && EventNode != null)
        {
            DateTime now = DateTime.Now;
            // Display registration form if opened
            if ((openFrom == DateTimeHelper.ZERO_TIME || openFrom < now) && (openTo == DateTimeHelper.ZERO_TIME || now <= openTo) && (now <= eventDate))
            {
                int actualCount = EventAttendeeInfoProvider.GetEventAttendeesCount(EventNode.OriginalNodeID);
                // Display registration form if capacity is not full
                if (allowRegistrationOverCapacity || (actualCount < capacity))
                {
                    // Preset fields with info of authenticated user
                    if (userInfo != null && AuthenticationHelper.IsAuthenticated() && !RequestHelper.IsPostBack())
                    {
                        txtFirstName.Text = userInfo.FirstName;
                        txtLastName.Text = userInfo.LastName;
                        txtEmail.Text = userInfo.Email;
                    }

                    // Hide non-required fields
                    if (!RequireName)
                    {
                        plcName.Visible = false;
                    }
                    if (!RequirePhone)
                    {
                        plcPhone.Visible = false;
                    }
                }
                else
                {
                    pnlReg.Visible = false;
                    lblError.Text = GetString("eventmanager.fullcapacity");
                    lblError.Visible = true;
                    errorOccurs = true;
                }
            }
            else
            {
                pnlReg.Visible = false;
                lblError.Text = GetString("eventmanager.notopened");
                lblError.Visible = true;
                errorOccurs = true;
            }
        }
        else
        {
            pnlReg.Visible = false;
            lblError.Text = GetString("eventmanager.notauthenticated");
            lblError.Visible = true;
            errorOccurs = true;
        }
    }


    /// <summary>
    /// On btnRegister click.
    /// </summary>
    protected void btnRegister_Click(object sender, EventArgs e)
    {
        string currentSiteName = SiteContext.CurrentSiteName;
        // Check banned ip
        if (!BannedIPInfoProvider.IsAllowed(currentSiteName, BanControlEnum.AllNonComplete))
        {
            lblError.Visible = true;
            lblError.Text = GetString("General.BannedIP");
            return;
        }

        // Exit if problem occurs
        if (errorOccurs)
        {
            return;
        }

        string result = null;
        Validator val = new Validator();
        // Check name fields if required
        if (RequireName)
        {
            result = val.NotEmpty(txtFirstName.Text.Trim(), GetString("eventmanager.firstnamerequired"))
                .NotEmpty(txtLastName.Text.Trim(), GetString("eventmanager.lastnamerequired")).Result;
        }
        // Check e-mail field
        if (string.IsNullOrEmpty(result))
        {
            result = val.IsEmail(txtEmail.Text.Trim(), GetString("eventmanager.emailrequired")).Result;
        }
        // Check phone field if required
        if (RequirePhone && string.IsNullOrEmpty(result))
        {
            result = val.NotEmpty(txtPhone.Text.Trim(), GetString("eventmanager.phonerequired")).Result;
        }

        if (string.IsNullOrEmpty(result))
        {
            DateTime now = DateTime.Now;
            // Allow registration if opened
            if ((openFrom == DateTimeHelper.ZERO_TIME || openFrom < now) && (openTo == DateTimeHelper.ZERO_TIME || now <= openTo) && (now <= eventDate))
            {
                if (EventNode != null)
                {
                    if (!EventAttendeeInfoProvider.IsRegisteredForEvent(EventNode.NodeID, txtEmail.Text.Trim()))
                    {
                        // Add new attendant to the event
                        EventAttendeeInfo eai = AddAttendantToEvent();

                        if (eai != null)
                        {
                            // Log activity
                            Activity activity = new ActivityEventBooking(EventNode, EventNode.GetDocumentName(), eai, AnalyticsContext.ActivityEnvironmentVariables);
                            activity.Log();

                            // Send invitation e-mail
                            TimeZoneInfo tzi = null;
                            TimeZoneMethods.GetDateTimeForControl(this, DateTime.Now, out tzi);
                            EventProvider.SendInvitation(currentSiteName, EventNode, eai, tzi);

                            lblRegInfo.Text = GetString("eventmanager.registrationsucceeded");
                            lblRegInfo.Visible = true;
                            // Hide registration form
                            pnlReg.Visible = false;
                        }
                    }
                    else
                    {
                        // User is already registered
                        lblError.Text = GetString("eventmanager.attendeeregistered");
                        lblError.Visible = true;
                    }
                }
                else
                {
                    // Event does not exist
                    lblError.Text = GetString("eventmanager.eventnotexist");
                    lblError.Visible = true;
                    // Hide registration form
                    pnlReg.Visible = false;
                }
            }
            else
            {
                // Event registration is not opened
                lblError.Text = GetString("eventmanager.notopened");
                lblError.Visible = true;
                // Hide registration form
                pnlReg.Visible = false;
            }
        }
        else
        {
            // Display error message
            lblError.Text = result;
            lblError.Visible = true;
        }
    }


    /// <summary>
    /// Add new attendant to the event.
    /// </summary>
    /// <returns>Newly created attendee</returns>
    private EventAttendeeInfo AddAttendantToEvent()
    {
        EventAttendeeInfo attendeeInfo = null;

        if (EventNode != null)
        {
            attendeeInfo = new EventAttendeeInfo();

            attendeeInfo.AttendeeEventNodeID = EventNode.OriginalNodeID;
            attendeeInfo.AttendeeEmail = txtEmail.Text.Trim();
            if (RequireName)
            {
                attendeeInfo.AttendeeFirstName = txtFirstName.Text;
                attendeeInfo.AttendeeLastName = txtLastName.Text;
            }
            if (RequirePhone)
            {
                attendeeInfo.AttendeePhone = txtPhone.Text;
            }

            // Add new attendant to the event
            EventAttendeeInfoProvider.SetEventAttendeeInfo(attendeeInfo);
        }

        return attendeeInfo;
    }
}