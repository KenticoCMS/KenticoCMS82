using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.UIControls;

public partial class CMSModules_ContactManagement_FormControls_ContactGroupDialog : CMSModalPage
{
    #region "Variables"

    private int siteId = -1;
    protected Hashtable mParameters;
    protected bool allowGlobalGroups = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Stop processing flag.
    /// </summary>
    public bool StopProcessing
    {
        get
        {
            return gridElem.StopProcessing;
        }
        set
        {
            gridElem.StopProcessing = value;
        }
    }


    /// <summary>
    /// Hashtable containing dialog parameters.
    /// </summary>
    private Hashtable Parameters
    {
        get
        {
            if (mParameters == null)
            {
                string identifier = QueryHelper.GetString("params", null);
                mParameters = (Hashtable)WindowHelper.GetItem(identifier);
            }
            return mParameters;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.PanelContent.RemoveCssClass("dialog-content");
        PageTitle.TitleText = GetString("om.contactgroup.selecttitle");
        Page.Title = PageTitle.TitleText;

        if (!QueryHelper.ValidateHash("hash") || Parameters == null)
        {
            StopProcessing = true;
            return;
        }

        siteId = ValidationHelper.GetInteger(Parameters["siteid"], 0);

        // Check permission
        if (ContactGroupHelper.AuthorizedReadContactGroup(siteId, true))
        {
            if (siteId > 0)
            {
                gridElem.WhereCondition = "ContactGroupSiteID = " + siteId;
            }
            else
            {
                gridElem.WhereCondition = "ContactGroupSiteID IS NULL";
            }

            gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
            gridElem.OnBeforeDataReload += gridElem_OnBeforeDataReload;
        }
    }


    protected void gridElem_OnBeforeDataReload()
    {
        if (!gridElem.StopProcessing)
        {
            // Hide the last column if it is not necessary
            gridElem.NamedColumns["isglobal"].Visible = allowGlobalGroups;
        }
    }


    /// <summary>
    /// Unigrid external databound handler.
    /// </summary>
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "contactgroupdisplayname":
                LinkButton btn = new LinkButton();
                DataRowView drv = parameter as DataRowView;
                btn.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ValidationHelper.GetString(drv["ContactGroupDisplayName"], null)));
                btn.Click += new EventHandler(btn_Click);
                btn.CommandArgument = ValidationHelper.GetString(drv["ContactGroupID"], null);
                btn.ToolTip = HTMLHelper.HTMLEncode(ValidationHelper.GetString(drv.Row["ContactGroupDescription"], null));
                return btn;

            case "isglobal":
                return (parameter is DBNull) ? UniGridFunctions.ColorLessSpanYesNo(true) : string.Empty;
        }
        return parameter;
    }


    /// <summary>
    /// Contact group selected event handler.
    /// </summary>
    protected void btn_Click(object sender, EventArgs e)
    {
        int groupId = ValidationHelper.GetInteger(((LinkButton)sender).CommandArgument, 0);
        string script = ScriptHelper.GetScript(@"
wopener.SelectValue_" + ValidationHelper.GetString(Parameters["clientid"], string.Empty) + @"(" + groupId + @");
CloseDialog();
");

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "CloseWindow", script);
    }

    #endregion
}