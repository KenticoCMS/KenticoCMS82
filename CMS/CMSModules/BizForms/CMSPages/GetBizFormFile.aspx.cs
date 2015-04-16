using System;

using CMS.FormEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.UIControls;

public partial class CMSModules_BizForms_CMSPages_GetBizFormFile : GetFilePage
{
    /// <summary>
    /// GetFilePage forces to implement AllowCache property.
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


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check 'ReadData' permission
        if (!CurrentUser.IsAuthorizedPerResource("cms.form", "ReadData"))
        {
            return;
        }

        // Get parameters
        string fileName = QueryHelper.GetString("filename", String.Empty);
        string siteName = QueryHelper.GetString("sitename", CurrentSiteName);

        // Check parameters
        if ((!ValidationHelper.IsFileName(fileName)) || (siteName == null))
        {
            return;
        }

        // Check physical path to the file
        string filePath = FormHelper.GetFilePhysicalPath(siteName, fileName);
        if (!File.Exists(filePath))
        {
            return;
        }

        // Clear response
        CookieHelper.ClearResponseCookies();
        Response.Clear();

        // Prepare response
        string extension = Path.GetExtension(filePath);
        Response.ContentType = MimeTypeHelper.GetMimetype(extension);

        // Set the file disposition
        SetDisposition(fileName, extension);

        // Get file binary from file system
        WriteFile(filePath);

        CompleteRequest();
    }
}