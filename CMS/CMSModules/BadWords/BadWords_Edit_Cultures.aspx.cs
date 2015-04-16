using System;
using System.Data;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.UIControls;
using CMS.Protection;

[EditedObject(BadWordInfo.OBJECT_TYPE, "badwordid")]
public partial class CMSModules_BadWords_BadWords_Edit_Cultures : GlobalAdminPage
{
    #region "Protected variables"

    private BadWordInfo mEditedBadWord;

    private BadWordInfo EditedBadWord
    {
        get
        {
            if (mEditedBadWord == null)
            {
                mEditedBadWord = EditedObject as BadWordInfo;
            }
            return mEditedBadWord;
        }
        set
        {
            mEditedBadWord = value;
            EditedObject = mEditedBadWord;
        }
    }
    

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (EditedBadWord != null)
        {
            radAll.CheckedChanged += isGlobal_CheckedChanged;
            radSelected.CheckedChanged += isGlobal_CheckedChanged;
            
            if (!RequestHelper.IsPostBack())
            {
                // Initialize radiobuttons
                radAll.Checked = EditedBadWord.WordIsGlobal;
                radSelected.Checked = !EditedBadWord.WordIsGlobal;
            }
            
            // Show / hide selector
            SetSelectorVisibility();

            // Get the word cultures
            if (!RequestHelper.IsPostBack() || (radSelected == ControlsHelper.GetPostBackControl(Page)))
            {
                usWordCultures.Value = GetCurrentValues();
            }
        }

        // Initialize selector properties
        usWordCultures.OnSelectionChanged += usWordCultures_OnSelectionChanged;
    }


    protected void isGlobal_CheckedChanged(object sender, EventArgs e)
    {
        if (EditedBadWord != null)
        {
            // Set whether the word is global
            EditedBadWord.WordIsGlobal = radAll.Checked;

            // Save badword
            BadWordInfoProvider.SetBadWordInfo(EditedBadWord);

            // Display message
            ShowChangesSaved();

            // Show / hide selector
            SetSelectorVisibility();
        }
    }

    #endregion


    #region "Control events"

    protected void usWordCultures_OnSelectionChanged(object sender, EventArgs e)
    {
        SaveCultures();
    }

    #endregion


    #region "Protected methods"

    protected string GetCurrentValues()
    {
        string currentValues = null;
        DataSet ds = BadWordCultureInfoProvider.GetBadWordCultures("WordID=" + EditedBadWord.WordID, null);
        
        // Initialize selector
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            currentValues = TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], "CultureID"));
        }
        return currentValues;
    }


    protected void SaveCultures()
    {
        if (EditedBadWord == null)
        {
            return;
        }

        // Remove old items
        string newValues = ValidationHelper.GetString(usWordCultures.Value, null);
        string currentValues = GetCurrentValues();
        string items = DataHelper.GetNewItemsInList(newValues, currentValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            // Add all new cultures to word
            foreach (string item in newItems)
            {
                int cultureId = ValidationHelper.GetInteger(item, 0);
                BadWordCultureInfoProvider.RemoveBadWordFromCulture(EditedBadWord.WordID, cultureId);
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(currentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            // Add all new cultures to word
            foreach (string item in newItems)
            {
                int cultureId = ValidationHelper.GetInteger(item, 0);
                BadWordCultureInfoProvider.AddBadWordToCulture(EditedBadWord.WordID, cultureId);
            }
        }

        ShowChangesSaved();
    }


    protected void SetSelectorVisibility()
    {
        usWordCultures.StopProcessing = !radSelected.Checked;
        usWordCultures.Visible = radSelected.Checked;
    }

    #endregion
}