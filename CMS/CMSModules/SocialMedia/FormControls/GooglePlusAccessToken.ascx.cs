using System;
using System.Linq;
using System.Text;

using CMS.FormControls;
using CMS.Base;
using CMS.Helpers;


public partial class CMSModules_SocialMedia_FormControls_GooglePlusAccessToken : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets enabled state.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return btnSelect.Enabled;
        }
        set
        {
            txtToken.Enabled = value;
            btnSelect.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets the value of access token.
    /// </summary>
    public override object Value
    {
        get
        {
            if (!string.IsNullOrEmpty(txtTokenSecret.Text))
            {
                return txtToken.Text + ";" + txtTokenSecret.Text;
            }
            return string.Empty;
        }
        set
        {
            string stringValue = value.ToString();
            if (!string.IsNullOrEmpty(stringValue))
            {
                txtToken.Text = stringValue.Substring(0, stringValue.IndexOfCSafe(";"));
            }
        }
    }


    /// <summary>
    /// Gets or sets the name of the client function to be used to get the client ID value.
    /// </summary>
    public string ClientIDValueAccessFunctionName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ClientIDValueAccessFunctionName"), null);
        }
        set
        {
            SetValue("ClientIDValueAccessFunctionName", value);
        }
    }


    /// <summary>
    /// Gets or sets the name of the client function to be used to get the client secret value.
    /// </summary>
    public string ClientSecretValueAccessFunctionName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ClientSecretValueAccessFunctionName"), null);
        }
        set
        {
            SetValue("ClientSecretValueAccessFunctionName", value);
        }
    }

    #endregion


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        lblError.Text = GetString("socialnetworking.googleplus.apisettingsmissing");

        // Register the dialog script
        ScriptHelper.RegisterDialogScript(Page);

        // Build script with modal dialog opener and set textbox value functions
        const string scriptStringFormat = @"
function tagSelect(id, idsecret) {{
    var txtToken = $cmsj('#' + id);
    var txtTokenSecret = $cmsj('#' + idsecret);
    var clientid = {2}();
    var clientsecret = {3}();
    if (txtToken != null){{
        if ((clientid.length == 0) || (clientsecret.length == 0))
        {{
            var label = $cmsj('#{1}');
            label.removeClass('hidden');

            return;
        }}

        modalDialog('{0}?txtToken=' + id + '&txtTokenSecret=' + idsecret + '&token_client_id=' + clientid + '&token_client_secret=' + clientsecret, 'GooglePlusAccessToken', 550, 400, null, null, null, true);
    }}
}}
function setAccessTokenToTextBox(textBoxId, accessTokenString, txtTokenSecret, accessTokenSecretString){{
    if (textBoxId != '') {{
        var textbox = document.getElementById(textBoxId);
        if (textbox != null){{
            textbox.value = accessTokenString;
        }}
        var textbox = document.getElementById(txtTokenSecret);
        if (textbox != null){{
            textbox.value = accessTokenSecretString;
        }}
    }}
}}";

        string scriptString = string.Format(scriptStringFormat,
            URLHelper.GetAbsoluteUrl("~/CMSModules/SocialMedia/Pages/GooglePlusAccessTokenPage.aspx"),
            lblError.ClientID,
            ClientIDValueAccessFunctionName,
            ClientSecretValueAccessFunctionName);

        // Register tag script
        ScriptHelper.RegisterStartupScript(this, typeof(string), "accessTokenScript", scriptString, true);

        btnSelect.OnClientClick = "tagSelect('" + txtToken.ClientID + "', '" + txtTokenSecret.ClientID + "'); return false;";
    }
}