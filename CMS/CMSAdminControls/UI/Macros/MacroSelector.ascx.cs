using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.UIControls;
using CMS.MacroEngine;

public partial class CMSAdminControls_UI_Macros_MacroSelector : CMSUserControl
{
    #region "Variables"

    /// <summary>
    /// Macro resolver.
    /// </summary>
    protected MacroResolver mResolver = null;

    /// <summary>
    /// Javascript function called on submit.
    /// </summary>
    protected string mJavaScriptFunction = "InsertMacro";

    /// <summary>
    /// CKEditor client ID.
    /// </summary>
    protected string mCKEditorID = null;

    /// <summary>
    /// Text area client ID.
    /// </summary>
    protected string mTextAreaID = null;

    /// <summary>
    /// Extended text area JS element object.
    /// </summary>
    protected string mExtendedTextAreaElem = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Macro resolver.
    /// </summary>
    public MacroResolver Resolver
    {
        get
        {
            return mResolver;
        }
        set
        {
            mResolver = value;
        }
    }


    /// <summary>
    /// Javascript function called on submit.
    /// </summary>
    public string JavaScriptFunction
    {
        get
        {
            return mJavaScriptFunction;
        }
        set
        {
            mJavaScriptFunction = value;
        }
    }


    /// <summary>
    /// Gets or sets the left offset of the autocomplete control (to position it correctly).
    /// </summary>
    public int LeftOffset
    {
        get
        {
            return macroElem.LeftOffset;
        }
        set
        {
            macroElem.LeftOffset = value;
        }
    }


    /// <summary>
    /// Gets or sets the top offset of the autocomplete control (to position it correctly).
    /// </summary>
    public int TopOffset
    {
        get
        {
            return macroElem.TopOffset;
        }
        set
        {
            macroElem.TopOffset = value;
        }
    }


    /// <summary>
    /// If true, tree is shown above the editor, otherwise it is below (default position is below).
    /// </summary>
    public bool ShowMacroTreeAbove
    {
        get
        {
            return macroElem.ShowMacroTreeAbove;
        }
        set
        {
            macroElem.ShowMacroTreeAbove = value;
        }
    }


    /// <summary>
    /// Gets the ExtendedTextArea control.
    /// </summary>
    public ExtendedTextArea Editor
    {
        get
        {
            return macroElem.Editor;
        }
    }


    /// <summary>
    /// Gets or sets ID of the CKEditor. If this property is set, script for insertion is automatically registered.
    /// </summary>
    public string CKEditorID
    {
        get
        {
            return mCKEditorID;
        }
        set
        {
            mCKEditorID = value;
        }
    }


    /// <summary>
    /// Gets or sets element of the ExtendedTextArea. If this property is set, script for insertion is automatically registered.
    /// </summary>
    public string ExtendedTextAreaElem
    {
        get
        {
            return mExtendedTextAreaElem;
        }
        set
        {
            mExtendedTextAreaElem = value;
        }
    }


    /// <summary>
    /// Gets or sets ID of the TextArea. If this property is set, script for insertion is automatically registered.
    /// </summary>
    public string TextAreaID
    {
        get
        {
            return mTextAreaID;
        }
        set
        {
            mTextAreaID = value;
        }
    }


    /// <summary>
    /// Indicates wheter the control is on live site or not
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
            macroElem.IsLiveSite = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (mResolver == null)
        {
            lblError.Text = "[MacroSelector.ascx]: You need to define \"Resolver\" property for this control to work properly.";
            lblError.ForeColor = Color.Red;
            lblError.Visible = true;

            return;
        }

        // Register script for CKEditor
        if (!string.IsNullOrEmpty(CKEditorID))
        {
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "MacroInsertCKEditor_" + ClientID,
                                                   ScriptHelper.GetScript("function InsertMacro_" + ClientID + "(macro) { var oEditor = CKEDITOR.instances['" + CKEditorID + "'] ; if (oEditor != null) { if (oEditor.mode == 'wysiwyg' ) { oEditor.insertHtml(macro); }  else { alert('" + GetString("macroselector.wysiwigerror") + "') } } } "));
        }
        else if (!string.IsNullOrEmpty(ExtendedTextAreaElem) || !string.IsNullOrEmpty(TextAreaID))
        {
            // Register scripts for text areas
            ScriptHelper.RegisterScriptFile(this.Page, "Macros/MacroSelector.js");

            if (!string.IsNullOrEmpty(ExtendedTextAreaElem))
            {
                ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "MacroInsertExtendedTextArea_" + ClientID,
                                                       ScriptHelper.GetScript("function InsertMacro_" + ClientID + "(macro) { InsertMacroExtended(macro, (typeof(" + ExtendedTextAreaElem + ") != 'undefined' ? " + ExtendedTextAreaElem + " : null), '" + TextAreaID + "'); }"));
            }
            else
            {
                ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "MacroInsertTextArea_" + ClientID,
                                                       ScriptHelper.GetScript("function InsertMacro_" + ClientID + "(macro) { InsertMacroPlain(macro, '" + TextAreaID + "'); }"));
            }
        }

        btnInsert.OnClientClick = GetInsertScript() + " return false;";
        macroElem.Resolver = Resolver;
    }


    /// <summary>
    /// Returns insert script for macro.
    /// </summary>
    private string GetInsertScript()
    {
        StringBuilder builder = new StringBuilder();

        // CKEditorID setting has higher priority, therefore override script name
        string scriptName = JavaScriptFunction;
        if (!string.IsNullOrEmpty(CKEditorID) || !string.IsNullOrEmpty(ExtendedTextAreaElem) || !string.IsNullOrEmpty(TextAreaID))
        {
            scriptName = "InsertMacro_" + ClientID;
        }

        // Call javascript macro function
        builder.Append(
            "if (window." + scriptName + ") {",
            scriptName + "('{%' + " + macroElem.Editor.GetValueGetterCommand() + ".replace(/(\\r\\n|\\n|\\r)+/, '') + '%}');",
            "} else { alert('No insert function!'); } ");

        return builder.ToString();
    }

    #endregion
}