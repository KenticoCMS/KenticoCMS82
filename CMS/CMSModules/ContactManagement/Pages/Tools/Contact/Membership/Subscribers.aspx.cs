using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

// Edited object
[EditedObject(ContactInfo.OBJECT_TYPE, "contactid")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Membership_Subscribers : CMSContactManagementContactsPage
{
    #region "Variables"

    private int contactId;
    private bool globalContact;
    private bool mergedContact;
    private bool modifyAllowed;
    private ContactInfo ci;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        CheckUIElementAccessHierarchical(ModuleName.ONLINEMARKETING, "ContactMembership.Subscribers");
        ci = (ContactInfo)EditedObject;
        if (ci == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        CheckReadPermission(ci.ContactSiteID);

        globalContact = (ci.ContactSiteID <= 0);
        mergedContact = (ci.ContactMergedWithContactID > 0);
        modifyAllowed = ContactHelper.AuthorizedModifyContact(ci.ContactSiteID, false);

        contactId = QueryHelper.GetInteger("contactid", 0);

        string where = null;
        // Filter only site members in CMSDesk (for global contacts)
        if (!IsSiteManager && globalContact && AuthorizedForSiteContacts)
        {
            where = " (ContactSiteID IS NULL OR ContactSiteID=" + SiteContext.CurrentSiteID + ")";
        }

        // Choose correct object ("query") according to type of contact
        if (globalContact)
        {
            gridElem.ObjectType = ContactMembershipGlobalSubscriberListInfo.OBJECT_TYPE;
        }
        else if (mergedContact)
        {
            gridElem.ObjectType = ContactMembershipMergedSubscriberListInfo.OBJECT_TYPE;
        }
        else
        {
            gridElem.ObjectType = ContactMembershipSubscriberListInfo.OBJECT_TYPE;
        }

        // Query parameters
        QueryDataParameters parameters = new QueryDataParameters();
        parameters.Add("@ContactId", contactId);

        gridElem.WhereCondition = where;
        gridElem.QueryParameters = parameters;
        gridElem.OnAction += gridElem_OnAction;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;

        // Hide header actions for global contact or merged contact.
        CurrentMaster.HeaderActionsPlaceHolder.Visible = modifyAllowed && !globalContact && !mergedContact;

        // Setup subscriber selector
        selectSubscriber.UniSelector.SelectionMode = SelectionModeEnum.MultipleButton;
        selectSubscriber.UniSelector.OnItemsSelected += UniSelector_OnItemsSelected;
        selectSubscriber.UniSelector.ReturnColumnName = "SubscriberID";
        selectSubscriber.UniSelector.DisplayNameFormat = "{%SubscriberFullName%} ({%SubscriberEmail%})";
        selectSubscriber.ShowSiteFilter = false;
        selectSubscriber.IsLiveSite = false;
        selectSubscriber.SiteID = ci.ContactSiteID;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Hide unwanted columns
        gridElem.NamedColumns["sitename"].Visible = globalContact;

        // Display contact full name if some merged contacts point to this contact or if required
        bool showFullName = globalContact;
        if (!showFullName)
        {
            object dataSrc = gridElem.GridView.DataSource;
            if (!DataHelper.DataSourceIsEmpty(dataSrc))
            {
                DataRow[] dr = null;
                if (dataSrc is DataSet)
                {
                    DataSet ds = (DataSet)dataSrc;
                    dr = ds.Tables[0].Select("ContactMergedWithContactID = " + contactId);
                }
                if (dataSrc is DataView)
                {
                    DataView dv = ((DataView)dataSrc);
                    dr = dv.Table.Select("ContactMergedWithContactID = " + contactId);
                }
                showFullName = (dr != null) && (dr.Length > 0);
            }
        }
        gridElem.NamedColumns["contactname"].Visible = showFullName;
    }


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "delete":
                DisableButton(sender);
                break;
        }

        return parameter;
    }


    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "delete":
                int membershipId = ValidationHelper.GetInteger(actionArgument, 0);
                if (membershipId > 0)
                {
                    // Check permissions
                    if (ContactHelper.AuthorizedModifyContact(ci.ContactSiteID, true))
                    {
                        ContactMembershipInfoProvider.DeleteRelationship(membershipId);
                    }
                }
                break;
        }
    }


    private void UniSelector_OnItemsSelected(object sender, EventArgs e)
    {
        // Check permissions
        if (ContactHelper.AuthorizedModifyContact(ci.ContactSiteID, true))
        {
            string values = ValidationHelper.GetString(selectSubscriber.UniSelector.Value, null);
            if (!String.IsNullOrEmpty(values))
            {
                // Store subscribers one by one
                string[] subscriberIds = values.Split(';');
                foreach (string subscriberId in subscriberIds)
                {
                    // Check if user ID is valid
                    int subscriberIdInt = ValidationHelper.GetInteger(subscriberId, 0);
                    if (subscriberIdInt <= 0)
                    {
                        continue;
                    }
                    // Add new relation
                    int parentId = (ci.ContactMergedWithContactID == 0) ? ci.ContactID : ci.ContactMergedWithContactID;
                    ContactMembershipInfoProvider.SetRelationship(subscriberIdInt, MemberTypeEnum.NewsletterSubscriber, ci.ContactID, parentId, true);
                    ci = ContactInfoProvider.GetContactInfo(contactId);
                }

                // When contact was merged then refresh complete page
                if ((ci != null) && (ci.ContactMergedWithContactID > 0))
                {
                    Page.Response.Redirect(RequestContext.URL.ToString(), true);
                }
                else
                {
                    gridElem.ReloadData();
                }
            }
        }
    }


    /// <summary>
    /// Disables button for certain conditions.
    /// </summary>
    private void DisableButton(object sender)
    {
        if (globalContact || mergedContact || !modifyAllowed)
        {
            CMSGridActionButton button = ((CMSGridActionButton)sender);
            button.Enabled = false;
        }
    }

    #endregion
}