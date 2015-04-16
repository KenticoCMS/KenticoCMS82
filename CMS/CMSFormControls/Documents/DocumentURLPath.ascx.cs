using System;
using System.Collections;
using System.Text.RegularExpressions;

using CMS.DocumentEngine;
using CMS.FormControls;
using CMS.Helpers;
using CMS.Base;

public partial class CMSFormControls_Documents_DocumentURLPath : FormEngineUserControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets whether the custom option is available for the URL path
    /// </summary>
    public bool HideCustom
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets URL path.
    /// </summary>
    public override object Value
    {
        get
        {
            return URLPath;
        }
        set
        {
            URLPath = ValidationHelper.GetString(value, "");
        }
    }


    /// <summary>
    /// Gets or sets the automatic URL path that will be used in case custom is not set
    /// </summary>
    public string AutomaticURLPath
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the URL path
    /// </summary>
    public string URLPath
    {
        get
        {
            return GetURLPath();
        }
        set
        {
            SetURLPath(value);
        }
    }


    /// <summary>
    /// Plain URL path without the path prefix
    /// </summary>
    public string PlainURLPath
    {
        get
        {
            return txtUrlPath.Text.Trim();
        }
        set
        {
            txtUrlPath.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets whether the given URL is custom or not
    /// </summary>
    public bool IsCustom
    {
        get
        {
            if (HideCustom)
            {
                return true;
            }

            return chkCustomUrl.Checked;
        }
        set
        {
            chkCustomUrl.Checked = value;
        }
    }


    /// <summary>
    /// Gets or sets if control is enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            pnlContainer.Enabled = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// PreRender event handler
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        plcMVC.Visible = radMVC.Checked;

        // Set controls according the custom setting
        txtUrlPath.Enabled = IsCustom;
        pnlType.Enabled = IsCustom;

        plcCustom.Visible = !HideCustom;
    }


    /// <summary>
    /// Gets the current URL path
    /// </summary>
    protected string GetURLPath()
    {
        // Process the URL path
        string urlPath = txtUrlPath.Text.Trim();
        if (String.IsNullOrEmpty(urlPath))
        {
            return null;
        }
        urlPath = "/" + urlPath.TrimStart('/');

        if (radMVC.Checked)
        {
            string prefix = TreePathUtils.URL_PREFIX_MVC;

            // Add default controller and action
            if (!String.IsNullOrEmpty(txtController.Text))
            {
                prefix += "[Controller=" + txtController.Text.Trim() + "]:";
            }

            if (!String.IsNullOrEmpty(txtAction.Text))
            {
                prefix += "[Action=" + txtAction.Text.Trim() + "]:";
            }

            urlPath = prefix + urlPath;
        }
        else if (radRoute.Checked)
        {
            urlPath = TreePathUtils.URL_PREFIX_ROUTE + urlPath;
        }

        return urlPath;
    }


    /// <summary>
    /// Sets the control with a new URL path
    /// </summary>
    /// <param name="urlPath">URL path to set</param>
    protected void SetURLPath(string urlPath)
    {
        radMVC.Checked = false;
        radRoute.Checked = false;
        radPage.Checked = false;

        // Process the URL path
        if (String.IsNullOrEmpty(urlPath))
        {
            radPage.Checked = true;
            txtUrlPath.Text = "";
            return;
        }

        // Parse the path
        string prefix;
        Hashtable values = new Hashtable();

        TreePathUtils.ParseUrlPath(ref urlPath, out prefix, values);

        // Examine the prefix
        if (prefix.StartsWithCSafe(TreePathUtils.URL_PREFIX_MVC, true))
        {
            radMVC.Checked = true;
        }
        else if (prefix.StartsWithCSafe(TreePathUtils.URL_PREFIX_ROUTE, true))
        {
            radRoute.Checked = true;
        }
        else
        {
            radPage.Checked = true;
        }

        txtUrlPath.Text = urlPath;

        txtController.Text = ValidationHelper.GetString(values["controller"], "");
        txtAction.Text = ValidationHelper.GetString(values["action"], "");
    }


    protected void chkCustomUrl_CheckedChanged(object sender, EventArgs e)
    {
        if (!IsCustom)
        {
            SetURLPath(AutomaticURLPath);
        }
    }


    /// <summary>
    /// Returns true if the input is valid
    /// </summary>
    public override bool IsValid()
    {
        if ((radMVC.Checked || radRoute.Checked) && String.IsNullOrEmpty(txtUrlPath.Text.Trim()))
        {
            ValidationError = GetString("doc.urls.requiresurlpath");
            return false;
        }

        if (radMVC.Checked)
        {
            Regex regularExp = RegexHelper.GetRegex("{controller(?:;[^)]*)?}", RegexOptions.None);

            bool pathHasController = regularExp.IsMatch(txtUrlPath.Text);
            if (!pathHasController && String.IsNullOrEmpty(txtController.Text.Trim()))
            {
                ValidationError = GetString("MVC.ControllerEmpty");
                return false;   
            }

            regularExp = RegexHelper.GetRegex("{action(?:;[^)]*)?}", RegexOptions.None);

            bool pathHasAction = regularExp.IsMatch(txtUrlPath.Text);
            if (!pathHasAction && String.IsNullOrEmpty(txtAction.Text.Trim()))
            {
                ValidationError = GetString("MVC.ActionEmpty");
                return false;
            }
        }

        return base.IsValid();
    }

    #endregion
}