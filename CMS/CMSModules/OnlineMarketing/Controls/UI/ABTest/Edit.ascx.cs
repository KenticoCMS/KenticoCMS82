using System;
using System.Linq;
using System.Text;

using CMS.DocumentEngine;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_OnlineMarketing_Controls_UI_AbTest_Edit : CMSAdminEditControl
{
    #region "Variables"

    private bool mNewCreated;
    private string mQueryAliasPath;
    private ABTestMessagesWriter mMessagesWriter;
    private ABTestStatusEnum? mTestStatus;

    #endregion


    #region "Properties"

    /// <summary>
    /// Strongly typed EditedObject.
    /// </summary>
    private ABTestInfo ABTest
    {
        get
        {
            return form.EditedObject as ABTestInfo;
        }
    }


    /// <summary>
    /// Status of the current test.
    /// </summary>
    private ABTestStatusEnum TestStatus
    {
        get
        {
            if (!mTestStatus.HasValue)
            {
                mTestStatus = ABTestStatusEvaluator.GetStatus(ABTest);
            }

            return mTestStatus.Value;
        }
    }


    /// <summary>
    /// Gets class that writes info messages into the page.
    /// </summary>
    private ABTestMessagesWriter MessagesWriter
    {
        get
        {
            return mMessagesWriter ?? (mMessagesWriter = new ABTestMessagesWriter(ShowMessage));
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        SetRedirectUrlAfterCreate();
        InitHeaderActions();
        DisableFieldsByTestStatus();

        // If this is GET request or POST request called by modal dialog, use test values from DB
        if (!RequestHelper.IsPostBack() || (Request["__EVENTARGUMENT"] == "modalClosed"))
        {
            form.FieldControls["ABTestOpenFrom"].Value = ABTest.ABTestOpenFrom;
            form.FieldControls["ABTestOpenTo"].Value = ABTest.ABTestOpenTo;
        }
    }


    protected void form_OnBeforeDataLoad(object sender, EventArgs e)
    {
        mQueryAliasPath = QueryHelper.GetString("AliasPath", string.Empty);
        if (!String.IsNullOrEmpty(mQueryAliasPath))
        {
            form.FieldsToHide.Add("ABTestOriginalPage");
        }
    }


    protected void form_OnBeforeSave(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(mQueryAliasPath))
        {
            form.Data.SetValue("ABTestOriginalPage", mQueryAliasPath);
        }

        form.Data.SetValue("ABTestSiteID", CurrentSite.SiteID);
        mNewCreated = (ABTest.ABTestID == 0);
    }


    protected void form_OnBeforeValidate(object sender, EventArgs e)
    {
        // Visitor targeting value is filled in by the condition builder so prevent it from being erased on failed validation
        form.FieldControls["ABTestVisitorTargeting"].Value = ValidationHelper.GetString(form.GetFieldValue("ABTestVisitorTargeting"), string.Empty);

        if (!IsValid())
        {
            form.StopProcessing = true;
        }
    }


    protected void form_OnAfterSave(object sender, EventArgs e)
    {
        // For new AB test create default variant
        if (mNewCreated)
        {
            CreateDefaultVariant(ABTest);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Show info labels when editing existing test 
        if (ABTest.ABTestID > 0)
        {
            MessagesWriter.ShowStatusInfo(ABTest);
            MessagesWriter.ShowABTestScheduleInformation(ABTest, TestStatus);
            MessagesWriter.ShowMissingVariantsTranslationsWarning(ABTest);
        }

        form.SubmitButton.Visible = false;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Disables fields not allowed in specific states of the test.
    /// </summary>
    private void DisableFieldsByTestStatus()
    {
        // Disable fields not allowed to be changed once set
        form.FieldControls["ABTestOriginalPage"].Enabled = (ABTest.ABTestID == 0);

        // Disable fields not allowed if the test has been started
        bool notStarted = ((TestStatus == ABTestStatusEnum.NotStarted) || (TestStatus == ABTestStatusEnum.Scheduled));
        form.FieldControls["ABTestOpenFrom"].Enabled = notStarted;
        form.FieldControls["ABTestCulture"].Enabled = notStarted;

        // Disable fields not allowed if the test has been finished
        bool notFinished = (TestStatus != ABTestStatusEnum.Finished);
        form.FieldControls["ABTestOpenTo"].Enabled = notFinished;
        form.FieldControls["ABTestVisitorTargeting"].Enabled = notFinished;
        form.FieldControls["ABTestIncludedTraffic"].Enabled = notFinished;
    }


    /// <summary>
    /// Creates a new AB variant that should be created by default to given AB test
    /// </summary>
    private void CreateDefaultVariant(ABTestInfo info)
    {
        // Create instance of AB variant
        ABVariantInfo variant = new ABVariantInfo();

        // Set properties
        variant.ABVariantPath = info.ABTestOriginalPage;
        variant.ABVariantTestID = info.ABTestID;
        variant.ABVariantDisplayName = GetString("abtesting.originalvariantdisplayname");
        variant.ABVariantName = "Original";
        variant.ABVariantSiteID = info.ABTestSiteID;

        // Save to the storage
        ABVariantInfoProvider.SetABVariantInfo(variant);
    }


    /// <summary>
    /// Checks whether the form is valid.
    /// </summary>
    private bool IsValid()
    {
        // If the test is finished, no validation is needed
        if (TestStatus == ABTestStatusEnum.Finished)
        {
            return true;
        }

        // Get page of the test so we can check for collisions
        string page = QueryHelper.GetString("AliasPath", null);
        if (String.IsNullOrEmpty(page))
        {
            page = form.GetFieldValue("ABTestOriginalPage").ToString();
        }

        // Validate original page of the test
        if (!PageExists(page))
        {
            ShowError(GetString("abtesting.testpath.pagenotfound"));
            return false;
        }

        // Create temporary test used for validation of the new values
        ABTestInfo updatedTest = new ABTestInfo
        {
            ABTestID = ABTest.ABTestID,
            ABTestOriginalPage = page,
            ABTestCulture = form.GetFieldValue("ABTestCulture").ToString(),
            ABTestSiteID = SiteContext.CurrentSiteID,
        };

        updatedTest.ABTestOpenFrom = ValidationHelper.GetDateTime(form.GetFieldValue("ABTestOpenFrom"), DateTimeHelper.ZERO_TIME);

        // Validate start time if the test is not already running
        if (TestStatus != ABTestStatusEnum.Running)
        {
            if (!ABTestValidator.IsValidStart(updatedTest.ABTestOpenFrom))
            {
                ShowError(GetString("om.wrongtimeinterval"));
                return false;
            }
        }

        updatedTest.ABTestOpenTo = ValidationHelper.GetDateTime(form.GetFieldValue("ABTestOpenTo"), DateTimeHelper.ZERO_TIME);

        // Validate finish time of the test
        if (!ABTestValidator.IsValidFinish(updatedTest.ABTestOpenFrom, updatedTest.ABTestOpenTo))
        {
            ShowError(GetString("om.wrongtimeinterval"));
            return false;
        }

        // Find out possible collision - another test running on the same page, culture and at the same time
        string collidingTestName = ABTestValidator.GetCollidingTestName(updatedTest);
        if (!String.IsNullOrEmpty(collidingTestName))
        {
            ShowError(String.Format(GetString("om.twotestsonepageerror"), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(collidingTestName)), updatedTest.ABTestOriginalPage));
            return false;
        }

        // If we get here, all fields are valid
        return true;
    }


    /// <summary>
    /// Sets url for redirect after creating abtest.
    /// </summary>
    private void SetRedirectUrlAfterCreate()
    {
        string url = UIContextHelper.GetElementUrl("CMS.ABTest", "Detail");

        url = URLHelper.AddParameterToUrl(url, "objectid", "{%EditedObject.ID%}");
        url = URLHelper.AddParameterToUrl(url, "tabname", "Settings");
        url = URLHelper.AddParameterToUrl(url, "saved", "1");

        url = URLHelper.PropagateUrlParameters(url, "aliaspath", "nodeid", "displayTitle", "dialog");
        
        url = UIContextHelper.AppendDialogHash(url);

        form.RedirectUrlAfterCreate = url;
    }


    /// <summary>
    /// Initializes header action control.
    /// </summary>
    private void InitHeaderActions()
    {
        var btnSave = new SaveAction(Page)
        {
            Permission = "Manage",
            ResourceName = "CMS.ABTest"
        };

        HeaderActions.AddAction(btnSave);

        switch (TestStatus)
        {
            case ABTestStatusEnum.NotStarted:
                if (ABTest.ABTestID != 0)
                {
                    AddStartTestButton();
                }
                break;
            case ABTestStatusEnum.Scheduled:
                AddStartTestButton();
                break;

            case ABTestStatusEnum.Running:
                AddFinishTestButton();
                break;
        }
    }


    /// <summary>
    /// Adds the "Start test" button
    /// </summary>
    private void AddStartTestButton()
    {
        string testStartUrl = ResolveUrl("~/CMSModules/OnlineMarketing/Pages/Content/ABTesting/ABTest/StartABTest.aspx?testid=" + ABTest.ABTestID);
        var btnStartTest = new HeaderAction
        {
            ResourceName = "CMS.ABTest",
            Permission = "Manage",
            Tooltip = GetString("abtesting.starttest.tooltip"),
            Text = GetString("abtesting.starttest"),
            OnClientClick = "modalDialog('" + testStartUrl + @"', '', 670, 320);",
            ButtonStyle = ButtonStyle.Default,
        };
        HeaderActions.AddAction(btnStartTest);
    }


    /// <summary>
    /// Adds the "Finish test" button
    /// </summary>
    private void AddFinishTestButton()
    {
        string testFinishUrl = ResolveUrl("~/CMSModules/OnlineMarketing/Pages/Content/ABTesting/ABTest/FinishABTest.aspx?testid=" + ABTest.ABTestID);
        var btnFinishTest = new HeaderAction
        {
            ResourceName = "CMS.ABTest",
            Permission = "Manage",
            Tooltip = GetString("abtesting.finishtest.tooltip"),
            Text = GetString("abtesting.finishtest"),
            OnClientClick = "modalDialog('" + testFinishUrl + @"', '', 670, 320);",
            ButtonStyle = ButtonStyle.Default,
        };

        HeaderActions.AddAction(btnFinishTest);
    }


    /// <summary>
    /// Checks if the page specified by the original variant path exists.
    /// </summary>
    /// <param name="originalVariantPath">Original variant path</param>
    private bool PageExists(string originalVariantPath)
    {
        return DocumentHelper.GetDocuments()
                               .PublishedVersion()
                               .TopN(1)
                               .All()
                               .OnCurrentSite()
                               .Culture(ABTest.ABTestCulture)
                               .CombineWithDefaultCulture(false)
                               .Columns("NodeID")
                               .WhereEquals("NodeAliasPath", originalVariantPath)
                               .Any();
    }

    #endregion
}