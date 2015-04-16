using System;
using System.Collections;
using System.Security.Principal;
using System.Web;
using System.Web.UI;

using CMS.Core;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.Base;

[UIElementAttribute(ModuleName.CMS, "Delete")]
public partial class CMSModules_Sites_Pages_Site_Delete : GlobalAdminPage, ICallbackEventHandler
{
    #region "Variables"

    private static readonly Hashtable mManagers = new Hashtable();

    // Site ID
    private int siteId;

    // Site name
    private string siteName = "";

    // Site display name
    private string siteDisplayName = "";

    private SiteInfo si;

    private string backToSiteListUrl;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Deletion manager.
    /// </summary>
    public SiteDeletionManager DeletionManager
    {
        get
        {
            string key = "delManagers_" + ProcessGUID;
            if (mManagers[key] == null)
            {
                // Restart of the application
                if (ApplicationInstanceGUID != SystemHelper.ApplicationInstanceGUID)
                {
                    LogStatusEnum progressLog = DeletionInfo.GetProgressState();
                    if (progressLog != LogStatusEnum.Finish)
                    {
                        DeletionInfo.LogDeletionState(LogStatusEnum.UnexpectedFinish, ResHelper.GetAPIString("Site_Delete.Applicationrestarted", "<strong>Application has been restarted and the logging of the site delete process has been terminated. Please make sure that the site is deleted. If it is not, please repeate the deletion process.</strong><br />"));
                    }
                }

                SiteDeletionManager dm = new SiteDeletionManager(DeletionInfo);
                mManagers[key] = dm;
            }
            return (SiteDeletionManager)mManagers[key];
        }
        set
        {
            string key = "delManagers_" + ProcessGUID;
            mManagers[key] = value;
        }
    }


    /// <summary>
    /// Application instance GUID.
    /// </summary>
    public Guid ApplicationInstanceGUID
    {
        get
        {
            if (ViewState["ApplicationInstanceGUID"] == null)
            {
                ViewState["ApplicationInstanceGUID"] = SystemHelper.ApplicationInstanceGUID;
            }

            return ValidationHelper.GetGuid(ViewState["ApplicationInstanceGUID"], Guid.Empty);
        }
    }


    /// <summary>
    /// Import process GUID.
    /// </summary>
    public Guid ProcessGUID
    {
        get
        {
            if (ViewState["ProcessGUID"] == null)
            {
                ViewState["ProcessGUID"] = Guid.NewGuid();
            }

            return ValidationHelper.GetGuid(ViewState["ProcessGUID"], Guid.Empty);
        }
    }


    /// <summary>
    /// Persistent settings key.
    /// </summary>
    public string PersistentSettingsKey
    {
        get
        {
            return "SiteDeletion_" + ProcessGUID + "_Settings";
        }
    }


    /// <summary>
    /// Deletion info.
    /// </summary>
    public DeletionInfo DeletionInfo
    {
        get
        {
            DeletionInfo delInfo = (DeletionInfo)PersistentStorageHelper.GetValue(PersistentSettingsKey);
            if (delInfo == null)
            {
                throw new Exception("[SiteDelete.DeletionInfo]: Deletion info has been lost.");
            }
            return delInfo;
        }
        set
        {
            PersistentStorageHelper.SetValue(PersistentSettingsKey, value);
        }
    }

    #endregion


    #region "Page events"

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        InitAlertLabels();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script for pendingCallbacks repair
        ScriptHelper.FixPendingCallbacks(Page);

        if (!RequestHelper.IsCallback())
        {
            if (!RequestHelper.IsPostBack())
            {
                // Initialize deletion info
                DeletionInfo = new DeletionInfo();
                DeletionInfo.PersistentSettingsKey = PersistentSettingsKey;
            }

            DeletionManager.DeletionInfo = DeletionInfo;

            // Register the script to perform get flags for showing buttons retrieval callback
            ScriptHelper.RegisterClientScriptBlock(this, GetType(), "GetState", ScriptHelper.GetScript("function GetState(cancel){ return " + Page.ClientScript.GetCallbackEventReference(this, "cancel", "SetStateMssg", null) + " } \n"));

            // Setup page title text and image
            PageTitle.TitleText = GetString("Site_Edit.DeleteSite");
            backToSiteListUrl = UIContextHelper.GetElementUrl(ModuleName.CMS, "Sites", false);

            PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
            {
                Text = GetString("general.sites"),
                RedirectUrl = backToSiteListUrl,
                Target = "cmsdesktop",
            });

            PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
            {
                Text = GetString("Site_Edit.DeleteSite"),
            });

            // Get site ID
            siteId = QueryHelper.GetInteger("siteId", 0);

            si = SiteInfoProvider.GetSiteInfo(siteId);
            if (si != null)
            {
                siteName = si.SiteName;
                siteDisplayName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(si.DisplayName));

                ucHeader.Header = string.Format(GetString("Site_Delete.Header"), siteDisplayName);
                ucHeaderConfirm.Header = GetString("Site_Delete.HeaderConfirm");

                // Initialize web root path
                DeletionInfo.WebRootFullPath = HttpContext.Current.Server.MapPath("~/");

                DeletionInfo.DeletionLog = string.Format("I" + SiteDeletionManager.SEPARATOR + DeletionManager.DeletionInfo.GetAPIString("Site_Delete.DeletingSite", "Initializing deletion of the site") + SiteDeletionManager.SEPARATOR + SiteDeletionManager.SEPARATOR, siteName);

                headConfirmation.Text = string.Format(GetString("Site_Edit.Confirmation"), siteDisplayName);
                btnYes.Text = GetString("General.Yes");
                btnNo.Text = GetString("General.No");
                btnOk.Text = GetString("General.OK");
                lblLog.Text = string.Format(GetString("Site_Delete.DeletingSite"), siteDisplayName);
            }

            btnYes.Click += btnYes_Click;
            btnNo.Click += btnNo_Click;
            btnOk.Click += btnOK_Click;

            // Javascript functions
            string script =
                "function SetStateMssg(rValue, context) \n" +
                "{\n" +
                "   var values = rValue.split('<#>');\n" +
                "   if((values[0]=='E') || (values[0]=='F') || values=='')\n" +
                "   {\n" +
                "       StopStateTimer();\n" +
                "       var actDiv = document.getElementById('actDiv');\n" +
                "       if (actDiv != null) { actDiv.style.display = 'none'; }\n" +
                "       BTN_Enable('" + btnOk.ClientID + "');\n" +
                "   }\n" +
                "   if((values[0]=='E') && values[2] && (values[2].length > 0))\n" +
                "   {\n" +
                "       document.getElementById('" + lblError.ClientID + "').innerHTML = values[2];\n" +
                "       document.getElementById('" + pnlError.ClientID + "').style.removeProperty('display');\n" +
                "   }\n" +
                "   else if(values[0]=='I')\n" +
                "   {\n" +
                "       document.getElementById('" + lblLog.ClientID + "').innerHTML = values[1];\n" +
                "   }\n" +
                "   else if((values=='') || (values[0]=='F'))\n" +
                "   {\n" +
                "       document.getElementById('" + lblLog.ClientID + "').innerHTML = values[1];\n" +
                "   }\n" +
                "   if (values[3] && (values[3].length > 0))\n" +
                "   {\n" +
                "       document.getElementById('" + lblWarning.ClientID + "').innerHTML = values[3];\n" +
                "       document.getElementById('" + pnlWarning.ClientID + "').style.removeProperty('display');\n" +
                "   }\n" +
                "}\n";

            // Register the script to perform get flags for showing buttons retrieval callback
            ScriptHelper.RegisterClientScriptBlock(this, GetType(), "GetDeletionState", ScriptHelper.GetScript(script));
        }
    }

    #endregion


    #region "Control event handlers"

    protected void btnOK_Click(object sender, EventArgs e)
    {
        URLHelper.Redirect(backToSiteListUrl);
    }


    protected void btnNo_Click(object sender, EventArgs e)
    {
        URLHelper.Redirect(backToSiteListUrl);
    }


    private void btnYes_Click(object sender, EventArgs e)
    {
        pnlConfirmation.Visible = false;
        pnlDeleteSite.Visible = true;

        // Start the timer for the callbacks
        ltlScript.Text = ScriptHelper.GetScript("StartStateTimer();");

        // Deletion info initialization
        var di = DeletionInfo;

        di.DeleteAttachments = chkDeleteDocumentAttachments.Checked;
        di.DeleteMediaFiles = chkDeleteMediaFiles.Checked;
        di.DeleteMetaFiles = chkDeleteMetaFiles.Checked;
        di.SiteName = siteName;
        di.SiteDisplayName = siteDisplayName;

        var dm = DeletionManager;

        dm.CurrentUser = MembershipContext.AuthenticatedUser;
        dm.DeletionInfo = di;

        AsyncWorker worker = new AsyncWorker();
        worker.RunAsync(dm.DeleteSite, WindowsIdentity.GetCurrent());
    }

    #endregion


    #region "ICallBackEventHandler methods"

    /// <summary>
    /// Callback event handler.
    /// </summary>
    /// <param name="argument">Callback argument</param>
    public void RaiseCallbackEvent(string argument)
    {
        hdnLog.Value = DeletionManager.DeletionInfo.DeletionLog;
    }


    /// <summary>
    /// Callback result retrieving handler.
    /// </summary>
    public string GetCallbackResult()
    {
        return hdnLog.Value;
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Iniliazes (hides) alert labels
    /// </summary>
    private void InitAlertLabels()
    {
        // Do not use Visible property to hide this elements. They are used in JS.
        pnlError.Attributes.CssStyle.Add(HtmlTextWriterStyle.Display, String.IsNullOrEmpty(lblError.Text) ? "none" : "block");
        pnlWarning.Attributes.CssStyle.Add(HtmlTextWriterStyle.Display, String.IsNullOrEmpty(lblWarning.Text) ? "none" : "block");
    }

    #endregion
}