using System;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.PortalControls;
using CMS.PortalEngine;

public partial class CMSWebParts_Viewers_DateTime : CMSAbstractWebPart
{
    #region "Javascript properties"

    /// <summary>
    /// Gets or sets the value that indicates whether to use server time or not.
    /// </summary>
    public bool JsUseServerTime
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("JsUseServerTime"), false);
        }
        set
        {
            SetValue("JsUseServerTime", value);
        }
    }


    /// <summary>
    /// Gets or sets the date time format (ie. "dd.mm.yy").
    /// </summary>
    public string JsFormat
    {
        get
        {
            return ValidationHelper.GetString(GetValue("JsFormat"), "dd.m.yy");
        }
        set
        {
            SetValue("JsFormat", value);
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControl();
        }
    }


    /// <summary>
    /// Page prerender.
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        bool checkCollision = false;
        if (ParentZone != null)
        {
            checkCollision = ParentZone.WebPartManagementRequired;
        }
        else
        {
            checkCollision = PortalContext.IsDesignMode(ViewMode, false);
        }
        if (ScriptHelper.IsPrototypeBoxRegistered() && checkCollision)
        {
            Label lblError = new Label();
            lblError.EnableViewState = false;
            lblError.CssClass = "ErrorLabel";
            lblError.Text = GetString("javascript.mootoolsprototype");
            Controls.Clear();
            Controls.Add(lblError);
        }
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        // If Content Slider is in design mode do not start scripts (IE z-index problem)
        if (PortalContext.ViewMode != ViewModeEnum.Design)
        {
            ltlDateTime.Text = "<div id=\"time_" + ClientID + "\" ></div>";

            //Register mootools javascript framework
            ScriptHelper.RegisterMooTools(Page);

            // Register DateTime.js script file
            ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Viewers/DateTime_files/DateTime.js");

            string jScript = "window.addEvent('load',function(){\n" +
                             "var now = new Date();\n";

            if (JsUseServerTime)
            {
                jScript += "var local = now.getTime()\n" +
                           "var server = " + Math.Round((DateTime.Now.ToUniversalTime() - DateTimeHelper.UNIX_TIME_START).TotalMilliseconds, 0, MidpointRounding.AwayFromZero) + "\n" +
                           "var diff = server - local;\n";
            }
            else
            {
                jScript += "var diff = 0;";
            }
            jScript += "startTimer(\"" + ClientID + "\",\"" + JsFormat + "\",diff)\n" +
                       "});";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), ("timerScript" + ClientID), ScriptHelper.GetScript(jScript));
        }
        else
        {
            ltlDateTime.Text = "<div id=\"time_" + ClientID + "\" >Timer</div>";
        }
    }

    #endregion
}