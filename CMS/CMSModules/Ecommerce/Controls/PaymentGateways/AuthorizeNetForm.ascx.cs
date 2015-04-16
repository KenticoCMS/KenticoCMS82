using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.EcommerceProvider;
using CMS.Helpers;

public partial class CMSModules_Ecommerce_Controls_PaymentGateways_AuthorizeNetForm : CMSPaymentGatewayForm
{
    #region "Private properties"

    /// <summary>
    /// Credit card number.
    /// </summary>
    private string CreditCardNumber
    {
        get
        {
            return txtCardNumber.Text.Trim();
        }
    }


    /// <summary>
    /// Credit card CCV (Card Code Verification).
    /// </summary>
    private string CreditCardCCV
    {
        get
        {
            return txtCCV.Text.Trim();
        }
    }


    /// <summary>
    /// Credit card expiration date.
    /// </summary>
    private DateTime CreditCardExpiration
    {
        get
        {
            if ((drpMonths.SelectedValue == "") || (drpYears.SelectedValue == ""))
            {
                return DateTimeHelper.ZERO_TIME;
            }
            else
            {
                DateTime result = DateTime.MinValue;
                result = result.AddYears(Convert.ToInt32(drpYears.SelectedValue) - 1);
                result = result.AddMonths(Convert.ToInt32(drpMonths.SelectedValue) - 1);
                return result;
            }
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Labels
        lblCCV.ToolTip = ResHelper.GetString("AuthorizeNetForm.CardCCVTooltip");
        lblCCV.Attributes["style"] = "cursor:help;";

        // Validations
        rfvCardNumber.ErrorMessage = ResHelper.GetString("AuthorizeNetForm.ErrorCreditCardNumber");
        rfvCCV.ErrorMessage = ResHelper.GetString("AuthorizeNetForm.ErrorCreditCardCCV");

        // Load dropdown lists
        if ((drpMonths.Items.Count == 0) || (drpYears.Items.Count == 0))
        {
            InitializeLists();
        }
    }


    /// <summary>
    /// Process customer payment data.
    /// </summary>
    public override string ProcessData()
    {
        PaymentGatewayCustomData[AuthorizeNetParameters.CARD_NUMBER] = CreditCardNumber;
        PaymentGatewayCustomData[AuthorizeNetParameters.CARD_CCV] = CreditCardCCV;
        PaymentGatewayCustomData[AuthorizeNetParameters.CARD_EXPIRATION] = CreditCardExpiration;
        return "";
    }


    /// <summary>
    /// Validates customer payment data.
    /// </summary>
    public override string ValidateData()
    {
        string errorMessage = "";

        if (CreditCardNumber == "")
        {
            errorMessage = ResHelper.GetString("AuthorizeNetForm.ErrorCreditCardNumber");
        }

        if ((errorMessage == "") && (CreditCardCCV == ""))
        {
            errorMessage = ResHelper.GetString("AuthorizeNetForm.ErrorCreditCardCCV");
        }

        if (errorMessage != "")
        {
            // Show error message
            lblError.Visible = true;
            lblError.Text = errorMessage;
        }

        if (CreditCardExpiration == DateTimeHelper.ZERO_TIME)
        {
            lblErrorDate.Text = ResHelper.GetString("AuthorizeNetForm.ErrorCreditCardExpiration");
            lblErrorDate.Visible = true;

            if (errorMessage == "")
            {
                errorMessage = lblErrorDate.Text;
            }
        }

        return errorMessage;
    }


    /// <summary>
    /// Loads data to dropdownlists.
    /// </summary>
    private void InitializeLists()
    {
        // Load years
        for (int i = 0; i < 10; i++)
        {
            string year = Convert.ToString(DateTime.Now.Year + i);
            drpYears.Items.Add(new ListItem(year, year));
        }
        drpYears.Items.Insert(0, new ListItem("-", ""));


        // Load months
        for (int i = 1; i <= 12; i++)
        {
            string text = (i < 10) ? "0" + i : i.ToString();
            drpMonths.Items.Add(new ListItem(text, i.ToString()));
        }
        drpMonths.Items.Insert(0, new ListItem("-", ""));
    }
}