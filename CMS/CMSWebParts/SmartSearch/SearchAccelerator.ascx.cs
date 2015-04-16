using System;
using System.Web;

using CMS.Helpers;
using CMS.PortalControls;
using CMS.Search;

public partial class CMSWebParts_SmartSearch_SearchAccelerator : CMSAbstractWebPart
{
    #region "Variables"

    // Result page url
    protected string mResultPageUrl = RequestContext.CurrentURL;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the accelerator description.
    /// </summary>
    public string AcceleratorDescription
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("AcceleratorDescription"), GetString("srch.accelerator.description"));
        }
        set
        {
            SetValue("AcceleratorDescription", value);
        }
    }


    /// <summary>
    /// Gets or sets the accelerator name.
    /// </summary>
    public string AcceleratorName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("AcceleratorName"), GetString("srch.accelerator.name"));
        }
        set
        {
            SetValue("AcceleratorName", value);
        }
    }


    /// <summary>
    /// Gets or sets the accelerator button text.
    /// </summary>
    public string AcceleratorButtonText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("AcceleratorButtonText"), GetString("srch.accelerator.addaccelerator"));
        }
        set
        {
            SetValue("AcceleratorButtonText", value);
        }
    }


    /// <summary>
    /// Gets or sets the search results page URL.
    /// </summary>
    public string SearchResultsPageUrl
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SearchResultsPageUrl"), mResultPageUrl);
        }
        set
        {
            SetValue("SearchResultsPageUrl", value);
            mResultPageUrl = value;
        }
    }


    /// <summary>
    ///  Gets or sets the Search mode.
    /// </summary>
    public SearchModeEnum SearchMode
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SearchMode"), "").ToEnum<SearchModeEnum>();
        }
        set
        {
            SetValue("SearchMode", value.ToStringRepresentation());
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();

        // If the accelerator request matches the ID of the control, 
        if (QueryHelper.GetString("getsearchaccelerator", "") == ClientID)
        {
            GetServiceDefinition();
        }

        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            return;
        }
        else
        {
            btnAcc.Text = AcceleratorButtonText;

            string serviceUrl = URLHelper.GetAbsoluteUrl(URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "getsearchaccelerator", ClientID));

            btnAcc.OnClientClick = "window.external.AddService('" + serviceUrl + "'); return false;";
        }
    }


    /// <summary>
    /// Gets the service definition for the accelerator.
    /// </summary>
    public void GetServiceDefinition()
    {
        // Create a new XML response
        Response.Clear();
        Response.ContentType = "text/xml";

        string appUrl = URLHelper.GetApplicationUrl();

        // Write acelerator definition
        Response.Write(
            @"<?xml version=""1.0"" encoding=""UTF-8"" ?>
                <os:openServiceDescription xmlns:os=""http://www.microsoft.com/schemas/openservicedescription/1.0""> 
                    <os:homepageUrl>" + appUrl + @"</os:homepageUrl> 
                    <os:display> 
                        <os:name>" + AcceleratorName + @"</os:name> 
                        <os:icon>" + appUrl + @"/favicon.ico</os:icon> 
                        <os:description>" + AcceleratorDescription + @"</os:description> 
                    </os:display> 
                    <os:activity category=""Search""> 
                        <os:activityAction context=""selection""> 
                            <os:execute action=""" + HttpUtility.HtmlEncode(URLHelper.GetAbsoluteUrl(SearchResultsPageUrl)) + @"""> 
                                <os:parameter name=""searchtext"" value=""{selection}"" type=""text"" /> 
                                <os:parameter name=""searchmode"" value=""" + SearchMode.ToStringRepresentation() + @""" /> 
                            </os:execute> 
                        </os:activityAction> 
                    </os:activity> 
                </os:openServiceDescription>");

        RequestHelper.EndResponse();
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }

    #endregion
}