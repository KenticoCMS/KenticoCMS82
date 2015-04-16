using System;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Base;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.DocumentEngine;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.PortalControls;
using CMS.Globalization;
using CMS.Membership;
using CMS.FormEngine;
using CMS.DataEngine;

using TimeZoneInfo = CMS.Globalization.TimeZoneInfo;
using TreeNode = CMS.DocumentEngine.TreeNode;

[UIElement(ModuleName.CONTENT, "Properties.General")]
public partial class CMSModules_Content_CMSDesk_Properties_General : CMSPropertiesPage
{
    #region "Variables"

    protected bool canEditOwner = false;
    protected bool canEdit = true;
    protected bool clearCache = false;

    private bool hasAdHocBoard;
    private bool hasAdHocForum;

    protected FormEngineUserControl usrOwner = null;
    protected FormEngineUserControl fcDocumentGroupSelector = null;

    private PageInfo currentPage;

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        // Culture independent data
        SplitModeAllwaysRefresh = true;

        // Non-versioned data are modified
        DocumentManager.UseDocumentHelper = false;

        base.OnInit(e);

        // Check UI element permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", "Properties.General"))
        {
            RedirectToUIElementAccessDenied("CMS.Content", "Properties.General");
        }

        // Redirect to information page when no UI elements displayed
        if (pnlUIAdvanced.IsHidden && pnlUICache.IsHidden && pnlUIDesign.IsHidden &&
            pnlUIOther.IsHidden && pnlUIOwner.IsHidden)
        {
            RedirectToUINotAvailable();
        }

        // Init document manager events
        DocumentManager.OnSaveData += DocumentManager_OnSaveData;
        DocumentManager.OnAfterAction += DocumentManager_OnAfterAction;

        EnableSplitMode = true;

        // Set user control properties
        usrOwner = Page.LoadUserControl("~/CMSModules/Membership/FormControls/Users/selectuser.ascx") as FormEngineUserControl;
        if (usrOwner != null)
        {
            usrOwner.ID = "ctrlUsrOwner";
            usrOwner.IsLiveSite = false;
            usrOwner.SetValue("ShowSiteFilter", false);
            usrOwner.StopProcessing = pnlUIOwner.IsHidden;
            plcUsrOwner.Controls.Add(usrOwner);
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        SetPropertyTab(TAB_GENERAL);

        // Register the scripts
        ScriptHelper.RegisterLoader(Page);
        ScriptHelper.RegisterTooltip(Page);
        ScriptHelper.RegisterDialogScript(this);
        
        btnEditableContent.OnClientClick = "ShowEditableContent(); return false;";
        btnMessageBoards.OnClientClick = "ShowMessageBoards(); return false;";
        btnForums.OnClientClick = "ShowForums(); return false;";

        // Get strings for radio buttons
        lblCacheMinutes.Text = GetString("GeneralProperties.cacheMinutes");
        
        // Get strings for labels
        lblNameTitle.Text = GetString("GeneralProperties.Name");
        lblNamePathTitle.Text = GetString("GeneralProperties.NamePath");
        lblAliasPathTitle.Text = GetString("GeneralProperties.AliasPath");
        lblTypeTitle.Text = GetString("GeneralProperties.Type");
        lblNodeIDTitle.Text = GetString("GeneralProperties.NodeID");
        lblLastModifiedByTitle.Text = GetString("GeneralProperties.LastModifiedBy");
        lblLastModifiedTitle.Text = GetString("GeneralProperties.LastModified");
        lblLiveURLTitle.Text = GetString("GeneralProperties.LiveURL");
        lblPreviewURLTitle.Text = GetString("GeneralProperties.PreviewURL");
        lblGUIDTitle.Text = GetString("GeneralProperties.GUID");
        lblDocGUIDTitle.Text = GetString("GeneralProperties.DocumentGUID");
        lblDocIDTitle.Text = GetString("GeneralProperties.DocumentID");
        lblCultureTitle.Text = GetString("GeneralProperties.Culture");
        lblCreatedByTitle.Text = GetString("GeneralProperties.CreatedBy");
        lblCreatedTitle.Text = GetString("GeneralProperties.Created");
        lblOwnerTitle.Text = GetString("GeneralProperties.Owner");
        lblCssStyle.Text = GetString("PageProperties.CssStyle");
        lblPublishedTitle.Text = GetString("PageProperties.Published");

        // Set default item value
        ctrlSiteSelectStyleSheet.AddDefaultRecord = false;
        ctrlSiteSelectStyleSheet.CurrentSelector.SpecialFields.AllowDuplicates = true;
        ctrlSiteSelectStyleSheet.CurrentSelector.SpecialFields.Add(new SpecialField { Text = GetString("general.defaultchoice"), Value = GetDefaultStylesheet() });

        ctrlSiteSelectStyleSheet.ReturnColumnName = "StyleSheetID";
        ctrlSiteSelectStyleSheet.SiteId = SiteContext.CurrentSiteID;

        if ((SiteContext.CurrentSite != null) && (usrOwner != null))
        {
            usrOwner.SetValue("SiteID", SiteContext.CurrentSite.SiteID);
        }

        int documentId = 0;

        StringBuilder script = new StringBuilder();

        TreeNode node = Node;
        if (node != null)
        {
            if (PortalContext.CurrentSiteStylesheet != null)
            {
                script.Append(@"
var currentStyleSheetId;
// Function raised before opening the Edit dialog of the CSS style sheet control. When 'default' style sheet is chosen, translate this value to the default site style sheet id.
function US_GetEditedItemId_", ctrlSiteSelectStyleSheet.ValueElementID, @"(selectedValue) {
    currentStyleSheetId = selectedValue;
    if (selectedValue == ""default"") {
        return ", PortalContext.CurrentSiteStylesheet.StylesheetID, @";
    }

    return selectedValue;
}

// Function raised from New/Edit dialog after save action. When 'default' style is used, the new/edit dialog will try to choose a real style sheet id (which was edited), but it is necessary keep the selected value to be 'default'.
function US_GetNewItemId_", ctrlSiteSelectStyleSheet.ValueElementID, @"(newStyleSheetId) {
    if ((currentStyleSheetId == ""default"") && (newStyleSheetId == ", PortalContext.CurrentSiteStylesheet.StylesheetID, @")) {
        return currentStyleSheetId;
    }

    return newStyleSheetId;
}
"
                );
            }

            // Create wireframe option
            if (node.NodeWireframeTemplateID <= 0)
            {
                btnWireframe.ResourceString = "Wireframe.Create";

                string createUrl = URLHelper.ResolveUrl(String.Format("~/CMSModules/Content/CMSDesk/Properties/CreateWireframe.aspx?nodeid={0}&culture={1}", node.NodeID, node.DocumentCulture));
                btnWireframe.OnClientClick = "parent.location.replace('" + createUrl + "'); return false;";
            }
            else
            {
                btnWireframe.ResourceString = "Wireframe.Remove";
                btnWireframe.OnClientClick = "return confirm(" + ScriptHelper.GetLocalizedString("Wireframe.ConfirmRemove") + ")";
                btnWireframe.Click += lnkWireframe_Click;
            }

            plcWireframe.Visible = PortalHelper.IsWireframingEnabled(SiteContext.CurrentSiteName);

            documentId = node.DocumentID;
            canEditOwner = (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.ModifyPermissions) == AuthorizationResultEnum.Allowed);
            ctrlSiteSelectStyleSheet.AliasPath = node.NodeAliasPath;

            ReloadData();

            // Check ad-hoc forum counts
            hasAdHocForum = (ModuleCommands.ForumsGetDocumentForumsCount(node.DocumentID) > 0);

            // Ad-Hoc message boards check
            hasAdHocBoard = (ModuleCommands.MessageBoardGetDocumentBoardsCount(node.DocumentID) > 0);

            script.Append("function ShowEditableContent() { modalDialog('", ResolveUrl("Advanced/EditableContent/default.aspx"), "?nodeid=", node.NodeID, "', 'EditableContent', '95%', '95%'); } \n");
        }

        // Generate executive script
        if (hasAdHocBoard)
        {
            plcAdHocBoards.Visible = true;
            script.Append("function ShowMessageBoards() { modalDialog('", ResolveUrl("~/CMSModules/MessageBoards/Content/Properties/default.aspx"), "?documentid=", documentId, "', 'MessageBoards', '95%', '95%'); } \n");
        }

        if (hasAdHocForum)
        {
            plcAdHocForums.Visible = true;
            script.Append("function ShowForums() { modalDialog('", ResolveUrl("~/CMSModules/Forums/Content/Properties/default.aspx"), "?documentid=", documentId, "', 'Forums', '95%', '95%'); } \n");
        }

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ModalDialogsToAdvancedSection", script.ToString(), true);

        // Reflect processing action
        pnlContent.Enabled = DocumentManager.AllowSave;

        if (chkCssStyle.Checked && (PortalContext.CurrentSiteStylesheet != null))
        {
            // Enable the edit button
            ctrlSiteSelectStyleSheet.ButtonEditEnabled = true;
        }
    }


    protected void lnkWireframe_Click(object sender, EventArgs e)
    {
        // Remove the wireframe
        TreeNode node = DocumentManager.Node;

        DocumentManager.RemoveWireframe();

        ScriptHelper.RegisterStartupScript(this, typeof(string), "Refresh", ScriptHelper.GetScript(String.Format(
            "parent.RefreshTree({0}, {0}); parent.SelectNode({0});",
            node.NodeID
        )));
    }


    /// <summary>
    /// PreRender event handler
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (RequestHelper.IsPostBack())
        {
            ReloadData();
        }
        chkPageVisitInherit.Visible = (Node != null) && (Node.NodeParentID > 0);
    }


    protected void chkPageVisitInherit_CheckedChanged(object sender, EventArgs e)
    {
        chkLogPageVisit.Enabled = !chkPageVisitInherit.Checked;
        if (chkPageVisitInherit.Checked && (Node != null))
        {
            string siteName = SiteContext.CurrentSiteName;
            if (!String.IsNullOrEmpty(siteName))
            {
                chkLogPageVisit.Checked = ValidationHelper.GetBoolean(Node.GetInheritedValue("DocumentLogVisitActivity", false), false);
            }
        }
    }

    #endregion


    #region "Private methods"

    private void ReloadData()
    {
        if (Node != null)
        {
            // Log activities checkboxes
            if (!RequestHelper.IsPostBack())
            {
                bool? logVisit = Node.DocumentLogVisitActivity;
                chkLogPageVisit.Checked = (logVisit == true);
                if (Node.NodeParentID > 0)  // Init "inherit" option for child nodes (and hide option for root)
                {
                    chkPageVisitInherit.Checked = (logVisit == null);
                    if (logVisit == null)
                    {
                        chkPageVisitInherit_CheckedChanged(null, EventArgs.Empty);
                    }
                }
                chkLogPageVisit.Enabled = !chkPageVisitInherit.Checked;
            }

            // Check modify permission
            canEdit = (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) != AuthorizationResultEnum.Denied);

            // Show document group owner selector
            if (ModuleEntryManager.IsModuleLoaded(ModuleName.COMMUNITY) && canEditOwner && LicenseHelper.CheckFeature(RequestContext.CurrentDomain, FeatureEnum.Groups))
            {
                plcOwnerGroup.Controls.Clear();
                // Initialize panel content
                Panel rowWrapperPanel = new Panel();
                rowWrapperPanel.CssClass = "form-group";
                Panel lblPanel = new Panel();
                lblPanel.CssClass = "editing-form-label-cell";
                Panel ctrlPanel = new Panel();
                ctrlPanel.CssClass = "editing-form-value-cell";

                // Initialize caption
                LocalizedLabel lblOwnerGroup = new LocalizedLabel();
                lblOwnerGroup.EnableViewState = false;
                lblOwnerGroup.ResourceString = "community.group.documentowner";
                lblOwnerGroup.ID = "lblOwnerGroup";
                lblOwnerGroup.CssClass = "control-label";
                lblPanel.Controls.Add(lblOwnerGroup);

                // Initialize selector
                fcDocumentGroupSelector = (FormEngineUserControl)Page.LoadUserControl("~/CMSAdminControls/UI/Selectors/DocumentGroupSelector.ascx");
                fcDocumentGroupSelector.ID = "fcDocumentGroupSelector";
                fcDocumentGroupSelector.StopProcessing = pnlUIOwner.IsHidden;
                ctrlPanel.Controls.Add(fcDocumentGroupSelector);
                fcDocumentGroupSelector.Value = ValidationHelper.GetInteger(Node.GetValue("NodeGroupID"), 0);
                fcDocumentGroupSelector.SetValue("siteid", SiteContext.CurrentSiteID);
                fcDocumentGroupSelector.SetValue("nodeid", Node.NodeID);

                // Add controls to containers
                rowWrapperPanel.Controls.Add(lblPanel);
                rowWrapperPanel.Controls.Add(ctrlPanel);
                plcOwnerGroup.Controls.Add(rowWrapperPanel);
                plcOwnerGroup.Visible = true;
            }

            // Show owner editing only when authorized to change the permissions
            if (canEditOwner)
            {
                lblOwner.Visible = false;
                usrOwner.Visible = true;
                usrOwner.SetValue("AdditionalUsers", new[] { Node.NodeOwner });
            }
            else
            {
                usrOwner.Visible = false;
            }

            if (!RequestHelper.IsPostBack())
            {
                if (canEditOwner)
                {
                    usrOwner.Value = Node.GetValue("NodeOwner");
                }
            }

            // Load the data
            lblName.Text = HttpUtility.HtmlEncode(Node.GetDocumentName());
            lblNamePath.Text = HttpUtility.HtmlEncode(Convert.ToString(Node.GetValue("DocumentNamePath")));
            lblAliasPath.Text = Convert.ToString(Node.NodeAliasPath);
            string typeName = DataClassInfoProvider.GetDataClassInfo(Node.NodeClassName).ClassDisplayName;
            lblType.Text = HttpUtility.HtmlEncode(ResHelper.LocalizeString(typeName));
            lblNodeID.Text = Convert.ToString(Node.NodeID);

            // Modifier
            SetUserLabel(lblLastModifiedBy, "DocumentModifiedByUserId");

            // Get modified time
            TimeZoneInfo usedTimeZone;
            DateTime lastModified = ValidationHelper.GetDateTime(Node.GetValue("DocumentModifiedWhen"), DateTimeHelper.ZERO_TIME);
            lblLastModified.Text = TimeZoneHelper.GetCurrentTimeZoneDateTimeString(lastModified, MembershipContext.AuthenticatedUser, SiteContext.CurrentSite, out usedTimeZone);
            ScriptHelper.AppendTooltip(lblLastModified, TimeZoneHelper.GetUTCLongStringOffset(usedTimeZone), "help");

            if (!canEditOwner)
            {
                // Owner
                SetUserLabel(lblOwner, "NodeOwner");
            }

            // Creator
            SetUserLabel(lblCreatedBy, "DocumentCreatedByUserId");
            DateTime createdWhen = ValidationHelper.GetDateTime(Node.GetValue("DocumentCreatedWhen"), DateTimeHelper.ZERO_TIME);
            lblCreated.Text = TimeZoneHelper.GetCurrentTimeZoneDateTimeString(createdWhen, MembershipContext.AuthenticatedUser, SiteContext.CurrentSite, out usedTimeZone);
            ScriptHelper.AppendTooltip(lblCreated, TimeZoneHelper.GetUTCLongStringOffset(usedTimeZone), "help");


            // URL
            string liveUrl = Node.IsLink ? DocumentURLProvider.GetUrl(Node.NodeAliasPath) : DocumentURLProvider.GetUrl(Node.NodeAliasPath, Node.DocumentUrlPath);
            lnkLiveURL.Text = URLHelper.ResolveUrl(liveUrl);
            lnkLiveURL.NavigateUrl = URLHelper.ResolveUrl(liveUrl);

            string permanentUrl = DocumentURLProvider.GetPermanentDocUrl(Node.NodeGUID, Node.NodeAlias, Node.NodeSiteName, PageInfoProvider.PREFIX_CMS_GETDOC, ".aspx");
            lnkPermanentUrl.Text = URLHelper.ResolveUrl(permanentUrl);
            lnkPermanentUrl.NavigateUrl = URLHelper.ResolveUrl(permanentUrl);

            bool isRoot = (Node.NodeClassName.ToLowerCSafe() == "cms.root");

            // Preview URL
            if (!isRoot)
            {
                plcPreview.Visible = true;
                btnResetPreviewGuid.ToolTip = GetString("GeneralProperties.InvalidatePreviewURL");
                btnResetPreviewGuid.Click += btnResetPreviewGuid_Click;
                btnResetPreviewGuid.OnClientClick = "if(!confirm(" + ScriptHelper.GetLocalizedString("GeneralProperties.GeneratePreviewURLConf") + ")){return false;}";

                InitPreviewUrl();
            }

            lblGUID.Text = Convert.ToString(Node.NodeGUID);
            lblDocGUID.Text = (Node.DocumentGUID == Guid.Empty) ? ResHelper.Dash : Node.DocumentGUID.ToString();
            lblDocID.Text = Convert.ToString(Node.DocumentID);

            // Culture
            CultureInfo ci = CultureInfoProvider.GetCultureInfo(Node.DocumentCulture);
            lblCulture.Text = ((ci != null) ? ResHelper.LocalizeString(ci.CultureName) : Node.DocumentCulture);

            lblPublished.Text = (Node.IsPublished ? "<span class=\"DocumentPublishedYes\">" + GetString("General.Yes") + "</span>" : "<span class=\"DocumentPublishedNo\">" + GetString("General.No") + "</span>");

            // Load page info for inherited cache settings
            currentPage = PageInfoProvider.GetPageInfo(Node.DocumentGUID, Node.NodeSiteID);

            if (!RequestHelper.IsPostBack())
            {
                // Init radio buttons for cache settings
                if (isRoot)
                {
                    radInherit.Visible = false;
                    radFSInherit.Visible = false;
                    chkCssStyle.Visible = false;
                }
                else
                {
                    // Show what is inherited value
                    radInherit.Text = GetString("GeneralProperties.radInherit") + " (" + GetInheritedCacheCaption("NodeCacheMinutes") + ")";
                    radFSInherit.Text = GetString("GeneralProperties.radInherit") + " (" + GetInheritedCacheCaption("NodeAllowCacheInFileSystem") + ")";
                }

                string cacheMinutes = "";

                switch (Node.NodeCacheMinutes)
                {
                    case -1:
                        // Cache setting is inherited
                        {
                            radNo.Checked = true;
                            radYes.Checked = false;
                            radInherit.Checked = false;
                            if (!isRoot)
                            {
                                radInherit.Checked = true;
                                radNo.Checked = false;

                                if ((currentPage != null) && (currentPage.NodeCacheMinutes > 0))
                                {
                                    cacheMinutes = currentPage.NodeCacheMinutes.ToString();
                                }
                            }
                        }
                        break;

                    case 0:
                        // Cache is off
                        radNo.Checked = true;
                        radYes.Checked = false;
                        radInherit.Checked = false;
                        break;

                    default:
                        // Cache is enabled
                        radNo.Checked = false;
                        radYes.Checked = true;
                        radInherit.Checked = false;
                        cacheMinutes = Node.NodeCacheMinutes.ToString();
                        break;
                }

                // Set secured radio buttons
                switch (Node.NodeAllowCacheInFileSystem)
                {
                    case 0:
                        radFSNo.Checked = true;
                        break;

                    case 1:
                        radFSYes.Checked = true;
                        break;

                    default:
                        if (!isRoot)
                        {
                            radFSInherit.Checked = true;
                        }
                        else
                        {
                            radFSYes.Checked = true;
                        }
                        break;
                }

                txtCacheMinutes.Text = cacheMinutes;

                if (!radYes.Checked)
                {
                    txtCacheMinutes.Enabled = false;
                }

                if (Node.GetValue("DocumentStylesheetID") == null)
                {
                    ctrlSiteSelectStyleSheet.Value = GetDefaultStylesheet();
                }
                else
                {
                    // If stylesheet is inherited from parent document
                    if (ValidationHelper.GetInteger(Node.GetValue("DocumentStylesheetID"), 0) == -1)
                    {
                        if (!isRoot)
                        {
                            chkCssStyle.Checked = true;

                            // Get parent stylesheet
                            string value = GetParentProperty();
                            ctrlSiteSelectStyleSheet.Value = String.IsNullOrEmpty(value) ? GetDefaultStylesheet() : value;
                        }
                    }
                    else
                    {
                        ctrlSiteSelectStyleSheet.Value = Node.GetValue("DocumentStylesheetID");
                    }
                }
            }

            // Disable new button if document inherit stylesheet
            bool disableCssSelector = (!isRoot && chkCssStyle.Checked);
            ctrlSiteSelectStyleSheet.Enabled = !disableCssSelector;
            ctrlSiteSelectStyleSheet.ButtonNewEnabled = !disableCssSelector;

            // Initialize Rating control
            RefreshCntRatingResult();

            double rating = 0.0f;
            if (Node.DocumentRatings > 0)
            {
                rating = Node.DocumentRatingValue / Node.DocumentRatings;
            }
            ratingControl.MaxRating = 10;
            ratingControl.CurrentRating = rating;
            ratingControl.Visible = true;
            ratingControl.Enabled = false;

            // Initialize Reset button for rating
            btnResetRating.Text = GetString("general.reset");
            btnResetRating.OnClientClick = "if (!confirm(" + ScriptHelper.GetString(GetString("GeneralProperties.ResetRatingConfirmation")) + ")) return false;";

            object[] param = new object[1];
            param[0] = Node.DocumentID;

            plcAdHocForums.Visible = hasAdHocForum;
            plcAdHocBoards.Visible = hasAdHocBoard;

            if (!canEdit)
            {
                // Disable form editing                                                            
                DisableFormEditing();
            }
        }
        else
        {
            btnResetRating.Visible = false;
        }
    }


    /// <summary>
    /// Gets inherited caption for security settings.
    /// </summary>
    /// <param name="columnName">Column name of inherited value</param>
    private string GetInheritedCacheCaption(string columnName)
    {
        // Convert to bool and check result
        return ValidationHelper.GetBoolean(Node.GetInheritedValue(columnName), false) ? GetString("General.Yes") : GetString("General.No");

    }


    /// <summary>
    /// Initializes the label with specified user text.
    /// </summary>
    private void SetUserLabel(Label label, string columnName)
    {
        // Get the user ID
        int userId = ValidationHelper.GetInteger(Node.GetValue(columnName), 0);
        if (userId > 0)
        {
            // Get the user object
            UserInfo ui = UserInfoProvider.GetUserInfo(userId);
            if (ui != null)
            {
                label.Text = HTMLHelper.HTMLEncode(ui.FullName);
            }
        }
        else
        {
            label.Text = GetString("general.selectnone");
        }
    }


    protected void btnClear_Click(object sender, EventArgs e)
    {
        if (Node != null)
        {
            // Check modify permissions
            if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
            {
                return;
            }

            // Clear the output cache with the children
            Node.ClearOutputCache(true, true);

            ShowConfirmation(GetString("GeneralProperties.CacheCleared"));
        }
    }


    private void DocumentManager_OnAfterAction(object sender, DocumentManagerEventArgs e)
    {
        // Clear cache if cache settings changed
        if (clearCache)
        {
            Node.ClearOutputCache(true, true);
        }
    }


    private void DocumentManager_OnSaveData(object sender, DocumentManagerEventArgs e)
    {
        TreeNode node = e.Node;

        // OWNER group is displayed by UI profile
        if (!pnlUIOwner.IsHidden)
        {
            // Set owner
            int ownerId = ValidationHelper.GetInteger(usrOwner.Value, 0);
            node.SetValue("NodeOwner", (ownerId > 0) ? usrOwner.Value : null);
        }


        // DESIGN group is displayed by UI profile
        if (!pnlUIDesign.IsHidden)
        {
            node.SetValue("DocumentStylesheetID", -1);
            if (!chkCssStyle.Checked)
            {
                // Set style sheet
                int selectedCssId = ValidationHelper.GetInteger(ctrlSiteSelectStyleSheet.Value, 0);
                if (selectedCssId < 1)
                {
                    node.SetValue("DocumentStylesheetID", null);
                }
                else
                {
                    node.SetValue("DocumentStylesheetID", selectedCssId);
                }

                ctrlSiteSelectStyleSheet.Enabled = true;
            }
            else
            {
                ctrlSiteSelectStyleSheet.Enabled = false;
            }
        }

        // CACHE group is displayed by UI profile
        if (!pnlUICache.IsHidden)
        {
            // Cache minutes
            int cacheMinutes = 0;
            if (radNo.Checked)
            {
                cacheMinutes = 0;
                txtCacheMinutes.Text = "";
            }
            else if (radYes.Checked)
            {
                cacheMinutes = ValidationHelper.GetInteger(txtCacheMinutes.Text, -5);
                if (cacheMinutes <= 0)
                {
                    e.IsValid = false;
                }
            }
            else if (radInherit.Checked)
            {
                cacheMinutes = currentPage.NodeCacheMinutes = -1;
                if ((currentPage != null) && (currentPage.NodeCacheMinutes > 0))
                {
                    txtCacheMinutes.Text = currentPage.NodeCacheMinutes.ToString();
                }
            }

            // Set cache minutes                
            if (cacheMinutes != node.NodeCacheMinutes)
            {
                node.NodeCacheMinutes = cacheMinutes;
                clearCache = true;
            }

            // Allow file system cache
            int allowFs = Node.NodeAllowCacheInFileSystem;

            if (radFSYes.Checked)
            {
                allowFs = 1;
            }
            else if (radFSNo.Checked)
            {
                allowFs = 0;
            }
            else if (radInherit.Checked)
            {
                allowFs = -1;
            }

            Node.NodeAllowCacheInFileSystem = allowFs;
        }

        if (e.IsValid)
        {
            // Check UI element permission
            if (MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", "General.OnlineMarketing"))
            {
                node.DocumentLogVisitActivity = (chkPageVisitInherit.Checked ? (bool?)null : chkLogPageVisit.Checked);
            }
        }
        else
        {
            // Show error message
            e.ErrorMessage = GetString("GeneralProperties.BadCacheMinutes");
        }
    }


    protected void btnResetPreviewGuid_Click(object sender, EventArgs e)
    {
        if (Node != null)
        {
            // Check modify permissions
            if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
            {
                return;
            }

            using (new CMSActionContext { LogEvents = false })
            {
                Node.DocumentWorkflowCycleGUID = Guid.NewGuid();
                Node.Update();
            }

            ShowConfirmation(ResHelper.GetString("GeneralProperties.PreviewLinkGenerated"));
            InitPreviewUrl();
        }
    }


    /// <summary>
    /// Disables form editing.
    /// </summary>
    protected void DisableFormEditing()
    {
        // Disable all panels
        pnlDesign.Enabled = false;
        pnlCache.Enabled = false;
        pnlOwner.Enabled = false;
        pnlOnlineMarketing.Enabled = false;

        // Disable 'save button'
        menuElem.Enabled = false;

        // Disable rating and owner selector
        btnResetPreviewGuid.Enabled = false;
        btnResetPreviewGuid.CssClass = "Disabled";
        btnResetRating.Enabled = false;
        btnClear.Enabled = false;
        usrOwner.Enabled = false;
        if (fcDocumentGroupSelector != null)
        {
            fcDocumentGroupSelector.Enabled = false;
        }

        ctrlSiteSelectStyleSheet.Enabled = false;
        ctrlSiteSelectStyleSheet.ButtonNewEnabled = false;
    }


    protected void radInherit_CheckedChanged(object sender, EventArgs e)
    {
        txtCacheMinutes.Enabled = false;

        // Enable textbox for cache minutes
        if (radYes.Checked)
        {
            txtCacheMinutes.Enabled = true;
            txtCacheMinutes.Text = Node.NodeCacheMinutes > 0 ? Node.NodeCacheMinutes.ToString() : String.Empty;
        }

        if (radNo.Checked)
        {
            txtCacheMinutes.Text = String.Empty;
        }

        if (radInherit.Checked && (currentPage != null))
        {
            // Raise parent cache settings search
            currentPage.NodeCacheMinutes = -1;
            txtCacheMinutes.Text = (currentPage.NodeCacheMinutes > 0) ? currentPage.NodeCacheMinutes.ToString() : String.Empty;
        }
    }


    protected void chkCssStyle_CheckedChanged(object sender, EventArgs e)
    {
        if (chkCssStyle.Checked)
        {
            // Set stylesheet to stylesheet selector
            ctrlSiteSelectStyleSheet.Enabled = false;
            ctrlSiteSelectStyleSheet.ButtonNewEnabled = false;

            string value = GetParentProperty(); 
            if (String.IsNullOrEmpty(value))
            {
                ctrlSiteSelectStyleSheet.Value = GetDefaultStylesheet();
            }
            else
            {
                try
                {
                    ctrlSiteSelectStyleSheet.Value = value;
                }
                catch
                {
                }
            }
        }
        else
        {
            ctrlSiteSelectStyleSheet.Enabled = true;
            ctrlSiteSelectStyleSheet.ButtonNewEnabled = true;
        }
    }


    /// <summary>
    /// Refreshes current rating result.
    /// </summary>
    protected void RefreshCntRatingResult()
    {
        string msg = null;

        // Avoid division by zero
        if ((Node != null) && (Node.DocumentRatings > 0))
        {
            msg = String.Format(GetString("GeneralProperties.ContentRatingResult"), (Node.DocumentRatingValue * 10) / Node.DocumentRatings, Node.DocumentRatings);
        }

        // Document wasn't rated
        if (msg == null)
        {
            msg = GetString("generalproperties.contentratingnoresult");
        }

        lblContentRatingResult.Text = msg;
    }


    /// <summary>
    /// Resets content rating score.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Args</param>
    protected void btnResetRating_Click(object sender, EventArgs e)
    {
        if (Node != null)
        {
            // Check modify permissions
            if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
            {
                return;
            }

            // Reset rating
            TreeProvider.ResetRating(Node);
            RefreshCntRatingResult();
            ratingControl.CurrentRating = 0.0;
            ratingControl.ReloadData();

            ShowChangesSaved();
        }
    }


    private void InitPreviewUrl()
    {
        if (Node.DocumentWorkflowCycleGUID != Guid.Empty)
        {
            lnkPreviewURL.Visible = true;
            lblNoPreviewGuid.Visible = false;
            lnkPreviewURL.Text = ResHelper.GetString("GeneralProperties.ShowPreview");
            bool isFile = CMSString.Equals(Node.NodeClassName, "cms.file", true);
            lnkPreviewURL.NavigateUrl = Node.GetPreviewLink(CurrentUser.UserName, isFile);
        }
        else
        {
            lnkPreviewURL.Visible = false;
            lblNoPreviewGuid.Visible = true;
            lblNoPreviewGuid.Text = GetString("GeneralProperties.NoPreviewGuid");
        }
    }


    /// <summary>
    /// Gets the default style sheet for the current site.
    /// </summary>
    private string GetDefaultStylesheet()
    {
        string value = "-1";

        // If default site not exist edit is set to -1 - disabled
        if (PortalContext.CurrentSiteStylesheet != null)
        {
            value = "default";
        }

        return value;
    }


    /// <summary>
    /// Gets value from parent node
    /// </summary>
    private string GetParentProperty()
    {
        var where = new WhereCondition()
            .WhereNotEquals("DocumentStylesheetID", -1)
            .Or()
            .WhereNull("DocumentStylesheetID");

        return PageInfoProvider.GetParentProperty<string>(Node.NodeSiteID, Node.NodeAliasPath, "DocumentStylesheetID", Node.DocumentCulture, where);
    }

    #endregion
}