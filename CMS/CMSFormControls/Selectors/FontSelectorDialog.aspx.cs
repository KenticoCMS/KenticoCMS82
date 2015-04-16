using System;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;


public partial class CMSFormControls_Selectors_FontSelectorDialog : CMSModalPage
{
    private string mHiddenFieldID;
    private string mFontTypeID;


    #region Methods

    protected void Page_Load(object sender, EventArgs e)
    {
        Save += btnOk_Click;

        mHiddenFieldID = Request.QueryString["HiddenID"];
        mFontTypeID = Request.QueryString["FontTypeID"];
        //Header settings
        ICMSMasterPage currentMaster = (ICMSMasterPage)Master;
        currentMaster.Title.TitleText = GetString("fontselector.title");
        
        RegisterScriptCode();

        if (!RequestHelper.IsPostBack())
        {
            FillFontStyleListBox();

            string fontType = (string)WindowHelper.GetItem(mHiddenFieldID);
            if (fontType != null)
            {
                string[] fontParams = fontType.Split(new char[] { ';' });
                if (fontParams.Length == 5)
                {
                    txtFontType.Text = fontParams[0];
                    //transfer from style to text and select proper index                    
                    lstFontStyle.SelectedValue = fontParams[1].ToLowerCSafe();
                    txtFontSize.Text = fontParams[2];

                    //setup sampletext
                    lblSampleText.Font.Name = txtFontType.Text;
                    //underline and strikethrough
                    if (fontParams[3].ToLowerCSafe() == "underline")
                    {
                        chkUnderline.Checked = true;
                        lblSampleText.Font.Underline = true;
                    }
                    if (fontParams[4].ToLowerCSafe() == "strikethrought")
                    {
                        chkStrike.Checked = true;
                        lblSampleText.Font.Strikeout = true;
                    }
                }
            }

            lblSampleText.Font.Name = txtFontType.Text;
            lblSampleText.Font.Size = FontUnit.Point(ValidationHelper.GetInteger(txtFontSize.Text, 11));

            //setup font style
            if (txtFontStyle.Text.ToLowerCSafe().Contains("bold"))
            {
                lblSampleText.Font.Bold = true;
            }
            if (txtFontStyle.Text.ToLowerCSafe().Contains("italic"))
            {
                lblSampleText.Font.Italic = true;
            }

            ListItem li = lstFontStyle.SelectedItem;
            if (li != null)
            {
                txtFontStyle.Text = li.Text;
            }

            FillFontSizeListBox();
            FillFontTypeListBox();
        }
        else
        {
            txtFontSize.Text = lstFontSize.SelectedValue;
            txtFontType.Text = lstFontType.SelectedValue;

            ListItem li = lstFontStyle.SelectedItem;
            if (li != null)
            {
                txtFontStyle.Text = li.Text;
            }
            //set up sample text
            lblSampleText.Font.Name = txtFontType.Text;
            lblSampleText.Font.Size = FontUnit.Point(ValidationHelper.GetInteger(txtFontSize.Text, 11));
            if (chkStrike.Checked)
            {
                lblSampleText.Font.Strikeout = true;
            }
            if (chkUnderline.Checked)
            {
                lblSampleText.Font.Underline = true;
            }
        }


        SetSaveJavascript("return btOk_clicked();");
        
        //setup jscripts
        lstFontSize.Attributes.Add("onChange", "fontSizeChange (this.options[this.selectedIndex].value)");
        lstFontType.Attributes.Add("onChange", "fontTypeChange(this.options[this.selectedIndex].value)");
        lstFontStyle.Attributes.Add("onChange", "fontStyleChange(this.selectedIndex,this.options[this.selectedIndex].text)");
        txtFontSize.Attributes.Add("onChange", "sizeManualUpdate ()");

        chkUnderline.Attributes.Add("onclick", "fontDecorationChange()");
        chkStrike.Attributes.Add("onclick", "fontDecorationChange()");

        chkStrike.Text = GetString("fontselector.strikethrought");
        chkUnderline.Text = GetString("fontselector.underline");

        Page.DataBind();
    }


    private void FillFontSizeListBox()
    {
        for (int i = 8; i < 80; i++)
        {
            ListItem li = new ListItem(i.ToString(), i.ToString());
            if (txtFontSize.Text == i.ToString())
            {
                li.Selected = true;
            }
            lstFontSize.Items.Add(li);
        }
    }


    private void FillFontTypeListBox()
    {
        foreach (FontFamily fontName in FontFamily.Families)
        {
            ListItem li = new ListItem(fontName.Name);
            if (txtFontType.Text.ToLowerCSafe() == fontName.Name.ToLowerCSafe())
            {
                li.Selected = true;
            }
            lstFontType.Items.Add(li);
        }
    }


    /// <summary>
    /// Fill font style list.
    /// </summary>
    private void FillFontStyleListBox()
    {
        lstFontStyle.Items.Add(new ListItem(GetString("fontselector.regular"), "regular"));
        lstFontStyle.Items.Add(new ListItem(GetString("fontselector.bold"), "bold"));
        lstFontStyle.Items.Add(new ListItem(GetString("fontselector.italic"), "italic"));
        lstFontStyle.Items.Add(new ListItem(GetString("fontselector.bolditalic"), "bolditalic"));
    }


    private void RegisterScriptCode()
    {
        ScriptHelper.RegisterWOpenerScript(Page);
        string script = ScriptHelper.GetScript(@"
            function ChangeSampleText(val) {
            var sampleText = document.getElementById('" + lblSampleText.ClientID + @"');
            sampleText.style.fontSize = val;
        }
        function fontSizeChange(val) {                  
            document.getElementById('" + txtFontSize.ClientID + @"').value = val;
            document.getElementById('" + lblSampleText.ClientID + @"').style.fontSize = val + 'pt';
        }
        function fontTypeChange(val) {
            document.getElementById('" + txtFontType.ClientID + @"').value = val;
            document.getElementById('" + lblSampleText.ClientID + @"').style.fontFamily = val;
        }
        function fontStyleChange(index,val) {                   
            document.getElementById('" + txtFontStyle.ClientID + @"').value = val;
            var sample = document.getElementById('" + lblSampleText.ClientID + @"');

            if (index == 1) {
                sample.style.fontWeight = 'bold';
                sample.style.fontStyle = 'normal';
            }   
            else if (index == 2) {
                sample.style.fontStyle = 'italic';
                sample.style.fontWeight = 'normal';
            }
            else if (index ==0) {
                sample.style.fontStyle = 'normal';
                sample.style.fontWeight = 'normal';
            }
            else {
                sample.style.fontStyle = 'Italic';
                sample.style.fontWeight = 'Bold';
            }
        }
    
        function fontDecorationChange() {
            var checkedUnderline = document.getElementById('" + chkUnderline.ClientID + @"').checked;
            var checkedStrike = document.getElementById('" + chkStrike.ClientID + @"').checked;
            var sample = document.getElementById('" + lblSampleText.ClientID + @"');
            if (checkedUnderline && checkedStrike) {
                sample.style.textDecoration = 'underline line-through';
            }
            else if (checkedUnderline) {
                sample.style.textDecoration = 'underline';
            }
            else if (checkedStrike) {
                sample.style.textDecoration = 'line-through';
            }
            else {
                sample.style.textDecoration = 'none';
            }
        }
        function btOk_clicked() {
            var size = document.getElementById('" + txtFontSize.ClientID + @"').value;
            var type = document.getElementById('" + txtFontType.ClientID + @"').value;
            var style = document.getElementById('" + txtFontStyle.ClientID + @"').value;

            if (size == '' || type  == '' || style == '' ) {
                alert(" + ScriptHelper.GetLocalizedString("fontselector.notallvaluesfilled") + @");   
                return false;                
            }
            return true;                
        }

        function sizeManualUpdate(tbInput) {
            var val = document.getElementById('" + txtFontSize.ClientID + @"').value;
            if (val != parseInt(val) || val == 0)   {
                alert (" + ScriptHelper.GetLocalizedString("fontselector.wrongsize") + @");                
                var doc = document.getElementById('" + txtFontSize.ClientID + @"') ;  
                var lb = document.getElementById('" + lstFontSize.ClientID + @"');
                doc.value = lb.options[lb.selectedIndex].value;              
            }

            var list = document.getElementById('" + lstFontSize.ClientID + @"');
            for (var i = 0; i < list.length; ++i) {
                if (list[i].value == val)
                    list[i].selected = true;
            }
            var sampleText = document.getElementById('" + lblSampleText.ClientID + @"');
            sampleText.style.fontSize = val + 'px';
        }                                             
        ");

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(Page), "FontSelectorDialogScript", script);
    }

    #endregion


    #region Event Handlers

    protected void btnOk_Click(object sender, EventArgs e)
    {
        if (txtFontSize.Text != String.Empty && txtFontStyle.Text != String.Empty && txtFontType.Text != String.Empty)
        {
            string ret = String.Format("{0};{1};{2};", txtFontType.Text, lstFontStyle.SelectedValue, txtFontSize.Text);
            if (chkUnderline.Checked)
            {
                ret += "underline;";
            }
            else
            {
                ret += ";";
            }
            if (chkStrike.Checked)
            {
                ret += "strikethrought";
            }

            FontStyle fs = new FontStyle();
            switch (lstFontStyle.SelectedValue.ToLowerCSafe())
            {
                case "bold":
                    fs = FontStyle.Bold;
                    break;

                case "italic":
                    fs = FontStyle.Italic;
                    break;

                case "bolditalic":
                    fs = FontStyle.Bold | FontStyle.Italic;
                    break;

                case "regular":
                    fs = FontStyle.Regular;
                    break;
            }

            if (chkUnderline.Checked)
            {
                fs |= FontStyle.Underline;
            }

            if (chkStrike.Checked)
            {
                fs |= FontStyle.Strikeout;
            }

            if (!FontExists(fs))
            {
                lblError.Visible = true;
                lblError.Text = GetString("fontselector.unsupportedfont");
                return;
            }

            string submitScript = ScriptHelper.GetScript(String.Format("wopener.getParameters('{0}','{1}','{2}'); CloseDialog();", ret, ScriptHelper.GetString(mHiddenFieldID), ScriptHelper.GetString(mFontTypeID)));
            ScriptHelper.RegisterClientScriptBlock(Page, typeof (Page), "SubmitScript", submitScript);
        }
    }


    private bool FontExists(FontStyle fontStyle)
    {
        try
        {
            new Font(txtFontType.Text, ValidationHelper.GetInteger(txtFontSize.Text, 10), fontStyle);
            return true;
        }
        catch
        {
            return false;
        }
    }

    #endregion
}