using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSModules_ContactManagement_Controls_UI_Ip_List : CMSAdminListControl
{
    #region "Variables"

    private CMSModules_ContactManagement_Controls_UI_IP_Filter filter;

    #endregion


    #region "Properties"

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
    /// True if site name column is visible.
    /// </summary>
    public bool ShowSiteNameColumn
    {
        get;
        set;
    }


    /// <summary>
    /// True if contact name is visible.
    /// </summary>
    public bool ShowContactNameColumn
    {
        get;
        set;
    }


    /// <summary>
    /// True if remove action button is visible.
    /// </summary>
    public bool ShowRemoveButton
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets contact ID.
    /// </summary>
    public int ContactID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets value that indicates whether contact is merged.
    /// </summary>
    public bool IsMergedContact
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets value that indicates whether contact is global contact.
    /// </summary>
    public bool IsGlobalContact
    {
        get;
        set;
    }


    /// <summary>
    /// Additional WHERE condition.
    /// </summary>
    public string WhereCondition
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        gridElem.OnFilterFieldCreated += new OnFilterFieldCreated(gridElem_OnFilterFieldCreated);
        gridElem.LoadGridDefinition();
        base.OnInit(e);
    }


    void gridElem_OnFilterFieldCreated(string columnName, UniGridFilterField filterDefinition)
    {
        filter = filterDefinition.ValueControl as CMSModules_ContactManagement_Controls_UI_IP_Filter;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (filter != null)
        {
            filter.IsLiveSite = IsLiveSite;
            filter.ShowSiteFilter = ShowSiteNameColumn;
            filter.ShowContactNameFilter = ShowContactNameColumn;
        }

        QueryDataParameters parameters = new QueryDataParameters();
        parameters.Add("@ContactID", ContactID);

        // Choose correct object type ("query") according to contact type
        if (IsGlobalContact)
        {
            gridElem.ObjectType = IPGlobalListInfo.OBJECT_TYPE;
        }
        else if (IsMergedContact)
        {
            gridElem.ObjectType = IPMergedListInfo.OBJECT_TYPE;
        }
        else
        {
            gridElem.ObjectType = IPListInfo.OBJECT_TYPE;
        }
        gridElem.QueryParameters = parameters;
        gridElem.WhereCondition = WhereCondition;
        gridElem.ZeroRowsText = GetString("om.ip.noips");
        gridElem.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridElem_OnExternalDataBound);
        gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        gridElem.NamedColumns["ContactSiteID"].Visible = ShowSiteNameColumn;

        // Display contact full name if some merged contacts point to this contact or if required
        bool showFullName = ShowContactNameColumn;
        if (!showFullName)
        {
            object dataSrc = gridElem.GridView.DataSource;
            if (!DataHelper.DataSourceIsEmpty(dataSrc))
            {
                DataRow[] dr = null;
                if (dataSrc is DataSet)
                {
                    DataSet ds = (DataSet)dataSrc;
                    dr = ds.Tables[0].Select("ContactMergedWithContactID > 0");
                }
                if (dataSrc is DataView)
                {
                    DataView dv = ((DataView)dataSrc);
                    dr = dv.Table.Select("ContactMergedWithContactID > 0");
                }
                showFullName = (dr != null) && (dr.Length > 0);
            }
        }
        gridElem.NamedColumns["ContactFullName"].Visible = showFullName;
    }


    /// <summary>
    /// UniGrid action handler.
    /// </summary>
    private void gridElem_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "delete")
        {
            ContactInfo contact = ContactInfoProvider.GetContactInfo(ContactID);

            // Check permission
            if ((contact != null) && ContactHelper.AuthorizedModifyContact(contact.ContactSiteID, true))
            {
                int ipId = ValidationHelper.GetInteger(actionArgument, 0);
                IPInfoProvider.DeleteIPInfo(ipId);
            }
        }
    }


    /// <summary>
    /// UniGrid databound handler.
    /// </summary>
    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "delete":
                if (ShowRemoveButton)
                {
                    ((CMSGridActionButton)sender).Enabled = true;
                }
                else
                {
                    ((CMSGridActionButton)sender).Enabled = false;
                }
                break;
        }
        return null;
    }

    #endregion
}