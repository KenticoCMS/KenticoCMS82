using System;

using CMS.Helpers;
using CMS.UIControls;


public partial class CMSInstall_Controls_WizardSteps_DBProgress : CMSUserControl
{
    /// <summary>
    /// Text displayed in progress panel.
    /// </summary>
    public string ProgressText
    {
        get
        {
            return ltlDBProgress.Text;
        }
        set
        {
            ltlDBProgress.Text = "<span id=\"lblProgress\" >" + value + "</span>";
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        lblDBProgress.Text = ResHelper.GetFileString("Install.lblDBProgress");

        if (!RequestHelper.IsPostBack())
        {
            ltlDBProgress.Text = "<span id=\"lblProgress\" ></span>";
        }
    }
}
