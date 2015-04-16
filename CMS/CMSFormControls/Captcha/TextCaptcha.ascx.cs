using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Helpers;

public partial class CMSFormControls_Captcha_TextCaptcha : FormEngineUserControl
{
    #region "Variables"

    private List<TextBox> textBoxList = null;
    private string captchaValue = "captcha";

    #endregion


    #region "Properties"

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
            pnlAnswer.Enabled = value;
        }
    }


    /// <summary>
    /// Number of textboxs.
    /// </summary>
    public int Count
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Count"), 5);
        }
        set
        {
            SetValue("Count", value);
        }
    }


    /// <summary>
    /// String separator between textboxs.
    /// </summary>
    public string Separator
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Separator"), "-");
        }
        set
        {
            SetValue("Separator", value);
        }
    }


    /// <summary>
    /// Indicates whether the info label should be displayed.
    /// </summary>
    public bool ShowInfoLabel
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowInfoLabel"), false);
        }
        set
        {
            SetValue("ShowInfoLabel", value);
        }
    }


    /// <summary>
    /// Width of the CAPTCHA image.
    /// </summary>
    public int ImageWidth
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ImageWidth"), 80);
        }
        set
        {
            SetValue("ImageWidth", value);
        }
    }


    /// <summary>
    /// Height of the CAPTCHA image.
    /// </summary>
    public int ImageHeight
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ImageHeight"), 20);
        }
        set
        {
            SetValue("ImageHeight", value);
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return GetCaptachaValue();
        }
        set
        {
            captchaValue = ValidationHelper.GetString(value, string.Empty);
        }
    }


    /// <summary>
    /// Indicates whether the after text should be displayed.
    /// </summary>
    public bool ShowAfterText
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowAfterText"), false);
        }
        set
        {
            SetValue("ShowAfterText", value);
        }
    }

    #endregion


    #region "Methods"

    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        CreateTextBoxs();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Capta image url with anti cache query string parameter
        imgSecurityCode.ImageUrl = ResolveUrl(string.Format("~/CMSPages/Dialogs/CaptchaImage.aspx?hash={0}&captcha={1}&useWarp=0&width={2}&height={3}", Guid.NewGuid(), ClientID, ImageWidth, ImageHeight));

        // Show info label
        pnlSecurityLbl.Visible = ShowInfoLabel;

        // Show after text
        lblAfterText.Visible = ShowAfterText;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Regenerate CAPTCHA if possible
        if (!RequestHelper.IsAsyncPostback() || ControlsHelper.IsInUpdatePanel(this))
        {
            GenerateNew();

            // Update update panel if needed
            if (ControlsHelper.IsInUpdatePanel(this))
            {
                ControlsHelper.UpdateCurrentPanel(this);
            }
        }
    }


    /// <summary>
    /// Generates new CAPTCHA.
    /// </summary>
    public void GenerateNew()
    {
        ClearTextBoxs();
        captchaValue = GenerateRandomCode();
        WindowHelper.Add("CaptchaImageText" + ClientID, captchaValue);
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        bool isValid = false;
        object savedValue = WindowHelper.GetItem("CaptchaImageText" + ClientID);

        if (savedValue != null)
        {
            string captchaValue = GetCaptachaValue();
            string generatedCaptcha = ValidationHelper.GetString(savedValue, string.Empty);
            isValid = (captchaValue == generatedCaptcha);
        }

        return isValid;
    }


    /// <summary>
    /// Creates textboxs.
    /// </summary>
    private void CreateTextBoxs()
    {
        textBoxList = new List<TextBox>(Count);
        pnlAnswer.Controls.Clear();

        int index = 1;
        int addIndex = 0;
        for (int i = 0; i < Count; i++)
        {
            CMSTextBox txtBox = new CMSTextBox();
            txtBox.ID = "captcha_" + i;
            txtBox.MaxLength = 1;
            txtBox.CssClass = "CaptchaTextBoxSmall";

            pnlAnswer.Controls.AddAt(addIndex, txtBox);

            if (index < Count)
            {
                Label sepLabel = new Label();
                sepLabel.Text = Separator;
                sepLabel.CssClass = "form-control-text";
                addIndex++;
                pnlAnswer.Controls.AddAt(addIndex, sepLabel);
            }
            index++;
            addIndex++;

            textBoxList.Add(txtBox);
        }
    }


    /// <summary>
    /// Returns a string of Count random digits.
    /// </summary>
    private string GenerateRandomCode()
    {
        Random random = new Random(ClientID.GetHashCode() + (int)DateTime.Now.Ticks);

        StringBuilder sb = new StringBuilder();
        int index = 1;

        for (int i = 0; i < Count; i++)
        {
            int randomNumber = random.Next(10);
            sb.Append(randomNumber);
            if (index < Count)
            {
                sb.Append(Separator);
            }
            index++;
        }

        return sb.ToString();
    }


    /// <summary>
    /// Gets CAPTCHA value.
    /// </summary>
    private string GetCaptachaValue()
    {
        StringBuilder value = new StringBuilder();

        foreach (Control control in pnlAnswer.Controls)
        {
            TextBox txtBox = control as TextBox;
            if (txtBox != null)
            {
                value.Append(txtBox.Text);
            }

            Label label = control as Label;
            if (label != null)
            {
                value.Append(label.Text);
            }
        }

        return value.ToString();
    }


    /// <summary>
    /// Clears textboxs.
    /// </summary>
    private void ClearTextBoxs()
    {
        foreach (TextBox txtBox in textBoxList)
        {
            txtBox.Text = string.Empty;
        }
    }

    #endregion
}