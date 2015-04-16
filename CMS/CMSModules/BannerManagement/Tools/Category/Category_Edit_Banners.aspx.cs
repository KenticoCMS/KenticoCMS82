using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.Base;
using CMS.BannerManagement;
using CMS.Modules;
using CMS.ExtendedControls;

[Action(0, "banner.bannercategory_edit_banner.newbanner", "~/CMSModules/BannerManagement/Tools/Banner/Banner_New.aspx?siteid={?siteid?}&parentobjectid={?parentobjectid?}")]

[EditedObject(BannerCategoryInfo.OBJECT_TYPE, "parentobjectid")]
[UIElement(ModuleName.BANNERMANAGEMENT, "Banners")]
public partial class CMSModules_BannerManagement_Tools_Category_Category_Edit_Banners : CMSBannerManagementEditPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize the grid view  
        this.gridBanners.OnAction += new OnActionEventHandler(gridBanners_OnAction);
        this.gridBanners.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridBanners_OnExternalDataBound);
        this.gridBanners.ZeroRowsText = ResHelper.GetString("banner.bannercategory_edit_banners.nobanners");

        // Look for category ID in the query string
        BannerCategoryInfo bci = EditedObject as BannerCategoryInfo;
        if (bci != null)
        {
            gridBanners.WhereCondition = "(BannerCategoryID = " + bci.BannerCategoryID + ")";
            gridBanners.EditActionUrl = GetEditUrl();
        }
        else
        {
            gridBanners.Visible = false;
            gridBanners.StopProcessing = true;
        }
    }

    #endregion


    #region "UniGrid events"

    private void gridBanners_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            // Deleteing of the category was fired
            case "delete":
                int bannerId = ValidationHelper.GetInteger(actionArgument, -1);
                BannerInfoProvider.DeleteBannerInfo(bannerId);                      
                break;
        }
    }


    object gridBanners_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "bannertype":
                BannerTypeEnum bannerType = (BannerTypeEnum)(int)parameter;
                
                return HTMLHelper.HTMLEncode(ResHelper.GetString("banner.bannertype." + bannerType.ToString().ToLowerCSafe()));
            case "hitsclicksleft":
                if (parameter == DBNull.Value)
                {
                    return GetString("general.unlimited");
                }

                int value = (int)parameter;

                if (value == 0)
                {
                    return "<span class=\"StatusDisabled\">" + value + "</span>";
                }

                return value;
            case "delete":
                DataRow row = ((DataRowView)((GridViewRow)parameter).DataItem).Row;

                int? siteID = row.IsNull("BannerSiteID") ? (int?)null : ValidationHelper.GetInteger(row["BannerSiteID"], 0);

                CMSGridActionButton button = ((CMSGridActionButton)sender);

                if (!HasUserModifyPermission(siteID))
                {
                    button.Enabled = false;
                }
                break;
        }

        return parameter;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    private String GetEditUrl()
    {
        UIElementInfo uiChild = UIElementInfoProvider.GetUIElementInfo("CMS.BannerManagement", "EditBanner");
        if (uiChild != null)
        {
            return URLHelper.AppendQuery(UIContextHelper.GetElementUrl(uiChild, UIContext), "objectid={0}&siteid={%SelectedSiteID%}&parentobjectid={?parentobjectid?}&displaytitle=false");
        }

        return String.Empty;
    }

    #endregion
}
