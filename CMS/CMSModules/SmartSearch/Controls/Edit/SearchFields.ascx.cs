using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Collections;

using CMS.FormEngine;
using CMS.Helpers;
using CMS.Base;
using CMS.Search;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.DataEngine;

public partial class CMSModules_SmartSearch_Controls_Edit_SearchFields : CMSAdminEditControl
{
    #region "Private variables"

    private DataClassInfo dci = null;
    private DataClassInfo document = null;
    private ArrayList attributes = new ArrayList();
    private FormInfo fi = null;
    private bool mLoadActualValues = false;
    private bool mAdvancedMode = true;

    // Contains item list for 'Title' drop-down list.
    private string allowedTitles = "DocumentName;DocumentNamePath;DocumentUrlPath;DocumentPageTitle;DocumentPageDescription;DocumentMenuCaption;DocumentCustomData;DocumentTags;NodeAliasPath;NodeName;NodeAlias;NodeCustomData;SKUNumber;SKUName;SKUDescription;SKUImagePath;SKUCustomData";

    // Contains item list for 'Content' drop-down list.
    private string allowedContent = "DocumentName;DocumentNamePath;DocumentUrlPath;DocumentPageTitle;DocumentPageDescription;DocumentMenuCaption;DocumentContent;DocumentCustomData;DocumentTags;NodeAliasPath;NodeName;NodeAlias;NodeCustomData;SKUNumber;SKUName;SKUDescription;SKUImagePath;SKUCustomData";

    // Contains item list for 'Image' field
    private string allowedImage = "DocumentContent;SKUImagePath";

    // Contains item list for 'Date' drop-down list.
    private string allowedDate = "DocumentModifiedWhen;DocumentCreatedWhen;DocumentCheckedOutWhen;DocumentPublishFrom;DocumentPublishTo;SKULastModified;SKUCreated";

    private string mSaveResourceString = "general.changessaved";
    private string mRebuildIndexResourceString = "searchindex.requiresrebuild";

    #endregion


    #region "Public properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether dropdown lists should be
    /// filled by actual object values or document values only
    /// </summary>
    public bool LoadActualValues
    {
        get
        {
            return mLoadActualValues;
        }
        set
        {
            mLoadActualValues = value;
        }
    }


    /// <summary>
    /// Gets or sets the resource string which is displayed after the save action.
    /// </summary>
    public string SaveResourceString
    {
        get
        {
            return mSaveResourceString;
        }
        set
        {
            mSaveResourceString = value;
        }
    }


    /// <summary>
    /// Resource text for rebuild index label.
    /// </summary>
    public string RebuildIndexResourceString
    {
        get
        {
            return mRebuildIndexResourceString;
        }
        set
        {
            mRebuildIndexResourceString = value;
        }
    }


    /// <summary>
    /// Indicates if advanced mode is used.
    /// </summary>
    public bool AdvancedMode
    {
        get
        {
            return mAdvancedMode;
        }
        set
        {
            mAdvancedMode = value;
        }
    }


    /// <summary>
    /// Gets current class.
    /// </summary>
    private DataClassInfo ClassInfo
    {
        get
        {
            if (dci == null)
            {
                dci = DataClassInfoProvider.GetDataClassInfo(ItemID);
            }
            return dci;
        }
    }


    /// <summary>
    /// Indicates if control is enabled.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return pnlBody.Enabled;
        }
        set
        {
            pnlBody.Enabled = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        ClassFields.OnSaved += ClassFields_OnSaved;
        ClassFields.DisplaySaved = false;

        // Setup controls
        if (!URLHelper.IsPostback() && (ClassInfo != null))
        {
            pnlSearchFields.Visible = chkSearchEnabled.Checked = ClassInfo.ClassSearchEnabled;
        }

        plcAdvancedMode.Visible = AdvancedMode;

        if (!URLHelper.IsPostback())
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reloads data in control.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        ReloadSearch(false);
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    /// <param name="setAutomatically">Indicates whether search options should be set automatically</param>
    public void ReloadSearch(bool setAutomatically)
    {
        ClassFields.ItemID = ItemID;
        ClassFields.ReloadData(setAutomatically, true);

        // Initialize properties
        List<IField> itemList = null;

        if (ClassInfo != null)
        {
            // Load XML definition
            fi = FormHelper.GetFormInfo(ClassInfo.ClassName, true);

            if (CMSString.Compare(ClassInfo.ClassName, "cms.user", true) == 0)
            {
                plcImage.Visible = false;
                ClassFields.DisplaySetAutomatically = false;
                pnlIndent.Visible = true;

                document = DataClassInfoProvider.GetDataClassInfo("cms.usersettings");
                if (document != null)
                {
                    FormInfo fiSettings = FormHelper.GetFormInfo(document.ClassName, true);
                    fi.CombineWithForm(fiSettings, true, String.Empty);
                }
            }

            // Get all fields
            itemList = fi.GetFormElements(true, true);
        }

        if (itemList != null)
        {
            if (itemList.Any())
            {
                pnlIndent.Visible = true;
            }

            // Store each field to array
            foreach (var item in itemList)
            {
                var formField = item as FormFieldInfo;
                if (formField != null)
                {
                    object[] obj = { formField.Name, DataTypeManager.GetSystemType(TypeEnum.Field, formField.DataType) };
                    attributes.Add(obj);
                }
            }
        }

        if (AdvancedMode)
        {
            ReloadControls();
        }
    }


    /// <summary>
    /// Reloads drop-down lists with new data.
    /// </summary>
    private void ReloadControls()
    {
        if ((ClassInfo != null))
        {
            #region "Load drop-down list 'Title field'"

            drpTitleField.Items.Clear();
            string[] array;

            if (!LoadActualValues)
            {
                array = allowedTitles.Split(';');
                foreach (string item in array)
                {
                    if (!String.IsNullOrEmpty(item))
                    {
                        drpTitleField.Items.Add(new ListItem(item));
                    }
                }
            }

            foreach (object[] item in attributes)
            {
                object[] obj = item;
                drpTitleField.Items.Add(new ListItem(obj[0].ToString()));
            }

            // Preselect value
            if (!String.IsNullOrEmpty(ClassInfo.ClassSearchTitleColumn))
            {
                drpTitleField.SelectedValue = ClassInfo.ClassSearchTitleColumn;
            }
            else
            {
                if (!LoadActualValues)
                {
                    drpTitleField.SelectedValue = SearchHelper.DEFAULT_SEARCH_TITLE_COLUMN;
                }
            }

            #endregion


            #region "Load drop-down list 'Content field'"

            drpContentField.Items.Clear();

            if (!LoadActualValues)
            {
                array = allowedContent.Split(';');
                foreach (string item in array)
                {
                    if (!String.IsNullOrEmpty(item))
                    {
                        drpContentField.Items.Add(new ListItem(item));
                    }
                }
            }
            else
            {
                drpContentField.Items.Add(new ListItem(GetString("general.selectnone"), "0"));
            }

            foreach (object[] item in attributes)
            {
                object[] obj = item;
                drpContentField.Items.Add(new ListItem(obj[0].ToString()));
            }

            // Preselect value
            if (!String.IsNullOrEmpty(ClassInfo.ClassSearchContentColumn))
            {
                drpContentField.SelectedValue = ClassInfo.ClassSearchContentColumn;
            }
            else
            {
                if (!LoadActualValues)
                {
                    drpContentField.SelectedValue = SearchHelper.DEFAULT_SEARCH_CONTENT_COLUMN;
                }
            }

            #endregion


            #region "Load drop-down list 'Image field'"

            drpImageField.Items.Clear();

            drpImageField.Items.Add(new ListItem(GetString("general.selectnone"), "0"));

            if (!LoadActualValues)
            {
                array = allowedImage.Split(';');
                foreach (string item in array)
                {
                    if (!String.IsNullOrEmpty(item))
                    {
                        drpImageField.Items.Add(new ListItem(item));
                    }
                }
            }

            foreach (object[] item in attributes)
            {
                object[] obj = item;
                drpImageField.Items.Add(new ListItem(obj[0].ToString()));
            }
            // Preselect value
            if (!String.IsNullOrEmpty(ClassInfo.ClassSearchImageColumn))
            {
                drpImageField.SelectedValue = ClassInfo.ClassSearchImageColumn;
            }

            #endregion


            #region "Load drop-down list 'Date field'"

            drpDateField.Items.Clear();

            if (!LoadActualValues)
            {
                array = allowedDate.Split(';');
                foreach (string item in array)
                {
                    if (!String.IsNullOrEmpty(item))
                    {
                        drpDateField.Items.Add(new ListItem(item));
                    }
                }
            }
            else
            {
                drpDateField.Items.Add(new ListItem(GetString("general.selectnone"), "0"));
            }

            foreach (object[] item in attributes)
            {
                object[] obj = item;
                drpDateField.Items.Add(new ListItem(obj[0].ToString()));
            }

            // Preselect value
            if (!String.IsNullOrEmpty(ClassInfo.ClassSearchCreationDateColumn))
            {
                drpDateField.SelectedValue = ClassInfo.ClassSearchCreationDateColumn;
            }
            else
            {
                if (!LoadActualValues)
                {
                    drpDateField.SelectedValue = SearchHelper.DEFAULT_SEARCH_CREATION_DATE_COLUMN;
                }
            }

            #endregion
        }
    }


    /// <summary>
    /// Enables or disables search for current class.
    /// </summary>
    public bool SaveSearchAvailability()
    {
        bool changed = false;
        if (ItemID > 0)
        {
            DataClassInfo classInfo = DataClassInfoProvider.GetDataClassInfo(ItemID);
            if (classInfo != null)
            {
                if (classInfo.ClassSearchEnabled != chkSearchEnabled.Checked)
                {
                    changed = true;
                }
                classInfo.ClassSearchEnabled = chkSearchEnabled.Checked;
                DataClassInfoProvider.SetDataClassInfo(classInfo);

                if (!chkSearchEnabled.Checked)
                {
                    ShowConfirmation(GetString("search.searchwasdisabled"));
                }
            }
        }
        return changed;
    }


    /// <summary>
    /// Calls method from ClassFields control which stores data.
    /// </summary>
    public void SaveData()
    {
        ClassFields.SaveData();
    }

    #endregion


    #region "Events"

    public void btnOK_Click(object sender, EventArgs e)
    {
        SaveData();
    }


    /// <summary>
    /// CheckedChanged event handler.
    /// </summary>
    protected void chkSearchEnabled_CheckedChanged(object sender, EventArgs e)
    {
        pnlSearchFields.Visible = chkSearchEnabled.Checked;

        if (chkSearchEnabled.Checked)
        {
            ReloadData();
        }
    }


    /// <summary>
    /// OK button click handler.
    /// </summary>
    private void ClassFields_OnSaved(object sender, EventArgs e)
    {
        bool enabledChanged = false;
        if (ClassInfo != null)
        {
            enabledChanged = SaveSearchAvailability();

            if (AdvancedMode)
            {
                // Save advanced information only in advanced mode
                ClassInfo.ClassSearchTitleColumn = drpTitleField.SelectedValue;
                ClassInfo.ClassSearchContentColumn = drpContentField.SelectedValue;
                if (drpImageField.SelectedValue != "0")
                {
                    ClassInfo.ClassSearchImageColumn = drpImageField.SelectedValue;
                }
                else
                {
                    ClassInfo.ClassSearchImageColumn = DBNull.Value.ToString();
                }
                ClassInfo.ClassSearchCreationDateColumn = drpDateField.SelectedValue;
                DataClassInfoProvider.SetDataClassInfo(ClassInfo);
            }

            RaiseOnSaved();
        }

        // Display a message
        if (!String.IsNullOrEmpty(SaveResourceString))
        {
            string saveMessage = GetString(SaveResourceString);
            if (!String.IsNullOrEmpty(saveMessage))
            {
                ShowConfirmation(saveMessage);
            }
        }

        if ((ClassFields.Changed || enabledChanged) && (!String.IsNullOrEmpty(RebuildIndexResourceString)))
        {
            ShowInformation(GetString(RebuildIndexResourceString));
        }
    }

    #endregion
}