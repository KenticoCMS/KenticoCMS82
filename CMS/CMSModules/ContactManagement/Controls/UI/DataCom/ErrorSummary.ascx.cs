using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using CMS.DataCom;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;

/// <summary>
/// A control that displays error messages and details about Data.com REST API errors.
/// </summary>
public partial class CMSModules_ContactManagement_Controls_UI_DataCom_ErrorSummary : CMSAdminControl
{
    #region "Variables"

    /// <summary>
    /// The error message to display.
    /// </summary>
    private new string mErrorMessage;


    /// <summary>
    /// The exception to display if development mode is enabled.
    /// </summary>
    private Exception mException;


    /// <summary>
    /// The list of Data.com REST API errors to display.
    /// </summary>
    private Error[] mErrors;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the value indicating whether this control will show errors using the page messages.
    /// </summary>
    public bool MessagesEnabled
    {
        get
        {
            object enabled = ViewState["MessagesEnabled"];

            return enabled == null ? false : (bool)enabled;
        }
        set
        {
            ViewState["MessagesEnabled"] = value;
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Reports an error.
    /// </summary>
    /// <param name="errorMessage">The error message to report.</param>
    public void Report(string errorMessage)
    {
        mErrorMessage = errorMessage;
        DisplayError();
    }


    /// <summary>
    /// Reports an error and related exception.
    /// </summary>
    /// <param name="errorMessage">The error message to report.</param>
    /// <param name="exception">The exception to report.</param>
    public void Report(string errorMessage, Exception exception)
    {
        mErrorMessage = errorMessage;
        mException = exception;
        DisplayError();
    }


    /// <summary>
    /// Reports the specified exception.
    /// </summary>
    /// <param name="exception">The exception to report.</param>
    public void Report(Exception exception)
    {
        mException = exception;
        mErrorMessage = GetErrorMessageForException(exception);
        DataComException customException = exception as DataComException;
        if (customException != null)
        {
            mErrorMessage = GetString("datacom.errormessage");
            mErrors = customException.Errors.ToArray();
        }
        DisplayError();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Displays an error using the provided information.
    /// </summary>
    private void DisplayError()
    {
        LocalizeErrors();
        string message = GetErrorMessage();
        string description = GetErrorDescription();
        string tooltip = GetPlainErrorTooltip();
        if (MessagesEnabled)
        {
            ShowError(message, description, tooltip);
        }
        else
        {
            tooltip = HTMLHelper.HTMLEncode(tooltip);

            HtmlGenericControl paragraph = new HtmlGenericControl("div")
            {
                InnerHtml = message
            };
            paragraph.Attributes.Add("class", "Red");
            if (!String.IsNullOrEmpty(tooltip))
            {
                paragraph.InnerHtml = String.Format("<div>{0}</div><div><b>{1}</b></div>", paragraph.InnerHtml, tooltip);
            }
            Controls.Add(paragraph);
            if (!String.IsNullOrEmpty(description))
            {
                Literal literal = new Literal
                {
                    Text = description
                };
                Controls.Add(literal);
            }
        }
    }


    /// <summary>
    /// Formats the error message using the provided information, and returns it.
    /// </summary>
    /// <returns>An error message.</returns>
    private string GetErrorMessage()
    {
        return HTMLHelper.HTMLEncode(mErrorMessage);
    }


    /// <summary>
    /// Formats the error description using the provided information, and returns it.
    /// </summary>
    /// <returns>An error description.</returns>
    private string GetErrorDescription()
    {
        if (mErrors != null && mErrors.Length > 0)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<ul>");
            foreach (Error error in mErrors)
            {
                builder.AppendFormat("<li>{0}</li>", HTMLHelper.HTMLEncode(String.IsNullOrEmpty(error.ErrorMessage) ? error.ErrorCode : error.ErrorMessage));
            }
            builder.Append("</ul>");
            return builder.ToString();
        }

        return String.Empty;
    }


    /// <summary>
    /// Formats the error tooltip using the provided information, and returns it.
    /// </summary>
    /// <returns>An error tooltip. Not encoded.</returns>
    private string GetPlainErrorTooltip()
    {
        if (SystemContext.DevelopmentMode && mException != null)
        {
            return mException.Message ?? mException.ToString();
        }

        return String.Empty;
    }


    /// <summary>
    /// Creates an error message for the specified exception, and returns it.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    /// <returns>An error message for the specified exception.</returns>
    private string GetErrorMessageForException(Exception exception)
    {
        string resourceName = "datacom.exception";
        if (exception is WebException || (exception.InnerException != null && exception.InnerException is WebException))
        {
            resourceName = "datacom.serviceexception";
        }

        return GetString(resourceName);
    }


    /// <summary>
    /// Localizes Data.com errors into current UI language, if applicable.
    /// </summary>
    private void LocalizeErrors()
    {
        if (mErrors != null)
        {
            foreach (Error error in mErrors.Where(x => !String.IsNullOrEmpty(x.ErrorCode)))
            {
                string name = String.Format("datacom.error.{0}", error.ErrorCode);
                string message = ResHelper.GetAPIString(name, Thread.CurrentThread.CurrentUICulture.Name, String.Empty);
                if (!String.IsNullOrEmpty(message))
                {
                    error.ErrorMessage = message;
                }
            }
        }
    }

    #endregion
}