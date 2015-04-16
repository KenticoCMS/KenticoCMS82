using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.Localization;
using CMS.PortalEngine;
using CMS.Base;
using CMS.SiteProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.WebAnalytics;

public partial class CMSPages_GetFile : GetFilePage
{
    #region "Advanced settings"

    /// <summary>
    /// Sets to false to disable the client caching.
    /// </summary>
    protected bool useClientCache = true;

    /// <summary>
    /// Sets to 0 if you do not wish to cache large files.
    /// </summary>
    protected int largeFilesCacheMinutes = 1;

    #endregion


    #region "Variables"

    protected CMSOutputAttachment outputAttachment = null;

    protected TreeProvider mTreeProvider = null;
    protected GeneralConnection mConnection = null;

    protected TreeNode node = null;
    protected PageInfo pi = null;

    protected int? mVersionHistoryID = null;
    protected bool mIsLatestVersion = false;
    protected bool? mIsLiveSite = null;
    protected Guid guid = Guid.Empty;
    protected string mCulture = null;
    protected Guid nodeGuid = Guid.Empty;

    protected string aliasPath = null;
    protected string fileName = null;

    protected int latestForDocumentId = 0;
    protected int latestForHistoryId = 0;

    protected bool allowLatestVersion = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the language for current file.
    /// </summary>
    public override string CultureCode
    {
        get
        {
            if (mCulture == null)
            {
                string culture = QueryHelper.GetString(URLHelper.LanguageParameterName, LocalizationContext.PreferredCultureCode);
                if (!CultureSiteInfoProvider.IsCultureAllowed(culture, CurrentSiteName))
                {
                    culture = LocalizationContext.PreferredCultureCode;
                }
                mCulture = culture;
            }

            return mCulture;
        }
    }


    /// <summary>
    /// Tree provider.
    /// </summary>
    public TreeProvider TreeProvider
    {
        get
        {
            return mTreeProvider ?? (mTreeProvider = new TreeProvider());
        }
    }


    /// <summary>
    /// Document version history ID.
    /// </summary>
    public int VersionHistoryID
    {
        get
        {
            if (mVersionHistoryID == null)
            {
                mVersionHistoryID = QueryHelper.GetInteger("versionhistoryid", 0);
            }
            return mVersionHistoryID.Value;
        }
    }


    /// <summary>
    /// Indicates if the file is latest version or comes from version history.
    /// </summary>
    public bool LatestVersion
    {
        get
        {
            return mIsLatestVersion || (VersionHistoryID > 0);
        }
    }


    /// <summary>
    /// Returns true if the process allows cache.
    /// </summary>
    public override bool AllowCache
    {
        get
        {
            if (mAllowCache == null)
            {
                // By default, cache for the files is disabled outside of the live site
                mAllowCache = CacheHelper.AlwaysCacheFiles || IsLiveSite;
            }

            return mAllowCache.Value;
        }
        set
        {
            mAllowCache = value;
        }
    }


    /// <summary>
    /// Cache minutes.
    /// </summary>
    public override int CacheMinutes
    {
        get
        {
            // Use 304 revalidation for non-LiveSite mode, for live side mode keep original values
            if (AllowCache && (PortalContext.ViewMode != ViewModeEnum.LiveSite))
            {
                return 0;
            }
            return base.CacheMinutes;
        }
        set
        {
            base.CacheMinutes = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        DebugHelper.SetContext("GetFile");

        // Load the site name
        LoadSiteName();

        // Check the site
        if (CurrentSiteName == "")
        {
            throw new Exception("[GetFile.aspx]: Site not running.");
        }

        ValidateCulture();

        // Set campaign
        if (IsLiveSite)
        {
            // Store campaign name if present
            string campaign = AnalyticsHelper.CurrentCampaign(CurrentSiteName);
            if (!String.IsNullOrEmpty(campaign) && DocumentContext.CurrentPageInfo != null)
            {
                AnalyticsHelper.SetCampaign(campaign, CurrentSiteName, DocumentContext.CurrentPageInfo.NodeAliasPath);
            }
        }


        int cacheMinutes = CacheMinutes;

        // Try to get data from cache
        using (var cs = new CachedSection<CMSOutputAttachment>(ref outputAttachment, cacheMinutes, true, null, "getfile", CurrentSiteName, GetBaseCacheKey(), Request.QueryString))
        {
            if (cs.LoadData)
            {
                // Store current value and temporary disable caching
                bool cached = cs.Cached;
                cs.Cached = false;

                // Process the file
                ProcessAttachment();

                // Restore cache settings - data were loaded
                cs.Cached = cached;

                if (cs.Cached)
                {
                    // Do not cache if too big file which would be stored in memory
                    if ((outputAttachment != null) &&
                        (outputAttachment.Attachment != null) &&
                        !CacheHelper.CacheImageAllowed(CurrentSiteName, outputAttachment.Attachment.AttachmentSize) &&
                        !AttachmentInfoProvider.StoreFilesInFileSystem(CurrentSiteName))
                    {
                        cacheMinutes = largeFilesCacheMinutes;
                    }

                    if (cacheMinutes > 0)
                    {
                        // Prepare the cache dependency
                        CMSCacheDependency cd = null;
                        if (outputAttachment != null)
                        {
                            List<string> dependencies = new List<string>
                            {
                                                            "node|" + CurrentSiteName.ToLowerCSafe() + "|" + outputAttachment.AliasPath.ToLowerCSafe(),
                                                            ""
                                                        };

                            // Do not cache if too big file which would be stored in memory
                            if (outputAttachment.Attachment != null)
                            {
                                if (!CacheHelper.CacheImageAllowed(CurrentSiteName, outputAttachment.Attachment.AttachmentSize) && !AttachmentInfoProvider.StoreFilesInFileSystem(CurrentSiteName))
                                {
                                    cacheMinutes = largeFilesCacheMinutes;
                                }

                                dependencies.Add("attachment|" + outputAttachment.Attachment.AttachmentGUID.ToString().ToLowerCSafe());
                            }

                            cd = GetCacheDependency(dependencies);
                        }

                        if (cd == null)
                        {
                            // Set default dependency
                            if (guid != Guid.Empty)
                            {
                                // By attachment GUID
                                cd = CacheHelper.GetCacheDependency(new[] { "attachment|" + guid.ToString().ToLowerCSafe() });
                            }
                            else if (nodeGuid != Guid.Empty)
                            {
                                // By node GUID
                                cd = CacheHelper.GetCacheDependency(new[] { "nodeguid|" + CurrentSiteName.ToLowerCSafe() + "|" + nodeGuid.ToString().ToLowerCSafe() });
                            }
                            else if (aliasPath != null)
                            {
                                // By node alias path
                                cd = CacheHelper.GetCacheDependency(new[] { "node|" + CurrentSiteName.ToLowerCSafe() + "|" + aliasPath.ToLowerCSafe() });
                            }
                        }

                        cs.CacheDependency = cd;
                    }

                    // Cache the data
                    cs.CacheMinutes = cacheMinutes;
                }

                cs.Data = outputAttachment;
            }
        }

        // Do not cache images in the browser if cache is not allowed
        if (LatestVersion)
        {
            useClientCache = false;
        }

        // Send the data
        SendFile(outputAttachment);

        DebugHelper.ReleaseContext();
    }


    /// <summary>
    /// Sends the given file within response.
    /// </summary>
    /// <param name="attachment">File to send</param>
    protected void SendFile(CMSOutputAttachment attachment)
    {
        // Clear response.
        CookieHelper.ClearResponseCookies();
        Response.Clear();

        // Set the revalidation
        SetRevalidation();

        // Send the file
        if ((attachment != null) && attachment.IsValid)
        {
            // Redirect if the file should be redirected
            if (attachment.RedirectTo != "")
            {
                // Log hit or activity before redirecting
                LogEvent(attachment);

                if (StorageHelper.IsExternalStorage(attachment.RedirectTo))
                {
                    string url = File.GetFileUrl(attachment.RedirectTo, CurrentSiteName);
                    if (!string.IsNullOrEmpty(url))
                    {
                        URLHelper.RedirectPermanent(url, CurrentSiteName);
                    }
                }

                URLHelper.RedirectPermanent(attachment.RedirectTo, CurrentSiteName);
                return;
            }

            // Check authentication if secured file
            var secured = attachment.IsSecured;
            if (secured)
            {
                PageSecurityHelper.CheckSecured(CurrentSiteName, ViewMode);
            }

            string etag = GetFileETag(attachment);

            // Client caching - only on the live site
            if (useClientCache && AllowCache && AllowClientCache && ETagsMatch(etag, attachment.LastModified))
            {
                // Set correct response content type
                SetResponseContentType(attachment);

                // Set the file time stamps to allow client caching
                SetTimeStamps(attachment.LastModified, !secured);

                RespondNotModified(etag, !secured);
                return;
            }

            // If physical file not present, try to load
            if (attachment.PhysicalFile == null)
            {
                EnsurePhysicalFile(outputAttachment);
            }

            // If the output data should be cached, return the output data
            bool cacheOutputData = false;
            if (attachment.Attachment != null)
            {
                // Cache data if allowed
                if (!LatestVersion && (CacheMinutes > 0))
                {
                    cacheOutputData = CacheHelper.CacheImageAllowed(CurrentSiteName, attachment.Attachment.AttachmentSize);
                }
            }

            // Ensure the file data if physical file not present
            if (!attachment.DataLoaded && (attachment.PhysicalFile == ""))
            {
                byte[] cachedData = GetCachedOutputData();
                if (attachment.EnsureData(cachedData))
                {
                    if ((cachedData == null) && cacheOutputData)
                    {
                        SaveOutputDataToCache(attachment.OutputData, GetOutputDataDependency(attachment.Attachment));
                    }
                }
            }

            // Send the file
            if ((attachment.OutputData != null) || (attachment.PhysicalFile != ""))
            {
                // Set correct response content type
                SetResponseContentType(attachment);

                if (attachment.Attachment != null)
                {
                    string extension = attachment.Attachment.AttachmentExtension;
                    SetDisposition(attachment.Attachment.AttachmentName, extension);

                    // Setup Etag property
                    ETag = etag;

                    // Set if resumable downloads should be supported
                    AcceptRange = !IsExtensionExcludedFromRanges(extension);
                }

                if (useClientCache && AllowCache)
                {
                    // Set the file time stamps to allow client caching
                    SetTimeStamps(attachment.LastModified);

                    Response.Cache.SetETag(etag);
                }
                else
                {
                    SetCacheability();
                }

                // Log hit or activity
                LogEvent(attachment);
                // Add the file data
                if ((attachment.PhysicalFile != "") && (attachment.OutputData == null))
                {
                    if (!File.Exists(attachment.PhysicalFile))
                    {
                        // File doesn't exist
                        NotFound();
                    }
                    else
                    {
                        // Stream the file from the file system
                        attachment.OutputData = WriteFile(attachment.PhysicalFile, cacheOutputData);
                    }
                }
                else
                {
                    // Use output data of the file in memory if present
                    WriteBytes(attachment.OutputData);
                }
            }
            else
            {
                NotFound();
            }
        }
        else
        {
            NotFound();
        }

        CompleteRequest();
    }


    /// <summary>
    /// Sets content type of the response based on file MIME type
    /// </summary>
    /// <param name="attachment">Output file</param>
    private void SetResponseContentType(CMSOutputAttachment attachment)
    {
        string mimeType = attachment.MimeType;
        if (attachment.Attachment != null)
        {
            string extension = attachment.Attachment.AttachmentExtension;
            switch (extension.ToLowerCSafe())
            {
                case ".flv":
                    // Correct MIME type
                    mimeType = "video/x-flv";
                    break;
            }
        }

        // Set content type
        Response.ContentType = mimeType;
    }


    /// <summary>
    /// Gets the file ETag
    /// </summary>
    /// <param name="attachment">File</param>
    private static string GetFileETag(CMSOutputAttachment attachment)
    {
        // Prepare etag
        string etag = attachment.CultureCode.ToLowerCSafe();
        if (attachment.Attachment != null)
        {
            etag += "|" + attachment.Attachment.AttachmentGUID + "|" + attachment.Attachment.AttachmentLastModified.ToUniversalTime();
        }

        if (attachment.IsSecured)
        {
            // For secured files, add user name to etag
            etag += "|" + RequestContext.UserName;
        }

        etag += "|" + PortalContext.ViewMode;

        // Put etag into ""
        etag = "\"" + etag + "\"";

        return etag;
    }


    /// <summary>
    /// Processes the attachment.
    /// </summary>
    protected void ProcessAttachment()
    {
        outputAttachment = null;

        // If guid given, process the attachment
        guid = QueryHelper.GetGuid("guid", Guid.Empty);
        allowLatestVersion = CheckAllowLatestVersion();

        if (guid != Guid.Empty)
        {
            // Check version
            if (VersionHistoryID > 0)
            {
                ProcessFile(guid, VersionHistoryID);
            }
            else
            {
                ProcessFile(guid);
            }
        }
        else
        {
            // Get by node GUID
            nodeGuid = QueryHelper.GetGuid("nodeguid", Guid.Empty);
            if (nodeGuid != Guid.Empty)
            {
                // If node GUID given, process the file
                ProcessNode(nodeGuid);
            }
            else
            {
                // Get by alias path and file name
                aliasPath = QueryHelper.GetString("aliaspath", null);
                fileName = QueryHelper.GetString("filename", null);
                if (aliasPath != null)
                {
                    ProcessNode(aliasPath, fileName);
                }
            }
        }

        // If chset specified, do not cache
        string chset = QueryHelper.GetString("chset", null);
        if (chset != null)
        {
            mIsLatestVersion = true;
        }
    }


    /// <summary>
    /// Processes the specified file and returns the data to the output stream.
    /// </summary>
    /// <param name="attachmentGuid">Attachment guid</param>
    protected void ProcessFile(Guid attachmentGuid)
    {
        AttachmentInfo atInfo;

        bool requiresData = true;

        // Check if it is necessary to load the file data
        if (useClientCache && IsLiveSite && AllowClientCache)
        {
            // If possibly cached by client, do not load data (may not be sent)
            string ifModifiedString = Request.Headers["If-Modified-Since"];
            if (ifModifiedString != null)
            {
                requiresData = false;
            }
        }

        // If output data available from cache, do not require loading the data
        byte[] cachedData = GetCachedOutputData();
        if (cachedData != null)
        {
            requiresData = false;
        }

        // Get AttachmentInfo object
        if (!IsLiveSite)
        {
            // Not livesite mode - get latest version
            if (node != null)
            {
                atInfo = DocumentHelper.GetAttachment(node, attachmentGuid, TreeProvider);
            }
            else
            {
                atInfo = DocumentHelper.GetAttachment(attachmentGuid, TreeProvider, CurrentSiteName);
            }
        }
        else
        {
            if (!requiresData || AttachmentInfoProvider.StoreFilesInFileSystem(CurrentSiteName))
            {
                // Do not require data from DB - Not necessary or available from file system
                atInfo = AttachmentInfoProvider.GetAttachmentInfoWithoutBinary(attachmentGuid, CurrentSiteName);
            }
            else
            {
                // Require data from DB - Stored in DB
                atInfo = AttachmentInfoProvider.GetAttachmentInfo(attachmentGuid, CurrentSiteName);
            }

            // If attachment not found, 
            if (allowLatestVersion && ((atInfo == null) || (latestForHistoryId > 0) || (atInfo.AttachmentDocumentID == latestForDocumentId)))
            {
                // Get latest version
                if (node != null)
                {
                    atInfo = DocumentHelper.GetAttachment(node, attachmentGuid, TreeProvider);
                }
                else
                {
                    atInfo = DocumentHelper.GetAttachment(attachmentGuid, TreeProvider, CurrentSiteName);
                }

                // If not attachment for the required document, do not return
                if ((atInfo.AttachmentDocumentID != latestForDocumentId) && (latestForHistoryId == 0))
                {
                    atInfo = null;
                }
                else
                {
                    mIsLatestVersion = true;
                }
            }
        }

        if (atInfo != null)
        {
            // Temporary attachment is always latest version
            if (atInfo.AttachmentFormGUID != Guid.Empty)
            {
                mIsLatestVersion = true;
            }

            // Check if current mimetype is allowed
            if (!CheckRequiredMimeType(atInfo))
            {
                return;
            }

            bool checkPublishedFiles = AttachmentInfoProvider.CheckPublishedFiles(CurrentSiteName);
            bool checkFilesPermissions = AttachmentInfoProvider.CheckFilesPermissions(CurrentSiteName);

            // Get the document node
            if ((node == null) && (checkPublishedFiles || checkFilesPermissions))
            {
                // Try to get data from cache
                using (var cs = new CachedSection<TreeNode>(ref node, CacheMinutes, !allowLatestVersion, null, "getfilenodebydocumentid", atInfo.AttachmentDocumentID))
                {
                    if (cs.LoadData)
                    {
                        // Get the document
                        node = TreeProvider.SelectSingleDocument(atInfo.AttachmentDocumentID, false);

                        // Cache the document
                        CacheNode(cs, node);
                    }
                }
            }

            bool secured = false;
            if ((node != null) && checkFilesPermissions)
            {
                secured = (node.IsSecuredNode == 1);

                // Check secured pages
                if (secured)
                {
                    PageSecurityHelper.CheckSecuredAreas(CurrentSiteName, false, ViewMode);
                }
                if (node.RequiresSSL == 1)
                {
                    PageSecurityHelper.RequestSecurePage(false, node.RequiresSSL, ViewMode, CurrentSiteName);
                }

                // Check permissions
                bool checkPermissions = false;
                switch (PageSecurityHelper.CheckPagePermissions(CurrentSiteName))
                {
                    case PageLocationEnum.All:
                        checkPermissions = true;
                        break;

                    case PageLocationEnum.SecuredAreas:
                        checkPermissions = secured;
                        break;
                }

                // Check the read permission for the page
                if (checkPermissions)
                {
                    if (CurrentUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.Read) == AuthorizationResultEnum.Denied)
                    {
                        URLHelper.Redirect(PageSecurityHelper.AccessDeniedPageURL(CurrentSiteName));
                    }
                }
            }


            bool resizeImage = (ImageHelper.IsImage(atInfo.AttachmentExtension) && AttachmentInfoProvider.CanResizeImage(atInfo, Width, Height, MaxSideSize));

            // If the file should be redirected, redirect the file
            if (!mIsLatestVersion && IsLiveSite && SettingsKeyInfoProvider.GetBoolValue(CurrentSiteName + ".CMSRedirectFilesToDisk"))
            {
                if (AttachmentInfoProvider.StoreFilesInFileSystem(CurrentSiteName))
                {
                    string path;
                    if (!resizeImage)
                    {
                        path = AttachmentInfoProvider.GetFilePhysicalURL(CurrentSiteName, atInfo.AttachmentGUID.ToString(), atInfo.AttachmentExtension);
                    }
                    else
                    {
                        int[] newDim = ImageHelper.EnsureImageDimensions(Width, Height, MaxSideSize, atInfo.AttachmentImageWidth, atInfo.AttachmentImageHeight);
                        path = AttachmentInfoProvider.GetFilePhysicalURL(CurrentSiteName, atInfo.AttachmentGUID.ToString(), atInfo.AttachmentExtension, newDim[0], newDim[1]);
                    }

                    // If path is valid, redirect
                    if (path != null)
                    {
                        // Check if file exists
                        string filePath = Server.MapPath(path);
                        if (File.Exists(filePath))
                        {
                            outputAttachment = NewOutputFile();
                            outputAttachment.IsSecured = secured;
                            outputAttachment.RedirectTo = path;
                            outputAttachment.Attachment = atInfo;
                        }
                    }
                }
            }

            // Get the data
            if ((outputAttachment == null) || (outputAttachment.Attachment == null))
            {
                outputAttachment = NewOutputFile(atInfo, null);
                outputAttachment.Width = Width;
                outputAttachment.Height = Height;
                outputAttachment.MaxSideSize = MaxSideSize;
                outputAttachment.SiteName = CurrentSiteName;
                outputAttachment.Resized = resizeImage;

                // Load the data if required
                if (requiresData)
                {
                    // Try to get the physical file, if not latest version
                    if (!mIsLatestVersion)
                    {
                        EnsurePhysicalFile(outputAttachment);
                    }
                    bool loadData = string.IsNullOrEmpty(outputAttachment.PhysicalFile);

                    // Load data if necessary
                    if (loadData)
                    {
                        if (atInfo.AttachmentBinary != null)
                        {
                            // Load from the attachment
                            outputAttachment.LoadData(atInfo.AttachmentBinary);
                        }
                        else
                        {
                            // Load from the disk
                            byte[] data = AttachmentInfoProvider.GetFile(atInfo, CurrentSiteName);
                            outputAttachment.LoadData(data);
                        }

                        // Save data to the cache, if not latest version
                        if (!mIsLatestVersion && (CacheMinutes > 0))
                        {
                            SaveOutputDataToCache(outputAttachment.OutputData, GetOutputDataDependency(outputAttachment.Attachment));
                        }
                    }
                }
                else if (cachedData != null)
                {
                    // Load the cached data if available
                    outputAttachment.OutputData = cachedData;
                }
            }

            if (outputAttachment != null)
            {
                outputAttachment.IsSecured = secured;

                // Add node data
                if (node != null)
                {
                    outputAttachment.AliasPath = node.NodeAliasPath;
                    outputAttachment.CultureCode = node.DocumentCulture;
                    outputAttachment.FileNode = node;

                    // Set the file validity
                    if (IsLiveSite && !mIsLatestVersion && checkPublishedFiles)
                    {
                        outputAttachment.ValidFrom = ValidationHelper.GetDateTime(node.GetValue("DocumentPublishFrom"), DateTime.MinValue);
                        outputAttachment.ValidTo = ValidationHelper.GetDateTime(node.GetValue("DocumentPublishTo"), DateTime.MaxValue);

                        // Set the published flag                   
                        outputAttachment.IsPublished = node.IsPublished;
                    }
                }
            }
        }
    }


    /// <summary>
    /// Processes the specified document node.
    /// </summary>
    /// <param name="currentAliasPath">Alias path</param>
    /// <param name="currentFileName">File name</param>
    protected void ProcessNode(string currentAliasPath, string currentFileName)
    {
        // Load the document node
        if (node == null)
        {
            // Try to get data from cache
            using (var cs = new CachedSection<TreeNode>(ref node, CacheMinutes, !allowLatestVersion, null, "getfilenodebyaliaspath|", CurrentSiteName, CacheHelper.GetBaseCacheKey(false, true), currentAliasPath))
            {
                if (cs.LoadData)
                {
                    // Get the document
                    string className = null;
                    bool combineWithDefaultCulture = SettingsKeyInfoProvider.GetBoolValue(CurrentSiteName + ".CMSCombineImagesWithDefaultCulture");
                    string culture = CultureCode;

                    // Get the document
                    if (currentFileName == null)
                    {
                        // CMS.File
                        className = "CMS.File";
                    }

                    // Get the document data
                    if (!IsLiveSite)
                    {
                        node = DocumentHelper.GetDocument(CurrentSiteName, currentAliasPath, culture, combineWithDefaultCulture, className, null, null, -1, false, null, TreeProvider);
                    }
                    else
                    {
                        node = TreeProvider.SelectSingleNode(CurrentSiteName, currentAliasPath, culture, combineWithDefaultCulture, className, null, null, -1, false);

                        // Documents should be combined with default culture
                        if ((node != null) && combineWithDefaultCulture && !node.IsPublished)
                        {
                            // Try to find published document in default culture
                            string defaultCulture = CultureHelper.GetDefaultCultureCode(CurrentSiteName);
                            TreeNode cultureNode = TreeProvider.SelectSingleNode(CurrentSiteName, currentAliasPath, defaultCulture, false, className, null, null, -1, false);
                            if ((cultureNode != null) && cultureNode.IsPublished)
                            {
                                node = cultureNode;
                            }
                        }
                    }

                    // Try to find node using the document aliases
                    if (node == null)
                    {
                        DataSet ds = DocumentAliasInfoProvider.GetDocumentAliases()
                                                                .TopN(1)
                                                                .Columns("AliasNodeID", "AliasCulture")
                                                                .WhereEquals("AliasURLPath", currentAliasPath)
                                                                .OrderByDescending("AliasCulture");
                        if (!DataHelper.DataSourceIsEmpty(ds))
                        {
                            DataRow dr = ds.Tables[0].Rows[0];
                            int nodeId = (int)dr["AliasNodeID"];
                            string nodeCulture = ValidationHelper.GetString(DataHelper.GetDataRowValue(dr, "AliasCulture"), null);
                            if (!IsLiveSite)
                            {
                                node = DocumentHelper.GetDocument(nodeId, nodeCulture, combineWithDefaultCulture, TreeProvider);
                            }
                            else
                            {
                                node = TreeProvider.SelectSingleNode(nodeId, nodeCulture, combineWithDefaultCulture);

                                // Documents should be combined with default culture
                                if ((node != null) && combineWithDefaultCulture && !node.IsPublished)
                                {
                                    // Try to find published document in default culture
                                    string defaultCulture = CultureHelper.GetDefaultCultureCode(CurrentSiteName);
                                    TreeNode cultureNode = TreeProvider.SelectSingleNode(nodeId, defaultCulture, false);
                                    if ((cultureNode != null) && cultureNode.IsPublished)
                                    {
                                        node = cultureNode;
                                    }
                                }
                            }
                        }
                    }

                    // Cache the document
                    CacheNode(cs, node);
                }
            }
        }

        // Process the document
        ProcessNode(node, null, currentFileName);
    }


    /// <summary>
    /// Processes the specified document node.
    /// </summary>
    /// <param name="currentNodeGuid">Node GUID</param>
    protected void ProcessNode(Guid currentNodeGuid)
    {
        // Load the document node
        string columnName = QueryHelper.GetString("columnName", String.Empty);
        if (node == null)
        {
            // Try to get data from cache
            using (var cs = new CachedSection<TreeNode>(ref node, CacheMinutes, !allowLatestVersion, null, "getfilenodebyguid|", CurrentSiteName, CacheHelper.GetBaseCacheKey(false, true), currentNodeGuid))
            {
                if (cs.LoadData)
                {
                    // Get the document
                    bool combineWithDefaultCulture = SettingsKeyInfoProvider.GetBoolValue(CurrentSiteName + ".CMSCombineImagesWithDefaultCulture");
                    string culture = CultureCode;
                    string where = "NodeGUID = '" + currentNodeGuid + "'";

                    // Get the document
                    string className = null;
                    if (columnName == "")
                    {
                        // CMS.File
                        className = "CMS.File";
                    }
                    else
                    {
                        // Other document types
                        TreeNode srcNode = TreeProvider.SelectSingleNode(currentNodeGuid, CultureCode, CurrentSiteName);
                        if (srcNode != null)
                        {
                            className = srcNode.NodeClassName;
                        }
                    }

                    // Get the document data
                    if (!IsLiveSite || allowLatestVersion)
                    {
                        node = DocumentHelper.GetDocument(CurrentSiteName, null, culture, combineWithDefaultCulture, className, where, null, -1, false, null, TreeProvider);
                    }
                    else
                    {
                        node = TreeProvider.SelectSingleNode(CurrentSiteName, null, culture, combineWithDefaultCulture, className, where, null, -1, false);

                        // Documents should be combined with default culture
                        if ((node != null) && combineWithDefaultCulture && !node.IsPublished)
                        {
                            // Try to find published document in default culture
                            string defaultCulture = CultureHelper.GetDefaultCultureCode(CurrentSiteName);
                            TreeNode cultureNode = TreeProvider.SelectSingleNode(CurrentSiteName, null, defaultCulture, false, className, where, null, -1, false);
                            if ((cultureNode != null) && cultureNode.IsPublished)
                            {
                                node = cultureNode;
                            }
                        }
                    }

                    // Cache the document
                    CacheNode(cs, node);
                }
            }
        }

        // Process the document node
        ProcessNode(node, columnName, null);
    }


    /// <summary>
    /// Processes the specified document node.
    /// </summary>
    /// <param name="treeNode">Document node to process</param>
    /// <param name="columnName">Column name</param>
    /// <param name="processedFileName">File name</param>
    protected void ProcessNode(TreeNode treeNode, string columnName, string processedFileName)
    {
        if (treeNode != null)
        {
            // Check if latest or live site version is required
            bool latest = !IsLiveSite;
            if (allowLatestVersion && ((treeNode.DocumentID == latestForDocumentId) || (treeNode.DocumentCheckedOutVersionHistoryID == latestForHistoryId)))
            {
                latest = true;
            }

            // If not published, return no content
            if (!latest && !treeNode.IsPublished)
            {
                outputAttachment = NewOutputFile(null, null);
                outputAttachment.AliasPath = treeNode.NodeAliasPath;
                outputAttachment.CultureCode = treeNode.DocumentCulture;
                if (IsLiveSite && AttachmentInfoProvider.CheckPublishedFiles(CurrentSiteName))
                {
                    outputAttachment.IsPublished = treeNode.IsPublished;
                }
                outputAttachment.FileNode = treeNode;
                outputAttachment.Height = Height;
                outputAttachment.Width = Width;
                outputAttachment.MaxSideSize = MaxSideSize;
            }
            else
            {
                // Get valid site name if link
                if (treeNode.IsLink)
                {
                    TreeNode origNode = TreeProvider.GetOriginalNode(treeNode);
                    if (origNode != null)
                    {
                        SiteInfo si = SiteInfoProvider.GetSiteInfo(origNode.NodeSiteID);
                        if (si != null)
                        {
                            CurrentSiteName = si.SiteName;
                        }
                    }
                }

                // Process the node
                // Get from specific column
                if (String.IsNullOrEmpty(columnName) && String.IsNullOrEmpty(processedFileName) && treeNode.NodeClassName.EqualsCSafe("CMS.File", true))
                {
                    columnName = "FileAttachment";
                }
                if (!String.IsNullOrEmpty(columnName))
                {
                    // File document type or specified by column
                    Guid attachmentGuid = ValidationHelper.GetGuid(treeNode.GetValue(columnName), Guid.Empty);
                    if (attachmentGuid != Guid.Empty)
                    {
                        ProcessFile(attachmentGuid);
                    }
                }
                else
                {
                    // Get by file name
                    if (processedFileName == null)
                    {
                        // CMS.File - Get 
                        Guid attachmentGuid = ValidationHelper.GetGuid(treeNode.GetValue("FileAttachment"), Guid.Empty);
                        if (attachmentGuid != Guid.Empty)
                        {
                            ProcessFile(attachmentGuid);
                        }
                    }
                    else
                    {
                        // Other document types, get the attachment by file name
                        AttachmentInfo ai;
                        if (latest)
                        {
                            // Not livesite mode - get latest version
                            ai = DocumentHelper.GetAttachment(treeNode, processedFileName, TreeProvider, false);
                        }
                        else
                        {
                            // Live site mode, get directly from database
                            ai = AttachmentInfoProvider.GetAttachmentInfo(treeNode.DocumentID, processedFileName, false);
                        }

                        if (ai != null)
                        {
                            ProcessFile(ai.AttachmentGUID);
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// Processes the specified version of the file and returns the data to the output stream.
    /// </summary>
    /// <param name="attachmentGuid">Attachment GUID</param>
    /// <param name="versionHistoryId">Document version history ID</param>
    protected void ProcessFile(Guid attachmentGuid, int versionHistoryId)
    {
        AttachmentInfo atInfo = GetFile(attachmentGuid, versionHistoryId);
        if (atInfo != null)
        {
            // If attachment is image, try resize
            byte[] mFile = atInfo.AttachmentBinary;
            if (mFile != null)
            {
                string mimetype = null;
                if (ImageHelper.IsImage(atInfo.AttachmentExtension))
                {
                    if (AttachmentInfoProvider.CanResizeImage(atInfo, Width, Height, MaxSideSize))
                    {
                        // Do not search thumbnail on the disk
                        mFile = AttachmentInfoProvider.GetImageThumbnail(atInfo, CurrentSiteName, Width, Height, MaxSideSize, false);
                        mimetype = "image/jpeg";
                    }
                }

                if (mFile != null)
                {
                    outputAttachment = NewOutputFile(atInfo, mFile);
                }
                else
                {
                    outputAttachment = NewOutputFile();
                }
                outputAttachment.Height = Height;
                outputAttachment.Width = Width;
                outputAttachment.MaxSideSize = MaxSideSize;
                outputAttachment.MimeType = mimetype;
            }

            // Get the file document
            if (node == null)
            {
                node = TreeProvider.SelectSingleDocument(atInfo.AttachmentDocumentID);
            }

            if (node != null)
            {
                // Check secured area
                SiteInfo si = SiteInfoProvider.GetSiteInfo(node.NodeSiteID);
                if (si != null)
                {
                    if (pi == null)
                    {
                        pi = PageInfoProvider.GetPageInfo(si.SiteName, node.NodeAliasPath, node.DocumentCulture, node.DocumentUrlPath, false);
                    }
                    if (pi != null)
                    {
                        PageSecurityHelper.RequestSecurePage(pi, false, ViewMode, CurrentSiteName);
                        PageSecurityHelper.CheckSecuredAreas(CurrentSiteName, pi, false, ViewMode);
                    }
                }

                // Check the permissions for the document
                if ((CurrentUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.Read) == AuthorizationResultEnum.Allowed) || (node.NodeOwner == CurrentUser.UserID))
                {
                    if (outputAttachment == null)
                    {
                        outputAttachment = NewOutputFile();
                    }

                    outputAttachment.AliasPath = node.NodeAliasPath;
                    outputAttachment.CultureCode = node.DocumentCulture;
                    if (IsLiveSite && AttachmentInfoProvider.CheckPublishedFiles(CurrentSiteName))
                    {
                        outputAttachment.IsPublished = node.IsPublished;
                    }
                    outputAttachment.FileNode = node;
                }
                else
                {
                    outputAttachment = null;
                }
            }
        }
    }


    /// <summary>
    /// Gets the file from version history.
    /// </summary>
    /// <param name="attachmentGuid">Atachment GUID</param>
    /// <param name="versionHistoryId">Version history ID</param>
    protected AttachmentInfo GetFile(Guid attachmentGuid, int versionHistoryId)
    {
        VersionManager vm = VersionManager.GetInstance(TreeProvider);

        // Get the attachment version
        AttachmentHistoryInfo attachmentVersion = vm.GetAttachmentVersion(versionHistoryId, attachmentGuid);
        if (attachmentVersion == null)
        {
            return null;
        }
        else
        {
            // Create the attachment object from the version
            AttachmentInfo ai = new AttachmentInfo(attachmentVersion.Generalized.DataClass);
            ai.AttachmentVersionHistoryID = versionHistoryId;
            return ai;
        }
    }


    /// <summary>
    /// Returns the output data dependency based on the given attachment record.
    /// </summary>
    /// <param name="ai">Attachment object</param>
    protected CMSCacheDependency GetOutputDataDependency(AttachmentInfo ai)
    {
        return (ai == null) ? null : CacheHelper.GetCacheDependency(AttachmentInfoProvider.GetDependencyCacheKeys(ai));
    }


    /// <summary>
    /// Returns the cache dependency for the given document node.
    /// </summary>
    /// <param name="document">Document node</param>
    protected CMSCacheDependency GetNodeDependency(TreeNode document)
    {
        string siteName = CurrentSiteName.ToLowerCSafe();

        return CacheHelper.GetCacheDependency(new[]
                                                  {
                                                      CacheHelper.FILENODE_KEY,
                                                      CacheHelper.FILENODE_KEY + "|" + siteName,
                                                      "node|" + siteName + "|" + document.NodeAliasPath.ToLowerCSafe()
                                                  });
    }


    /// <summary>
    /// Ensures the security settings in the given document node.
    /// </summary>
    /// <param name="document">Document node</param>
    protected void EnsureSecuritySettings(TreeNode document)
    {
        if (AttachmentInfoProvider.CheckFilesPermissions(CurrentSiteName))
        {
            // Load secured values
            document.LoadInheritedValues(new[] { "IsSecuredNode", "RequiresSSL" });
        }
    }


    /// <summary>
    /// Handles the document caching actions.
    /// </summary>
    /// <param name="cs">Cached section</param>
    /// <param name="document">Document node</param>
    protected void CacheNode(CachedSection<TreeNode> cs, TreeNode document)
    {
        if (document != null)
        {
            // Load the security settings
            EnsureSecuritySettings(document);

            // Save to the cache
            if (cs.Cached)
            {
                cs.CacheDependency = GetNodeDependency(document);
            }
        }
        else
        {
            // Do not cache in case not cached
            cs.CacheMinutes = 0;
        }

        cs.Data = document;
    }


    /// <summary>
    /// Logs analytics and/or activity event.
    /// </summary>
    /// <param name="attachment">File to be sent</param>
    protected void LogEvent(CMSOutputAttachment attachment)
    {
        if (IsLiveSite && (attachment != null) && (attachment.FileNode != null) && (attachment.FileNode.NodeClassName.ToLowerCSafe() == "cms.file"))
        {
            // Check if request is multipart request and log event if not
            GetRange(100, HttpContext.Current);  // GetRange() parses request header and sets 'IsMultipart' and 'IsRangeRequest' properties
            if (IsMultipart || IsRangeRequest)
            {
                return;
            }

            if (attachment.Attachment == null)
            {
                return;
            }

            // Log analytics hit
            if (AnalyticsHelper.IsLoggingEnabled(CurrentSiteName, String.Empty, LogExcludingFlags.SkipFileExtensionCheck) && AnalyticsHelper.TrackFileDownloadsEnabled(CurrentSiteName) && !AnalyticsHelper.IsFileExtensionExcluded(CurrentSiteName, attachment.Attachment.AttachmentExtension))
            {
                HitLogProvider.LogHit(HitLogProvider.FILE_DOWNLOADS, CurrentSiteName, attachment.FileNode.DocumentCulture, attachment.FileNode.NodeAliasPath, attachment.FileNode.NodeID);
            }

            // Log download activity
            if (LoggingActivityEnabled(attachment) && LogFileDownload(attachment))
            {
                Activity activity = new ActivityPageVisit(attachment.FileNode, attachment.FileNode.GetDocumentName(), null, null, AnalyticsContext.ActivityEnvironmentVariables);
                if (activity.Data != null)
                {
                    activity.Data.Value = attachment.Attachment.AttachmentName;
                    activity.Data.Culture = attachment.FileNode.DocumentCulture;
                    activity.Data.NodeID = attachment.FileNode.NodeID;
                    activity.Data.SiteID = SiteInfoProvider.GetSiteID(CurrentSiteName);
                    activity.Log();
                }
            }
        }
    }


    /// <summary>
    /// Checks if page visit activity logging is enabled, if so returns contact ID.
    /// </summary>
    /// <param name="attachment">File to be sent</param>
    protected bool LoggingActivityEnabled(CMSOutputAttachment attachment)
    {
        if ((attachment == null) || (attachment.FileNode == null))
        {
            return false;
        }

        // Check if logging is enabled
        if (ActivitySettingsHelper.ActivitiesEnabledAndModuleLoaded(CurrentSiteName) && ActivitySettingsHelper.ActivitiesEnabledForThisUser(CurrentUser))
        {
            if (attachment.Attachment != null)
            {
                // Get allowed extensions (if not specified log everything)
                bool doLog = true;
                string tracked = SettingsKeyInfoProvider.GetValue(CurrentSiteName + ".CMSActivityTrackedExtensions");
                if (!String.IsNullOrEmpty(tracked))
                {
                    string extension = attachment.Attachment.AttachmentExtension;
                    if (extension != null)
                    {
                        string extensions = String.Format(";{0};", tracked.ToLowerCSafe().Trim().Trim(';'));
                        extension = extension.TrimStart('.').ToLowerCSafe();
                        doLog = extensions.Contains(String.Format(";{0};", extension));
                    }
                }

                return doLog;
            }
        }
        return false;
    }


    /// <summary>
    /// Check if logging is enabled for current document.
    /// </summary>
    private bool LogFileDownload(CMSOutputAttachment attachment)
    {
        if ((attachment == null) || (attachment.FileNode == null))
        {
            return false;
        }

        return (((attachment.FileNode.DocumentLogVisitActivity == true)
             || (attachment.FileNode.DocumentLogVisitActivity == null)
             && ValidationHelper.GetBoolean(attachment.FileNode.GetInheritedValue("DocumentLogVisitActivity", false), false)));
    }


    /// <summary>
    /// Ensures the physical file.
    /// </summary>
    /// <param name="attachment">Output file</param>
    public bool EnsurePhysicalFile(CMSOutputAttachment attachment)
    {
        if (attachment == null)
        {
            return false;
        }

        // Try to link to file system
        if (String.IsNullOrEmpty(attachment.Watermark) && (attachment.Attachment != null) && (attachment.Attachment.AttachmentVersionHistoryID == 0) && AttachmentInfoProvider.StoreFilesInFileSystem(attachment.SiteName))
        {
            string filePath = AttachmentInfoProvider.EnsurePhysicalFile(attachment.Attachment, attachment.SiteName);
            if (filePath != null)
            {
                if (attachment.Resized)
                {
                    // If resized, ensure the thumbnail file
                    if (AttachmentInfoProvider.GenerateThumbnails(attachment.SiteName))
                    {
                        filePath = AttachmentInfoProvider.EnsureThumbnailFile(attachment.Attachment, attachment.SiteName, Width, Height, MaxSideSize);
                        if (filePath != null)
                        {
                            // Link to the physical file
                            attachment.PhysicalFile = filePath;
                            return true;
                        }
                    }
                }
                else
                {
                    // Link to the physical file
                    attachment.PhysicalFile = filePath;
                    return false;
                }
            }
        }

        attachment.PhysicalFile = "";
        return false;
    }


    /// <summary>
    /// Returns true if latest version of the document is allowed.
    /// </summary>
    public bool CheckAllowLatestVersion()
    {
        // Check if latest version is required
        latestForDocumentId = QueryHelper.GetInteger("latestfordocid", 0);
        latestForHistoryId = QueryHelper.GetInteger("latestforhistoryid", 0);

        if ((latestForDocumentId > 0) || (latestForHistoryId > 0))
        {
            // Validate the hash
            string hash = QueryHelper.GetString("hash", "");
            string validate = (latestForDocumentId > 0) ? "d" + latestForDocumentId : "h" + latestForHistoryId;

            if (!String.IsNullOrEmpty(hash) && QueryHelper.ValidateHashString(validate, hash))
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Gets the new output file object.
    /// </summary>
    public CMSOutputAttachment NewOutputFile()
    {
        CMSOutputAttachment attachment = new CMSOutputAttachment();

        attachment.Watermark = Watermark;
        attachment.WatermarkPosition = WatermarkPosition;

        return attachment;
    }


    /// <summary>
    /// Gets the new output file object.
    /// </summary>
    /// <param name="ai">AttachmentInfo</param>
    /// <param name="data">Output file data</param>
    public CMSOutputAttachment NewOutputFile(AttachmentInfo ai, byte[] data)
    {
        CMSOutputAttachment attachment = new CMSOutputAttachment(ai, data);

        attachment.Watermark = Watermark;
        attachment.WatermarkPosition = WatermarkPosition;

        return attachment;
    }
}