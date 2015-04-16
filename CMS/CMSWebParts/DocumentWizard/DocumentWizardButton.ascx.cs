using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.PortalControls;
using CMS.Helpers;
using CMS.Base;
using CMS.DocumentEngine;

public partial class CMSWebParts_DocumentWizard_DocumentWizardButton : CMSAbstractWizardWebPart
{
    #region "Properties"

    /// <summary>
    /// Action type.
    /// </summary>
    public string ActionType
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("ActionType"), "");
        }
        set
        {
            this.SetValue("ActionType", value);
        }
    }


    /// <summary>
    /// Text.
    /// </summary>
    public string Text
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Text"), "");
        }
        set
        {
            this.SetValue("Text", value);
        }
    }


    /// <summary>
    /// Button type.
    /// </summary>
    public string ButtonType
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("ButtonType"), "");
        }
        set
        {
            this.SetValue("ButtonType", value);
        }
    }


    /// <summary>
    /// Inactive button.
    /// </summary>
    public string InactiveButton
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("InactiveButton"), "disabled");
        }
        set
        {
            this.SetValue("InactiveButton", value);
        }
    }


    /// <summary>
    /// CSS Class.
    /// </summary>
    public string ButtonCssClass
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("ButtonCssClass"), "");
        }
        set
        {
            this.SetValue("ButtonCssClass", value);
        }
    }

    #endregion


    #region "Methods"


    /// <summary>
    /// Init handler
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Disable web part, web part is allowed in "StepLoaded" method
        StopProcessing = true;
        Visible = false;
    }


    /// <summary>
    /// PreRender handler
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        if (!StopProcessing)
        {
            // Set CSS Class
            btn.CssClass = ButtonCssClass;

            // PREVIOUS
            if (ActionType == "previous")
            {
                // Hide button if it's required by step event args
                if (WizardManager.StepEventArgs.HideBackButton)
                {
                    Visible = false;
                }
                // Set button text if it's set in step event args
                else if (!String.IsNullOrEmpty(WizardManager.StepEventArgs.BackButtonText))
                {
                    btn.LinkText = WizardManager.StepEventArgs.BackButtonText;
                }

                // Check disable/visible state
                if (WizardManager.CurrentStep == WizardManager.FirstStep)
                {
                    if (InactiveButton == "disabled")
                    {
                        btn.Enabled = false;
                    }
                    else
                    {
                        Visible = false;
                    }
                }
            }
            // NEXT
            else if (ActionType == "next")
            {
                // Hide button if it's required by step event args
                if (WizardManager.StepEventArgs.HideNextButton)
                {
                    Visible = false;
                }
                // Set button text if it's set in step event args
                else if (!String.IsNullOrEmpty(WizardManager.StepEventArgs.NextButtonText))
                {
                    btn.LinkText = WizardManager.StepEventArgs.NextButtonText;
                }

                // Hide next button for not existing final step URL
                if (WizardManager.LastStep == WizardManager.CurrentStep)
                {
                    // Use step custom URL if set
                    string url = WizardManager.StepEventArgs["NextStepUrl"] as string;

                    // Use default step URL
                    if (String.IsNullOrEmpty(url))
                    {
                        url = string.IsNullOrEmpty(WizardManager.StepEventArgs.FinalStepUrl) ? WizardManager.FinalStepNextUrl : WizardManager.StepEventArgs.FinalStepUrl;
                    }

                    // Check disable/visible state
                    if (String.IsNullOrEmpty(url))
                    {
                        if (InactiveButton == "disabled")
                        {
                            btn.Enabled = false;
                        }
                        else
                        {
                            Visible = false;
                        }
                    }

                }
            }
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Step loaded
    /// </summary>
    protected override void StepLoaded(object sender, StepEventArgs e)
    {
        base.StepLoaded(sender, e);

        if (WizardManager.Steps.Count != 0)
        {
            // Allow button
            Visible = true;
            StopProcessing = false;

            // Set current control
            if (ButtonType == "button")
            {
                btn.ShowAsButton = true;
            }
            else
            {
                btn.ShowAsButton = false;
            }

            // Button settings
            btn.LinkText = Text;
            btn.Click += btn_Click;

            btn.Visible = true;
        }
    }


    /// <summary>
    /// Button click
    /// </summary>
    void btn_Click(object sender, EventArgs e)
    {
        // NEXT
        if (ActionType == "next")
        {
            ComponentEvents.RequestEvents.RaiseComponentEvent(this, WizardManager.StepEventArgs, "PageWizardManager", ComponentEvents.NEXT);
        }
        // PREVIOUS
        else
        {
            ComponentEvents.RequestEvents.RaiseComponentEvent(this, WizardManager.StepEventArgs, "PageWizardManager", ComponentEvents.PREVIOUS);
        }
    }

    #endregion
}
