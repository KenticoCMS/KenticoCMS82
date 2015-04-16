using System;

using CMS.FormControls;
using CMS.UIControls;
using CMS.FormEngine;

public partial class CMSModules_MessageBoards_Controls_Messages_BoardSelector : FormEngineUserControl
{
    #region "Variables"

    private bool mAddAllItemsRecord = false;
    private int mGroupId = 0;
    private int mSiteId = 0;

    #endregion


    #region "Public properties"

    /// <summary>
    /// ID of the current group.
    /// </summary>
    public int GroupID
    {
        get
        {
            return mGroupId;
        }
        set
        {
            mGroupId = value;
        }
    }


    /// <summary>
    /// ID of the current site.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            if (uniSelector != null)
            {
                uniSelector.Enabled = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return uniSelector.Value;
        }
        set
        {
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }

            uniSelector.Value = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether to add none item record to the dropdownlist.
    /// </summary>
    public bool AddAllItemsRecord
    {
        get
        {
            return mAddAllItemsRecord;
        }
        set
        {
            mAddAllItemsRecord = value;
        }
    }


    /// <summary>
    /// Gets the inner UniSelector control.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            return uniSelector;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            uniSelector.StopProcessing = true;
        }
        else
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        uniSelector.WhereCondition = "BoardSiteID = " + SiteID + " AND " + (GroupID > 0 ? "BoardGroupID = " + GroupID : "((BoardGroupID = 0) OR (BoardGroupID IS NULL))");
        uniSelector.IsLiveSite = IsLiveSite;
        uniSelector.SelectionMode = SelectionModeEnum.SingleDropDownList;
        uniSelector.AllowAll = false;
        uniSelector.AllowEmpty = false;
        if (AddAllItemsRecord)
        {
            uniSelector.SpecialFields.Add(new SpecialField() { Text = GetString("general.selectall"), Value = "ALL" }); 
        }
    }
}