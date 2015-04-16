using System;

using CMS.FormControls;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

public partial class CMSModules_WebAnalytics_Controls_UI_Campaign_Edit : CMSAdminEditControl
{
    #region "Variables"

    private String formerCodeName = String.Empty;

    #endregion


    #region "Properties"

    /// <summary>
    /// UIForm control used for editing objects properties.
    /// </summary>
    public UIForm UIFormControl
    {
        get
        {
            return EditForm;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            EditForm.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            EditForm.IsLiveSite = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsModalDialog())
        {
            EditForm.SubmitButton.Visible = false;
            EditForm.RedirectUrlAfterCreate = "";
        }
        else
        {
            string editUrl = UIContextHelper.GetElementUrl("CMS.WebAnalytics", "CampaignProperties");
            editUrl = URLHelper.AddParameterToUrl(editUrl, "objectId", "{%EditedObject.ID%}");
            editUrl = URLHelper.AddParameterToUrl(editUrl, "saved", "1");
            editUrl = URLHelper.AddParameterToUrl(editUrl, "displayTitle", "0");
            EditForm.RedirectUrlAfterCreate = editUrl;
        }
    }


    protected void EditForm_OnAfterSave(object sender, EventArgs e)
    {
        CampaignInfo ci = EditForm.EditedObject as CampaignInfo;
        // If code name has changed (on existing object) => Rename all analytics statistics data.
        if ((ci != null) && (ci.CampaignName != formerCodeName) && (formerCodeName != String.Empty))
        {
            CampaignInfoProvider.RenameCampaignStatistics(formerCodeName, ci.CampaignName, SiteContext.CurrentSiteID);
        }
    }


    protected void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        CampaignInfo ci = EditForm.EditedObject as CampaignInfo;
        if (ci != null)
        {
            if (ci.CampaignSiteID == 0)
            {
                ci.CampaignSiteID = SiteContext.CurrentSiteID;
            }

            // Save old codename
            formerCodeName = ci.CampaignName;
        }
    }


    protected void EditForm_OnAfterValidate(object sender, EventArgs e)
    {
        DateTime from = ValidationHelper.GetDateTime(EditForm.GetFieldValue("CampaignOpenFrom"), DateTimeHelper.ZERO_TIME);
        DateTime to = ValidationHelper.GetDateTime(EditForm.GetFieldValue("CampaignOpenTo"), DateTimeHelper.ZERO_TIME);

        // Validate FromDate <= ToDate
        if (!DateTimeHelper.IsValidFromTo(from, to))
        {
            EditForm.StopProcessing = true;
            EditForm.DisplayErrorLabel("CampaignOpenFrom", GetString("campaign.wronginterval"));
        }
    }


    /// <summary>
    /// Saves the data
    /// </summary>
    /// <param name="redirect">If true, use server redirect after successful save</param>
    public bool Save(bool redirect)
    {
        string url = "tab_general.aspx?campaignid={%EditedObject.ID%}&saved=1&modaldialog=true&selectorID={?selectorID?}";
        
        return EditForm.SaveData(redirect ? url : String.Empty);
    }


    /// <summary>
    /// Returns true if control is opened as a modal dialog.
    /// </summary>
    public bool IsModalDialog()
    {
        return QueryHelper.GetBoolean("modaldialog", false);
    }

    #endregion
}