using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DocumentEngine;
using CMS.UIControls;

public partial class CMSModules_Content_Attachments_Dialogs_MetaDataEditor : CMSModalPage
{
    #region "Variables"

    private new string mCurrentSiteName = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns the site name from query string 'sitename' or 'siteid' if present, otherwise SiteContext.CurrentSiteName.
    /// </summary>
    protected new string CurrentSiteName
    {
        get
        {
            if (mCurrentSiteName == null)
            {
                mCurrentSiteName = QueryHelper.GetString("sitename", SiteContext.CurrentSiteName);

                int siteId = QueryHelper.GetInteger("siteid", 0);

                SiteInfo site = SiteInfoProvider.GetSiteInfo(siteId);
                if (site != null)
                {
                    mCurrentSiteName = site.SiteName;
                }
            }
            return mCurrentSiteName;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize modal page
        RegisterEscScript();

        if (QueryHelper.ValidateHash("hash"))
        {
            string title = GetString("general.editmetadata");
            Page.Title = title;
            PageTitle.TitleText = title;
            // Default image
            Save += (s, ea) =>
            {
                if (metaDataEditor.SaveMetadata())
                {
                    ltlScript.Text += ScriptHelper.GetScript("CloseDialog();");
                }
            };

            AddNoCacheTag();

            // Set metadata editor properties
            metaDataEditor.ObjectGuid = QueryHelper.GetGuid("attachmentguid", Guid.Empty);
            metaDataEditor.ObjectType = AttachmentInfo.OBJECT_TYPE;
            metaDataEditor.ExternalControlID = QueryHelper.GetText("clientid", null);
            metaDataEditor.VersionHistoryID = QueryHelper.GetInteger("versionhistoryid", 0);
            metaDataEditor.SiteName = CurrentSiteName;
        }
        else
        {
            // Hide all controls
            metaDataEditor.Visible = false;

            string url = ResolveUrl("~/CMSMessages/Error.aspx?title=" + GetString("dialogs.badhashtitle") + "&text=" + GetString("dialogs.badhashtext") + "&cancel=1");
            ltlScript.Text = ScriptHelper.GetScript("window.location = '" + url + "';");
        }
    }

    #endregion
}