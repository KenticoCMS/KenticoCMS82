using System;

using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_ContactManagement_Controls_UI_Activity_Details : CMSAdminListControl
{
    #region "Constants"

    private const string PATH_TO_CONTROLS = "~/CMSModules/ContactManagement/Controls/UI/ActivityDetails/{0}.ascx";

    #endregion
    

    #region "Properties"

    /// <summary>
    /// Gets or sets activity ID.
    /// </summary>
    public int ActivityID
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        LoadData();
    }


    private void LoadData()
    {
        ActivityInfo ai = ActivityInfoProvider.GetActivityInfo(ActivityID);
        if (ai == null)
        {
            return;
        }
        
        string pathToControl = String.Format(PATH_TO_CONTROLS, ai.ActivityType);

        if (FileHelper.FileExists(pathToControl))
        {
            ActivityDetail ucDetails = Page.LoadUserControl(pathToControl) as ActivityDetail;
            if (ucDetails.LoadData(ai))
            {
                pnlDetails.Controls.Add(ucDetails);
                return;
            }
        }

        // Control doesn't exist or couldn't load data. It's ok for custom activities or activities without details.
        Visible = false;
    }

    #endregion
}