using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.IO;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSAdminControls_Silverlight_MultiFileUploader_MultiFileUploader : CMSUserControl
{
    #region "Constants"

    private const string PARAMETER_SEPARATOR = "|";
    private const string APPLICATION_PATH = "~/ClientBin/MultiFileUploader.xap";

    #endregion


    #region "Variables"

    private bool? mEnabled = null;
    private int mResizeToWidth = -1;
    private int mResizeToHeight = -1;
    private int mResizeToMaxSideSize = -1;
    private bool mMultiSelect = false;
    private int mMaxNumberToUpload = 0;
    private long mMaximumTotalUpload = 0;
    private long mMaximumUpload = 0;
    private int mMaxConcurrentUploads = 1;
    private long mUploadChunkSize = 0;
    private MultifileUploaderModeEnum mUploadMode = MultifileUploaderModeEnum.DirectSingle;
    private bool mIsInsertMode = false;
    private bool mCheckPermissions = false;
    private bool mOnlyImages = false;
    private string mTargetFolderPath = null;
    private string mTargetFilePath = null;
    private string mTargetAliasPath = null;
    private string mTargetCulture = null;
    private bool mIncludeExtension = false;
    private MediaSourceEnum mSourceType = MediaSourceEnum.Web;
    private bool mIncludeNewItemInfo = false;
    private bool mRaiseOnClick = false;

    private int mNodeID = 0;
    private string mDocumentCulture = null;
    private string mNodeSiteName = null;
    private int mNodeGroupID = 0;

    private int mMediaLibraryID = 0;
    private bool mIsMediaThumbnail = false;
    private int mMediaFileID = 0;

    private int mPostForumID = 0;
    private int mPostID = 0;

    private Guid mAttachmentGroupGUID = Guid.Empty;
    private Guid mFormGUID = Guid.Empty;
    private int mDocumentID = 0;
    private int mDocumentParentNodeID = 0;
    private bool mIsFiledAttachment = false;
    private Guid mAttachmentGUID = Guid.Empty;

    private int mSiteID = 0;
    private int mMetaFileID = 0;
    private int mObjectID = 0;
    private string mObjectType = null;
    private string mCategory = null;

    #endregion


    #region "General properties"

    /// <summary>
    /// Indicates whether the post-upload JavaScript function call should include created attachment GUID information.
    /// </summary>
    public bool IncludeNewItemInfo
    {
        get
        {
            return mIncludeNewItemInfo;
        }
        set
        {
            mIncludeNewItemInfo = value;
        }
    }


    /// <summary>
    /// Gets or sets type of the content uploaded by the control.
    /// </summary>
    public virtual MediaSourceEnum SourceType
    {
        get
        {
            return mSourceType;
        }
        set
        {
            mSourceType = value;
            // Ensure field attachment flag
            if (value == MediaSourceEnum.Attachment)
            {
                IsFieldAttachment = true;
            }
        }
    }


    /// <summary>
    /// Unique ID of the control, for which the postback should be made after upload is finished.
    /// </summary>
    public string EventTarget
    {
        get;
        set;
    }


    /// <summary>
    /// Target folder path, to which physical files will be uploaded.
    /// </summary>
    public string TargetFolderPath
    {
        get
        {
            return mTargetFolderPath;
        }
        set
        {
            mTargetFolderPath = value;
        }
    }


    /// <summary>
    /// Target file name, to which physical files will be uploaded.
    /// </summary>
    public string TargetFileName
    {
        get
        {
            return mTargetFilePath;
        }
        set
        {
            mTargetFilePath = value;
        }
    }


    /// <summary>
    /// Target alias path, to which files will be uploaded.
    /// </summary>
    public string TargetAliasPath
    {
        get
        {
            return mTargetAliasPath;
        }
        set
        {
            mTargetAliasPath = value;
        }
    }


    /// <summary>
    /// Target culture, to which files will be uploaded.
    /// </summary>
    public string TargetCulture
    {
        get
        {
            return mTargetCulture;
        }
        set
        {
            mTargetCulture = value;
        }
    }


    /// <summary>
    /// Indicates if file extension sould be included in file name during grid file upload.
    /// </summary>
    public bool IncludeExtension
    {
        get
        {
            return mIncludeExtension;
        }
        set
        {
            mIncludeExtension = value;
        }
    }


    /// <summary>
    /// ID of parent element.
    /// </summary>
    public string ParentElemID
    {
        get;
        set;
    }


    /// <summary>
    /// JavaScript function name called after save of new file.
    /// </summary>
    public string AfterSaveJavascript
    {
        get;
        set;
    }


    /// <summary>
    /// Uploader instance guid.
    /// </summary>
    private Guid InstanceGuid
    {
        get
        {
            object o = ViewState["InstanceGuid"];
            if (o == null)
            {
                o = Guid.NewGuid();
            }
            return ValidationHelper.GetGuid(o, Guid.Empty);
        }
    }


    /// <summary>
    /// Indicates if only images is allowed for upload.
    /// </summary>
    public bool OnlyImages
    {
        get
        {
            return mOnlyImages;
        }
        set
        {
            mOnlyImages = value;
        }
    }


    /// <summary>
    /// Indicates whether the permissions should be explicitly checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return mCheckPermissions;
        }
        set
        {
            mCheckPermissions = value;
        }
    }


    /// <summary>
    /// Value indicating whether the silverlight application should be windowless or not. Default is <c>true</c>.
    /// </summary>
    public bool IsWindowless
    {
        get
        {
            return slApplication.IsWindowless;
        }
        set
        {
            slApplication.IsWindowless = value;
        }
    }


    /// <summary>
    /// Upload mode for the silverlight uploader application.
    /// </summary>
    public MultifileUploaderModeEnum UploadMode
    {
        get
        {
            return mUploadMode;
        }
        set
        {
            mUploadMode = value;
        }
    }


    /// <summary>
    /// Size of the upload chunk. If set to <c>0</c> the default value is used. The default value is 4 MB.
    /// </summary>
    public long UploadChunkSize
    {
        get
        {
            return mUploadChunkSize;
        }
        set
        {
            mUploadChunkSize = value;
        }
    }


    /// <summary>
    /// Max number of concurrent uploads.
    /// </summary>
    public int MaxConcurrentUploads
    {
        get
        {
            return mMaxConcurrentUploads;
        }
        set
        {
            mMaxConcurrentUploads = value;
        }
    }


    /// <summary>
    /// Value of the maximum upload size of a single file.
    /// </summary>
    public long MaximumUpload
    {
        get
        {
            return mMaximumUpload;
        }
        set
        {
            mMaximumUpload = value;
        }
    }


    /// <summary>
    /// Value of the maximum total upload.
    /// </summary>
    public long MaximumTotalUpload
    {
        get
        {
            return mMaximumTotalUpload;
        }
        set
        {
            mMaximumTotalUpload = value;
        }
    }


    /// <summary>
    /// Max number of possible upload files.
    /// </summary>
    public int MaxNumberToUpload
    {
        get
        {
            return mMaxNumberToUpload;
        }
        set
        {
            mMaxNumberToUpload = value;
        }
    }


    /// <summary>
    /// Value indicating whether multiselect is enabled in the open file dialog window.
    /// </summary>
    public bool Multiselect
    {
        get
        {
            return mMultiSelect;
        }
        set
        {
            mMultiSelect = value;
        }
    }


    /// <summary>
    /// Silverlight application container width.
    /// </summary>
    public Unit Width
    {
        get
        {
            return slApplication.Width;
        }
        set
        {
            slApplication.Width = value;
        }
    }


    /// <summary>
    /// Silverlight application container height.
    /// </summary>
    public Unit Height
    {
        get
        {
            return slApplication.Height;
        }
        set
        {
            slApplication.Height = value;
        }
    }


    /// <summary>
    /// Width of attachment.
    /// </summary>
    public int ResizeToWidth
    {
        get
        {
            if (mResizeToWidth == -1)
            {
                mResizeToWidth = SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAutoResizeImageWidth");
            }
            return mResizeToWidth;
        }
        set
        {
            mResizeToWidth = value;
        }
    }


    /// <summary>
    /// Height of attachment.
    /// </summary>
    public int ResizeToHeight
    {
        get
        {
            if (mResizeToHeight == -1)
            {
                mResizeToHeight = SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAutoResizeImageHeight");
            }
            return mResizeToHeight;
        }
        set
        {
            mResizeToHeight = value;
        }
    }


    /// <summary>
    /// Maximum side size of attachment.
    /// </summary>
    public int ResizeToMaxSideSize
    {
        get
        {
            if (mResizeToMaxSideSize == -1)
            {
                mResizeToMaxSideSize = SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAutoResizeImageMaxSideSize");
            }
            return mResizeToMaxSideSize;
        }
        set
        {
            mResizeToMaxSideSize = value;
        }
    }


    /// <summary>
    /// List of allowed extensions.
    /// </summary>
    public string AllowedExtensions
    {
        get;
        set;
    }


    /// <summary>
    /// Correct file handler url according to uploader mode.
    /// </summary>
    public string UploadHandlerUrl
    {
        get
        {
            string url = string.Empty;
            if (MediaLibraryID > 0)
            {
                url = "~/CMSModules/MediaLibrary/CMSPages/MultiFileUploader.ashx";
            }
            else
            {
                if (PostForumID > 0)
                {
                    url = "~/CMSModules/Forums/CMSPages/MultiFileUploader.ashx";
                }
                else
                {
                    url = "~/CMSModules/Content/CMSPages/MultiFileUploader.ashx";
                }
            }
            return HttpUtility.UrlEncode(URLHelper.GetAbsoluteUrl(url));
        }
    }


    /// <summary>
    /// Gets the ClientID of the silverlight application.
    /// </summary>
    public string SilverlightApplicationClientID
    {
        get
        {
            return slApplication.ClientID;
        }
    }


    /// <summary>
    /// Value indicating whether control is in insert mode or not. Default is false.
    /// </summary>
    public bool IsInsertMode
    {
        get
        {
            return mIsInsertMode;
        }
        set
        {
            mIsInsertMode = value;
        }
    }


    /// <summary>
    /// Indicates if supported browser.
    /// </summary>
    public bool RaiseOnClick
    {
        get
        {
            return mRaiseOnClick;
        }
        set
        {
            mRaiseOnClick = value;
        }
    }


    /// <summary>
    /// Enabled property specifing whether the silverlight uploader application is enabled. If set to <c>false</c> only alternate uploader is rendered.
    /// </summary>
    public bool Enabled
    {
        get
        {
            if (mEnabled == null)
            {
                mEnabled = DirectoryHelper.CheckPermissions(Server.MapPath(UploaderHelper.TEMP_PATH), true, true, true, true);
            }
            return (mEnabled ?? false);
        }
        set
        {
            mEnabled = value;
        }
    }


    /// <summary>
    /// HTML content which is displayed to user when Silverlight plugin is not installed.
    /// </summary>
    [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public virtual ControlCollection AlternateContent
    {
        get
        {
            return (ControlCollection)slApplication.Controls;
        }
    }

    #endregion


    #region "Media Library Properties"

    /// <summary>
    /// ID of the media file
    /// </summary>
    public int MediaFileID
    {
        get
        {
            return mMediaFileID;
        }
        set
        {
            mMediaFileID = value;
        }
    }


    /// <summary>
    /// Determines whether the uploader should upload media file thumbnail or basic media file
    /// </summary>
    public bool IsMediaThumbnail
    {
        get
        {
            return mIsMediaThumbnail;
        }
        set
        {
            mIsMediaThumbnail = value;
        }
    }


    /// <summary>
    /// Media file name
    /// </summary>
    public string MediaFileName
    {
        get;
        set;
    }


    /// <summary>
    /// Id of the media library.
    /// </summary>
    public int MediaLibraryID
    {
        get
        {
            return mMediaLibraryID;
        }
        set
        {
            mMediaLibraryID = value;
        }
    }


    /// <summary>
    /// Name of the media library folder.
    /// </summary>
    public string MediaFolderPath
    {
        get;
        set;
    }

    #endregion


    #region "CMS.File Properties"

    /// <summary>
    /// Current site name.
    /// </summary>
    public string NodeSiteName
    {
        get
        {
            if (mNodeSiteName == null)
            {
                mNodeSiteName = SiteContext.CurrentSiteName;
            }
            return mNodeSiteName;
        }
        set
        {
            mNodeSiteName = value;
        }
    }


    /// <summary>
    /// ID of the parent node, where to upload CMS.File.
    /// </summary>
    public int NodeID
    {
        get
        {
            return mNodeID;
        }
        set
        {
            mNodeID = value;
        }
    }


    /// <summary>
    /// The culture in which the new CMS.Files should be created.
    /// </summary>
    public string DocumentCulture
    {
        get
        {
            if (mDocumentCulture == null)
            {
                mDocumentCulture = LocalizationContext.PreferredCultureCode;
            }
            return mDocumentCulture;
        }
        set
        {
            mDocumentCulture = value;
        }
    }


    /// <summary>
    /// If set, NodeGroupID of content file is set also.
    /// </summary>
    public virtual int NodeGroupID
    {
        get
        {
            return mNodeGroupID;
        }
        set
        {
            mNodeGroupID = value;
        }
    }

    #endregion


    #region "Forum Attachment Properties"

    /// <summary>
    /// ID of the post forum.
    /// </summary>
    public int PostForumID
    {
        get
        {
            return mPostForumID;
        }
        set
        {
            mPostForumID = value;
        }
    }


    /// <summary>
    /// ID of the post.
    /// </summary>
    public int PostID
    {
        get
        {
            return mPostID;
        }
        set
        {
            mPostID = value;
        }
    }

    #endregion


    #region "Attachment properties"

    /// <summary>
    /// GUID of attachment
    /// </summary>
    public Guid AttachmentGUID
    {
        get
        {
            return mAttachmentGUID;
        }
        set
        {
            mAttachmentGUID = value;
        }
    }


    /// <summary>
    /// GUID of the attachment group.
    /// </summary>
    public Guid AttachmentGroupGUID
    {
        get
        {
            return mAttachmentGroupGUID;
        }
        set
        {
            mAttachmentGroupGUID = value;
        }
    }


    /// <summary>
    /// The name of the document attachment column.
    /// </summary>
    public string AttachmentGUIDColumnName
    {
        get;
        set;
    }


    /// <summary>
    /// GUID of the form.
    /// </summary>
    public Guid FormGUID
    {
        get
        {
            return mFormGUID;
        }
        set
        {
            mFormGUID = value;
        }
    }


    /// <summary>
    /// ID of the document.
    /// </summary>
    public int DocumentID
    {
        get
        {
            return mDocumentID;
        }
        set
        {
            mDocumentID = value;
        }
    }


    /// <summary>
    /// Id of the parent node.
    /// </summary>
    public int DocumentParentNodeID
    {
        get
        {
            return mDocumentParentNodeID;
        }
        set
        {
            mDocumentParentNodeID = value;
        }
    }


    /// <summary>
    /// Document class name.
    /// </summary>
    public string NodeClassName
    {
        get;
        set;
    }


    /// <summary>
    /// Value which determines whether the upload is for field attachment or not.
    /// </summary>
    public bool IsFieldAttachment
    {
        get
        {
            return mIsFiledAttachment;
        }
        set
        {
            mIsFiledAttachment = value;
        }
    }

    #endregion


    #region "MetaFile Properties"

    /// <summary>
    /// ID of the site, where to upload MetaFile.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteID;
        }
        set
        {
            mSiteID = value;
        }
    }


    /// <summary>
    /// Metafile ID for reupload.
    /// </summary>
    public int MetaFileID
    {
        get
        {
            return mMetaFileID;
        }
        set
        {
            mMetaFileID = value;
        }
    }


    /// <summary>
    /// ID of the meta file parent object.
    /// </summary>
    public int ObjectID
    {
        get
        {
            return mObjectID;
        }
        set
        {
            mObjectID = value;
        }
    }


    /// <summary>
    /// The object type of the metafile.
    /// </summary>
    public string ObjectType
    {
        get
        {
            return mObjectType;
        }
        set
        {
            mObjectType = value;
        }
    }


    /// <summary>
    /// The category of the metafile.
    /// </summary>
    public string Category
    {
        get
        {
            return mCategory;
        }
        set
        {
            mCategory = value;
        }
    }

    #endregion


    #region "Javascript Properties"

    /// <summary>
    /// Container client Id passed to each upload javascript functions(OnUploadBegin, OnUploadProgressChanged, OnUploadCompleted) as first parameter.
    /// </summary>
    public string ContainerID
    {
        get;
        set;
    }


    /// <summary>
    /// Name of the javascript function called when upload progress is changed.
    /// </summary>
    public string OnUploadProgressChanged
    {
        get;
        set;
    }


    /// <summary>
    /// Name of the javascript function called when upload is finished.
    /// </summary>
    public string OnUploadCompleted
    {
        get;
        set;
    }


    /// <summary>
    /// Name of the javascript function called before upload begins.
    /// </summary>
    public string OnUploadBegin
    {
        get;
        set;
    }

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            ReloadData();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ReloadData();
    }

    #endregion


    #region "Public Methods"

    /// <summary>
    /// Loads data of the control.
    /// </summary>
    public void ReloadData()
    {
        if (BrowserHelper.IsSafari() || BrowserHelper.IsOpera())
        {
            // Disable uploader in safari and opera - silverlight is not fully supported
            slApplication.Enabled = false;
        }
        else
        {
            string[,] additionParams = new string[1, 2];
            additionParams[0, 0] = "splashscreensource";
            additionParams[0, 1] = ResolveUrl("~/ClientBin/MultiFileUploader.xaml");

            slApplication.AdditionalParameters = additionParams;
            slApplication.Enabled = Enabled;
            slApplication.ApplicationPath = APPLICATION_PATH;
            slApplication.Parameters = GetParameters();

            LoadResources();
        }
    }


    private void LoadResources()
    {
        StringBuilder sb = new StringBuilder();

        List<String> resources = new List<String>()
                                     {
                                         "sl.mfu.upload",
                                         "sl.mfu.cancel",
                                         "sl.mfu.name",
                                         "sl.mfu.size",
                                         "sl.mfu.progress",
                                         "sl.mfu.totalfiles",
                                         "sl.mfu.totalsize",
                                         "sl.mfu.selectfiles",
                                         "sl.mfu.clear",
                                         "sl.mfu.error.noaliaspath",
                                         "sl.mfu.error.maxnumbertoupload",
                                         "sl.mfu.error.maxuploadamount",
                                         "sl.mfu.error.maxuploadsize",
                                         "sl.mfu.error.fileexists"
                                     };

        // Create javascript object with all resources
        sb.Append("window.MFUResourceKeys = [];\nwindow.MFUResources = {};\n");

        resources.ForEach(resource =>
                          sb.Append(String.Format("window.MFUResourceKeys.push('{0}');\nwindow.MFUResources['{0}'] = {1};\n",
                                                  resource,
                                                  ScriptHelper.GetString(ResHelper.GetString(resource)))
                              )
            );

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "MFUResources", sb.ToString(), true);
    }

    #endregion


    #region "Private Methods"

    /// <summary>
    /// Returns arguments string with security hash.
    /// </summary>
    /// <param name="args">Arguments array</param>
    private static string GetArgumentsString(string[] args)
    {
        string arguments = null;

        if ((args != null) && (args.Length > 0))
        {
            arguments = String.Join(PARAMETER_SEPARATOR, args);
        }

        if (!String.IsNullOrEmpty(arguments))
        {
            arguments = String.Format("{0}{1}Hash{1}{2}", arguments, PARAMETER_SEPARATOR, ValidationHelper.GetHashString(arguments, new HashSettings { UserSpecific = false }));
        }

        return HttpUtility.UrlEncode(arguments);
    }


    /// <summary>
    /// Returns additional parameters needed by the handler.
    /// </summary>
    private string GetAdditionalParameters()
    {
        string[] args = new string[]
                            {
                                "SourceType", SourceType.ToString(),
                                "ParentElementID", ParentElemID,
                                "IsInsertMode", IsInsertMode.ToString(),
                                "AfterSaveJavascript", AfterSaveJavascript,
                                "TargetFolderPath", HttpUtility.UrlEncode(TargetFolderPath),
                                "TargetFileName", HttpUtility.UrlEncode(TargetFileName),
                                "IncludeNewItemInfo", IncludeNewItemInfo.ToString(),
                                "OnlyImages", OnlyImages.ToString(),
                                "RaiseOnClick", RaiseOnClick.ToString(),
                                "TargetAliasPath", TargetAliasPath,
                                "TargetCulture", TargetCulture,
                                "EventTarget", EventTarget
                            };
        return "AdditionalParameters=" + GetArgumentsString(args);
    }


    /// <summary>
    /// Returns parameters according to the upload mode.
    /// </summary>
    private string GetModeParameters()
    {
        string[] args = null;

        if (MediaLibraryID > 0)
        {
            // MediaLibrary mode
            args = new string[]
                       {
                           "MediaLibraryID", MediaLibraryID.ToString(),
                           "MediaFolderPath", HttpUtility.UrlEncode(MediaFolderPath),
                           "MediaFileID", MediaFileID.ToString(),
                           "IsMediaThumbnail", IsMediaThumbnail.ToString(),
                           "MediaFileName", HttpUtility.UrlEncode(MediaFileName)
                       };
            return "MediaLibraryArgs=" + GetArgumentsString(args);
        }
        else
        {
            if ((NodeID > 0) && (SourceType == MediaSourceEnum.Content))
            {
                // CMS.File mode
                args = new string[]
                           {
                               "NodeID", NodeID.ToString(),
                               "DocumentCulture", DocumentCulture,
                               "IncludeExtension", IncludeExtension.ToString(),
                               "NodeGroupID", NodeGroupID.ToString()
                           };
                return "FileArgs=" + GetArgumentsString(args);
            }
            else
            {
                if (ObjectID > 0)
                {
                    // MetaFile mode
                    args = new string[]
                               {
                                   "MetaFileID", MetaFileID.ToString(),
                                   "ObjectID", ObjectID.ToString(),
                                   "SiteID", SiteID.ToString(),
                                   "ObjectType", ObjectType,
                                   "Category", Category
                               };
                    return "MetaFileArgs=" + GetArgumentsString(args);
                }
                else
                {
                    if (PostForumID > 0)
                    {
                        // Forum attachment
                        args = new string[]
                                   {
                                       "PostForumID", PostForumID.ToString(),
                                       "PostID", PostID.ToString()
                                   };
                        return "ForumArgs=" + GetArgumentsString(args);
                    }
                    else
                    {
                        if ((DocumentID > 0) || (FormGUID != Guid.Empty))
                        {
                            // Attachment mode
                            args = new string[]
                                       {
                                           "DocumentID", DocumentID.ToString(),
                                           "DocumentParentNodeID", DocumentParentNodeID.ToString(),
                                           "NodeClassName", NodeClassName,
                                           "AttachmentGUIDColumnName", AttachmentGUIDColumnName,
                                           "AttachmentGUID", AttachmentGUID.ToString(),
                                           "AttachmentGroupGUID", AttachmentGroupGUID.ToString(),
                                           "FormGUID", FormGUID.ToString(),
                                           "IsFieldAttachment", mIsFiledAttachment.ToString()
                                       };
                            return "AttachmentArgs=" + GetArgumentsString(args);
                        }
                    }
                }
            }
        }
        return String.Empty;
    }


    /// <summary>
    /// Returns the silverlight parematers according to the settings.
    /// </summary>
    private string GetParameters()
    {
        // Get the extension filter
        string[] parameters = {
                                  // Uploader instance guid
                                  string.Format("InstanceGuid={0}", InstanceGuid),
                                  // Resize parameters
                                  string.Format("ResizeArgs={0};{1};{2}", ResizeToWidth, ResizeToHeight, ResizeToMaxSideSize),
                                  // Mode parameters
                                  GetModeParameters(),
                                  // Upload page parameters
                                  string.Format("UploadPage={0}", UploadHandlerUrl),
                                  // Filter parameters
                                  GetFilterParameters(),
                                  // Javascript event parameters
                                  GetJavascriptEventsParameters(),
                                  // Restriction parameters
                                  GetRestrictionParameters(),
                                  // Upload mode
                                  string.Format("UploadMode={0}", UploadMode),
                                  // Additional parameters
                                  GetAdditionalParameters()
                              };

        return string.Join(",", parameters);
    }


    /// <summary>
    /// Returns restriction parameters for the uploader.
    /// </summary>
    private string GetRestrictionParameters()
    {
        return string.Format("Multiselect={0},MaxNumberToUpload={1},MaximumTotalUpload={2},MaximumUpload={3},MaxConcurrentUploads={4},UploadChunkSize={5}",
            Multiselect,
            MaxNumberToUpload,
            MaximumTotalUpload,
            MaximumUpload,
            MaxConcurrentUploads,
            UploadChunkSize);
    }


    /// <summary>
    /// Returns the filter parameter.
    /// </summary>
    private string GetFilterParameters()
    {
        string filter = null;
        if (String.IsNullOrEmpty(AllowedExtensions))
        {
            if (MediaLibraryID > 0)
            {
                filter = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSMediaFileAllowedExtensions");
            }
            else
            {
                filter = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSUploadExtensions");
            }
        }
        else
        {
            filter = AllowedExtensions;
        }

        if (!string.IsNullOrEmpty(filter))
        {
            // Append hash to list of allowed extensions
            string hash = ValidationHelper.GetHashString(filter, new HashSettings { UserSpecific = false });
            filter = String.Format("Filter=Allowed files|*.{0}{1}Hash{1}{2}", filter.Replace(";", "; *."), PARAMETER_SEPARATOR, hash);
        }

        return filter;
    }


    /// <summary>
    /// Returns javascript events parameters.
    /// </summary>
    private string GetJavascriptEventsParameters()
    {
        return string.Format("OnUploadBegin={0},OnUploadCompleted={1},OnUploadProgressChanged={2},ContainerID={3}", OnUploadBegin, OnUploadCompleted, OnUploadProgressChanged, ContainerID);
    }

    #endregion
}