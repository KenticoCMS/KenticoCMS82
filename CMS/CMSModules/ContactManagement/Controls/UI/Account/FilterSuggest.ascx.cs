using System;
using System.Linq;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_ContactManagement_Controls_UI_Account_FilterSuggest : CMSUserControl
{
    #region "Variables"

    private AccountInfo ai;
    private int mSelectedSiteID;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets current account.
    /// </summary>
    private AccountInfo CurrentAccount
    {
        get
        {
            if (ai == null)
            {
                ai = (AccountInfo)UIContext.EditedObject;
            }
            return ai;
        }
    }


    /// <summary>
    /// Gets value indicating if contacts checkbox is selected.
    /// </summary>
    public bool ContactsChecked
    {
        get
        {
            return chkContacts.Checked;
        }
    }


    /// <summary>
    /// Gets value indicating if post address checkbox is selected.
    /// </summary>
    public bool AddressChecked
    {
        get
        {
            return chkAddress.Checked;
        }
    }


    /// <summary>
    /// Gets value indicating if email is selected.
    /// </summary>
    public bool EmailChecked
    {
        get
        {
            return chkEmail.Checked;
        }
    }


    /// <summary>
    /// Gets value indicating if URL checkbox is selected.
    /// </summary>
    public bool URLChecked
    {
        get
        {
            return chkURL.Checked;
        }
    }


    /// <summary>
    /// Gets value indicating if phone & fax checkbox is selected.
    /// </summary>
    public bool PhonesChecked
    {
        get
        {
            return chkPhone.Checked;
        }
    }

    /// <summary>
    /// Gets selected site ID.
    /// </summary>
    public int SelectedSiteID
    {
        get
        {
            return mSelectedSiteID;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        chkContacts.Enabled = (CurrentAccount.AccountPrimaryContactID != 0) || (CurrentAccount.AccountSecondaryContactID != 0);
        chkAddress.Enabled = !String.IsNullOrEmpty(CurrentAccount.AccountAddress1) || !String.IsNullOrEmpty(CurrentAccount.AccountAddress2) || !String.IsNullOrEmpty(CurrentAccount.AccountCity) || !String.IsNullOrEmpty(CurrentAccount.AccountZIP);
        chkEmail.Enabled = !String.IsNullOrEmpty(CurrentAccount.AccountEmail);
        chkURL.Enabled = !String.IsNullOrEmpty(CurrentAccount.AccountWebSite);
        chkPhone.Enabled = !String.IsNullOrEmpty(CurrentAccount.AccountPhone) || !String.IsNullOrEmpty(CurrentAccount.AccountFax);

        // Current account is global object
        if (ai.AccountSiteID == 0)
        {
            plcSite.Visible = true;
            plcContact.Visible = false;
            // Display site selector in site manager
            if (ContactHelper.IsSiteManager)
            {
                siteOrGlobalSelector.Visible = false;
            }
            // Display 'site or global' selector in CMS desk for global objects
            else if (AccountHelper.AuthorizedReadAccount(SiteContext.CurrentSiteID, false) && AccountHelper.AuthorizedModifyAccount(SiteContext.CurrentSiteID, false))
            {
                siteSelector.Visible = false;
            }
            else
            {
                plcSite.Visible = false;
            }
        }
    }


    /// <summary>
    /// Returns SQL WHERE condition depending on selected checkboxes.
    /// </summary>
    /// <returns>Returns SQL WHERE condition</returns>
    public string GetWhereCondition()
    {
        var where = new WhereCondition();

        // Contacts checked
        if (chkContacts.Checked)
        {
            where.Where(GetContactWhereCondition());
        }

        // Address checked
        if (chkAddress.Checked)
        {
            where.Where(GetAddressWhereCondition());
        }

        // Email address checked
        if (chkEmail.Checked)
        {
            string domain = ContactHelper.GetEmailDomain(CurrentAccount.AccountEmail);
            if (!String.IsNullOrEmpty(domain))
            {
                var emailWhere = new WhereCondition().WhereEndsWith("AccountEmail", "@" + domain);
                where.Where(emailWhere);
            }
        }

        // URL checked
        if (chkURL.Checked && !String.IsNullOrEmpty(CurrentAccount.AccountWebSite))
        {
            var urlWhere = new WhereCondition().WhereContains("AccountWebSite", URLHelper.CorrectDomainName(CurrentAccount.AccountWebSite));
            where.Where(urlWhere);
        }

        // Phone & fax checked
        if (chkPhone.Checked && (!String.IsNullOrEmpty(CurrentAccount.AccountPhone) || !String.IsNullOrEmpty(CurrentAccount.AccountFax)))
        {
            where.Where(GetPhoneWhereCondition());
        }

        if ((!chkContacts.Checked && !chkAddress.Checked && !chkEmail.Checked && !chkURL.Checked && !chkPhone.Checked) || (String.IsNullOrEmpty(where.WhereCondition)))
        {
            return "(1 = 0)";
        }

        // Filter out current account
        where.WhereNotEquals("AccountID", CurrentAccount.AccountID);

        // Filter out merged records
        where.Where(w => w.Where(x => x.WhereNull("AccountMergedWithAccountID")
                                       .WhereNull("AccountGlobalAccountID")
                                       .WhereGreaterThan("AccountSiteID", 0))
                          .Or(y => y.WhereNull("AccountGlobalAccountID")
                                    .WhereNull("AccountSiteID")));

        // For global object use siteselector's value
        if (plcSite.Visible)
        {
            mSelectedSiteID = UniSelector.US_ALL_RECORDS;
            if (siteSelector.Visible)
            {
                mSelectedSiteID = siteSelector.SiteID;
            }
            else if (siteOrGlobalSelector.Visible)
            {
                mSelectedSiteID = siteOrGlobalSelector.SiteID;
            }

            // Only global objects
            if (mSelectedSiteID == UniSelector.US_GLOBAL_RECORD)
            {
                where.WhereNull("AccountSiteID");
            }
            // Global and site objects
            else if (mSelectedSiteID == UniSelector.US_GLOBAL_AND_SITE_RECORD)
            {
                where.Where(w => w.WhereNull("AccountSiteID").Or().WhereEquals("AccountSiteID", SiteContext.CurrentSiteID));
            }
            // Site objects
            else if (mSelectedSiteID != UniSelector.US_ALL_RECORDS)
            {
                where.WhereEquals("AccountSiteID", mSelectedSiteID);
            }
        }
        // Filter out accounts from different sites
        else
        {
            // Site accounts only
            if (CurrentAccount.AccountSiteID > 0)
            {
                where.WhereEquals("AccountSiteID", CurrentAccount.AccountSiteID);
            }
            // Global accounts only
            else
            {
                where.WhereNull("AccountSiteID");
            }
        }

        return where.ToString(expand: true);
    }


    private WhereCondition GetPhoneWhereCondition()
    {
        var phoneWhere = new WhereCondition();
        if (!String.IsNullOrEmpty(CurrentAccount.AccountPhone))
        {
            phoneWhere.WhereContains("AccountPhone", CurrentAccount.AccountPhone);
        }
        if (!String.IsNullOrEmpty(CurrentAccount.AccountFax))
        {
            phoneWhere.Or().WhereContains("AccountFax", CurrentAccount.AccountFax);
        }
        return phoneWhere;
    }


    private WhereCondition GetAddressWhereCondition()
    {
        var addressWhere = new WhereCondition();
        if (!String.IsNullOrEmpty(CurrentAccount.AccountAddress1))
        {
            addressWhere.Or().WhereContains("AccountAddress1", CurrentAccount.AccountAddress1);
        }
        if (!String.IsNullOrEmpty(CurrentAccount.AccountAddress2))
        {
            addressWhere.Or().WhereContains("AccountAddress2", CurrentAccount.AccountAddress2);
        }
        if (!String.IsNullOrEmpty(CurrentAccount.AccountCity))
        {
            addressWhere.Or().WhereContains("AccountCity", CurrentAccount.AccountCity);
        }
        if (!String.IsNullOrEmpty(CurrentAccount.AccountZIP))
        {
            addressWhere.Or().WhereContains("AccountZIP", CurrentAccount.AccountZIP);
        }
        return addressWhere;
    }


    private WhereCondition GetContactWhereCondition()
    {
        var contactWhere = new WhereCondition();
        ContactInfo contact;

        // Get primary contact WHERE condition
        if (CurrentAccount.AccountPrimaryContactID != 0)
        {
            contact = ContactInfoProvider.GetContactInfo(CurrentAccount.AccountPrimaryContactID);
            if (contact != null)
            {
                if (!String.IsNullOrEmpty(contact.ContactFirstName))
                {
                    contactWhere.Or().WhereContains("PrimaryContactFirstName", contact.ContactFirstName);
                }
                if (!String.IsNullOrEmpty(contact.ContactMiddleName))
                {
                    contactWhere.Or().WhereContains("PrimaryContactMiddleName", contact.ContactMiddleName);
                }
                if (!String.IsNullOrEmpty(contact.ContactLastName))
                {
                    contactWhere.Or().WhereContains("PrimaryContactLastName", contact.ContactLastName);
                }
            }
        }

        // Get secondary contact WHERE condition
        if (CurrentAccount.AccountSecondaryContactID != 0)
        {
            contact = ContactInfoProvider.GetContactInfo(CurrentAccount.AccountSecondaryContactID);
            if (contact != null)
            {
                if (!String.IsNullOrEmpty(contact.ContactFirstName))
                {
                    contactWhere.Or().WhereContains("SecondaryContactFirstName", contact.ContactFirstName);
                }
                if (!String.IsNullOrEmpty(contact.ContactMiddleName))
                {
                    contactWhere.Or().WhereContains("SecondaryContactMiddleName", contact.ContactMiddleName);
                }
                if (!String.IsNullOrEmpty(contact.ContactLastName))
                {
                    contactWhere.Or().WhereContains("SecondaryContactLastName", contact.ContactLastName);
                }
            }
        }
        return contactWhere;
    }

    #endregion
}