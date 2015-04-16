using System;

using CMS.Core;
using CMS.Helpers;
using CMS.OnlineForms;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;


[Security(Resource = ModuleName.CONTACTMANAGEMENT, Permission = "ReadActivities")]
public partial class CMSModules_ContactManagement_Controls_UI_ActivityDetails_BizFormDetails : CMSModalPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        // Check permissions
        if (ActivityHelper.AuthorizedReadActivity(SiteContext.CurrentSiteID, true))
        {
            if (!QueryHelper.ValidateHash("hash"))
            {
                return;
            }

            int bizId = QueryHelper.GetInteger("bizid", 0);
            int recId = QueryHelper.GetInteger("recid", 0);

            if ((bizId > 0) && (recId > 0))
            {
                BizFormInfo bfi = BizFormInfoProvider.GetBizFormInfo(bizId);

                if (bfi == null)
                {
                    return;
                }

                bizRecord.ItemID = recId;
                bizRecord.SiteName = SiteInfoProvider.GetSiteName(bfi.FormSiteID);
                bizRecord.FormName = bfi.FormName;
            }

            PageTitle.TitleText = GetString("om.activitydetals.viewrecorddetail");
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (bizRecord != null)
        {
            bizRecord.SubmitButton.Visible = false;
        }
    }
}