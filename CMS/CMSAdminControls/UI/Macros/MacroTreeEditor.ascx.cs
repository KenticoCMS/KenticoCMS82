using System;
using System.Web.UI;

using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Helpers;
using CMS.MacroEngine;

public partial class CMSAdminControls_UI_Macros_MacroTreeEditor : FormEngineUserControl
{
    #region "Variables"

    private bool mShowMacroTreeAbove = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets client ID of the editor.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return editorElem.Editor.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets text in the editor.
    /// </summary>
    public override string Text
    {
        get
        {
            return editorElem.Editor.Text;
        }
        set
        {
            editorElem.Editor.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
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
            editorElem.Editor.Enabled = value;
            btnShowTree.Enabled = false;
        }
    }


    /// <summary>
    /// Name of the resolver to use.
    /// </summary>
    public override string ResolverName
    {
        get
        {
            return editorElem.ResolverName;
        }
        set
        {
            editorElem.ResolverName = value;
            treeElem.ResolverName = value;
        }
    }


    /// <summary>
    /// Resolver to use.
    /// </summary>
    public MacroResolver Resolver
    {
        get
        {
            return editorElem.Resolver;
        }
        set
        {
            editorElem.Resolver = value;
            treeElem.ContextResolver = value;
        }
    }


    /// <summary>
    /// Gets the ExtendedTextArea control.
    /// </summary>
    public ExtendedTextArea Editor
    {
        get
        {
            return editorElem.Editor;
        }
    }


    /// <summary>
    /// If true, tree is shown above the editor, otherwise it is below (default position is below).
    /// </summary>
    public bool ShowMacroTreeAbove
    {
        get
        {
            return mShowMacroTreeAbove;
        }
        set
        {
            mShowMacroTreeAbove = value;
        }
    }


    /// <summary>
    /// Gets or sets the left offset of the autocomplete control (to position it correctly).
    /// </summary>
    public int LeftOffset
    {
        get
        {
            return editorElem.LeftOffset;
        }
        set
        {
            editorElem.LeftOffset = value;
        }
    }


    /// <summary>
    /// Gets or sets the top offset of the autocomplete control (to position it correctly).
    /// </summary>
    public int TopOffset
    {
        get
        {
            return editorElem.TopOffset;
        }
        set
        {
            editorElem.TopOffset = value;
        }
    }


    /// <summary>
    /// Gets the name of java script object of the auto completion extender.
    /// </summary>
    public string AutoCompletionObject
    {
        get
        {
            return editorElem.AutoCompletionObject;
        }
    }


    /// <summary>
    /// Indicates whether the control is on live site or not
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
            treeElem.IsLiveSite = value;
            editorElem.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets field value. You need to override this method to make the control work properly with the form.
    /// </summary>
    public override object Value
    {
        get
        {
            return Editor.Text;
        }
        set
        {
            Editor.Text = ValidationHelper.GetString(value, "");
        }
    }


    /// <summary>
    /// If true, Tree element is wrapped in the div with position:absolute.
    /// </summary>
    public bool AbsolutePosition
    {
        get;
        set;
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        editorElem.Editor.ShowToolbar = false;
        editorElem.Editor.DynamicHeight = true;
        editorElem.ShowAutoCompletionAbove = ShowMacroTreeAbove;

        treeElem.OnNodeClickHandler = "nodeClick_" + ClientID;

        btnShowTree.ToolTip = GetString("macros.editor.showhidetree");
        btnShowTree.OnClientClick = GetShowHideScript();

        pnlMacroTree.Style.Add("position", "absolute");
        pnlMacroTree.Style.Add("display", "none");
        pnlMacroTree.Attributes.Add("onmouseover", "macroTreeHasFocus = true;");
        pnlMacroTree.Attributes.Add("onmouseout", "macroTreeHasFocus = false;");

        if (AbsolutePosition)
        {
            pnlTreeWrapper.Style.Add("position", "absolute");
        }
        else
        {
            pnlTreeWrapper.Style.Add("position", "relative");
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Register the main script
        ScriptHelper.RegisterScriptFile(Page, "Macros/MacroTreeEditor.js");
    }


    protected override void Render(HtmlTextWriter writer)
    {
        base.Render(writer);

        // We need to generate this script on Render since extended area does that that late as well
        // and editor object is not available before
        string script = null;
        if (editorElem.Editor.SyntaxHighlightingEnabled)
        {
            script = "function nodeClick_" + ClientID + "(macro) { nodeClick(macro, " + editorElem.Editor.EditorID + ", '" + pnlMacroTree.ClientID + "', '" + editorElem.Editor.ClientID + "'); }";
        }
        else
        {
            script = "function nodeClick_" + ClientID + "(macro) { nodeClick(macro, null, '" + pnlMacroTree.ClientID + "', '" + editorElem.Editor.ClientID + "'); }";
        }

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "MacroTreeEditorScript_" + ClientID, script, true);
    }


    private string GetShowHideScript()
    {
        if (editorElem.Editor.SyntaxHighlightingEnabled)
        {
            return "showHideMacroTree('" + pnlMacroTree.ClientID + "', " + editorElem.Editor.EditorID + ", " + editorElem.AutoCompletionObject + ", " + LeftOffset + ", " + TopOffset + ", " + (ShowMacroTreeAbove ? "true" : "false") + ", false); return false;";
        }
        else
        {
            return "showHideMacroTree('" + pnlMacroTree.ClientID + "', null, null, " + LeftOffset + ", " + TopOffset + ", " + (ShowMacroTreeAbove ? "true" : "false") + ", false); return false;";
        }
    }
}