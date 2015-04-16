using System;
using System.Web.UI;

using CMS;
using CMS.Controls;
using CMS.DataEngine;
using CMS.EmailEngine;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.FormControls;
using CMS.Helpers;
using CMS.Membership;
using CMS.Synchronization;

[assembly: RegisterCustomClass("EmailTemplateEditExtender", typeof(EmailTemplateEditExtender))]

/// <summary>
/// Email template UIForm extender
/// </summary>
public class EmailTemplateEditExtender : ControlExtender<UIForm>
{
    private const string mAttachmentsActionClass = "attachments-header-action";


    public override void OnInit()
    {
        Control.Page.Load += Page_Load;
        Control.Page.PreRender += Page_PreRender;
    }


    private void Page_Load(object sender, EventArgs e)
    {
        var typeValue = Control.GetFieldValue("EmailTemplateType");
        if (typeValue != null)
        {
            SetProperty("EmailTemplateSubject", typeValue);
            SetProperty("EmailTemplateText", typeValue);
            SetProperty("EmailTemplatePlainText", typeValue);
        }
    }


    private void Page_PreRender(object sender, EventArgs e)
    {
        if (!Control.IsInsertMode)
        {
            InitAttachmentAction();
        }
    }


    private void SetProperty(string fieldName, object value)
    {
        var control = Control.FieldControls[fieldName];
        if (control != null)
        {
            control.SetValue("ResolverName", value);
        }
    }


    private void InitAttachmentAction()
    {
        EmailTemplateInfo emailTemplate = Control.EditedObject as EmailTemplateInfo;

        if ((emailTemplate != null) && (emailTemplate.TemplateID > 0))
        {
            int siteId = emailTemplate.TemplateSiteID;
            Page page = Control.Page;

            // Get number of attachments
            InfoDataSet<MetaFileInfo> ds = MetaFileInfoProvider.GetMetaFiles(emailTemplate.TemplateID, EmailTemplateInfo.OBJECT_TYPE, ObjectAttachmentsCategories.TEMPLATE,
                siteId > 0 ? "MetaFileSiteID=" + siteId : "MetaFileSiteID IS NULL", null, "MetafileID", -1);
            int attachCount = ds.Items.Count;

            // Register attachments count update module
            ScriptHelper.RegisterModule(page, "CMS/AttachmentsCountUpdater", new { Selector = "." + mAttachmentsActionClass, Text = ResHelper.GetString("general.attachments") });

            // Register dialog scripts
            ScriptHelper.RegisterDialogScript(page);

            // Prepare metafile dialog URL
            string metaFileDialogUrl = URLHelper.ResolveUrl(@"~/CMSModules/AdminControls/Controls/MetaFiles/MetaFileDialog.aspx");
            string query = String.Format("?objectid={0}&objecttype={1}&siteid={2}", emailTemplate.TemplateID, EmailTemplateInfo.OBJECT_TYPE, siteId);
            metaFileDialogUrl += String.Format("{0}&category={1}&hash={2}", query, ObjectAttachmentsCategories.TEMPLATE, QueryHelper.GetHash(query));

            ObjectEditMenu menu = (ObjectEditMenu)ControlsHelper.GetChildControl(page, typeof(ObjectEditMenu));
            if (menu != null)
            {
                menu.AddExtraAction(new HeaderAction()
                {
                    Text = ResHelper.GetString("general.attachments") + ((attachCount > 0) ? " (" + attachCount.ToString() + ")" : String.Empty),
                    OnClientClick = String.Format(@"if (modalDialog) {{modalDialog('{0}', 'Attachments', '700', '500');}}", metaFileDialogUrl) + " return false;",
                    Enabled = !SynchronizationHelper.UseCheckinCheckout || emailTemplate.Generalized.IsCheckedOutByUser(MembershipContext.AuthenticatedUser),
                    CssClass = mAttachmentsActionClass
                });
            }
        }
    }
}