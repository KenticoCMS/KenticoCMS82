using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Collections.Generic;

using CMS.Controls;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.PortalEngine;
using CMS.Base;
using CMS.Membership;
using CMS.UIControls;
using CMS.MacroEngine;
using CMS.DataEngine;

[UIElement("CMS.Newsletter", "Newsletters")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Iframe_Edit : CMSNewsletterPage
{
    #region "Variables"

    // Issue ID
    private int mIssueID;

    // Newsletter ID
    private int mNewsletterID;

    // Template ID
    private int mTemplateID;

    // Read only flag (if TRUE toolbar is hidden)
    private bool mReadOnly;

    // Editable regions' contents
    private Hashtable regionsContents = new Hashtable();

    // Issue object
    private IssueInfo issue;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get issue ID
        mIssueID = QueryHelper.GetInteger("objectid", 0);

        // Get newsletter ID
        mNewsletterID = QueryHelper.GetInteger("parentobjectid", 0);

        // Get template ID
        mTemplateID = QueryHelper.GetInteger("templateid", 0);

        // Get read only flag
        mReadOnly = QueryHelper.GetInteger("readonly", 0) == 1;

        if (mIssueID > 0)
        {
            // Get issue object
            issue = IssueInfoProvider.GetIssueInfo(mIssueID);
        }

        if (mTemplateID == 0)
        {
            if (issue != null)
            {
                mTemplateID = issue.IssueTemplateID;
            }
            else if (mNewsletterID > 0)
            {
                NewsletterInfo newsletter = NewsletterInfoProvider.GetNewsletterInfo(mNewsletterID);
                if (newsletter != null)
                {
                    mTemplateID = newsletter.NewsletterTemplateID;
                }
            }
        }

        if (mTemplateID > 0)
        {
            // Load content from the template
            LoadContent();
            LoadRegionList();
            RegisterScript();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        
        RegisterActionScripts();
    }


    /// <summary>
    /// Registers action scripts
    /// </summary>
    private void RegisterActionScripts()
    {
        ScriptHelper.RegisterEditScript(Page);
        ScriptHelper.RegisterModule(Page, "CMS/HeaderShadow");
        ScriptHelper.RegisterSpellChecker(Page);

        StringBuilder sb = new StringBuilder();
        sb.Append("var spellURL = '", AuthenticationHelper.ResolveDialogUrl("~/CMSModules/Content/CMSDesk/Edit/SpellCheck.aspx"), "'; \n");
        sb.Append("function SpellCheck_", ClientID, "() { checkSpelling(spellURL); }");

        ControlsHelper.RegisterClientScriptBlock(this, Page, typeof(string), "SpellCheckAction" + ClientID, ScriptHelper.GetScript(sb.ToString()));
    }


    protected void RegisterScript()
    {
        if (mTemplateID <= 0)
        {
            return;
        }

        // Register JS scripts
        string script =
@"        
// Set id of curently focused region to focusedRegionID (declared in LoadRegionList() in code behind)
function RememberFocusedRegion() {
    if(typeof regions !== 'undefined')
    {
        for (i = 0; i < regions.length; i++) //regions array is declared in LoadRegionList() in code behind
        {
            if (window.CKEDITOR != null) {
                var oEditor = window.CKEDITOR.instances[regions[i]];
                if ((oEditor != null) && (oEditor.focusManager.hasFocus)) {
                    focusedRegionID = regions[i];
                    break;
                }
            }
        }
    }
}

// Set id of currently focused region to focusedRegionID.
// If no region is focused then set focus to the first region.
function SetFocusedRegion() {
    RememberFocusedRegion();
    if ((focusedRegionID == '') && (regions.length > 0)) {
        if (window.CKEDITOR != null) {
            var oEditor = window.CKEDITOR.instances[regions[0]];
            if (oEditor != null) {
                oEditor.focusManager.focus();
                focusedRegionID = regions[0];

                return oEditor;
            }
        }
    }
}

// Insert desired HTML at the current cursor position of the CK editor
function InsertHTML(htmlString) {
    var oEditor;
    if (focusedRegionID == '') //focusedRegionID is declared in LoadRegionList() in code behind
    {
        oEditor = SetFocusedRegion();
    }

    if (focusedRegionID != '') //focusedRegionID is declared in LoadRegionList() in code behind
    {
        if (window.CKEDITOR != null) {
            if (oEditor == null) {
                // Get the editor instance that we want to interact with.
                oEditor = window.CKEDITOR.instances[focusedRegionID];
            }
            if (oEditor != null) {
                // Check the active editing mode.
                if (oEditor.mode == 'wysiwyg') {
                    // Insert the desired HTML.
                    oEditor.focus();
                    oEditor.insertHtml(htmlString);
                }
                else {
                    alert('You must be on WYSIWYG mode!');
                }
            }
        }
    }
    return false;
}

// Get content of all editable regions of edited newsletter issue.
// Format: '<region1_ID>::<region1_content>#|#<region2_ID>::<region2_content>#|#...'
function GetContent() {
    var content = '';
    // regions array is declared in LoadRegionList() in code behind
    for (i = 0; i < regions.length; i++) {
        if (window.CKEDITOR != null) {
            var oEditor = window.CKEDITOR.instances[regions[i]];
            if (oEditor != null) {
                content += regionIDs[i] + '::' + oEditor.getData() + '#|#';
            }
        }
    }

    return content;
}

// Set content to editable regions based on given content string.
// Format of content string: '<region1_ID>::<region1_content>#|#<region2_ID>::<region2_content>#|#...'
function SetRegionContent(regcontent) {
    if ((regcontent != null || regcontent != '') && (window.CKEDITOR != null)) {
        // Method cannot be used directly, because it can be not loaded at execution time
        WaitForOn(window.CKEDITOR, regcontent);        
    }
}

function WaitForOn(editor, content)
{
    if ( editor.on != null  ) {
        editor.on('instanceReady', function(e) {SetRegionContentInternal(e.editor, content);});
    } else {
        setTimeout( function(){ WaitForOn(editor, content); }, 250 );
    }
}

function SetRegionContentInternal(editor, regcontent) {
    var contents = regcontent.split('#|#');
    if (contents.length > 0) {
        var region = new Array();
        var oEditor;
        for (i = 0; i < contents.length; i++) {
            region = contents[i].split('::');
            if ((region.length == 2) && (region[1] != '') && (editor.name == region[0] + '_HtmlEditor')) {
                editor.insertHtml(region[1]);
            }
        }
    }
}";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "IssueContentScripts_" + ClientID, script, true);
    }


    /// <summary>
    /// Loads content from specific newsletter template.
    /// </summary>
    private void LoadContent()
    {
        EmailTemplateInfo emailTemplate = EmailTemplateInfoProvider.GetEmailTemplateInfo(mTemplateID);
        if ((emailTemplate == null) || string.IsNullOrEmpty(emailTemplate.TemplateBody))
        {
            return;
        }

        // Remove security parameters from macros
        string templateText = MacroSecurityProcessor.RemoveSecurityParameters(emailTemplate.TemplateBody, true, null);

        if (!RequestHelper.IsPostBack() && (issue != null))
        {
            // Load content of editable regions
            IssueHelper.LoadRegionsContents(ref regionsContents, issue.IssueText);
        }

        LiteralControl before;
        int count = 0;
        int textStart = 0;
        string toolbarLocation = "Out:CKToolbar";
        string toolbarSetName = "Newsletter";

        int editRegStart = templateText.IndexOfCSafe("$$", textStart);

        // Apply CSS e-mail template style        
        HTMLHelper.AddToHeader(Page, CSSHelper.GetCSSFileLink(EmailTemplateInfoProvider.GetStylesheetUrl(emailTemplate.TemplateName)));

        while (editRegStart >= 0)
        {
            count++;

            before = new LiteralControl();
            // Get template text surrounding editable regions - make links absolute
            before.Text = URLHelper.MakeLinksAbsolute(templateText.Substring(textStart, (editRegStart - textStart)));
            plcContent.Controls.Add(before);

            // End of region
            editRegStart += 2;
            textStart = editRegStart;
            if (editRegStart < templateText.Length - 1)
            {
                int editRegEnd = templateText.IndexOfCSafe("$$", editRegStart);
                if (editRegEnd >= 0)
                {
                    string region = templateText.Substring(editRegStart, editRegEnd - editRegStart);
                    string[] parts = (region + ":" + ":").Split(':');

                    try
                    {
                        string name = parts[0];
                        if (!string.IsNullOrEmpty(name.Trim()))
                        {
                            Regex intNumber = RegexHelper.GetRegex("^[0-9]+");
                            int width = ValidationHelper.GetInteger(intNumber.Match(parts[1]).Value, 0);
                            int height = ValidationHelper.GetInteger(intNumber.Match(parts[2]).Value, 0);

                            CMSEditableRegion editableRegion = new CMSEditableRegion();
                            editableRegion.ID = name;
                            editableRegion.RegionType = CMSEditableRegionTypeEnum.HtmlEditor;
                            editableRegion.ViewMode = ViewModeEnum.Edit;

                            editableRegion.DialogHeight = height;
                            editableRegion.DialogWidth = width;

                            editableRegion.WordWrap = false;
                            editableRegion.HtmlAreaToolbarLocation = toolbarLocation;
                            editableRegion.RegionTitle = name;
                            editableRegion.UseStylesheet = false;
                            editableRegion.HTMLEditorCssStylesheet = EmailTemplateInfoProvider.GetStylesheetUrl(emailTemplate.TemplateName);

                            if (!mReadOnly)
                            {
                                editableRegion.HtmlAreaToolbar = toolbarSetName;
                            }
                            else
                            {
                                editableRegion.HtmlAreaToolbar = "Disabled";
                            }

                            CMSHtmlEditor editor = editableRegion.HtmlEditor;
                            editor.ExtraPlugins.Add("CMSPlugins");
                            editor.ExtraPlugins.Add("autogrow");
                            editor.AutoGrowMinHeight = height;
                            editor.ResolverName = "NewsletterResolver";

                            DialogConfiguration dialogConfig = editor.MediaDialogConfig;
                            dialogConfig.MetaFileObjectID = (issue != null) ? issue.IssueID : 0;
                            dialogConfig.MetaFileObjectType = (issue != null) && issue.IssueIsVariant ? IssueInfo.OBJECT_TYPE_VARIANT : IssueInfo.OBJECT_TYPE;
                            dialogConfig.MetaFileCategory = ObjectAttachmentsCategories.ISSUE;
                            dialogConfig.HideAttachments = false;

                            editableRegion.LoadContent(ValidationHelper.GetString(regionsContents[name.ToLowerCSafe()], string.Empty));

                            plcContent.Controls.Add(editableRegion);

                            textStart = editRegEnd + 2;
                        }
                    }
                    catch
                    {
                    }
                }
            }
            editRegStart = templateText.IndexOfCSafe("$$", textStart);
        }

        before = new LiteralControl();
        before.Text = URLHelper.MakeLinksAbsolute(templateText.Substring(textStart));

        plcContent.Controls.Add(before);
    }


    /// <summary>
    /// Prepares script with array of editable regions.
    /// </summary>
    protected void LoadRegionList()
    {
        // Get all Editable controls within 'plcContent'
        List<ICMSEditableControl> regionList = CMSControlsHelper.CollectEditableControls(plcContent);

        // Create array of regions IDs in javascript. We will use it to find out the focused region
        StringBuilder script = new StringBuilder();
        script.AppendFormat("var focusedRegionID = '';\n var regions = new Array({0});\n var regionIDs = new Array({0});\n", regionList.Count);

        for (int i = 0; i < regionList.Count; i++)
        {
            CMSEditableRegion editRegion = (CMSEditableRegion)regionList[i];
            if (editRegion != null)
            {
                script.AppendFormat("regions[{0}] = '{1}_HtmlEditor'; \n ", i, editRegion.ClientID);
                script.AppendFormat("regionIDs[{0}] = '{1}'; \n ", i, editRegion.ID);
            }
        }

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "IssueRegions", ScriptHelper.GetScript(script.ToString()));
    }

    #endregion
}