using System;

using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.SiteProvider;


/// <summary>
/// Form control providing functionality for defining an image by a meta file or a font icon.
/// </summary>
public partial class CMSFormControls_Metafiles_MetafileOrFontIconSelector : FormEngineUserControl
{
    #region "Variables"

    private IconTypeEnum iconType = IconTypeEnum.Metafile;
    private string mValue = null;
    private BaseInfo mFormObject = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return mValue;
        }
        set
        {
            mValue = ValidationHelper.GetString(value, null);
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Currently edited object in the form.
    /// </summary>
    private BaseInfo FormObject
    {
        get
        {
            if ((mFormObject == null) && (Form != null))
            {
                mFormObject = Form.Data as BaseInfo;
            }

            return mFormObject;
        }
    }


    /// <summary>
    /// Gets or sets the name of the column which stores the icon CSS class.
    /// </summary>
    private string IconCssFieldName
    {
        get
        {
            return GetValue("IconCssFieldName", String.Empty);
        }
        set
        {
            SetValue("IconCssFieldName", value);
        }
    }
    

    /// <summary>
    /// Gets or sets the category of the image.
    /// </summary>
    private string Category
    {
        get
        {
            return GetValue("Category", ObjectAttachmentsCategories.THUMBNAIL);
        }
        set
        {
            SetValue("Category", value);
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        lstOptions.SelectedIndexChanged += lstOptions_SelectedIndexChanged;

        if (Form != null)
        {
            Form.OnBeforeSave += Form_OnBeforeSave;
            Form.OnAfterSave += Form_OnAfterSave;

            if (!RequestHelper.IsPostBack())
            {
                InitializeControl();
            }

            iconType = lstOptions.SelectedValue.ToEnum<IconTypeEnum>();
        }

        CheckFieldEmptiness = false;
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        // Setup control visibility
        switch (iconType)
        {
            case IconTypeEnum.CssClass:
                plcMetaFile.Visible = false;
                plcCssClass.Visible = true;
                break;

            case IconTypeEnum.Metafile:
                plcMetaFile.Visible = true;
                plcCssClass.Visible = false;
                break;
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Handles the SelectedIndexChanged event of the lstOptions control.
    /// </summary>
    private void lstOptions_SelectedIndexChanged(object sender, EventArgs e)
    {
        iconType = lstOptions.SelectedValue.ToEnum<IconTypeEnum>();
    }


    /// <summary>
    /// Handles the OnBeforeSave event of the Form control.
    /// </summary>
    private void Form_OnBeforeSave(object sender, EventArgs e)
    {
        if (FormObject != null)
        {
            switch (iconType)
            {
                case IconTypeEnum.Metafile:
                    // Remove icon css class
                    FormObject.SetValue(IconCssFieldName, null);
                    txtCssClass.Text = string.Empty;
                    break;

                case IconTypeEnum.CssClass:
                    // Delete uploaded metafile
                    Guid metaFileguid = ValidationHelper.GetGuid(Value, Guid.Empty);
                    if (metaFileguid != Guid.Empty)
                    {
                        MetaFileInfo metaFile = MetaFileInfoProvider.GetMetaFileInfo(metaFileguid, null, true);
                        MetaFileInfoProvider.DeleteMetaFileInfo(metaFile);
                    }

                    // Delete the metafile thumbnail
                    Value = null;
                    FormObject.SetValue(Field, null);

                    // Update the Icon CSS class field
                    FormObject.SetValue(IconCssFieldName, txtCssClass.Text);
                    break;
            }
        }
    }


    /// <summary>
    /// Handles the OnAfterSave event of the Form control.
    /// </summary>
    private void Form_OnAfterSave(object sender, EventArgs e)
    {
        if (Form.Mode == FormModeEnum.Insert)
        {
            InitializeUploadControl();

            // Upload new metafile
            fileUploader.UploadFile();
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Loads the form control data.
    /// </summary>
    private void InitializeControl()
    {
        if (FormObject != null)
        {
            // Icon CSS class
            txtCssClass.Text = FormObject.GetStringValue(IconCssFieldName, null);
            txtCssClass.ToolTip = GetString("fontIconCss.tooltip");

            // Load file uploader
            fileUploader.Category = Category;
            fileUploader.ObjectType = FormObject.TypeInfo.ObjectType;
            fileUploader.ObjectID = FormObject.Generalized.ObjectID;

            if (FormObject.Generalized.ObjectSiteID > 0)
            {
                fileUploader.SiteID = FormObject.Generalized.ObjectSiteID;
            }

            // Identify the currently selected icon type
            iconType = IconTypeEnum.Metafile;
            if (!string.IsNullOrEmpty(txtCssClass.Text))
            {
                iconType = IconTypeEnum.CssClass;
            }

            lstOptions.SelectedValue = iconType.ToStringRepresentation();
        }

        InitializeUploadControl();
    }


    /// <summary>
    /// Initializes the file upload control.
    /// </summary>
    private void InitializeUploadControl()
    {
        if (Form.EditedObject is BaseInfo)
        {
            BaseInfo info = Form.EditedObject as BaseInfo;
            if (info != null)
            {
                fileUploader.ObjectType = info.TypeInfo.ObjectType;

                if (info.Generalized.ObjectSiteID > 0)
                {
                    fileUploader.SiteID = info.Generalized.ObjectSiteID;
                }

                fileUploader.ObjectID = info.Generalized.ObjectID;
            }
        }
        else if (Form.EditedObject is IDataClass)
        {
            IDataClass item = Form.EditedObject as IDataClass;

            if (item != null)
            {
                fileUploader.ObjectType = item.ClassName;

                if (!string.IsNullOrEmpty(Form.SiteName))
                {
                    fileUploader.SiteID = SiteInfoProvider.GetSiteID(Form.SiteName);
                }

                fileUploader.ObjectID = item.ID;
            }
        }
    }

    #endregion


    #region "Icon type enum"

    /// <summary>
    /// Internal enum - used for distinguishing between icon types
    /// </summary>
    private enum IconTypeEnum
    {
        /// <summary>
        /// Icon is defined by a metafile
        /// </summary>
        [EnumDefaultValue]
        [EnumStringRepresentation("metafile")]
        Metafile,

        /// <summary>
        /// Icon is a font icon and is defined by a css class
        /// </summary>
        [EnumStringRepresentation("cssclass")]
        CssClass
    }

    #endregion
}