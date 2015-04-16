using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.PortalControls;
using CMS.Helpers;
using CMS.PortalEngine;

public partial class CMSWebParts_General_CookieLaw : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Default user cookie level
    /// </summary>
    public string DefaultLevel
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("DefaultLevel"), "");
        }
        set
        {
            this.SetValue("DefaultLevel", value);
        }
    }


    /// <summary>
    /// Compare current cookie level to
    /// </summary>
    public string MatchLevel
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("MatchLevel"), "Essential");
        }
        set
        {
            this.SetValue("MatchLevel", value);
        }
    }


    /// <summary>
    /// Level simulated in preview mode
    /// </summary>
    public string PreviewLevel
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("PreviewLevel"), "Essential");
        }
        set
        {
            this.SetValue("PreviewLevel", value);
        }
    }


    /// <summary>
    /// If level is below, display
    /// </summary>
    public bool BelowLevelVisible
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("BelowLevelVisible"), true);
        }
        set
        {
            this.SetValue("BelowLevelVisible", value);
        }
    }


    /// <summary>
    /// Text
    /// </summary>
    public string BelowLevelText
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("BelowLevelText"), "");
        }
        set
        {
            this.SetValue("BelowLevelText", value);
        }
    }


    /// <summary>
    /// Show deny button
    /// </summary>
    public bool BelowShowDeny
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("BelowShowDeny"), false);
        }
        set
        {
            this.SetValue("BelowShowDeny", value);
        }
    }


    /// <summary>
    /// Show allow specific button
    /// </summary>
    public bool BelowShowSpecific
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("BelowShowSpecific"), true);
        }
        set
        {
            this.SetValue("BelowShowSpecific", value);
        }
    }


    /// <summary>
    /// Show allow all button
    /// </summary>
    public bool BelowShowAll
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("BelowShowAll"), true);
        }
        set
        {
            this.SetValue("BelowShowAll", value);
        }
    }


    /// <summary>
    /// If level matches, display
    /// </summary>
    public bool ExactLevelVisible
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("ExactLevelVisible"), true);
        }
        set
        {
            this.SetValue("ExactLevelVisible", value);
        }
    }


    /// <summary>
    /// Text
    /// </summary>
    public string ExactLevelText
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("ExactLevelText"), "");
        }
        set
        {
            this.SetValue("ExactLevelText", value);
        }
    }


    /// <summary>
    /// Show deny button
    /// </summary>
    public bool ExactShowDeny
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("ExactShowDeny"), true);
        }
        set
        {
            this.SetValue("ExactShowDeny", value);
        }
    }


    /// <summary>
    /// Show allow specific button
    /// </summary>
    public bool ExactShowSpecific
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("ExactShowSpecific"), false);
        }
        set
        {
            this.SetValue("ExactShowSpecific", value);
        }
    }


    /// <summary>
    /// Show allow all button
    /// </summary>
    public bool ExactShowAll
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("ExactShowAll"), true);
        }
        set
        {
            this.SetValue("ExactShowAll", value);
        }
    }


    /// <summary>
    /// If level is above, display
    /// </summary>
    public bool AboveLevelVisible
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("AboveLevelVisible"), true);
        }
        set
        {
            this.SetValue("AboveLevelVisible", value);
        }
    }


    /// <summary>
    /// Text
    /// </summary>
    public string AboveLevelText
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("AboveLevelText"), "");
        }
        set
        {
            this.SetValue("AboveLevelText", value);
        }
    }


    /// <summary>
    /// Show deny button
    /// </summary>
    public bool AboveShowDeny
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("AboveShowDeny"), true);
        }
        set
        {
            this.SetValue("AboveShowDeny", value);
        }
    }


    /// <summary>
    /// Show allow specific button
    /// </summary>
    public bool AboveShowSpecific
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("AboveShowSpecific"), true);
        }
        set
        {
            this.SetValue("AboveShowSpecific", value);
        }
    }


    /// <summary>
    /// Show allow all button
    /// </summary>
    public bool AboveShowAll
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("AboveShowAll"), false);
        }
        set
        {
            this.SetValue("AboveShowAll", value);
        }
    }


    /// <summary>
    /// Deny all button text
    /// </summary>
    public string DenyAllText
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("DenyAllText"), "Deny all cookies");
        }
        set
        {
            this.SetValue("DenyAllText", value);
        }
    }


    /// <summary>
    /// Allow specific button text
    /// </summary>
    public string AllowSpecificText
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("AllowSpecificText"), "Allow only essential cookies");
        }
        set
        {
            this.SetValue("AllowSpecificText", value);
        }
    }


    /// <summary>
    /// Allow specific button sets level
    /// </summary>
    public string AllowSpecificSetLevel
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("AllowSpecificSetLevel"), "Essential");
        }
        set
        {
            this.SetValue("AllowSpecificSetLevel", value);
        }
    }


    /// <summary>
    /// Allow all button text
    /// </summary>
    public string AllowAllText
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("AllowAllText"), "Allow all cookies");
        }
        set
        {
            this.SetValue("AllowAllText", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties
    /// </summary>
    protected void SetupControl()
    {
        if (this.StopProcessing)
        {
            // Do not process
        }
        else
        {
            btnAllowAll.Text = AllowAllText;
            btnDenyAll.Text = DenyAllText;
            btnAllowSpecific.Text = AllowSpecificText;
        }
    }


    /// <summary>
    /// Reloads the control data
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (this.StopProcessing)
        {
            // Do not process
        }
        else
        {
            int level = CookieLevel.Unknown;

            bool liveSite = PortalContext.ViewMode.IsLiveSite();
            if (!liveSite)
            {
                // Use simulated cookie level
                level = CookieHelper.GetCookieLevel(PreviewLevel, CookieLevel.Unknown);
            }

            if (level == CookieLevel.Unknown)
            {
                // Ensure default level
                int defaultLevel = CookieHelper.GetCookieLevel(DefaultLevel, CookieLevel.Unknown);
                if (!liveSite)
                {
                    defaultLevel = CookieLevel.Unknown;
                }

                level = CookieHelper.GetCurrentCookieLevel(defaultLevel);
            }

            int matchLevel = CookieHelper.GetCookieLevel(MatchLevel, level);

            if (level < matchLevel)
            {
                // Set the components to the Above level
                SetComponents(BelowLevelVisible, BelowLevelText, BelowShowAll, BelowShowSpecific, BelowShowDeny);
            }
            else if (level == matchLevel)
            {
                // Set the components to the Above level
                SetComponents(ExactLevelVisible, ExactLevelText, ExactShowAll, ExactShowSpecific, ExactShowDeny);
            }
            else
            {
                // Set the components to the Above level
                SetComponents(AboveLevelVisible, AboveLevelText, AboveShowAll, AboveShowSpecific, AboveShowDeny);
            } 
        }
    }


    /// <summary>
    /// Initializes the components based on the given properties
    /// </summary>
    /// <param name="visible">Flag whether this mode should be visible</param>
    /// <param name="text">Information text</param>
    /// <param name="allowAll">Show allow all button</param>
    /// <param name="allowSpecific">Show allow Specific button</param>
    /// <param name="allowDeny">Show allow deny button</param>
    private void SetComponents(bool visible, string text, bool allowAll, bool allowSpecific, bool allowDeny)
    {
        if (!visible)
        {
            this.Visible = false;
            return;
        }

        lblText.Text = text;

        btnAllowAll.Visible = !String.IsNullOrEmpty(btnAllowAll.Text) && allowAll;
        btnAllowSpecific.Visible = !String.IsNullOrEmpty(btnAllowSpecific.Text) && allowSpecific;
        btnDenyAll.Visible = !String.IsNullOrEmpty(btnDenyAll.Text) && allowDeny;

        // Hide the web part in case no content or button is displayed
        if (String.IsNullOrEmpty(text) && !btnAllowAll.Visible && !btnAllowSpecific.Visible && !btnDenyAll.Visible)
        {
            this.Visible = false;
        }
    }


    /// <summary>
    /// Deny all click
    /// </summary>
    protected void btnDenyAll_Click(object sender, EventArgs e)
    {
        ChangeLevel(CookieLevel.System);
    }


    /// <summary>
    /// Allow Specific click
    /// </summary>
    protected void btnAllowSpecific_Click(object sender, EventArgs e)
    {
        int specificLevel = CookieHelper.GetCookieLevel(AllowSpecificSetLevel, CookieLevel.Essential);

        ChangeLevel(specificLevel);
    }
    

    /// <summary>
    /// Allow all click
    /// </summary>
    protected void btnAllowAll_Click(object sender, EventArgs e)
    {
        ChangeLevel(CookieLevel.All);
    }


    /// <summary>
    /// Changes the cookie level
    /// </summary>
    /// <param name="newLevel">New cookie level to set</param>
    private static void ChangeLevel(int newLevel)
    {
        if (PortalContext.ViewMode.IsLiveSite())
        {
            CookieHelper.ChangeCookieLevel(newLevel);
        }
    }
    
    #endregion
}



