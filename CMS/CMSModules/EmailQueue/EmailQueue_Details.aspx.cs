using System;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.UI;

using CMS.Base;
using CMS.DataEngine;
using CMS.EmailEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_EmailQueue_EmailQueue_Details : CMSModalGlobalAdminPage
{
    #region "Protected variables"

    protected int emailId;


    protected int prevId;


    protected int nextId;


    protected Hashtable mParameters;

    #endregion


    #region "Properties"

    /// <summary>
    /// Hashtable containing dialog parameters.
    /// </summary>
    private Hashtable Parameters
    {
        get
        {
            if (mParameters == null)
            {
                string identifier = QueryHelper.GetString("params", null);
                mParameters = (Hashtable)WindowHelper.GetItem(identifier);
            }
            return mParameters;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!QueryHelper.ValidateHash("hash", "emailid") || Parameters == null)
        {
            return;
        }

        PageTitle.TitleText = GetString("emailqueue.details.title");
        // Get the ORDER BY column and starting event ID
        string orderBy = DataHelper.GetNotEmpty(Parameters["orderby"], "EmailID DESC");
        if (orderBy.IndexOfCSafe(';') >= 0)
        {
            orderBy = "EmailID DESC"; // ORDER BY with semicolon is considered to be dangerous
        }
        string whereCondition = ValidationHelper.GetString(Parameters["where"], string.Empty);

        // Get e-mail ID from query string
        emailId = QueryHelper.GetInteger("emailid", 0);

        if (!RequestHelper.IsPostBack())
        {
            LoadData();
        }

        // Initialize next/previous buttons
        int[] prevNext = EmailInfoProvider.GetPreviousNext(emailId, whereCondition, orderBy);
        if (prevNext != null)
        {
            prevId = prevNext[0];
            nextId = prevNext[1];

            btnPrevious.Enabled = (prevId != 0);
            btnNext.Enabled = (nextId != 0);

            btnPrevious.Click += btnPrevious_Click;
            btnNext.Click += btnNext_Click;
        }
    }

    #endregion


    #region "Button handling"

    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        // Redirect to previous
        URLHelper.Redirect(URLHelper.UpdateParameterInUrl(RequestContext.CurrentURL, "emailId", prevId.ToString()));
    }


    protected void btnNext_Click(object sender, EventArgs e)
    {
        // Redirect to next
        URLHelper.Redirect(URLHelper.UpdateParameterInUrl(RequestContext.CurrentURL, "emailId", nextId.ToString()));
    }

    #endregion


    #region "Protected methods"

    /// <summary>
    /// Loads data of specific e-mail from DB.
    /// </summary>
    protected void LoadData()
    {
        if (emailId <= 0)
        {
            return;
        }

        // Get specific e-mail
        EmailInfo ei = EmailInfoProvider.GetEmailInfo(emailId);        

        if (ei == null)
        {
            plcDetails.Visible = false;
            ShowInformation(GetString("emailqueue.details.emailalreadysent"));
            return;
        }

        EditedObject = ei;

        lblFromValue.Text = HTMLHelper.HTMLEncode(ei.EmailFrom);

        if (!ei.EmailIsMass)
        {
            lblToValue.Text = HTMLHelper.HTMLEncode(ei.EmailTo);
        }
        else
        {
            lblToValue.Text = GetString("emailqueue.detail.multiplerecipients");
        }

        lblCcValue.Text = HTMLHelper.HTMLEncode(ei.EmailCc);
        lblBccValue.Text = HTMLHelper.HTMLEncode(ei.EmailBcc);
        lblSubjectValue.Text = HTMLHelper.HTMLEncode(ei.EmailSubject);

        string body = null;

        if (string.IsNullOrEmpty(ei.EmailPlainTextBody))
        {
            body = GetHTMLBody(ei);
        }
        else
        {
            body = GetPlainTextBody(ei);
        }

        // Show/hide send result message
        if (!string.IsNullOrEmpty(ei.EmailLastSendResult))
        {
            lblErrorMessageValue.Text = HTMLHelper.HTMLEncode(ei.EmailLastSendResult).Replace("\r\n", "<br />").Replace("\n", "<br />");
            plcErrorMessage.Visible = true;
        }
        else
        {
            plcErrorMessage.Visible = false;
        }

        GetAttachments();
    }


    /// <summary>
    /// Gets the HTML body of the e-mail message.
    /// </summary>
    /// <param name="ei">The e-mail message object</param>
    /// <returns>HTML body</returns>
    private string GetHTMLBody(EmailInfo ei)
    {
        string body = ei.EmailBody;

        // Regular expression to search the tracking image in HTML code
        Regex regExp = RegexHelper.GetRegex("(src=\"[^\"]+Track.ashx)\\?[^\"]*", true);
        Match matchTrack = regExp.Match(body);
        if (matchTrack.Success && (matchTrack.Groups.Count > 0))
        {
            // Remove parameters from tracking image URL so the statistics are not influenced by e-mail previews
            body = regExp.Replace(body, matchTrack.Groups[1].Value);
        }

        // Transform inline attachments back to metafile attachments
        regExp = RegexHelper.GetRegex("src=\"cid:(?<cidCode>[a-z0-9]{32})\"", true);
        body = regExp.Replace(body, match => TransformSrc(match));

        htmlTemplateBody.Visible = true;
        htmlTemplateBody.ResolvedValue = body;
        htmlTemplateBody.AutoDetectLanguage = false;
        htmlTemplateBody.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

        return body;
    }


    /// <summary>
    /// Gets the plain text body of the e-mail message.
    /// </summary>
    /// <param name="ei">The e-mail message object</param>
    /// <returns>Plain-text body</returns>
    private string GetPlainTextBody(EmailInfo ei)
    {
        DiscussionMacroResolver dmh = new DiscussionMacroResolver { ResolveToPlainText = true };
        string body = dmh.ResolveMacros(ei.EmailPlainTextBody);

        body = HTMLHelper.HTMLEncode(body);

        lblBodyValue.Visible = true;

        // Replace line breaks with br tags and modify discussion macros
        lblBodyValue.Text = DiscussionMacroResolver.RemoveTags(HTMLHelper.HTMLEncodeLineBreaks(body));

        return body;
    }


    /// <summary>
    /// Gets the attachments for the specified e-mail message.
    /// </summary>
    private void GetAttachments()
    {
        // Get basic info about all attachments attached to current e-mail
        DataSet ds = EmailAttachmentInfoProvider.GetEmailAttachmentInfos(emailId, null, -1, "AttachmentID, AttachmentName, AttachmentSize");
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            plcAttachments.Visible = true;
            if (ds.Tables.Count > 0)
            {
                int i = 0;
                EmailAttachmentInfo eai = null;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (i > 0)
                    {
                        pnlAttachmentsList.Controls.Add(new LiteralControl("<br />"));
                    }
                    eai = new EmailAttachmentInfo(dr);
                    pnlAttachmentsList.Controls.Add(new LiteralControl(HTMLHelper.HTMLEncode(eai.AttachmentName) + "&nbsp;(" + DataHelper.GetSizeString(eai.AttachmentSize) + ")"));
                    i++;
                }
            }
        }
        else
        {
            plcAttachments.Visible = false;
        }
    }


    /// <summary>
    /// Transforms inline attachment source back to metafile attachment source.
    /// </summary>
    /// <param name="match">Regex match result for inline attachment source</param>
    /// <returns>MetaFile attachment source</returns>
    private static string TransformSrc(Match match)
    {
        if (match.Groups.Count > 0)
        {
            // Get content ID (metafile GUID without '-') and make GUID of it
            string cidCode = match.Groups["cidCode"].Value;
            Guid mfGuid = (cidCode.Length == 32) ? ValidationHelper.GetGuid(cidCode.Insert(20, "-").Insert(16, "-").Insert(12, "-").Insert(8, "-"), Guid.Empty) : Guid.Empty;

            if (mfGuid != Guid.Empty)
            {
                // Get metafile by GUID
                MetaFileInfo mfi = MetaFileInfoProvider.GetMetaFileInfo(mfGuid, null, false);
                if (mfi != null)
                {
                    SiteInfo site = SiteInfoProvider.GetSiteInfo((mfi.MetaFileSiteID > 0) ? mfi.MetaFileSiteID : SiteContext.CurrentSiteID);
                    if (site !=null)
                    {
                        // return metafile source
                        return "src=\"" + URLHelper.GetAbsoluteUrl("~/CMSPages/GetMetaFile.aspx?fileguid=" + mfGuid, site.DomainName) + "\"";
                    }
                }
            }
        }

        return match.Value;
    }

    #endregion
}