using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.FormControls;
using CMS.Helpers;

public partial class CMSFormControls_Selectors_FontSelector : FormEngineUserControl
{
    #region Variables

    private string mDefaultFont = "Arial";
    private string mDefaultStyle = "Regular";
    private int mDefaultSize = 11;
    private bool mDisplayClearButton = true;

    #endregion


    #region "Public properties"

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
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return txtFontType.Text;
        }
        set
        {
            txtFontType.Text = (string)value;
        }
    }


    /// <summary>
    /// Default font family.
    /// </summary>
    public string DefaultFont
    {
        get
        {
            return mDefaultFont;
        }
        set
        {
            mDefaultFont = value;
        }
    }


    /// <summary>
    /// Default font style.
    /// </summary>
    public string DefaultStyle
    {
        get
        {
            return mDefaultStyle;
        }
        set
        {
            mDefaultStyle = value;
        }
    }


    /// <summary>
    /// Default font size.
    /// </summary>
    public int DefaultSize
    {
        get
        {
            return mDefaultSize;
        }
        set
        {
            mDefaultSize = value;
        }
    }


    /// <summary>
    /// If true display button for clear font.
    /// </summary>
    public bool DisplayClearButton
    {
        get
        {
            return mDisplayClearButton;
        }
        set
        {
            mDisplayClearButton = value;
        }
    }

    /// <summary>
    /// ClientId of font type text box.
    /// </summary>
    public string FontTypeTextBoxClientId
    {
        get
        {
            return txtFontType.ClientID;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        return true;
    }


    public void SetOnChangeAttribute(string fnc)
    {
        txtFontType.Attributes["onchange"] = fnc;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        btnClearFont.Visible = DisplayClearButton;

        registerScripts();

        // Setup 'Select' button
        btnChangeFontType.Text = ResHelper.GetString("fontselector.select");
        btnChangeFontType.Attributes.Add("OnClick", "SelectFont('" + hfValue.ClientID + "','" + txtFontType.ClientID + "');return false;");

        btnClearFont.Text = ResHelper.GetString("fontselector.clear");

        if (hfValue.Value != String.Empty)
        {
            txtFontType.Text = hfValue.Value;
        }

        // Add font parameters to selector dialog
        if (txtFontType.Text == String.Empty)
        {
            WindowHelper.Add(hfValue.ClientID, String.Format("{0};{1};{2};;", DefaultFont, DefaultStyle, DefaultSize));
        }
        else
        {
            WindowHelper.Add(hfValue.ClientID, txtFontType.Text);
        }
    }


    private void registerScripts()
    {
        // Register dialog script
        ScriptHelper.RegisterDialogScript(Page);

        string postBack = Page.ClientScript.GetPostBackEventReference(hfValue, "");

        // Register script for refresh data
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(Page), "RegisterPostabackScript", ScriptHelper.GetScript(
@"
function refreshData() {
    " + postBack + @";
}"
        ));

        // Create script for open dialog and get parameters
        string script = ScriptHelper.GetScript(
@" 
function SelectFont(hdID,ftID) {
        modalDialog('" + ResolveUrl("~/CMSFormControls/Selectors/FontSelectorDialog.aspx") + @"?HiddenID=' + hdID + '&FontTypeID=' + ftID, 'FontSelector', 500, 470);
}
function getParameters(val,hf,tb) {                  
    document.getElementById (hf).value= val;              
    document.getElementById (tb).value= val;
    refreshData(); 
}
"
        );

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(Page), "script", script);
    }

    #endregion


    #region Event handlers

    protected void btnClearFont_Click(object sender, EventArgs e)
    {
        // Clear value in selector
        txtFontType.Text = String.Empty;
        hfValue.Value = String.Empty;
        WindowHelper.Add("FontSelector", String.Empty);
    }

    #endregion
}