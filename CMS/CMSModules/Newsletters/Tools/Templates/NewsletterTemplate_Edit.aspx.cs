using System;
using System.Threading;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.DataEngine;

// Set edited object
[EditedObject(EmailTemplateInfo.OBJECT_TYPE, "objectid")]
[UIElement(ModuleName.NEWSLETTER, "Templates.General")]
public partial class CMSModules_Newsletters_Tools_Templates_NewsletterTemplate_Edit : CMSNewsletterPage
{
    #region "Variables"

    protected int templateid = 0;
    private const string mAttachmentsActionClass = "attachments-header-action";

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get subscriber by its ID and check its existence
        var template = EditedObject as EmailTemplateInfo;

        if (template == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        if (!template.CheckPermissions(PermissionsEnum.Read, CurrentSiteName, CurrentUser))
        {
            RedirectToAccessDenied(template.TypeInfo.ModuleName, "ManageTemplates");
        }

        rfvTemplateDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
        rfvTemplateName.ErrorMessage = GetString("NewsletterTemplate_Edit.ErrorEmptyName");

        ScriptHelper.RegisterSpellChecker(this);

        // Control initializations
        string varsScript = string.Format("var emptyNameMsg = '{0}'; \nvar emptyWidthMsg = '{1}'; \nvar emptyHeightMsg = '{2}'; \nvar spellURL = '{3}'; \n",
                                          GetString("NewsletterTemplate_Edit.EmptyNameMsg"),
                                          GetString("NewsletterTemplate_Edit.EmptyWidthMsg"),
                                          GetString("NewsletterTemplate_Edit.EmptyHeightMsg"),
                                          AuthenticationHelper.ResolveDialogUrl("~/CMSModules/Content/CMSDesk/Edit/SpellCheck.aspx"));
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "Script_" + ClientID, ScriptHelper.GetScript(varsScript));

        // Set fields to be checked by Spell Checker
        string spellCheckScript = string.Format("if (typeof(spellCheckFields)==='undefined') {{var spellCheckFields = new Array();}} spellCheckFields.push('{0}');",
                                                htmlTemplateBody.ClientID);
        ScriptHelper.RegisterStartupScript(this, typeof(string), ClientID, ScriptHelper.GetScript(spellCheckScript));

        // Get edited object and its existence
        EmailTemplateInfo emailTemplateObj = (EmailTemplateInfo)EditedObject;
        templateid = emailTemplateObj.TemplateID;

        // Display editable region section only for e-mail templates of type "Issue template"
        if (emailTemplateObj.TemplateType == EmailTemplateType.Issue)
        {
            pnlEditableRegion.Visible = true;
            plcThumb.Visible = true;
            ucThumbnail.Visible = true;
            ucThumbnail.ObjectID = emailTemplateObj.TemplateID;
            ucThumbnail.ObjectType = EmailTemplateInfo.OBJECT_TYPE;
            ucThumbnail.Category = ObjectAttachmentsCategories.THUMBNAIL;
            ucThumbnail.SiteID = emailTemplateObj.TemplateSiteID;
        }
        else
        {
            plcSubject.Visible = true;
        }

        // Init CSS styles every time during page load
        htmlTemplateBody.EditorAreaCSS = EmailTemplateInfoProvider.GetStylesheetUrl(emailTemplateObj.TemplateName) + "&timestamp=" + DateTime.Now.Millisecond;

        // Initialize header actions
        InitHeaderActions(emailTemplateObj.TemplateID);

        // Initialize HTML editor
        InitHTMLEditor(emailTemplateObj);

        if (!RequestHelper.IsPostBack())
        {
            // Initialize dialog
            LoadData(emailTemplateObj);

            // Show that the emailTemplate was created successfully
            if (QueryHelper.GetBoolean("saved", false))
            {
                ShowChangesSaved();
            }
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Register client scripts
        string script = string.Format(@"
// Insert desired HTML at the current cursor position of the HTML editor
function InsertHTML(htmlString) {{
    // Get the editor instance that we want to interact with.
    var oEditor = CKEDITOR.instances['{0}'];
    // Check the active editing mode.
    if (oEditor.mode == 'wysiwyg') {{
        // Insert the desired HTML.
        oEditor.insertHtml(htmlString);
    }}
    else
        alert('You must be on WYSIWYG mode!');
    return false;
}}

function PasteImage(imageurl) {{
    imageurl = '<img src=""' + imageurl + '"" />';
    return InsertHTML(imageurl);
}}

function InsertEditableRegion() {{
    if (document.getElementById('{1}').value == '') {{
        alert(emptyNameMsg);
        return;
    }}

    var region = ""$$"";
    region += document.getElementById('{1}').value + "":"";
    region += document.getElementById('{2}').value + "":"";
    region += document.getElementById('{3}').value + ""$$"";
    InsertHTML(region);
}}
", htmlTemplateBody.ClientID, txtName.ClientID, txtWidth.ClientID, txtHeight.ClientID);

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "InsertHTMLScripts_" + ClientID, ScriptHelper.GetScript(script));
    }

    #endregion


    #region "Methods"
    /// <summary>
    /// Initializes header action control.
    /// </summary>
    /// <param name="templateId">Template ID</param>
    protected void InitHeaderActions(int templateId)
    {
        bool isAuthorized = CurrentUser.IsAuthorizedPerResource("CMS.Newsletter", "ManageTemplates") && (EditedObject != null);

        // Init save button
        CurrentMaster.HeaderActions.ActionsList.Add(new SaveAction(this)
        {
            Enabled = isAuthorized
        });

        // Init spellcheck button
        CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("spellcheck.title"),
            Tooltip = GetString("spellcheck.title"),
            OnClientClick = "checkSpelling(spellURL); return false;",
            ButtonStyle = ButtonStyle.Default,
        });

        int attachCount = 0;
        if (isAuthorized)
        {
            // Get number of attachments
            InfoDataSet<MetaFileInfo> ds = MetaFileInfoProvider.GetMetaFiles(templateId, EmailTemplateInfo.OBJECT_TYPE, ObjectAttachmentsCategories.TEMPLATE, null, null, "MetafileID", -1);
            attachCount = ds.Items.Count;

            // Register attachments count update module
            ScriptHelper.RegisterModule(this, "CMS/AttachmentsCountUpdater", new { Selector = "." + mAttachmentsActionClass, Text = ResHelper.GetString("general.attachments") });

            // Register dialog scripts
            ScriptHelper.RegisterDialogScript(Page);
        }

        // Prepare metafile dialog URL
        string metaFileDialogUrl = ResolveUrl(@"~/CMSModules/AdminControls/Controls/MetaFiles/MetaFileDialog.aspx");
        string query = string.Format("?objectid={0}&objecttype={1}", templateId, EmailTemplateInfo.OBJECT_TYPE);
        metaFileDialogUrl += string.Format("{0}&category={1}&hash={2}", query, ObjectAttachmentsCategories.TEMPLATE, QueryHelper.GetHash(query));

        // Init attachment button
        CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("general.attachments") + ((attachCount > 0) ? " (" + attachCount + ")" : string.Empty),
            Tooltip = GetString("general.attachments"),
            OnClientClick = string.Format(@"if (modalDialog) {{modalDialog('{0}', 'Attachments', '700', '500');}}", metaFileDialogUrl) + " return false;",
            Enabled = isAuthorized,
            CssClass = mAttachmentsActionClass,
            ButtonStyle = ButtonStyle.Default,
        });

        CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }


    /// <summary>
    /// Load data of editing emailTemplate.
    /// </summary>
    /// <param name="emailTemplateObj">EmailTemplate object</param>
    protected void LoadData(EmailTemplateInfo emailTemplateObj)
    {
        if (emailTemplateObj != null)
        {
            htmlTemplateBody.ResolvedValue = emailTemplateObj.TemplateBody;
            txtTemplateName.Text = emailTemplateObj.TemplateName;
            txtTemplateHeader.Value = emailTemplateObj.TemplateHeader;
            txtTemplateFooter.Value = emailTemplateObj.TemplateFooter;
            txtTemplateDisplayName.Text = emailTemplateObj.TemplateDisplayName;
            txtTemplateStyleSheetText.Text = emailTemplateObj.TemplateStylesheetText;

            // Display temaplate subject only for 'subscription' and 'unsubscription' template types
            if (emailTemplateObj.TemplateType != EmailTemplateType.Issue)
            {
                txtTemplateSubject.Text = emailTemplateObj.TemplateSubject;
            }
        }
    }


    /// <summary>
    /// Initializes HTML editor's settings.
    /// </summary>
    protected void InitHTMLEditor(EmailTemplateInfo emailTemplateObj)
    {
        htmlTemplateBody.AutoDetectLanguage = false;
        htmlTemplateBody.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        htmlTemplateBody.ToolbarSet = "Newsletter";
        if ((emailTemplateObj != null) && (emailTemplateObj.TemplateType == EmailTemplateType.DoubleOptIn))
        {
            htmlTemplateBody.ResolverName = "NewsletterOptInResolver";
        }
        else
        {
            htmlTemplateBody.ResolverName = "NewsletterResolver";
        }

        DialogConfiguration config = htmlTemplateBody.MediaDialogConfig;
        config.MetaFileObjectID = (emailTemplateObj != null) ? emailTemplateObj.TemplateID : 0;
        config.MetaFileObjectType = EmailTemplateInfo.OBJECT_TYPE;
        config.MetaFileCategory = ObjectAttachmentsCategories.TEMPLATE;
        config.HideAttachments = false;
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Actions handler.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "save":
                // Check 'Manage templates' permission
                if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.newsletter", "managetemplates"))
                {
                    RedirectToAccessDenied("cms.newsletter", "managetemplates");
                }

                string errorMessage;
                // Check template code name
                if (!ValidationHelper.IsCodeName(txtTemplateName.Text))
                {
                    errorMessage = GetString("General.ErrorCodeNameInIdentifierFormat");
                }
                else
                {
                    // Check code and display name for emptiness
                    errorMessage = new Validator().NotEmpty(txtTemplateDisplayName.Text, GetString("general.requiresdisplayname")).NotEmpty(txtTemplateName.Text, GetString("NewsletterTemplate_Edit.ErrorEmptyName")).Result;
                }

                if (string.IsNullOrEmpty(errorMessage))
                {
                    // TemplateName must to be unique
                    EmailTemplateInfo emailTemplateObj = EmailTemplateInfoProvider.GetEmailTemplateInfo(txtTemplateName.Text.Trim(), SiteContext.CurrentSiteID);

                    // If templateName value is unique														
                    if ((emailTemplateObj == null) || (emailTemplateObj.TemplateID == templateid))
                    {
                        // If templateName value is unique -> determine whether it is update or insert 
                        if ((emailTemplateObj == null))
                        {
                            // Get EmailTemplate object by primary key
                            emailTemplateObj = EmailTemplateInfoProvider.GetEmailTemplateInfo(templateid) ?? new EmailTemplateInfo();
                        }

                        // Check region names validity
                        bool isValidRegionName;
                        bool isValid;
                        string templateBody = htmlTemplateBody.ResolvedValue.Trim();

                        EmailTemplateHelper.ValidateEditableRegions(templateBody, out isValid, out isValidRegionName, null);
                        if (isValid)
                        {
                            if (isValidRegionName)
                            {
                                // Set template object
                                emailTemplateObj.TemplateBody = templateBody;
                                emailTemplateObj.TemplateName = txtTemplateName.Text.Trim();
                                emailTemplateObj.TemplateHeader = ValidationHelper.GetString(txtTemplateHeader.Value, "").Trim();
                                emailTemplateObj.TemplateFooter = ValidationHelper.GetString(txtTemplateFooter.Value, "").Trim();
                                emailTemplateObj.TemplateDisplayName = txtTemplateDisplayName.Text.Trim();
                                emailTemplateObj.TemplateStylesheetText = txtTemplateStyleSheetText.Text.Trim();

                                // Set temaplte subject only for 'subscription' and 'unsubscription' template types
                                if (plcSubject.Visible)
                                {
                                    emailTemplateObj.TemplateSubject = txtTemplateSubject.Text.Trim();
                                }

                                // Save the template object and display info message
                                EmailTemplateInfoProvider.SetEmailTemplateInfo(emailTemplateObj);
                                ShowChangesSaved();

                                // Reload header if changes were saved
                                ScriptHelper.RefreshTabHeader(Page, emailTemplateObj.TemplateDisplayName);
                            }
                            else
                            {
                                ShowError(GetString("NewsletterTemplate_Edit.EditableRegionNameError"));
                            }
                        }
                        else
                        {
                            ShowError(GetString("NewsletterTemplate_Edit.EditableRegionError"));
                        }
                    }
                    else
                    {
                        ShowError(GetString("NewsletterTemplate_Edit.TemplateNameExists"));
                    }
                }
                else
                {
                    ShowError(errorMessage);
                }
                break;
        }
    }

    #endregion
}