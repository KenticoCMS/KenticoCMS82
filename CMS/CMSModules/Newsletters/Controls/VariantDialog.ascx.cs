using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.UIControls;
using CMS.Newsletters;
using CMS.Base;
using CMS.EventLog;

using TaskInfo = CMS.Scheduler.TaskInfo;
using TaskInfoProvider = CMS.Scheduler.TaskInfoProvider;
using CMS.Synchronization;
using CMS.DataEngine;

public partial class CMSModules_Newsletters_Controls_VariantDialog : CMSUserControl, IPostBackEventHandler
{
    #region "Variables"

    private bool alertAdded;
    private List<IssueABVariantItem> mVariants;

    #endregion


    #region "Properties"

    /// <summary>
    /// List of variants
    /// </summary>
    public List<IssueABVariantItem> Variants
    {
        get
        {
            return mVariants;
        }
        set
        {
            mVariants = value;
            lstTemplate.Items.Clear();
            int i = 0;
            foreach (IssueABVariantItem variant in value)
            {
                lstTemplate.Items.Add(new ListItem(variant.IssueVariantName, i.ToString()));
                i++;
            }

            lstTemplate.SelectedIndex = 0;
        }
    }


    /// <summary>
    /// Gets or sets issue ID
    /// </summary>
    public int IssueID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets identifier of current dialog (in case of more dialogs).
    /// </summary>
    private string CurrentDialogID
    {
        get
        {
            return ValidationHelper.GetString(ViewState["CurrentDialogID"], string.Empty);
        }
        set
        {
            ViewState["CurrentDialogID"] = value;
        }
    }


    /// <summary>
    /// Gets or sets current modal dialog ID (in case of more popups).
    /// </summary>
    private string CurrentModalID
    {
        get
        {
            return ValidationHelper.GetString(ViewState["CurrentModalID"], string.Empty);
        }
        set
        {
            ViewState["CurrentModalID"] = value;
        }
    }


    /// <summary>
    /// Gets or sets flage that indicates whether control has been loaded.
    /// </summary>
    private bool ControlLoaded
    {
        get;
        set;
    }


    /// <summary>
    /// Gets current dialog control (in case of more dialogs).
    /// </summary>
    private Panel CurrentDialog
    {
        get
        {
            return (Panel)FindControl(CurrentDialogID);
        }
    }


    /// <summary>
    /// Gets current modal control (in case of more popups).
    /// </summary>
    private ModalPopupDialog CurrentModal
    {
        get
        {
            return (ModalPopupDialog)FindControl(CurrentModalID);
        }
    }


    /// <summary>
    /// If true, control does not process the data.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            ucTitle.StopProcessing = value;
            mdlVariants.StopProcessing = value;
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Occurs when existing variant should be deleted.
    /// </summary>
    public event EventHandler OnDeleteVariant;


    /// <summary>
    /// Occurs when new variant should be created.
    /// </summary>
    public event EventHandler OnAddVariant;


    /// <summary>
    /// Occurs when existing variant should be moodified.
    /// </summary>
    public event EventHandler OnUpdateVariant;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        SetupControl();
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            ScriptHelper.RegisterJQueryDialog(Page);

            // Register control scripts
            StringBuilder dialogScript = new StringBuilder();
            dialogScript.Append(@"
function ShowVariantDialog_", ClientID, @"(format, param)
{
    document.getElementById('", hdnParameter.ClientID, @"').value = param;",
    ControlsHelper.GetPostBackEventReference(this, "##PARAM##").Replace("'##PARAM##'", "format"), @";
}");

            dialogScript.Append(@"
function DisableList_", ClientID, @"(val) 
{
   ShowHideErr('", lblError.ClientID, @"', 0);
   ShowHideErr('", lblError2.ClientID, @"', 0);
   elem = document.getElementById('", lstTemplate.ClientID, @"');
   if (elem) { elem.disabled = val; }
}");

            dialogScript.Append(@"
function ShowHideErr(elemId, show) {
  errlabel = document.getElementById(elemId);
  if (errlabel) { if (show==1) { errlabel.style.display='inline'; } else { errlabel.style.display='none'; } }
}
function ValidateInput_", ClientID, @"(val)
{
   ShowHideErr('", lblError.ClientID, @"', 0);
   ShowHideErr('", lblError2.ClientID, @"', 0);
   elem = document.getElementById(val);
   if (elem.value.trim() == '') {
      ShowHideErr('", lblError.ClientID, @"', 1);
      return true; 
   }
   lb = document.getElementById('", lstTemplate.ClientID, @"');
   if (lb && lb.disabled != true) {
     hdn = document.getElementById('", hdnSelected.ClientID, @"');
     if (hdn) {
        hdn.value=lb.selectedIndex;
        if (hdn.value == '' || hdn.value < 0) {
          ShowHideErr('", lblError2.ClientID, @"', 1);
          return true;
        }
     }
   }
}");
            ScriptHelper.RegisterStartupScript(pnlUpdate, typeof(string), "variantDlgScript_" + ClientID, ScriptHelper.GetScript(dialogScript.ToString()));

            if (Visible && pnlUpdate.Visible)
            {
                if (RequestHelper.IsPostBack() && (CurrentModal != null))
                {
                    // Show popup after postback
                    CurrentModal.Show();
                }
            }
        }
    }

    #endregion


    #region "Button handling"

    /// <summary>
    /// When dialog's closing button is clicked.
    /// </summary>
    protected void btnClose_Click(object sender, EventArgs e)
    {
        // Hide current popup dialog
        HideCurrentPopup();
    }


    /// <summary>
    /// When dialog's OK button for remove action is clicked.
    /// </summary>
    void btnOKRemove_Click(object sender, EventArgs e)
    {
        RaiseOnDeleteEvent();
        HideCurrentPopup();
    }


    /// <summary>
    /// When dialog's OK button for update action is clicked.
    /// </summary>
    void btnOKProperties_Click(object sender, EventArgs e)
    {
        RaiseOnUpdateEvent(txtPropertyName.Text, -1);
        HideCurrentPopup();
    }


    /// <summary>
    /// When dialog's OK button for new variant action is clicked.
    /// </summary>
    void btnOKAdd_Click(object sender, EventArgs e)
    {
        int variantContentID = 0;
        if (radBasedOnTemplate.Checked)
        {
            int variantIndex = ValidationHelper.GetInteger(hdnSelected.Value, 0);
            variantIndex = (variantIndex < 0 ? 0 : variantIndex);
            variantIndex = (variantIndex >= mVariants.Count ? mVariants.Count - 1 : variantIndex);
            IssueABVariantItem issueVariant = mVariants[variantIndex];
            variantContentID = issueVariant.IssueID;
        }
        RaiseOnAddEvent(txtDisplayName.Text, variantContentID);
        HideCurrentPopup();
    }

    #endregion


    #region "IPostBackEventHandler Members"

    /// <summary>
    /// Handles postbacks invoked upon this control.
    /// </summary>
    /// <param name="eventArgument">Argument that goes with postback</param>
    public void RaisePostBackEvent(string eventArgument)
    {
        if (!string.IsNullOrEmpty(eventArgument))
        {
            try
            {
                if (!String.IsNullOrEmpty(eventArgument))
                {
                    // Parse event argument
                    pnlVariants.Visible = true;
                    ShowPopup(pnlVariants, mdlVariants, eventArgument.ToLowerCSafe());
                }
            }
            catch (Exception ex)
            {
                AddAlert(GetString("general.erroroccurred") + " " + ex.Message);
            }
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Sets the control up.
    /// </summary>
    private void SetupControl()
    {
        if (!ControlLoaded)
        {
            if (StopProcessing)
            {
                // Do nothing
            }
            else
            {
                // Register full postback buttons
                ControlsHelper.RegisterPostbackControl(btnOKAdd);
                ControlsHelper.RegisterPostbackControl(btnOKProperties);
                ControlsHelper.RegisterPostbackControl(btnOKRemove);

                // Initialize help icon
                ucTitle.IsDialog = true;

                ControlLoaded = true;

                string validationMethod = "if (ValidateInput_" + ClientID + "('{0}')) return false; ";
                btnOKAdd.OnClientClick = String.Format(validationMethod, txtDisplayName.ClientID);
                btnOKProperties.OnClientClick = String.Format(validationMethod, txtPropertyName.ClientID);

                btnOKAdd.Click += btnOKAdd_Click;
                btnOKProperties.Click += btnOKProperties_Click;
                btnOKRemove.Click += btnOKRemove_Click;

                radEmpty.Attributes.Add("onclick", String.Format("DisableList_{0}(true);", ClientID));
                radBasedOnTemplate.Attributes.Add("onclick", String.Format("DisableList_{0}(false);", ClientID));

                // Load variant names to listbox
                if (mVariants == null)
                {
                    mVariants = new List<IssueABVariantItem>();
                    mVariants.Add(new IssueABVariantItem(IssueID, GetString("newsletter.abvariantoriginal"), false, IssueStatusEnum.Idle));
                }

                lstTemplate.Items.Clear();
                foreach (IssueABVariantItem variant in mVariants)
                {
                    lstTemplate.Items.Add(new ListItem(variant.IssueVariantName, variant.IssueVariantName));
                }

                // Preselect first item
                lstTemplate.SelectedIndex = 0;
            }
        }
    }


    /// <summary>
    /// Performs actions necessary to show the popup dialog.
    /// </summary>
    /// <param name="dialogControl">New dialog control</param>
    /// <param name="modalPopup">Modal control</param>
    /// <param name="mode">Dialog mode</param>
    private void ShowPopup(Control dialogControl, ModalPopupDialog modalPopup, string mode)
    {
        // Set new identifiers
        CurrentModalID = modalPopup.ID;
        CurrentDialogID = dialogControl.ID;

        if ((CurrentModal != null) && (CurrentDialog != null))
        {
            // Enable dialog control's viewstate and visibility
            CurrentDialog.EnableViewState = true;
            CurrentDialog.Visible = true;

            // Init title
            ucTitle.TitleText = GetString("newslettervariant." + mode);

            if (mode == "properties")
            {
                txtPropertyName.Text = hdnParameter.Value;
            }

            plcAddVariant.Visible = mode.EqualsCSafe("addvariant", StringComparison.InvariantCultureIgnoreCase);
            btnOKAdd.Visible = plcAddVariant.Visible;
            plcProperties.Visible = mode.EqualsCSafe("properties", StringComparison.InvariantCultureIgnoreCase);
            btnOKProperties.Visible = plcProperties.Visible;
            plcRemoveVariant.Visible = mode.EqualsCSafe("removevariant", StringComparison.InvariantCultureIgnoreCase);
            btnOKRemove.Visible = plcRemoveVariant.Visible;

            // Show modal popup
            CurrentModal.Show();
        }
    }


    /// <summary>
    /// Performs actions necessary to hide popup dialog.
    /// </summary>
    private void HideCurrentPopup()
    {
        if ((CurrentModal != null) && (CurrentDialog != null))
        {
            // Hide modal dialog
            CurrentModal.Hide();

            // Reset dialog control's viewstate and visibility
            CurrentDialog.EnableViewState = false;
            CurrentDialog.Visible = false;
        }

        // Reset identifiers
        CurrentModalID = null;
        CurrentDialogID = null;
    }


    /// <summary>
    /// Adds alert script to the page.
    /// </summary>
    /// <param name="message">Message to show</param>
    private void AddAlert(string message)
    {
        if (!alertAdded)
        {
            string script = ScriptHelper.GetScript("setTimeout(function() {" + ScriptHelper.GetAlertScript(message, false) + "}, 50);");
            ScriptHelper.RegisterStartupScript(this, typeof(string), script.GetHashCode().ToString(), script);
            alertAdded = true;
        }
    }


    /// <summary>
    /// Creates new variant and raises "Add" event if specified.
    /// </summary>
    /// <param name="name">Name of new variant</param>
    /// <param name="issueId">ID of source issue on which the new variant will be based</param>
    private void RaiseOnAddEvent(string name, int issueId)
    {
        // Get main issue (original)
        int currentIssuedId = IssueID;
        IssueInfo parentIssue = IssueInfoProvider.GetOriginalIssue(currentIssuedId);

        // Allow modifying issues in idle state only
        if ((parentIssue == null) || (parentIssue.IssueStatus != IssueStatusEnum.Idle))
        {
            return;
        }

        // Get issue content specified by ID (if not found use original)
        IssueInfo contentIssue = null;
        if (issueId > 0)
        {
            if (issueId == parentIssue.IssueID)
            {
                contentIssue = parentIssue;
            }
            else
            {
                contentIssue = IssueInfoProvider.GetIssueInfo(issueId);
            }
        }

        NewsletterInfo newsletter = NewsletterInfoProvider.GetNewsletterInfo(parentIssue.IssueNewsletterID);

        // ID of the first child (if new A/B test is being created (i.e. parent and 2 children)
        int origVariantId = 0;

        // Check if current issue is variant issue
        if (!parentIssue.IssueIsABTest)
        {
            // Variant issue has not been created yet => create original and 2 child variants
            parentIssue.IssueIsABTest = true;

            // Create 1st variant based on parent issue, the 2nd variant will be created as ordinary variant below
            IssueInfo issueOrigVariant = parentIssue.Clone(true);
            issueOrigVariant.IssueVariantOfIssueID = parentIssue.IssueID;
            issueOrigVariant.IssueVariantName = GetString("newsletter.abvariantoriginal");
            issueOrigVariant.IssueScheduledTaskID = 0;
            IssueInfoProvider.SetIssueInfo(issueOrigVariant);
            // Create scheduled task for variant mail-out and update issue variant
            issueOrigVariant.IssueScheduledTaskID = CreateScheduledTask(issueOrigVariant);
            IssueInfoProvider.SetIssueInfo(issueOrigVariant);
            // Update parent issue
            IssueInfoProvider.SetIssueInfo(parentIssue);
            try
            {
                ObjectVersionManager.DestroyObjectHistory(parentIssue.TypeInfo.ObjectType, parentIssue.IssueID);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("Newsletter-AddVariant", "EXCEPTION", ex);
            }

            origVariantId = issueOrigVariant.IssueID;
        }

        // Variant issue has been created => create new variant only
        IssueInfo issueVariant = (contentIssue != null ? contentIssue.Clone(true) : parentIssue.Clone(true));
        issueVariant.IssueVariantName = name;
        issueVariant.IssueVariantOfIssueID = parentIssue.IssueID;

        // Prepare content with empty regions if empty content will be used
        string[] regions = null;
        if ((contentIssue == null) && (newsletter != null))
        {
            EmailTemplateInfo template = EmailTemplateInfoProvider.GetEmailTemplateInfo(newsletter.NewsletterTemplateID);
            if (template != null)
            {
                bool isValidRegionName;
                List<string> regionNames = new List<string>();
                EmailTemplateHelper.ValidateEditableRegions(template.TemplateBody, out isValidRegionName, out isValidRegionName, regionNames);
                for (int i = regionNames.Count - 1; i >= 0; i--)
                {
                    regionNames[i] = regionNames[i] + "::";
                }
                regions = regionNames.ToArray();
                // Set template ID (i.e. this template with these regions is used for current issue)
                issueVariant.IssueTemplateID = template.TemplateID;
            }
        }

        issueVariant.IssueText = (contentIssue != null ? contentIssue.IssueText : IssueHelper.GetContentXML(regions));
        issueVariant.IssueScheduledTaskID = 0;
        IssueInfoProvider.SetIssueInfo(issueVariant);

        // Duplicate attachments and replace old guids with new guids in issue text if current variant issue is based on content of another
        if (contentIssue != null)
        {
            List<Guid> guids = new List<Guid>();
            MetaFileInfoProvider.CopyMetaFiles(contentIssue.IssueID, issueVariant.IssueID,
                (contentIssue.IssueIsVariant ? IssueInfo.OBJECT_TYPE_VARIANT : IssueInfo.OBJECT_TYPE),
                ObjectAttachmentsCategories.ISSUE, IssueInfo.OBJECT_TYPE_VARIANT, ObjectAttachmentsCategories.ISSUE, guids);
            if (guids.Count > 0)
            {
                for (int i = 0; i < guids.Count; i += 2)
                {
                    issueVariant.IssueText = LinkConverter.ReplaceInLink(issueVariant.IssueText, guids[i].ToString(), guids[i + 1].ToString());
                }
            }
        }

        // Create scheduled task for variant mail-out
        issueVariant.IssueScheduledTaskID = CreateScheduledTask(issueVariant);
        // Update issue variant
        IssueInfoProvider.SetIssueInfo(issueVariant);

        if (origVariantId > 0)
        {
            // New A/B test issue created => create new A/B test info
            ABTestInfo abi = new ABTestInfo
            {
                TestIssueID = parentIssue.IssueID, TestSizePercentage = 50, TestWinnerOption = ABTestWinnerSelectionEnum.OpenRate, TestSelectWinnerAfter = 60
            };
            ABTestInfoProvider.SetABTestInfo(abi);

            // Move attachments (meta files) from parent issue to first variant
            MetaFileInfoProvider.MoveMetaFiles(parentIssue.IssueID, origVariantId, IssueInfo.OBJECT_TYPE, ObjectAttachmentsCategories.ISSUE, IssueInfo.OBJECT_TYPE_VARIANT, ObjectAttachmentsCategories.ISSUE);
            MetaFileInfoProvider.MoveMetaFiles(parentIssue.IssueID, origVariantId, IssueInfo.OBJECT_TYPE_VARIANT, ObjectAttachmentsCategories.ISSUE, IssueInfo.OBJECT_TYPE_VARIANT, ObjectAttachmentsCategories.ISSUE);
        }

        if (OnAddVariant != null)
        {
            VariantEventArgs args = new VariantEventArgs(name, issueVariant.IssueID);
            OnAddVariant(this, args);
        }
    }


    /// <summary>
    /// Creates new scheduled task for the given issue and newsletter.
    /// </summary>
    /// <param name="issue">Issue</param>
    private int CreateScheduledTask(IssueInfo issue)
    {
        if (issue == null)
        {
            throw new ArgumentNullException("issue");
        }

        // Create new scheduled task
        TaskInfo task = NewsletterTasksManager.CreateMailoutTask(issue, DateTime.Now, false);
        TaskInfoProvider.SetTaskInfo(task);
        return task.TaskID;
    }


    /// <summary>
    /// Raises "Delete" event if specified.
    /// </summary>
    private void RaiseOnDeleteEvent()
    {
        if (OnDeleteVariant != null)
        {
            OnDeleteVariant(this, EventArgs.Empty);
        }
    }


    /// <summary>
    /// Raises "Update" event if specified.
    /// </summary>
    /// <param name="name">Variant name</param>
    /// <param name="index">Varaint index</param>
    private void RaiseOnUpdateEvent(string name, int index)
    {
        if (OnUpdateVariant != null)
        {
            VariantEventArgs args = new VariantEventArgs(name, index);
            OnUpdateVariant(this, args);
        }
    }

    #endregion
}