using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_General_PageSize : CMSUserControl
{
    #region "Event & delegate"

    public delegate void OnSelectedItemChanged(int pageSize);

    public event OnSelectedItemChanged SelectedItemChanged;

    #endregion


    #region "Private variables"

    private string[] mPageSizeItems = null;

    #endregion


    #region "Public propeties"

    /// <summary>
    /// Gets or sets an array holding available size items ("10", "20", etc).
    /// </summary>
    public string[] Items
    {
        get
        {
            return mPageSizeItems;
        }
        set
        {
            mPageSizeItems = value;
        }
    }


    /// <summary>
    /// Gets or sets currently selected value.
    /// </summary>
    public string SelectedValue
    {
        get
        {
            return drpPageSize.SelectedValue;
        }
        set
        {
            try
            {
                drpPageSize.SelectedValue = value;
            }
            catch
            {
            }
        }
    }

    #endregion


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!StopProcessing)
        {
            SetupControl();
        }
        else
        {
            Visible = false;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
    }


    /// <summary>
    /// Initializes all the nested controls.
    /// </summary>
    private void SetupControl()
    {
        lblPageSize.Text = GetString("unigrid.itemsperpage");

        if ((drpPageSize.Items == null) || (drpPageSize.Items.Count == 0))
        {
            // Add custom items
            if ((Items != null) && (Items.Length > 0))
            {
                foreach (string itemName in Items)
                {
                    drpPageSize.Items.Add(new ListItem(itemName, itemName));
                }
            }

            // Add default item
            drpPageSize.Items.Insert(drpPageSize.Items.Count, new ListItem(GetString("general.selectall"), "-1"));

            drpPageSize.DataBind();
        }
    }


    protected void drpPageSize_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SelectedItemChanged != null)
        {
            // Raise event
            SelectedItemChanged(ValidationHelper.GetInteger(drpPageSize.SelectedValue, 0));
        }
    }
}