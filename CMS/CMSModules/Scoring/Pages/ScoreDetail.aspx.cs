using System;
using System.Data;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;

/// <summary>
/// Displays a table of score details.
/// </summary>
[Title("om.score.details.title")]
[Security(Resource = "CMS.Scoring", Permission = "Read")]
[CheckLicence(FeatureEnum.LeadScoring)]
[CheckLicence(FeatureEnum.ContactManagement)]
public partial class CMSModules_Scoring_Pages_ScoreDetail : CMSToolsModalPage
{
    #region "Variables"

    private int contactId;

    // Default page size 15
    private const int PAGESIZE = 15;

    #endregion


    #region "Methods"
    
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get contact ID
        contactId = QueryHelper.GetInteger("contactId", 0);
        if (contactId == 0)
        {
            RequestHelper.EndResponse();
        }

        // Check contact's existence
        ContactInfo contact = ContactInfoProvider.GetContactInfo(contactId);
        EditedObject = contact;

        // Prevent accessing issues from sites other than current site
        if (contact.ContactSiteID != SiteContext.CurrentSiteID)
        {
            RedirectToResourceNotAvailableOnSite("Contact with ID " + contactId);
        }

        // Initialize unigrid
        gridElem.WhereCondition = string.Format("ContactID={0} AND ScoreID={1} AND (Expiration IS NULL OR (DATEDIFF(d, getdate(), Expiration) >= 0))", contactId, QueryHelper.GetInteger("scoreid", 0));
        gridElem.Pager.DefaultPageSize = PAGESIZE;
        gridElem.Pager.ShowPageSize = false;
        gridElem.FilterLimit = PAGESIZE;
        gridElem.OnExternalDataBound += UniGrid_OnExternalDataBound;
    }


    protected object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView row = (DataRowView)parameter;

        switch (sourceName)
        {
            case "quantity":
                // Get contact's total value for the rule
                int value = ValidationHelper.GetInteger(DataHelper.GetDataRowValue(row.Row, "Value"), 0);
                // Get rule value
                int ruleValue = ValidationHelper.GetInteger(DataHelper.GetDataRowValue(row.Row, "RuleValue"), 0);
                if (value == 0 || ruleValue == 0)
                {
                    return 0;
                }

                // Display number of recurrences of the rule evaluation
                return value / ruleValue;

            default:
                return parameter;
        }
    }

    #endregion
}