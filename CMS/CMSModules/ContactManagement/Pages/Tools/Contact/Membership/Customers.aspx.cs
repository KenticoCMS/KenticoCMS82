using System;
using System.Data;

using CMS.Core;
using CMS.FormControls;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.DataEngine;
using CMS.Ecommerce;

// Edited object
[EditedObject(ContactInfo.OBJECT_TYPE, "contactid")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Membership_Customers : CMSContactManagementContactsPage
{
    #region "Variables"

    private int contactId;
    private bool globalContact;
    private bool mergedContact;
    private bool modifyAllowed;
    private ContactInfo ci;

    private FormEngineUserControl ucSelectCustomer;

    #endregion


    #region "Page events"
    
    protected void Page_Load(object sender, EventArgs e)
    {
        CheckUIElementAccessHierarchical(ModuleName.ONLINEMARKETING, "ContactMembership.Customers");
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

        // Choose correct object ("query") according to type of contact
        if (globalContact)
        {
            gridElem.ObjectType = ContactMembershipGlobalCustomerListInfo.OBJECT_TYPE;
        }
        else if (mergedContact)
        {
            gridElem.ObjectType = ContactMembershipMergedCustomerListInfo.OBJECT_TYPE;
        }
        else
        {
            gridElem.ObjectType = ContactMembershipCustomerListInfo.OBJECT_TYPE;
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

        // Setup customer selector
        try
        {
            ucSelectCustomer = (FormEngineUserControl)Page.LoadUserControl("~/CMSModules/Ecommerce/FormControls/CustomerSelector.ascx");
            ucSelectCustomer.SetValue("Mode", "OnlineMarketing");
            ucSelectCustomer.SetValue("SiteID", ci.ContactSiteID);
            ucSelectCustomer.Changed += ucSelectCustomer_Changed;
            ucSelectCustomer.IsLiveSite = false;
            pnlSelectCustomer.Controls.Clear();
            pnlSelectCustomer.Controls.Add(ucSelectCustomer);
        }
        catch
        {
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
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
                    dr = dv.Table.Select("ContactMergedWithContactID  = " + contactId);
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
                DisableDeleteButton(sender);
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


    private void ucSelectCustomer_Changed(object sender, EventArgs e)
    {
        // Check permissions
        if (ContactHelper.AuthorizedModifyContact(ci.ContactSiteID, true))
        {
            // Load value form dynamic control
            string values = null;
            if (ucSelectCustomer != null)
            {
                values = ValidationHelper.GetString(ucSelectCustomer.GetValue("OnlineMarketingValue"), null);
            }

            if (!String.IsNullOrEmpty(values))
            {
                // Store users one by one
                string[] customerIds = values.Split(';');
                int currentSiteID = SiteContext.CurrentSiteID;
                foreach (string customerId in customerIds)
                {
                    // Check if user ID is valid
                    int customerIdInt = ValidationHelper.GetInteger(customerId, 0);
                    if (customerIdInt <= 0)
                    {
                        continue;
                    }

                    var customer = CustomerInfoProvider.GetCustomerInfo(customerIdInt);
                    // Only allow adding customers on the same site as contact or registered customers
                    if ((customer == null) || ((customer.CustomerSiteID != currentSiteID) && (customer.CustomerSiteID != 0)))
                    {
                        continue;
                    }

                    // Add new relation
                    int parentId = (ci.ContactMergedWithContactID == 0)
                   ? ci.ContactID
                   : ci.ContactMergedWithContactID;

                    ContactMembershipInfoProvider.SetRelationship(customerIdInt, MemberTypeEnum.EcommerceCustomer, ci.ContactID, parentId, true);
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
    /// For global, merged contacts or users without permissions is delete button disabled.
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