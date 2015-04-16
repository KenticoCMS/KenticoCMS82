using System;
using System.Linq;

using CMS.UIControls;
using CMS.Helpers;
using CMS.Reporting;
using CMS.EmailEngine;
using CMS.SiteProvider;
using CMS.MacroEngine;
using CMS.DataEngine;

public partial class CMSModules_Reporting_CMSPages_Unsubscribe : CMSPage
{
    #region "Variables"

    ReportSubscriptionInfo rsi = null;
    ReportInfo ri = null;
    String email = String.Empty;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        email = QueryHelper.GetString(Server.UrlDecode("email"), String.Empty);
        Guid subscriptionGUID = QueryHelper.GetGuid("guid", Guid.Empty);
        rsi = ReportSubscriptionInfoProvider.GetReportSubscriptionInfo(subscriptionGUID);
        if (rsi != null)
        {
            ri = ReportInfoProvider.GetReportInfo(rsi.ReportSubscriptionReportID);
            if (ri != null)
            {
                // Set info label based by subscription's report
                lblInfo.Text = String.Format(GetString("reportsubscription.unsubscription.info"), email, ri.ReportDisplayName);
            }
        }
        else
        {
            ShowError(GetString("reportsubscription.notfound"));
            pnlInfo.Visible = false;
            btnUnsubscribe.Enabled = false;
        }

        btnUnsubscribe.Text = GetString("reportsubscription.unsubscribe");
        Title = GetString("reportsubscription.unsubscribe.title");

        PageTitle.TitleText = GetString("reportsubscription.unsubscribe.title");
    }


    protected void btnUnsubscribe_click(object sender, EventArgs ea)
    {
        // Validate email
        if (String.IsNullOrEmpty(email) || !ValidationHelper.IsEmail(email))
        {
            ShowError(GetString("om.contact.enteremail"));
            return;
        }

        // Delete subscription if email is valid
        if (rsi != null)
        {
            if (rsi.ReportSubscriptionEmail.Trim() != email)
            {
                ShowError(GetString("reportsubscription.emailnotmatch"));
                return;
            }

            ReportSubscriptionInfoProvider.DeleteReportSubscriptionInfo(rsi.ReportSubscriptionID);
            ShowInformation(GetString("reportsubscription.unsubscription.success"));
            btnUnsubscribe.Visible = false;
            pnlInfo.Visible = false;

            // Send info about successful unsubscription to set email
            String siteName = SiteContext.CurrentSiteName;
            EmailTemplateInfo eti = EmailTemplateProvider.GetEmailTemplate("Reporting_Unsubscription_template", siteName);
            if (eti != null)
            {
                // Create email
                EmailMessage em = new EmailMessage();
                em.EmailFormat = EmailFormatEnum.Default;
                em.From = String.IsNullOrEmpty(eti.TemplateFrom) ? EmailHelper.Settings.NotificationsSenderAddress(siteName) : eti.TemplateFrom;
                em.Recipients = rsi.ReportSubscriptionEmail;
                em.Subject = eti.TemplateSubject;
                em.BccRecipients = eti.TemplateBcc;
                em.CcRecipients = eti.TemplateCc;

                MacroResolver resolver = ReportSubscriptionSender.CreateSubscriptionMacroResolver(ri, rsi, SiteContext.CurrentSite, em.Recipients);

                em.Body = resolver.ResolveMacros(eti.TemplateText);
                em.PlainTextBody = resolver.ResolveMacros(eti.TemplatePlainText);

                EmailHelper.ResolveMetaFileImages(em, eti.TemplateID, EmailTemplateInfo.OBJECT_TYPE, ObjectAttachmentsCategories.TEMPLATE);

                EmailSender.SendEmail(siteName, em);
            }
        }
    }

    #endregion
}
