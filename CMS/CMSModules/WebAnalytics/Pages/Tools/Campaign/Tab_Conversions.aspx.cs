using System;
using System.Linq;

using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

[EditedObject(CampaignInfo.OBJECT_TYPE, "campaignId")]
[UIElement(ModuleName.WEBANALYTICS, "Campaign.Conversions")]
public partial class CMSModules_WebAnalytics_Pages_Tools_Campaign_Tab_Conversions : CMSCampaignPage
{
    #region "Variables"

    private string currentValues = string.Empty;
    private int campaignID;
    private CampaignInfo ci;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        ci = EditedObject as CampaignInfo;
        if (!RequestHelper.IsPostBack())
        {
            if (ci != null)
            {
                rbAllConversions.Checked = ci.CampaignUseAllConversions;
                rbSelectedConversions.Checked = !ci.CampaignUseAllConversions;
            }
        }

        // Validate SiteID for non administrators
        if (ci != null)
        {
            if (!ci.CheckPermissions(PermissionsEnum.Read, CurrentSiteName, CurrentUser))
            {
                RedirectToAccessDenied(ci.TypeInfo.ModuleName, "Read");
            }
        }

        plcTable.Visible = !rbAllConversions.Checked;

        campaignID = QueryHelper.GetInteger("campaignid", 0);

        // Get the conversions
        currentValues = GetConversions();

        if (!RequestHelper.IsPostBack())
        {
            usConversions.Value = currentValues;
        }

        usConversions.WhereCondition = "ConversionSiteID = " + SiteContext.CurrentSiteID;
        usConversions.OnSelectionChanged += usConversions_OnSelectionChanged;
        rbAllConversions.CheckedChanged += ConversionsSelection_changed;
        rbSelectedConversions.CheckedChanged += ConversionsSelection_changed;
    }


    protected void ConversionsSelection_changed(object sender, EventArgs ea)
    {
        // Check "modify" permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.WebAnalytics", "ManageCampaigns"))
        {
            RedirectToAccessDenied("CMS.WebAnalytics", "Manage campaigns");
        }

        if (ci != null)
        {
            ci.CampaignUseAllConversions = rbAllConversions.Checked;
            if (ci.CampaignUseAllConversions)
            {
                ConversionCampaignInfoProvider.RemoveAllConversionsFromCampaign(campaignID);
            }

            CampaignInfoProvider.SetCampaignInfo(ci);

            ShowChangesSaved();
        }
    }


    /// <summary>
    /// Returns string with conversions ids
    /// </summary>    
    private string GetConversions()
    {
        var conversionIds =
            ConversionCampaignInfoProvider.GetConversionCampaigns()
                .Columns("ConversionID")
                .WhereEquals("CampaignID", campaignID)
                .Select(x => x.ConversionID);

        return TextHelper.Join(";", conversionIds);
    }


    protected void usConversions_OnSelectionChanged(object sender, EventArgs e)
    {
        // Check permissions
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.WebAnalytics", "ManageConversions"))
        {
            RedirectToAccessDenied("CMS.WebAnalytics", "Manage conversions");
        }

        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.WebAnalytics", "ManageCampaigns"))
        {
            RedirectToAccessDenied("CMS.WebAnalytics", "Manage campaigns");
        }

        SaveConversions();
    }


    /// <summary>
    /// Saves the changes in conversions
    /// </summary>
    protected void SaveConversions()
    {
        // Remove old items
        string newValues = ValidationHelper.GetString(usConversions.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, currentValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            // Add all new items to site
            foreach (int conversionID in newItems.Select(item => ValidationHelper.GetInteger(item, 0)))
            {
                // remove conversion
                ConversionCampaignInfoProvider.RemoveConversionFromCampaign(conversionID, campaignID);
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(currentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            // Add all new items to site
            foreach (int conversionID in newItems.Select(item => ValidationHelper.GetInteger(item, 0)))
            {
                ConversionCampaignInfoProvider.AddConversionToCampaign(conversionID, campaignID);
            }
        }

        ShowChangesSaved();
    }
}