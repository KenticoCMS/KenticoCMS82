using System;

using CMS.Helpers;
using CMS.DataEngine;
using CMS.UIControls;


public partial class CMSModules_Settings_Pages_Categories : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterJQuery(Page);

        TreeViewCategories.RootIsClickable = true;

        // Get selected category ID
        int categoryId;
        if (!RequestHelper.IsPostBack() && QueryHelper.Contains("selectedCategoryId"))
        {
            // Get from URL
            categoryId = QueryHelper.GetInteger("selectedCategoryId", 0);
        }
        else if (Request.Form["selectedCategoryId"] != null)
        {
            // Get from postback
            categoryId = ValidationHelper.GetInteger(Request.Form["selectedCategoryId"], 0);
        }
        else
        {
            // Select root by default
            categoryId = SettingsCategoryInfoProvider.GetRootSettingsCategoryInfo().CategoryID;
        }
        TreeViewCategories.CategoryID = categoryId;
    }


    /// <summary>
    /// Reloads tree content.
    /// </summary>
    protected override void OnLoadComplete(EventArgs e)
    {
        base.OnLoadComplete(e);

        // Reload tree after selected site has changed.
        if (RequestHelper.IsPostBack())
        {
            TreeViewCategories.ReloadData();
        }
    }
}