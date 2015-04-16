using System;
using System.Text;
using System.Text.RegularExpressions;

using CMS;
using CMS.Base;
using CMS.Core;
using CMS.Helpers;

using Microsoft.Ajax.Utilities;

[assembly: RegisterImplementation(typeof(IJavaScriptMinifier), typeof(AjaxJavaScriptMinifier))]

/// <summary>
/// Provides minification of JavaScript using the Microsoft Ajax Minifier.
/// </summary>
public sealed class AjaxJavaScriptMinifier : IJavaScriptMinifier
{
    #region "Variables"

    /// <summary>
    /// A value indicating whether the minification errors will be recorded in the event log.
    /// </summary>
    private static readonly bool mLogErrors;


    /// <summary>
    /// A regular expression matching multi-line JavaScript comments.
    /// </summary>
    private static readonly Regex mCommentRegex = new Regex(@"/\*.+?\*/", RegexOptions.Multiline);

    #endregion


    #region "Constructors"

    /// <summary>
    /// Initializes the <see cref="AjaxJavaScriptMinifier"/> class.
    /// </summary>
    static AjaxJavaScriptMinifier()
    {
        mLogErrors = ValidationHelper.GetBoolean(SettingsHelper.AppSettings["CMSLogJSMinifierParseError"], false);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Minifies the specified JavaScript.
    /// </summary>
    /// <param name="resource">The JavaScript to minify.</param>
    /// <returns>The minified JavaScript, if minification was successful; otherwise, the original JavaScript with minification errors appended at the end.</returns>
    public string Minify(string resource)
    {
        if (String.IsNullOrEmpty(resource))
        {
            return resource;
        }

        var settings = new CodeSettings
        {
            AllowEmbeddedAspNetBlocks = false,
            EvalTreatment = EvalTreatment.MakeAllSafe
        };
        var minifier = new Minifier();
        try
        {
            resource = minifier.MinifyJavaScript(resource, settings);
        }
        catch
        {
            var minificationErrors = String.Join(Environment.NewLine, minifier.Errors);
            resource = AppendMinificationErrors(resource, minificationErrors);

            if (mLogErrors)
            {
                CoreServices.EventLog.LogEvent("W", "Resource minification", "JavaScriptMinificationFailed", minificationErrors);
            }
        }

        return resource;
    }


    /// <summary>
    /// Appends minification errors at the end of the specified JavaScript as a comment.
    /// </summary>
    /// <param name="resource">The JavaScript to append minification errors to.</param>
    /// <param name="minificationErrors">Minification errors to append.</param>
    /// <returns>The specified JavaScript with minification errors appended at the end as a comment.</returns>
    private string AppendMinificationErrors(string resource, string minificationErrors)
    {
        var builder = new StringBuilder(resource);
        var epilogue = mCommentRegex.Replace(minificationErrors, String.Empty);
        builder.AppendLine().AppendLine().AppendLine("/* Minification failed").AppendLine(epilogue).Append("*/");

        return builder.ToString();
    }

    #endregion
}