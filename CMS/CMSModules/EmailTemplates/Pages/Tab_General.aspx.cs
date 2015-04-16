using System;

using CMS.Controls;
using CMS.DataEngine;
using CMS.EmailEngine;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.Synchronization;
using CMS.UIControls;

[EditedObject(EmailTemplateInfo.OBJECT_TYPE, "templateid")]
public partial class CMSModules_EmailTemplates_Pages_Tab_General : CMSEmailTemplatesPage
{
    #region "Variables"

    private bool isDialog = false;

    #endregion


    #region "Constants"

    private const string mAttachmentsActionClass = "attachments-header-action";

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        isDialog = QueryHelper.GetBoolean("editonlycode", false);
        if (isDialog)
        {
            // Check hash
            var settings = new HashSettings
            {
                Redirect = false
            };

            if (!QueryHelper.ValidateHash("hash", "saved;name;templateid;selectorid;tabmode;siteid;selectedsiteid", settings, true))
            {
                URLHelper.Redirect(ResolveUrl(String.Format("~/CMSMessages/Error.aspx?title={0}&text={1}", GetString("dialogs.badhashtitle"), GetString("dialogs.badhashtext"))));
            }

            string templateName = QueryHelper.GetString("name", String.Empty);
            EmailTemplateInfo templateInfo = EmailTemplateProvider.GetEmailTemplate(templateName, SiteID);
            if (templateInfo != null)
            {
                EditedObject = templateInfo;
            }

            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master";
        }

        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (isDialog)
        {
            RegisterEscScript();
            RegisterModalPageScripts();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        InitAttachmentAction();
    }

    #endregion


    #region "Custom methods"

    protected void InitAttachmentAction()
    {
        EmailTemplateInfo emailTemplate = (EmailTemplateInfo)EditedObject;

        if ((emailTemplate != null) && (emailTemplate.TemplateID > 0))
        {
            int siteId = emailTemplate.TemplateSiteID;

            // Get number of attachments
            InfoDataSet<MetaFileInfo> ds = MetaFileInfoProvider.GetMetaFiles(emailTemplate.TemplateID, EmailTemplateInfo.OBJECT_TYPE, ObjectAttachmentsCategories.TEMPLATE,
                siteId > 0 ? "MetaFileSiteID=" + siteId : "MetaFileSiteID IS NULL", null, "MetafileID", -1);
            int attachCount = ds.Items.Count;

            // Register attachments count update module
            ScriptHelper.RegisterModule(Page, "CMS/AttachmentsCountUpdater", new { Selector = "." + mAttachmentsActionClass, Text = ResHelper.GetString("general.attachments") });

            // Register dialog scripts
            ScriptHelper.RegisterDialogScript(Page);

            // Prepare metafile dialog URL
            string metaFileDialogUrl = ResolveUrl(@"~/CMSModules/AdminControls/Controls/MetaFiles/MetaFileDialog.aspx");
            string query = String.Format("?objectid={0}&objecttype={1}&siteid={2}", emailTemplate.TemplateID, EmailTemplateInfo.OBJECT_TYPE, siteId);
            metaFileDialogUrl += String.Format("{0}&category={1}&hash={2}", query, ObjectAttachmentsCategories.TEMPLATE, QueryHelper.GetHash(query));

            ObjectEditMenu menu = (ObjectEditMenu)ControlsHelper.GetChildControl(Page, typeof(ObjectEditMenu));
            if (menu != null)
            {
                menu.AddExtraAction(new HeaderAction()
                {
                    Text = GetString("general.attachments") + ((attachCount > 0) ? string.Format(" ({0})", attachCount) : String.Empty),
                    OnClientClick = String.Format(@"if (modalDialog) {{modalDialog('{0}', 'Attachments', '700', '500');}}", metaFileDialogUrl) + " return false;",
                    Enabled = menu.ShowCheckIn || !SynchronizationHelper.UseCheckinCheckout,
                    CssClass = mAttachmentsActionClass
                });
            }
        }
    }

    #endregion
}
