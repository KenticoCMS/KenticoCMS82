using System;
using System.Collections.Generic;

using CMS.Helpers;
using CMS.Base;
using CMS.Membership;
using CMS.UIControls;
using CMS.IO;

public partial class CMSModules_Avatars_CMSPages_GetAvatar : GetFilePage
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

    protected CMSOutputAvatar outputFile = null;
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
                mAllowCache = ValidationHelper.GetBoolean(SettingsHelper.AppSettings["CMSAlwaysCacheAvatars"], true) || IsLiveSite;
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
        DebugHelper.SetContext("GetAvatar");

        // Prepare the cache key
        fileGuid = QueryHelper.GetGuid("avatarguid", Guid.Empty);

        // Try get guid from routed URL
        if (fileGuid == Guid.Empty)
        {
            fileGuid = ValidationHelper.GetGuid(RouteData.Values["avatarGuid"], Guid.Empty);
            if (fileGuid != Guid.Empty)
            {
                MaxSideSize = ValidationHelper.GetInteger(RouteData.Values["maxSideSize"], 0);
            }
        }

        int cacheMinutes = CacheMinutes;

        // Try to get data from cache
        using (var cs = new CachedSection<CMSOutputAvatar>(ref outputFile, cacheMinutes, true, null, "getavatar", GetBaseCacheKey(), Request.QueryString))
        {
            if (cs.LoadData)
            {
                // Process the file
                ProcessFile();

                // Ensure the cache settings
                if (cs.Cached)
                {
                    // Add cache dependency
                    CMSCacheDependency cd = null;
                    if (outputFile != null)
                    {
                        if (outputFile.Avatar != null)
                        {
                            cd = CacheHelper.GetCacheDependency(outputFile.Avatar.GetDependencyCacheKeys());

                            // Do not cache if too big file which would be stored in memory
                            if ((outputFile.Avatar != null) && !CacheHelper.CacheImageAllowed(CurrentSiteName, outputFile.Avatar.AvatarFileSize) && !AvatarInfoProvider.StoreFilesInFileSystem())
                            {
                                cacheMinutes = largeFilesCacheMinutes;
                            }
                        }
                    }

                    if ((cd == null) && (cacheMinutes > 0))
                    {
                        // Set default dependency based on GUID
                        cd = GetCacheDependency(new List<string> { "avatarfile|" + fileGuid.ToString().ToLowerCSafe() });
                    }

                    // Cache the data
                    cs.CacheMinutes = cacheMinutes;
                    cs.CacheDependency = cd;
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
        if (fileGuid == Guid.Empty)
        {
            return;
        }

        // Get the avatar
        var avatar = AvatarInfoProvider.GetAvatarInfoWithoutBinary(fileGuid);
        if (avatar == null)
        {
            return;
        }

        bool resizeImage = (ImageHelper.IsImage(avatar.AvatarFileExtension) && AvatarInfoProvider.CanResizeImage(avatar, Width, Height, MaxSideSize));

        // Get the data
        if ((outputFile == null) || (outputFile.Avatar == null))
        {
            outputFile = new CMSOutputAvatar(avatar, null);
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
    protected void SendFile(CMSOutputAvatar file)
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

            // Prepare etag
            string etag = file.LastModified.ToString();

            // Client caching
            if (useClientCache && AllowCache && AllowClientCache && ETagsMatch(etag, file.LastModified))
            {
                // Set correct response content type
                SetResponseContentType(file);

                SetTimeStamps(file.LastModified);
                
                RespondNotModified(etag);
                return;
            }

            // If physical file not present, try to load
            if (file.PhysicalFile == null)
            {
                EnsurePhysicalFile(outputFile);
            }

            // Ensure the file data if physical file not present
            if (!file.DataLoaded && (file.PhysicalFile == ""))
            {
                byte[] cachedData = GetCachedOutputData();
                if (file.EnsureData(cachedData))
                {
                    if ((cachedData == null) && (CacheMinutes > 0))
                    {
                        SaveOutputDataToCache(file.OutputData, GetOutputDataDependency(file.Avatar));
                    }
                }
            }

            // Send the file
            if ((file.OutputData != null) || (file.PhysicalFile != ""))
            {
                // Set correct response content type
                SetResponseContentType(file);

                // Prepare response
                string extension = file.Avatar.AvatarFileExtension;
                SetDisposition(file.Avatar.AvatarFileName, extension);

                ETag = etag;
                // Set if resumable downloads should be supported
                AcceptRange = !IsExtensionExcludedFromRanges(extension);

                if (useClientCache && AllowCache)
                {
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
                        // If the output data should be cached, return the output data
                        bool cacheOutputData = false;
                        if (file.Avatar != null)
                        {
                            cacheOutputData = CacheHelper.CacheImageAllowed(CurrentSiteName, file.Avatar.AvatarFileSize);
                        }

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
    private void SetResponseContentType(CMSOutputAvatar file)
    {
        string mimeType = file.MimeType;
        if (file.Avatar != null)
        {
            string extension = file.Avatar.AvatarFileExtension;
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
    /// Returns the output data dependency based on the given attachment record.
    /// </summary>
    /// <param name="ai">Avatar object</param>
    protected CMSCacheDependency GetOutputDataDependency(AvatarInfo ai)
    {
        if (ai == null)
        {
            return null;
        }

        return CacheHelper.GetCacheDependency(ai.GetDependencyCacheKeys());
    }


    /// <summary>
    /// Ensures the physical file.
    /// </summary>
    /// <param name="file">Output file</param>
    public bool EnsurePhysicalFile(CMSOutputAvatar file)
    {
        if (file == null)
        {
            return false;
        }

        // Try to link to file system
        if ((file.Avatar != null) && (file.Avatar.AvatarID > 0) && AvatarInfoProvider.StoreFilesInFileSystem())
        {
            string filePath = AvatarInfoProvider.EnsureAvatarPhysicalFile(file.Avatar);
            if (filePath != null)
            {
                if (file.Resized)
                {
                    // If resized, ensure the thumbnail file
                    if (AvatarInfoProvider.GenerateThumbnails())
                    {
                        filePath = AvatarInfoProvider.EnsureThumbnailFile(file.Avatar, Width, Height, MaxSideSize);
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
}