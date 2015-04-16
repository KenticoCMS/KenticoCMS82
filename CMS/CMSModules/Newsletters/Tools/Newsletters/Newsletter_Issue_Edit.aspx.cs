using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.UIControls;
using CMS.ExtendedControls;

// Set edited object
[EditedObject(IssueInfo.OBJECT_TYPE, "objectid")]
[UIElement(ModuleName.NEWSLETTER, "Newsletter.Issue.Content")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_Edit : CMSNewsletterPage
{
    #region "Variables"

    private NewsletterInfo newsletter;
    private const string mAttachmentsActionClass = "attachments-header-action";

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Set update mode to ALWAYS
        CurrentMaster.HeaderActions.UpdatePanel.UpdateMode = UpdatePanelUpdateMode.Always;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get edited issue object and check its existence
        IssueInfo issue = EditedObject as IssueInfo;

        if (issue == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        if (!issue.CheckPermissions(PermissionsEnum.Modify, CurrentSiteName, CurrentUser))
        {
            RedirectToAccessDenied(issue.TypeInfo.ModuleName, "AuthorIssues");
        }

        // Reset master page content CSS class
        CurrentMaster.PanelContent.CssClass = string.Empty;

        // Ensure correct padding
        CurrentMaster.MessagesPlaceHolder.OffsetX = 16;
        CurrentMaster.MessagesPlaceHolder.OffsetY = 8;

        // Get newsletter and check its existence
        newsletter = NewsletterInfoProvider.GetNewsletterInfo(issue.IssueNewsletterID);
        if ((newsletter != null) && (newsletter.NewsletterType == NewsletterType.Dynamic))
        {
            ShowError(GetString("Newsletter_Issue_Edit.CannotBeEdited"));
            editElem.StopProcessing = true;
            editElem.Visible = false;
            return;
        }

        // Get variant issue ID if A/B testing is ON
        int issueId = InitVariantSlider(issue.IssueID);

        // Initialize edit control
        editElem.IssueID = issueId;
        editElem.NewsletterID = issue.IssueNewsletterID;
        editElem.IsDialogMode = false;

        InitHeaderActions(issueId);
    }


    /// <summary>
    /// Updates user friendly info message according to current issue.
    /// </summary>
    /// <param name="issueId">Issue ID</param>
    /// <param name="editingIssueEnabled">TRUE if editing issue is allowed</param>
    /// <param name="variantSliderEnabled">TRUE if modifying variants is allowed</param>
    private void UpdateDialog(int issueId, out bool editingIssueEnabled, out bool variantSliderEnabled)
    {
        editingIssueEnabled = variantSliderEnabled = false;

        // Get issue
        IssueInfo issue = IssueInfoProvider.GetIssueInfo(issueId);

        if (issue != null)
        {
            switch (issue.IssueStatus)
            {
                case IssueStatusEnum.Idle:
                case IssueStatusEnum.ReadyForSending:
                    editingIssueEnabled = true;
                    // Enable variant slider only if parent issue is idle
                    if (issue.IssueIsABTest)
                    {
                        if (issue.IssueIsVariant)
                        {
                            IssueInfo parentIssue = IssueInfoProvider.GetIssueInfo(issue.IssueVariantOfIssueID);
                            variantSliderEnabled = (parentIssue != null) && (parentIssue.IssueStatus == IssueStatusEnum.Idle);
                        }
                    }
                    break;
                case IssueStatusEnum.PreparingData:
                case IssueStatusEnum.TestPhase:
                case IssueStatusEnum.Sending:
                    break;
                case IssueStatusEnum.Finished:
                    if (!issue.IssueIsABTest && (newsletter != null))
                    {
                        editingIssueEnabled = newsletter.NewsletterEnableResending;
                    }
                    break;
            }
        }
    }


    /// <summary>
    /// Initializes header action control.
    /// </summary>
    /// <param name="issueId">Issue ID</param>
    private void InitHeaderActions(int issueId)
    {
        // Show info message and get current issue state
        bool editingEnabled;
        bool variantSliderEnabled;
        UpdateDialog(issueId, out editingEnabled, out variantSliderEnabled);
        editElem.Enabled = editingEnabled;

        bool isIssueVariant = ucVariantSlider.Variants.Count > 0;

        ucVariantSlider.Visible = isIssueVariant;
        ucVariantSlider.EditingEnabled = editingEnabled && variantSliderEnabled;

        ScriptHelper.RegisterDialogScript(Page);

        CurrentMaster.HeaderActions.ActionsList.Clear();

        // Init save button
        CurrentMaster.HeaderActions.ActionsList.Add(new SaveAction(this)
        {
            OnClientClick = "if (GetContent != null) {return GetContent();} else {return false;}",
            Enabled = editingEnabled
        });

        // Ensure spell check action
        if (editingEnabled)
        {
            CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction
            {
                Text = GetString("EditMenu.IconSpellCheck"),
                Tooltip = GetString("EditMenu.SpellCheck"),
                OnClientClick = "var frame = GetFrame(); if ((frame != null) && (frame.contentWindow.SpellCheck_" + ClientID + " != null)) {frame.contentWindow.SpellCheck_" + ClientID + "();} return false;",
                ButtonStyle = ButtonStyle.Default,
            });
        }

        // Init send draft button
        CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("newsletterissue_content.senddraft"),
            Tooltip = GetString("newsletterissue_content.senddraft"),
            OnClientClick = string.Format(@"if (modalDialog) {{modalDialog('{0}?objectid={1}', 'SendDraft', '700', '300');}}", ResolveUrl(@"~/CMSModules/Newsletters/Tools/Newsletters/Newsletter_Issue_SendDraft.aspx"), issueId) + " return false;",
            Enabled = true,
            ButtonStyle = ButtonStyle.Default,
        });

        // Init preview button
        CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("general.preview"),
            Tooltip = GetString("general.preview"),
            OnClientClick = string.Format(@"if (modalDialog) {{modalDialog('{0}?objectid={1}', 'Preview', '90%', '90%');}}", ResolveUrl(@"~/CMSModules/Newsletters/Tools/Newsletters/Newsletter_Issue_Preview.aspx"), issueId) + " return false;",
            ButtonStyle = ButtonStyle.Default,
        });

        int attachCount = 0;
        // Get number of attachments
        InfoDataSet<MetaFileInfo> ds = MetaFileInfoProvider.GetMetaFiles(issueId,
            (isIssueVariant ? IssueInfo.OBJECT_TYPE_VARIANT : IssueInfo.OBJECT_TYPE), ObjectAttachmentsCategories.ISSUE, null, null, "MetafileID", -1);
        attachCount = ds.Items.Count;

        // Register attachments count update module
        ScriptHelper.RegisterModule(this, "CMS/AttachmentsCountUpdater", new { Selector = "." + mAttachmentsActionClass, Text = ResHelper.GetString("general.attachments") });
        

        // Prepare metafile dialog URL
        string metaFileDialogUrl = ResolveUrl(@"~/CMSModules/AdminControls/Controls/MetaFiles/MetaFileDialog.aspx");
        string query = string.Format("?objectid={0}&objecttype={1}", issueId, (isIssueVariant? IssueInfo.OBJECT_TYPE_VARIANT : IssueInfo.OBJECT_TYPE));
        metaFileDialogUrl += string.Format("{0}&category={1}&hash={2}", query, ObjectAttachmentsCategories.ISSUE, QueryHelper.GetHash(query));

        // Init attachment button
        CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("general.attachments") + ((attachCount > 0) ? " (" + attachCount + ")" : string.Empty),
            Tooltip = GetString("general.attachments"),
            OnClientClick = string.Format(@"if (modalDialog) {{modalDialog('{0}', 'Attachments', '700', '500');}}", metaFileDialogUrl) + " return false;",
            Enabled = true,
            CssClass = mAttachmentsActionClass,
            ButtonStyle = ButtonStyle.Default,
        });

        // Init create A/B test button - online marketing, open email and click through tracking are required
        if (!isIssueVariant)
        {
            IssueInfo issue = (IssueInfo)EditedObject;
            if (editingEnabled && (issue.IssueStatus == IssueStatusEnum.Idle) && NewsletterHelper.IsABTestingAvailable())
            {
                // Check that trackings are enabled
                bool trackingsEnabled = (newsletter != null) && newsletter.NewsletterTrackOpenEmails && newsletter.NewsletterTrackClickedLinks;

                CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction
                    {
                        Text = GetString("newsletterissue_content.createabtest"),
                        Tooltip = trackingsEnabled ? GetString("newsletterissue_content.createabtest") : GetString("newsletterissue_content.abtesttooltip"),
                        OnClientClick = trackingsEnabled ? "ShowVariantDialog_" + ucVariantDialog.ClientID + "('addvariant', ''); return false;" : "return false;",
                        Enabled = trackingsEnabled,
                        ButtonStyle = ButtonStyle.Default,
                    });
                ucVariantDialog.IssueID = issueId;
                ucVariantDialog.OnAddVariant += ucVariantSlider_OnVariantAdded;
            }
        }

        // Init masterpage
        CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        CurrentMaster.DisplayControlsPanel = isIssueVariant;
        CurrentMaster.PanelContent.Attributes.Add("onmouseout", "if (RememberFocusedRegion) {RememberFocusedRegion();}");
    }


    /// <summary>
    /// Actions handler.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "save":
                // Check issue status before saving (saving variant issue is allowed only in during "idle" and "ready for sending")
                IssueInfo issue = IssueInfoProvider.GetIssueInfo(editElem.IssueID);
                if ((issue != null) && issue.IssueIsABTest) 
                {
                    if ((issue.IssueStatus != IssueStatusEnum.Idle) && (issue.IssueStatus != IssueStatusEnum.ReadyForSending))
                    {
                        CurrentMaster.MessagesPlaceHolder.UseRelativePlaceHolder = false;
                        ShowError(GetString("newsletterissue.notallowedtosaveissue"));
                        break;
                    }
                }

                if (editElem.Save())
                {
                    // Show save message
                    ShowChangesSaved();

                    // Update breadcrumbs
                    if (issue != null)
                    {
                        ScriptHelper.RefreshTabHeader(Page, issue.IssueSubject);
                    }
                }
                else if (!string.IsNullOrEmpty(editElem.ErrorMessage))
                {
                    CurrentMaster.MessagesPlaceHolder.UseRelativePlaceHolder = false;
                    ShowError(editElem.ErrorMessage);
                }

                break;
        }
    }

    #endregion


    #region "A/B test methods"

    /// <summary>
    /// Initializes variant slider. Returns selected variant issue ID.
    /// </summary>
    /// <param name="issueId">Issue ID (currently edited)</param>
    private int InitVariantSlider(int issueId)
    {
        // Initialize variant slider
        ucVariantSlider.IssueID = issueId;
        bool isIssueVariant = ucVariantSlider.Variants.Count > 0;
        if (isIssueVariant)
        {
            int variantIndex = ucVariantSlider.CurrentVariant;
            if (variantIndex >= ucVariantSlider.Variants.Count)
            {
                variantIndex = ucVariantSlider.Variants.Count - 1;
            }
            IssueABVariantItem issueVariant = ucVariantSlider.Variants[(variantIndex < 0 ? 0 : variantIndex)];

            ucVariantSlider.OnVariantDeleted -= ucVariantSlider_OnVariantDeleted;
            ucVariantSlider.OnVariantDeleted += ucVariantSlider_OnVariantDeleted;
            ucVariantSlider.OnVariantAdded -= ucVariantSlider_OnVariantAdded;
            ucVariantSlider.OnVariantAdded += ucVariantSlider_OnVariantAdded;
            ucVariantSlider.OnSelectionChanged -= ucVariantSlider_OnSelectionChanged;
            ucVariantSlider.OnSelectionChanged += ucVariantSlider_OnSelectionChanged;
            return issueVariant.IssueID;
        }
        return issueId;
    }


    /// <summary>
    /// Reloads newly added variant to edit control.
    /// </summary>
    protected void ucVariantSlider_OnVariantAdded(object sender, EventArgs e)
    {
        int issueId;
        if (sender == ucVariantDialog)
        {
            if (!(e is VariantEventArgs)) return;
            VariantEventArgs args = (VariantEventArgs)e;
            issueId = args.ID;
            InitVariantSlider(issueId);
            ucVariantSlider.SetVariant(issueId);
        }
        else
        {
            issueId = ucVariantSlider.IssueID;
        }

        editElem.IssueID = issueId;
        editElem.ReloadData(true);
        InitHeaderActions(issueId);
    }


    /// <summary>
    /// Additional actions after variant has been deleted.
    /// </summary>
    protected void ucVariantSlider_OnVariantDeleted(object sender, EventArgs e)
    {
        int issueId;
        if (ucVariantSlider.Variants.Count > 1)
        {
            IssueABVariantItem issueVariant = ucVariantSlider.Variants[0];
            issueId = issueVariant.IssueID;
        }
        else
        {
            issueId = ucVariantSlider.OriginalIssueID;
        }
        editElem.IssueID = issueId;
        editElem.ReloadData(true);
        InitVariantSlider(issueId);
        ucVariantSlider.SetVariant(issueId);
        InitHeaderActions(issueId);
    }


    protected void ucVariantSlider_OnSelectionChanged(object sender, EventArgs e)
    {
        int issueId = QueryHelper.GetInteger("objectid", 0);
        int variantIndex = ucVariantSlider.CurrentVariant;
        if (variantIndex >= 0 && variantIndex < ucVariantSlider.Variants.Count)
        {
            IssueABVariantItem item = ucVariantSlider.Variants[variantIndex];
            issueId = item.IssueID;
        }

        editElem.IssueID = issueId;
        editElem.ReloadData(true);
        editElem.UpdateContent();
    }

    #endregion
}