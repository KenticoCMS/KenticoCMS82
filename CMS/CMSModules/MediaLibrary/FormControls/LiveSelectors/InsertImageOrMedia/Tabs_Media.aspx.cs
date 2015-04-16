using System;
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

public partial class CMSModules_MediaLibrary_FormControls_LiveSelectors_InsertImageOrMedia_Tabs_Media : CMSLiveModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.ValidateHash("hash"))
        {
            // Check site availability
            if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.MediaLibrary", SiteContext.CurrentSiteName))
            {
                RedirectToResourceNotAvailableOnSite("CMS.MediaLibrary");
            }

            string output = QueryHelper.GetString("output", "");

            bool checkUI = ValidationHelper.GetBoolean(SettingsHelper.AppSettings["CKEditor:PersonalizeToolbarOnLiveSite"], false);
            if ((output == "copy") || (output == "move") || (output == "relationship") || (output == "selectpath"))
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
                }

                if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.MediaDialog", "MediaLibrariesTab"))
                {
                    errorMessage = "MediaLibrariesTab";
                }

                if (errorMessage != "")
                {
                    RedirectToUIElementAccessDenied("CMS.MediaDialog", errorMessage);
                }
            }

            ScriptHelper.RegisterJQuery(Page);
            CMSDialogHelper.RegisterDialogHelper(Page);

            linkMedia.InitFromQueryString();
        }
        else
        {
            linkMedia.StopProcessing = true;
        }
    }
}