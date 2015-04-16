using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.IO;
using CMS.Base;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.IO.Compression;
using CMS.ExtendedControls;
using CMS.DocumentEngine;

public partial class CMSAdminControls_Validation_HTMLValidator : DocumentValidator
{
    #region "Constants"

    private const string DEFAULT_VALIDATOR_URL = "http://validator.w3.org/check";

    #endregion


    #region "Variables"

    private string mValidatorURL = null;
    private DataSet mDataSource = null;
    private bool mUseServerRequest = false;
    private string mAppValidatorPath = null;
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
            return mValidatorURL ?? (mValidatorURL = DataHelper.GetNotEmpty(SettingsHelper.AppSettings["CMSValidationHTMLValidatorURL"], DEFAULT_VALIDATOR_URL));
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
    /// Validator application path use to resolve links referencing on validator pages
    /// </summary>
    private string AppValidatorPath
    {
        get
        {
            if (mAppValidatorPath == null)
            {
                mAppValidatorPath = URLHelper.RemoveProtocolAndDomain(ValidatorURL);
                int lastId = mAppValidatorPath.LastIndexOfCSafe('/');
                if (lastId >= 0)
                {
                    mAppValidatorPath = mAppValidatorPath.Substring(0, lastId).TrimEnd('/');
                }
                else
                {
                    mAppValidatorPath = "";
                }
            }
            return mAppValidatorPath;
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
    /// Key to store validation result
    /// </summary>
    protected override string ResultKey
    {
        get
        {
            return "validation|html|" + CultureCode + "|" + Url;
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

        // Configure controls
        SetupControls();

        if (RequestHelper.IsPostBack())
        {
            ReloadData();
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
        gridValidationResult.OrderBy = "line ASC";
        gridValidationResult.IsLiveSite = IsLiveSite;
        gridValidationResult.OnExternalDataBound += gridValidationResult_OnExternalDataBound;
        gridValidationResult.OnDataReload += gridValidationResult_OnDataReload;
        gridValidationResult.ZeroRowsText = GetString("validation.html.notvalidated");
        gridValidationResult.ShowActionsMenu = true;
        gridValidationResult.AllColumns = "line, column, message, explanation, source";

        // Set custom validating text
        up.ProgressText = GetString("validation.validating");
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
        DataSource = null;
        DataSource = ValidateHtml();
        ReloadData();
    }


    /// <summary>
    /// Loads data from the data source property.
    /// </summary>
    public void ReloadData()
    {
        SetupControls();

        gridValidationResult.ReloadData();

        ProcessResult(DataSource);
    }


    protected DataSet gridValidationResult_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        return DataSource;
    }


    /// <summary>
    /// On external databound event
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


    #region "Validation request methods"

    /// <summary>
    /// Get validation request parameters
    /// </summary>
    /// <param name="htmlDocument">Content of HTML document</param>
    /// <returns>Validator request parameters</returns>
    private string GetRequestParameters(string htmlDocument)
    {
        string requestData = "fragment=" + HttpUtility.UrlEncode(htmlDocument);
        requestData += "&output=soap12";

        return requestData;
    }


    /// <summary>
    /// Send validation request to validator and obtain result 
    /// </summary>
    /// <param name="validatorParameters">Validator parameters</param>
    /// <returns>DataSet containing validator response</returns>
    private DataSet GetValidationResult(string validatorParameters)
    {
        try
        {
            DataSet dsValResult = null;

            // Create web request
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(ValidatorURL);
            req.Method = "POST";
            req.UserAgent = HttpContext.Current.Request.UserAgent;
            req.ContentType = "application/x-www-form-urlencoded";
            byte[] data = Encoding.GetEncoding("UTF-8").GetBytes(validatorParameters);
            req.ContentLength = data.Length;
            using (StreamWrapper writer = StreamWrapper.New(req.GetRequestStream()))
            {
                writer.Write(data, 0, data.Length);
            }


            // Process server answer
            StreamWrapper answer = StreamWrapper.New(req.GetResponse().GetResponseStream());
            if (answer != null)
            {
                dsValResult = new DataSet();
                dsValResult.ReadXml(answer.SystemStream);
                answer.Close();
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
    /// General method to process validation and return validation results
    /// </summary>
    private DataSet ValidateHtml()
    {
        if (!String.IsNullOrEmpty(Url))
        {
            string docHtml = GetHtml(Url);
            if (!String.IsNullOrEmpty(docHtml))
            {
                DataSet dsValidationResult = GetValidationResult(GetRequestParameters(docHtml));

                if (!DataHelper.DataSourceIsEmpty(dsValidationResult))
                {
                    // Check if result contains error table
                    if (!DataHelper.DataSourceIsEmpty(dsValidationResult.Tables["error"]))
                    {
                        Dictionary<string, object> parameters = new Dictionary<string, object>();
                        parameters["validatorurl"] = ValidatorURL;
                        parameters["validatorapppath"] = AppValidatorPath;

                        DataTable tbError = DocumentValidationHelper.ProcessValidationResult(dsValidationResult, DocumentValidationEnum.HTML, parameters);
                        DataSet result = new DataSet();
                        result.Tables.Add(tbError);
                        return result;
                    }
                    else
                    {
                        return new DataSet();
                    }
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
    /// Process validation results
    /// </summary>
    /// <param name="validationResult">Validation result</param>
    public void ProcessResult(DataSet validationResult)
    {
        if (validationResult != null)
        {
            mErrorText = null;

            // Check if result is not empty
            if (!DataHelper.DataSourceIsEmpty(validationResult) && !DataHelper.DataSourceIsEmpty(validationResult.Tables["error"]))
            {
                // Show validation errors
                lblResults.Text = GetString("validation.validationresults");
                lblResults.Visible = true;
                gridValidationResult.Visible = true;
                ShowError(GetString("validation.html.resultinvalid"));
            }
            else
            {
                // Show validation is valid
                lblResults.Visible = false;
                gridValidationResult.Visible = false;
                ShowConfirmation(GetString("validation.html.resultvalid"));
            }
        }
        else
        {
            // No results obtained from validator, show error
            lblResults.Visible = false;
            gridValidationResult.Visible = false;
            if (string.IsNullOrEmpty(mErrorText))
            {
                mErrorText = GetString("validation.errorinitialization");
            }
        }
    }


    private string GetHtml(string url)
    {
        if (UseServerRequestType)
        {
            // Create web client and try to obtaining HTML using it
            WebClient client = new WebClient();
            try
            {
                StreamReader reader = StreamReader.New(client.OpenRead(url));
                return reader.ReadToEnd();
            }
            catch
            {
                return null;
            }
        }
        else
        {
            // Get HTML stored using javascript
            return ValidationHelper.Base64Decode(hdnHTML.Value);
        }
    }

    #endregion
}