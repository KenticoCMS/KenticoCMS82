using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

// Edited object
[EditedObject(ContactInfo.OBJECT_TYPE, "contactid")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Membership_Users : CMSContactManagementContactsPage
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
        CheckUIElementAccessHierarchical(ModuleName.ONLINEMARKETING, "ContactMembership.Users");
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
            where += " (ContactSiteID IS NULL OR ContactSiteID=" + SiteContext.CurrentSiteID + ")";
        }

        // Choose correct query according to type of contact
        if (globalContact)
        {
            gridElem.ObjectType = ContactMembershipGlobalUserListInfo.OBJECT_TYPE;
        }
        else if (mergedContact)
        {
            gridElem.ObjectType = ContactMembershipMergedUserListInfo.OBJECT_TYPE;
        }
        else
        {
            gridElem.ObjectType = ContactMembershipUserListInfo.OBJECT_TYPE;
        }

        // Query parameters
        QueryDataParameters parameters = new QueryDataParameters();
        parameters.Add("@ContactId", contactId);

        gridElem.WhereCondition = where;
        gridElem.QueryParameters = parameters;
        gridElem.OnAction += gridElem_OnAction;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;

        // Setup user selector
        selectUser.UniSelector.SelectionMode = SelectionModeEnum.MultipleButton;
        selectUser.UniSelector.OnItemsSelected += UniSelector_OnItemsSelected;
        selectUser.UniSelector.ReturnColumnName = "UserID";
        selectUser.ShowSiteFilter = false;
        selectUser.ResourcePrefix = "addusers";
        selectUser.IsLiveSite = false;
        selectUser.SiteID = ci.ContactSiteID;

        // Hide header actions for global contact or merged contact.
        CurrentMaster.HeaderActionsPlaceHolder.Visible = modifyAllowed && !globalContact && !mergedContact;
    }


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "delete":
                DisableDeleteButton(sender);
                break;
        }

        return parameter;
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
            string values = ValidationHelper.GetString(selectUser.UniSelector.Value, null);
            if (!String.IsNullOrEmpty(values))
            {
                // Store users one by one
                string[] userIds = values.Split(';');
                foreach (string userId in userIds)
                {
                    // Check if user ID is valid
                    int userIdInt = ValidationHelper.GetInteger(userId, 0);
                    if (userIdInt <= 0)
                    {
                        continue;
                    }
                    // Add new relation
                    int parentId = (ci.ContactMergedWithContactID == 0)
                        ? ci.ContactID
                        : ci.ContactMergedWithContactID;
                    ContactMembershipInfoProvider.SetRelationship(userIdInt, MemberTypeEnum.CmsUser, ci.ContactID, parentId, true);

                    // When contact was merged update contact info
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

    #endregion


    #region "Private methods"

    /// <summary>
    /// Disables delete button for merged, global contacts or when user has no modify rights.
    /// </summary>
    private void DisableDeleteButton(object sender)
    {
        if (globalContact || mergedContact || !modifyAllowed)
        {
            var button = ((CMSGridActionButton)sender);
            button.Enabled = false;
        }
    }

    #endregion
}