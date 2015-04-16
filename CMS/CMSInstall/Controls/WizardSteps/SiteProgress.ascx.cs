using System;

using CMS.Helpers;
using CMS.UIControls;


public partial class CMSInstall_Controls_WizardSteps_SiteProgress : CMSUserControl
{    
    /// <summary>
    /// Text displayed in progress panel.
    /// </summary>
    public string ProgressText
    {
        get
        {
            return ltlProgress.Text;
        }
        set
        {
            ltlProgress.Text = "<span id=\"lblProgress\" >" + value + "</span>";
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            ltlProgress.Text = "<span id=\"lblProgress\" ></span>";
        }
    }
}
