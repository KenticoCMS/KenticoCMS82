using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.FormControls;
using CMS.Helpers;

public partial class CMSFormControls_Inputs_TextBox : FormEngineUserControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets encoded textbox value.
    /// </summary>
    public override object Value
    {
        get
        {
            return Trim ? txtValue.Text.Trim() : txtValue.Text;
        }
        set
        {
            txtValue.Text = ValidationHelper.GetString(value, String.Empty);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set trimming ability from form controls parameters
        Trim = ValidationHelper.GetBoolean(GetValue("trim"), false);

        CheckMinMaxLength = true;
        CheckRegularExpression = true;
    }

    #endregion
}