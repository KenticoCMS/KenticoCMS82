using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Net;
using System.Web;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.Helpers;
using CMS.Base;
using CMS.IO;
using CMS.ExtendedControls.ActionsConfig;
using CMS.FormControls;
using CMS.ExtendedControls;

using HtmlAgilityPack;
using CMS.DocumentEngine;

public partial class CMSAdminControls_Validation_AccessibilityValidator : DocumentValidator
{
    #region "Accessibility standard enum"

    /// <summary>
    /// Accessibility standard enum
    /// </summary>
    public enum AccessibilityStandardEnum : int
    {
        /// <summary>
        /// BIK BITV 1
        /// </summary>
        BITV1_0 = 1,

        /// <summary>
        /// Section 508
        /// </summary>
        Section508 = 2,

        /// <summary>
        /// Stanca Act
        /// </summary>
        StancaAct = 3,

        /// <summary>
        /// WCAG 1 A
        /// </summary>
        WCAG1_0A = 4,

        /// <summary>
        /// WCAG 1 AA
        /// </summary>
        WCAG1_0AA = 5,

        /// <summary>
        /// WCAG 1 AAA
        /// </summary>
        WCAG1_0AAA = 6,

        /// <summary>
        /// WCAG 2 A
        /// </summary>
        WCAG2_0A = 7,

        /// <summary>
        /// WCAG 2 AA
        /// </summary>
        WCAG2_0AA = 8,

        /// <summary>
        /// WCAG 2 AAA
        /// </summary>
        WCAG2_0AAA = 9
    }

    #endregion


    #region "Accessibility code"

    /// <summary>
    /// Accessibility standard code
    /// </summary>
    public static class AccessibilityStandardCode
    {
        #region "Constants"

        /// <summary>
        /// BIK BITV 1
        /// </summary>
        public const int BITV1_0 = 1;

        /// <summary>
        /// Section 508
        /// </summary>
        public const int Section508 = 2;

        /// <summary>
        /// Stanca Act
        /// </summary>
        public const int StancaAct = 3;

        /// <summary>
        /// WCAG 1 A
        /// </summary>
        public const int WCAG1_0A = 4;

        /// <summary>
        /// WCAG 1 AA
        /// </summary>
        public const int WCAG1_0AA = 5;

        /// <summary>
        /// WCAG 1 AAA
        /// </summary>
        public const int WCAG1_0AAA = 6;

        /// <summary>
        /// WCAG 2 A
        /// </summary>
        public const int WCAG2_0A = 7;

        /// <summary>
        /// WCAG 2 AA
        /// </summary>
        public const int WCAG2_0AA = 8;

        /// <summary>
        /// WCAG 2 AAA
        /// </summary>
        public const int WCAG2_0AAA = 9;

        #endregion


        #region "Methods"

        /// <summary>
        /// Returns the enumeration representation of the Accessibility standard code.
        /// </summary>
        /// <param name="code">Accessibility standard code</param>
        public static AccessibilityStandardEnum ToEnum(int code)
        {
            switch (code)
            {
                case BITV1_0:
                    return AccessibilityStandardEnum.BITV1_0;

                case Section508:
                    return AccessibilityStandardEnum.Section508;

                case StancaAct:
                    return AccessibilityStandardEnum.StancaAct;

                case WCAG1_0A:
                    return AccessibilityStandardEnum.WCAG1_0A;

                case WCAG1_0AA:
                    return AccessibilityStandardEnum.WCAG1_0AA;

                case WCAG1_0AAA:
                    return AccessibilityStandardEnum.WCAG1_0AAA;

                case WCAG2_0A:
                    return AccessibilityStandardEnum.WCAG2_0A;

                case WCAG2_0AA:
                    return AccessibilityStandardEnum.WCAG2_0AA;

                case WCAG2_0AAA:
                    return AccessibilityStandardEnum.WCAG2_0AAA;

                default:
                    return AccessibilityStandardEnum.WCAG2_0AA;
            }
        }


        /// <summary>
        /// Returns the accessibility standard code from the enumeration value.
        /// </summary>
        /// <param name="value">Value to convert</param>
        public static int FromEnum(AccessibilityStandardEnum value)
        {
            switch (value)
            {
                case AccessibilityStandardEnum.BITV1_0:
                    return BITV1_0;

                case AccessibilityStandardEnum.Section508:
                    return Section508;

                case AccessibilityStandardEnum.StancaAct:
                    return StancaAct;

                case AccessibilityStandardEnum.WCAG1_0A:
                    return WCAG1_0A;

                case AccessibilityStandardEnum.WCAG1_0AA:
                    return WCAG1_0AA;

                case AccessibilityStandardEnum.WCAG1_0AAA:
                    return WCAG1_0AAA;

                case AccessibilityStandardEnum.WCAG2_0A:
                    return WCAG2_0A;

                case AccessibilityStandardEnum.WCAG2_0AA:
                    return WCAG2_0AA;

                case AccessibilityStandardEnum.WCAG2_0AAA:
                    return WCAG2_0AAA;

                default:
                    return WCAG2_0AA;
            }
        }

        #endregion
    }

    #endregion


    #region "Constants"

    private const string DEFAULT_VALIDATOR_URL = "http://achecker.ca/checker/index.php";

    #endregion


    #region "Variables"

    private string mValidatorURL = null;
    private bool mUseServerRequest = false;
    private static DataSet mDataSource = null;
    private Regex mIntNumberRegEx = null;
    private CMSDropDownList mStandardList = null;
    private string mErrorText = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// URL to which validator requests will be sent
    /// </summary>
    public string ValidatorURL
    {
        get
        {
            return mValidatorURL ?? (mValidatorURL = DataHelper.GetNotEmpty(SettingsHelper.AppSettings["CMSValidationAccessibilityValidatorURL"], DEFAULT_VALIDATOR_URL));
        }
        set
        {
            mValidatorURL = value;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            gridValidationResult.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Indicates if server request  will be used rather than JavaScript request to obtain HTML
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
    /// Key to store validation result
    /// </summary>
    protected override string ResultKey
    {
        get
        {
            return "validation|access|" + CultureCode + "|" + Url;
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
                mDataSource = base.DataSource;
            }
            base.DataSource = mDataSource;

            return mDataSource;
        }
        set
        {
            base.DataSource = value;
            mDataSource = value;
        }
    }


    /// <summary>
    /// Gets or sets validation standard
    /// </summary>
    public AccessibilityStandardEnum Standard
    {
        get;
        set;
    }


    /// <summary>
    /// Regular expression for integer numbers
    /// </summary>
    private Regex IntNumberRegEx
    {
        get
        {
            return mIntNumberRegEx ?? (mIntNumberRegEx = RegexHelper.GetRegex("\\b\\d+\\b"));
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
    protected override void OnInit(EventArgs e)
    {
        FormEngineUserControl label = LoadUserControl("~/CMSFormControls/Basic/LabelControl.ascx") as FormEngineUserControl;
        if (label != null)
        {
            label.Value = GetString("validation.accessibility.standard");
        }

        // Add validation standard
        FormEngineUserControl standard = LoadUserControl("~/CMSFormControls/Basic/DropDownListControl.ascx") as FormEngineUserControl;
        if (standard != null)
        {
            mStandardList = standard.FindControl(standard.InputControlID) as CMSDropDownList;
            mStandardList.Attributes.Add("class", "form-control input-width-60");
        }
        ControlsHelper.FillListControlWithEnum<AccessibilityStandardEnum>(mStandardList, "validation.accessibility.standard");

        // Set default standard value
        if (!RequestHelper.IsPostBack() && (standard != null))
        {
            standard.Value = AccessibilityStandardCode.FromEnum(AccessibilityStandardEnum.WCAG2_0A);
        }

        HeaderActions.AdditionalControls.Add(label);
        HeaderActions.AdditionalControls.Add(standard);
        HeaderActions.AdditionalControlsCssClass = "HeaderActionsLabel control-group-inline";
        HeaderActions.ReloadAdditionalControls();
    }


    /// <summary>
    /// Page load 
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        bool isPostBack = RequestHelper.IsPostBack();

        if (!isPostBack)
        {
            DataSource = null;
        }

        ReloadData(isPostBack);
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
    }


    /// <summary>
    /// Initializes all nested controls.
    /// </summary>
    private void SetupControls()
    {
        IsLiveSite = false;

        InitializeScripts();

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
            newWindow.Enabled = true;
            string encodedKey = ScriptHelper.GetString(HttpUtility.UrlEncode(ResultKey), false);
            newWindow.OnClientClick = String.Format("modalDialog('" + ResolveUrl("~/CMSModules/Content/CMSDesk/Validation/ValidationResults.aspx") + "?datakey={0}&docid={1}&hash={2}', 'ViewValidationResult', 800, 600);return false;", encodedKey, Node.DocumentID, QueryHelper.GetHash(String.Format("?datakey={0}&docid={1}", encodedKey, Node.DocumentID)));
        }

        HeaderActions.AddAction(validate);
        HeaderActions.AddAction(viewCode);
        HeaderActions.AddAction(newWindow);
        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

        // Set sorting and add events       
        gridValidationResult.OrderBy = "line";
        gridValidationResult.IsLiveSite = IsLiveSite;
        gridValidationResult.OnExternalDataBound += gridValidationResult_OnExternalDataBound;
        gridValidationResult.OnDataReload += gridValidationResult_OnDataReload;
        gridValidationResult.ZeroRowsText = GetString("validation.access.notvalidated");
        gridValidationResult.ShowActionsMenu = true;
        gridValidationResult.AllColumns = "line, column, accessibilityrule, error, fixsuggestion, source";

        // Set custom validating text
        up.ProgressText = GetString("validation.validating");
        mStandardList.CssClass = "";
    }


    /// <summary>
    /// Actions handler.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "validate":
                Standard = AccessibilityStandardCode.ToEnum(ValidationHelper.GetInteger(((FormEngineUserControl)HeaderActions.AdditionalControls[1]).Value, 0));
                Validate();
                break;
        }
    }

    /// <summary>
    /// Actions handler.
    /// </summary>
    private void Validate()
    {
        DataSource = null;
        DataSource = ValidateHtml();

        ReloadData(true);
    }


    /// <summary>
    /// Loads data from the data source property.
    /// </summary>
    public void ReloadData(bool force)
    {
        SetupControls();

        gridValidationResult.ReloadData();

        if (force)
        {
            ProcessResult(DataSource);
        }

    }

    /// <summary>
    /// Data reload event
    /// </summary> 
    protected DataSet gridValidationResult_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        if (!DataHelper.DataSourceIsEmpty(DataSource))
        {
            totalRecords = DataSource.Tables[0].Rows.Count;
        }
        return DataSource;
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

    #endregion


    #region "Validation methods"

    /// <summary>
    /// Get HTML code using server or client method
    /// </summary>
    /// <param name="url">URL to obtain HTML from</param>
    private string GetHtml(string url)
    {
        if (UseServerRequestType)
        {
            // Create web client and try to obtain HTML using it
            WebClient client = new WebClient();
            try
            {
                StreamReader reader = StreamReader.New(client.OpenRead(url));
                return reader.ReadToEnd();
            }
            catch (Exception e)
            {
                mErrorText = String.Format(ResHelper.GetString("validation.exception"), e.Message);
                return null;
            }
        }
        else
        {
            // Get HTML stored using JavaScript
            return ValidationHelper.Base64Decode(hdnHTML.Value);
        }
    }


    /// <summary>
    /// Send validation request to validator and obtain result 
    /// </summary>
    /// <param name="htmlData">HTML to validate</param>
    /// <returns>DataSet containing validator response</returns>
    private DataSet GetValidationResult(string htmlData)
    {
        try
        {
            DataSet dsValResult = null;
            Random randGen = new Random();

            // Create web request
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(ValidatorURL);
            req.Method = "POST";
            string boundary = "---------------------------" + randGen.Next(1000000, 9999999) + randGen.Next(1000000, 9999999);
            req.ContentType = "multipart/form-data; boundary=" + boundary;

            // Set data to web request for validation           
            byte[] data = Encoding.GetEncoding("UTF-8").GetBytes(GetRequestData(GetRequestDictionary(htmlData), boundary));
            req.ContentLength = data.Length;
            StreamWrapper writer = StreamWrapper.New(req.GetRequestStream());
            writer.Write(data, 0, data.Length);
            writer.Close();

            // Process server answer
            StreamWrapper answer = StreamWrapper.New(req.GetResponse().GetResponseStream());
            if (answer != null)
            {
                // Load document with given HTML
                HtmlDocument doc = new HtmlDocument();
                doc.Load(answer.SystemStream);

                // Find the div node with validation results
                var divErrors = doc.DocumentNode.SelectSingleNode("//div[@id='AC_errors']");
                DataTable dtErrors = CreateAccessibilityTable("errors", divErrors);

                dsValResult = new DataSet();
                dsValResult.Tables.Add(dtErrors);
            }

            return dsValResult;
        }
        catch
        {
            mErrorText = GetString("validation.servererror");
            return null;
        }
    }


    /// <summary>
    /// Create accessibility table with validation results
    /// </summary>
    /// <param name="tableName">Name of the result table</param>
    /// <param name="node">HTML node containing validation results</param>
    private DataTable CreateAccessibilityTable(string tableName, HtmlNode node)
    {
        DataTable tb = null;

        if (node != null)
        {
            // Create table to store results
            tb = new DataTable(tableName);
            tb.Columns.AddRange(new DataColumn[] {  new DataColumn("line",typeof(int)),
                                                    new DataColumn("column",typeof(int)),
                                                    new DataColumn("accessibilityrule",typeof(string)),
                                                    new DataColumn("error",typeof(string)),
                                                    new DataColumn("fixsuggestion",typeof(string)),
                                                    new DataColumn("source",typeof(string))
                                                 });

            // Variables to store data
            string mainRule = null;
            string minorRule = null;
            string error = null;
            string fix = null;
            int line = 0;
            int col = 0;
            string source = null;

            // Process specified HTML nodes containing results
            HtmlNodeCollection nodes = node.SelectNodes("./h3 | ./h4 | ./div[@class='gd_one_check']//span[@class='gd_msg']/a | ./div[@class='gd_one_check']//div[@class='gd_question_section'] | ./div[@class='gd_one_check']//table//tr");

            // Go through results
            if (nodes != null)
            {
                foreach (HtmlNode child in nodes)
                {
                    switch (child.Name.ToLowerCSafe())
                    {
                        case "h3":
                            mainRule = child.InnerText;
                            break;

                        case "h4":
                            minorRule = child.InnerText;
                            break;

                        case "a":
                            error = child.InnerHtml;
                            break;

                        case "div":
                            fix = child.InnerHtml;
                            break;

                        // Process error details
                        case "tr":

                            HtmlNodeCollection errorChilds = child.SelectNodes(".//td//em | .//td//code[@class='input']");

                            foreach (HtmlNode errorChild in errorChilds)
                            {
                                switch (errorChild.Name.ToLowerCSafe())
                                {
                                    case "em":
                                        Match[] position = new Match[2];
                                        IntNumberRegEx.Matches(errorChild.InnerText).CopyTo(position, 0);
                                        line = ValidationHelper.GetInteger(position[0].Value, 0);
                                        col = ValidationHelper.GetInteger(position[1].Value, 0);
                                        break;

                                    case "code":
                                        source = errorChild.InnerHtml;
                                        break;
                                }
                            }

                            // Fill datarow content
                            DataRow dr = tb.NewRow();
                            dr["accessibilityrule"] = mainRule + "<br/>" + minorRule;
                            dr["error"] = error;
                            dr["fixsuggestion"] = fix;
                            dr["line"] = line;
                            dr["column"] = col;
                            dr["source"] = source;

                            // Add row to table
                            tb.Rows.Add(dr);
                            break;
                    }
                }
            }
        }
        return tb;
    }


    /// <summary>
    /// General method to process validation and return validation results
    /// </summary>
    private DataSet ValidateHtml()
    {
        if (!String.IsNullOrEmpty(Url))
        {
            string docHtml = GetHtml(Url);
            if (!String.IsNullOrEmpty(docHtml))
            {
                DataSet dsValidationResult = GetValidationResult(docHtml);

                // Check if result contains error table
                if (!DataHelper.DataSourceIsEmpty(dsValidationResult) && !DataHelper.DataSourceIsEmpty(dsValidationResult.Tables["errors"]))
                {
                    DataTable tbError = DocumentValidationHelper.ProcessValidationResult(dsValidationResult, DocumentValidationEnum.Accessibility, null);
                    tbError.DefaultView.Sort = "line ASC";
                    DataSet result = new DataSet();
                    result.Tables.Add(tbError);
                    return result;
                }
                else
                {
                    return dsValidationResult;
                }
            }
            else
            {
                mErrorText = GetString("validation.diffdomainorprotocol");
            }
        }

        return null;
    }


    /// <summary>
    /// Get dictionary with request parameters
    /// </summary>
    /// <param name="data">HTML data to be checked</param>
    private Dictionary<string, string> GetRequestDictionary(string data)
    {
        Dictionary<string, string> reqData = new Dictionary<string, string>();
        reqData.Add("pastehtml", data);
        reqData.Add("validate_paste", "Check It");
        reqData.Add("checkbox_gid[]", AccessibilityStandardCode.FromEnum(Standard).ToString());
        reqData.Add("radio_gid[]", AccessibilityStandardCode.FromEnum(Standard).ToString());
        reqData.Add("rpt_format", "1");
        return reqData;
    }


    /// <summary>
    /// Get request data which will be sent using HTTP request to validator
    /// </summary>
    /// <param name="data">Data to create </param>
    /// <param name="boundary">HTTP boundary string</param>
    private string GetRequestData(Dictionary<string, string> data, string boundary)
    {
        string separator = TextHelper.NewLine;
        boundary = "--" + boundary;

        // Prepare beginning of the request data
        StringBuilder sbRequest = new StringBuilder();
        sbRequest.Append(boundary);
        sbRequest.Append(separator);

        // Process request form data
        foreach (string key in data.Keys)
        {
            // Note: Do not encode key name. The server side requires utf-8 defined name
            sbRequest.Append(String.Format("Content-Disposition: form-data; name=\"{0}\"", key));
            sbRequest.Append(separator);
            sbRequest.Append(separator);
            sbRequest.Append(data[key]);
            sbRequest.Append(separator);
            sbRequest.Append(boundary);
            sbRequest.Append(separator);
        }
        string request = sbRequest.ToString();

        // Add final boundary dashes
        request = request.Insert(request.Length - 2, "--");
        return request;
    }


    /// <summary>
    /// Process validation results
    /// </summary>
    /// <param name="validationResult">DataSet with result of validation</param>
    public void ProcessResult(DataSet validationResult)
    {
        if (validationResult != null)
        {
            mErrorText = null;

            // Check if result is not empty
            if (!DataHelper.DataSourceIsEmpty(validationResult))
            {
                // Show validation errors
                ShowError(GetString("validation.access.resultinvalid"));
                lblResults.Visible = true;
                lblResults.Text = ResHelper.GetString("validation.validationresults");
                gridValidationResult.Visible = true;
            }
            else
            {
                // Show validation is valid
                ShowConfirmation(GetString("validation.access.resultvalid"));
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

    #endregion
}