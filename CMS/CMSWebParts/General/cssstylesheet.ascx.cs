using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Helpers;
using CMS.PortalControls;

public partial class CMSWebParts_General_cssstylesheet : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets path to the stylesheet file.
    /// </summary>
    public string FilePath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilePath"), "");
        }
        set
        {
            SetValue("FilePath", value);
        }
    }


    /// <summary>
    /// Gets or sets the media type.
    /// </summary>
    public string Media
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Media"), "screen");
        }
        set
        {
            SetValue("Media", value);
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
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    /// <param name="e">EventArgs</param>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!StopProcessing)
        {
            // Add link to page header
            string url = CSSHelper.GetPhysicalCSSUrl(FilePath);
            string link = CSSHelper.GetCSSFileLink(url, Media);

            LiteralControl ltlCss = new LiteralControl(link);
            ltlCss.EnableViewState = false;

            Page.Header.Controls.Add(ltlCss);
        }
    }

    #endregion
}