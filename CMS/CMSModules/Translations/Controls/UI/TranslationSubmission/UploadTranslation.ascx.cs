using System;
using System.Text;

using CMS.UIControls;
using CMS.TranslationServices;
using CMS.IO;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DataEngine;

public partial class CMSModules_Translations_Controls_UI_TranslationSubmission_UploadTranslation : CMSAdminEditControl
{
    #region "Variables"

    private bool mCheckPermissions = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// If true, permissions are checked.
    /// </summary>
    public bool CheckTranslationsPermissions
    {
        get;
        set;
    }


    /// <summary>
    /// If true, submissions are processed automatically when the submission is uploaded.
    /// </summary>
    public bool AutoImport
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets ID of the submission to upload the translated documents for.
    /// </summary>
    public int SubmissionID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets ID of the submission item to upload the translated document for.
    /// </summary>
    public int SubmissionItemID
    {
        get;
        set;
    }


    /// <summary>
    /// Submission Item Info.
    /// </summary>
    private TranslationSubmissionItemInfo SubmissionItem
    {
        get;
        set;
    }


    /// <summary>
    /// Submission Info.
    /// </summary>
    private TranslationSubmissionInfo Submission
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether to check permissions or not.
    /// </summary>
    public bool CheckContentPermissions
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

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            if (SubmissionID > 0)
            {
                Submission = TranslationSubmissionInfoProvider.GetTranslationSubmissionInfo(SubmissionID);

                if (CheckTranslationsPermissions && (Submission != null))
                {
                    if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerObject(PermissionsEnum.Modify, Submission, SiteInfoProvider.GetSiteName(Submission.SubmissionSiteID)))
                    {
                        RedirectToAccessDenied("CMS.TranslationServices", "Modify");
                    }
                }
            }

            if (Submission == null)
            {
                if (SubmissionItemID > 0)
                {
                    SubmissionItem = TranslationSubmissionItemInfoProvider.GetTranslationSubmissionItemInfo(SubmissionItemID);

                    if (CheckTranslationsPermissions && (SubmissionItem != null))
                    {
                        TranslationSubmissionInfo submission = TranslationSubmissionInfoProvider.GetTranslationSubmissionInfo(SubmissionItem.SubmissionItemSubmissionID);
                        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerObject(PermissionsEnum.Modify, submission, SiteInfoProvider.GetSiteName(submission.SubmissionSiteID)))
                        {
                            RedirectToAccessDenied("CMS.TranslationServices", "Modify");
                        }
                    }
                }
            }
            else
            {
                lblInfo.ResourceString = "translationservice.uploadzipinfo";
            }

            if ((SubmissionItem == null) && (Submission == null))
            {
                pnlContent.Visible = false;
                ShowError(GetString("general.objectnotfound"));
            }
        }
    }


    #endregion


    #region "Methods"

    /// <summary>
    /// Uploads the translations submitted.
    /// </summary>
    /// <returns>Zero if upload was not successful. Positive number if upload was successful (one if whole translation was uploaded and status can be therefore changed, two if only part of submission was uploaded).</returns>
    public bool UploadTranslation()
    {
        // Check permissions
        if (CheckContentPermissions)
        {
            var user = MembershipContext.AuthenticatedUser;
            if (!user.IsAuthorizedPerResource("CMS.Content", "Read"))
            {
                RedirectToAccessDenied("CMS.Content", "Read");
            }
            if (!user.IsAuthorizedPerResource("CMS.Content", "Create"))
            {
                RedirectToAccessDenied("CMS.Content", "Create");
            }
        }

        try
        {
            if ((uploadElem.PostedFile == null) || string.IsNullOrEmpty(uploadElem.PostedFile.FileName))
            {
                ShowError(GetString("newfile.errorempty"));
                return false;
            }

            if (SubmissionItem != null)
            {
                if (!FileInfo.New(uploadElem.PostedFile.FileName).Extension.TrimStart('.').EqualsCSafe(TranslationServiceHelper.XLIFFEXTENSION, true))
                {
                    ShowError(GetString("translationservice.xliffallowed"));
                    return false;
                }

                byte[] xliffBytes = uploadElem.FileBytes;
                if (xliffBytes != null)
                {
                    SubmissionItem.SubmissionItemTargetXLIFF = Encoding.UTF8.GetString(xliffBytes);
                    TranslationSubmissionItemInfoProvider.SetTranslationSubmissionItemInfo(SubmissionItem);

                    return true;
                }
            }
            else if (Submission != null)
            {
                if (uploadElem.PostedFile.FileName.EndsWithCSafe(".zip", true))
                {
                    string badFiles = TranslationServiceHelper.ImportXLIFFfromZIP(Submission, StreamWrapper.New(uploadElem.PostedFile.InputStream));
                    if (string.IsNullOrEmpty(badFiles))
                    {
                        // Update status of the submission to "Translation ready"
                        Submission.SubmissionStatus = TranslationStatusEnum.TranslationReady;
                        TranslationSubmissionInfoProvider.SetTranslationSubmissionInfo(Submission);

                        if (!AutoImport)
                        {
                            return true;
                        }

                        // Handle auto import
                        string importErr = TranslationServiceHelper.AutoImportSubmission(Submission);
                        if (string.IsNullOrEmpty(importErr))
                        {
                            return true;
                        }

                        ShowError(importErr);
                        return false;
                    }

                    ShowError(string.Format(GetString("translationservice.badfilesinzip"), badFiles));
                }
                else
                {
                    ShowError(GetString("translationservice.zipfileexpected"));
                }
            }
        }
        catch (Exception ex)
        {
            TranslationServiceHelper.LogEvent(ex);
            ShowError(ex.Message);
        }

        return false;
    }

    #endregion
}

