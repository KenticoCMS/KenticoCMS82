using System;
using System.Collections;
using System.Web.UI;

using CMS.Base;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;


public partial class CMSFormControls_System_FieldMacroRuleEditor : FormEngineUserControl
{
    #region "Variables"

    private FieldMacroRule mMacroRule;
    private MacroRuleTree mMacroRuleTree;
    private bool mParamsInitialized;

    #endregion


    #region "Properties"

    /// <summary>
    /// Control works with <see cref="FieldMacroRule"/>.
    /// </summary>
    public override object Value
    {
        get
        {
            return mMacroRule = CreateMacroRule();
        }
        set
        {
            mMacroRule = (FieldMacroRule)value;
            if (mMacroRule != null)
            {
                string userName = null;
                string rule = MacroProcessor.RemoveDataMacroBrackets(mMacroRule.MacroRule);
                rule = MacroSecurityProcessor.RemoveMacroSecurityParams(rule, out userName);
                rule = ValidationHelper.GetString(MacroExpression.ExtractParameter(rule, "rule", 1).Value, string.Empty);

                mMacroRuleTree = new MacroRuleTree();
                mMacroRuleTree.LoadFromXml(rule);
                mSelectedRuleName = mMacroRuleTree.Children[0].RuleName;
            }
            else
            {
                // Reset inner controls
                mSelectedRuleName = null;
                uniSelector.Value = null;
            }

            mParamsInitialized = false;
        }
    }


    /// <summary>
    /// Codename of selected rule.
    /// </summary>
    private string mSelectedRuleName
    {
        get
        {
            string selectedValue = ValidationHelper.GetString(ViewState["LastSelected"], string.Empty);
            if (string.IsNullOrEmpty(selectedValue) && (mMacroRuleTree != null))
            {
                selectedValue = mMacroRuleTree.Children[0].RuleName;
            }
            if (string.IsNullOrEmpty(selectedValue))
            {
                selectedValue = ValidationHelper.GetString(uniSelector.Value, string.Empty);
            }
            ViewState["LastSelected"] = selectedValue;

            return selectedValue;
        }
        set
        {
            ViewState["LastSelected"] = value;
        }
    }


    /// <summary>
    /// Default error message which will be used as watermark.
    /// </summary>
    public string DefaultErrorMessage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DefaultErrorMessage"), string.Empty);
        }
        set
        {
            SetValue("DefaultErrorMessage", value);
        }
    }

    #endregion


    #region "Control events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        uniSelector.DropDownSingleSelect.AutoPostBack = true;
        formProperties.SubmitButton.Visible = false;
        formProperties.SubmitButton.RegisterHeaderAction = false;
    }


    /// <summary>
    /// Customized LoadViewState.
    /// </summary>
    protected override void LoadViewState(object savedState)
    {
        base.LoadViewState(((Pair)savedState).First);

        if (!StopProcessing)
        {
            InitializeControl();
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!mParamsInitialized && !StopProcessing)
        {
            Reload();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!mParamsInitialized && !StopProcessing)
        {
            Reload();
        }
    }


    /// <summary>
    /// Handles rule selector change.
    /// </summary>
    protected void uniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        mSelectedRuleName = ValidationHelper.GetString(uniSelector.Value, string.Empty);
        InitializeControl(true);
    }


    /// <summary>
    /// Customized SaveViewState.
    /// </summary>
    protected override object SaveViewState()
    {
        return new Pair(base.SaveViewState(), null);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns if control's inputs are valid.
    /// </summary>
    public override bool IsValid()
    {
        return formProperties.ValidateData();
    }


    /// <summary>
    /// Initializes inner controls.
    /// </summary>
    /// <param name="forceReload">Indicates if form with parameters should be reloaded</param>
    private void InitializeControl(bool forceReload = false)
    {
        // Init rule selector
        uniSelector.Value = mSelectedRuleName;

        MacroRuleInfo mri = MacroRuleInfoProvider.GetMacroRuleInfo(mSelectedRuleName);
        if (mri != null)
        {
            // Show rule description
            ltlDescription.Text = mri.MacroRuleDescription;
            txtErrorMsg.Value = string.Empty;
            txtErrorMsg.WatermarkText = string.IsNullOrEmpty(DefaultErrorMessage) ? ResHelper.GetString("basicform.invalidinput") : DefaultErrorMessage;

            // Prepare form for rule parameters
            FormInfo fi = new FormInfo(mri.MacroRuleParameters);
            formProperties.FormInformation = fi;
            formProperties.DataRow = fi.GetDataRow();

            fi.LoadDefaultValues(formProperties.DataRow, FormResolveTypeEnum.AllFields);
            if ((mMacroRule != null) && (mMacroRuleTree != null) && mMacroRuleTree.Children[0].RuleName.EqualsCSafe(mSelectedRuleName, true))
            {
                // Set params from rule given by Value property
                foreach (DictionaryEntry entry in mMacroRuleTree.Children[0].Parameters)
                {
                    string paramName = ValidationHelper.GetString(entry.Key, string.Empty);
                    MacroRuleParameter param = (MacroRuleParameter)entry.Value;

                    if (!string.IsNullOrEmpty(paramName) && (param != null) && formProperties.DataRow.Table.Columns.Contains(paramName))
                    {
                        formProperties.DataRow[paramName] = param.Value;
                    }
                }

                txtErrorMsg.Value = mMacroRule.ErrorMessage;
            }

            if (forceReload)
            {
                // Reload params
                formProperties.ReloadData();
            }

            mParamsInitialized = true;
        }
    }


    /// <summary>
    /// Creates new <see cref="FieldMacroRule"/> object based on inputs.
    /// </summary>
    private FieldMacroRule CreateMacroRule()
    {
        if (!IsValid())
        {
            return null;
        }

        MacroRuleTree main = null;
        FieldMacroRule fmr = null;

        MacroRuleInfo mri = MacroRuleInfoProvider.GetMacroRuleInfo(mSelectedRuleName);
        if (mri != null)
        {
            main = new MacroRuleTree();

            MacroRuleTree childern = new MacroRuleTree()
            {
                RuleText = mri.MacroRuleText,
                RuleName = mri.MacroRuleName,
                RuleCondition = mri.MacroRuleCondition,
                Parent = main
            };
            main.Children.Add(childern);

            foreach (string paramName in formProperties.Fields)
            {
                // Load value from the form control
                FormEngineUserControl ctrl = formProperties.FieldControls[paramName];
                if (ctrl != null)
                {
                    // Convert value to EN culture
                    var dataType = ctrl.FieldInfo.DataType;

                    var convertedValue = DataTypeManager.ConvertToSystemType(TypeEnum.Field, dataType, ctrl.Value);

                    string value = ValidationHelper.GetString(convertedValue, "", CultureHelper.EnglishCulture);
                    string displayName = ctrl.ValueDisplayName;

                    if (String.IsNullOrEmpty(displayName))
                    {
                        displayName = value;
                    }

                    MacroRuleParameter param = new MacroRuleParameter
                    {
                        Value = value,
                        Text = displayName,
                        ValueType = dataType
                    };

                    childern.Parameters.Add(paramName, param);
                }
            }

            fmr = new FieldMacroRule();
            fmr.ErrorMessage = txtErrorMsg.Text;
            fmr.MacroRule = string.Format("Rule(\"{0}\", \"{1}\")", MacroElement.EscapeSpecialChars(main.GetCondition()), MacroElement.EscapeSpecialChars(main.GetXML()));

            if (!MacroSecurityProcessor.IsSimpleMacro(fmr.MacroRule))
            {
                // Sign complex macros
                fmr.MacroRule = MacroSecurityProcessor.AddMacroSecurityParams(fmr.MacroRule, MembershipContext.AuthenticatedUser.UserName);
            }
            fmr.MacroRule = string.Format("{{%{0}%}}", fmr.MacroRule);
        }

        return fmr;
    }


    /// <summary>
    /// Reloads the control.
    /// </summary>
    private void Reload()
    {
        uniSelector.Reload(true);
        InitializeControl(true);
    }

    #endregion
}