using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.Globalization;

public partial class CMSModules_Content_CMSDesk_Properties_Languages : CMSPropertiesPage
{
    #region "Variables"

    private UserInfo currentUserInfo;
    private SiteInfo currentSiteInfo;
    private DateTime defaultLastModification = DateTimeHelper.ZERO_TIME;
    private DateTime defaultLastPublished = DateTimeHelper.ZERO_TIME;
    private string mDefaultSiteCulture;

    #endregion


    #region "Properties"

    /// <summary>
    /// Default culture of the site.
    /// </summary>
    protected string DefaultSiteCulture
    {
        get
        {
            return mDefaultSiteCulture ?? (mDefaultSiteCulture = CultureHelper.GetDefaultCultureCode(SiteContext.CurrentSiteName));
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", "Properties.Languages"))
        {
            RedirectToUIElementAccessDenied("CMS.Content", "Properties.Languages");
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        gridLanguages.FilteredZeroRowsText = GetString("transman.nodocumentculture");
        gridLanguages.OnDataReload += gridDocuments_OnDataReload;
        gridLanguages.OnExternalDataBound += gridLanguages_OnExternalDataBound;
        gridLanguages.ShowActionsMenu = true;
        gridLanguages.Columns = "DocumentName,  Published";
        gridLanguages.AllColumns = SqlHelper.JoinColumnList(ObjectTypeManager.GetColumnNames(PredefinedObjectType.NODE, PredefinedObjectType.DOCUMENTLOCALIZATION));

        pnlContainer.Enabled = !DocumentManager.ProcessingAction;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ScriptHelper.RegisterDialogScript(Page);

        // Check license limitations
        if (!CultureSiteInfoProvider.LicenseVersionCheck())
        {
            LicenseHelper.GetAllAvailableKeys(FeatureEnum.Multilingual);
        }

        // Display document information
        DocumentManager.ShowDocumentInfo(false);

        // Set selected tab
        SetPropertyTab(TAB_LANGUAGES);
    }

    #endregion


    #region "Grid events"

    protected object gridLanguages_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        TranslationStatusEnum status;
        DataRowView drv;
        sourceName = sourceName.ToLowerCSafe();

        if (currentUserInfo == null)
        {
            currentUserInfo = MembershipContext.AuthenticatedUser;
        }
        if (currentSiteInfo == null)
        {
            currentSiteInfo = SiteContext.CurrentSite;
        }

        switch (sourceName)
        {
            case "translate":
            case "action":
                CMSGridActionButton img = sender as CMSGridActionButton;
                if (img != null)
                {
                    if ((sourceName == "translate") &&
                        (!CMS.TranslationServices.TranslationServiceHelper.AnyServiceAvailable(CurrentSiteName) ||
                         !CMS.TranslationServices.TranslationServiceHelper.IsTranslationAllowed(CurrentSiteName)))
                    {
                        img.Visible = false;
                        return img;
                    }

                    GridViewRow gvr = parameter as GridViewRow;
                    if (gvr != null)
                    {
                        // Get datarowview
                        drv = gvr.DataItem as DataRowView;

                        if ((drv != null) && (drv.Row["TranslationStatus"] != DBNull.Value))
                        {
                            // Get translation status
                            status = (TranslationStatusEnum)drv.Row["TranslationStatus"];
                        }
                        else
                        {
                            status = TranslationStatusEnum.NotAvailable;
                        }

                        string culture = (drv != null) ? ValidationHelper.GetString(drv.Row["DocumentCulture"], string.Empty) : string.Empty;

                        // Set appropriate icon
                        if (sourceName == "action")
                        {
                            switch (status)
                            {
                                case TranslationStatusEnum.NotAvailable:
                                    img.IconCssClass = "icon-plus"; 
                                    img.IconStyle = GridIconStyle.Allow;
                                    img.ToolTip = GetString("transman.createnewculture");
                                    break;

                                default:
                                    img.IconCssClass = "icon-edit"; 
                                    img.IconStyle = GridIconStyle.Allow;
                                    img.ToolTip = GetString("transman.editculture");
                                    break;
                            }

                            // Register redirect script
                            if (RequiresDialog)
                            {
                                if ((sourceName == "action") && (status == TranslationStatusEnum.NotAvailable))
                                {
                                    // New culture version
                                    img.OnClientClick = "parent.parent.parent.NewDocumentCulture(" + NodeID + ",'" + culture + "');";
                                }
                                else
                                {
                                    // Existing culture version
                                    ScriptHelper.RegisterWOpenerScript(Page);
                                    string url = ResolveUrl(DocumentURLProvider.GetUrl(Node.NodeAliasPath, Node.DocumentUrlPath, currentSiteInfo.SiteName));
                                    url = URLHelper.AppendQuery(url, "lang=" + culture);
                                    img.OnClientClick = "window.refreshPageOnClose = true; window.reloadPageUrl = " + ScriptHelper.GetString(url) + "; if (wopener.RefreshWOpener) { wopener.RefreshWOpener(window); } CloseDialog();";
                                }
                            }
                            else
                            {
                                img.OnClientClick = "RedirectItem(" + NodeID + ", '" + culture + "');";
                            }

                            img.ID = "imgAction";
                        }
                        else
                        {
                            // Add parameters identifier and hash, encode query string
                            if (LicenseHelper.CheckFeature(RequestContext.CurrentDomain, FeatureEnum.TranslationServices))
                            {
                                string returnUrl = AuthenticationHelper.ResolveDialogUrl("~/CMSModules/Translations/Pages/TranslateDocuments.aspx") + "?targetculture=" + culture + "&dialog=1&nodeid=" + NodeID;
                                returnUrl = URLHelper.AddParameterToUrl(returnUrl, "hash", QueryHelper.GetHash(URLHelper.GetQuery(returnUrl)));

                                img.ToolTip = GetString("transman.translate");
                                img.OnClientClick = "modalDialog('" + returnUrl + "', 'TranslateDocument', 988, 634); ";
                            }
                            else
                            {
                                img.Visible = false;
                            }
                            break;
                        }
                    }
                }
                return img;

            case "translationstatus":
                if (parameter == DBNull.Value)
                {
                    status = TranslationStatusEnum.NotAvailable;
                }
                else
                {
                    status = (TranslationStatusEnum)parameter;
                }
                string statusName = GetString("transman." + status);
                string statusHtml = "<span class=\"" + status + "\">" + statusName + "</span>";
                // .Outdated
                return statusHtml;

            case "documentculturedisplayname":
                drv = (DataRowView)parameter;
                // Add icon
                return UniGridFunctions.DocumentCultureFlag(drv, Page);

            case "documentmodifiedwhen":
            case "documentmodifiedwhentooltip":
                if (string.IsNullOrEmpty(parameter.ToString()))
                {
                    return "-";
                }
                else
                {
                    if (sourceName.EqualsCSafe("documentmodifiedwhen", StringComparison.InvariantCultureIgnoreCase))
                    {
                        DateTime modifiedWhen = ValidationHelper.GetDateTime(parameter, DateTimeHelper.ZERO_TIME);
                        return TimeZoneHelper.ConvertToUserTimeZone(modifiedWhen, true, currentUserInfo, currentSiteInfo);
                    }
                    else
                    {
                        return TimeZoneHelper.GetUTCLongStringOffset(CurrentUser, currentSiteInfo);
                    }
                }

            case "versionnumber":
                if (string.IsNullOrEmpty(parameter.ToString()))
                {
                    return "-";
                }
                break;

            case "documentname":
                if (string.IsNullOrEmpty(parameter.ToString()))
                {
                    parameter = "-";
                }
                return HTMLHelper.HTMLEncode(parameter.ToString());

            case "published":
                bool published = ValidationHelper.GetBoolean(parameter, false);
                if (published)
                {
                    return "<span class=\"DocumentPublishedYes\">" + GetString("General.Yes") + "</span>";
                }
                else
                {
                    return "<span class=\"DocumentPublishedNo\">" + GetString("General.No") + "</span>";
                }
        }
        return parameter;
    }


    protected DataSet gridDocuments_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        string currentSiteName = SiteContext.CurrentSiteName;

        // Check if node is not null
        if (Node != null)
        {
            // Get documents
            int topN = gridLanguages.GridView.PageSize * (gridLanguages.GridView.PageIndex + 1 + gridLanguages.GridView.PagerSettings.PageButtonCount);
            columns = SqlHelper.MergeColumns(SqlHelper.MergeColumns(TreeProvider.SELECTNODES_REQUIRED_COLUMNS, columns), "DocumentModifiedWhen, VersionNumber, DocumentLastPublished, DocumentIsWaitingForTranslation");
            DataSet documentsDS = DocumentHelper.GetDocuments(currentSiteName, Node.NodeAliasPath, TreeProvider.ALL_CULTURES, false, null, null, null, -1, false, topN, columns, Tree);
            DataTable documents = documentsDS.Tables[0];

            if (!DataHelper.DataSourceIsEmpty(documents))
            {
                // Get site cultures
                DataSet allSiteCultures = CultureSiteInfoProvider.GetSiteCultures(currentSiteName).Copy();

                // Rename culture column to enable row transfer
                allSiteCultures.Tables[0].Columns[2].ColumnName = "DocumentCulture";

                // Create where condition for row transfer
                string where = documents.Rows.Cast<DataRow>().Aggregate("DocumentCulture NOT IN (", (current, row) => current + ("'" + SqlHelper.EscapeQuotes(ValidationHelper.GetString(row["DocumentCulture"], string.Empty)) + "',"));
                where = where.TrimEnd(',') + ")";

                // Transfer missing cultures, keep original list of site cultures
                DataHelper.TransferTableRows(documents, allSiteCultures.Copy().Tables[0], where, null);
                DataHelper.EnsureColumn(documents, "DocumentCultureDisplayName", typeof(string));

                // Ensure culture names
                foreach (DataRow cultDR in documents.Rows)
                {
                    string cultureCode = cultDR["DocumentCulture"].ToString();
                    DataRow[] cultureRow = allSiteCultures.Tables[0].Select("DocumentCulture='" + cultureCode + "'");
                    if (cultureRow.Length > 0)
                    {
                        cultDR["DocumentCultureDisplayName"] = cultureRow[0]["CultureName"].ToString();
                    }
                }

                // Ensure default culture to be first
                DataRow[] culturreDRs = documents.Select("DocumentCulture='" + DefaultSiteCulture + "'");
                if (culturreDRs.Length <= 0)
                {
                    throw new Exception("[ReloadData]: Default site culture '" + DefaultSiteCulture + "' is not assigned to the current site.");
                }

                DataRow defaultCultureRow = culturreDRs[0];

                DataRow dr = documents.NewRow();
                dr.ItemArray = defaultCultureRow.ItemArray;
                documents.Rows.InsertAt(dr, 0);
                documents.Rows.Remove(defaultCultureRow);

                // Get last modification date of default culture
                defaultCultureRow = documents.Select("DocumentCulture='" + DefaultSiteCulture + "'")[0];
                defaultLastModification = ValidationHelper.GetDateTime(defaultCultureRow["DocumentModifiedWhen"], DateTimeHelper.ZERO_TIME);
                defaultLastPublished = ValidationHelper.GetDateTime(defaultCultureRow["DocumentLastPublished"], DateTimeHelper.ZERO_TIME);

                // Add column containing translation status
                documents.Columns.Add("TranslationStatus", typeof(TranslationStatusEnum));

                // Get proper translation status and store it to datatable
                foreach (DataRow document in documents.Rows)
                {
                    TranslationStatusEnum status;
                    int documentId = ValidationHelper.GetInteger(document["DocumentID"], 0);
                    if (documentId == 0)
                    {
                        status = TranslationStatusEnum.NotAvailable;
                    }
                    else
                    {
                        string versionNumber = ValidationHelper.GetString(DataHelper.GetDataRowValue(document, "VersionNumber"), null);
                        
                        if (ValidationHelper.GetBoolean(document["DocumentIsWaitingForTranslation"], false))
                        {
                            status = TranslationStatusEnum.WaitingForTranslation;
                        }
                        else
                        {
                            DateTime lastModification;

                            // Check if document is outdated
                            if (versionNumber != null)
                            {
                                lastModification = ValidationHelper.GetDateTime(document["DocumentLastPublished"], DateTimeHelper.ZERO_TIME);
                                status = (lastModification < defaultLastPublished) ? TranslationStatusEnum.Outdated : TranslationStatusEnum.Translated;
                            }
                            else
                            {
                                lastModification = ValidationHelper.GetDateTime(document["DocumentModifiedWhen"], DateTimeHelper.ZERO_TIME);
                                status = (lastModification < defaultLastModification) ? TranslationStatusEnum.Outdated : TranslationStatusEnum.Translated;
                            }
                        }
                    }
                    document["TranslationStatus"] = status;
                }

                // Bind datasource
                DataSet filteredDocuments = documentsDS.Clone();
                DataRow[] filteredDocs = documents.Select(gridLanguages.GetFilter());

                foreach (DataRow row in filteredDocs)
                {
                    filteredDocuments.Tables[0].ImportRow(row);
                }

                return filteredDocuments;
            }
        }

        return null;
    }

    #endregion
}