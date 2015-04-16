using System;

using CMS.Controls.Configuration;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;

public partial class CMSAdminControls_UI_Macros_MacroDesigner : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Returns resulting condition.
    /// </summary>
    public override object Value
    {
        get
        {
            switch (ValidationHelper.GetInteger(hdnSelTab.Value, 0))
            {
                case 1:
                    return editorElem.Text;

                default:
                    return ruleElem.Value;
            }
        }
        set
        {
            string val = ValidationHelper.GetString(value, "");
            if (val.EndsWithCSafe("%}"))
            {
                val = val.Substring(0, val.Length - 2);
            }
            if (val.StartsWithCSafe("{%"))
            {
                val = val.Substring(2);
            }

            string condition = ruleElem.ConditionFromExpression(val);
            hdnCondition.Value = condition;

            int tab = ValidationHelper.GetInteger(CookieHelper.GetValue(CookieName.MacroDesignerTab), 0);

            if (val == "")
            {
                if (tab == 1)
                {
                    ShowCode();
                }
                else
                {
                    ShowRuleEditor();
                }
            }
            else
            {
                if (condition != val)
                {
                    // If condition is different from whole expression, it means that it's rule editor
                    ShowRuleEditor();
                    ruleElem.Value = val;
                }
                else
                {
                    editorElem.Text = condition;
                    ShowCode();
                }
            }
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed by default when there is no rule defined.
    /// </summary>
    public string DefaultConditionText
    {
        get
        {
            return ruleElem.DefaultConditionText;
        }
        set
        {
            ruleElem.DefaultConditionText = value;
        }
    }


    /// <summary>
    /// Returns the editor object.
    /// </summary>
    public ExtendedTextArea Editor
    {
        get
        {
            return editorElem.Editor;
        }
    }


    /// <summary>
    /// Gets or sets name(s) of the Macro rule category(ies) which should be displayed in Rule designer. Items should be separated by semicolon.
    /// </summary>
    public string RuleCategoryNames
    {
        get
        {
            return ruleElem.RuleCategoryNames;
        }
        set
        {
            ruleElem.RuleCategoryNames = value;
        }
    }


    /// <summary>
    /// Determines whether the global rules are shown among with the specific rules defined in the RuleCategoryNames property.
    /// </summary>
    public bool ShowGlobalRules
    {
        get
        {
            return ruleElem.ShowGlobalRules;
        }
        set
        {
            ruleElem.ShowGlobalRules = value;
        }
    }


    /// <summary>
    /// Determines which rules to display. 0 means all rules, 1 means only rules which does not require context, 2 only rules which require context.
    /// </summary>
    public int DisplayRuleType
    {
        get
        {
            return ruleElem.DisplayRuleType;
        }
        set
        {
            ruleElem.DisplayRuleType = value;
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
            ruleElem.ResolverName = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        editorElem.Editor.UseSmallFonts = true;

        btnShowCode.Click += btnShowCode_Click;
        btnShowRuleEditor.Click += btnShowRuleEditor_Click;

        // Init tabs
        tabsElem.UrlTarget = "_self";

        tabsElem.AddTab(new UITabItem()
        {
            Text = GetString("macrodesigner.ruleeditor"),
            Tooltip = GetString("macrodesigner.codetooltip"),
            RedirectUrl = "",
            OnClientClick = "if (GetSelected() != 0) { if (DisplayWarning(true)) {" + ControlsHelper.GetPostBackEventReference(btnShowRuleEditor) + "; } else { SelTab(GetSelected(), '_self', '') }} return false;",
        });

        tabsElem.AddTab(new UITabItem()
        {
            Text = GetString("macrodesigner.code"),
            Tooltip = GetString("macrodesigner.codetooltip"),
            RedirectUrl = "",
            OnClientClick = "if (GetSelected() != 1) { if (DisplayWarning(true)) {" + ControlsHelper.GetPostBackEventReference(btnShowCode) + "; } else { SelTab(GetSelected(), '_self', '') }} return false;",
        });

        // Register move script
        string script = string.Format(@"
function GetSelected() {{ 
  return document.getElementById('{0}').value;
}}

function DisplayWarning(checkCodeChange) {{ 
  var selectedTab = GetSelected();
  if (selectedTab == 0) {{
    return confirm({1});
  }}
  if (checkCodeChange && (selectedTab == 1)) {{
    var hidden = document.getElementById('{2}');
    if (hidden.value != {3}) {{
      return confirm({4});
    }}
  }}
  return true;
}}
", hdnSelTab.ClientID, ScriptHelper.GetString(GetString("macrodesigner.switchfromruleeditor")), hdnCondition.ClientID, editorElem.Editor.EditorID + ".getValue()", ScriptHelper.GetString(GetString("macrodesigner.switchtoruledesigner")));

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "DesignerOnDropGroup", script, true);
        ltlScript.Text = ScriptHelper.GetScript("var parsingError = ''; var groupLocations = new Array();");
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        SelectTab(ValidationHelper.GetInteger(hdnSelTab.Value, 0));
    }


    protected void btnShowRuleEditor_Click(object sender, EventArgs e)
    {
        ShowRuleEditor();
    }


    protected void btnShowCode_Click(object sender, EventArgs e)
    {
        ShowCode();
    }


    private void ShowRuleEditor()
    {
        if (hdnCondition.Value != editorElem.Text)
        {
            // Erase the designer when the condition is different
            ruleElem.Value = "";
        }
        SaveValue();

        pnlEditor.Visible = false;
        pnlRuleEditor.Visible = true;

        SelectTab(0);
    }


    private void ShowCode()
    {
        SaveValue();
        editorElem.Text = hdnCondition.Value;

        pnlRuleEditor.Visible = false;
        pnlEditor.Visible = true;

        SelectTab(1);
    }


    private void SaveValue()
    {
        if (RequestHelper.IsPostBack())
        {
            switch (ValidationHelper.GetInteger(hdnSelTab.Value, 0))
            {
                case 1:
                    hdnCondition.Value = editorElem.Text;
                    break;

                default:
                    var condition = ruleElem.GetCondition();
                    if (!string.IsNullOrEmpty(condition))
                    {
                        hdnCondition.Value = condition;
                    }
                    break;
            }
        }
    }


    private void SelectTab(int num)
    {
        tabsElem.SelectedTab = num;
        hdnSelTab.Value = num.ToString();

        CookieHelper.SetValue(CookieName.MacroDesignerTab, num.ToString(), DateTime.Now.AddDays(1));
    }

    #endregion
}
