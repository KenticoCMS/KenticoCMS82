using System;
using System.Linq;

using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

// Edited objects
[EditedObject(ConversionInfo.OBJECT_TYPE, "conversionId")]

[UIElement(ModuleName.WEBANALYTICS, "Conversions.Campaigns")]
public partial class CMSModules_WebAnalytics_Pages_Tools_Conversion_Tab_Campaigns : CMSConversionPage
{
    #region "Variables"

    private string currentValues = string.Empty;
    private ConversionInfo ci;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        ci = EditedObject as ConversionInfo;

        if (ci == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        if (!ci.CheckPermissions(PermissionsEnum.Read, CurrentSiteName, CurrentUser))
        {
            RedirectToAccessDenied(ci.TypeInfo.ModuleName, "Read");
        }
        
        // Get the conversions
        currentValues = GetCampaigns();

        if (!RequestHelper.IsPostBack())
        {
            usCampaigns.Value = currentValues;
        }

        usCampaigns.WhereCondition = "CampaignSiteID = " + SiteContext.CurrentSiteID;
        usCampaigns.OnSelectionChanged += usConversions_OnSelectionChanged;
    }


    /// <summary>
    /// Returns string with campaign ids.
    /// </summary>    
    private string GetCampaigns()
    {
        var campaignIds =
            ConversionCampaignInfoProvider.GetConversionCampaigns()
                .Columns("CampaignID")
                .WhereEquals("ConversionID", ci.ConversionID)
                .Select(x => x.CampaignID);

        return TextHelper.Join(";", campaignIds);
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

        SaveCampaigns();
    }


    /// <summary>
    /// Saves the selected (removed) campaigns
    /// </summary>
    protected void SaveCampaigns()
    {
        // Remove old items
        string newValues = ValidationHelper.GetString(usCampaigns.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, currentValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            // Add all new items to site
            foreach (int campaignID in newItems.Select(item => ValidationHelper.GetInteger(item, 0)))
            {
                // remove conversion
                ConversionCampaignInfoProvider.RemoveConversionFromCampaign(ci.ConversionID, campaignID);
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(currentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            // Add all new items to site
            foreach (int campaignID in newItems.Select(item => ValidationHelper.GetInteger(item, 0)))
            {
                ConversionCampaignInfoProvider.AddConversionToCampaign(ci.ConversionID, campaignID);
            }
        }

        ShowChangesSaved();
    }

    #endregion
}