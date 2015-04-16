using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.FormControls;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_OnlineMarketing_FormControls_SelectVariation : FormEngineUserControl
{
    #region "Variables"

    private string mWhereCondition = String.Empty;
    private bool mShowAllVariations = true;
    private bool mPostbackOnChange = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Value representing the control.
    /// </summary>
    public override object Value
    {
        get
        {
            return ucUniSelector.Value;
        }
        set
        {
            ucUniSelector.Value = value;
        }
    }


    /// <summary>
    /// Uniselector control.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            return ucUniSelector;
        }
    }


    /// <summary>
    /// Where condition for selector.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return mWhereCondition;
        }
        set
        {
            mWhereCondition = value;
        }
    }


    /// <summary>
    /// If true (all) is added to conversion selector.
    /// </summary>
    public bool ShowAllVariations
    {
        get
        {
            return mShowAllVariations;
        }
        set
        {
            mShowAllVariations = value;
        }
    }


    /// <summary>
    /// If true, full postback is raised when item changed.
    /// </summary>
    public bool PostbackOnChange
    {
        get
        {
            return mPostbackOnChange;
        }
        set
        {
            mPostbackOnChange = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (ShowAllVariations)
        {
            ucUniSelector.AllRecordValue = String.Empty;
            ucUniSelector.AllowAll = true;
        }

        if (PostbackOnChange)
        {
            ucUniSelector.DropDownSingleSelect.AutoPostBack = true;
            ScriptManager scr = ScriptManager.GetCurrent(Page);
            scr.RegisterPostBackControl(ucUniSelector);
        }

        if (!URLHelper.IsPostback())
        {
            ReloadData(false);
        }
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    /// <param name="forceReload">If true data are always loaded</param>
    public void ReloadData(bool forceReload)
    {
        ucUniSelector.WhereCondition = WhereCondition;
        ucUniSelector.Reload(forceReload);
    }

    #endregion
}