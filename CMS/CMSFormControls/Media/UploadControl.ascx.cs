using System;

using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.OnlineForms;
using CMS.SiteProvider;

public partial class CMSFormControls_Media_UploadControl : FormEngineUserControl
{
    #region "Variables"

    private string mValue;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return uploader.Enabled;
        }
        set
        {
            uploader.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            if (String.IsNullOrEmpty(mValue) || (mValue == Guid.Empty.ToString()))
            {
                return null;
            }
            return mValue;
        }
        set
        {
            mValue = ValidationHelper.GetString(value, String.Empty);
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (FieldInfo != null)
        {
            uploader.ID = FieldInfo.Name;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (Form != null)
        {
            uploader.OnUploadFile += Form.RaiseOnUploadFile;
            uploader.OnDeleteFile += Form.RaiseOnDeleteFile;
        }

        // Apply styles
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            uploader.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }
        if (!String.IsNullOrEmpty(CssClass))
        {
            uploader.CssClass = CssClass;
            CssClass = null;
        }

        // Set image auto resize configuration
        if (FieldInfo != null)
        {
            int uploaderWidth;
            int uploaderHeight;
            int uploaderMaxSideSize;
            ImageHelper.GetAutoResizeDimensions(FieldInfo.Settings, SiteContext.CurrentSiteName, out uploaderWidth, out uploaderHeight, out uploaderMaxSideSize);
            uploader.ResizeToWidth = uploaderWidth;
            uploader.ResizeToHeight = uploaderHeight;
            uploader.ResizeToMaxSideSize = uploaderMaxSideSize;
        }
        CheckFieldEmptiness = false;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Hide hidden button
        hdnPostback.Style.Add("display", "none");

        // Do not process special actions if there is no form
        if (Form == null)
        {
            return;
        }

        // ItemValue is GUID or file name (GUID + extension) when working with forms
        if (mValue.LastIndexOfCSafe(".") == -1)
        {
            Guid fileGuid = ValidationHelper.GetGuid(Value, Guid.Empty);
            if (fileGuid != Guid.Empty)
            {
                // Get the file record
                AttachmentInfo ai = Form.GetAttachment(fileGuid);
                if (ai != null)
                {
                    uploader.CurrentFileName = ai.AttachmentName;
                    if (string.IsNullOrEmpty(ai.AttachmentUrl))
                    {
                        uploader.CurrentFileUrl = "~/CMSPages/GetFile.aspx?guid=" + ai.AttachmentGUID;
                    }
                    else
                    {
                        uploader.CurrentFileUrl = ai.AttachmentUrl;
                    }

                    // Register dialog script
                    ScriptHelper.RegisterDialogScript(Page);

                    string jsFuncName;
                    string baseUrl;
                    int width;
                    int height;
                    string tooltip;

                    // Image attachment
                    if (ImageHelper.IsSupportedByImageEditor(ai.AttachmentExtension))
                    {
                        // Dialog URL for image editing
                        width = 905;
                        height = 670;
                        jsFuncName = "OpenImageEditor";
                        tooltip = ResHelper.GetString("general.editimage");
                        baseUrl = string.Format(!IsLiveSite ? "{0}/CMSModules/Content/CMSDesk/Edit/ImageEditor.aspx" : "{0}/CMSFormControls/LiveSelectors/ImageEditor.aspx", URLHelper.GetFullApplicationUrl());
                    }
                    else
                    {
                        // Dialog URL for editing metadata
                        width = 700;
                        height = 400;
                        jsFuncName = "OpenMetaEditor";
                        tooltip = ResHelper.GetString("general.edit");
                        baseUrl = string.Format(!IsLiveSite ? "{0}/CMSModules/Content/Attachments/Dialogs/MetaDataEditor.aspx" : "{0}/CMSModules/Content/Attachments/CMSPages/MetaDataEditor.aspx", URLHelper.GetFullApplicationUrl());
                    }

                    string script = "function " + jsFuncName + "(attachmentGuid, versionHistoryId, siteId, hash, clientid) {\n modalDialog('" + baseUrl + "?refresh=1&attachmentguid=' + attachmentGuid + (versionHistoryId > 0 ? '&versionhistoryid=' + versionHistoryId : '' ) + '&siteid=' + siteId + '&hash=' + hash + '&clientid=' + clientid, 'imageEditorDialog', " + width + ", " + height + ");\n return false;\n}";

                    // Dialog for attachment editing
                    ScriptHelper.RegisterClientScriptBlock(this, typeof(string), jsFuncName,
                        ScriptHelper.GetScript(script));

                    if (Form.Mode != FormModeEnum.InsertNewCultureVersion)
                    {
                        // Create security hash
                        string parameters = "?refresh=1&attachmentGUID=" + ai.AttachmentGUID;
                        if (ai.AttachmentVersionHistoryID > 0)
                        {
                            parameters += "&versionhistoryid=" + ai.AttachmentVersionHistoryID;
                        }
                        parameters += "&siteid=" + ai.AttachmentSiteID;
                        parameters += "&clientid=" + ClientID;

                        string validationHash = QueryHelper.GetHash(parameters);

                        // Setup uploader's action button - it opens image editor when clicked
                        uploader.ActionButton.Attributes.Add("onclick", string.Format("{0}('{1}', {2}, {3}, '{4}', '{5}'); return false;", jsFuncName, ai.AttachmentGUID, ai.AttachmentVersionHistoryID, ai.AttachmentSiteID, validationHash, ClientID));

                        uploader.ActionButton.ToolTip = tooltip;
                        uploader.ShowActionButton = true;

                        // Initialize refresh script
                        string refresh = string.Format("function InitRefresh_{0}() {{ {1}; if (RefreshTree != null) RefreshTree(); }}", ClientID, Page.ClientScript.GetPostBackEventReference(hdnPostback, "refresh"));
                        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "AttachmentScripts_" + ClientID, ScriptHelper.GetScript(refresh));
                    }
                    else
                    {
                        uploader.ShowActionButton = false;
                    }
                }
            }
        }
        else
        {
            uploader.CurrentFileName = (Form is BizForm) ? ((BizForm)Form).GetFileNameForUploader(mValue) : FormHelper.GetOriginalFileName(mValue);
            uploader.CurrentFileUrl = "~/CMSModules/BizForms/CMSPages/GetBizFormFile.aspx?filename=" + FormHelper.GetGuidFileName(mValue) + "&sitename=" + Form.SiteName;
        }

        if (Form != null)
        {
            // Register post back button for update panel
            if (Form.ShowImageButton && Form.SubmitImageButton.Visible)
            {
                ControlsHelper.RegisterPostbackControl(Form.SubmitImageButton);
            }
            else if (Form.SubmitButton.Visible)
            {
                ControlsHelper.RegisterPostbackControl(Form.SubmitButton);
            }
        }
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        // Check allow empty
        if ((FieldInfo != null) && !FieldInfo.AllowEmpty && ((Form == null) || Form.CheckFieldEmptiness))
        {
            if (String.IsNullOrEmpty(uploader.CurrentFileName) && (uploader.PostedFile == null))
            {
                // Error empty
                ValidationError += ResHelper.GetString("BasicForm.ErrorEmptyValue");
                return false;
            }
        }

        // Test if file has allowed file-type
        if ((uploader.PostedFile != null) && (!String.IsNullOrEmpty(uploader.PostedFile.FileName.Trim())))
        {
            string customExtension = ValidationHelper.GetString(GetValue("extensions"), String.Empty);
            string extensions = null;

            if (CMSString.Compare(customExtension, "custom", true) == 0)
            {
                extensions = ValidationHelper.GetString(GetValue("allowed_extensions"), String.Empty);
            }

            // Only extensions that are also allowed in settings can be uploaded
            extensions = UploadHelper.RestrictExtensions(extensions, SiteContext.CurrentSiteName);

            string ext = Path.GetExtension(uploader.PostedFile.FileName);
            string validationError = string.Empty;

            if (extensions.EqualsCSafe(UploadHelper.NO_ALLOWED_EXTENSION))
            {
                validationError = ResHelper.GetString("uploader.noextensionallowed");
            }
            else if (!UploadHelper.IsExtensionAllowed(ext, extensions))
            {
                validationError = string.Format(ResHelper.GetString("BasicForm.ErrorWrongFileType"), ext.TrimStart('.'), extensions.Replace(";", ", "));
            }

            if (!string.IsNullOrEmpty(validationError))
            {
                ValidationError += validationError;
                return false;
            }
        }

        return true;
    }

    #endregion
}