using System;
using System.Collections;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Helpers;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.SiteProvider;

[ValidationPropertyAttribute("Value")]
public partial class CMSFormControls_System_LocalizableTextBox : LocalizableFormEngineUserControl, ICallbackEventHandler
{
    #region "Constants"

    /// <summary>
    /// Localization macro starts with '{$' characters.
    /// </summary>
    public const string MACRO_START = "{$";

    /// <summary>
    /// Localization macro ends with '$}' characters.
    /// </summary>
    public const string MACRO_END = "$}";

    /// <summary>
    /// In-place localization macro starts with '{$=' characters and should not be localized in localizable textbox!
    /// </summary>
    protected const string INPLACE_MACRO_START = "{$=";

    /// <summary>
    /// URL of field localization modal dialog.
    /// </summary>
    public const string LOCALIZE_FIELD = "~/CMSFormControls/Selectors/LocalizableTextBox/LocalizeField.aspx";

    /// <summary>
    /// URL of string localization modal dialog.
    /// </summary>
    public const string LOCALIZE_STRING = "~/CMSFormControls/Selectors/LocalizableTextBox/LocalizeString.aspx";

    /// <summary>
    /// Default prefix for keys created in development mode.
    /// </summary>
    private const string DEV_PREFIX = "test.";

    /// <summary>
    /// Maximum resource string key length.
    /// </summary>
    private const int MAX_KEY_LENGTH = 200;


    private const string REMOVE_ARGUMENT = "remove|";
    private const string ADD_ARGUMENT = "add|";
    private const string TEXT_BOX_LOCALIZED_CSS = "input-localized";

    #endregion


    #region "Private variables"

    // Indicates if changes to resource string should be performed immediately after each PostBack.
    private bool mAutoSave = true;

    private string mResourceKeyPrefix;
    private bool mSaveAutomatically;
    private readonly bool mUserHasPermissionForLocalizations = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Localization", "LocalizeStrings");

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets value indicating if localization of key exists.
    /// </summary>
    private bool LocalizationExists
    {
        get
        {
            return ResHelper.TranslationFoundForLocalizedString(OriginalValue);
        }
    }


    /// <summary>
    /// Indicates if text contained in textbox is resolved resource string or if it is just plain text.
    /// </summary>
    private bool IsLocalizationMacro
    {
        get
        {
            return MacroProcessor.IsLocalizationMacro(OriginalValue);
        }
    }


    /// <summary>
    /// Contains unresolved resource string.
    /// </summary>
    private string OriginalValue
    {
        get
        {
            if (!string.IsNullOrEmpty(hdnValue.Value))
            {
                return hdnValue.Value;
            }
            return ValidationHelper.GetString(ViewState["OriginalValue"], string.Empty);
        }
        set
        {
            hdnValue.Value = value;
            ViewState["OriginalValue"] = value;
        }
    }


    /// <summary>
    /// Modal dialog identifier.
    /// </summary>
    private Guid Identifier
    {
        get
        {
            // Try to load data from control ViewState
            Guid identifier = ValidationHelper.GetGuid(Request.Params[hdnIdentifier.UniqueID], Guid.Empty);

            // Create new GUID for identifier
            if (identifier == Guid.Empty)
            {
                identifier = Guid.NewGuid();
            }

            // Assign identifier to hidden field
            hdnIdentifier.Value = identifier.ToString();

            return identifier;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets whether the control is read only
    /// </summary>
    public bool ReadOnly
    {
        get
        {
            return TextBox.ReadOnly;
        }
        set
        {
            TextBox.ReadOnly = value;
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
            if (TextBox != null)
            {
                TextBox.Enabled = value;
                btnLocalize.Enabled = value;
                btnOtherLanguages.Enabled = value;
                btnRemoveLocalization.Enabled = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            // Return macro contained in hidden field if text is macro
            if (IsLocalizationMacro && !IsInplaceMacro(OriginalValue) && LocalizationExists)
            {
                return OriginalValue;
            }
            // Return plain text contained in textbox
            else
            {
                return TextBox.Text;
            }
        }
        set
        {
            if (mSaveAutomatically)
            {
                // Save if configured to save automatically
                Save();

                mSaveAutomatically = false;
            }

            string valueStr = OriginalValue = ValidationHelper.GetString(value, null);

            if (!IsInplaceMacro(valueStr) && LocalizationExists)
            {
                using (LocalizationActionContext context = new LocalizationActionContext())
                {
                    context.ResolveSubstitutionMacros = false;
                    TextBox.Text = ResHelper.LocalizeString(valueStr);
                }
            }
            else
            {
                TextBox.Text = valueStr;
            }
        }
    }


    /// <summary>
    /// Returns client ID of the textbox.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return TextBox.ClientID;
        }
    }


    /// <summary>
    /// Publicly visible textbox which contains translated string or plain text.
    /// </summary>
    public CMSTextBox TextBox
    {
        get
        {
            cntrlContainer.LoadContainer();
            return textbox;
        }
    }


    /// <summary>
    /// TextMode of textbox.
    /// </summary>
    public TextBoxMode TextMode
    {
        get
        {
            return TextBox.TextMode;
        }
        set
        {
            TextBox.TextMode = value;
        }
    }


    /// <summary>
    /// Number of columns of textbox in multiline mode.
    /// </summary>
    public int Columns
    {
        get
        {
            return TextBox.Columns;
        }
        set
        {
            TextBox.Columns = value;
        }
    }


    /// <summary>
    /// Number of rows of textbox in multiline mode.
    /// </summary>
    public int Rows
    {
        get
        {
            return TextBox.Rows;
        }
        set
        {
            TextBox.Rows = value;
        }
    }


    /// <summary>
    /// Gets or sets value indicating if control should save changes to resource string immediately after each PostBack. Default true.
    /// </summary>
    public bool AutoSave
    {
        get
        {
            return mAutoSave;
        }
        set
        {
            mAutoSave = value;
        }
    }


    /// <summary>
    /// Maximum length of plain text or resource string key. Validates in IsValid() method.
    /// </summary>
    public int MaxLength
    {
        get
        {
            return TextBox.MaxLength;
        }
        set
        {
            TextBox.MaxLength = value;
        }
    }


    /// <summary>
    /// Resource key prefix.
    /// </summary>
    public string ResourceKeyPrefix
    {
        get
        {
            // If user set prefix
            if (!String.IsNullOrEmpty(mResourceKeyPrefix))
            {
                return mResourceKeyPrefix;
            }
            // If in DevelopmentMode
            else if (SystemContext.DevelopmentMode)
            {
                return DEV_PREFIX;
            }
            // Otherwise return "custom."
            else
            {
                return "custom.";
            }
        }
        set
        {
            mResourceKeyPrefix = value;
        }
    }


    /// <summary>
    /// If TRUE then resource string key selection is skipped. Instead resource string key is automaticaly created from entered text.
    /// Also when removing localization it also deletes resource string key assigned.
    /// </summary>
    public bool AutomaticMode
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if control is used on live site. Default value is FALSE for localizable text box.
    /// </summary>
    public override bool IsLiveSite
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if textbox value is used in content.
    /// Default value is FALSE for localizable text box. 
    /// </summary>
    public bool ValueIsContent
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ValueIsContent"), false);
        }
        set
        {
            SetValue("ValueIsContent", value);
        }
    }


    /// <summary>
    /// Text which will be used as watermark.
    /// </summary>
    public string WatermarkText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WatermarkText"), string.Empty);
        }
        set
        {
            SetValue("WatermarkText", value);
            TextBox.WatermarkText = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        btnOtherLanguages.ToolTip = GetString("localizable.otherlanguages");
        btnLocalize.ToolTip = GetString("localizable.localize");
        btnRemoveLocalization.ToolTip = GetString("localizable.remove");

        btnOtherLanguages.OnClientClick = "LocalizeString('" + hdnValue.ClientID + "', '" + TextBox.ClientID + "'); return false;";
        btnRemoveLocalization.OnClientClick = String.Format("RemoveLocalization('{0}', '{1}', '{2}', '{3}', '{4}', '{5}'); return false;",
            hdnValue.ClientID, TextBox.ClientID, btnLocalize.ClientID, btnOtherLanguages.ClientID, btnRemoveLocalization.ClientID, cntrlContainer.ClientID);

        // In automatic mode resource string key is generated from plain text
        if (AutomaticMode)
        {
            btnLocalize.Click += btnLocalize_Click;
        }
        // Otherwise user has option to select resource string key
        else
        {
            btnLocalize.OnClientClick = "LocalizationDialog" + ClientID + "('" + ADD_ARGUMENT + "' + Get('" + TextBox.ClientID + "').value); return false;";
        }

        // Apply CSS style
        if (!String.IsNullOrEmpty(CssClass))
        {
            TextBox.CssClass = CssClass;
            CssClass = null;
        }

        // Register event handler for saving data in BasicForm
        if (Form != null)
        {
            Form.OnAfterSave += Form_OnAfterSave;
        }
        // Save changes after each PostBack if set so
        else if (RequestHelper.IsPostBack() && Visible && AutoSave && IsLocalizationMacro && !String.IsNullOrEmpty(TextBox.Text.Trim()))
        {
            mSaveAutomatically = true;
        }

        SetClientSideMaxLength();
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (mSaveAutomatically)
        {
            Save();
        }

        // Ensure the text in textbox
        if (IsLocalizationMacro && !IsInplaceMacro(TextBox.Text) && RequestHelper.IsPostBack())
        {
            using (LocalizationActionContext context = new LocalizationActionContext())
            {
                context.ResolveSubstitutionMacros = false;
                TextBox.Text = ResHelper.LocalizeString(OriginalValue);
            }
        }

        // Set watermark text
        TextBox.WatermarkText = WatermarkText;

        SetVisibility();

        Reload();

        // Register the scripts
        if (cntrlContainer.DisplayActions)
        {
            RegisterScripts();
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "Custom methods"

    private bool IsInplaceMacro(string text)
    {
        if (text == null)
        {
            return false;
        }

        return text.Trim().StartsWithCSafe(INPLACE_MACRO_START);
    }


    private int GetNumberOfCurrentSiteCultures()
    {
        return CultureSiteInfoProvider.GetSiteCultures(SiteContext.CurrentSiteName).Tables[0].Rows.Count;
    }


    /// <summary>
    /// Sets client-side max length of the textbox control.
    /// </summary>
    private void SetClientSideMaxLength()
    {
        if (FieldInfo != null)
        {
            var maxLength = FieldInfo.GetMaxInputLength(ContextResolver);
            if (maxLength > 0)
            {
                TextBox.MaxLength = maxLength;
            }
        }
    }


    /// <summary>
    /// Sets the dialog parameters to the context.
    /// </summary>
    protected void SetFieldDialogParameters(string textboxValue)
    {
        Hashtable parameters = new Hashtable();
        parameters["TextBoxValue"] = textboxValue;
        parameters["HiddenValue"] = hdnValue.ClientID;
        parameters["TextBoxID"] = TextBox.ClientID;
        parameters["ButtonLocalizeField"] = btnLocalize.ClientID;
        parameters["ButtonLocalizeString"] = btnOtherLanguages.ClientID;
        parameters["ButtonRemoveLocalization"] = btnRemoveLocalization.ClientID;
        parameters["ResourceKeyPrefix"] = ResourceKeyPrefix;
        parameters["LocalizedContainer"] = cntrlContainer.ClientID;

        WindowHelper.Add(Identifier.ToString(), parameters);
    }


    /// <summary>
    /// Sets visibility according to user permissions.
    /// </summary>
    private void SetVisibility()
    {
        bool hasMoreCultures = ValueIsContent ? CultureInfoProvider.NumberOfUICultures > 1 : GetNumberOfCurrentSiteCultures() > 1;

        btnLocalize.Visible = btnOtherLanguages.Visible = mUserHasPermissionForLocalizations && (SystemContext.DevelopmentMode || hasMoreCultures);

        cntrlContainer.DisplayActions =
            (SystemContext.DevelopmentMode || hasMoreCultures || LocalizationExists)
            && !IsLiveSite
            && !IsInplaceMacro(TextBox.Text)
            && (LocalizationExists || mUserHasPermissionForLocalizations);

        // User without permissions can't edit localization, only remove localized value
        ReadOnly |= !mUserHasPermissionForLocalizations && LocalizationExists;
    }


    /// <summary>
    /// Validates control.
    /// </summary>
    public override bool IsValid()
    {
        // Check for maximum length
        if (TextBox.MaxLength > 0)
        {
            return (OriginalValue.Length <= MaxLength) && (TextBox.Text.Length <= MaxLength);
        }
        else
        {
            return true;
        }
    }


    /// <summary>
    /// Reloads control.
    /// </summary>
    public void Reload()
    {
        // Textbox contains translated macro
        if (IsLocalizationMacro && LocalizationExists)
        {
            btnLocalize.Style.Add(HtmlTextWriterStyle.Display, "none");
            btnOtherLanguages.Style.Add(HtmlTextWriterStyle.Display, "inline");
            btnRemoveLocalization.Style.Add(HtmlTextWriterStyle.Display, "inline");

            // Add localized CSS class
            cntrlContainer.AddCssClass(TEXT_BOX_LOCALIZED_CSS);

            TextBox.ToolTip = 
                mUserHasPermissionForLocalizations ?
                String.Format(GetString("localizable.localized"), GetResouceKeyFromString(OriginalValue)) : 
                GetString("localizable.localizedwithoutpermissions");
        }
        // Textbox contains only plain text
        else
        {
            btnLocalize.Style.Add(HtmlTextWriterStyle.Display, "inline");
            btnOtherLanguages.Style.Add(HtmlTextWriterStyle.Display, "none");
            btnRemoveLocalization.Style.Add(HtmlTextWriterStyle.Display, "none");

            cntrlContainer.RemoveCssClass(TEXT_BOX_LOCALIZED_CSS);
            TextBox.ToolTip = null;
        }
    }


    /// <summary>
    /// Registers JS.
    /// </summary>
    private void RegisterScripts()
    {
        // Register function to set translation string key from dialog window
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterJQuery(Page);
        StringBuilder script = new StringBuilder();
        script.Append(
            @"
function Get(id) {
    return document.getElementById(id);
}

function SetResourceAndOpen(hdnValId, resKey, textBoxId, textBoxValue, btnLocalizeFieldId, btnLocalizeStringId, btnRemoveLocalizationId, localizedContainerId) {
    SetResource(hdnValId, resKey, textBoxId, textBoxValue, btnLocalizeFieldId, btnLocalizeStringId, btnRemoveLocalizationId, localizedContainerId);
    LocalizeString(hdnValId, textBoxId);
    return false;
}

function SetResource(hdnValId, resKey, textBoxId, textBoxValue, btnLocalizeFieldId, btnLocalizeStringId, btnRemoveLocalizationId, localizedContainerId) {
    Get(hdnValId).value = '", MACRO_START, @"' + resKey + '", MACRO_END, @"';

    var $textBox = $cmsj('#' + textBoxId);
    $textBox.val(textBoxValue);
    $textBox.trigger('change');

    $cmsj('#' + localizedContainerId).addClass('", TEXT_BOX_LOCALIZED_CSS, @"').trigger('checkScrollbar');
    
    $cmsj('#' + btnLocalizeFieldId).hide();
    $cmsj('#' + btnLocalizeStringId).css('display', 'inline');
    $cmsj('#' + btnRemoveLocalizationId).css('display', 'inline');
    return false;
}

function SetTranslation(textBoxId, textBoxValue, hdnValId, resKey) {
    var $textBox = $cmsj('#' + textBoxId);
    $textBox.val(textBoxValue);
    $textBox.trigger('change');

    $cmsj('#' + hdnValId).val('", MACRO_START, @"' + resKey + '", MACRO_END, @"');
}

function LocalizeFieldReady(rvalue, context) {
    modalDialog(context, 'localizableField', 720, 250, null, null, true);
    return false;
}

function LocalizeString(hdnValId, textBoxId) {
    var stringKey = Get(hdnValId).value;
    stringKey = stringKey.substring(", MACRO_START.Length, @", stringKey.length - ", MACRO_END.Length, @");
    modalDialog('", ResolveUrl(LOCALIZE_STRING), @"?hiddenValueControl=' + hdnValId + '&stringKey=' + escape(stringKey) + '&parentTextbox=' + textBoxId, 'localizableString', 900, 635, null, null, true);
    return false;
}

function RemoveLocalization(hdnValId, textBoxId, btnLocalizeFieldId, btnLocalizeStringId, btnRemoveLocalizationId, localizedContainerId) {
    if (confirm(" + ScriptHelper.GetLocalizedString("localizable.removelocalization") + @")) {
        var $textBox = $cmsj('#' + textBoxId);
        $textBox.prop('readonly', false);
        var hdnVal = Get(hdnValId);
        " + Page.ClientScript.GetCallbackEventReference(this, "'" + REMOVE_ARGUMENT + "' + hdnVal.value", "function(){}", null) + @";
        hdnVal.value = $textBox.val();
        $textBox.trigger('change');

        $cmsj('#' + localizedContainerId).removeClass('", TEXT_BOX_LOCALIZED_CSS, @"');
        $cmsj('#' + btnLocalizeStringId).hide();
        $cmsj('#' + btnRemoveLocalizationId).hide();
        $cmsj('#' + btnLocalizeFieldId).css('display', 'inline');
    }
    return false;
}");

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "LocalizationDialogFunction", ScriptHelper.GetScript(script.ToString()));

        // Register callback to send current plain text to modal window for localization
        string url = LOCALIZE_FIELD + "?params=" + Identifier;
        url += "&hash=" + QueryHelper.GetHash(url, false);

        script = new StringBuilder();
        script.Append(@"
function LocalizationDialog", ClientID, @"(value) {
    ", Page.ClientScript.GetCallbackEventReference(this, "value", "LocalizeFieldReady", "'" + ResolveUrl(url) + "'"), @"
}");

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "LocalizationDialog" + ClientID, ScriptHelper.GetScript(script.ToString()));
    }


    /// <summary>
    /// Saves translation for given resource string.
    /// </summary>
    /// <returns>Returns TRUE if resource string was successfully modified</returns>
    public override bool Save()
    {
        // Save changes only when macro is edited
        if (IsLocalizationMacro && mUserHasPermissionForLocalizations)
        {
            string resKey = GetResouceKeyFromString(OriginalValue);

            if (!IsInplaceMacro(TextBox.Text))
            {
                resKey = resKey.Trim();

                // Update / insert key
                var ri = ResourceStringInfoProvider.GetResourceStringInfo(resKey) ?? new ResourceStringInfo
                {
                    StringKey = resKey,
                    StringIsCustom = !SystemContext.DevelopmentMode
                };

                ri.TranslationText = TextBox.Text.Trim();
                if (CultureInfoProvider.GetCultureID(CultureHelper.PreferredUICultureCode) != 0)
                {
                    ri.CultureCode = CultureHelper.PreferredUICultureCode;
                }
                else
                {
                    ri.CultureCode = CultureHelper.DefaultUICultureCode;
                }
                ResourceStringInfoProvider.SetResourceStringInfo(ri);
                return true;
            }
            else
            {
                // Remove localization if in-place macro was inserted
                RemoveLocalization(resKey);
            }
        }
        return false;
    }


    /// <summary>
    /// Gets the resource string name from the text
    /// </summary>
    /// <param name="text">Text</param>
    private static string GetResouceKeyFromString(string text)
    {
        return text.Substring(MACRO_START.Length, text.Length - (MACRO_END.Length + MACRO_START.Length));
    }


    /// <summary>
    /// Removes localization from database if AutomaticMode is on.
    /// </summary>
    private void RemoveLocalization(string resKey)
    {
        // In automatic mode remove resource string key 
        if (AutomaticMode)
        {
            ResourceStringInfoProvider.DeleteResourceStringInfo(resKey);
        }
    }


    public override bool SetValue(string propertyName, object value)
    {
        switch (propertyName.ToLowerCSafe())
        {
            case "autosave":
                AutoSave = ValidationHelper.GetBoolean(value, AutoSave);
                return true;

            case "textmode":
                string textMode = ValidationHelper.GetString(value, TextBoxMode.SingleLine.ToString());
                TextBoxMode textBoxMode;
                Enum.TryParse(textMode, true, out textBoxMode);
                TextMode = textBoxMode;
                return true;
        }

        return base.SetValue(propertyName, value);
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Button localize click. In AutomaticMode available only.
    /// </summary>
    void btnLocalize_Click(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(TextBox.Text.Trim()))
        {
            // Initialize resource string
            OriginalValue = LocalizationHelper.GetUniqueResStringKey(TextBox.Text.Trim(), ResourceKeyPrefix, true, MAX_KEY_LENGTH);

            string cultureCode = CultureHelper.PreferredUICultureCode;
            if (String.IsNullOrEmpty(cultureCode))
            {
                cultureCode = CultureHelper.DefaultUICultureCode;
            }

            // Save ResourceString
            ResourceStringInfo ri = new ResourceStringInfo();
            ri.StringKey = OriginalValue;
            ri.CultureCode = cultureCode;
            ri.TranslationText = TextBox.Text.Trim();
            ri.StringIsCustom = !SystemContext.DevelopmentMode;
            ResourceStringInfoProvider.SetResourceStringInfo(ri);

            // Open 'localization to other languages' window
            string script = ScriptHelper.GetScript("modalDialog('" + ResolveUrl(LOCALIZE_STRING) + "?hiddenValueControl=" + hdnValue.ClientID + "&stringKey=" + ri.StringKey + "&parentTextbox=" + textbox.ClientID + "', 'localizableString', 600, 635, null, null, true);");
            ScriptHelper.RegisterStartupScript(this, typeof(string), "OpenLocalization", script);

            // Set macro settings
            Value = MACRO_START + OriginalValue + MACRO_END;
            Reload();
        }
        else
        {
            lblError.Visible = true;
            lblError.ResourceString = "localize.entertext";
        }
    }


    /// <summary>
    /// BasicForm saved event handler.
    /// </summary>
    private void Form_OnAfterSave(object sender, EventArgs e)
    {
        Save();
    }


    /// <summary>
    /// Gets callback result.
    /// </summary>
    string ICallbackEventHandler.GetCallbackResult()
    {
        return String.Empty;
    }


    /// <summary>
    /// Raise callback event.
    /// </summary>
    void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
    {
        if (eventArgument != null)
        {
            if (eventArgument.StartsWithCSafe(ADD_ARGUMENT))
            {
                SetFieldDialogParameters(eventArgument.Substring(ADD_ARGUMENT.Length));
            }
            else if (eventArgument.StartsWithCSafe(REMOVE_ARGUMENT))
            {
                string resKey = GetResouceKeyFromString(eventArgument.Substring(REMOVE_ARGUMENT.Length));
                RemoveLocalization(resKey);
            }
        }
    }

    #endregion
}