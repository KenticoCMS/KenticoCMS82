using System;

using CMS.ExtendedControls;
using CMS.Helpers;

using AjaxControlToolkit;

public partial class CMSAdminControls_ContentRating_Controls_Stars : AbstractRatingControl
{
    /// <summary>
    /// Enables/disables rating scale.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return !elemRating.ReadOnly;
        }
        set
        {
            elemRating.ReadOnly = !value;
            if (value)
            {
                elemRating.RemoveCssClass("disabled");
            }
            else
            {
                elemRating.AddCssClass("disabled");
            }
        }
    }


    /// <summary>
    /// Returns current rating.
    /// </summary>
    public override double GetCurrentRating()
    {
        if (elemRating.MaxRating <= 0)
        {
            CurrentRating = 0;
        }
        else
        {
            CurrentRating = ValidationHelper.GetDouble(elemRating.CurrentRating, 0) / elemRating.MaxRating;
        }
        return CurrentRating;
    }


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        ReloadData();
    }


    public override void ReloadData()
    {
        // Avoid exception if max value is less or equal to zero
        if (MaxRating <= 0)
        {
            elemRating.Visible = false;
            return;
        }

        elemRating.MaxRating = MaxRating;
        // Clear stars to enable further rating
        if (Enabled && !ExternalManagement)
        {
            elemRating.CurrentRating = 0;
        }
        else
        {
            // Display rating result if in readonly mode
            elemRating.CurrentRating = Convert.ToInt32(Math.Round(CurrentRating * MaxRating, MidpointRounding.AwayFromZero));
        }

        if (Enabled)
        {
            elemRating.Changed += new RatingEventHandler(elemRating_Changed);
            elemRating.AutoPostBack = true;
        }
        else
        {
            elemRating.Changed -= new RatingEventHandler(elemRating_Changed);
            elemRating.AutoPostBack = false;
        }

        // Switch RTL or LTR layout
        if (CultureHelper.IsPreferredCultureRTL())
        {
            elemRating.RatingDirection = RatingDirection.RightToLeftBottomToTop;
        }
        else
        {
            elemRating.RatingDirection = RatingDirection.LeftToRightTopToBottom;
        }
    }


    protected void elemRating_Changed(object sender, RatingEventArgs e)
    {
        // Actualize CurrentRating properties
        elemRating.CurrentRating = ValidationHelper.GetInteger(e.Value, 0);
        GetCurrentRating();
        // Throw the rating event
        OnRating();
    }
}