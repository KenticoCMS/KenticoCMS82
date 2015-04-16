using System;

using CMS.Helpers;
using CMS.Base;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.DataEngine;
using CMS.IO;

public partial class CMSPages_GetMetaFile : GetFilePage
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

    protected CMSOutputMetaFile outputFile = null;
    protected Guid fileGuid = Guid.Empty;

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns true if the process allows cache.
    /// </summary>
    public override bool AllowCache
    {
        get
        {
            if (mAllowCache == null)
            {
                // By default, cache for the metafiles is always enabled (even outside of the live site)
                mAllowCache = ValidationHelper.GetBoolean(SettingsHelper.AppSettings["CMSAlwaysCacheMetaFiles"], true) || IsLiveSite;
            }

            return mAllowCache.Value;
        }
        set
        {
            mAllowCache = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        DebugHelper.SetContext("GetMetaFile");

        // Load the site name
        LoadSiteName();

        int cacheMinutes = CacheMinutes;

        // Try to get data from cache
        using (var cs = new CachedSection<CMSOutputMetaFile>(ref outputFile, cacheMinutes, true, null, "getmetafile", CurrentSiteName, GetBaseCacheKey(), Request.QueryString))
        {
            if (cs.LoadData)
            {
                // Process the file
                ProcessFile();

                if (cs.Cached)
                {
                    // Do not cache if too big file which would be stored in memory
                    if ((outputFile != null) &&
                        (outputFile.MetaFile != null) &&
                        !CacheHelper.CacheImageAllowed(CurrentSiteName, outputFile.MetaFile.MetaFileSize) &&
                        !AttachmentInfoProvider.StoreFilesInFileSystem(CurrentSiteName))
                    {
                        cacheMinutes = largeFilesCacheMinutes;
                    }

                    if (cacheMinutes > 0)
                    {
                        // Prepare the cache dependency
                        CMSCacheDependency cd = null;
                        if (outputFile != null)
                        {
                            if (outputFile.MetaFile != null)
                            {
                                // Add dependency on this particular meta file
                                cd = GetCacheDependency(outputFile.MetaFile.GetDependencyCacheKeys());
                            }
                        }

                        if (cd == null)
                        {
                            // Set default dependency based on GUID
                            cd = CacheHelper.GetCacheDependency(new[] { "metafile|" + fileGuid.ToString().ToLowerCSafe() });
                        }

                        cs.CacheDependency = cd;
                    }

                    // Cache the data
                    cs.CacheMinutes = cacheMinutes;
                }

                cs.Data = outputFile;
            }
        }

        // Send the data
        SendFile(outputFile);

        DebugHelper.ReleaseContext();
    }


    /// <summary>
    /// Processes the file.
    /// </summary>
    protected void ProcessFile()
    {
        outputFile = null;

        // Get file GUID from querystring
        fileGuid = QueryHelper.GetGuid("fileguid", Guid.Empty);
        if (fileGuid == Guid.Empty)
        {
            // Get file GUID from context
            fileGuid = ValidationHelper.GetGuid(Context.Items["fileguid"], Guid.Empty);
        }

        if (fileGuid == Guid.Empty)
        {
            return;
        }

        // Get the file
        var fileInfo = MetaFileInfoProvider.GetMetaFileInfoWithoutBinary(fileGuid, CurrentSiteName, true);
        if (fileInfo == null)
        {
            return;
        }

        bool resizeImage = (ImageHelper.IsImage(fileInfo.MetaFileExtension) && MetaFileInfoProvider.CanResizeImage(fileInfo, Width, Height, MaxSideSize));

        // Get the data
        if ((outputFile == null) || (outputFile.MetaFile == null))
        {
            outputFile = NewOutputFile(fileInfo, null);
            outputFile.Width = Width;
            outputFile.Height = Height;
            outputFile.MaxSideSize = MaxSideSize;
            outputFile.Resized = resizeImage;
        }
    }


    /// <summary>
    /// Sends the given file within response.
    /// </summary>
    /// <param name="file">File to send</param>
    protected void SendFile(CMSOutputMetaFile file)
    {
        // Clear response.
        CookieHelper.ClearResponseCookies();
        Response.Clear();

        // Set the revalidation
        SetRevalidation();

        // Send the file
        if (file != null)
        {
            // Redirect if the file should be redirected
            if (file.RedirectTo != "")
            {
                URLHelper.RedirectPermanent(file.RedirectTo, CurrentSiteName);
            }

            string etag = GetFileETag(file);

            // Client caching - only on the live site
            if (useClientCache && AllowCache && AllowClientCache && ETagsMatch(etag, file.LastModified))
            {
                // Set correct response content type
                SetResponseContentType(file);

                // Set the file time stamps to allow client caching
                SetTimeStamps(file.LastModified);

                RespondNotModified(etag);
                return;
            }

            // If physical file not present, try to load
            if (file.PhysicalFile == null)
            {
                EnsurePhysicalFile(outputFile);
            }

            // If the output data should be cached, return the output data
            bool cacheOutputData = false;
            if ((CacheMinutes > 0) && (file.MetaFile != null))
            {
                cacheOutputData = CacheHelper.CacheImageAllowed(CurrentSiteName, file.MetaFile.MetaFileSize);
            }

            // Ensure the file data if physical file not present
            if (!file.DataLoaded && (file.PhysicalFile == ""))
            {
                byte[] cachedData = GetCachedOutputData();
                if (file.EnsureData(cachedData))
                {
                    if ((cachedData == null) && cacheOutputData)
                    {
                        SaveOutputDataToCache(file.OutputData, GetOutputDataDependency(file.MetaFile));
                    }
                }
            }

            // Send the file
            if ((file.MetaFile != null) && ((file.OutputData != null) || (file.PhysicalFile != "")))
            {
                // Set correct response content type
                SetResponseContentType(file);

                string extension = file.MetaFile.MetaFileExtension;
                SetDisposition(file.MetaFile.MetaFileName, extension);

                // Setup Etag property
                ETag = etag;

                // Set if resumable downloads should be supported
                AcceptRange = !IsExtensionExcludedFromRanges(extension);

                if (useClientCache && AllowCache)
                {
                    // Set the file time stamps to allow client caching
                    SetTimeStamps(file.LastModified);

                    Response.Cache.SetETag(etag);
                }
                else
                {
                    SetCacheability();
                }

                // Add the file data
                if ((file.PhysicalFile != "") && (file.OutputData == null))
                {
                    if (!File.Exists(file.PhysicalFile))
                    {
                        // File doesn't exist
                        NotFound();
                    }
                    else
                    {
                        // Stream the file from the file system
                        file.OutputData = WriteFile(file.PhysicalFile, cacheOutputData);
                    }
                }
                else
                {
                    // Use output data of the file in memory if present
                    WriteBytes(file.OutputData);
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
    /// <param name="file">Output file</param>
    private void SetResponseContentType(CMSOutputMetaFile file)
    {
        string mimeType = file.MimeType;
        if (file.MetaFile != null)
        {
            string extension = file.MetaFile.MetaFileExtension;
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
    /// Gets the ETag for the given file
    /// </summary>
    /// <param name="file">File</param>
    private static string GetFileETag(CMSOutputMetaFile file)
    {
        // Prepare etag
        string etag = null;
        if (file.MetaFile != null)
        {
            etag += file.MetaFile.MetaFileID;
        }

        etag += "|" + file.LastModified;

        // Put etag into ""
        etag = "\"" + etag + "\"";

        return etag;
    }


    /// <summary>
    /// Returns the output data dependency based on the given attachment record.
    /// </summary>
    /// <param name="mi">Metafile object</param>
    protected CMSCacheDependency GetOutputDataDependency(MetaFileInfo mi)
    {
        if (mi == null)
        {
            return null;
        }

        return CacheHelper.GetCacheDependency(mi.GetDependencyCacheKeys());
    }


    /// <summary>
    /// Ensures the physical file.
    /// </summary>
    /// <param name="file">Output file</param>
    public bool EnsurePhysicalFile(CMSOutputMetaFile file)
    {
        if (file == null)
        {
            return false;
        }

        // Try to link to file system
        if (String.IsNullOrEmpty(file.Watermark) && (file.MetaFile != null) && (file.MetaFile.MetaFileID > 0) && MetaFileInfoProvider.StoreFilesInFileSystem(file.SiteName))
        {
            string filePath = MetaFileInfoProvider.EnsurePhysicalFile(file.MetaFile, file.SiteName);
            if (filePath != null)
            {
                if (file.Resized)
                {
                    // If resized, ensure the thumbnail file
                    if (MetaFileInfoProvider.GenerateThumbnails(file.SiteName))
                    {
                        filePath = MetaFileInfoProvider.EnsureThumbnailFile(file.MetaFile, file.SiteName, Width, Height, MaxSideSize);
                        if (filePath != null)
                        {
                            // Link to the physical file
                            file.PhysicalFile = filePath;
                            return true;
                        }
                    }
                }
                else
                {
                    // Link to the physical file
                    file.PhysicalFile = filePath;
                    return false;
                }
            }
        }

        file.PhysicalFile = "";
        return false;
    }


    /// <summary>
    /// Gets the new output MetaFile object.
    /// </summary>
    public CMSOutputMetaFile NewOutputFile()
    {
        CMSOutputMetaFile mf = new CMSOutputMetaFile();

        mf.Watermark = Watermark;
        mf.WatermarkPosition = WatermarkPosition;

        return mf;
    }


    /// <summary>
    /// Gets the new output MetaFile object.
    /// </summary>
    /// <param name="mfi">Meta file info</param>
    /// <param name="data">Output MetaFile data</param>
    public CMSOutputMetaFile NewOutputFile(MetaFileInfo mfi, byte[] data)
    {
        CMSOutputMetaFile mf = new CMSOutputMetaFile(mfi, data);

        mf.Watermark = Watermark;
        mf.WatermarkPosition = WatermarkPosition;

        return mf;
    }
}