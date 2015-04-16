using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using System.Web;

using CMS.DataEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.IO;
using CMS.Base;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.ExtendedControls;
using CMS.DocumentEngine;

public partial class CMSAdminControls_Validation_LinkChecker : DocumentValidator
{
    #region "Constants"

    private const int VALIDATION_DELAY = 100;

    #endregion


    #region "Variables"

    private Regex mMatchUrlRegex = null;
    private bool mUseServerRequest = false;
    private static readonly Hashtable mErrors = new Hashtable();
    private string currentCulture = CultureHelper.DefaultUICultureCode;
    private static DataSet mDataSource = null;
    private static readonly Hashtable mDataSources = new Hashtable();
    private string mUrlRequestExceptions = ";webresource;";
    private const string mSkipUrlsStartingWith = ";javascript;mail;ftp;";
    private int mValidationDelay = 0;
    private string mErrorText = null;
    private string mInfoText = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Regular expression to remove unnecessary text from validation error explanation
    /// </summary>
    private Regex MatchUrlRegex
    {
        get
        {
            return mMatchUrlRegex ?? (mMatchUrlRegex = RegexHelper.GetRegex("<(a|link|script|img)\\s[^>]*(href|src)\\s*=\\s*(?<1>[\"']?)(?<url>[^\"'>]*)\\k<1>[^>]*>", RegexOptions.Singleline));
        }
    }


    /// <summary>
    /// Indicates if server request  will be used rather than javascript request to obtain HTML
    /// </summary>
    public bool UseServerRequestType
    {
        get
        {
            return mUseServerRequest;
        }
        set
        {
            mUseServerRequest = value;
        }
    }


    /// <summary>
    /// Gets or sets source of the data for unigrid control
    /// </summary>
    public override DataSet DataSource
    {
        get
        {
            if (mDataSource == null)
            {
                mDataSource = base.DataSource ?? mDataSources[ctlAsyncLog.ProcessGUID] as DataSet;
            }
            base.DataSource = mDataSource;

            return mDataSource;
        }
        set
        {
            mDataSource = value;
            mDataSources[ctlAsyncLog.ProcessGUID] = mDataSource;
            base.DataSource = mDataSource;
        }
    }


    /// <summary>
    /// Current log context
    /// </summary>
    public LogContext CurrentLog
    {
        get
        {
            return EnsureLog();
        }
    }


    /// <summary>
    /// Current Error
    /// </summary>
    private string CurrentError
    {
        get
        {
            return ValidationHelper.GetString(mErrors["LinkChecker_" + ctlAsyncLog.ProcessGUID], string.Empty);
        }
        set
        {
            mErrors["LinkChecker_" + ctlAsyncLog.ProcessGUID] = value;
        }
    }


    /// <summary>
    /// Exceptions which won't be processed
    /// </summary>
    public string UrlRequestExceptions
    {
        get
        {
            return mUrlRequestExceptions;
        }
        set
        {
            mUrlRequestExceptions = value;
        }
    }


    /// <summary>
    /// Key to store validation result
    /// </summary>
    protected override string ResultKey
    {
        get
        {
            return "validation|link|" + CultureCode + "|" + Url;
        }
    }


    /// <summary>
    /// Delay between validation requests to server
    /// </summary>
    private int ValidationDelay
    {
        get
        {
            if (mValidationDelay == 0)
            {
                mValidationDelay = DataHelper.GetNotZero(SettingsHelper.AppSettings["CMSValidationLinkValidatorDelay"], VALIDATION_DELAY);
            }
            return mValidationDelay;
        }
    }


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

    #endregion


    #region "Control methods"

    /// <summary>
    /// Page load 
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            DataSource = null;
        }

        SetupControls();

        if (RequestHelper.IsPostBack())
        {
            ProcessResult(DataSource);
        }
    }


    /// <summary>
    /// Page PreRender 
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(mErrorText))
        {
            ShowError(mErrorText);
        }

        if (!string.IsNullOrEmpty(mInfoText))
        {
            ShowInformation(mInfoText);
        }
    }


    /// <summary>
    /// Initializes all nested controls.
    /// </summary>
    private void SetupControls()
    {
        IsLiveSite = false;
        
        if (!RequestHelper.IsCallback())
        {
            InitializeScripts();
        }
        // Set current UI culture
        currentCulture = CultureHelper.PreferredUICultureCode;

        // Initialize events
        ctlAsyncLog.OnFinished += ctlAsync_OnFinished;
        ctlAsyncLog.OnError += ctlAsync_OnError;
        ctlAsyncLog.OnRequestLog += ctlAsync_OnRequestLog;
        ctlAsyncLog.OnCancel += ctlAsync_OnCancel;
        
        ctlAsyncLog.TitleText = GetString("validation.link.checkingurls");
        HeaderActions.ActionsList.Clear();

        // Validate action
        HeaderAction validate = new HeaderAction();
        validate.OnClientClick = "LoadHTMLToElement('" + hdnHTML.ClientID + "'," + ScriptHelper.GetString(Url) + ");";
        validate.Text = GetString("general.validate");
        validate.Tooltip = validate.Text;
        validate.CommandName = "validate";

        // View HTML code
        string click = GetViewSourceActionClick();
        
        HeaderAction viewCode = new HeaderAction();
        viewCode.OnClientClick = click;
        viewCode.Text = GetString("validation.viewcode");
        viewCode.Tooltip = viewCode.Text;
        viewCode.ButtonStyle = ButtonStyle.Default;

        // Show results in new window
        HeaderAction newWindow = new HeaderAction();
        newWindow.OnClientClick = click;
        newWindow.Text = GetString("validation.showresultsnewwindow");
        newWindow.Tooltip = newWindow.Text;
        newWindow.ButtonStyle = ButtonStyle.Default;

        if (DataHelper.DataSourceIsEmpty(DataSource))
        {
            newWindow.Enabled = false;
            newWindow.OnClientClick = null;
        }
        else
        {
            string encodedKey = ScriptHelper.GetString(HttpUtility.UrlEncode(ResultKey), false);
            newWindow.OnClientClick = String.Format("modalDialog('" + ResolveUrl("~/CMSModules/Content/CMSDesk/Validation/ValidationResults.aspx") + "?datakey={0}&docid={1}&hash={2}', 'ViewValidationResult', 800, 600);return false;", encodedKey, Node.DocumentID, QueryHelper.GetHash(String.Format("?datakey={0}&docid={1}", encodedKey, Node.DocumentID)));
        }

        // Add actions and set help topic
        HeaderActions.AddAction(validate);
        HeaderActions.AddAction(viewCode);
        HeaderActions.AddAction(newWindow);
        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

        // Set sorting and add events
        gridValidationResult.IsLiveSite = IsLiveSite;
        gridValidationResult.ZeroRowsText = GetString("validation.link.notvalidated");
        gridValidationResult.OnExternalDataBound += gridValidationResult_OnExternalDataBound;
        gridValidationResult.OnDataReload += gridValidationResult_OnDataReload;
        gridValidationResult.GridView.RowDataBound += GridView_RowDataBound;
        gridValidationResult.ShowActionsMenu = true;
        gridValidationResult.AllColumns = "statuscode, type, message, url, time, statuscodevalue, timeint";
    }


    /// <summary>
    /// Actions handler.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "validate":
                Validate();
                break;
        }
    }


    /// <summary>
    /// Actions handler.
    /// </summary>
    private void Validate()
    {
        mErrorText = null;
        pnlLog.Visible = true;
        DataSource = null;
        pnlGrid.Visible = false;

        CurrentLog.Close();
        CurrentError = string.Empty;
        EnsureLog();

        // Get the full domain
        ctlAsyncLog.Parameter = RequestContext.FullDomain + ";" + URLHelper.GetFullApplicationUrl() + ";" + URLHelper.RemoveProtocolAndDomain(Url);
        ctlAsyncLog.RunAsync(CheckLinks, WindowsIdentity.GetCurrent());
    }


    /// <summary>
    /// Row databound event
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Row event arguments</param>
    protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string color = null;
            string code = ValidationHelper.GetString(((DataRowView)(e.Row.DataItem)).Row["type"], string.Empty);
            switch (HTMLHelper.StripTags(code.ToLowerCSafe(), false).Trim())
            {
                case "e":
                    color = ((e.Row.RowIndex & 1) == 1) ? "#EEC9C9" : "#FFDADA";
                    break;
            }

            // Add color to error rows
            if (!string.IsNullOrEmpty(color))
            {
                e.Row.Style.Add("background-color", color);
            }
        }
    }


    /// <summary>
    /// On external data bound event
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="sourceName">Action what is called</param>
    /// <param name="parameter">Parameter</param>
    /// <returns>Result object</returns>
    protected object gridValidationResult_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        return GridExternalDataBound(sender, sourceName, parameter);
    }


    protected DataSet gridValidationResult_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        DataSet result = null;
        if (!DataHelper.DataSourceIsEmpty(DataSource))
        {
            result = DocumentValidationHelper.PostProcessValidationData(DataSource.Copy(), DocumentValidationEnum.Link, null);
        }
        return result;
    }

    #endregion


    #region "Validation methods"

    /// <summary>
    /// Process validation results
    /// </summary>
    /// <param name="validationResult">DataSet with results of validation</param>
    public void ProcessResult(DataSet validationResult)
    {
        if (validationResult != null)
        {
            mErrorText = null;

            // Check if result is not empty
            if (!DataHelper.DataSourceIsEmpty(validationResult))
            {
                // Show validation errors
                string text = GetString("validation.link.resultinvalidwarning");
                bool isError = false;
                foreach (DataRow dr in validationResult.Tables[0].Rows)
                {
                    string type = HTMLHelper.StripTags(ValidationHelper.GetString(dr["type"], "")).ToUpperCSafe().Trim();
                    if (type == EventType.ERROR)
                    {
                        text = GetString("validation.link.resultinvalid");
                        isError = true;
                        break;
                    }
                }

                if (isError)
                {
                    ShowError(text);
                }
                else
                {
                    ShowWarning(text);
                }
                
                lblResults.Visible = true;
                lblResults.Text = ResHelper.GetString("validation.validationresults");
                gridValidationResult.Visible = true;
            }
            else
            {
                // Show validation is valid
                ShowConfirmation(GetString("validation.link.resultvalid"));
                lblResults.Visible = false;
                gridValidationResult.Visible = false;
            }
        }
        else
        {
            // No results obtained during validation, show error
            lblResults.Visible = false;
            gridValidationResult.Visible = false;
            if (string.IsNullOrEmpty(mErrorText))
            {
                mErrorText = GetString("validation.errorinitialization");
            }
        }
    }


    /// <summary>
    /// Get HTML code using server or client method and removes HTML comments
    /// </summary>
    /// <param name="url">URL to obtain HTML from</param>
    private string GetHtmlWithoutComments(string url)
    {
        string html;
        if (UseServerRequestType)
        {
            // Create web client and try to obtain HTML using it
            WebClient client = new WebClient();
            try
            {
                StreamReader reader = StreamReader.New(client.OpenRead(url));
                html = reader.ReadToEnd();
            }
            catch (Exception e)
            {
                mErrorText = String.Format(ResHelper.GetString("validation.exception"), e.Message);
                return null;
            }
        }
        else
        {
            // Get HTML stored using javascript
            html = ValidationHelper.Base64Decode(hdnHTML.Value);
        }
        return Regex.Replace(html, "<!--.*?-->|<!--.*?$", "", RegexOptions.Multiline);
    }


    /// <summary>
    /// Get list of URLs contained in document
    /// </summary>
    private List<string> GetUrls()
    {
        string html = GetHtmlWithoutComments(Url);
        if (!String.IsNullOrEmpty(html))
        {
            Dictionary<int, string> urls = new Dictionary<int, string>();
            int counter = 0;
            string[] skippedUrlsStartingWith = mSkipUrlsStartingWith.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            // Process URLs found in document
            foreach (Match m in MatchUrlRegex.Matches(html))
            {
                string captured = m.Groups["url"].Value;

                // Check if conditions for URL to be processed are met
                if (!captured.StartsWithCSafe("#") && !urls.ContainsValue(captured) && !String.IsNullOrEmpty(captured))
                {
                    bool addUrl = true;
                    foreach (string skippedUrlStart in skippedUrlsStartingWith)
                    {
                        if (captured.StartsWithCSafe(skippedUrlStart, true))
                        {
                            addUrl = false;
                        }
                    }

                    // Add URL to list of processed URLs
                    if (addUrl)
                    {
                        urls[counter++] = Server.HtmlDecode(captured);
                    }
                }
            }

            return urls.Values.ToList();
        }

        return null;
    }


    /// <summary>
    /// Check URLs contained in document. Returns DataSet with validation results.
    /// </summary>
    /// <param name="urls">List of URLs to be processed</param>
    /// <param name="parameter">Parameter containing data to resolve relative URLs to absolute</param>
    private void CheckUrls(List<string> urls, string parameter)
    {
        int index = 0;
        int indexOffset = 0;

        // Initialize DataTable
        DataTable tbErrors = new DataTable();
        tbErrors.Columns.Add("statuscode");
        tbErrors.Columns.Add("type");
        tbErrors.Columns.Add("message");
        tbErrors.Columns.Add("url");
        tbErrors.Columns.Add("time");

        // Store table to DataSet
        DataSource = new DataSet();
        DataSource.Tables.Add(tbErrors);

        // Prepare variables
        string[] urlParams = parameter.Split(';');
        string message = null;
        int firstResponseCode = 0;
        Uri reqUri = null;
        HttpStatusCode statusCode = HttpStatusCode.OK;
        string statusDescription = null;
        bool loadDataFromResponse = true;

        // Process URLs
        while (index < urls.Count)
        {
            string url = urls[index + indexOffset];
            string type = "E";
            bool cont = false;
            loadDataFromResponse = true;
            string time = null;
            HttpWebResponse response = null;
            bool sslWarning = false;

            try
            {
                AddLog(urls[index + indexOffset], false);

                // Create HEAD web request for each URL
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URLHelper.GetAbsoluteUrl(url, urlParams[0], urlParams[1], urlParams[2]));
                req.Method = "HEAD";
                req.AllowAutoRedirect = false;

                // If exception use GET request instead
                foreach (string exception in UrlRequestExceptions.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (url.ToLowerCSafe().Contains(exception.ToLowerCSafe()))
                    {
                        req.Method = "GET";
                        break;
                    }
                }

                // Sleep thread for specified time
                Thread.Sleep(ValidationDelay);

                // Initialize watcher to get time required to access URL
                Stopwatch sw = new Stopwatch();
                sw.Start();

                try
                {
                    response = (HttpWebResponse)req.GetResponse();
                }
                catch (WebException e)
                {
                    response = (HttpWebResponse)e.Response;
                    if ((e.InnerException != null) && (e.InnerException is AuthenticationException))
                    {
                        statusDescription = e.InnerException.Message;
                        statusCode = HttpStatusCode.SwitchingProtocols;
                        loadDataFromResponse = false;
                        sslWarning = true;
                    }
                }

                sw.Stop();
                time = "(" + sw.ElapsedMilliseconds.ToString() + " ms)";
                reqUri = req.RequestUri;
            }
            catch
            {
                time = "(0 ms)";
                response = null;
            }

            // Store response values
            if (loadDataFromResponse)
            {
                if (response != null)
                {
                    statusCode = response.StatusCode;
                    statusDescription = response.StatusDescription;
                    response.Close();
                }
                else
                {
                    statusCode = HttpStatusCode.NotFound;
                    statusDescription = HttpWorkerRequest.GetStatusDescription((int)statusCode);
                }
            }

            // Process response status code
            switch (statusCode)
            {
                // Response OK status
                case HttpStatusCode.Accepted:
                case HttpStatusCode.Continue:
                case HttpStatusCode.Created:

                case HttpStatusCode.NoContent:
                case HttpStatusCode.NonAuthoritativeInformation:
                case HttpStatusCode.NotModified:
                case HttpStatusCode.OK:
                case HttpStatusCode.PartialContent:

                case HttpStatusCode.ResetContent:
                case HttpStatusCode.SwitchingProtocols:
                case HttpStatusCode.Unused:
                case HttpStatusCode.UseProxy:
                    message = statusDescription;
                    break;

                // Moved, follow redirection
                case HttpStatusCode.MultipleChoices:
                case HttpStatusCode.MovedPermanently:
                case HttpStatusCode.Found:
                case HttpStatusCode.RedirectMethod:
                case HttpStatusCode.RedirectKeepVerb:
                    indexOffset++;
                    cont = true;
                    string host = reqUri.Host;
                    if (firstResponseCode == 0)
                    {
                        firstResponseCode = (int)statusCode;
                    }

                    string newLocation = response.Headers["Location"];
                    string redirectUrl = URLHelper.ContainsProtocol(newLocation) ? newLocation : reqUri.AbsoluteUri.Substring(0, reqUri.AbsoluteUri.IndexOfCSafe(host) + host.Length) + newLocation;
                    urls.Insert(index + indexOffset, redirectUrl);
                    break;

                // Client errors
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.PaymentRequired:
                case HttpStatusCode.Forbidden:
                case HttpStatusCode.NotFound:
                case HttpStatusCode.MethodNotAllowed:
                case HttpStatusCode.NotAcceptable:
                case HttpStatusCode.ProxyAuthenticationRequired:
                case HttpStatusCode.RequestTimeout:
                case HttpStatusCode.Conflict:
                case HttpStatusCode.Gone:
                case HttpStatusCode.LengthRequired:
                case HttpStatusCode.PreconditionFailed:
                case HttpStatusCode.RequestEntityTooLarge:
                case HttpStatusCode.RequestUriTooLong:
                case HttpStatusCode.UnsupportedMediaType:
                case HttpStatusCode.RequestedRangeNotSatisfiable:
                case HttpStatusCode.ExpectationFailed:
                    message = ResHelper.GetString("validation.link.clienterror", currentCulture) + " " + statusDescription;
                    break;

                // Internal server errors
                case HttpStatusCode.InternalServerError:
                case HttpStatusCode.NotImplemented:
                case HttpStatusCode.BadGateway:
                case HttpStatusCode.ServiceUnavailable:
                case HttpStatusCode.GatewayTimeout:
                case HttpStatusCode.HttpVersionNotSupported:
                    message = ResHelper.GetString("validation.link.servererror", currentCulture) + " " + statusDescription;
                    break;
            }

            string statusCodeText = ((int)statusCode).ToString();

            // Add log describing link validation result
            AddLog(" " + time + " <b>" + DocumentValidationHelper.GetStatusCodeDescription((int)statusCode, currentCulture) + "</b> ");

            if (!cont)
            {
                // Store link validation result if link broken or final target of redirection found
                if (LinkBroken(response) || (indexOffset > 0))
                {
                    if (!LinkBroken(response) || sslWarning)
                    {
                        type = "W";
                    }

                    // Check if redirection was present
                    if (indexOffset > 0)
                    {
                        statusCodeText = firstResponseCode + "->" + (int)statusCode;
                        firstResponseCode = 0;

                        message = EnsureMaximumLineLength(urls[index]) + "<br />" + ResHelper.GetString("validation.link.permanentredir") + "<br />" + EnsureMaximumLineLength(urls[index + indexOffset]) + " <b>" + message + "</b>";
                    }

                    // Add validation result to result table
                    tbErrors.Rows.Add(statusCodeText, type, message, EnsureMaximumLineLength(urls[index + indexOffset]), time);
                }

                // Move to next url
                index += indexOffset + 1;
                indexOffset = 0;
            }
        }
    }


    /// <summary>
    /// Check links contained in document
    /// </summary>
    /// <param name="parameter">Parameter containing data to resolve relative URLs to absolute</param>
    private void CheckLinks(object parameter)
    {
        try
        {
            AddLog(ResHelper.GetString("validation.link.checkingurls", currentCulture));
            List<string> urls = GetUrls();

            // Ensure thread doesn't finish to early in special situations 
            if ((urls == null) || urls.Count == 0)
            {
                Thread.Sleep(200);
            }

            if (urls != null)
            {
                CheckUrls(urls, ValidationHelper.GetString(parameter, null));
            }
            else
            {
                CurrentError = GetString("validation.diffdomainorprotocol");
            }
            pnlLog.Visible = false;
        }
        catch (ThreadAbortException ex)
        {
            string state = ValidationHelper.GetString(ex.ExceptionState, string.Empty);
            if (state == CMSThread.ABORT_REASON_STOP)
            {
                // When canceled
                AddLog(ResHelper.GetString("validation.link.abort", currentCulture));
                ctlAsyncLog.RaiseError(null, null);
            }
            else
            {
                // Log error
                mErrorText = ex.Message;
            }
        }
        catch (Exception ex)
        {
            // Log error
            mErrorText = ex.Message;
        }
    }


    /// <summary>
    /// Indicates if link is broken according to supplied HTTP response 
    /// </summary>
    /// <param name="response">HTTP web response of URL</param>
    private bool LinkBroken(HttpWebResponse response)
    {
        if (response != null)
        {
            switch (response.StatusCode)
            {
                // Response OK status
                case HttpStatusCode.Accepted:
                case HttpStatusCode.Continue:
                case HttpStatusCode.Created:
                case HttpStatusCode.Found:
                case HttpStatusCode.MultipleChoices:
                case HttpStatusCode.NoContent:
                case HttpStatusCode.NonAuthoritativeInformation:
                case HttpStatusCode.NotModified:
                case HttpStatusCode.OK:
                case HttpStatusCode.PartialContent:
                case HttpStatusCode.RedirectKeepVerb:
                case HttpStatusCode.RedirectMethod:
                case HttpStatusCode.ResetContent:
                case HttpStatusCode.SwitchingProtocols:
                case HttpStatusCode.Unused:
                case HttpStatusCode.UseProxy:
                    return false;
            }
        }
        return true;
    }


    /// <summary>
    /// Ensures text maximal line length
    /// </summary>
    /// <param name="text">Text in which length of line should be ensured</param>
    private string EnsureMaximumLineLength(string text)
    {
        return TextHelper.EnsureMaximumLineLength(text, 50, BrowserHelper.IsIE() ? "<span></span>" : "<wbr>", false);
    }

    #endregion


    #region "Handling async thread"

    /// <summary>
    /// On cancel event
    /// </summary>
    private void ctlAsync_OnCancel(object sender, EventArgs e)
    {
        ctlAsyncLog.Parameter = null;
        AddError(ResHelper.GetString("validation.validationcanceled"));
        ScriptHelper.RegisterStartupScript(this, typeof(string), "CancelLog", ScriptHelper.GetScript("var __pendingCallbacks = new Array();"));
        mInfoText = CurrentError;
        pnlLog.Visible = false;
        pnlGrid.Visible = true;
        CurrentLog.Close();

        PostProcessData();
    }


    /// <summary>
    /// On request log event
    /// </summary>
    private void ctlAsync_OnRequestLog(object sender, EventArgs e)
    {
        ctlAsyncLog.LogContext = CurrentLog;
    }


    /// <summary>
    /// On error event
    /// </summary>
    private void ctlAsync_OnError(object sender, EventArgs e)
    {
        if (ctlAsyncLog.Status == AsyncWorkerStatusEnum.Running)
        {
            ctlAsyncLog.Stop();
        }
        ctlAsyncLog.Parameter = null;
        DataSource = null;
        mErrorText = CurrentError;
        CurrentLog.Close();
    }


    /// <summary>
    /// On finished event
    /// </summary>
    private void ctlAsync_OnFinished(object sender, EventArgs e)
    {
        mErrorText = CurrentError;
        CurrentLog.Close();
        pnlLog.Visible = false;
        pnlGrid.Visible = true;

        PostProcessData();
    }


    /// <summary>
    /// Ensures the logging context
    /// </summary>
    protected LogContext EnsureLog()
    {
        LogContext log = LogContext.EnsureLog(ctlAsyncLog.ProcessGUID);
        return log;
    }


    /// <summary>
    /// Adds the log information
    /// </summary>
    /// <param name="newLog">New log information</param>
    protected void AddLog(string newLog)
    {
        AddLog(newLog, true);
    }


    /// <summary>
    /// Adds the log information
    /// </summary>
    /// <param name="newLog">New log information</param>
    /// <param name="addWholeLine">Indicates if log text forms whole line</param>
    protected void AddLog(string newLog, bool addWholeLine)
    {
        EnsureLog();
        if (addWholeLine)
        {
            LogContext.AppendLine(newLog);
        }
        else
        {
            LogContext.Append(newLog);
        }
    }


    /// <summary>
    /// Adds the error to collection of errors
    /// </summary>
    /// <param name="error">Error message</param>
    protected void AddError(string error)
    {
        AddLog(error);
        CurrentError = (error + "<br />" + CurrentError);
    }


    /// <summary>
    /// Final data processing
    /// </summary>
    protected void PostProcessData()
    {
        // Final data processing
        if (!DataHelper.DataSourceIsEmpty(DataSource))
        {
            DataSource.Tables[0].DefaultView.Sort = "type ASC";
            DataTable dtResult = DataSource.Tables[0].DefaultView.ToTable();
            DataSource.Tables.Clear();
            DataSource.Tables.Add(dtResult);

            dtResult = DocumentValidationHelper.ProcessValidationResult(DataSource, DocumentValidationEnum.Link, new Dictionary<string, object> { { "culture", currentCulture } });
        }

        SetupControls();

        // Fill the grid data source
        if (!DataHelper.DataSourceIsEmpty(DataSource))
        {
            gridValidationResult.ReloadData();
        }

        ProcessResult(DataSource);
    }

    #endregion
}