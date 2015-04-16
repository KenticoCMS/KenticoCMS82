using System;
using System.Collections;
using System.Data;

using CMS.EventLog;
using CMS.Helpers;
using CMS.UIControls;
using CMS.Base;

public partial class CMSAPIExamples_Controls_APIExample : CMSUserControl
{
    #region "Enumeration"

    /// <summary>
    /// Type of API example.
    /// </summary>
    public enum APIExampleTypeEnum
    {
        /// <summary>
        /// Main API example.
        /// </summary>
        ManageMain = 0,

        /// <summary>
        /// Additional API example.
        /// </summary>
        ManageAdditional = 1,

        /// <summary>
        /// Main cleanup API example.
        /// </summary>
        CleanUpMain = 2,

        /// <summary>
        /// Additional cleanup API example.
        /// </summary>
        CleanUpAdditional = 3
    }

    #endregion


    #region "Variables"

    private int? mExampleOrder;
    private string mButtonClass;
    private string mInfoMessage = "The API example ran successfully.";
    private string mErrorMessage = "Something went wrong.";
    private string mMethodName;
    private APIExampleTypeEnum mAPIExampleType = APIExampleTypeEnum.ManageMain;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Text which is set as inner button's text.
    /// </summary>
    public string ButtonText
    {
        get;
        set;
    }


    /// <summary>
    /// Css class which is applied to inner button.
    /// </summary>
    public string ButtonClass
    {
        get
        {
            if (string.IsNullOrEmpty(mButtonClass))
            {
                switch (APIExampleType)
                {
                    case APIExampleTypeEnum.ManageAdditional:
                    case APIExampleTypeEnum.CleanUpAdditional:
                        // Blue button
                        mButtonClass = "XXLongButton";
                        break;
                    default:
                        // Green button
                        mButtonClass = "XXLongSubmitButton";
                        break;
                }
            }

            return mButtonClass;
        }
        set
        {
            mButtonClass = value;
        }
    }


    /// <summary>
    /// Text which is applied when API example ran successfully.
    /// </summary>
    public string InfoMessage
    {
        get
        {
            return mInfoMessage;
        }
        set
        {
            mInfoMessage = value;
        }
    }


    /// <summary>
    /// Text which is displayed when API example failed.
    /// </summary>
    public string ErrorMessage
    {
        get
        {
            return mErrorMessage;
        }
        set
        {
            mErrorMessage = value;
        }
    }


    /// <summary>
    /// Type of the example.
    /// </summary>
    public APIExampleTypeEnum APIExampleType
    {
        get
        {
            return mAPIExampleType;
        }
        set
        {
            mAPIExampleType = value;
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Specifies order within all module's examples from same container.
    /// </summary>
    private int ExampleOrder
    {
        get
        {
            if (!mExampleOrder.HasValue)
            {
                if (IsCleanupExample)
                {
                    mExampleOrder = ValidationHelper.GetInteger(RequestStockHelper.GetItem("CMSRightAPIExampleOrder"), 0) + 1;
                    RequestStockHelper.Add("CMSRightAPIExampleOrder", mExampleOrder, false);
                }
                else
                {
                    mExampleOrder = ValidationHelper.GetInteger(RequestStockHelper.GetItem("CMSLeftAPIExampleOrder"), 0) + 1;
                    RequestStockHelper.Add("CMSLeftAPIExampleOrder", mExampleOrder, false);
                }
            }

            return (int)mExampleOrder;
        }
    }


    /// <summary>
    /// Indicates if controls is used for cleanup example.
    /// </summary>
    private bool IsCleanupExample
    {
        get
        {
            return (APIExampleType == APIExampleTypeEnum.CleanUpMain) || (APIExampleType == APIExampleTypeEnum.CleanUpAdditional);
        }
    }


    /// <summary>
    /// Name of the method handling this example.
    /// </summary>
    private string MethodName
    {
        get
        {
            if (string.IsNullOrEmpty(mMethodName))
            {
                if ((RunExample != null) && (RunExample.Method != null))
                {
                    mMethodName = RunExample.Method.Name;
                }
                else if ((RunExampleSimple != null) && (RunExampleSimple.Method != null))
                {
                    mMethodName = RunExampleSimple.Method.Name;
                }
                else
                {
                    // Skip leading "api" substring
                    mMethodName = ID.Substring(3);
                }
            }
            return mMethodName;
        }
    }


    /// <summary>
    /// Path to file where this control is used.
    /// </summary>
    private string CurrentFilePath
    {
        get
        {
            string path = RequestStockHelper.GetItem("CurrentFilePath", true) as string;
            if (string.IsNullOrEmpty(path))
            {
                path = Request.PhysicalPath + ".cs";
                RequestStockHelper.Add("CurrentFilePath", path);
            }

            return path;
        }
    }

    #endregion


    #region "Events"

    public delegate bool OnRunExample();

    /// <summary>
    /// Event raised on button click. You can throw <see cref="CMSAPIExampleException"/> with error message that will be shown to user and won't be logged to event log.
    /// </summary>
    public event OnRunExample RunExample;


    public delegate void OnRunExampleSimple();

    /// <summary>
    /// Event raised on button click. You can throw <see cref="CMSAPIExampleException"/> with error message that will be shown to user and won't be logged to event log.
    /// </summary>
    public event OnRunExampleSimple RunExampleSimple;


    public delegate void OnDisplayCode(string filePath, string method);

    /// <summary>
    /// Event raised on "Disaplay Code" button click.
    /// </summary>
    public event OnDisplayCode DisplayCode;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set up displaying of code
        CMSAPIExamplePage examplePage = Page as CMSAPIExamplePage;
        if (examplePage != null)
        {
            DisplayCode += examplePage.ReadCode;
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        btnAction.CssClass = ButtonClass;
        btnAction.Text = ButtonText;

        if (DisplayCode != null)
        {
            btnShowCode.Visible = true;
           
            if (SystemContext.IsRunningOnAzure)
            {
                btnShowCode.Enabled = false;       
            }
        }

        lblNumber.Text = ExampleOrder + ".";
    }


    protected void btnAction_Click(object sender, EventArgs e)
    {
        Run();
    }


    /// <summary>
    /// Runs example.
    /// </summary>
    public void Run()
    {
        try
        {
            if (RunExample != null)
            {
                bool success = RunExample();

                if (!success)
                {
                    // Display error message
                    lblError.Text = ErrorMessage;
                    lblError.Visible = true;

                    return;
                }
            }
            else if (RunExampleSimple != null)
            {
                RunExampleSimple();
            }
        }
        catch (CMSAPIExampleException ex)
        {
            lblError.Text = String.IsNullOrWhiteSpace(ex.Message) ? ErrorMessage : ex.Message;
            lblError.Visible = true;

            return;
        }
        catch (Exception ex)
        {
            // Log exception
            EventLogProvider.LogException("APIExample", "EXCEPTION", ex);

            string msg = "";

            // Try to find id of last interesting log
            string where = EventLogProvider.ProviderObject.GetSiteWhereCondition(0) + " AND (Source = 'APIExample') AND (IPAddress = '" + RequestContext.UserHostAddress + "')";
            string orderBy = "EventTime DESC, EventID DESC";
            DataSet ds = EventLogProvider.GetAllEvents(where, orderBy, 1, "EventID");

            // Get id
            int eventId = 0;
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                eventId = ValidationHelper.GetInteger(ds.Tables[0].Rows[0]["EventID"], 0);
            }

            if (eventId != 0)
            {
                string identifier = Guid.NewGuid().ToString();
                Hashtable mParameters = new Hashtable();
                mParameters["where"] = where;
                mParameters["orderby"] = orderBy;

                WindowHelper.Add(identifier, mParameters);

                string queryString = "?params=" + identifier;
                queryString = URLHelper.AddParameterToUrl(queryString, "hash", QueryHelper.GetHash(queryString));
                queryString = URLHelper.AddParameterToUrl(queryString, "eventid", eventId.ToString());

                // Add link to event details in event log
                msg = String.Format("The API example failed. See event log for <a href=\"\" onclick=\"modalDialog('" + ResolveUrl("~/CMSModules/EventLog/EventLog_Details.aspx") + queryString + "', 'eventdetails', 920, 700); return false;\">more details</a>.");
            }
            else
            {
                // Add link to Event log
                msg = String.Format("The API example failed. See <a href=\"" + ResolveUrl("~/CMSModules/EventLog/EventLog.aspx") + "\" target=\"_blank\">event log</a> for more details.");
            }

            // Display error message
            lblError.Text = msg;
            lblError.ToolTip = ex.Message;
            lblError.Visible = true;

            return;
        }

        //Display info message
        lblInfo.Text = InfoMessage;
        lblInfo.Visible = true;
    }


    protected void btnShowCode_Click(object sender, EventArgs e)
    {
        if (DisplayCode != null)
        {
            DisplayCode(CurrentFilePath, MethodName);
        }
    }

    #endregion
}