using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.ExtendedControls;
using CMS.Helpers;

public partial class CMSInlineControls_RatingControl : InlineUserControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets type of rating scale.
    /// </summary>
    public string RatingType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RatingType"), null);
        }
        set
        {
            SetValue("RatingType", value);
            elemRating.RatingType = value;
        }
    }


    /// <summary>
    /// Gets or sets control parameter.
    /// </summary>
    public override string Parameter
    {
        get
        {
            return RatingType;
        }
        set
        {
            RatingType = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        elemRating.RatingType = RatingType;
    }

    #endregion
}