using System;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.FormControls;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Newsletters_FormControls_NewsletterSubscriberSelector : FormEngineUserControl
{
    #region "Variables"

    private int mSiteId = 0;
    private bool mShowSiteFilter = true;
    private string mResourcePrefix = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets ID of the site. User of this site are displayed
    /// Use 0 for current siteid, -1 for all sites(no filter)
    /// Note: SiteID is not used if site filter is enabled
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether to allow more than one user to select.
    /// </summary>
    public SelectionModeEnum SelectionMode
    {
        get
        {
            EnsureChildControls();
            return usSubscribers.SelectionMode;
        }
        set
        {
            EnsureChildControls();
            usSubscribers.SelectionMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            EnsureChildControls();

            base.Enabled = value;
            usSubscribers.Enabled = value;
        }
    }


    /// <summary>
    /// Gets the current UniSelector instance.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            EnsureChildControls();
            return usSubscribers;
        }
    }


    ///<summary>
    /// Gets or sets field value.
    ///</summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            return usSubscribers.Value;
        }
        set
        {
            EnsureChildControls();
            usSubscribers.Value = value;
        }
    }


    /// <summary>
    /// Gets or sets if site filter should be shown or not.
    /// </summary>
    public bool ShowSiteFilter
    {
        get
        {
            return mShowSiteFilter;
        }
        set
        {
            mShowSiteFilter = value;
        }
    }


    /// <summary>
    /// Gets or sets the resource prefix of uni selector. If not set default values are used.
    /// </summary>
    public string ResourcePrefix
    {
        get
        {
            return mResourcePrefix;
        }
        set
        {
            mResourcePrefix = value;
            usSubscribers.ResourcePrefix = value;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            EnsureChildControls();
            base.IsLiveSite = value;
            usSubscribers.IsLiveSite = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set prefix if not set
        if (string.IsNullOrEmpty(ResourcePrefix))
        {
            usSubscribers.ResourcePrefix = "selectsubscriber";
        }

        // Add sites filter
        if (ShowSiteFilter)
        {
            usSubscribers.FilterControl = "~/CMSFormControls/Filters/SiteFilter.ascx";

            usSubscribers.SetValue("DefaultFilterValue", (SiteID > 0) ? SiteID : SiteContext.CurrentSiteID);
            usSubscribers.SetValue("FilterMode", "subscriber");
        }

        // Select users depending on site; if filter enabled, where condition is added from filter itself
        if (!ShowSiteFilter && (SiteID >= 0))
        {
            int siteId = (SiteID == 0) ? SiteContext.CurrentSiteID : SiteID;
            string where = "SubscriberSiteID = " + siteId.ToString();
            usSubscribers.WhereCondition = SqlHelper.AddWhereCondition(usSubscribers.WhereCondition, where);
        }

        // Limit selection only to subscribers, remove roles and users
        usSubscribers.WhereCondition = SqlHelper.AddWhereCondition(usSubscribers.WhereCondition, "(SubscriberType IS NULL)");
    }


    /// <summary>
    /// Creates child controls and loads update panel container if it is required.
    /// </summary>
    protected override void CreateChildControls()
    {
        // If selector is not defined load updat panel container
        if (usSubscribers == null)
        {
            pnlUpdate.LoadContainer();
        }
        // Call base method
        base.CreateChildControls();
    }

    #endregion
}