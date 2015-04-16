using System;
using System.Text;
using System.Web.UI;

using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Newsletters;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSModules_Newsletters_Controls_EditIssue : CMSAdminControl
{
    #region "Variables"

    private bool loaded;
    private bool validated;
    private bool mIsDialogMode;
    private int mTemplateID = 0;

    #endregion


    #region "Properties"

    /// <summary>
    /// ID of newsletter issue that should be edited, required when editing an issue.
    /// </summary>
    public int IssueID
    {
        get;
        set;
    }


    /// <summary>
    /// Newsletter ID, required when creating new issue.
    /// </summary>
    public int NewsletterID
    {
        get;
        set;
    }


    /// <summary>
    /// ID of newsletter template that should be used for new issue.
    /// If not set, template from newsletter configuration is used.
    /// </summary>
    public int TemplateID
    {
        get
        {
            if (mTemplateID == 0)
            {
                // Try to get value from selector
                mTemplateID = ValidationHelper.GetInteger(issueTemplate.Value, 0);
                if (mTemplateID == 0)
                {
                    // Try to get value from hidden field
                    mTemplateID = ValidationHelper.GetInteger(hdnTemplateID.Value, 0);
                }
            }
            
            return mTemplateID;
        }
        set
        {
            mTemplateID = value;
        }
    }


    /// <summary>
    /// Indicates that the control is used in new dialog.
    /// </summary>
    public bool IsDialogMode
    {
        get
        {
            return mIsDialogMode;
        }
        set
        {
            mIsDialogMode = value;
            contentBody.IsDialogMode = value;
        }
    }


    /// <summary>
    /// Gets or sets value that indicates whether control is enabled.
    /// </summary>
    public bool Enabled
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if advanced options should be displayed.
    /// </summary>
    protected bool ShowAdvancedOptions
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["ShowAdvancedOptions"], false);
        }
        set
        {
            ViewState["ShowAdvancedOptions"] = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing || ((NewsletterID <= 0) && (IssueID <= 0)))
        {
            return;
        }
  
        ReloadData(false);
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Preserve ID of selected template
        hdnTemplateID.Value = ValidationHelper.GetString(issueTemplate.Value, string.Empty);

        if (!StopProcessing)
        {
            RegisterScript();
        }
    }


    /// <summary>
    /// Swithes simple and advanced options.
    /// </summary>
    protected void lnkToggleAdvanced_Click(object sender, EventArgs e)
    {
        ShowAdvancedOptions = !plcAdvanced.Visible;
        
        // Switch simple and advanced
        InitSimpleAdvancedOptions();

        // JS function for resizing is specified in Newsletter_ContentEditor.ascx
        ScriptManager.RegisterStartupScript(Page, typeof(string), "ResizeContent_" + ClientID, "if (SetIFrameHeight) { SetIFrameHeight(); }", true);
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Reloads control data.
    /// </summary>
    /// <param name="forceReload">Indicates if force reload should be used</param>
    public override void ReloadData(bool forceReload)
    {
        if (!loaded || forceReload)
        {
            IssueInfo issue = null;
            if (IssueID > 0)
            {
                // Get issue object
                issue = IssueInfoProvider.GetIssueInfo(IssueID);
                if (issue != null)
                {
                    if (NewsletterID == 0)
                    {
                        // Set newsletter ID
                        NewsletterID = issue.IssueNewsletterID;
                    }

                    if (string.IsNullOrEmpty(txtSubject.Text) || forceReload)
                    {
                        txtSubject.Text = issue.IssueSubject;
                        chkShowInArchive.Checked = issue.IssueShowInNewsletterArchive;
                    }
                }
            }

            // Get newsletter object
            NewsletterInfo newsletterObj = NewsletterInfoProvider.GetNewsletterInfo(NewsletterID);
            if (newsletterObj != null)
            {
                // Modify where condition of template selector if issue exists
                string issueTemplateWhere = (issue != null) ? string.Format(" OR TemplateID IN (SELECT IssueTemplateID From Newsletter_NewsletterIssue WHERE IssueID={0})", issue.IssueID) : string.Empty;

                // Initialize template selector
                issueTemplate.WhereCondition = String.Format("(TemplateType='{0}') AND (TemplateID IN (SELECT NewsletterTemplateID FROM Newsletter_Newsletter WHERE NewsletterID={1})" +
                    " OR TemplateID IN (SELECT TemplateID FROM Newsletter_EmailTemplateNewsletter WHERE NewsletterID={1}){2}) AND (TemplateSiteID={3})",
                    EmailTemplateType.Issue, NewsletterID, issueTemplateWhere, newsletterObj.NewsletterSiteID);

                if (TemplateID > 0)
                {
                    // Set selected value
                    issueTemplate.Value = TemplateID;
                }

                if ((forceReload || (TemplateID <= 0)) && (issue != null) && (issue.IssueTemplateID != TemplateID))
                {
                    // Change selected value
                    issueTemplate.Value = TemplateID = issue.IssueTemplateID;

                    issueTemplate.Reload(forceReload);
                }

                if (TemplateID <= 0)
                {
                    // Get ID of default template
                    issueTemplate.Value = TemplateID = newsletterObj.NewsletterTemplateID;
                }

                // Initialize inputs and content controls
                if (!RequestHelper.IsPostBack() || (IsDialogMode && !forceReload && string.IsNullOrEmpty(txtSenderName.Text)) || forceReload)
                {
                    txtSenderName.Text = (issue != null ? issue.IssueSenderName : newsletterObj.NewsletterSenderName);
                }
                if (!RequestHelper.IsPostBack() || (IsDialogMode && !forceReload && string.IsNullOrEmpty(txtSenderEmail.Text)) || forceReload)
                {
                    txtSenderEmail.Text = (issue != null ? issue.IssueSenderEmail : newsletterObj.NewsletterSenderEmail);
                }

                contentBody.NewsletterID = NewsletterID;
                contentBody.IssueID = IssueID;
                contentBody.TemplateID = TemplateID;
                contentBody.IsDialogMode = IsDialogMode;
                contentBody.Enabled = Enabled;
                contentBody.ReloadData(forceReload);

                // Set simple/advanced options visibility
                InitSimpleAdvancedOptions();

                // Set flag
                loaded = true;
            }
        }

        txtSubject.Enabled = Enabled;
        txtSenderEmail.Enabled = Enabled;
        txtSenderName.Enabled = Enabled;
        issueTemplate.Enabled = Enabled;
        chkShowInArchive.Enabled = Enabled;
    }


    /// <summary>
    /// Validates dialog's content before saving.
    /// </summary>
    public bool IsValid()
    {
        // Check subject field for emptyness
        ErrorMessage = new Validator().NotEmpty(txtSubject.Text.Trim(), GetString("NewsletterContentEditor.SubjectRequired")).Result;

        // Check sender email format if entered
        if (string.IsNullOrEmpty(ErrorMessage) && !string.IsNullOrEmpty(txtSenderEmail.Text.Trim()) && !ValidationHelper.IsEmail(txtSenderEmail.Text.Trim()))
        {
            ErrorMessage = GetString("Newsletter_Edit.ErrorEmailFormat");
        }

        return validated = String.IsNullOrEmpty(ErrorMessage);
    }


    /// <summary>
    /// Creates new or updates existing newsletter issue.
    /// </summary>
    public bool Save()
    {
        if (validated || IsValid())
        {
            IssueInfo issue = null;

            if (IssueID == 0)
            {
                // Initialize new issue
                issue = new IssueInfo();
                issue.IssueUnsubscribed = 0;
                issue.IssueSentEmails = 0;
                issue.IssueNewsletterID = NewsletterID;
                issue.IssueSiteID = SiteContext.CurrentSiteID;
            }
            else
            {
                issue = IssueInfoProvider.GetIssueInfo(IssueID);
            }

            if (issue != null)
            {
                issue.IssueTemplateID = TemplateID;
                issue.IssueShowInNewsletterArchive = chkShowInArchive.Checked;
                issue.IssueSenderName = txtSenderName.Text.Trim();
                issue.IssueSenderEmail = txtSenderEmail.Text.Trim();

                // Saves content of editable region(s)
                // Get content from hidden field
                string content = hdnIssueContent.Value;
                string[] regions = null;
                if (!string.IsNullOrEmpty(content))
                {
                    // Split content for each region, separator is '#|#'
                    regions = content.Split(new [] { "#|#" }, StringSplitOptions.RemoveEmptyEntries);
                }
                issue.IssueText = IssueHelper.GetContentXML(regions);

                // Remove '#' from macros if included
                txtSubject.Text = txtSubject.Text.Trim().Replace("#%}", "%}");

                // Sign macros if included in the subject
                issue.IssueSubject = MacroSecurityProcessor.AddSecurityParameters(txtSubject.Text, MembershipContext.AuthenticatedUser.UserName, null);

                // Save issue
                IssueInfoProvider.SetIssueInfo(issue);

                // Update IssueID
                IssueID = issue.IssueID;

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Causes an update of issue properties and content areas.
    /// </summary>
    public void UpdateContent()
    {
        // Update issue properties
        pnlUpdate.Update();
        // Update content area
        pnlBodyUpdate.Update();
    }


    /// <summary>
    /// Refreshes content of editable regions in the issue body.
    /// Can be used when validation has failed.
    /// </summary>
    public void RefreshEditableRegions()
    {
        // Init editable regions on start up
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "SetRegionContent", string.Format("SetRegionContent({0});", ScriptHelper.GetString(hdnIssueContent.Value)), true);
    }

    #endregion


    #region "Protected methods"

    /// <summary>
    /// Shows or hides advanced options.
    /// </summary>
    protected void InitSimpleAdvancedOptions()
    {
        plcAdvanced.Visible = ShowAdvancedOptions;
        imgToggleAdvanced.ImageUrl = (ShowAdvancedOptions ? GetImageUrl("Design/Controls/UniGrid/Actions/SortUp.png") : GetImageUrl("Design/Controls/UniGrid/Actions/SortDown.png"));
        lnkToggleAdvanced.Text = (ShowAdvancedOptions ? GetString("install.HideAdvancedOptions") : GetString("install.ShowAdvancedOptions"));
    }


    /// <summary>
    /// Registers JS script.
    /// </summary>
    protected void RegisterScript()
    {
        string script = string.Format(
@"// iframe with ID 'iframeContent' is in Newsletter_ContentEditor.ascx control
var frame = null;

function GetFrame() {{
    if (frame == null) {{
         frame = document.getElementById('iframeContent');
    }}
    return frame;
}}

// Gets content of editable regions to hidden field
function GetContent() {{
    var hdnContent = document.getElementById('{0}');
    var F = GetFrame();
    if ((hdnContent != null) && (F != null) && (F.contentWindow.GetContent != null)) {{
        hdnContent.value = F.contentWindow.GetContent();
        return true;
    }}
    return false;
}}

// Sets hidden field value to editable regions
function SetRegionContent(regcontent) {{
    var F = GetFrame();
    if ((regcontent != null) && (F != null)) {{
        // Frame onload event cannot be used, because it is fired too late
        WaitForFrame(F, regcontent);
    }}
    return false;
}}


function WaitForFrame(frame, content) {{
    if ( frame.contentWindow != null && frame.contentWindow.SetRegionContent != null ) {{
        frame.contentWindow.SetRegionContent(content);
    }} else {{
        setTimeout( function(){{ WaitForFrame(frame, content); }}, 250 );
    }}
}};

// Remembers last focused region
function RememberFocusedRegion() {{
    var F = GetFrame();
    if ((F != null) && (F.contentWindow.RememberFocusedRegion != null)) {{
        F.contentWindow.RememberFocusedRegion();
    }}
}}

// Pastes image into last focused editable region
function PasteImage(imageurl) {{
    var imageHtml = '<img src=""' + imageurl + '"" alt="""" />';
    var F = GetFrame();
    if ((F != null) && (F.contentWindow.InsertHTML != null)) {{
        return F.contentWindow.InsertHTML(imageHtml);
    }}
}}

// Prevent enter key on form as postback does not send data from issue text automatically
// Default button cannot be easily set because 'Save button' is generated dynamically
$cmsj(function () {{
    $cmsj(this).keypress(function (event) {{
        if (event.which == 13) {{
            event.preventDefault();
        }}
    }});
}});", hdnIssueContent.ClientID);

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "EditIssueScript_" + ClientID, script, true);
    }

    #endregion
}