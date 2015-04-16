using System;

using CMS.Helpers;
using CMS.UIControls;
using CMS.DataEngine;
using CMS.ExtendedControls.ActionsConfig;
using CMS.SiteProvider;

[EditedObject(DataClassInfo.OBJECT_TYPE, "documenttypeid")]
public partial class CMSModules_DocumentTypes_Pages_Development_Scopes_List : GlobalAdminPage
{
    #region "Variables"

    private int siteID = 0;

    #endregion


    #region "Events"

    /// <summary>
    /// Page load event.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        siteID = URLHelper.IsPostback() ? ValidationHelper.GetInteger(selectSite.Value, UniSelector.US_ALL_RECORDS) : QueryHelper.GetInteger("siteid", SiteContext.CurrentSiteID);

        // Ensure default site value for (all) option
        if (siteID == 0)
        {
            siteID = UniSelector.US_ALL_RECORDS;
        }

        CurrentMaster.DisplayActionsPanel = true;
        CurrentMaster.DisplaySiteSelectorPanel = true;

        // New item link
        headerActions.AddAction(new HeaderAction()
        {
            Text = GetString("documenttype.scopes.newscope"),
            OnClientClick = "AddNewItem()",
            Enabled = (siteID > 0)
        });

        lblWarnNew.Visible = (siteID < 0);

        // Setup unigrid
        unigridScopes.WhereCondition = GenerateWhereCondition();
        unigridScopes.ZeroRowsText = GetString("general.nodatafound");
        unigridScopes.EditActionUrl = URLHelper.AddParameterToUrl(unigridScopes.EditActionUrl, "siteid", siteID.ToString());
        unigridScopes.OnBeforeDataReload += unigridScopes_OnBeforeDataReload;
        

        // Set site selector
        selectSite.PostbackOnDropDownChange = true;
        selectSite.OnlyRunningSites = false;
        selectSite.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;
        selectSite.Value = siteID;

        // Register correct script for new item
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "AddNewItem", ScriptHelper.GetScript(
            "function AddNewItem() { this.window.location = '" + ResolveUrl("Edit.aspx?siteID=" + siteID) + "'} "));
    }


    /// <summary>
    /// OnBeforeDataReload event handler.
    /// </summary>
    protected void unigridScopes_OnBeforeDataReload()
    {
        unigridScopes.NamedColumns["sitename"].Visible = (siteID < 0);
    }


    /// <summary>
    /// On selection changed.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    private void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        pnlUpdate.Update();
    }


    /// <summary>
    /// Generates where condition for unigrid.
    /// </summary>    
    private string GenerateWhereCondition()
    {
        string where = String.Empty;

        if (siteID >= 0)
        {
            where = SqlHelper.AddWhereCondition(where, "ScopeSiteID " + (siteID == 0 ? "IS NULL" : "= " + siteID));
        }

        return where;
    }

    #endregion
}