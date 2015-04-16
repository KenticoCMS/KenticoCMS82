<%@ WebHandler Language="C#" Class="MultiFileUploader.ContentUploader" %>

using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.SessionState;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.WorkflowEngine;

namespace MultiFileUploader
{
    /// <summary>
    /// Multi-file content uploader class for Http handler.
    /// </summary>
    public class ContentUploader : IHttpHandler, IRequiresSessionState
    {
        #region "Constants"

        private const int DEFAULT_OBJECT_WIDTH = 300;
        private const int DEFAULT_OBJECT_HEIGHT = 200;

        #endregion


        #region "Variables"

        protected TreeNode node = null;
        protected TreeProvider mTreeProvider = null;
        private WorkflowManager mWorkflowManager;
        private WorkflowInfo wi;
        private VersionManager mVersionManager;

        #endregion


        #region "Properties"

        /// <summary>
        /// Gets a value indicating whether another request can use the System.Web.IHttpHandler
        /// instance.
        /// </summary>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }


        /// <summary>
        /// Gets Workflow manager instance.
        /// </summary>
        protected WorkflowManager WorkflowManager
        {
            get
            {
                return mWorkflowManager ?? (mWorkflowManager = WorkflowManager.GetInstance(TreeProvider));
            }
        }


        /// <summary>
        /// Gets Version manager instance.
        /// </summary>
        protected VersionManager VersionManager
        {
            get
            {
                return mVersionManager ?? (mVersionManager = VersionManager.GetInstance(TreeProvider));
            }
        }


        /// <summary>
        /// Indicates if check-in/check-out functionality is automatic
        /// </summary>
        protected bool AutoCheck
        {
            get
            {
                if (node != null)
                {
                    // Get workflow info
                    wi = node.GetWorkflow();

                    // Check if the document uses workflow
                    if (wi != null)
                    {
                        return !wi.UseCheckInCheckOut(SiteContext.CurrentSiteName);
                    }
                }
                return false;
            }
        }


        /// <summary>
        /// Tree provider instance.
        /// </summary>
        protected TreeProvider TreeProvider
        {
            get
            {
                return mTreeProvider ?? (mTreeProvider = new TreeProvider(MembershipContext.AuthenticatedUser));
            }
        }

        #endregion


        #region "Public methods"

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                // Get arguments passed via query string
                UploaderHelper args = new UploaderHelper(context);
                String appPath = context.Server.MapPath("~/");
                DirectoryHelper.EnsureDiskPath(args.FilePath, appPath);

                if (args.Canceled)
                {
                    // Remove file from server if canceled
                    args.CleanTempFile();
                }
                else
                {
                    // Check permissions
                    switch (args.SourceType)
                    {
                        case MediaSourceEnum.Attachment:
                        case MediaSourceEnum.DocumentAttachments:
                            CheckAttachmentUploadPermissions(args);
                            break;

                        case MediaSourceEnum.Content:
                            CheckContentUploadPermissions(args);
                            break;
                    }

                    args.ProcessFile();
                    if (args.Complete && args.FileSuccessfullyProcessed)
                    {
                        switch (args.SourceType)
                        {
                            case MediaSourceEnum.Attachment:
                            case MediaSourceEnum.DocumentAttachments:
                                HandleAttachmentUpload(args, context);
                                break;

                            case MediaSourceEnum.Content:
                                HandleContentUpload(args, context);
                                break;

                            case MediaSourceEnum.PhysicalFile:
                                HandlePhysicalFilesUpload(args, context);
                                break;

                            case MediaSourceEnum.MetaFile:
                                HandleMetafileUpload(args, context);
                                break;
                        }

                        args.CleanTempFile();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                EventLogProvider.LogException("Uploader", "ProcessRequest", ex);

                // Send error message
                context.Response.Write(String.Format(@"0|{0}", HTMLHelper.EnsureLineEnding(ex.Message, " ")));
                context.Response.Flush();
            }
        }

        #endregion


        #region "Handling Methods"

        /// <summary>
        /// Provides operations necessary to create and store new cms file.
        /// </summary>
        /// <param name="args">Upload arguments.</param>
        /// <param name="context">HttpContext instance.</param>
        private void HandleContentUpload(UploaderHelper args, HttpContext context)
        {
            bool newDocumentCreated = false;
            string name = args.Name;

            try
            {
                if (args.FileArgs.NodeID == 0)
                {
                    throw new Exception(ResHelper.GetString("dialogs.document.parentmissing"));
                }

                // Check if class exists
                DataClassInfo ci = DataClassInfoProvider.GetDataClassInfo("CMS.File");
                if (ci == null)
                {
                    throw new Exception(string.Format(ResHelper.GetString("dialogs.newfile.classnotfound"), "CMS.File"));
                }

                if (args.FileArgs.IncludeExtension)
                {
                    name += args.Extension;
                }

                // Make sure the file name with extension respects maximum file name
                name = TreePathUtils.EnsureMaxFileNameLength(name, ci.ClassName);

                node = TreeNode.New("CMS.File", TreeProvider);
                node.DocumentCulture = args.FileArgs.Culture;
                node.DocumentName = name;
                if (args.FileArgs.NodeGroupID > 0)
                {
                    node.SetValue("NodeGroupID", args.FileArgs.NodeGroupID);
                }

                // Load default values
                FormHelper.LoadDefaultValues(node.NodeClassName, node);

                node.SetValue("FileDescription", "");
                node.SetValue("FileName", name);
                node.SetValue("FileAttachment", Guid.Empty);
                node.SetValue("DocumentType", args.Extension);

                node.SetDefaultPageTemplateID(ci.ClassDefaultPageTemplateID);

                // Insert the document
                var parent = DocumentHelper.GetDocument(args.FileArgs.NodeID, TreeProvider.ALL_CULTURES, TreeProvider);
                DocumentHelper.InsertDocument(node, parent, TreeProvider);
                newDocumentCreated = true;

                // Add the attachment data
                DocumentHelper.AddAttachment(node, "FileAttachment", args.FilePath, TreeProvider, args.ResizeToWidth, args.ResizeToHeight, args.ResizeToMaxSide);

                // Create default SKU if configured
                if (ModuleManager.CheckModuleLicense(ModuleName.ECOMMERCE, RequestContext.CurrentDomain, FeatureEnum.Ecommerce, ObjectActionEnum.Insert))
                {
                    node.CreateDefaultSKU();
                }

                DocumentHelper.UpdateDocument(node, TreeProvider);

                // Get workflow info
                wi = node.GetWorkflow();

                // Check if auto publish changes is allowed
                if ((wi != null) && wi.WorkflowAutoPublishChanges && !wi.UseCheckInCheckOut(SiteContext.CurrentSiteName))
                {
                    // Automatically publish document
                    node.MoveToPublishedStep();
                }
            }
            catch (Exception ex)
            {
                // Delete the document if something failed
                if (newDocumentCreated && (node != null) && (node.DocumentID > 0))
                {
                    DocumentHelper.DeleteDocument(node, TreeProvider, false, true, true);
                }

                args.Message = ex.Message;

                // Log the error
                EventLogProvider.LogException("MultiFileUploader", "UPLOADATTACHMENT", ex);
            }
            finally
            {
                // Create node info string
                string nodeInfo = ((node != null) && (node.NodeID > 0) && args.IncludeNewItemInfo) ? String.Format("'{0}', ", node.NodeID) : "";

                // Ensure message text
                args.Message = HTMLHelper.EnsureLineEnding(args.Message, " ");

                // Call function to refresh parent window  
                if (!string.IsNullOrEmpty(args.AfterSaveJavascript))
                {
                    // Calling javascript function with parameters attachments url, name, width, height
                    args.AfterScript += string.Format(@"
                    if (window.{0} != null)
                    {{
                        window.{0}();
                    }}
                    else if((window.parent != null) && (window.parent.{0} != null))
                    {{
                        window.parent.{0}();
                    }}", args.AfterSaveJavascript);
                }

                // Create after script and return it to the silverlight application, this script will be evaluated by the SL application in the end
                args.AfterScript += string.Format(@"
                if (window.InitRefresh_{0} != null)
                {{
                    window.InitRefresh_{0}('{1}', false, false, {2});
                }}
                else {{ 
                    if ('{1}' != '') {{
                        alert('{1}');
                    }}
                }}",
                                                  args.ParentElementID,
                                                  ScriptHelper.GetString(args.Message.Trim(), false),
                                                  nodeInfo + (args.IsInsertMode ? "'insert'" : "'update'"));

                args.AddEventTargetPostbackReference();
                context.Response.Write(args.AfterScript);
                context.Response.Flush();
            }
        }


        /// <summary>
        /// Provides operations necessary to create and store new metafile.
        /// </summary>
        /// <param name="args">Upload arguments.</param>
        /// <param name="context">HttpContext instance.</param>
        private void HandleMetafileUpload(UploaderHelper args, HttpContext context)
        {
            MetaFileInfo mfi = null;

            try
            {
                if (args.IsInsertMode)
                {
                    // Create new metafile info
                    mfi = new MetaFileInfo(args.FilePath, args.MetaFileArgs.ObjectID, args.MetaFileArgs.ObjectType, args.MetaFileArgs.Category);
                    mfi.MetaFileSiteID = args.MetaFileArgs.SiteID;
                }
                else
                {
                    if (args.MetaFileArgs.MetaFileID > 0)
                    {
                        mfi = MetaFileInfoProvider.GetMetaFileInfo(args.MetaFileArgs.MetaFileID);
                    }
                    else
                    {
                        DataSet ds = MetaFileInfoProvider.GetMetaFilesWithoutBinary(args.MetaFileArgs.ObjectID, args.MetaFileArgs.ObjectType, args.MetaFileArgs.Category, null, null);
                        if (!DataHelper.DataSourceIsEmpty(ds))
                        {
                            mfi = new MetaFileInfo(ds.Tables[0].Rows[0]);
                        }
                    }

                    if (mfi != null)
                    {
                        FileInfo fileInfo = FileInfo.New(args.FilePath);
                        // Init the MetaFile data
                        mfi.MetaFileName = URLHelper.GetSafeFileName(fileInfo.Name, null);
                        mfi.MetaFileExtension = fileInfo.Extension;

                        FileStream file = fileInfo.OpenRead();
                        mfi.MetaFileSize = Convert.ToInt32(fileInfo.Length);
                        mfi.MetaFileMimeType = MimeTypeHelper.GetMimetype(mfi.MetaFileExtension);
                        mfi.InputStream = file;

                        // Set image properties
                        if (ImageHelper.IsImage(mfi.MetaFileExtension))
                        {
                            ImageHelper ih = new ImageHelper(mfi.MetaFileBinary);
                            mfi.MetaFileImageHeight = ih.ImageHeight;
                            mfi.MetaFileImageWidth = ih.ImageWidth;
                        }
                    }
                }

                if (mfi != null)
                {
                    using (var actionContext = new CMSActionContext())
                    {
                        actionContext.CreateVersion = args.IsLastUpload;
                        MetaFileInfoProvider.SetMetaFileInfo(mfi);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                EventLogProvider.LogException("Uploader", "UploadMetaFile", ex);

                args.Message = ex.Message;
            }
            finally
            {
                if (String.IsNullOrEmpty(args.Message))
                {
                    if (!string.IsNullOrEmpty(args.AfterSaveJavascript))
                    {
                        args.AfterScript = String.Format(@"
                        if (window.{0} != null) {{
                            window.{0}()
                        }} else if ((window.parent != null) && (window.parent.{0} != null)) {{
                            window.parent.{0}() 
                        }}", args.AfterSaveJavascript);
                    }
                    else
                    {
                        args.AfterScript = String.Format(@"
                        if (window.InitRefresh_{0})
                        {{
                            window.InitRefresh_{0}('{1}', false, false, {2});
                        }}
                        else {{ 
                            if ('{1}' != '') {{
                                alert('{1}');
                            }}
                        }}", args.ParentElementID, ScriptHelper.GetString(args.Message.Trim(), false), mfi.MetaFileID);
                    }
                }
                else
                {
                    args.AfterScript += ScriptHelper.GetAlertScript(args.Message, false);
                }

                args.AddEventTargetPostbackReference();
                context.Response.Write(args.AfterScript);
                context.Response.Flush();
            }
        }


        /// <summary>
        /// Provides operations necessary to create and store new physical file.
        /// </summary>
        /// <param name="args">Upload arguments.</param>
        /// <param name="context">HttpContext instance.</param>
        private void HandlePhysicalFilesUpload(UploaderHelper args, HttpContext context)
        {
            try
            {
                // Prepare the file name
                string extension = args.Extension;
                string fileName = args.TargetFileName;
                if (String.IsNullOrEmpty(fileName))
                {
                    fileName = args.FileName;
                }
                else if (!fileName.Contains("."))
                {
                    fileName += args.Extension;
                }

                // Prepare the path
                string filePath = String.IsNullOrEmpty(args.TargetFolderPath) ? "~/" : args.TargetFolderPath;

                // Try to map virtual and relative path to server
                try
                {
                    if (!Path.IsPathRooted(filePath))
                    {
                        filePath = context.Server.MapPath(filePath);
                    }
                }
                catch
                {
                }

                filePath = DirectoryHelper.CombinePath(filePath, fileName);

                // Ensure directory
                DirectoryHelper.EnsureDiskPath(filePath, SystemContext.WebApplicationPhysicalPath);

                // Ensure unique file name
                if (String.IsNullOrEmpty(args.TargetFileName) && File.Exists(filePath))
                {
                    int index = 0;
                    string basePath = filePath.Substring(0, filePath.Length - extension.Length);
                    string newPath;

                    do
                    {
                        index++;
                        newPath = String.Format("{0}_{1}{2}", basePath, index, extension);
                    }
                    while (File.Exists(newPath));

                    filePath = newPath;
                }

                // Copy file
                args.CopyFile(filePath);
            }
            catch (Exception ex)
            {
                // Log the exception
                EventLogProvider.LogException("Uploader", "UploadPhysicalFile", ex);

                // Store exception message
                args.Message = ex.Message;
            }
            finally
            {
                if (String.IsNullOrEmpty(args.Message))
                {
                    if (!string.IsNullOrEmpty(args.AfterSaveJavascript))
                    {
                        args.AfterScript = String.Format(@"
                        if (window.{0} != null) {{
                            window.{0}()
                        }} else if ((window.parent != null) && (window.parent.{0} != null)) {{
                            window.parent.{0}() 
                        }}", args.AfterSaveJavascript);
                    }
                    else
                    {
                        args.AfterScript = String.Format(@"
                        if (window.InitRefresh_{0})
                        {{
                            window.InitRefresh_{0}('{1}', false, false);
                        }}
                        else {{ 
                            if ('{1}' != '') {{
                                alert('{1}');
                            }}
                        }}", args.ParentElementID, ScriptHelper.GetString(args.Message.Trim(), false));
                    }
                }
                else
                {
                    args.AfterScript += ScriptHelper.GetAlertScript(args.Message, false);
                }

                args.AddEventTargetPostbackReference();
                context.Response.Write(args.AfterScript);
                context.Response.Flush();
            }
        }


        /// <summary>
        /// Provides operations necessary to create and store new attachment.
        /// </summary>
        /// <param name="args">Upload arguments.</param>
        /// <param name="context">HttpContext instance.</param>
        private void HandleAttachmentUpload(UploaderHelper args, HttpContext context)
        {
            AttachmentInfo newAttachment = null;
            bool refreshTree = false;

            try
            {
                // Get existing document
                if (args.AttachmentArgs.DocumentID != 0)
                {
                    node = DocumentHelper.GetDocument(args.AttachmentArgs.DocumentID, TreeProvider);
                    if (node == null)
                    {
                        throw new Exception("Given document doesn't exist!");
                    }
                    
                    // Check out the document when first attachment is uploaded
                    if (AutoCheck && args.IsFirstUpload)
                    {
                        VersionManager.CheckOut(node, node.IsPublished, true);
                    }

                    // Handle field attachment
                    if (args.AttachmentArgs.FieldAttachment)
                    {
                        // Extension of CMS file before saving
                        string oldExtension = node.DocumentType;

                        newAttachment = DocumentHelper.AddAttachment(node, args.AttachmentArgs.AttachmentGuidColumnName, Guid.Empty, Guid.Empty, args.FilePath, TreeProvider, args.ResizeToWidth, args.ResizeToHeight, args.ResizeToMaxSide);
                        DocumentHelper.UpdateDocument(node, TreeProvider);

                        // Different extension
                        if ((oldExtension != null) && !oldExtension.EqualsCSafe(node.DocumentType, true))
                        {
                            refreshTree = true;
                        }
                    }
                    else
                    {
                        // Handle grouped and unsorted attachments
                        if (args.AttachmentArgs.AttachmentGroupGuid != Guid.Empty)
                        {
                            // Grouped attachment
                            newAttachment = DocumentHelper.AddGroupedAttachment(node, args.AttachmentArgs.AttachmentGUID, args.AttachmentArgs.AttachmentGroupGuid, args.FilePath, TreeProvider, args.ResizeToWidth, args.ResizeToHeight, args.ResizeToMaxSide);
                        }
                        else
                        {
                            // Unsorted attachment
                            newAttachment = DocumentHelper.AddUnsortedAttachment(node, args.AttachmentArgs.AttachmentGUID, args.FilePath, TreeProvider, args.ResizeToWidth, args.ResizeToHeight, args.ResizeToMaxSide);
                        }

                        // Log synchronization task if not under workflow
                        if (wi == null)
                        {
                            DocumentSynchronizationHelper.LogDocumentChange(node, TaskTypeEnum.UpdateDocument, TreeProvider);
                        }
                    }

                    // Check in the document after last attachment was uploaded
                    if (AutoCheck && args.IsLastUpload)
                    {
                        VersionManager.CheckIn(node, null);

                        // Get current step info
                        WorkflowStepInfo si = WorkflowManager.GetStepInfo(node);
                        if (si != null)
                        {
                            // Decide if full refresh is needed
                            args.FullRefresh = si.StepIsPublished || si.StepIsArchived;
                        }
                    }
                }
                else if (args.AttachmentArgs.FormGuid != Guid.Empty)
                {
                    newAttachment = AttachmentInfoProvider.AddTemporaryAttachment(args.AttachmentArgs.FormGuid, args.AttachmentArgs.AttachmentGuidColumnName, args.AttachmentArgs.AttachmentGUID, args.AttachmentArgs.AttachmentGroupGuid, args.FilePath, SiteContext.CurrentSiteID, args.ResizeToWidth, args.ResizeToHeight, args.ResizeToMaxSide);
                }

                if (newAttachment == null)
                {
                    throw new Exception("The attachment hasn't been created since no DocumentID or FormGUID was supplied.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                EventLogProvider.LogException("Content", "UploadAttachment", ex);

                // Store exception message
                args.Message = ex.Message;
            }
            finally
            {
                // Call after save javascript if exists
                if (!string.IsNullOrEmpty(args.AfterSaveJavascript))
                {
                    if ((args.Message == string.Empty) && (newAttachment != null))
                    {
                        string url = null;
                        string safeName = URLHelper.GetSafeFileName(newAttachment.AttachmentName, SiteContext.CurrentSiteName);
                        if (node != null)
                        {
                            SiteInfo si = SiteInfoProvider.GetSiteInfo(node.NodeSiteID);
                            if (si != null)
                            {
                                url = DocumentURLProvider.UsePermanentUrls(si.SiteName) ?
                                    URLHelper.ResolveUrl(AttachmentInfoProvider.GetAttachmentUrl(newAttachment.AttachmentGUID, safeName))
                                    : URLHelper.ResolveUrl(AttachmentInfoProvider.GetAttachmentUrl(safeName, node.NodeAliasPath));
                            }
                        }
                        else
                        {
                            url = URLHelper.ResolveUrl(AttachmentInfoProvider.GetAttachmentUrl(newAttachment.AttachmentGUID, safeName));
                        }

                        Hashtable obj = new Hashtable();
                        if (ImageHelper.IsImage(newAttachment.AttachmentExtension))
                        {
                            obj[DialogParameters.IMG_URL] = url;
                            obj[DialogParameters.IMG_TOOLTIP] = newAttachment.AttachmentName;
                            obj[DialogParameters.IMG_WIDTH] = newAttachment.AttachmentImageWidth;
                            obj[DialogParameters.IMG_HEIGHT] = newAttachment.AttachmentImageHeight;
                        }
                        else if (MediaHelper.IsFlash(newAttachment.AttachmentExtension))
                        {
                            obj[DialogParameters.OBJECT_TYPE] = "flash";
                            obj[DialogParameters.FLASH_URL] = url;
                            obj[DialogParameters.FLASH_EXT] = newAttachment.AttachmentExtension;
                            obj[DialogParameters.FLASH_TITLE] = newAttachment.AttachmentName;
                            obj[DialogParameters.FLASH_WIDTH] = DEFAULT_OBJECT_WIDTH;
                            obj[DialogParameters.FLASH_HEIGHT] = DEFAULT_OBJECT_HEIGHT;
                        }
                        else if (MediaHelper.IsAudioVideo(newAttachment.AttachmentExtension))
                        {
                            obj[DialogParameters.OBJECT_TYPE] = "audiovideo";
                            obj[DialogParameters.AV_URL] = url;
                            obj[DialogParameters.AV_EXT] = newAttachment.AttachmentExtension;
                            obj[DialogParameters.AV_WIDTH] = DEFAULT_OBJECT_WIDTH;
                            obj[DialogParameters.AV_HEIGHT] = DEFAULT_OBJECT_HEIGHT;
                        }
                        else
                        {
                            obj[DialogParameters.LINK_URL] = url;
                            obj[DialogParameters.LINK_TEXT] = newAttachment.AttachmentName;
                        }

                        // Calling javascript function with parameters attachments url, name, width, height
                        args.AfterScript += string.Format(@"{5}
                        if (window.{0})
                        {{
                            window.{0}('{1}', '{2}', '{3}', '{4}', obj);
                        }}
                        else if((window.parent != null) && window.parent.{0})
                        {{
                            window.parent.{0}('{1}', '{2}', '{3}', '{4}', obj);
                        }}", args.AfterSaveJavascript, url, newAttachment.AttachmentName, newAttachment.AttachmentImageWidth, newAttachment.AttachmentImageHeight, CMSDialogHelper.GetDialogItem(obj));
                    }
                    else
                    {
                        args.AfterScript += ScriptHelper.GetAlertScript(args.Message, false);
                    }
                }

                // Create attachment info string
                string attachmentInfo = ((newAttachment != null) && (newAttachment.AttachmentGUID != Guid.Empty) && (args.IncludeNewItemInfo)) ? String.Format("'{0}', ", newAttachment.AttachmentGUID) : "";

                // Create after script and return it to the silverlight application, this script will be evaluated by the SL application in the end
                args.AfterScript += string.Format(@"
                if (window.InitRefresh_{0})
                {{
                    window.InitRefresh_{0}('{1}', {2}, {3}, {4});
                }}
                else {{ 
                    if ('{1}' != '') {{
                        alert('{1}');
                    }}
                }}",
                    args.ParentElementID,
                    ScriptHelper.GetString(args.Message.Trim(), false),
                    args.FullRefresh.ToString().ToLowerCSafe(),
                    refreshTree.ToString().ToLowerCSafe(),
                    attachmentInfo + (args.IsInsertMode ? "'insert'" : "'update'"));

                args.AddEventTargetPostbackReference();
                context.Response.Write(args.AfterScript);
                context.Response.Flush();
            }
        }

        #endregion


        #region "Permissions check methods"

        /// <summary>
        /// Checks permissions for uploading attachments.
        /// </summary>
        /// <param name="args">Upload arguments.</param>
        private void CheckAttachmentUploadPermissions(UploaderHelper args)
        {
            // For new document
            if (args.AttachmentArgs.FormGuid != Guid.Empty)
            {
                if (args.AttachmentArgs.ParentNodeID == 0)
                {
                    throw new Exception(ResHelper.GetString("attach.document.parentmissing"));
                }

                if (!MembershipContext.AuthenticatedUser.IsAuthorizedToCreateNewDocument(args.AttachmentArgs.ParentNodeID, args.AttachmentArgs.NodeClassName))
                {
                    throw new Exception(ResHelper.GetString("attach.actiondenied"));
                }
            }
            // For existing document
            else if (args.AttachmentArgs.DocumentID > 0)
            {
                node = DocumentHelper.GetDocument(args.AttachmentArgs.DocumentID, TreeProvider);
                if (node == null)
                {
                    throw new Exception("Given document doesn't exist!");
                }
                else
                {
                    if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
                    {
                        throw new Exception(ResHelper.GetString("attach.actiondenied"));
                    }
                }
            }
        }


        /// <summary>
        /// Checks permissions for uploading to content.
        /// </summary>
        /// <param name="args">Upload arguments.</param>
        private void CheckContentUploadPermissions(UploaderHelper args)
        {
            // Check license limitations
            if (!LicenseHelper.LicenseVersionCheck(RequestContext.CurrentDomain, FeatureEnum.Documents, ObjectActionEnum.Insert))
            {
                throw new Exception(ResHelper.GetString("cmsdesk.documentslicenselimits"));
            }

            // Check user permissions
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedToCreateNewDocument(args.FileArgs.NodeID, "CMS.File"))
            {
                throw new Exception(string.Format(ResHelper.GetString("dialogs.newfile.notallowed"), "CMS.File"));
            }

            // Check if class exists
            DataClassInfo ci = DataClassInfoProvider.GetDataClassInfo("CMS.File");
            if (ci == null)
            {
                throw new Exception(string.Format(ResHelper.GetString("dialogs.newfile.classnotfound"), "CMS.File"));
            }

            // Get the node
            using (TreeNode parentNode = TreeProvider.SelectSingleNode(args.FileArgs.NodeID, args.FileArgs.Culture, true))
            {
                if (parentNode != null)
                {
                    // Check whether node class is allowed on site and parent node
                    if (!DocumentHelper.IsDocumentTypeAllowed(parentNode, ci.ClassID) || (ClassSiteInfoProvider.GetClassSiteInfo(ci.ClassID, parentNode.NodeSiteID) == null))
                    {
                        throw new Exception(ResHelper.GetString("Content.ChildClassNotAllowed"));
                    }
                }

                // Check user permissions
                if (!MembershipContext.AuthenticatedUser.IsAuthorizedToCreateNewDocument(parentNode, "CMS.File"))
                {
                    throw new Exception(string.Format(ResHelper.GetString("dialogs.newfile.notallowed"), args.AttachmentArgs.NodeClassName));
                }
            }
        }

        #endregion
    }
}