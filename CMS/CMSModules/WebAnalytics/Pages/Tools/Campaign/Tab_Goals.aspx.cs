using System;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

// Edited object
[EditedObject(CampaignInfo.OBJECT_TYPE, "campaignId")]

[UIElement("CMS.WebAnalytics", "Campaign.Goals")]
public partial class CMSModules_WebAnalytics_Pages_Tools_Campaign_Tab_Goals : CMSCampaignPage
{
    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        CampaignInfo ci = EditedObject as CampaignInfo;

        // Validate SiteID for non administrators
        if ((ci != null) && (!MembershipContext.AuthenticatedUser.IsGlobalAdministrator))
        {
            if (ci.CampaignSiteID != SiteContext.CurrentSiteID)
            {
                RedirectToAccessDenied(GetString("cmsmessages.accessdenied"));
            }
        }

        if (ci != null)
        {
            if ((ci.CampaignImpressions == 0) || (ci.CampaignTotalCost == 0))
            {
                ShowInformation(GetString("campaign.noimpressionsorcost"));
            }
        }

        // Register event handlers
        EditForm.OnAfterDataLoad += EditForm_OnAfterDataLoad;
        EditForm.OnBeforeValidate += EditForm_OnBeforeValidate;

        base.OnInit(e);
    }


    private void EditForm_OnAfterDataLoad(object sender, EventArgs e)
    {
        // Set control properties
        string options = "<item value=\"False\" text=\"" + GetString("campaign.absolutevalue") + "\" /><item value=\"True\" text=\"{0}\" />";

        rbVisitorsPercent.SetValue("repeatdirection", "horizontal");
        rbVisitorsPercent.SetValue("options", String.Format(options, GetString("campaign.percofimpress")));

        rbConversionsPercent.SetValue("repeatdirection", "horizontal");
        rbConversionsPercent.SetValue("options", String.Format(options, GetString("campaign.percofimpress")));

        rbValuePercent.SetValue("repeatdirection", "horizontal");
        rbValuePercent.SetValue("options", String.Format(options, GetString("campaign.percoftotalcost")));

        rbPerVisitorPercent.SetValue("repeatdirection", "horizontal");
        rbPerVisitorPercent.SetValue("options", String.Format(options, GetString("campaign.percofpervisitor")));
    }


    private void EditForm_OnBeforeValidate(object sender, EventArgs e)
    {
        String errorMsg = String.Empty;

        // Test whether all goal values are higher then the min values
        int visitorsMin = ValidationHelper.GetInteger(EditForm.FieldControls["CampaignGoalVisitorsMin"].Value, 0);
        int visitorsGoal = ValidationHelper.GetInteger(EditForm.FieldControls["CampaignGoalVisitors"].Value, 0);

        int conversionsMin = ValidationHelper.GetInteger(EditForm.FieldControls["CampaignGoalConversionsMin"].Value, 0);
        int conversionsGoal = ValidationHelper.GetInteger(EditForm.FieldControls["CampaignGoalConversions"].Value, 0);

        int goalValueMin = ValidationHelper.GetInteger(EditForm.FieldControls["CampaignGoalValueMin"].Value, 0);
        int goalValue = ValidationHelper.GetInteger(EditForm.FieldControls["CampaignGoalValue"].Value, 0);

        int goalPerVisitorMin = ValidationHelper.GetInteger(EditForm.FieldControls["CampaignGoalPerVisitorMin"].Value, 0);
        int goalPerVisitor = ValidationHelper.GetInteger(EditForm.FieldControls["CampaignGoalPerVisitor"].Value, 0);

        bool visitorsAsPercent = ValidationHelper.GetBoolean(EditForm.FieldControls["CampaignGoalVisitorsPercent"].Value, false);
        bool conversionsAsPercent = ValidationHelper.GetBoolean(EditForm.FieldControls["CampaignGoalConversionsPercent"].Value, false);

        if (visitorsMin > visitorsGoal)
        {
            errorMsg = GetString("campaign.error.goal");
        }

        if (conversionsMin > conversionsGoal)
        {
            errorMsg = GetString("campaign.error.goal");
        }

        if (goalValueMin > goalValue)
        {
            errorMsg = GetString("campaign.error.goal");
        }

        if (goalPerVisitorMin > goalPerVisitor)
        {
            errorMsg = GetString("campaign.error.goal");
        }

        // Percent of impressions may not be higher then 100
        if ((conversionsAsPercent) && (conversionsGoal > 100))
        {
            errorMsg = GetString("campaign.error.goalspercent");
        }

        if ((visitorsAsPercent) && (visitorsGoal > 100))
        {
            errorMsg = GetString("campaign.error.goalspercent");
        }

        // Test zero values
        if (visitorsMin < 0)
        {
            errorMsg = GetString("campaign.valuegreaterzero");
        }

        if (visitorsGoal < 0)
        {
            errorMsg = GetString("campaign.valuegreaterzero");
        }

        if (conversionsMin < 0)
        {
            errorMsg = GetString("campaign.valuegreaterzero");
        }

        if (conversionsGoal < 0)
        {
            errorMsg = GetString("campaign.valuegreaterzero");
        }

        if (goalValueMin < 0)
        {
            errorMsg = GetString("campaign.valuegreaterzero");
        }

        if (goalValue < 0)
        {
            errorMsg = GetString("campaign.valuegreaterzero");
        }

        if (goalPerVisitorMin < 0)
        {
            errorMsg = GetString("campaign.valuegreaterzero");
        }

        if (goalPerVisitor < 0)
        {
            errorMsg = GetString("campaign.valuegreaterzero");
        }

        if (errorMsg != String.Empty)
        {
            EditForm.ShowError(errorMsg);
            EditForm.StopProcessing = true;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Hide some default error labels  - we want them on specific positions 
        // They destroy design otherwise
        Label lblErr = EditForm.FieldErrorLabels["CampaignConversionsCount"] as Label;
        Label lblVis = EditForm.FieldErrorLabels["CampaignVisitors"] as Label;

        if (lblErr != null)
        {
            lblErr.Visible = false;
        }

        if (lblVis != null)
        {
            lblVis.Visible = false;
        }

        base.OnPreRender(e);
    }


    protected void btnOk_Click(object sender, EventArgs ea)
    {
        EditForm.SaveData("");
    }

    #endregion
}