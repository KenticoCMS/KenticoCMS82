using System;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.DataEngine;
using CMS.ExtendedControls;

public partial class CMSModules_ContactManagement_Controls_UI_ContactGroup_List : CMSAdminListControl
{
    #region "Variables"

    private int mSiteId = -1;
    private string mWhereCondition = null;
    private bool? mModifyGroupPermission;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets permission for modifying a group.
    /// </summary>
    protected bool ModifyGroupPermission
    {
        get
        {
            return (bool)(mModifyGroupPermission ??
                          (mModifyGroupPermission = ContactGroupHelper.AuthorizedModifyContactGroup(SiteID, false)));
        }
    }


    /// <summary>
    /// Inner grid.
    /// </summary>
    public UniGrid Grid
    {
        get
        {
            return gridElem;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            gridElem.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            gridElem.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets the site id.
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
    /// Additional WHERE condition to filter data.
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

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        var editUrl = UIContextHelper.GetElementUrl("CMS.OnlineMarketing", "EditContactGroup");
        editUrl = URLHelper.AddParameterToUrl(editUrl, "displayTitle", "false");
        editUrl = URLHelper.AddParameterToUrl(editUrl, "objectId", "{0}");
        editUrl = URLHelper.AddParameterToUrl(editUrl, "siteId", SiteID.ToString());

        // Setup unigrid
        gridElem.OnAction += gridElem_OnAction;
        gridElem.WhereCondition = GetWhereCondition();
        gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, WhereCondition);
        gridElem.EditActionUrl = editUrl;
        gridElem.ZeroRowsText = GetString("om.contactgroup.notfound");
        gridElem.OnBeforeDataReload += gridElem_OnBeforeDataReload;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;

        if (ContactHelper.IsSiteManager)
        {
            gridElem.EditActionUrl = URLHelper.AddParameterToUrl(gridElem.EditActionUrl, "issitemanager", "1");
        }
    }


    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        CMSGridActionButton btn = null;

        switch (sourceName.ToLowerCSafe())
        {
            case "delete":
                if (!ModifyGroupPermission)
                {
                    btn = (CMSGridActionButton)sender;
                    btn.Enabled = false;
                }
                break;
        }

        return null;
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Unigrid OnBeforeDataReload event handler.
    /// </summary>
    private void gridElem_OnBeforeDataReload()
    {
        gridElem.NamedColumns["sitename"].Visible = (SiteID == UniSelector.US_GLOBAL_AND_SITE_RECORD) || (SiteID == UniSelector.US_ALL_RECORDS);
    }


    /// <summary>
    /// Unigrid button clicked.
    /// </summary>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "delete")
        {
            int groupId = ValidationHelper.GetInteger(actionArgument, 0);
            ContactGroupInfo cgi = ContactGroupInfoProvider.GetContactGroupInfo(groupId);

            // Check permission
            if ((cgi != null) && ContactGroupHelper.AuthorizedModifyContactGroup(cgi.ContactGroupSiteID, true))
            {
                // Delete contact group
                ContactGroupInfoProvider.DeleteContactGroupInfo(groupId);
            }
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns where condition for unigrid.
    /// </summary>
    protected string GetWhereCondition()
    {
        string result = null;

        // Filter site objects
        if (SiteID > 0)
        {
            result = "(ContactGroupSiteID = " + SiteID + ")";
        }
        // Filter only global objects
        else if (SiteID == UniSelector.US_GLOBAL_RECORD)
        {
            result = "(ContactGroupSiteID IS NULL)";
        }
        else if (SiteID == UniSelector.US_GLOBAL_AND_SITE_RECORD)
        {
            result = "(ContactGroupSiteID IS NULL) OR (ContactGroupSiteID = " + SiteContext.CurrentSiteID + ")";
        }
        return result;
    }

    #endregion
}