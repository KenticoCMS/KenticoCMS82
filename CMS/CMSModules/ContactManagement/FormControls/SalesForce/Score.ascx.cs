using System;
using System.Web.UI.WebControls;

using CMS.EventLog;
using CMS.FormControls;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;

/// <summary>
/// Displays the score setting, and allows the user to edit it.
/// </summary>
public partial class CMSModules_ContactManagement_FormControls_SalesForce_Score : FormEngineUserControl
{

    #region "Private members"

    private bool mEnabled = true;
    private object mValue = null;

    #endregion

    #region "Public properties"

    /// <summary>
    /// Gets or sets the score identifier.
    /// </summary>
    public override object Value
    {
        get
        {
            return ScoreSelector.Value;
        }
        set
        {
            mValue = value;
            ScoreSelector.Value = mValue;
        }
    }

    /// <summary>
    /// Gets or sets the value indicating whether this control is enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
        }
    }

    /// <summary>
    /// Gets the client identifier of the control holding the setting value.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return ScoreSelector.ClientID;
        }
    }

    #endregion

    #region "Life-cycle methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        try
        {
            SiteInfo site = GetCurrentSiteForSettings();
            if (site == null)
            {
                MessageLabel.InnerText = GetString("sf.noglobalscore");
                MessageLabel.Visible = true;
            }
            else
            {
                ScoreSelector.WhereCondition = String.Format("ScoreSiteID = {0:D}", site.SiteID);
                ScoreSelector.Visible = true;
            }
        }
        catch (Exception exception)
        {
            HandleError(exception);
        }
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        ScoreSelector.Value = mValue;
        ScoreSelector.Enabled = Enabled;
    }

    #endregion

    #region "Private methods"

    private SiteInfo GetCurrentSiteForSettings()
    {
        int siteId = QueryHelper.GetInteger("SiteID", 0);
        SiteInfo site = SiteInfoProvider.GetSiteInfo(siteId);

        return site;
    }

    private void HandleError(Exception exception)
    {
        SalesForceError.Report(exception);
        EventLogProvider.LogException("Salesforce.com Connector", "ScoreFormControl", exception);
    }

    #endregion

}