using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Data;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Search;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSModules_SmartSearch_Controls_Edit_ClassFields : CMSAdminEditControl
{
    #region "Private variables"

    private SearchSettings mFields;
    private DataClassInfo mDci;
    private bool mDisplayIField = true;
    private DataSet mInfos;
    private List<ColumnDefinition> mAttributes = new List<ColumnDefinition>();
    private bool mLoaded;
    private bool mDisplaySaved = true;
    private SearchSettings mSearchSettings;
    private bool mDisplaySetAutomatically = true;

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
    /// Indicates if iField column should be displayed. Default true.
    /// </summary>
    public bool DisplayIField
    {
        get
        {
            return mDisplayIField;
        }
        set
        {
            mDisplayIField = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which indicates whether "Set automatically" button should be visible.
    /// </summary>
    public bool DisplaySetAutomatically
    {
        get
        {
            return mDisplaySetAutomatically;
        }
        set
        {
            mDisplaySetAutomatically = value;
        }
    }


    /// <summary>
    /// Indicates if "Changes were saved" info label should be displayed.
    /// </summary>
    public bool DisplaySaved
    {
        get
        {
            return mDisplaySaved;
        }
        set
        {
            mDisplaySaved = value;
        }
    }


    /// <summary>
    /// Use after item saved, if true, relevant data for index rebuilt was changed.
    /// </summary>
    public bool Changed
    {
        get;
        private set;
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!DisplaySetAutomatically)
        {
            pnlButton.Visible = false;
        }

        ReloadData(false, false);
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    /// <param name="setAutomatically">Indicates if table should be pre-set according to field type</param>
    /// <param name="forceReload">Forces data to reload even if they are already loaded.</param>
    public void ReloadData(bool setAutomatically, bool forceReload)
    {
        if (!mLoaded || forceReload)
        {
            mLoaded = true;
            pnlContent.Controls.Clear();
            
            mAttributes = SearchHelper.GetClassSearchFields(ItemID);

            mDci = DataClassInfoProvider.GetDataClassInfo(ItemID);

            if (mDci != null)
            {
                mSearchSettings = new SearchSettings();
                mSearchSettings.LoadData(mDci.ClassSearchSettings);
            }

            btnAutomatically.Click += btnAutomatically_Click;

            // Display checkbox matrix only if field names array is not empty
            if (mAttributes.Count > 0)
            {
                // Setup controls
                btnAutomatically.Visible = true;
                if (mFields == null)
                {
                    mFields = new SearchSettings();
                }

                if (mDci != null)
                {
                    mFields.LoadData(mDci.ClassSearchSettings);
                }
                mInfos = mFields.GetAllSettingsInfos();

                CreateTable(setAutomatically);

                Literal ltl = new Literal();
                ltl.Text = "<br />";
                pnlContent.Controls.Add(ltl);
            }
        }
    }


    /// <summary>
    /// Creates table.
    /// </summary>
    private void CreateTable(bool setAutomatically)
    {
        Table table = new Table();
        table.CssClass = "table table-hover";
        table.CellPadding = -1;
        table.CellSpacing = -1;
        
        // Create table header
        TableHeaderRow header = new TableHeaderRow();
        header.TableSection = TableRowSection.TableHeader;
        TableHeaderCell thc = new TableHeaderCell();
        thc.Text = GetString("srch.settings.fieldname");
        thc.Scope = TableHeaderScope.Column;
        header.Cells.Add(thc);
        thc = new TableHeaderCell();
        thc.Text = GetString("development.content");
        thc.Scope = TableHeaderScope.Column;
        header.Cells.Add(thc);
        thc = new TableHeaderCell();
        thc.Text = GetString("srch.settings.searchable");
        thc.Scope = TableHeaderScope.Column;
        header.Cells.Add(thc);
        thc = new TableHeaderCell();
        thc.Text = GetString("srch.settings.tokenized");
        thc.Scope = TableHeaderScope.Column;
        header.Cells.Add(thc);

        if (DisplayIField)
        {
            thc = new TableHeaderCell();
            thc.Text = GetString("srch.settings.ifield");
            header.Cells.Add(thc);
        }

        thc = new TableHeaderCell();
        thc.CssClass = "main-column-100";
        header.Cells.Add(thc);

        table.Rows.Add(header);
        pnlContent.Controls.Add(table);

        // Create table content
        if ((mAttributes != null) && (mAttributes.Count > 0))
        {
            // Create row for each field
            foreach (ColumnDefinition column in mAttributes)
            {
                SearchSettingsInfo ssi = null;
                TableRow tr = new TableRow();
                if (!DataHelper.DataSourceIsEmpty(mInfos))
                {
                    DataRow[] dr = mInfos.Tables[0].Select("name = '" + column.ColumnName + "'");
                    if ((dr.Length > 0) && (mSearchSettings != null))
                    {
                        ssi = mSearchSettings.GetSettingsInfo((string)dr[0]["id"]);
                    }
                }

                // Add cell with field name
                TableCell tc = new TableCell();
                Label lbl = new Label();
                lbl.Text = column.ColumnName;
                tc.Controls.Add(lbl);
                tr.Cells.Add(tc);

                // Add cell with 'Content' value
                tc = new TableCell();
                CMSCheckBox chk = new CMSCheckBox();
                chk.ID = column.ColumnName + SearchSettings.CONTENT;

                if (setAutomatically)
                {
                    chk.Checked = SearchHelper.GetSearchFieldDefaultValue(SearchSettings.CONTENT, column.ColumnType);
                }
                else if (ssi != null)
                {
                    chk.Checked = ssi.Content;
                }

                tc.Controls.Add(chk);
                tr.Cells.Add(tc);

                // Add cell with 'Searchable' value
                tc = new TableCell();
                chk = new CMSCheckBox();
                chk.ID = column.ColumnName + SearchSettings.SEARCHABLE;

                if (setAutomatically)
                {
                    chk.Checked = SearchHelper.GetSearchFieldDefaultValue(SearchSettings.SEARCHABLE, column.ColumnType);
                }
                else if (ssi != null)
                {
                    chk.Checked = ssi.Searchable;
                }

                tc.Controls.Add(chk);
                tr.Cells.Add(tc);

                // Add cell with 'Tokenized' value
                tc = new TableCell();
                chk = new CMSCheckBox();
                chk.ID = column.ColumnName + SearchSettings.TOKENIZED;

                if (setAutomatically)
                {
                    chk.Checked = SearchHelper.GetSearchFieldDefaultValue(SearchSettings.TOKENIZED, column.ColumnType);
                }
                else if (ssi != null)
                {
                    chk.Checked = ssi.Tokenized;
                }

                tc.Controls.Add(chk);
                tr.Cells.Add(tc);

                // Add cell with 'iFieldname' value
                if (DisplayIField)
                {
                    tc = new TableCell();
                    CMSTextBox txt = new CMSTextBox();
                    txt.ID = column.ColumnName + SearchSettings.IFIELDNAME;
                    txt.CssClass += " form-control";
                    txt.MaxLength = 200;
                    if (ssi != null)
                    {
                        txt.Text = ssi.FieldName;
                    }
                    tc.Controls.Add(txt);
                    tr.Cells.Add(tc);
                }
                tc = new TableCell();
                tr.Cells.Add(tc);
                table.Rows.Add(tr);
            }
        }
    }


    /// <summary>
    /// Stores data and raises OnSaved event.
    /// </summary>
    public void SaveData()
    {
        // Clear old values
        mFields = new SearchSettings();
        Changed = false;

        // Create new SearchSettingInfos
        foreach (ColumnDefinition column in mAttributes)
        {
            SearchSettingsInfo ssiOld = null;

            // Return old data to compare changes
            if (mInfos != null)
            {
                DataRow[] dr = mInfos.Tables[0].Select("name = '" + column.ColumnName + "'");
                if ((dr.Length > 0) && (mSearchSettings != null))
                {
                    ssiOld = mSearchSettings.GetSettingsInfo((string)dr[0]["id"]);
                }
            }

            var name = column.ColumnName;
            var content = false;
            var searchable = false;
            var tokenized = false;
            var fieldname = string.Empty;

            CMSCheckBox chk = (CMSCheckBox)pnlContent.FindControl(name + SearchSettings.CONTENT);
            if (chk != null)
            {
                content = chk.Checked;
            }
            chk = (CMSCheckBox)pnlContent.FindControl(name + SearchSettings.SEARCHABLE);
            if (chk != null)
            {
                searchable = chk.Checked;
            }
            chk = (CMSCheckBox)pnlContent.FindControl(name + SearchSettings.TOKENIZED);
            if (chk != null)
            {
                tokenized = chk.Checked;
            }
            TextBox txt = (TextBox)pnlContent.FindControl(name + SearchSettings.IFIELDNAME);
            if (txt != null)
            {
                fieldname = txt.Text;
            }

            bool fieldChanged;
            var ssi = SearchHelper.CreateSearchSettings(name, content, searchable, tokenized, fieldname, ssiOld, out fieldChanged);

            if (fieldChanged)
            {
                Changed = true;
            }

            mFields.SetSettingsInfo(ssi);
        }

        // Store values to DB
        if (mDci != null)
        {
            mDci.ClassSearchSettings = mFields.GetData();
            DataClassInfoProvider.SetDataClassInfo(mDci);
        }

        if (DisplaySaved)
        {
            ShowChangesSaved();
        }
        RaiseOnSaved();
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Sets automatically button - click event handler.
    /// </summary>
    private void btnAutomatically_Click(object sender, EventArgs e)
    {
        ReloadData(true, true);
    }

    #endregion
}