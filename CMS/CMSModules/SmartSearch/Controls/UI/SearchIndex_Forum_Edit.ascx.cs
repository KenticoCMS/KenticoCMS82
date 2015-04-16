using System;
using System.Web.UI;
using System.Linq;

using CMS.Core;
using CMS.FormControls;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.DataEngine;
using CMS.Search;

public partial class CMSModules_SmartSearch_Controls_UI_SearchIndex_Forum_Edit : CMSAdminEditControl, IPostBackEventHandler
{
    #region "Variables"

    private string mItemType;
    private FormEngineUserControl selForum;
    private readonly bool smartSearchEnabled = SettingsKeyInfoProvider.GetBoolValue("CMSSearchIndexingEnabled");

    #endregion


    #region "Public properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets item type.
    /// </summary>
    public string ItemType
    {
        get
        {
            return mItemType;
        }
        set
        {
            mItemType = value;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Show panel with message how to enable indexing
        ucDisabledModule.SettingsKeys = "CMSSearchIndexingEnabled";
        ucDisabledModule.InfoText = GetString("srch.searchdisabledinfo");

        // Module forums is not available
        if (!(ModuleManager.IsModuleLoaded(ModuleName.FORUMS)))
        {
            return;
        }

        selForum = LoadUserControl("~/CMSModules/Forums/FormControls/ForumSelector.ascx") as FormEngineUserControl;
        if (selForum != null)
        {
            // Set default vaules for forum selector
            selForum.IsLiveSite = false;
            selForum.SetValue("selectionmode", SelectionModeEnum.MultipleTextBox);
            selForum.SetValue("DisplayAdHocOption", false);

            plcForumSelector.Controls.Add(selForum);
        }

        // Set events and default values for site selector
        selSite.AllowAll = false;
        selSite.UseCodeNameForSelection = true;
        selSite.DropDownSingleSelect.AutoPostBack = true;
        selSite.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;

        LoadControls();

        if (ItemType == SearchIndexSettingsInfo.TYPE_ALLOWED)
        {
            selSite.AllowAll = true;
        }

        var siteIDs = SearchIndexSiteInfoProvider.GetIndexSiteBindings(ItemID).Column("IndexSiteID").Select(X => X.IndexSiteID.ToString()).ToList();
        if (siteIDs.Count > 0)
        {
            var siteWhere = SqlHelper.GetWhereCondition<string>("SiteID", siteIDs, false);

            // Preselect current site if it is assigned to index
            var sisiId = QueryHelper.GetGuid("guid", Guid.Empty);
            var isNew = sisiId == Guid.Empty;
            if (!RequestHelper.IsPostBack() && isNew && siteIDs.Contains(SiteContext.CurrentSiteID.ToString()))
            {
                selSite.Value = SiteContext.CurrentSiteName;
            }

            selSite.UniSelector.WhereCondition = siteWhere;
        }
        else
        {
            selSite.Enabled = false;
            selForum.Enabled = false;
            btnOk.Enabled = false;

            ShowError(GetString("srch.index.nositeselected"));
        }

        selSite.Reload(true);

        selForum.SetValue("SiteName", Convert.ToString(selSite.Value));

        // Init controls
        if (!RequestHelper.IsPostBack())
        {
            SetControlsStatus();
        }
    }


    private void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        selForum.Value = String.Empty;
        SetControlsStatus();
    }


    /// <summary>
    /// Enable or disable controls with dependence on current settings.
    /// </summary>
    protected void SetControlsStatus()
    {
        selForum.Enabled = true;

        string siteName = ValidationHelper.GetString(selSite.Value, String.Empty);
        if (String.IsNullOrEmpty(siteName) || (siteName == "##ALL##"))
        {
            selForum.Enabled = false;
        }
        else
        {
            selForum.SetValue("SiteName", siteName);
        }
    }


    /// <summary>
    /// Resets all boxes.
    /// </summary>
    public void LoadControls()
    {
        SearchIndexInfo sii = SearchIndexInfoProvider.GetSearchIndexInfo(ItemID);

        // If we are editing existing search index
        if (sii != null)
        {
            SearchIndexSettings sis = new SearchIndexSettings();
            sis.LoadData(sii.IndexSettings.GetData());
            SearchIndexSettingsInfo sisi = sis.GetSearchIndexSettingsInfo(ItemGUID);
            if (sisi != null)
            {
                if (!RequestHelper.IsPostBack())
                {
                    selSite.Value = sisi.SiteName;
                    selForum.SetValue("SiteName", sisi.SiteName);
                    selForum.Value = sisi.ForumNames;
                }
                ItemType = sisi.Type;
            }
        }

        plcForumsInfo.Visible = true;
        if (ItemType == SearchIndexSettingsInfo.TYPE_EXLUDED)
        {
            plcForumsInfo.Visible = false;
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Stores data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Perform validation
        string errorMessage = new Validator().NotEmpty(selSite.Value, GetString("srch.err.emptysite")).Result;

        if (String.IsNullOrEmpty(errorMessage) && (ItemType == SearchIndexSettingsInfo.TYPE_EXLUDED) && String.IsNullOrEmpty(Convert.ToString(selForum.Value)))
        {
            errorMessage = GetString("srch.err.emptyforum");
        }

        if (String.IsNullOrEmpty(errorMessage))
        {
            SearchIndexInfo sii = SearchIndexInfoProvider.GetSearchIndexInfo(ItemID);
            if (sii != null)
            {
                SearchIndexSettings sis = sii.IndexSettings;
                SearchIndexSettingsInfo sisi;

                var isInsert = false;

                if (ItemGUID != Guid.Empty)
                {
                    // If we are updating existing Search Index Settings Info
                    sisi = sis.GetSearchIndexSettingsInfo(ItemGUID);
                }
                else
                {
                    // If we are creating new Search Index Settings Info
                    isInsert = true;

                    sisi = new SearchIndexSettingsInfo();
                    sisi.ID = Guid.NewGuid();
                    sisi.Type = ItemType;
                }

                // Save values
                if (sisi != null)
                {
                    string siteName = selSite.Value.ToString();
                    if (siteName == "-1")
                    {
                        siteName = String.Empty;
                    }

                    sisi.SiteName = siteName;
                    sisi.ForumNames = selForum.Value.ToString();

                    // Update settings item
                    sis.SetSearchIndexSettingsInfo(sisi);

                    // Update xml value
                    sii.IndexSettings = sis;
                    SearchIndexInfoProvider.SetSearchIndexInfo(sii);
                    ItemGUID = sisi.ID;

                    if (isInsert)
                    {
                        // Redirect to edit mode
                        var editUrl = "SearchIndex_Content_Edit.aspx";
                        editUrl = URLHelper.AddParameterToUrl(editUrl, "indexId", sii.IndexID.ToString());
                        editUrl = URLHelper.AddParameterToUrl(editUrl, "guid", sisi.ID.ToString());
                        editUrl = URLHelper.AddParameterToUrl(editUrl, "saved", "1");
                        if (smartSearchEnabled)
                        {
                            editUrl = URLHelper.AddParameterToUrl(editUrl, "rebuild", "1");
                        }
                        URLHelper.Redirect(editUrl);
                    }

                    ShowChangesSaved();

                    if (smartSearchEnabled)
                    {
                        // Show rebuild message
                        ShowInformation(String.Format(GetString("srch.indexrequiresrebuild"), "<a href=\"javascript:" + Page.ClientScript.GetPostBackEventReference(this, "saved") + "\">" + GetString("General.clickhere") + "</a>"));
                    }
                }
                // Error loading SearchIndexSettingsInfo
                else
                {
                    ShowError(GetString("srch.err.loadingsisi"));
                }
            }
            // Error loading SearchIndexInfo
            else
            {
                ShowError(GetString("srch.err.loadingsii"));
            }
        }
        else
        {
            ShowError(errorMessage);
        }
    }

    #endregion


    #region "IPostBackEventHandler Members"

    public void RaisePostBackEvent(string eventArgument)
    {
        if (eventArgument == "saved")
        {
            if (SearchHelper.CreateRebuildTask(ItemID))
            {
                ShowInformation(GetString("srch.index.rebuildstarted"));
            }
            else
            {
                ShowError(GetString("index.nocontent"));
            }
        }
    }

    #endregion
}