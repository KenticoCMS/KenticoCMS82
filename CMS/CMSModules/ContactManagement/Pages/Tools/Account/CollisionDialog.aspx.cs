using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Globalization;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_ContactManagement_Pages_Tools_Account_CollisionDialog : CMSModalPage
{
    #region "Variables"

    private string mIdentifier;
    private DataSet mMergedAccounts;
    private AccountInfo mParentAccount;
    private string mStamp;
    private readonly Hashtable mRoleControls = new Hashtable();
    private readonly Hashtable mRoles = new Hashtable();
    private readonly Hashtable mCustomFields = new Hashtable();
    private bool mIsSitemanager;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.PanelContent.RemoveCssClass("dialog-content");

        // Set title
        PageTitle.TitleText = GetString("om.contact.collision");
        // Validate hash
        Regex re = RegexHelper.GetRegex(@"[\w\d_$$]*");
        mIdentifier = QueryHelper.GetString("params", "");
        if (!QueryHelper.ValidateHash("hash") || !re.IsMatch(mIdentifier))
        {
            pnlContent.Visible = false;
            return;
        }

        // Load dialog parameters
        Hashtable parameters = (Hashtable)WindowHelper.GetItem(mIdentifier);
        if (parameters != null)
        {
            mMergedAccounts = (DataSet)parameters["MergedAccounts"];
            mParentAccount = (AccountInfo)parameters["ParentAccount"];

            if (!mParentAccount.CheckPermissions(PermissionsEnum.Read, CurrentSiteName, CurrentUser))
            {
                RedirectToAccessDenied(mParentAccount.TypeInfo.ModuleName, "Read");
            }

            mIsSitemanager = ValidationHelper.GetBoolean(parameters["issitemanager"], false);

            if (mIsSitemanager)
            {
                mStamp = SettingsKeyInfoProvider.GetValue("CMSCMStamp");
            }
            else
            {
                mStamp = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSCMStamp");
            }

            mStamp = MacroResolver.Resolve(mStamp);

            if (mParentAccount != null)
            {
                // Check permissions
                AccountHelper.AuthorizedReadAccount(mParentAccount.AccountSiteID, true);

                // Load data
                if (!RequestHelper.IsPostBack())
                {
                    Initialize();
                }
                LoadContactCollisions();
                LoadContactGroups();
                LoadCustomFields();

                // Init controls
                btnMerge.Click += new EventHandler(btnMerge_Click);
                btnStamp.OnClientClick = "AddStamp('" + htmlNotes.CurrentEditor.ClientID + "'); return false;";
                ScriptHelper.RegisterTooltip(Page);
                RegisterScripts();
                accountStatusSelector.SiteID = mParentAccount.AccountSiteID;
                accountSelector.SiteID = mParentAccount.AccountSiteID;
                accountSelector.WhereCondition = "((AccountMergedWithAccountID IS NULL) AND (AccountSiteID > 0)) OR ((AccountGlobalAccountID IS NULL) AND (AccountSiteID IS NULL))";
                accountSelector.WhereCondition = GetSubsidiaryWhere(accountSelector.WhereCondition);

                // Set tabs
                tabFields.HeaderText = GetString("om.contact.fields");
                tabContacts.HeaderText = GetString("om.contact.list");
                tabContactGroups.HeaderText = GetString("om.contactgroup.list");
                tabCustomFields.HeaderText = GetString("general.customfields");
            }
        }

        // User relative messages placeholder so that JQueryTab isn't moved a bit
        MessagesPlaceHolder.UseRelativePlaceHolder = false;

        // Do not let the editor overflow dialog window
        htmlNotes.SetValue("width", "520");
    }


    /// <summary>
    /// Initializes window with data
    /// </summary>
    private void Initialize()
    {
        if (!DataHelper.DataSourceIsEmpty(mMergedAccounts) && (mParentAccount != null))
        {
            BindCombobox(cmbAccountName, "AccountName", mParentAccount.AccountName, imgAccountName);
            BindCombobox(cmbAccountAddress1, "AccountAddress1", mParentAccount.AccountAddress1, imgAccountAddress1);
            BindCombobox(cmbAccountAddress2, "AccountAddress2", mParentAccount.AccountAddress2, imgAccountAddress2);
            BindCombobox(cmbAccountCity, "AccountCity", mParentAccount.AccountCity, imgAccountCity);
            BindCombobox(cmbAccountZIP, "AccountZIP", mParentAccount.AccountZIP, imgAccountZIP);
            BindCombobox(cmbAccountPhone, "AccountPhone", mParentAccount.AccountPhone, imgAccountPhone);
            BindCombobox(cmbAccountFax, "AccountFax", mParentAccount.AccountFax, imgAccountFax);
            BindCombobox(cmbAccountEmail, "AccountEmail", mParentAccount.AccountEmail, imgAccountEmail);
            BindCombobox(cmbAccountWebSite, "AccountWebSite", mParentAccount.AccountWebSite, imgAccountWebSite);

            InitAccountStatus();
            InitHeadquarters();
            InitCountry();
            InitNotes();

            lblOwner.Text = HTMLHelper.HTMLEncode(MembershipContext.AuthenticatedUser.FullName);
        }
    }


    /// <summary>
    /// Loads account-contact role collisions.
    /// </summary>
    private void LoadContactCollisions()
    {
        StringBuilder resultQuery = new StringBuilder("AccountID IN (" + mParentAccount.AccountID);
        foreach (DataRow dr in mMergedAccounts.Tables[0].Rows)
        {
            resultQuery.Append("," + dr["AccountID"]);
        }
        resultQuery.Append(")");
        // Get all account-contact relations
        DataSet relations = new AccountContactListInfo().Generalized.GetData(null, resultQuery.ToString(), null, -1, "ContactID,ContactFirstName,ContactMiddleName,ContactLastName,ContactRoleID", false);

        // Group by contactID to get distinct results
        DataTable result = relations.Tables[0].DefaultView.ToTable(true, "ContactID");
        int totalMerging = 0;

        // Display contact-account relations
        if (!DataHelper.DataSourceIsEmpty(result))
        {
            // Display prefix
            Literal prepend = new Literal();
            prepend.Text = "<div class=\"form-horizontal\">";
            plcAccountContact.Controls.Add(prepend);

            // Display collisions
            foreach (DataRow dr in result.Rows)
            {
                totalMerging += DisplayRoleCollisions(ValidationHelper.GetInteger(dr[0], 0), relations);
            }

            // Display suffix if any relation found
            if (totalMerging > 0)
            {
                Literal append = new Literal();
                append.Text = "</div>";
                plcAccountContact.Controls.Add(append);
            }
            else
            {
                tabContacts.Visible = false;
                tabContacts.HeaderText = null;
                plcAccountContact.Visible = false;
            }
        }
            // Hide content
        else
        {
            tabContacts.Visible = false;
            tabContacts.HeaderText = null;
            plcAccountContact.Visible = false;
        }
    }


    /// <summary>
    /// Loads contact groups of merged contacts into checkboxlist.
    /// </summary>
    private void LoadContactGroups()
    {
        if (!RequestHelper.IsPostBack())
        {
            StringBuilder idList = new StringBuilder("(");
            foreach (DataRow dr in mMergedAccounts.Tables[0].Rows)
            {
                idList.Append(dr["AccountID"] + ",");
            }
            // Remove last comma
            idList.Remove(idList.Length - 1, 1);
            idList.Append(")");

            // Remove site contact groups
            string addWhere = null;
            if (mParentAccount.AccountSiteID == 0)
            {
                addWhere = " AND ContactGroupMemberContactGroupID IN (SELECT ContactGroupID FROM OM_ContactGroup WHERE ContactGroupSiteID IS NULL)";
            }

            string where = "ContactGroupMemberType = 1 AND ContactGroupMemberRelatedID IN " + idList + " AND ContactGroupMemberContactGroupID NOT IN (SELECT ContactGroupMemberContactGroupID FROM OM_ContactGroupMember WHERE ContactGroupMemberRelatedID = " + mParentAccount.AccountID + " AND ContactGroupMemberType = 1)" + addWhere;

            // Limit selection of contact groups according to current user's permissions
            if (!MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
            {
                bool readModifySite = ContactGroupHelper.AuthorizedReadContactGroup(mParentAccount.AccountSiteID, false) && ContactGroupHelper.AuthorizedModifyContactGroup(mParentAccount.AccountSiteID, false);
                bool readGlobal = ContactGroupHelper.AuthorizedReadContactGroup(UniSelector.US_GLOBAL_RECORD, false) && ContactGroupHelper.AuthorizedModifyContactGroup(UniSelector.US_GLOBAL_RECORD, false);
                if (!readModifySite && !readGlobal)
                {
                    tabContactGroups.Visible = false;
                    tabContactGroups.HeaderText = null;
                }
                else if (readModifySite && !readGlobal)
                {
                    where = SqlHelper.AddWhereCondition(where, " ContactGroupMemberContactGroupID IN (SELECT ContactGroupID FROM OM_ContactGroup WHERE ContactGroupSiteID = " + SiteContext.CurrentSiteID + ")");
                }
                else if (!readModifySite && readGlobal)
                {
                    where = SqlHelper.AddWhereCondition(where, " ContactGroupMemberContactGroupID IN (SELECT ContactGroupID FROM OM_ContactGroup WHERE ContactGroupSiteID IS NULL)");
                }
                else
                {
                    where = SqlHelper.AddWhereCondition(where, " ContactGroupMemberContactGroupID IN (SELECT ContactGroupID FROM OM_ContactGroup WHERE ContactGroupSiteID IS NULL OR ContactGroupSiteID = " + SiteContext.CurrentSiteID + ")");
                }
            }

            // Get contact group relations
            DataSet result = ContactGroupMemberInfoProvider.GetRelationships().Where(where).Column("ContactGroupMemberContactGroupID").Distinct();

            if (!DataHelper.DataSourceIsEmpty(result))
            {
                ListItem contactGroup;
                ContactGroupInfo cg;
                foreach (DataRow dr in result.Tables[0].Rows)
                {
                    contactGroup = new ListItem();
                    contactGroup.Value = ValidationHelper.GetString(dr["ContactGroupMemberContactGroupID"], "0");
                    contactGroup.Selected = true;

                    // Fill in checkbox list
                    cg = ContactGroupInfoProvider.GetContactGroupInfo(ValidationHelper.GetInteger(dr["ContactGroupMemberContactGroupID"], 0));
                    if (cg != null)
                    {
                        contactGroup.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(cg.ContactGroupDisplayName));
                        chkContactGroups.Items.Add(contactGroup);
                    }
                }
            }
            else
            {
                tabContactGroups.Visible = false;
                tabContactGroups.HeaderText = null;
            }
        }
    }


    /// <summary>
    /// Loads custom fields collisions.
    /// </summary>
    private void LoadCustomFields()
    {
        // Check if account has any custom fields
        FormInfo formInfo = FormHelper.GetFormInfo(mParentAccount.ClassName, false);
        var list = formInfo.GetFormElements(true, false, true);
        if (list.OfType<FormFieldInfo>().Any())
        {
            FormFieldInfo ffi;
            Literal content;
            LocalizedLabel lbl;
            CMSTextBox txt;
            content = new Literal();
            content.Text = "<div class=\"form-horizontal\">";
            plcCustomFields.Controls.Add(content);

            // Display all custom fields
            foreach (IField item in list)
            {
                ffi = item as FormFieldInfo;
                if (ffi != null)
                {
                    // Display layout
                    content = new Literal();
                    content.Text = "<div class=\"form-group\"><div class=\"editing-form-label-cell\">";
                    plcCustomFields.Controls.Add(content);
                    lbl = new LocalizedLabel();
                    lbl.Text = ffi.GetDisplayName(MacroContext.CurrentResolver);
                    lbl.DisplayColon = true;
                    lbl.EnableViewState = false;
                    lbl.CssClass = "control-label";
                    content = new Literal();
                    content.Text = "</div><div class=\"editing-form-control-cell\"><div class=\"control-group-inline-forced\">";
                    txt = new CMSTextBox();
                    txt.ID = "txt" + ffi.Name;
                    lbl.AssociatedControlID = txt.ID;
                    plcCustomFields.Controls.Add(lbl);
                    plcCustomFields.Controls.Add(content);
                    plcCustomFields.Controls.Add(txt);
                    mCustomFields.Add(ffi.Name, new object[]
                    {
                        txt,
                        ffi.DataType
                    });
                    DataTable dt;

                    // Get grouped dataset
                    if (DataTypeManager.IsString(TypeEnum.Field, ffi.DataType))
                    {
                        dt = SortGroupAccountsByColumn(ffi.Name + SqlHelper.ORDERBY_ASC, ffi.Name + " NOT LIKE ''", ffi.Name);
                    }
                    else
                    {
                        dt = SortGroupAccountsByColumn(ffi.Name + SqlHelper.ORDERBY_ASC, ffi.Name + " IS NOT NULL", ffi.Name);
                    }

                    // Load value into textbox
                    txt.Text = ValidationHelper.GetString(mParentAccount.GetValue(ffi.Name), null);
                    if (string.IsNullOrEmpty(txt.Text) && (dt.Rows.Count > 0))
                    {
                        txt.Text = ValidationHelper.GetString(dt.Rows[0][ffi.Name], null);
                    }

                    // Display tooltip
                    var img = new HtmlGenericControl("i");
                    img.Attributes["class"] = "validation-warning icon-exclamation-triangle form-control-icon";
                    DisplayTooltip(img, dt, ffi.Name, ValidationHelper.GetString(mParentAccount.GetValue(ffi.Name), ""), ffi.DataType);
                    plcCustomFields.Controls.Add(img);
                    content = new Literal();
                    content.Text = "</div></div></div>";
                    plcCustomFields.Controls.Add(content);
                    mMergedAccounts.Tables[0].DefaultView.RowFilter = null;
                }
            }
            content = new Literal();
            content.Text = "</div>";
            plcCustomFields.Controls.Add(content);
        }
        else
        {
            tabCustomFields.Visible = false;
            tabCustomFields.HeaderText = null;
        }
    }


    /// <summary>
    /// Displays contact relation which has more than 1 role.
    /// </summary>
    private int DisplayRoleCollisions(int contactID, DataSet relations)
    {
        DataRow[] drs = relations.Tables[0].Select("ContactID = " + contactID + " AND ContactRoleID > 0", "ContactRoleID");

        // Contact is specified more than once
        if ((drs != null) && (drs.Length > 1))
        {
            // Find out if contact roles are different
            var roleIDs = new List<int>();
            int id;
            roleIDs.Add(ValidationHelper.GetInteger(drs[0]["ContactRoleID"], 0));
            mRoles.Add(drs[0]["ContactID"], drs[0]["ContactRoleID"]);
            foreach (DataRow dr in drs)
            {
                id = ValidationHelper.GetInteger(dr["ContactRoleID"], 0);
                if (!roleIDs.Contains(id))
                {
                    roleIDs.Add(id);
                }
            }

            // Display relation only for contacts with more roles
            if (roleIDs.Count > 1)
            {
                // Display table first part
                Literal ltl = new Literal();
                string contactName = drs[0]["ContactFirstName"] + " " + drs[0]["ContactMiddleName"];
                contactName = contactName.Trim() + " " + drs[0]["ContactLastName"];
                ltl.Text = "<div class=\"form-group\"><div class=\"editing-form-label-cell\"><span class=\"control-label\">" + HTMLHelper.HTMLEncode(contactName.Trim()) + "</span></div>";
                ltl.Text += "<div class=\"editing-form-value-cell\"><div class=\"control-group-inline-forced\">";
                plcAccountContact.Controls.Add(ltl);

                // Display role selector
                FormEngineUserControl roleSelector = Page.LoadUserControl("~/CMSModules/ContactManagement/FormControls/ContactRoleSelector.ascx") as FormEngineUserControl;
                roleSelector.SetValue("siteid", mParentAccount.AccountSiteID);
                plcAccountContact.Controls.Add(roleSelector);
                mRoleControls.Add(drs[0]["ContactID"], roleSelector);

                // Display tooltip
                var img = new HtmlGenericControl("i");
                img.Attributes["class"] = "validation-warning icon-exclamation-triangle form-control-icon";
                ScriptHelper.AppendTooltip(img, AccountContactTooltip(roleIDs), "help");
                plcAccountContact.Controls.Add(img);

                // Display table last part
                Literal ltlLast = new Literal();
                ltlLast.Text = "</div></div></div>";
                plcAccountContact.Controls.Add(ltlLast);

                return 1;
            }
        }

        return 0;
    }


    /// <summary>
    /// Fills tooltip with appropriate data.
    /// </summary>
    private string AccountContactTooltip(IList<int> roleIDs)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("<div><em>{0}</em></div>", GetString("om.contactrole.roles"));

        // Get contact role display names
        var contactRoles = ContactRoleInfoProvider.GetContactRoles()
                                                  .WhereIn("ContactRoleID", roleIDs)
                                                  .OrderBy("ContactRoleDisplayName")
                                                  .Column("ContactRoleDisplayName");


        // Loop through all distinct values of given column
        foreach (var contactRole in contactRoles)
        {
            sb.AppendFormat("<div>&nbsp;-&nbsp;{0}</div>", HTMLHelper.HTMLEncode(contactRole.ContactRoleDisplayName));
        }

        return sb.ToString();
    }


    /// <summary>
    /// Binds combobox control with data.
    /// </summary>
    private void BindCombobox(CMSFormControls_Basic_DropDownListControl control, string fieldName, string fieldValue, HtmlGenericControl image)
    {
        DataTable dt;
        // Get grouped dataset
        dt = SortGroupAccountsByColumn(fieldName + SqlHelper.ORDERBY_ASC, fieldName + " NOT LIKE ''", fieldName);

        // Bind control with data
        control.DropDownList.DataSource = dt;
        control.DropDownList.DataTextField = fieldName;
        control.DropDownList.DataValueField = fieldName;
        control.DataBind();

        // Insert parent value to first position
        if (!String.IsNullOrEmpty(fieldValue) && !control.DropDownList.Items.Contains(new ListItem(fieldValue)))
        {
            control.DropDownList.Items.Insert(0, fieldValue);
        }
        // Preselect parent value
        if (!String.IsNullOrEmpty(fieldValue) && control.DropDownList.Items.Contains(new ListItem(fieldValue)))
        {
            control.SelectedValue = fieldValue;
        }

        // Display appropriate icon
        DisplayTooltip(image, dt, fieldName, fieldValue, FieldDataType.Unknown);
    }


    /// <summary>
    /// Displays corresponding tooltip image.
    /// </summary>
    private bool DisplayTooltip(HtmlGenericControl image, DataTable dt, string fieldName, string fieldValue, string dataType)
    {
        // Single value - not collision
        if (((!String.IsNullOrEmpty(fieldValue)) && (DataHelper.DataSourceIsEmpty(dt) || ((dt.Rows.Count == 1) && (ValidationHelper.GetString(dt.Rows[0][fieldName], null) == fieldValue))))
            || ((String.IsNullOrEmpty(fieldValue) || (((fieldName == "AccountCountryID") || (fieldName == "AccountStateID")) && (ValidationHelper.GetInteger(fieldValue, -1) == 0))) && (dt.Rows.Count == 1)))
        {
            ScriptHelper.AppendTooltip(image, FillTooltipData(dt, fieldName, fieldValue, dataType), "help");
            image.Attributes["class"] = "validation-success icon-check-circle form-control-icon";
            image.Visible = true;
        }
            // No data - hide icon
        else if (String.IsNullOrEmpty(fieldValue) && DataHelper.DataSourceIsEmpty(dt))
        {
            image.Visible = false;
        }
            // Multiple values - collision
        else
        {
            ScriptHelper.AppendTooltip(image, FillTooltipData(dt, fieldName, fieldValue, dataType), "help");
            image.Attributes["class"] = "validation-warning icon-exclamation-triangle form-control-icon";
            image.Visible = true;
        }
        image.Style.Add("cursor", "help");

        // Reset row filter
        mMergedAccounts.Tables[0].DefaultView.RowFilter = null;
        return image.Visible;
    }


    /// <summary>
    /// Fills tooltip with appropriate data.
    /// </summary>
    private string FillTooltipData(DataTable dt, string fieldName, string fieldValue, string dataType)
    {
        var output = String.Empty;

        // Insert header into tooltip with parent value
        if (!String.IsNullOrEmpty(fieldValue))
        {
            output += "<em>" + GetString("om.account.parentvalue") + "</em> ";

            // Datetime values
            if (dataType == FieldDataType.DateTime)
            {
                output += "<strong>" + ValidationHelper.GetDateTime(fieldValue, DateTimeHelper.ZERO_TIME) + "</strong>";
            }
            else
            {
                // Country
                if (fieldName == "AccountCountryID")
                {
                    CountryInfo country = CountryInfoProvider.GetCountryInfo(ValidationHelper.GetInteger(fieldValue, 0));
                    if (country != null)
                    {
                        output += "<strong>" + HTMLHelper.HTMLEncode(country.CountryDisplayName) + "</strong>";
                    }
                    else
                    {
                        output += GetString("general.na");
                    }
                }
                    // State
                else if (fieldName == "AccountStateID")
                {
                    StateInfo state = StateInfoProvider.GetStateInfo(ValidationHelper.GetInteger(fieldValue, 0));
                    if (state != null)
                    {
                        output += "<strong>" + HTMLHelper.HTMLEncode(state.StateDisplayName) + "</strong>";
                    }
                    else
                    {
                        output += GetString("general.na");
                    }
                }
                    // Status
                else if (fieldName == "AccountStatusID")
                {
                    AccountStatusInfo status = AccountStatusInfoProvider.GetAccountStatusInfo(ValidationHelper.GetInteger(fieldValue, 0));
                    if (status != null)
                    {
                        output += "<strong>" + HTMLHelper.HTMLEncode(status.AccountStatusDisplayName) + "</strong>";
                    }
                    else
                    {
                        output += GetString("general.na");
                    }
                }
                    // Otherwise
                else
                {
                    output += "<strong>" + HTMLHelper.HTMLEncode(fieldValue) + "</strong>";
                }
            }
        }
        else
        {
            output += "<em>" + GetString("om.account.parentvalue") + "</em> " + GetString("general.na");
        }
        output += "<p><em>" + GetString("om.account.mergedvalues") + "</em></p>";

        // Display N/A for empty merged records
        if (DataHelper.DataSourceIsEmpty(dt))
        {
            output += "<div>" + GetString("general.na") + "</div>";
        }
            // Display values of merged records
        else
        {
            // Loop through all distinct values of given column
            foreach (DataRow dr in dt.Rows)
            {
                output += "<div>";

                // Sort accounts by full name
                var accountsView = mMergedAccounts.Tables[0].DefaultView;
                accountsView.Sort = "AccountName";

                mMergedAccounts.CaseSensitive = true;

                var value = dr[fieldName];

                // Need to transform status ID to displayname
                if (fieldName == "AccountStatusID")
                {
                    AccountStatusInfo status = AccountStatusInfoProvider.GetAccountStatusInfo((int)value);
                    output += GetTooltipItem(ResHelper.LocalizeString(status.AccountStatusDisplayName));
                    accountsView.RowFilter = fieldName + " = '" + status.AccountStatusID + "'";
                }
                    // Need to transform country ID to displayname
                else if (fieldName == "AccountCountryID")
                {
                    CountryInfo country = CountryInfoProvider.GetCountryInfo((int)value);
                    output += GetTooltipItem(country.CountryDisplayName);
                    accountsView.RowFilter = fieldName + " = '" + country.CountryID + "'";
                }
                    // Need to transform state ID to displayname
                else if (fieldName == "AccountStateID")
                {
                    StateInfo state = StateInfoProvider.GetStateInfo((int)value);
                    output += GetTooltipItem(state.StateDisplayName);
                    accountsView.RowFilter = fieldName + " = '" + state.StateID + "'";
                }
                    // Other fields, process based on field type
                else
                {
                    output += GetTooltipItem(ValidationHelper.GetString(value, null));
                    accountsView.RowFilter = fieldName + " = '" + ContactHelper.EscapeString(ValidationHelper.GetString(value, "")) + "'";
                }

                // Display all accounts
                var accounts = accountsView.ToTable(false, "AccountName");
                foreach (DataRow row in accounts.Rows)
                {
                    output += "<div>&nbsp;-&nbsp;" + HTMLHelper.HTMLEncode(((string)row["AccountName"]).Trim()) + "</div>";
                }
                output += "</div>";
            }
        }

        return output;
    }


    /// <summary>
    /// Gets the tooltip item code
    /// </summary>
    /// <param name="text">Item text</param>
    private static string GetTooltipItem(string text)
    {
        return "<div><strong>" + HTMLHelper.HTMLEncode(text) + "</strong></div>";
    }


    /// <summary>
    /// Initializes account status selector.
    /// </summary>
    private void InitAccountStatus()
    {
        DataTable table = SortGroupAccountsByColumn("AccountStatusID", "AccountStatusID > 0", "AccountStatusID");

        // Preselect account status with data from parent
        if (mParentAccount.AccountStatusID > 0)
        {
            accountStatusSelector.Value = mParentAccount.AccountStatusID;
        }
            // Preselect account status with data from merged accounts only if single value exists in merged items
        else if ((table.Rows != null) && (table.Rows.Count > 0))
        {
            accountStatusSelector.Value = ValidationHelper.GetInteger(table.Rows[0][0], 0);
        }

        DisplayTooltip(imgAccountStatus, table, "AccountStatusID", mParentAccount.AccountStatusID.ToString(), FieldDataType.Unknown);
    }


    /// <summary>
    /// Initializes headquarters selector
    /// </summary>
    private void InitHeadquarters()
    {
        mMergedAccounts.Tables[0].DefaultView.Sort = "AccountSubsidiaryOfID";
        mMergedAccounts.Tables[0].DefaultView.RowFilter = "AccountSubsidiaryOfID > 0";
        DataTable table = mMergedAccounts.Tables[0].DefaultView.ToTable(true, new string[]
        {
            "AccountSubsidiaryOfID",
            "SubsidiaryOfName"
        });

        // Preselect headquarters with data from parent
        if (mParentAccount.AccountSubsidiaryOfID > 0)
        {
            accountSelector.Value = mParentAccount.AccountSubsidiaryOfID;
        }
            // Preselect account status with data from merged accounts only if single value exists in merged items
        else if ((table.Rows != null) && (table.Rows.Count > 0))
        {
            accountSelector.Value = ValidationHelper.GetInteger(table.Rows[0][0], 0);
        }

        AccountInfo ai = AccountInfoProvider.GetAccountInfo(mParentAccount.AccountSubsidiaryOfID);
        string name = ai == null ? null : ai.AccountName;
        DisplayTooltip(imgAccountHeadquarters, table, "SubsidiaryOfName", name, FieldDataType.Unknown);
    }


    /// <summary>
    /// Initializes country selector with tooltip.
    /// </summary>
    private void InitCountry()
    {
        // Init countries
        DataTable table = SortGroupAccountsByColumn("AccountCountryID", "AccountCountryID > 0", "AccountCountryID");
        countrySelector.CountryID = PreselectCountryState(mParentAccount.AccountCountryID, table);
        bool display = DisplayTooltip(imgAccountCountry, table, "AccountCountryID", mParentAccount.AccountCountryID.ToString(), FieldDataType.Unknown);

        // Init states
        table = SortGroupAccountsByColumn("AccountStateID", "AccountStateID > 0", "AccountStateID");
        countrySelector.StateID = PreselectCountryState(mParentAccount.AccountStateID, table);
        display &= DisplayTooltip(imgAccountState, table, "AccountStateID", mParentAccount.AccountStateID.ToString(), FieldDataType.Unknown);
    }


    /// <summary>
    /// Initializes notes.
    /// </summary>
    private void InitNotes()
    {
        // Merge accounts notes
        htmlNotes.Value = mParentAccount.AccountNotes;
        DataTable table = SortGroupAccountsByColumn(null, "AccountNotes NOT LIKE ''", "AccountNotes");

        // Preselect value only if single value exists in merged items
        if ((table.Rows != null) && (table.Rows.Count > 0))
        {
            foreach (DataRow dr in table.Rows)
            {
                htmlNotes.Value += ValidationHelper.GetString(dr[0], null);
            }

            htmlNotes.Value += "<div>" + mStamp + "</div><div>" + GetString("om.contact.notesmerged") + "</div>";
        }
    }


    /// <summary>
    /// Button merge click.
    /// </summary>
    private void btnMerge_Click(object sender, EventArgs e)
    {
        // Check permissions
        if (AccountHelper.AuthorizedModifyAccount(mParentAccount.AccountSiteID, true))
        {
            // Validate form
            if (ValidateForm())
            {
                // Change parent contact values
                SaveChanges();

                // Update hashtable with account-contact roles
                UpdateRoles();

                // Merge contacts
                AccountHelper.Merge(mParentAccount, mMergedAccounts, mRoles, GetContactGroups());

                // Close window and refresh parent window
                ScriptHelper.RegisterClientScriptBlock(this, typeof (string), "WOpenerRefresh", ScriptHelper.GetScript(@"wopener.RefreshPage(); CloseDialog();"));
            }
        }
    }


    /// <summary>
    /// Returns hashtable based on selected values of contact groups.
    /// </summary>
    private Hashtable GetContactGroups()
    {
        Hashtable selectedContactGroups = new Hashtable();
        foreach (ListItem cg in chkContactGroups.Items)
        {
            selectedContactGroups.Add(ValidationHelper.GetInteger(cg.Value, 0), cg.Selected);
        }
        return selectedContactGroups;
    }


    /// <summary>
    /// Updates contact roles with roles selected by user.
    /// </summary>
    private void UpdateRoles()
    {
        foreach (int key in mRoleControls.Keys)
        {
            mRoles[key] = ((FormEngineUserControl)mRoleControls[key]).Value;
        }
    }


    /// <summary>
    /// Saves merge changes.
    /// </summary>
    private void SaveChanges()
    {
        mParentAccount.AccountName = cmbAccountName.Text.Trim();
        mParentAccount.AccountAddress1 = cmbAccountAddress1.Text.Trim();
        mParentAccount.AccountAddress2 = cmbAccountAddress2.Text.Trim();
        mParentAccount.AccountCity = cmbAccountCity.Text.Trim();
        mParentAccount.AccountZIP = cmbAccountZIP.Text.Trim();
        mParentAccount.AccountStateID = countrySelector.StateID;
        mParentAccount.AccountCountryID = countrySelector.CountryID;
        mParentAccount.AccountWebSite = cmbAccountWebSite.Text.Trim();
        mParentAccount.AccountPhone = cmbAccountPhone.Text.Trim();
        mParentAccount.AccountEmail = cmbAccountEmail.Text.Trim();
        mParentAccount.AccountFax = cmbAccountFax.Text.Trim();
        mParentAccount.AccountStatusID = accountStatusSelector.AccountStatusID;
        mParentAccount.AccountNotes = (string)htmlNotes.Value;
        mParentAccount.AccountOwnerUserID = MembershipContext.AuthenticatedUser.UserID;
        mParentAccount.AccountSubsidiaryOfID = accountSelector.AccountID;

        // Save cusotm fields
        foreach (string key in mCustomFields.Keys)
        {
            // Get value from
            object value = ((object[])mCustomFields[key])[0];
            var datatype = (string)((object[])mCustomFields[key])[1];
            string text = ((TextBox)value).Text;

            if (!String.IsNullOrEmpty(text))
            {
                // Convert the value to a proper type
                var convertedValue = DataTypeManager.ConvertToSystemType(TypeEnum.Field, datatype, text);

                // Set the parent value
                mParentAccount.SetValue(key, convertedValue);
            }
        }

        AccountInfoProvider.SetAccountInfo(mParentAccount);
    }


    /// <summary>
    /// Sorts, filters and groups accounts by specified rules
    /// </summary>
    /// <param name="sortRule">Sorting rule by specified column</param>
    /// <param name="rowFilter">Filtering rule by specified column</param>
    /// <param name="groupByColumn">Groupping by specified column</param>
    /// <returns>Returns sorted DataTable</returns>
    private DataTable SortGroupAccountsByColumn(string sortRule, string rowFilter, string groupByColumn)
    {
        mMergedAccounts.Tables[0].DefaultView.Sort = sortRule;
        mMergedAccounts.Tables[0].DefaultView.RowFilter = rowFilter;
        return mMergedAccounts.Tables[0].DefaultView.ToTable(true, groupByColumn);
    }


    /// <summary>
    /// Preselects Country/State selector with default value.
    /// </summary>
    /// <param name="parentValue">Parent value</param>
    /// <param name="table">Grouped child values</param>
    /// <returns>Selected value</returns>
    private int PreselectCountryState(int parentValue, DataTable table)
    {
        if (parentValue > 0)
        {
            return parentValue;
        }
        else if ((table.Rows != null) && (table.Rows.Count > 0))
        {
            return ValidationHelper.GetInteger(table.Rows[0][0], 0);
        }
        return 0;
    }


    /// <summary>
    /// Performs custom validation and displays error in top of the page.
    /// </summary>
    /// <returns>Returns true if validation is successful.</returns>
    private bool ValidateForm()
    {
        // Validate name
        string errorMessage = new Validator().NotEmpty(cmbAccountName.Text.Trim(), GetString("om.account.entername")).Result;
        if (!String.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
            return false;
        }

        // Validates email
        if (!String.IsNullOrEmpty(cmbAccountEmail.Text) && !ValidationHelper.IsEmail(cmbAccountEmail.Text))
        {
            ShowError(GetString("om.contact.enteremail"));
            return false;
        }

        return true;
    }


    /// <summary>
    /// Registers JavaScripts on page.
    /// </summary>
    private void RegisterScripts()
    {
        ScriptHelper.RegisterClientScriptBlock(Page, typeof (string), "AddStamp", ScriptHelper.GetScript(
            @"function InsertHTML(htmlString, ckClientID)
{
    // Get the editor instance that we want to interact with.
    var oEditor = oEditor = window.CKEDITOR.instances[ckClientID];
    // Check the active editing mode.
    if (oEditor != null) {
        // Check the active editing mode.
        if (oEditor.mode == 'wysiwyg') {
            // Insert the desired HTML.
            oEditor.focus();
            oEditor.insertHtml(htmlString);        
        }
    }    
    return false;
}   

function AddStamp(ckClientID)
{
    InsertHTML('<div>" + MacroResolver.Resolve(mStamp).Replace("'", @"\'") + @"</div>', ckClientID);
}"
            ));
    }


    /// <summary>
    /// Returns WHERE condition limiting account selector for subsidiaries.
    /// </summary>
    /// <param name="currentWhere">WHERE condition</param>
    /// <returns>Modified WHERE condition</returns>
    private string GetSubsidiaryWhere(string currentWhere)
    {
        foreach (int id in GetAccountIDs())
        {
            currentWhere = SqlHelper.AddWhereCondition(currentWhere, "AccountID NOT IN (SELECT * FROM Func_OM_Account_GetSubsidiaries(" + id + ", 1))");
        }

        return currentWhere;
    }


    /// <summary>
    /// Returns list of all merged account IDs including parent account ID separated by colon.
    /// </summary>
    /// <returns>String of IDs separated by colon</returns>
    private List<int> GetAccountIDs()
    {
        List<int> IDlist = new List<int>();
        IDlist.Add(mParentAccount.AccountID);

        foreach (DataRow dr in mMergedAccounts.Tables[0].Rows)
        {
            IDlist.Add(ValidationHelper.GetInteger(dr["AccountID"], 0));
        }

        return IDlist;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        imgAccountState.Visible = lblState.Visible = countrySelector.StateIsVisible();
    }

    #endregion
}