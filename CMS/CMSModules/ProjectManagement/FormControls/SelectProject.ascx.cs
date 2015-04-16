using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.Membership;
using CMS.ProjectManagement;
using CMS.SiteProvider;

public partial class CMSModules_ProjectManagement_FormControls_SelectProject : FormEngineUserControl
{
    #region "Properties"

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
            usProjects.Enabled = value;
            base.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            return usProjects.Value;
        }
        set
        {
            EnsureChildControls();
            usProjects.Value = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Live site mode
        usProjects.IsLiveSite = IsLiveSite;
        // Where condition
        string where = "(ProjectGroupID IS NULL AND ProjectNodeID IS NOT NULL AND ProjectSiteID = " + SiteContext.CurrentSiteID + ")";
        // Security where
        where = ProjectInfoProvider.CombineSecurityWhereCondition(where, MembershipContext.AuthenticatedUser, SiteContext.CurrentSiteName);
        // Set where condition to uniselector
        usProjects.WhereCondition = where;
    }


    /// <summary>
    /// Creates child controls and loads update panle container if it is required.
    /// </summary>
    protected override void CreateChildControls()
    {
        // If selector is not defined load updat panel container
        if (usProjects == null)
        {
            pnlUpdate.LoadContainer();
        }
        // Call base method
        base.CreateChildControls();
    }

    #endregion
}