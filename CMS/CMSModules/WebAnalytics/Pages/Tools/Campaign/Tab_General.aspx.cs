using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

// Edited objects
[EditedObject(CampaignInfo.OBJECT_TYPE, "campaignId")]
[UIElement(ModuleName.WEBANALYTICS, "Campaign.General", false, true)]
public partial class CMSModules_WebAnalytics_Pages_Tools_Campaign_Tab_General : CMSCampaignPage
{
    #region "Variables"

    private bool modalDialog;

    // Help variable for set info label of UI form
    private string infoText = String.Empty;

    #endregion


    #region "Methods"


    protected void Page_PreInit(object sender, EventArgs e)
    {
        modalDialog = QueryHelper.GetBoolean("modalDialog", false);
        if (modalDialog)
        {
            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master";
            SetDialogButtons();
        }

        IsDialog = modalDialog;
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        // Checks all permissions for web analytics
        CheckAllPermissions();

        CampaignInfo ci = EditedObject as CampaignInfo;

        string campaignName = QueryHelper.GetString("campaignName", String.Empty);
        if (campaignName != String.Empty)
        {
            // Try to check dialog mode
            ci = CampaignInfoProvider.GetCampaignInfo(campaignName, SiteContext.CurrentSiteName);
        }

        if ((campaignName != String.Empty) && (ci == null))
        {
            // Set warning text
            infoText = String.Format(GetString("campaign.editedobjectnotexits"), HTMLHelper.HTMLEncode(campaignName));

            // Create campaign info based on campaign name
            ci = new CampaignInfo();
            ci.CampaignDisplayName = campaignName;
            ci.CampaignName = campaignName;
        }

        // Validate SiteID for non administrators
        if (ci != null)
        {
            if (!ci.CheckPermissions(PermissionsEnum.Read, CurrentSiteName, CurrentUser))
            {
                RedirectToAccessDenied(ci.TypeInfo.ModuleName, "Read");
            }
        }

        if (modalDialog)
        {
            if (ci != null)
            {
                PageTitle.TitleText = GetString("analytics.campaign");
            }
            else
            {
                PageTitle.TitleText = GetString("campaign.campaign.new");
            }
        }

        if (ci != null)
        {
            EditedObject = ci;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        editElem.UIFormControl.SubmitButton.Visible = !modalDialog;

        // Set info label text created in preinit
        editElem.UIFormControl.ShowInformation(infoText);
    }


    private void UpdateUniSelector(bool closeOnSave)
    {
        string selector = HTMLHelper.HTMLEncode(QueryHelper.GetString("selectorid", string.Empty));
        CampaignInfo info = editElem.UIFormControl.EditedObject as CampaignInfo;
        if (!string.IsNullOrEmpty(selector) && (info != null))
        {
            ScriptHelper.RegisterWOpenerScript(this);
            // Add selector refresh
            string script =
                string.Format(@"if (wopener) {{ wopener.US_SelectNewValue_{0}('{1}'); }}", selector, info.CampaignName);

            if (closeOnSave)
            {
                script += "CloseDialog();";
            }

            ScriptHelper.RegisterStartupScript(this, GetType(), "UpdateSelector", script, true);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (QueryHelper.GetBoolean("saved", false) && !URLHelper.IsPostback())
        {
            UpdateUniSelector(true);
        }

        base.OnPreRender(e);
    }


    private void SetDialogButtons()
    {
        var master = CurrentMaster as ICMSModalMasterPage;
        if (master != null)
        {
            master.Save += (s, ea) => editElem.Save(true);
            master.ShowSaveAndCloseButton();
        }
    }

    #endregion
}