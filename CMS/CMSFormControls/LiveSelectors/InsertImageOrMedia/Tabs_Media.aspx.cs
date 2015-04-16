﻿using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.Modules;

public partial class CMSFormControls_LiveSelectors_InsertImageOrMedia_Tabs_Media : CMSLiveModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string source = QueryHelper.GetString("source", "");
        MediaSourceEnum src = CMSDialogHelper.GetMediaSource(source);

        if (src == MediaSourceEnum.Content)
        {
            // Check site availability
            if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.Content", SiteContext.CurrentSiteName))
            {
                RedirectToResourceNotAvailableOnSite("CMS.Content");
            }
        }

        // Check UIProfile
        string output = QueryHelper.GetString("output", "");
        bool checkUI = ValidationHelper.GetBoolean(SettingsHelper.AppSettings["CKEditor:PersonalizeToolbarOnLiveSite"], false);
        if ((output == "copy") || (output == "move") || (output == "link") || (output == "linkdoc") || (output == "relationship") || (output == "selectpath"))
        {
            checkUI = false;
        }

        if (checkUI)
        {
            string errorMessage = "";

            OutputFormatEnum outputFormat = CMSDialogHelper.GetOutputFormat(output, QueryHelper.GetBoolean("link", false));
            if ((outputFormat == OutputFormatEnum.HTMLLink) && !MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.WYSIWYGEditor", "InsertLink"))
            {
                errorMessage = "InsertLink";
            }
            else if ((outputFormat == OutputFormatEnum.HTMLMedia) && !MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.WYSIWYGEditor", "InsertImageOrMedia"))
            {
                errorMessage = "InsertImageOrMedia";
            }

            if (errorMessage != "")
            {
                RedirectToUIElementAccessDenied("CMS.WYSIWYGEditor", errorMessage);
                return;
            }

            switch (src)
            {
                case MediaSourceEnum.DocumentAttachments:
                case MediaSourceEnum.Attachment:
                    if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.MediaDialog", "AttachmentsTab"))
                    {
                        errorMessage = "AttachmentsTab";
                    }
                    break;

                case MediaSourceEnum.Content:
                    if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.MediaDialog", "ContentTab"))
                    {
                        errorMessage = "ContentTab";
                    }
                    break;
            }
            if (errorMessage != "")
            {
                RedirectToUIElementAccessDenied("CMS.MediaDialog", errorMessage);
                return;
            }
        }

        if (QueryHelper.ValidateHash("hash"))
        {
            ScriptHelper.RegisterJQuery(Page);
            CMSDialogHelper.RegisterDialogHelper(Page);
            ScriptHelper.RegisterStartupScript(Page, typeof(Page), "InitResizers", ScriptHelper.GetScript("InitResizers();"));

            linkMedia.InitFromQueryString();
        }
        else
        {
            linkMedia.StopProcessing = true;
            linkMedia.Visible = false;
            string url = ResolveUrl("~/CMSMessages/Error.aspx?title=" + GetString("dialogs.badhashtitle") + "&text=" + GetString("dialogs.badhashtext") + "&cancel=1");
            ltlScript.Text = ScriptHelper.GetScript("if (window.parent != null) { window.parent.location = '" + url + "' }");
        }
    }
}