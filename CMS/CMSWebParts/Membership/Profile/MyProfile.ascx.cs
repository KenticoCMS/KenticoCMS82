using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.FormEngine;
using CMS.Helpers;
using CMS.PortalControls;

public partial class CMSWebParts_Membership_Profile_MyProfile : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the name of alternative form
    /// Default value is cms.user.EditProfile
    /// </summary>
    public string AlternativeFormName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("AlternativeFormName"), "cms.user.EditProfile");
        }
        set
        {
            SetValue("AlternativeFormName", value);
            myProfile.AlternativeFormName = value;
        }
    }

    /// <summary>
    /// Indicates if field visibility could be edited on user form.
    /// </summary>
    public bool AllowEditVisibility
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowEditVisibility"), myProfile.AllowEditVisibility);
        }
        set
        {
            SetValue("AllowEditVisibility", value);
            myProfile.AllowEditVisibility = value;
        }
    }

    #endregion


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        myProfile.IsLiveSite = true;

        if (StopProcessing)
        {
            myProfile.StopProcessing = true;
        }
        else
        {
            // Get alternative form info
            AlternativeFormInfo afi = AlternativeFormInfoProvider.GetAlternativeFormInfo(AlternativeFormName);
            if (afi != null)
            {
                myProfile.AlternativeFormName = AlternativeFormName;
                myProfile.AllowEditVisibility = AllowEditVisibility;
            }
            else
            {
                lblError.Text = String.Format(GetString("altform.formdoesntexists"), AlternativeFormName);
                lblError.Visible = true;
                plcContent.Visible = false;
            }
        }
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }
}