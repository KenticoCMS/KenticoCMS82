using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

using CMS.FormEngine;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSFormControls_Selectors_AlternativeFormSelection : DesignerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Validate query params against hash value and check if control ID is not potential XSS threat
        string txtClientId = QueryHelper.GetString("txtelem", "");
        string lblClientId = QueryHelper.GetString("lblelem", "");
        Regex re = RegexHelper.GetRegex(@"[\w\d_$$]*");

        if (!QueryHelper.ValidateHash("hash") || !re.IsMatch(txtClientId) || !re.IsMatch(lblClientId))
        {
            pnlContent.Visible = false;
            return;
        }

        if (!RequestHelper.IsPostBack())
        {
            DataSet dsClasses = AlternativeFormInfoProvider.GetClassesWithAlternativeForms();

            drpClass.DataSource = dsClasses;
            drpClass.DataValueField = "FormClassID";
            drpClass.DataTextField = "ClassName";
            drpClass.DataBind();

            if (!DataHelper.DataSourceIsEmpty(dsClasses))
            {
                // Try to preselect class from drop-down list
                string className = QueryHelper.GetString("classname", string.Empty);
                if (className != string.Empty)
                {
                    drpClass.SelectedIndex = drpClass.Items.IndexOf(drpClass.Items.FindByText(className));
                }
                else
                {
                    drpClass.SelectedIndex = 0;
                }
                // Load alternative forms for selected class
                LoadAltFormsList();
            }
        }

        SetSaveJavascript("SelectCurrentAlternativeForm('" + txtClientId + "','" + lblClientId + "'); return false;");

        PageTitle.TitleText = GetString("altforms.selectaltform");
        SetSaveResourceString("general.select");

        ltlScript.Text = ScriptHelper.GetScript("var lstAlternativeForms = document.getElementById('" + lstAlternativeForms.ClientID + "');");
    }


    /// <summary>
    /// Fills alternative form list according to selection in class CMSDropDownList.
    /// </summary>
    protected void LoadAltFormsList()
    {
        int formClassId = ValidationHelper.GetInteger(drpClass.SelectedValue, 0);
        DataSet ds = AlternativeFormInfoProvider.GetAlternativeForms("FormClassID=" + formClassId, "FormName");

        lstAlternativeForms.Items.Clear();

        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                AlternativeFormInfo afi = new AlternativeFormInfo(dr);
                if (afi != null)
                {
                    if ((afi.FormDisplayName != String.Empty) && (afi.FormName != String.Empty))
                    {
                        lstAlternativeForms.Items.Add(new ListItem(ResHelper.LocalizeString(afi.FormDisplayName), afi.FullName));
                    }
                }
            }
            lstAlternativeForms.SelectedValue = null;
            lstAlternativeForms.DataBind();
        }

        ds.Dispose();
    }


    protected void drpClass_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Load alternative forms for selected class
        LoadAltFormsList();
    }
}