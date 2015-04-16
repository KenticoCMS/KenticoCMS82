using System;
using System.Web.UI.WebControls;
using System.Web.UI;

using CMS.CMSImportExport;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.Protection;
using CMS.DataEngine;

using CultureInfo = System.Globalization.CultureInfo;

public partial class CMSModules_AbuseReport_Controls_AbuseReportStatusEdit : CMSAdminEditControl
{
    #region "Private variables"

    private AbuseReportInfo mCurrentReport;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    ///ID of current report.
    /// </summary>
    public int ReportID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets current report.
    /// </summary>
    private AbuseReportInfo CurrentReport
    {
        get
        {
            if (mCurrentReport == null)
            {
                mCurrentReport = AbuseReportInfoProvider.GetAbuseReportInfo(ReportID);
                // Set edited object

                EditedObject = mCurrentReport;
                if (mCurrentReport == null)
                {
                    throw new Exception(string.Format("[AbuseReportStatusEdit.CurrentReport]: The abuse report with ID '{0}' doesn't exist.", ReportID));
                }
            }
            return mCurrentReport;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            ReloadData(true);
        }

        btnOk.Click += btnOK_Click;
        rfvText.ErrorMessage = GetString("abuse.textreqired");
    }

    #endregion


    #region "Other events"

    /// <summary>
    /// Button OK click event handler.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check permissions
        if (!CheckPermissions("CMS.AbuseReport", "Manage"))
        {
            return;
        }

        // Check that text area is not empty
        txtCommentValue.Text = txtCommentValue.Text.Trim();
        if (txtCommentValue.Text.Length > 1000)
        {
            txtCommentValue.Text = txtCommentValue.Text.Substring(0, 1000);
        }

        if (txtCommentValue.Text.Length > 0)
        {
            // Load new values
            CurrentReport.ReportComment = txtCommentValue.Text;
            CurrentReport.ReportStatus = (AbuseReportStatusEnum)ValidationHelper.GetInteger(drpStatus.SelectedValue, 0);

            // Save AbuseReport
            AbuseReportInfoProvider.SetAbuseReportInfo(CurrentReport);
            ShowChangesSaved();
        }
        else
        {
            ShowError(GetString("abuse.errors.comment"));
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads all data.
    /// </summary>
    public override void ReloadData(bool forceLoad)
    {
        if (CurrentReport != null)
        {
            // Load labels
            if (!RequestHelper.IsPostBack() || forceLoad)
            {
                // Create query parameters
                string query = "?ObjectID=" + CurrentReport.ReportObjectID;

                // Set link value
                string url = CurrentReport.ReportURL;
                if (CurrentReport.ReportCulture != String.Empty)
                {
                    url = URLHelper.AddParameterToUrl(url, URLHelper.LanguageParameterName, CurrentReport.ReportCulture);
                }
                lnkUrlValue.Text = HTMLHelper.HTMLEncode(url);
                lnkUrlValue.NavigateUrl = url;
                lnkUrlValue.ToolTip = HTMLHelper.HTMLEncode(url);
                lnkUrlValue.Target = "_blank";

                // Set culture value
                CultureInfo ci = CultureHelper.GetCultureInfo(CurrentReport.ReportCulture);
                lblCultureValue.Text = (ci != null) ? ci.DisplayName : ResHelper.Dash;

                // Set site value
                SiteInfo si = SiteInfoProvider.GetSiteInfo(CurrentReport.ReportSiteID);
                lblSiteValue.Text = (si != null) ? HTMLHelper.HTMLEncode(si.DisplayName) : ResHelper.Dash;

                // Set title
                lblTitleValue.Text = HTMLHelper.HTMLEncode(CurrentReport.ReportTitle);

                // Set labels
                if (!string.IsNullOrEmpty(CurrentReport.ReportObjectType))
                {
                    lblObjectTypeValue.Text = GetString("ObjectType." + ImportExportHelper.GetSafeObjectTypeName(CurrentReport.ReportObjectType));
                    query += "&ObjectType=" + CurrentReport.ReportObjectType;
                }
                else
                {
                    lblObjectTypeValue.Text = ResHelper.Dash;
                }

                // Get object display name
                lblObjectNameValue.Text = ResHelper.Dash;

                string objectType = CurrentReport.ReportObjectType;
                int objectId = CurrentReport.ReportObjectID;

                if ((objectId > 0) && !string.IsNullOrEmpty(objectType) && !DocumentHelper.IsDocumentObjectType(objectType))
                {
                    GeneralizedInfo obj = BaseAbstractInfoProvider.GetInfoById(objectType, objectId);
                    if ((obj != null) && !string.IsNullOrEmpty(obj.ObjectDisplayName))
                    {
                        lblObjectNameValue.Text = HTMLHelper.HTMLEncode(obj.ObjectDisplayName);
                    }
                }

                // Set Reported by label
                lblReportedByValue.Text = ResHelper.Dash;
                if (CurrentReport.ReportUserID != 0)
                {
                    UserInfo ui = UserInfoProvider.GetUserInfo(CurrentReport.ReportUserID);
                    lblReportedByValue.Text = (ui != null) ? HTMLHelper.HTMLEncode(ui.FullName) : GetString("general.NA");
                }

                // Set other parameters
                lblReportedWhenValue.Text = CurrentReport.ReportWhen.ToString();

                if ((CurrentReport.ReportObjectID > 0) && (!string.IsNullOrEmpty(CurrentReport.ReportObjectType)) && AbuseReportInfoProvider.IsObjectTypeSupported(CurrentReport.ReportObjectType))
                {
                    string detailUrl = "~/CMSModules/AbuseReport/AbuseReport_ObjectDetails.aspx" + query;
                    detailUrl = URLHelper.AddParameterToUrl(detailUrl, "hash", QueryHelper.GetHash(detailUrl));

                    var headerActions = ((CMSPage)Page).HeaderActions;
                    headerActions.AddAction(new HeaderAction
                    {
                        Text = GetString("abuse.details"),
                        OnClientClick = ScriptHelper.GetModalDialogScript(URLHelper.ResolveUrl(detailUrl), "objectdetails", 960, 600),
                        ButtonStyle = ButtonStyle.Default
                    });
                }

                Control postback = ControlsHelper.GetPostBackControl(Page);
                CMSPage page = Page as CMSPage;

                // Not post-back not caused by OK button or Save action in header
                if ((postback != btnOk) && ((page == null) || (postback != page.HeaderActions)))
                {
                    txtCommentValue.Text = CurrentReport.ReportComment;
                    LoadStatus((int)CurrentReport.ReportStatus);
                }
            }
        }
    }


    /// <summary>
    /// Loads status from enumeration to dropdown list.
    /// </summary>
    private void LoadStatus(int reportStatus)
    {
        drpStatus.Items.Clear();
        drpStatus.Items.Add(new ListItem(GetString("general.new"), "0"));
        drpStatus.Items.Add(new ListItem(GetString("general.solved"), "1"));
        drpStatus.Items.Add(new ListItem(GetString("general.rejected"), "2"));
        drpStatus.SelectedValue = reportStatus.ToString();
    }

    #endregion
}