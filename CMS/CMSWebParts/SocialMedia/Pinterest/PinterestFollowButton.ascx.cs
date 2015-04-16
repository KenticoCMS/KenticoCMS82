using System;

using CMS.Helpers;
using CMS.SocialMedia;


public partial class CMSWebParts_SocialMedia_Pinterest_PinterestFollowButton : SocialMediaAbstractWebPart
{
    #region "Private fiels"

    private bool mHide;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Indicates whether to hide content of the WebPart
    /// </summary>
    public override bool HideContent
    {
        get
        {
            return mHide;
        }
        set
        {
            mHide = value;
            ltlPluginCode.Visible = !value;
        }
    }



    /// <summary>
    /// Username.
    /// </summary>
    public string Username
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Username"), string.Empty);
        }
        set
        {
            SetValue("Username", value);
        }
    }


    /// <summary>
    /// Display type.
    /// </summary>
    public string DisplayType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DisplayType"), string.Empty);
        }
        set
        {
            SetValue("DisplayType", value);
        }
    }

    #endregion


    #region "Methods"


    /// <summary>
    /// Initializes the control properties
    /// </summary>
    protected override void SetupControl()
    {
        if (this.StopProcessing)
        {
            // Do not process
        }
        else
        {
            // Build plugin code
            string src = "http://pinterest.com/" + Username;

            // Output HTML code
            string output = "<a href=\"{0}\"><img src=\"http://passets-cdn.pinterest.com/images/{1}.png\" alt=\"Follow Me on Pinterest\" /></a>";
            ltlPluginCode.Text = String.Format(output, src, DisplayType);
        }
    }


    /// <summary>
    /// Reloads the control data
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }

    #endregion
}