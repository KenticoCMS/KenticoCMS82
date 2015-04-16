using System;

using CMS.FormControls;
using CMS.Helpers;
using CMS.SiteProvider;

public partial class CMSModules_Scoring_FormControls_SelectScore : FormEngineUserControl
{
    #region "Variables"

    int mSiteID;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return uniSelector.Enabled;
        }
        set
        {
            uniSelector.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return uniSelector.Value;
        }
        set
        {
            uniSelector.Value = value;
        }
    }


    /// <summary>
    /// Current site ID.
    /// </summary>
    public int SiteID
    {
        get
        {
            if (mSiteID <= 0)
            {
                mSiteID = ValidationHelper.GetInteger(GetValue("SiteID"), SiteContext.CurrentSiteID);
            }
            return mSiteID;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        InitSelector();
    }


    /// <summary>
    /// Initializes uniselector
    /// </summary>
    private void InitSelector()
    {
        uniSelector.WhereCondition = "ScoreEnabled = 1 AND ScoreSiteID = " + SiteID;
        uniSelector.AllowAll = ValidationHelper.GetBoolean(GetValue("AllowAll"), uniSelector.AllowAll);
        uniSelector.AllowEmpty = ValidationHelper.GetBoolean(GetValue("AllowEmpty"), uniSelector.AllowEmpty);
        uniSelector.Reload(true);
    }

    #endregion
}