using System.Web.UI;
using System;

using CMS.Helpers;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.Base;

public partial class CMSWebParts_General_javascript : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the inline JavaScript code.
    /// </summary>
    public string InlineScript
    {
        get
        {
            return ValidationHelper.GetString(GetValue("InlineScript"), string.Empty);
        }
        set
        {
            SetValue("InlineScript", value);
        }
    }


    /// <summary>
    /// Gets or sets the inline JavaScript code page location.
    /// </summary>
    public string InlineScriptPageLocation
    {
        get
        {
            return ValidationHelper.GetString(GetValue("InlineScriptPageLocation"), string.Empty);
        }
        set
        {
            SetValue("InlineScriptPageLocation", value);
        }
    }


    /// <summary>
    /// Indicates whether the script tags are generated or not.
    /// </summary>
    public bool GenerateScriptTags
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("GenerateScriptTags"), true);
        }
        set
        {
            SetValue("GenerateScriptTags", value);
        }
    }


    /// <summary>
    /// Gets or sets the linked file url.
    /// </summary>
    public string LinkedFile
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LinkedFile"), string.Empty);
        }
        set
        {
            SetValue("LinkedFile", value);
        }
    }


    /// <summary>
    /// Gets or sets the linked file url page location.
    /// </summary>
    public string LinkedFilePageLocation
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LinkedFilePageLocation"), string.Empty);
        }
        set
        {
            SetValue("LinkedFilePageLocation", value);
        }
    }

    #endregion


    #region "Methods"


    /// <summary>
    /// Registers the control scripts
    /// </summary>
    protected void RegisterScripts()
    {
        // Include javascript only in live site or preview mode
        ViewModeEnum viewMode = PortalContext.ViewMode;
        if (viewMode != ViewModeEnum.Design)
        {
            RegisterLinkedFiles();
            RegisterInlineScript();
        }
    }


    /// <summary>
    /// Registers the inline script
    /// </summary>
    private void RegisterInlineScript()
    {
        // Render the inline script
        if (InlineScript.Trim() != string.Empty)
        {
            string inlineScript = InlineScript;

            // Check if script tags must be generated
            if (GenerateScriptTags && (InlineScriptPageLocation.ToLowerCSafe() != "submit"))
            {
                inlineScript = ScriptHelper.GetScript(InlineScript);
            }

            // Switch for script position on the page
            switch (InlineScriptPageLocation.ToLowerCSafe())
            {
                case "header":
                    Page.Header.Controls.Add(new LiteralControl(inlineScript));
                    break;

                case "beginning":
                    ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), ClientID + "inlinescript", inlineScript);
                    break;

                case "startup":
                    ScriptHelper.RegisterStartupScript(Page, typeof(string), ClientID + "inlinescript", inlineScript);
                    break;

                case "submit":
                    ScriptHelper.RegisterOnSubmitStatement(Page, typeof(string), ClientID + "inlinescript", inlineScript);
                    break;

                default:
                    ltlInlineScript.Text = inlineScript;
                    break;
            }
        }
    }


    /// <summary>
    /// Registers the linked files
    /// </summary>
    private void RegisterLinkedFiles()
    {
        // Create linked JS file
        if (LinkedFile.Trim() != string.Empty)
        {
            string linkedFile = String.Empty;

            // Register some script files manually, to prevent multiple registration
            switch (LinkedFile.Trim().ToLowerCSafe())
            {
                case "jquery":
                    if (ScriptHelper.RequestScriptRegistration(ScriptHelper.JQUERY_KEY))
                    {
                        linkedFile = ScriptHelper.JQUERY_FILENAME;
                    }
                    break;

                case "prototype":
                    if (ScriptHelper.RequestScriptRegistration(ScriptHelper.PROTOTYPE_KEY))
                    {
                        linkedFile = ScriptHelper.PROTOTYPE_FILENAME;
                    }
                    break;

                case "mootools":
                    if (ScriptHelper.RequestScriptRegistration(ScriptHelper.MOOTOOLS_KEY))
                    {
                        linkedFile = ScriptHelper.MOOTOOLS_FILENAME;
                    }
                    break;

                case "silverlight":
                    if (ScriptHelper.RequestScriptRegistration(ScriptHelper.SILVERLIGHT_KEY))
                    {
                        linkedFile = ScriptHelper.SILVERLIGHT_FILENAME;
                    }
                    break;

                default:
                    {
                        // File URL
                        string file = LinkedFile;

                        string key = ScriptHelper.SCRIPTFILE_PREFIX_KEY + file;
                        if (ScriptHelper.RequestScriptRegistration(key))
                        {
                            linkedFile = ScriptHelper.GetScriptUrl(file, false);
                            linkedFile = ResolveUrl(linkedFile);
                        }
                    }
                    break;
            }

            if (!String.IsNullOrEmpty(linkedFile))
            {
                string script = ScriptHelper.GetIncludeScript(linkedFile);

                // Switch for script position on the page
                switch (LinkedFilePageLocation.ToLowerCSafe())
                {
                    case "beginning":
                        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), ClientID + "script", script);
                        break;

                    case "startup":
                        ScriptHelper.RegisterStartupScript(Page, typeof(string), ClientID + "script", script);
                        break;

                    case "header":
                    default:
                        Page.Header.Controls.Add(new LiteralControl(script));
                        break;
                }
            }
        }
    }
    

    /// <summary>
    /// PreRender event handler
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!StopProcessing)
        {
            RegisterScripts();
        }
    }

    #endregion
}