using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Helpers;
using CMS.IO;

public partial class CMSModules_Content_FormControls_SelectRatings : FormEngineUserControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return drpRatingsControls.Enabled;
        }
        set
        {
            drpRatingsControls.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return ValidationHelper.GetString(drpRatingsControls.SelectedValue, null);
        }
        set
        {
            EnsureChildControls();
            drpRatingsControls.SelectedValue = ValidationHelper.GetString(value, null);
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


    protected override void CreateChildControls()
    {
        base.CreateChildControls();
        string oldSelectedValue = drpRatingsControls.SelectedValue;

        string[] fileList = Directory.GetFiles(Server.MapPath(AbstractRatingControl.GetRatingControlUrl("")), "*.ascx");
        string fileNameOnly = null;
        drpRatingsControls.Items.Clear();
        foreach (string fileName in fileList)
        {
            fileNameOnly = Path.GetFileNameWithoutExtension(fileName);
            drpRatingsControls.Items.Add(new ListItem(fileNameOnly, fileNameOnly));
        }

        if (drpRatingsControls.Items.FindByValue(oldSelectedValue) != null)
        {
            drpRatingsControls.SelectedValue = oldSelectedValue;
        }
    }

    #endregion
}