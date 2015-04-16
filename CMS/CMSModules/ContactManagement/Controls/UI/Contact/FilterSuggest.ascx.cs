using System;
using System.Linq;

using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_ContactManagement_Controls_UI_Contact_FilterSuggest : CMSUserControl
{
    #region "Variables"

    private ContactInfo ci;
    private int mSelectedSiteID;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets current contact.
    /// </summary>
    private ContactInfo CurrentContact
    {
        get
        {
            if ((ci == null) && (UIContext.EditedObject != null))
            {
                ci = (ContactInfo)UIContext.EditedObject;
            }
            return ci;
        }
    }


    /// <summary>
    /// Gets value indicating if IP address checkbox is selected.
    /// </summary>
    public bool IPAddressChecked
    {
        get
        {
            return chkIPaddress.Checked;
        }
    }


    /// <summary>
    /// Gets value indicating if e-mail address checkbox is selected.
    /// </summary>
    public bool EmailChecked
    {
        get
        {
            return chkEmail.Checked;
        }
    }


    /// <summary>
    /// Gets value indicating if phone checkbox is selected.
    /// </summary>
    public bool PhoneChecked
    {
        get
        {
            return chkPhone.Checked;
        }
    }


    /// <summary>
    /// Gets value indicating if birthday checkbox is selected.
    /// </summary>
    public bool BirthdayChecked
    {
        get
        {
            return chkBirthDay.Checked;
        }
    }


    /// <summary>
    /// Gets value indicating if membership checkbox is selected.
    /// </summary>
    public bool MembershipChecked
    {
        get
        {
            return chkMembership.Checked;
        }
    }


    /// <summary>
    /// Gets value indicating if post address checkbox is selected.
    /// </summary>
    public bool PostAddressChecked
    {
        get
        {
            return chkAddress.Checked;
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
        chkEmail.Enabled = !String.IsNullOrEmpty(ContactHelper.GetEmailDomain(CurrentContact.ContactEmail));
        chkAddress.Enabled = !String.IsNullOrEmpty(CurrentContact.ContactAddress1) || !String.IsNullOrEmpty(CurrentContact.ContactAddress2) || !String.IsNullOrEmpty(CurrentContact.ContactCity) || !String.IsNullOrEmpty(CurrentContact.ContactZIP);
        chkBirthDay.Enabled = (CurrentContact.ContactBirthday != DateTimeHelper.ZERO_TIME);
        chkPhone.Enabled = !String.IsNullOrEmpty(CurrentContact.ContactBusinessPhone) || !String.IsNullOrEmpty(CurrentContact.ContactHomePhone) || !String.IsNullOrEmpty(CurrentContact.ContactMobilePhone);
        chkMembership.Visible = chkIPaddress.Visible = ci.ContactSiteID != 0;

        if (chkMembership.Visible)
        {
            var relationships = ContactMembershipInfoProvider.GetRelationships()
                .WhereEquals("ActiveContactID", CurrentContact.ContactID);
            chkMembership.Enabled = relationships.Any();

            var ips = IPInfoProvider.GetIps()
                .WhereEquals("IPActiveContactID", CurrentContact.ContactID);
            chkIPaddress.Enabled = ips.Any();
        }

        // Current contact is global object
        if (ci.ContactSiteID == 0)
        {
            plcSite.Visible = true;
            // Display site selector in site manager
            if (ContactHelper.IsSiteManager)
            {
                siteOrGlobalSelector.Visible = false;
            }
            // Display 'site or global' selector in CMS desk for global objects
            else if (ContactHelper.AuthorizedReadContact(SiteContext.CurrentSiteID, false) && ContactHelper.AuthorizedModifyContact(SiteContext.CurrentSiteID, false))
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
        string where = null;

        // IP address checked
        if (chkIPaddress.Checked)
        {
            where = "ContactID IN (SELECT IP2.IPActiveContactID FROM OM_IP AS IP1 LEFT JOIN OM_IP AS IP2 ON IP1.IPAddress LIKE IP2.IPAddress WHERE IP1.IPID <> IP2.IPID AND IP1.IPActiveContactID = " + CurrentContact.ContactID + ")";
        }

        // Email address checked
        if (chkEmail.Checked)
        {
            string domain = ContactHelper.GetEmailDomain(CurrentContact.ContactEmail);
            if (!String.IsNullOrEmpty(domain))
            {
                string emailWhere = "ContactEmail LIKE N'%@" + SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(domain)) + "'";
                where = SqlHelper.AddWhereCondition(where, emailWhere);
            }
        }

        // Address checked
        if (chkAddress.Checked)
        {
            string addressWhere = null;
            if (!String.IsNullOrEmpty(CurrentContact.ContactAddress1))
            {
                addressWhere = SqlHelper.AddWhereCondition(addressWhere, "ContactAddress1 LIKE N'%" + SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(CurrentContact.ContactAddress1)) + "%'", "OR");
            }
            if (!String.IsNullOrEmpty(CurrentContact.ContactAddress2))
            {
                addressWhere = SqlHelper.AddWhereCondition(addressWhere, "ContactAddress2 LIKE N'%" + SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(CurrentContact.ContactAddress2)) + "%'", "OR");
            }
            if (!String.IsNullOrEmpty(CurrentContact.ContactCity))
            {
                addressWhere = SqlHelper.AddWhereCondition(addressWhere, "ContactCity LIKE N'%" + SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(CurrentContact.ContactCity)) + "%'", "OR");
            }
            if (!String.IsNullOrEmpty(CurrentContact.ContactZIP))
            {
                addressWhere = SqlHelper.AddWhereCondition(addressWhere, "ContactZIP LIKE N'%" + SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(CurrentContact.ContactZIP)) + "%'", "OR");
            }

            if (!String.IsNullOrEmpty(addressWhere))
            {
                where = SqlHelper.AddWhereCondition(where, "(" + addressWhere + ")");
            }
        }

        // Birthday checked
        if (chkBirthDay.Checked && (CurrentContact.ContactBirthday != DateTimeHelper.ZERO_TIME))
        {
            where = SqlHelper.AddWhereCondition(where, "ContactBirthDay = '" + FormHelper.GetDateTimeValueInSystemCulture(CurrentContact.ContactBirthday.ToString()) + "'");
        }

        // Phone checked
        if (chkPhone.Checked)
        {
            string phoneWhere = null;
            if (!String.IsNullOrEmpty(CurrentContact.ContactBusinessPhone))
            {
                phoneWhere = SqlHelper.AddWhereCondition(phoneWhere, "ContactBusinessPhone LIKE N'%" + SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(CurrentContact.ContactBusinessPhone)) + "%'");
            }
            if (!String.IsNullOrEmpty(CurrentContact.ContactHomePhone))
            {
                phoneWhere = SqlHelper.AddWhereCondition(phoneWhere, "ContactHomePhone LIKE N'%" + SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(CurrentContact.ContactHomePhone)) + "%'", "OR");
            }
            if (!String.IsNullOrEmpty(CurrentContact.ContactMobilePhone))
            {
                phoneWhere = SqlHelper.AddWhereCondition(phoneWhere, "ContactMobilePhone LIKE N'%" + SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(CurrentContact.ContactMobilePhone)) + "%'", "OR");
            }

            if (!String.IsNullOrEmpty(phoneWhere))
            {
                where = SqlHelper.AddWhereCondition(where, "(" + phoneWhere + ")");
            }
        }

        // Membership checked
        if (chkMembership.Checked)
        {
            where = SqlHelper.AddWhereCondition(where, "ContactID IN (SELECT Member2.ActiveContactID FROM OM_Membership AS Member1 LEFT JOIN OM_Membership AS Member2 ON Member1.RelatedID = Member2.RelatedID AND Member1.MemberType LIKE Member2.MemberType WHERE Member1.MembershipID <> Member2.MembershipID AND Member1.ActiveContactID = " + CurrentContact.ContactID + ")");
        }

        if ((!chkAddress.Checked && !chkBirthDay.Checked && !chkEmail.Checked && !chkIPaddress.Checked && !chkPhone.Checked && !chkMembership.Checked) || (String.IsNullOrEmpty(where)))
        {
            return "(1 = 0)";
        }

        // Filter out records related to current contact
        where = SqlHelper.AddWhereCondition(where, "ContactID <> " + CurrentContact.ContactID);
        // Filter out merged records - merging into global contact
        if (CurrentContact.ContactSiteID == 0)
        {
            where = SqlHelper.AddWhereCondition(where, "(ContactMergedWithContactID IS NULL AND ContactGlobalContactID IS NULL AND ContactSiteID > 0) OR (ContactGlobalContactID IS NULL AND ContactSiteID IS NULL)");
        }
        // Merging into site contact
        else
        {
            where = SqlHelper.AddWhereCondition(where, "(ContactMergedWithContactID IS NULL AND ContactSiteID > 0)");
        }

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
                where = SqlHelper.AddWhereCondition(where, "ContactSiteID IS NULL");
            }
            // Global and site objects
            else if (mSelectedSiteID == UniSelector.US_GLOBAL_AND_SITE_RECORD)
            {
                where = SqlHelper.AddWhereCondition(where, "ContactSiteID IS NULL OR ContactSiteID = " + SiteContext.CurrentSiteID);
            }
            // Site objects
            else if (mSelectedSiteID != UniSelector.US_ALL_RECORDS)
            {
                where = SqlHelper.AddWhereCondition(where, "ContactSiteID = " + mSelectedSiteID);
            }
        }
        // Filter out contacts from different sites
        else
        {
            // Site contacts only
            if (CurrentContact.ContactSiteID > 0)
            {
                where = SqlHelper.AddWhereCondition(where, "ContactSiteID = " + CurrentContact.ContactSiteID);
            }
            // Global contacts only
            else
            {
                where = SqlHelper.AddWhereCondition(where, "ContactSiteID IS NULL");
            }
        }

        return where;
    }

    #endregion
}