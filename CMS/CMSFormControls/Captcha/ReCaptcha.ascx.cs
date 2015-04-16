using System;
using System.Collections.Generic;
using System.Linq;

using CMS.DataEngine;
using CMS.FormControls;
using CMS.Helpers;
using CMS.Localization;
using CMS.PortalEngine;
using CMS.Base;

public partial class CMSFormControls_Captcha_ReCaptcha : FormEngineUserControl
{
    #region "Variables"

    /// <summary>
    /// Custom translations
    /// </summary>
    private readonly Dictionary<string, string> mCustomTranslations = new Dictionary<string, string> 
    {
        {"instructions_visual" , ResHelper.GetString("recaptcha.visualinstructions")},
        {"instructions_audio" , ResHelper.GetString("recaptcha.auidoinstructions")},
        {"play_again" , ResHelper.GetString("recaptcha.playagin")},
        {"cant_hear_this" , ResHelper.GetString("recaptcha.canthearthis")},
        {"visual_challenge" , ResHelper.GetString("recaptcha.visualchallenge")},
        {"audio_challenge" , ResHelper.GetString("recaptcha.audiochallenge")},
        {"refresh_btn" , ResHelper.GetString("recaptcha.refreshbutton")},
        {"help_btn" , ResHelper.GetString("recaptcha.helpbutton")},
        {"incorrect_try_again" , ResHelper.GetString("recaptcha.incorrecttryagain")}
    };

    /// <summary>
    /// Languages for which the translation is available
    /// </summary>
    private readonly string[] mSupportedLanguages = { "en", "nl", "fr", "de", "pt", "ru", "es", "tr" };

    /// <summary>
    ///  reCAPTCHA primary API key
    /// </summary>
    private string mPublicKey = null;

    /// reCAPTCHA secondary API key
    private string mPrivateKey = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// reCAPTCHA public API key
    /// </summary>
    public string PublicKey
    {
        get
        {
            if (string.IsNullOrEmpty(mPublicKey))
            {
                mPublicKey = SettingsKeyInfoProvider.GetValue("CMSReCaptchaPublicKey");
            }

            return mPublicKey;
        }
        set
        {
            mPublicKey = value;
        }
    }


    /// <summary>
    /// reCAPTCHA private API key
    /// </summary>
    public string PrivateKey
    {
        get
        {
            if (string.IsNullOrEmpty(mPrivateKey))
            {
                mPrivateKey = SettingsKeyInfoProvider.GetValue("CMSReCaptchaPrivateKey");
            }

            return mPrivateKey;
        }
        set
        {
            mPrivateKey = value;
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

            // Disable CAPTCHA control
            captcha.Enabled = value;
            captcha.Visible = value;

            if (FieldInfo != null)
            {
                FieldInfo.Visible = value;
            }
        }
    }


    /// <summary>
    /// Get or sets control value
    /// </summary>
    public override object Value
    {
        get
        {
            return null;
        }
        set
        {
        }
    }


    /// <summary>
    /// Indicates if validation of form control was successful
    /// </summary>
    public override bool IsValid()
    {
        captcha.Validate();
        return captcha.IsValid && base.IsValid();
    }


    /// <summary>
    /// Error message displayed when validation fails
    /// </summary>
    public override string ErrorMessage
    {
        get
        {
            return captcha.ErrorMessage;
        }
        set
        {
        }
    }


    /// <summary>
    /// Gets or sets reCaptcha theme.
    /// </summary>
    public string Theme
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Theme"), "clean");
        }
        set
        {
            SetValue("Theme", value);
        }
    }

    #endregion


    #region "Control methods"

    /// <summary>
    /// Page load event
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        CheckFieldEmptiness = false;

        string culture = GetCurrentCulture();

        captcha.PublicKey = PublicKey;
        captcha.PrivateKey = PrivateKey;
        captcha.AllowMultipleInstances = false;
        captcha.Language = culture;
        captcha.Theme = Theme;

        if (!mSupportedLanguages.Contains(culture))
        {
            captcha.CustomTranslations = mCustomTranslations;
        }
    }


    /// <summary>
    /// Page pre render event
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (Visible && Enabled && !StopProcessing)
        {
            RegisterClientScripts();
        }
    }


    /// <summary>
    /// Gets current short culture code.
    /// </summary>
    private string GetCurrentCulture()
    {
        // Get default UI culture
        string culture = CultureHelper.GetPreferredUICultureCode();

        // Check if other culture should be used
        switch (PortalContext.ViewMode)
        {
            case ViewModeEnum.LiveSite:
            case ViewModeEnum.EditLive:
                culture = LocalizationContext.PreferredCultureCode;
                break;
        }

        return CultureHelper.GetShortCultureCode(culture).ToLowerCSafe();
    }


    /// <summary>
    /// Register jQuery scripts
    /// </summary>
    private void RegisterClientScripts()
    {
        // Process scripts
        ScriptHelper.RegisterJQuery(Page);

        string script = @"
$cmsj(document).ready(function () {
    // Register new reCAPTCHA reload event to support multiple reCAPTCHAs (using CAPTCHA clones) on one page
    if((Recaptcha != null) && (Recaptcha.old_finish_reload == null)) {       
        Recaptcha.old_finish_reload = Recaptcha.finish_reload;
        Recaptcha.finish_reload = function(a, b, c, d) {
            Recaptcha.old_finish_reload(a, b, c, d);
            $cmsj('div[id=\""recaptcha_widget_div\""]').each(function(index) {
                if(index > 0) {
                    $cmsj(this).parent().html($cmsj('#recaptcha_widget_div').clone(true, true));
                }
            });
        }
    }

    // Attach to asynchronous postback event to force reloading reCAPTCHA control, which normally reloads only on full postback
    if (typeof Sys != 'undefined') {
        var requestManager = Sys.WebForms.PageRequestManager.getInstance();
        requestManager.add_endRequest(function(sender, args) {
            if(Recaptcha != null) {
                Recaptcha._init_options(RecaptchaOptions);    
                
                // Check if widget placeholder is rendered, if not attach widget placeholder to rendered HTML 
                if ((Recaptcha.widget == null) || !document.getElementById('recaptcha_widget_div')) {
                    $cmsj('#cbCaptcha').show().html('<div id=""recaptcha_widget_div"" style=""display:none""></div>');
                    Recaptcha.widget = Recaptcha.$('recaptcha_widget_div');
                }
                
                // Reload reCAPTCHA using specified options and widget placeholder and get new CAPTCHA challenge
                Recaptcha.reload();
                Recaptcha.challenge_callback();
            }
        });
    }
});";
        // Register global script required to synchronize multiple ReCaptchas
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ReCaptchaScripts", script, true);

        // Copy reCAPTCHA HTML code from first instance
        if (captcha.IsClonnedInstance)
        {
            script = "$cmsj(document).ready(function() { $cmsj('#" + pnlCaptchaWrap.ClientID + "').html($cmsj('#recaptcha_widget_div').clone(true, true));})";
            ScriptHelper.RegisterStartupScript(this, typeof(string), "ReCaptchaScripts" + ClientID, script, true);
        }
    }

    #endregion
}