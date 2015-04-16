using System;
using System.Text;
using System.Text.RegularExpressions;

using CMS;
using CMS.Base;
using CMS.Core;
using CMS.Helpers;

using Microsoft.Ajax.Utilities;

[assembly: RegisterImplementation(typeof(ICssMinifier), typeof(AjaxCssMinifier))]

/// <summary>
/// Provides minification of CSS using the Microsoft Ajax Minifier.
/// </summary>
public sealed class AjaxCssMinifier : ICssMinifier
{
    #region "Variables"

    /// <summary>
    /// A value indicating whether the minification errors will be recorded in the event log.
    /// </summary>
    private static readonly bool mLogErrors;


    /// <summary>
    /// A regular expression matching multi-line CSS comments.
    /// </summary>
    private static readonly Regex mCommentRegex = new Regex(@"/\*.+?\*/", RegexOptions.Multiline);

    #endregion


    #region "Constructors"

    /// <summary>
    /// Initializes the <see cref="AjaxCssMinifier"/> class.
    /// </summary>
    static AjaxCssMinifier()
    {
        mLogErrors = ValidationHelper.GetBoolean(SettingsHelper.AppSettings["CMSLogCSSMinifierParseError"], false);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Minifies the specified CSS.
    /// </summary>
    /// <param name="resource">The CSS to minify.</param>
    /// <returns>The minified CSS, if minification was successful; otherwise, the original CSS with minification errors appended at the end.</returns>
    public string Minify(string resource)
    {
        if (String.IsNullOrEmpty(resource))
        {
            return resource;
        }

        var settings = new CssSettings
        {
            AllowEmbeddedAspNetBlocks = false
        };
        var minifier = new Minifier();
        try
        {
            resource = minifier.MinifyStyleSheet(resource, settings);
        }
        catch
        {
            var minificationErrors = String.Join(Environment.NewLine, minifier.Errors);
            resource = AppendMinificationErrors(resource, minificationErrors);

            if (mLogErrors)
            {
                CoreServices.EventLog.LogEvent("W", "Resource minification", "CssMinificationFailed", minificationErrors);
            }
        }

        return resource;
    }


    /// <summary>
    /// Appends minification errors at the end of the specified CSS as a comment.
    /// </summary>
    /// <param name="resource">The CSS to append minification errors to.</param>
    /// <param name="minificationErrors">Minification errors to append.</param>
    /// <returns>The specified CSS with minification errors appended at the end as a comment.</returns>
    private string AppendMinificationErrors(string resource, string minificationErrors)
    {
        var builder = new StringBuilder(resource);
        var epilogue = mCommentRegex.Replace(minificationErrors, String.Empty);
        builder.AppendLine().AppendLine().AppendLine("/* Minification failed").AppendLine(epilogue).Append("*/");

        return builder.ToString();
    }

    #endregion
}