using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.ExtendedControls;
using CMS.Helpers;

public partial class CMSAdminControls_ContentRating_Controls_RadioButtons : AbstractRatingControl
{
    #region "Public properties"

    /// <summary>
    /// Enables/disables rating scale
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return btnSubmit.Enabled;
        }
        set
        {
            btnSubmit.Enabled = value;
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Rerturns current rating.
    /// </summary>
    public override double GetCurrentRating()
    {
        CurrentRating = 0;

        // Check for division by zero
        if (MaxRating > 0)
        {
            // Loop through entire control collection and find checked radio button
            foreach (Control cntrl in plcContent.Controls)
            {
                if ((cntrl is CMSRadioButton) && (((CMSRadioButton)cntrl).Checked))
                {
                    int tmp = ValidationHelper.GetInteger(cntrl.ID.Substring(7), -1);
                    if (tmp > 0)
                    {
                        CurrentRating = (double)tmp / MaxRating;
                        break;
                    }
                }
            }
        }

        return CurrentRating;
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        ReloadData();
    }


    public override void ReloadData()
    {
        int currPos = Convert.ToInt32(Math.Round(CurrentRating * MaxRating, MidpointRounding.AwayFromZero));
        plcContent.Controls.Clear();
        plcContent.Controls.Add(new LiteralControl("<table class=\"CntRatingRadioTable\"><tr>\n"));

        // Create radio buttons
        for (int i = 1; i <= MaxRating; i++)
        {
            plcContent.Controls.Add(new LiteralControl("<td>"));

            CMSRadioButton radBtn = new CMSRadioButton();
            radBtn.ID = "radBtn_" + Convert.ToString(i);
            radBtn.Enabled = Enabled;
            if (!Enabled)
            {
                radBtn.Checked = i == currPos;
            }
            radBtn.GroupName = ClientID;
            plcContent.Controls.Add(radBtn);

            // WAI validation
            LocalizedLabel lbl = new LocalizedLabel();
            lbl.Display = false;
            lbl.EnableViewState = false;
            lbl.ResourceString = "general.rating";
            lbl.AssociatedControlID = radBtn.ID;

            plcContent.Controls.Add(lbl);
            plcContent.Controls.Add(new LiteralControl("</td>"));
        }
        plcContent.Controls.Add(new LiteralControl("\n</tr>\n<tr>\n"));

        // Create labels
        for (int i = 1; i <= MaxRating; i++)
        {
            plcContent.Controls.Add(new LiteralControl("<td>" + i.ToString() + "</td>"));
        }
        plcContent.Controls.Add(new LiteralControl("\n</tr>\n</table>"));

        if (Enabled)
        {
            btnSubmit.Text = ResHelper.GetString("general.ok");
            btnSubmit.Click += new EventHandler(btnSubmit_Click);
        }

        // Hide button when control is disabled or external management is used
        btnSubmit.Visible = Enabled && !ExternalManagement;
    }


    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        // Actualize CurrentRating property
        GetCurrentRating();
        // Throw the rating event
        OnRating();
    }

    #endregion
}