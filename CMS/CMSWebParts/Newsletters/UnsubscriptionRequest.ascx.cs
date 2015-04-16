using System;
using System.Collections.Generic;
using System.Data;

using CMS.EmailEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.PortalControls;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;

using EmailTemplateProvider = CMS.EmailEngine.EmailTemplateProvider;
using EmailTemplateInfo = CMS.EmailEngine.EmailTemplateInfo;
using CMS.DataEngine;

public partial class CMSWebParts_Newsletters_UnsubscriptionRequest : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets submit button text.
    /// </summary>
    public string ButtonText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ButtonText"), String.Empty);
        }
        set
        {
            SetValue("ButtonText", value);
        }
    }


    /// <summary>
    /// Gets or sets newsletter name.
    /// </summary>
    public string NewsletterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NewsletterName"), null);
        }
        set
        {
            SetValue("NewsletterName", value);
        }
    }


    /// <summary>
    /// Gets or sets info message.
    /// </summary>
    public string InformationText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("InformationText"), null);
        }
        set
        {
            SetValue("InformationText", value);
        }
    }


    /// <summary>
    /// Gets or sets error message.
    /// </summary>
    public string ErrorText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ErrorText"), null);
        }
        set
        {
            SetValue("ErrorText", value);
        }
    }


    /// <summary>
    /// Gets or sets message that will be shown after successful unsubscription.
    /// </summary>
    public string ResultText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ResultText"), null);
        }
        set
        {
            SetValue("ResultText", value);
        }
    }

    #endregion


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Reloads data for partial caching.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            if (!String.IsNullOrEmpty(InformationText))
            {
                lblInfo.Text = InformationText;
                lblInfo.Visible = true;
            }
            else
            {
                lblInfo.Visible = false;
            }

            if (!String.IsNullOrEmpty(ButtonText))
            {
                btnSubmit.Text = ButtonText;
            }
            else
            {
                btnSubmit.Text = GetString("general.ok");
            }
            btnSubmit.Click += new EventHandler(btnSubmit_Click);
        }
    }


    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        // Check email format
        string email = txtEmail.Text.Trim();
        string result = new Validator().IsEmail(email, GetString("unsubscribe.invalidemailformat")).Result;

        if (String.IsNullOrEmpty(result))
        {
            bool requestSent = false;
            int siteId = 0;
            if (SiteContext.CurrentSite != null)
            {
                siteId = SiteContext.CurrentSiteID;
            }

            // Try to get all subscriber infos with given e-mail
            DataSet ds = SubscriberInfoProvider.GetSubscribersFromView(email, siteId);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SubscriberInfo sb = new SubscriberInfo(dr);
                    if ((string.IsNullOrEmpty(sb.SubscriberType) || sb.SubscriberType.EqualsCSafe(PredefinedObjectType.CONTACT, true) || sb.SubscriberType.EqualsCSafe(UserInfo.OBJECT_TYPE, true)))
                    {
                        // Get newsletter information
                        NewsletterInfo news = NewsletterInfoProvider.GetNewsletterInfo(NewsletterName, siteId);
                        if (news != null)
                        {
                            // Get subscription info
                            SubscriberNewsletterInfo sni = SubscriberNewsletterInfoProvider.GetSubscriberNewsletterInfo(sb.SubscriberID, news.NewsletterID);
                            // Send e-mail to subscribed user only
                            if ((sni != null) && (sni.SubscriptionEnabled))
                            {
                                SendUnsubscriptionRequest(sb, news, sni, SiteContext.CurrentSiteName);
                                requestSent = true; // At least one request was sent
                            }
                        }
                    }
                }
            }

            // Unsubscription failed if none confirmation e-mail was sent
            if (!requestSent)
            {
                // Use default error message if none is specified
                if (String.IsNullOrEmpty(ErrorText))
                {
                    result = GetString("unsubscribe.notsubscribed");
                }
                else
                {
                    result = ErrorText;
                }
            }
        }

        // Display error message if set
        if (!string.IsNullOrEmpty(result))
        {
            lblError.Text = result;
            lblError.Visible = true;
        }
        else
        {
            // Display unsubscription confirmation
            lblInfo.Visible = true;
            if (String.IsNullOrEmpty(ResultText))
            {
                // Use default message if none was specified
                lblInfo.Text = GetString("unsubscribe.confirmtext");
            }
            else
            {
                lblInfo.Text = ResultText;
            }
            lblError.Visible = false;
            txtEmail.Visible = false;
            btnSubmit.Visible = false;
        }
    }


    /// <summary>
    /// Creates and sends unsubscription e-mail.
    /// </summary>
    /// <param name="subscriber">Subscriber object</param>
    /// <param name="news">Newsletter object</param>
    /// <param name="subscription">Subscription object</param>
    /// <param name="siteName">Site name</param>
    protected void SendUnsubscriptionRequest(SubscriberInfo subscriber, NewsletterInfo news, SubscriberNewsletterInfo subscription, string siteName)
    {
        // Get global e-mail template with unsubscription request
        EmailTemplateInfo et = EmailTemplateProvider.GetEmailTemplate("newsletter.unsubscriptionrequest", siteName);
        if (et != null)
        {
            // Get subscriber member
            SortedDictionary<int, SubscriberInfo> subscribers = SubscriberInfoProvider.GetSubscribers(subscriber, 1, 0);
            foreach (KeyValuePair<int, SubscriberInfo> item in subscribers)
            {
                // Get 1st subscriber's member
                SubscriberInfo sb = item.Value;

                string body = et.TemplateText;
                string plainBody = et.TemplatePlainText;

                // Resolve newsletter macros (first name, last name etc.)
                IssueHelper ih = new IssueHelper();
                if (ih.LoadDynamicFields(sb, news, subscription, null, false, siteName, null, null, null))
                {
                    body = ih.ResolveDynamicFieldMacros(body, news);
                    plainBody = ih.ResolveDynamicFieldMacros(plainBody, news);
                }

                // Create e-mail
                EmailMessage msg = new EmailMessage();
                msg.EmailFormat = EmailFormatEnum.Default;
                msg.From = EmailHelper.GetSender(et, news.NewsletterSenderEmail);
                msg.Recipients = sb.SubscriberEmail;
                msg.BccRecipients = et.TemplateBcc;
                msg.CcRecipients = et.TemplateCc;
                msg.Subject = ResHelper.LocalizeString(et.TemplateSubject);
                msg.Body = URLHelper.MakeLinksAbsolute(body);
                msg.PlainTextBody = URLHelper.MakeLinksAbsolute(plainBody);

                // Add attachments and send e-mail
                EmailHelper.ResolveMetaFileImages(msg, et.TemplateID, EmailTemplateInfo.OBJECT_TYPE, ObjectAttachmentsCategories.TEMPLATE);

                EmailSender.SendEmail(siteName, msg);
            }
        }
        else
        {
            // Log missing template
            EventLogProvider.LogEvent(EventType.ERROR, "UnsubscriptionRequest", "Unsubscription request e-mail template is missing.");
        }
    }
}