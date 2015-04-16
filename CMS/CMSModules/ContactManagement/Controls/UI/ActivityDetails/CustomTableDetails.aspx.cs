using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;

[Security(Resource = ModuleName.CONTACTMANAGEMENT, Permission = "ReadActivities")]
public partial class CMSModules_ContactManagement_Controls_UI_ActivityDetails_CustomTableDetails : CMSModalPage
{
    #region "Methods"

    /// <summary>
    /// Page init event handler
    /// </summary>
    protected void Page_Init(object sender, EventArgs e)
    {
        // Check permissions
        if (ActivityHelper.AuthorizedReadActivity(SiteContext.CurrentSiteID, true))
        {
            if (!QueryHelper.ValidateHash("hash"))
            {
                return;
            }

            int tableID = QueryHelper.GetInteger("tableid", 0);
            int itemID = QueryHelper.GetInteger("itemid", 0);

            if ((tableID > 0) && (itemID > 0))
            {
                DataClassInfo customTable = DataClassInfoProvider.GetDataClassInfo(tableID);
                if (customTable == null)
                {
                    return;
                }
                form.CustomTableId = tableID;
                form.ItemID = itemID;
            }

            PageTitle.TitleText = GetString("om.activitydetals.viewrecorddetail");
        }
    }


    /// <summary>
    /// Page PreRender event handler
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (form != null)
        {
            form.SubmitButton.Visible = false;
        }
    }

    #endregion
}