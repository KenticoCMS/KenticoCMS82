using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.PortalControls;

using AjaxControlToolkit;

public partial class CMSFormControls_Basic_NumericUpDown : FormEngineUserControl
{
    #region "Variables"

    private Dictionary<string, string> mValues;
    private string mInnerValue;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return textbox.Enabled;
        }
        set
        {
            textbox.Enabled = value;
            btnDown.Enabled = value;
            btnUp.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            if ((mValues != null) && (mValues.Count > 0))
            {
                string key = textbox.Text;
                if (mValues.ContainsKey(key))
                {
                    return mValues[key];
                }
                return mValues.First().Value;
            }

            return textbox.Text;
        }
        set
        {
            if ((value != null) || ((FieldInfo != null) && FieldInfo.AllowEmpty))
            {
                if (FieldInfo != null)
                {
                    // Convert the value to a proper type
                    value = ConvertInputValue(value);
                }

                mInnerValue = ValidationHelper.GetString(value, String.Empty);

                LoadValues();

                if ((mValues != null) && (mValues.Count > 0))
                {
                    foreach (string key in mValues.Keys)
                    {
                        if (mValues[key] == mInnerValue)
                        {
                            textbox.Text = key;
                            break;
                        }
                    }
                }
                else
                {
                    textbox.Text = mInnerValue;
                }
            }
        }
    }

    #endregion


    #region "Custom properties"

    /// <summary>
    /// Step used for simple numeric incrementing and decrementing. The default value is 1.
    /// </summary>
    public int Step
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Step"), 1);
        }
        set
        {
            SetValue("Step", value);
        }
    }


    /// <summary>
    /// Minimal value of the selector.
    /// </summary>
    public double Minimum
    {
        get
        {
            return ValidationHelper.GetDouble(GetValue("Minimum"), 0);
        }
        set
        {
            SetValue("Minimum", value);
        }
    }


    /// <summary>
    /// Maximal value of the selector.
    /// </summary>
    public double Maximum
    {
        get
        {
            return ValidationHelper.GetDouble(GetValue("Maximum"), 0);
        }
        set
        {
            SetValue("Maximum", value);
        }
    }


    /// <summary>
    /// Combined size of the TextBox and Up/Down buttons (min value 25). This property is not used if you provide custom buttons.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), 112);
        }
        set
        {
            SetValue("Width", value);
        }
    }


    /// <summary>
    /// URL of the image to display as the Up button.
    /// </summary>
    public string UpButtonImageUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("UpButtonImageUrl"), null);
        }
        set
        {
            SetValue("UpButtonImageUrl", value);
        }
    }


    /// <summary>
    /// URL of the image to display as the Down button.
    /// </summary>
    public string DownButtonImageUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DownButtonImageUrl"), null);
        }
        set
        {
            SetValue("DownButtonImageUrl", value);
        }
    }


    /// <summary>
    /// Web service method that returns the data used to get the previous value, or the name of a method declared on the Page which is decorated with the WebMethodAttribute.
    /// </summary>
    public string ServiceDownMethod
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ServiceDownMethod"), null);
        }
        set
        {
            SetValue("ServiceDownMethod", value);
        }
    }


    /// <summary>
    /// Path to a web service that returns the data used to get the previous value. This property should be left null if ServiceUpMethod or ServiceDownMethod refers to a page method. The web service should be decorated with the System.Web.Script.Services.ScriptService attribute.
    /// </summary>
    public string ServiceDownPath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ServiceDownPath"), null);
        }
        set
        {
            SetValue("ServiceDownPath", value);
        }
    }


    /// <summary>
    /// Web service method that returns the data used to get the next value, or the name of a method declared on the Page which is decorated with the WebMethodAttribute.
    /// </summary>
    public string ServiceUpMethod
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ServiceUpMethod"), null);
        }
        set
        {
            SetValue("ServiceUpMethod", value);
        }
    }


    /// <summary>
    /// Path to a web service that returns the data used to get the next value. This property should be left null if ServiceUpMethod or ServiceDownMethod refers to a page method. The web service should be decorated with the System.Web.Script.Services.ScriptService attribute.
    /// </summary>
    public string ServiceUpPath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ServiceUpPath"), null);
        }
        set
        {
            SetValue("ServiceUpPath", value);
        }
    }


    /// <summary>
    /// Specifies a custom parameter to pass to the Web Service.
    /// </summary>
    public string Tag
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Tag"), null);
        }
        set
        {
            SetValue("Tag", value);
        }
    }


    /// <summary>
    /// The alt text to show when the mouse is over the  Up button.
    /// </summary>
    public string UpButtonImageAlternateText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("UpButtonImageAlternateText"), ResHelper.GetString("general.up"));
        }
        set
        {
            SetValue("UpButtonImageAlternateText", value);
        }
    }


    /// <summary>
    /// The alt text to show when the mouse is over the  Down button.
    /// </summary>
    public string DownButtonImageAlternateText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DownButtonImageAlternateText"), ResHelper.GetString("general.down"));
        }
        set
        {
            SetValue("DownButtonImageAlternateText", value);
        }
    }

    #endregion


    #region "Methods"

    private void LoadValues()
    {
        if (mValues == null)
        {
            string options = GetResolvedValue<string>("options", null);
            string query = ValidationHelper.GetString(GetValue("query"), null);
            ListItemCollection items = new ListItemCollection();
            mValues = new Dictionary<string, string>();

            try
            {
                FormHelper.LoadItemsIntoList(options, query, items, FieldInfo, ContextResolver);

                foreach (ListItem item in items)
                {
                    mValues.Add(item.Text, item.Value);
                }
            }
            catch (Exception ex)
            {
                FormControlError ctrlError = new FormControlError();
                ctrlError.FormControlName = "NumericUpDown";
                ctrlError.InnerException = ex;
                Controls.Add(ctrlError);
                pnlContainer.Visible = false;
            }
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Initialize properties
        PortalHelper.EnsureScriptManager(Page);
        btnDown.ScreenReaderDescription = GetString("spinner.decrement");
        btnUp.ScreenReaderDescription = GetString("spinner.increment");
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Create extender
        NumericUpDownExtender exNumeric = new NumericUpDownExtender();
        exNumeric.ID = "exNum";
        exNumeric.TargetControlID = textbox.ID;
        exNumeric.EnableViewState = false;
        Controls.Add(exNumeric);

        // Initialize extender
        exNumeric.Minimum = Minimum;
        exNumeric.Maximum = Maximum;
        exNumeric.Step = Step;
        exNumeric.Width = Width;

        // Disable checking changes before complete initialization
        textbox.Attributes.Add("data-ignorechanges", "true");

        textbox.Width = Width;

        exNumeric.TargetButtonUpID = btnUp.ID;
        exNumeric.TargetButtonDownID = btnDown.ID;

        LoadValues();
        if ((mValues != null) && (mValues.Count > 0))
        {
            exNumeric.RefValues = String.Join(";", mValues.Keys);
        }

        // Initialize up button
        if (!string.IsNullOrEmpty(UpButtonImageUrl))
        {
            btnImgUp.Visible = true;
            btnUp.Visible = false;
            btnImgUp.ImageUrl = UpButtonImageUrl;
            btnImgUp.AlternateText = ContextResolver.ResolveMacros(UpButtonImageAlternateText);
            btnImgUp.ImageAlign = ImageAlign.Middle;
            exNumeric.TargetButtonUpID = btnImgUp.ID;
        }

        // Initialize down button
        if (!string.IsNullOrEmpty(DownButtonImageUrl))
        {
            btnImgDown.Visible = true;
            btnDown.Visible = false;
            btnImgDown.ImageUrl = DownButtonImageUrl;
            btnImgDown.AlternateText = ContextResolver.ResolveMacros(DownButtonImageAlternateText);
            btnImgDown.ImageAlign = ImageAlign.Middle;
            exNumeric.TargetButtonDownID = btnImgDown.ID;
        }

        exNumeric.ServiceDownMethod = ServiceDownMethod;
        exNumeric.ServiceDownPath = ServiceDownPath;
        exNumeric.ServiceUpMethod = ServiceUpMethod;
        exNumeric.ServiceUpPath = ServiceUpPath;
        exNumeric.Tag = ContextResolver.ResolveMacros(Tag);

        // Apply CSS styles
        if (!String.IsNullOrEmpty(CssClass))
        {
            pnlContainer.CssClass = CssClass;
            CssClass = null;
        }
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            pnlContainer.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }

        CheckRegularExpression = true;
        CheckFieldEmptiness = true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ScriptHelper.RegisterJQuery(Page);

        if (Enabled)
        {
            FixMinimumScript();
            EnableCheckingChanges();
        }
        else
        {
            DisableUpDown();
        }
    }


    /// <summary>
    /// Makes NumericUpDown extender read-only by removing javascript event handlers from buttons.
    /// </summary>
    private void DisableUpDown()
    {
        // Remove all javascript handlers from up and down buttons to make control read-only
        StringBuilder script = new StringBuilder();

        // Bind function to AJAX life cycle event
        // NumericUpDown extender elements are not created earlier
        script.Append(@"
Sys.Application.add_load(function (){
    $clearHandlers($cmsj('#", textbox.ClientID, @"_bUp')[0]);
    $clearHandlers($cmsj('#", textbox.ClientID, @"_bDown')[0]);
});");

        ScriptHelper.RegisterStartupScript(this, typeof(string), ClientID + "_disableUpDown", ScriptHelper.GetScript(script.ToString()));
    }


    /// <summary>
    /// Replaces flawed implementation of NumericUpDown's function _compitePrecision with a correct one.
    /// </summary>
    /// <remarks>
    /// Pull request with the same change has been issued to the AjaxControlToolkit repository.
    /// If it's accepted, future update of the toolkit will make this piece of code redundant.
    /// </remarks>
    private void FixMinimumScript()
    {
        // <summary>
        // Compute the precision of the value by counting the number
        // of digits in the fractional part of its string representation
        // </summary>
        // <param name=""value"" type=""Number"">Value</param>
        // <returns type=""Number"" integer=""true"">
        // Fractional precision of the number
        // </returns>
        const string scriptFix = @"Sys.Extended.UI.NumericUpDownBehavior.prototype._computePrecision = function(value) {
        if (value == Number.Nan) {
            return 0;
        }
        // Call toString which does not localize, according to ECMA 262
        var str = value.toString();
        if (str) {
            var fractionalPart = /\.(\d*)$/;
            var matches = str.match(fractionalPart);
            if (matches && matches.length == 2 && matches[1]) {
                return matches[1].length;
            }
        }
        return 0;
    };";

        // Fix should be applied only once no matter how many numeric up/down controls are on the page.
        ScriptHelper.RegisterStartupScript(this, typeof(string), "NumericUpDownMinimumFix", ScriptHelper.GetScript(scriptFix));
    }


    /// <summary>
    /// Overrides initialize function of NumericUpDown extender to enable checking changes.
    /// After control initialization the data-ignorechanges attribute is set to false.
    /// </summary>
    private void EnableCheckingChanges()
    {
        const string script = @"
Sys.Extended.UI.NumericUpDownBehavior.prototype.initializeOrig = Sys.Extended.UI.NumericUpDownBehavior.prototype.initialize;
Sys.Extended.UI.NumericUpDownBehavior.prototype.initialize = function(){
    this.initializeOrig();
    $cmsj(this._elementTextBox).data('ignorechanges', false);
};";
        ScriptHelper.RegisterStartupScript(this, typeof(string), "NumericUpDownInit", ScriptHelper.GetScript(script));
    }

    #endregion
}