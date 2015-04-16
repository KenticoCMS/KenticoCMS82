using System;
using System.Web;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;

[HashValidation(HashValidationSalts.GETIMAGEVERSION_PAGE)]
public partial class CMSAdminControls_ImageEditor_GetImageVersion : GetFilePage
{
    #region "Variables"

    protected TempFileInfo tempFile = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns false - do not allow cache.
    /// </summary>
    public override bool AllowCache
    {
        get
        {
            return false;
        }
        set
        {
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        DebugHelper.SetContext("GetImageVersion");
        
        // Get the parameters
        Guid editorGuid = QueryHelper.GetGuid("editorguid", Guid.Empty);
        int num = QueryHelper.GetInteger("versionnumber", -1);

        // Load the temp file info
        if (num >= 0)
        {
            tempFile = TempFileInfoProvider.GetTempFileInfo(editorGuid, num);
        }
        else
        {
            var data = TempFileInfoProvider.GetTempFiles(null, "FileNumber DESC", 1, null);
            if (!DataHelper.DataSourceIsEmpty(data))
            {
                tempFile = new TempFileInfo(data.Tables[0].Rows[0]);
            }
        }

        // Send the data
        SendFile();

        DebugHelper.ReleaseContext();
    }


    /// <summary>
    /// Sends the given file within response.
    /// </summary>
    protected void SendFile()
    {
        // Clear response.
        CookieHelper.ClearResponseCookies();
        Response.Clear();

        Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);

        if (tempFile != null)
        {
            // Prepare etag
            string etag = "\"" + tempFile.FileID + "\"";

            // Setup the mime type - Fix the special types
            string mimetype = tempFile.FileMimeType;
            string extension = tempFile.FileExtension;
            switch (extension.ToLowerCSafe())
            {
                case ".flv":
                    mimetype = "video/x-flv";
                    break;
            }

            // Prepare response
            Response.ContentType = mimetype;
            SetDisposition(tempFile.FileNumber.ToString(), extension);

            // Setup Etag property
            ETag = etag;

            // Set if resumable downloads should be supported
            AcceptRange = !IsExtensionExcludedFromRanges(extension);

            // Add the file data
            tempFile.Generalized.EnsureBinaryData();
            WriteBytes(tempFile.FileBinary);
        }
        else
        {
            NotFound();
        }

        CompleteRequest();
    }
}