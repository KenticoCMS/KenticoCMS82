using System;
using System.Web.UI.WebControls;
using System.Data;

using CMS.Helpers;
using CMS.UIControls;
using CMS.OnlineMarketing;
using CMS.Helpers.Markup;
using CMS.DataEngine;
using CMS.PortalEngine;
using CMS.ExtendedControls;

public partial class CMSModules_ContactManagement_Controls_UI_Contact_Scoring : CMSAdminListControl
{
    #region "Properties"

    /// <summary>
    /// Sets or gets contact ID to filter unigrid.
    /// </summary>
    public int? ContactID
    {
        get;
        set;
    }


    /// <summary>
    /// Sets or gets site ID to filter unigrid.
    /// </summary>
    public int? SiteID
    {
        get;
        set;
    }


    /// <summary>
    /// Returns inner unigrid.
    /// </summary>
    public UniGrid UniGrid
    {
        get
        {
            return gridElem;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        var whereCondition = new WhereCondition(gridElem.WhereCondition);

        if (ContactID != null)
        {
            whereCondition.WhereEquals("ContactID", ContactID);
        }
        if (SiteID != null)
        {
            whereCondition.WhereEquals("ScoreSiteID", SiteID);
        }

        gridElem.WhereCondition = whereCondition.ToString(true);

        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
    }


    protected override void OnPreRender(EventArgs e)
    {
        RegisterScripts();
    }


    /// <summary>
    /// Puts JavaScript functions to page.
    /// </summary>
    private void RegisterScripts()
    {
        ScriptHelper.RegisterDialogScript(Page);

        string scriptBlock = string.Format(@"
            function ViewDetails(id) {{ modalDialog('{0}&objectid=' + id, 'ScoreDetails', '1050px', '700px'); }}
            function Refresh()
            {{
                __doPostBack('" + pnlUpdate.ClientID + @"', '');
            }}",
                                           UIContextHelper.GetElementDialogUrl("CMS.Scoring", "ScoringProperties"));

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "Actions", scriptBlock, true);
    }


    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "view":
                CMSGridActionButton viewBtn = (CMSGridActionButton)sender;
                viewBtn.OnClientClick = "ViewDetails(" + viewBtn.CommandArgument + "); return false;";
                break;

            case "scorestatus":
                DataRowView rowView = parameter as DataRowView;
                int ScoreID = ValidationHelper.GetInteger(rowView["ScoreID"], 0);
                ScoreInfo info = ScoreInfoProvider.GetScoreInfo(ScoreID);
                return GetFormattedStatus(info);
        }
        return null;
    }


    /// <summary>
    /// Gets formatted score status. Score can be disabled, it can be scheduled to rebuild in the future or its status is one of <see cref="ScoreStatusEnum"/>.
    /// </summary>
    private FormattedText GetFormattedStatus(ScoreInfo info)
    {
        var formatter = new ScoreStatusFormatter(info);
        return formatter.GetFormattedStatus();
    }

    #endregion
}