using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.UIControls;

public partial class CMSModules_ImportExport_Controls_Global_Import_SiteDetails : CMSUserControl
{
    /// <summary>
    /// Site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return txtSiteName.Text;
        }
        set
        {
            txtSiteName.Text = value;
        }
    }


    /// <summary>
    /// Site display name.
    /// </summary>
    public string SiteDisplayName
    {
        get
        {
            return txtSiteDisplayName.Text;
        }
        set
        {
            txtSiteDisplayName.Text = value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        rfvSiteDisplayName.ErrorMessage = GetString("ImportSite.StepSiteDetails.SiteDisplayNameError");
        rfvSiteName.ErrorMessage = GetString("ImportSite.StepSiteDetails.SiteNameError");

        lblSiteDisplayName.Text = GetString("ImportSite.StepSiteDetails.SiteDisplayName");
        lblSiteName.Text = GetString("ImportSite.StepSiteDetails.SiteName");
    }
}