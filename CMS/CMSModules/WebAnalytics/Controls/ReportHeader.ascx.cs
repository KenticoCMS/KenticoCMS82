using System;
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;
using CMS.WebAnalytics;

public partial class CMSModules_WebAnalytics_Controls_ReportHeader : CMSAdminControl
{
    #region "Variables"

    private bool mManageData = true;
    private string mPrintPageURL = "~/CMSModules/Reporting/Tools/Analytics_Print.aspx";
    private HitsIntervalEnum mSelectedInterval = HitsIntervalEnum.None;
    private string mPanelCssClass = "cms-edit-menu";
    private bool mPrintEnabled = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Header actions
    /// </summary>
    public override HeaderActions HeaderActions
    {
        get
        {
            return headerActions;
        }
    }


    /// <summary>
    /// CSS class for header actions container. Default class is cms-edit-menu.
    /// </summary>
    public string PanelCssClass
    {
        get
        {
            return mPanelCssClass;
        }
        set
        {
            mPanelCssClass = value;
        }
    }


    /// <summary>
    /// Datarow with report's parameters.
    /// </summary>
    public DataRow ReportParameters
    {
        get;
        set;
    }


    /// <summary>
    /// Report's name.
    /// </summary>
    public String ReportName
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates report's interval (hour,day,week,month,year,none).
    /// </summary>
    public HitsIntervalEnum SelectedInterval
    {
        get
        {
            return mSelectedInterval;
        }
        set
        {
            mSelectedInterval = value;
        }
    }


    /// <summary>
    /// Set/get codename (for parameter datacodename) which is passed to manage analytics data dialog.
    /// </summary>
    public String ManageDataCodeName
    {
        get;
        set;
    }


    /// <summary>
    /// If true, button for manage analytics data is displayed.
    /// </summary>
    public bool DisplayManageData
    {
        get
        {
            return mManageData;
        }
        set
        {
            mManageData = value;
        }
    }


    /// <summary>
    /// Gets or sets the print page URL for the print action.
    /// </summary>
    public string PrintPageURL
    {
        get
        {
            return mPrintPageURL;
        }
        set
        {
            mPrintPageURL = value;
        }
    }


    /// <summary>
    /// If false, button for print will be disabled.
    /// </summary>
    public bool PrintEnabled
    {
        get
        {
            return mPrintEnabled;
        }
        set
        {
            mPrintEnabled = value;
        }
    }

    #endregion


    #region "Events"

    public event CommandEventHandler ActionPerformed;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        headerActions.ActionPerformed += HeaderActions_ActionPerformed;
    }


    protected override void OnPreRender(EventArgs e)
    {
        string dataCodeName = string.IsNullOrEmpty(ManageDataCodeName) ? QueryHelper.GetString("dataCodeName", string.Empty) : ManageDataCodeName;

        string deleteDialogUrl = ResolveUrl("~/CMSModules/Reporting/WebAnalytics/Analytics_ManageData.aspx");
        
        deleteDialogUrl = URLHelper.AddParameterToUrl(deleteDialogUrl, "statcodename", URLHelper.URLEncode(dataCodeName));
        deleteDialogUrl = URLHelper.AddParameterToUrl(deleteDialogUrl, "hash", QueryHelper.GetHash(deleteDialogUrl));

        string deleteScript = string.Format("modalDialog('{0}','AnalyticsManageData',{1},{2});", deleteDialogUrl, 680, 350);

        string printDialogUrl = string.Format("{0}?reportname={1}&parameters={2}",
            ResolveUrl(PrintPageURL),
            ReportName,
            AnalyticsHelper.GetQueryStringParameters(ReportParameters));

        string printScript = string.Format("myModalDialog('{0}&UILang={1}&hash={2}','PrintReport {3}',800,700);return false",
            printDialogUrl,
            CultureInfo.CurrentUICulture.IetfLanguageTag,
            QueryHelper.GetHash(printDialogUrl),
            ReportName);

        string subscriptionScript = String.Format("modalDialog('{0}?reportname={1}&parameters={2}&interval={3}','Subscription',{4},{5});return false",
            ResolveUrl("~/CMSModules/Reporting/Dialogs/EditSubscription.aspx"),
            ReportName,
            AnalyticsHelper.GetQueryStringParameters(ReportParameters),
            HitsIntervalEnumFunctions.HitsConversionToString(SelectedInterval),
            AnalyticsHelper.SUBSCRIPTION_WINDOW_WIDTH,
            AnalyticsHelper.SUBSCRIPTION_WINDOW_HEIGHT);

        string refreshScript = "function RefreshPage() {" + ControlsHelper.GetPostBackEventReference(this, "") + "};";
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshScript", ScriptHelper.GetScript(refreshScript));

        // Register special script for print window
        ScriptHelper.RegisterPrintDialogScript(Page);

        ScriptHelper.RegisterDialogScript(Page);

        headerActions.PanelCssClass = PanelCssClass;

        // Create header actions
        SaveAction save = new SaveAction(Page);
        headerActions.ActionsList.Add(save);

        // Print
        HeaderAction print = new HeaderAction
        {
            Text = GetString("Analytics_Report.Print"),
            OnClientClick = printScript,
            Enabled = PrintEnabled,
            ButtonStyle = ButtonStyle.Default,
        };
        headerActions.ActionsList.Add(print);

        var cui = MembershipContext.AuthenticatedUser;

        // Manage data
        if (cui.IsAuthorizedPerResource("CMS.WebAnalytics", "ManageData") && DisplayManageData)
        {
            HeaderAction delete = new HeaderAction
            {
                Text = GetString("Analytics_Report.ManageData"),
                OnClientClick = deleteScript,
                ButtonStyle = ButtonStyle.Default,
            };
            headerActions.ActionsList.Add(delete);
        }

        // Report subscription enabled test
        GeneralizedInfo ri = BaseAbstractInfoProvider.GetInfoByName(PredefinedObjectType.REPORT, ReportName);
        if (ri != null)
        {
            bool enableSubscription = ValidationHelper.GetBoolean(ri.GetValue("ReportEnableSubscription"), true);

            // Show enable subscription only for users with subscribe or modify.            
            enableSubscription &= (cui.IsAuthorizedPerResource("cms.reporting", "subscribe") || cui.IsAuthorizedPerResource("cms.reporting", "modify"));

            if (enableSubscription)
            {
                // Subscription
                HeaderAction subscription = new HeaderAction
                {
                    Text = GetString("notifications.subscribe"),
                    OnClientClick = subscriptionScript,
                    ButtonStyle = ButtonStyle.Default,
                };
                headerActions.ActionsList.Add(subscription);
            }
        }

        base.OnPreRender(e);
    }


    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        if (ActionPerformed != null)
        {
            ActionPerformed(sender, e);
        }
    }

    #endregion
}