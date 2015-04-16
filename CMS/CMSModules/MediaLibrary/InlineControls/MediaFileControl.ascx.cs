using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;

using MediaHelper = CMS.Helpers.MediaHelper;

public partial class CMSModules_MediaLibrary_InlineControls_MediaFileControl : InlineUserControl
{
    #region "Private variables"

    private const int DEFAULT_WIDTH = 200;
    private const int DEFAULT_HEIGHT = 200;

    private Guid mFileGuid = Guid.Empty;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Guid of the media file being inserted.
    /// </summary>
    public override string Parameter
    {
        get
        {
            return FileGuid.ToString();
        }
        set
        {
            FileGuid = ValidationHelper.GetGuid(value, Guid.Empty);
        }
    }


    /// <summary>
    /// MaxSideSize of the media file being inserted.
    /// </summary>
    public int MaxSideSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxSideSize"), 0);
        }
        set
        {
            SetValue("MaxSideSize", value);
        }
    }


    /// <summary>
    /// Width of the media file being inserted.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), 0);
        }
        set
        {
            SetValue("Width", value);
        }
    }


    /// <summary>
    /// Height of the media file being inserted.
    /// </summary>
    public int Height
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Height"), 0);
        }
        set
        {
            SetValue("Height", value);
        }
    }


    /// <summary>
    /// Guid of the media file being inserted.
    /// </summary>
    public Guid FileGuid
    {
        get
        {
            return ValidationHelper.GetGuid(GetValue("FileGuid"), Guid.Empty);
        }
        set
        {
            SetValue("FileGuid", value);
        }
    }


    /// <summary>
    /// Site name of media file.
    /// </summary>
    public string SiteName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SiteName"), SiteContext.CurrentSiteName);
        }
        set
        {
            SetValue("SiteName", value);
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize controls
        SetupControls();
    }


    /// <summary>
    /// Initializes all the nested controls.
    /// </summary>
    private void SetupControls()
    {
        MediaFileInfo mFile = MediaFileInfoProvider.GetMediaFileInfo(FileGuid, SiteName);
        if (mFile != null)
        {
            SiteInfo si = SiteInfoProvider.GetSiteInfo(mFile.FileSiteID);
            if (si != null)
            {
                MediaLibraryInfo mLibrary = MediaLibraryInfoProvider.GetMediaLibraryInfo(mFile.FileLibraryID);

                string extension = mFile.FileExtension.ToLowerCSafe().TrimStart('.');

                string path = MediaFileInfoProvider.GetMediaFilePath(SiteName, mLibrary.LibraryFolder, mFile.FilePath);
                string url = MediaFileInfoProvider.GetMediaFileAbsoluteUrl(SiteName, mFile.FileGUID, mFile.FileName);

                if (ImageHelper.IsImage(extension) && File.Exists(path))
                {
                    // Get image dimension
                    // New dimensions
                    int[] newDims = ImageHelper.EnsureImageDimensions(Width, Height, MaxSideSize, mFile.FileImageWidth, mFile.FileImageHeight);

                    // If new dimensions are diferent use them
                    if (((newDims[0] != mFile.FileImageWidth) || (newDims[1] != mFile.FileImageHeight)) && (newDims[0] > 0) && (newDims[1] > 0))
                    {
                        string dimensions = "?width=" + newDims[0] + "&height=" + newDims[1];

                        ltlOutput.Text = "<img alt=\"" + mFile.FileName + "\" src=\"" + url + dimensions + "\" width=\"" + newDims[0] + "\" height=\"" + newDims[1] + "\" border=\"0\" />";
                    }
                    else
                    {
                        ltlOutput.Text = "<img alt=\"" + mFile.FileName + "\" src=\"" + url + "\" width=\"" + Width + "\" height=\"" + Height + "\" border=\"0\" />";
                    }
                }
                else
                {
                    // Set default dimensions of rendered object if required
                    int width = (Width > 0) ? Width : DEFAULT_WIDTH;
                    int height = (Height > 0) ? Height : DEFAULT_HEIGHT;

                    if (MediaHelper.IsFlash(extension))
                    {
                        // Initialize flash parameters
                        FlashParameters flashParams = new FlashParameters();
                        flashParams.Height = height;
                        flashParams.Width = width;
                        flashParams.Url = url;

                        ltlOutput.Text = MediaHelper.GetFlash(flashParams);
                    }
                    else
                    {
                        if (MediaHelper.IsAudio(extension))
                        {
                            // Initialize audio/video parameters
                            AudioVideoParameters audioParams = new AudioVideoParameters();

                            audioParams.SiteName = SiteContext.CurrentSiteName;
                            audioParams.Url = url;
                            audioParams.Width = width;
                            audioParams.Height = height;
                            audioParams.Extension = extension;

                            ltlOutput.Text = MediaHelper.GetAudioVideo(audioParams);
                        }
                        else if (MediaHelper.IsVideo(extension))
                        {
                            // Initialize audio/video parameters
                            AudioVideoParameters videoParams = new AudioVideoParameters();

                            videoParams.SiteName = SiteContext.CurrentSiteName;
                            videoParams.Url = url;
                            videoParams.Width = width;
                            videoParams.Height = height;
                            videoParams.Extension = extension;

                            ltlOutput.Text = MediaHelper.GetAudioVideo(videoParams);
                        }
                    }
                }
            }
        }
    }
}