using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.Base;
using CMS.DataEngine;
using CMS.PortalEngine;
using CMS.UIControls;

[UIElementAttribute(ModuleName.CMS, "Settings")]
public partial class CMSModules_Settings_Pages_Keys : GlobalAdminPage, IPostBackEventHandler
{
    #region "Private variables"

    private bool mSearchDescription = true;
    private string mSearchText = "";
    private int mCategoryId;
    private int mSiteId;

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get query strings
        mCategoryId = QueryHelper.GetInteger("categoryid", 0);
        mSiteId = QueryHelper.GetInteger("siteid", 0);

        // Assign category id, site id
        SettingsGroupViewer.CategoryID = mCategoryId;
        SettingsGroupViewer.SiteID = mSiteId;

        if (SettingsGroupViewer.SettingsCategoryInfo == null)
        {
            // Get root category info
            SettingsCategoryInfo sci = SettingsCategoryInfoProvider.GetRootSettingsCategoryInfo();
            SettingsGroupViewer.CategoryID = sci.CategoryID;
        }

        // Get search text if exist
        mSearchText = QueryHelper.GetString("search", String.Empty).Trim();
        mSearchDescription = QueryHelper.GetBoolean("description", false);

        // If root selected show search controls
        if (SettingsGroupViewer.CategoryName == "CMS.Settings")
        {
            pnlSearch.Visible = true;
            anchorDropup.Visible = false;
            lblNoSettings.ResourceString = "Development.Settings.SearchSettings";
            if (!string.IsNullOrEmpty(mSearchText))
            {
                // Set searched values
                if (!URLHelper.IsPostback())
                {
                    txtSearch.Text = mSearchText;
                    chkDescription.Checked = QueryHelper.GetBoolean("description", true);
                }
            }
            RegisterSearchScript();
        }

        // Set master title
        PageTitle.TitleText = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(SettingsGroupViewer.SettingsCategoryInfo.CategoryDisplayName));
        // Check, if there are any groups
        DataSet ds = SettingsCategoryInfoProvider.GetSettingsCategories("CategoryIsGroup = 1 AND CategoryParentID = " + mCategoryId, null, -1, "CategoryID");
        if ((!DataHelper.DataSourceIsEmpty(ds)) || (!string.IsNullOrEmpty(mSearchText)))
        {
            CurrentMaster.HeaderActions.ActionsList = GetHeaderActions();
            CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        }
        else
        {
            lblNoSettings.Visible = true;
        }

        ScriptHelper.RegisterSaveShortcut(this, "save", null);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!pnlSearch.Visible && (SettingsGroupViewer.KeyItems.Count == 0))
        {
            lblNoSettings.Visible = true;
            CurrentMaster.HeaderActions.Visible = false;
        }
    }

    #endregion


    #region "Private Methods"

    /// <summary>
    /// Script for search parameters for site selector
    /// </summary>
    private void RegisterSearchScript()
    {
        Control headerActions = CurrentMaster.HeaderActionsPlaceHolder.FindControl("pnlActions");


        string script = @" 
    function GetSearchValues() {
        var search = document.getElementById('" + txtSearch.ClientID + @"').value;
        var searchSettings = new Array('" + mCategoryId + @"', document.getElementById('" + txtSearch.ClientID + @"').value, document.getElementById('" + chkDescription.ClientID + @"').checked );
        return searchSettings;
    }

    function DisableHeaderActions(){
        var element = document.getElementById('" + headerActions.ClientID + @"');
        element.style.display = 'none';
    }
";
        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "SearchScripts", ScriptHelper.GetScript(script));

    }


    /// <summary>
    /// Gets string array representing header actions.
    /// </summary>
    /// <returns>Array of strings</returns>
    private List<HeaderAction> GetHeaderActions()
    {
        List<HeaderAction> actions = new List<HeaderAction>();

        actions.Add(new SaveAction(this)
        {
            Text = GetString("Header.Settings.SaveChanged"),
            CommandName = "lnkSaveChanges_Click",
            CommandArgument = String.Empty,
            RegisterShortcutScript = true
        });

        // Add reset action only for global settings
        if (mSiteId == 0)
        {
            actions.Add(new HeaderAction
            {
                Text = GetString("Header.Settings.ResetToDefault"),
                OnClientClick = string.Format(@" if (confirm({0})) {{ return true; }} return false;", ScriptHelper.GetString(GetString("Header.Settings.ResetToDefaultConfirmation"))),
                CommandName = "lnkResetToDefault_Click",
                CommandArgument = String.Empty,
                ButtonStyle = ButtonStyle.Default
            });
        }

        // Add export link if required
        var exportUrl = "GetSettings.aspx";
        exportUrl = URLHelper.AddParameterToUrl(exportUrl, "siteId", mSiteId.ToString());
        exportUrl = URLHelper.AddParameterToUrl(exportUrl, "categoryId", SettingsGroupViewer.CategoryID.ToString());
        exportUrl = URLHelper.AddParameterToUrl(exportUrl, "search", mSearchText);
        exportUrl = URLHelper.AddParameterToUrl(exportUrl, "description", mSearchDescription.ToString());

        actions.Add(new HeaderAction
        {
            Text = GetString("settings.keys.exportsettings"),
            OnClientClick = "window.open(" + ScriptHelper.GetString(exportUrl) + ");",
            ButtonStyle = ButtonStyle.Default
        });

        return actions;
    }


    /// <summary>
    /// Saves the settings.
    /// </summary>
    private void SaveSettings()
    {
        SettingsGroupViewer.SaveChanges();

        bool containsDebugKey = false;
        bool containsPerformanceKey = false;

        foreach(var keyItem in SettingsGroupViewer.KeyItems)
        {
            if (keyItem.CategoryName.StartsWithCSafe("cms.debug", true))
            {
                containsDebugKey = true;
            }

            if (keyItem.CategoryName.StartsWithCSafe("cms.performance", true))
            {
                containsPerformanceKey = true;
            }
        }

        // Special case for Debug settings - debug settings have to be reloaded to take effect
        if (containsDebugKey)
        {
            DebugHelper.ResetDebugSettings();
        }

        // Special case for performance settings - performance settings have to be reloaded to take effect
        if (containsPerformanceKey)
        {
            RequestHelper.ResetPerformanceSettings();
        }
    }

    #endregion


    #region "Event Handlers"

    /// <summary>
    /// Handles actions performed on the master header.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event argument</param>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "lnksavechanges_click":
                SaveSettings();
                break;

            case "lnkresettodefault_click":
                if ((mSiteId >= 0) && (mCategoryId > 0))
                {
                    SettingsGroupViewer.ResetToDefault();
                    URLHelper.Redirect(GetRefreshUrl(true));
                }
                break;
        }
    }


    /// <summary>
    /// Handles search button action
    /// </summary>
    protected void btnSearch_OnClick(object sender, EventArgs e)
    {
        URLHelper.Redirect(GetRefreshUrl());
    }


    /// <summary>
    /// Generates url that refreshed the content of the page based on the current settings
    /// </summary>
    protected string GetRefreshUrl(bool reserToDefault = false)
    {
        string queryString = string.Format(
            "categoryid={0}&siteid={1}&search={2}&description={3}&resettodefault={4}",
            mCategoryId,
            mSiteId,
            txtSearch.Text,
            chkDescription.Checked,
            reserToDefault ? "1" : "0"
        );

        return URLHelper.AppendQuery(UIContextHelper.GetElementUrl(ModuleName.CMS, "Settings.Keys"), queryString);
    }

    #endregion


    #region "IPostBackEventHandler Members"

    /// <summary>
    /// Handles postback events.
    /// </summary>
    /// <param name="eventArgument">Postback argument</param>
    public void RaisePostBackEvent(string eventArgument)
    {
        if (eventArgument.ToLowerCSafe() == "save")
        {
            SaveSettings();
        }
    }

    #endregion
}