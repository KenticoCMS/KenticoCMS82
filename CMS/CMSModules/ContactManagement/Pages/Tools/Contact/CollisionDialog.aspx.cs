using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using CMS.Base;
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

public partial class CMSModules_ContactManagement_Pages_Tools_Contact_CollisionDialog : CMSModalPage
{
    #region "Variables"

    private string mIdentifier;
    private DataSet mMergedContacts;
    private ContactInfo mParentContact;
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
            mMergedContacts = (DataSet)parameters["MergedContacts"];
            mParentContact = (ContactInfo)parameters["ParentContact"];

            if (!mParentContact.CheckPermissions(PermissionsEnum.Read, CurrentSiteName, CurrentUser))
            {
                RedirectToAccessDenied(mParentContact.TypeInfo.ModuleName, "Read");
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

            if (mParentContact != null)
            {
                // Check permissions
                ContactHelper.AuthorizedReadContact(mParentContact.ContactSiteID, true);

                // Load data
                Initialize();
                LoadContactCollisions();
                LoadContactGroups();
                LoadCustomFields();

                // Init controls
                btnMerge.Click += new EventHandler(btnMerge_Click);
                btnStamp.OnClientClick = "AddStamp('" + htmlNotes.CurrentEditor.ClientID + "'); return false;";
                ScriptHelper.RegisterTooltip(Page);
                RegisterScripts();

                // Set tabs
                tabFields.HeaderText = GetString("om.contact.fields");
                tabContacts.HeaderText = GetString("om.account.list");
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
    /// Loads account-contact role collisions.
    /// </summary>
    private void LoadContactCollisions()
    {
        StringBuilder resultQuery = new StringBuilder("ContactID IN (" + mParentContact.ContactID);
        foreach (DataRow dr in mMergedContacts.Tables[0].Rows)
        {
            resultQuery.Append("," + dr["ContactID"]);
        }
        resultQuery.Append(")");
        // Get all account-contact relations
        DataSet relations = new ContactAccountListInfo().Generalized.GetData(null, resultQuery.ToString(), null, -1, "AccountID,AccountName,ContactRoleID", false);

        // Group by AccountID to get distinct results
        DataTable result = relations.Tables[0].DefaultView.ToTable(true, "AccountID");
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
                headContactInfo.ResourceString = "om.contact.accountroles";
            }
            else
            {
                tabContacts.HeaderText = null;
                tabContacts.Visible = false;
                plcAccountContact.Visible = false;
            }
        }
            // Hide content
        else
        {
            tabContacts.HeaderText = null;
            tabContacts.Visible = false;
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
            foreach (DataRow dr in mMergedContacts.Tables[0].Rows)
            {
                idList.Append(dr["ContactID"] + ",");
            }
            // Remove last comma
            idList.Remove(idList.Length - 1, 1);
            idList.Append(")");

            // Remove site contact groups for global contact
            string addWhere = null;
            if (mParentContact.ContactSiteID == 0)
            {
                addWhere = " AND ContactGroupMemberContactGroupID IN (SELECT ContactGroupID FROM OM_ContactGroup WHERE ContactGroupSiteID IS NULL)";
            }

            string where = " ContactGroupMemberType = 0 AND ContactGroupMemberRelatedID IN " + idList + " AND ContactGroupMemberContactGroupID NOT IN (SELECT ContactGroupMemberContactGroupID FROM OM_ContactGroupMember WHERE ContactGroupMemberRelatedID = " + mParentContact.ContactID + " AND ContactGroupMemberType = 0)" + addWhere;

            // Show only manually added contact groups
            where = SqlHelper.AddWhereCondition(where, "ContactGroupMemberFromManual = 1");

            // Limit selection of contact groups according to current user's permissions
            if (!MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
            {
                bool readModifySite = ContactGroupHelper.AuthorizedReadContactGroup(mParentContact.ContactSiteID, false) && ContactGroupHelper.AuthorizedModifyContactGroup(mParentContact.ContactSiteID, false);
                bool readModifyGlobal = ContactGroupHelper.AuthorizedReadContactGroup(UniSelector.US_GLOBAL_RECORD, false) && ContactGroupHelper.AuthorizedModifyContactGroup(UniSelector.US_GLOBAL_RECORD, false);

                if (!readModifySite && !readModifyGlobal)
                {
                    tabContactGroups.Visible = false;
                    tabContactGroups.HeaderText = null;
                }
                else if (readModifySite && !readModifyGlobal)
                {
                    where = SqlHelper.AddWhereCondition(where, " ContactGroupMemberContactGroupID IN (SELECT ContactGroupID FROM OM_ContactGroup WHERE ContactGroupSiteID = " + SiteContext.CurrentSiteID + ")");
                }
                else if (!readModifySite && readModifyGlobal)
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
        // Check if contact has any custom fields
        FormInfo formInfo = FormHelper.GetFormInfo(mParentContact.ClassName, false);
        var list = formInfo.GetFormElements(true, false, true);
        if (list.OfType<FormFieldInfo>().Any())
        {
            FormFieldInfo ffi;
            Literal content;
            LocalizedLabel lbl;
            CMSTextBox txt;
            content = new Literal();
            content.Text = "<div class=\"form-horizontal form-merge-collisions\">";
            plcCustomFields.Controls.Add(content);

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
                    content.Text = "</div><div class=\"editing-form-value-cell\"><div class=\"control-group-inline-forced\">";
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
                        dt = SortGroupContactsByColumn(ffi.Name + SqlHelper.ORDERBY_ASC, ffi.Name + " NOT LIKE ''", ffi.Name);
                    }
                    else
                    {
                        dt = SortGroupContactsByColumn(ffi.Name + SqlHelper.ORDERBY_ASC, ffi.Name + " IS NOT NULL", ffi.Name);
                    }

                    // Load value into textbox
                    txt.Text = ValidationHelper.GetString(mParentContact.GetValue(ffi.Name), null);
                    if (string.IsNullOrEmpty(txt.Text) && (dt.Rows.Count > 0))
                    {
                        txt.Text = ValidationHelper.GetString(dt.Rows[0][ffi.Name], null);
                    }

                    var img = new HtmlGenericControl("i");
                    img.Attributes["class"] = "validation-warning icon-exclamation-triangle form-control-icon";
                    DisplayTooltip(img, dt, ffi.Name, ValidationHelper.GetString(mParentContact.GetValue(ffi.Name), ""), ffi.DataType);
                    plcCustomFields.Controls.Add(img);
                    content = new Literal();
                    content.Text = "</div></div></div>";
                    plcCustomFields.Controls.Add(content);
                    mMergedContacts.Tables[0].DefaultView.RowFilter = null;
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
    /// Displays account relation which has more than 1 role.
    /// </summary>
    private int DisplayRoleCollisions(int accountID, DataSet relations)
    {
        DataRow[] drs = relations.Tables[0].Select("AccountID = " + accountID + " AND ContactRoleID > 0", "ContactRoleID");

        // Account is specified more than once
        if ((drs != null) && (drs.Length > 1))
        {
            // Find out if contact roles are different
            var roleIDs = new List<int>();
            int id;
            roleIDs.Add(ValidationHelper.GetInteger(drs[0]["ContactRoleID"], 0));
            mRoles.Add(drs[0]["AccountID"], drs[0]["ContactRoleID"]);
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
                ltl.Text = "<div class=\"control-group\"><div class=\"editing-form-label-cell\"><span class=\"control-label\">" + HTMLHelper.HTMLEncode(DataHelper.GetStringValue(drs[0], "AccountName")) + "</span></div><div class=\"editing-form-value-cell\"><div class=\"control-group-inline-forced\">";
                plcAccountContact.Controls.Add(ltl);

                // Display role selector
                FormEngineUserControl roleSelector = Page.LoadUserControl("~/CMSModules/ContactManagement/FormControls/ContactRoleSelector.ascx") as FormEngineUserControl;
                roleSelector.SetValue("siteid", mParentContact.ContactSiteID);
                plcAccountContact.Controls.Add(roleSelector);
                mRoleControls.Add(drs[0]["AccountID"], roleSelector);

                // Display icon with tooltip

                var img = new HtmlGenericControl("i");
                img.Attributes["class"] = "validation-warning icon-exclamation-triangle form-control-icon";
                //DisplayTooltip(img, dt, ffi.Name, ValidationHelper.GetString(parentContact.GetValue(ffi.Name), ""), ffi.DataType);

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
    /// Initializes window with data
    /// </summary>
    private void Initialize()
    {
        if (!DataHelper.DataSourceIsEmpty(mMergedContacts) && (mParentContact != null))
        {
            if (!RequestHelper.IsPostBack())
            {
                BindCombobox(cmbContactFirstName, "ContactFirstName", mParentContact.ContactFirstName, imgContactFirstName);
                BindCombobox(cmbContactMiddleName, "ContactMiddleName", mParentContact.ContactMiddleName, imgContactMiddleName);
                BindCombobox(cmbContactSalutation, "ContactSalutation", mParentContact.ContactSalutation, imgContactSalutation);
                BindCombobox(cmbContactTitleBefore, "ContactTitleBefore", mParentContact.ContactTitleBefore, imgContactTitleBefore);
                BindCombobox(cmbContactTitleAfter, "ContactTitleAfter", mParentContact.ContactTitleAfter, imgContactTitleAfter);
                BindCombobox(cmbContactCompanyName, "ContactCompanyName", mParentContact.ContactCompanyName, imgContactCompanyName);
                BindCombobox(cmbContactJobTitle, "ContactJobTitle", mParentContact.ContactJobTitle, imgContactJobTitle);
                BindCombobox(cmbContactAddress1, "ContactAddress1", mParentContact.ContactAddress1, imgContactAddress1);
                BindCombobox(cmbContactAddress2, "ContactAddress2", mParentContact.ContactAddress2, imgContactAddress2);
                BindCombobox(cmbContactCity, "ContactCity", mParentContact.ContactCity, imgContactCity);
                BindCombobox(cmbContactZIP, "ContactZIP", mParentContact.ContactZIP, imgContactZIP);
                BindCombobox(cmbContactMobilePhone, "ContactMobilePhone", mParentContact.ContactMobilePhone, imgContactMobilePhone);
                BindCombobox(cmbContactHomePhone, "ContactHomePhone", mParentContact.ContactHomePhone, imgContactHomePhone);
                BindCombobox(cmbContactBusinessPhone, "ContactBusinessPhone", mParentContact.ContactBusinessPhone, imgContactBusinessPhone);
                BindCombobox(cmbContactEmail, "ContactEmail", mParentContact.ContactEmail, imgContactEmail);
                BindCombobox(cmbContactWebSite, "ContactWebSite", mParentContact.ContactWebSite, imgContactWebSite);

                InitLastName();
                InitCountrySelector();
                InitBirthday();
                InitGender();
                InitNotes();
                InitMonitored();
                InitContactStatus();
                InitCampaign();

                lblOwner.Text = HTMLHelper.HTMLEncode(MembershipContext.AuthenticatedUser.FullName);
            }
        }
    }


    /// <summary>
    /// Binds combobox control with data.
    /// </summary>
    private void BindCombobox(CMSFormControls_Basic_DropDownListControl control, string fieldName, string fieldValue, HtmlGenericControl image)
    {
        // Get grouped dataset
        DataTable dt = SortGroupContactsByColumn(fieldName + SqlHelper.ORDERBY_ASC, fieldName + " NOT LIKE ''", fieldName);

        // Bind control with data
        control.DropDownList.DataSource = dt;
        control.DropDownList.DataTextField = fieldName;
        control.DropDownList.DataValueField = fieldName;
        control.DataBind();

        // Insert parent value to first position
        if (!String.IsNullOrEmpty(fieldValue))
        {
            if (!control.DropDownList.Items.Contains(new ListItem(fieldValue)))
            {
                control.DropDownList.Items.Insert(0, fieldValue);
            }
            // Preselect parent value
            control.SelectedValue = fieldValue;
        }

        foreach (ListItem item in control.DropDownList.Items)
        {
            item.Text = HTMLHelper.HTMLEncode(item.Text);
        }

        // Display appropriate icon
        DisplayTooltip(image, dt, fieldName, fieldValue, FieldDataType.Unknown);
    }


    /// <summary>
    /// Displays corresponding tooltip image for 'monitored' checkbox.
    /// </summary>
    private void DisplayTooltipForMonitored(HtmlGenericControl image, DataTable tableTrueValues, DataTable tableFalseValues, bool fieldValue)
    {
        FillTooltipDataForMonitored(image, fieldValue);

        // Single value - not collision
        if ((fieldValue && DataHelper.DataSourceIsEmpty(tableFalseValues)) || (!fieldValue && DataHelper.DataSourceIsEmpty(tableTrueValues)))
        {
            image.Attributes["class"] = "validation-success icon-check-circle form-control-icon";
            image.Visible = true;
        }
            // Multiple values - collision
        else
        {
            image.Attributes["class"] = "validation-warning icon-exclamation-triangle form-control-icon";
            image.Visible = true;
        }

        image.Style.Add("cursor", "help");

        // Reset row filter
        mMergedContacts.Tables[0].DefaultView.RowFilter = null;
    }


    /// <summary>
    /// Fill tooltip data for 'monitored' checkbox.
    /// </summary>
    private string FillTooltipDataForMonitored(HtmlGenericControl image, bool fieldValue)
    {
        var output = String.Empty;

        // Insert header into tooltip with parent value
        output += "<em>" + GetString("om.contact.parentvalue") + "</em> <strong>";
        if (fieldValue)
        {
            output += GetString("om.contact.ismonitored");
        }
        else
        {
            output += GetString("om.contact.isnotmonitored");
        }
        output += "</strong>";

        // Merged values
        output += "<div><em>" + GetString("om.contact.mergedvalues") + "</em></div>";

        // Loop through all TRUE values of given column
        output += "<div><strong>" + GetString("om.contact.ismonitored") + "</strong></div>";
        // Sort contacts by full name
        mMergedContacts.Tables[0].DefaultView.Sort = "ContactFullNameJoined";
        mMergedContacts.Tables[0].DefaultView.RowFilter = "ContactMonitored = 1";
        // Display all contacts
        DataTable contacts = mMergedContacts.Tables[0].DefaultView.ToTable(false, "ContactFullNameJoined");
        foreach (DataRow contactRow in contacts.Rows)
        {
            output += "<div>&nbsp;-&nbsp;" + HTMLHelper.HTMLEncode(ValidationHelper.GetString(contactRow["ContactFullNameJoined"], "").Trim()) + "</div>";
        }

        // Loop through all FALSE values of given column
        output += "<div><strong>" + GetString("om.contact.isnotmonitored") + "</strong></div>";
        // Sort contacts by full name
        mMergedContacts.Tables[0].DefaultView.Sort = "ContactFullNameJoined";
        mMergedContacts.Tables[0].DefaultView.RowFilter = "(ContactMonitored = 0) OR (ContactMonitored IS NULL)";
        // Display all contacts
        contacts = mMergedContacts.Tables[0].DefaultView.ToTable(false, "ContactFullNameJoined");
        foreach (DataRow contactRow in contacts.Rows)
        {
            output += "<div>&nbsp;-&nbsp;" + HTMLHelper.HTMLEncode(ValidationHelper.GetString(contactRow["ContactFullNameJoined"], "").Trim()) + "</div>";
        }

        return output;
    }


    /// <summary>
    /// Displays corresponding tooltip image.
    /// </summary>
    private bool DisplayTooltip(HtmlGenericControl image, DataTable dt, string fieldName, string fieldValue, string dataType)
    {
        // Single value - not collision
        if (((!String.IsNullOrEmpty(fieldValue)) && (DataHelper.DataSourceIsEmpty(dt) || ((dt.Rows.Count == 1) && (ValidationHelper.GetString(dt.Rows[0][fieldName], null) == fieldValue))))
            || ((String.IsNullOrEmpty(fieldValue) || (((fieldName == "ContactCountryID") || (fieldName == "ContactStateID")) && (ValidationHelper.GetInteger(fieldValue, -1) == 0))) && (dt.Rows.Count == 1))
            || ((fieldName == "ContactGender") && (dt.Rows.Count == 1) && (ValidationHelper.GetInteger(dt.Rows[0][fieldName], 0) == (int)UserGenderEnum.Unknown)))
        {
            ScriptHelper.AppendTooltip(image, FillTooltipData(dt, fieldName, fieldValue, dataType), "help");
            image.Attributes["class"] = "validation-success icon-check-circle form-control-icon";
            image.Visible = true;
        }
            // Hide icon - no data for given field
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
        mMergedContacts.Tables[0].DefaultView.RowFilter = null;
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
            // Display birthday
            if (fieldName == "ContactBirthday")
            {
                output += "<em>" + GetString("om.contact.parentvalue") + "</em> <strong>" + HTMLHelper.HTMLEncode(ValidationHelper.GetDateTime(fieldValue, DateTimeHelper.ZERO_TIME).ToShortDateString()) + "</strong>";
            }
                // Display gender
            else if (fieldName == "ContactGender")
            {
                int gender = ValidationHelper.GetInteger(fieldValue, 0);
                if (gender == (int)UserGenderEnum.Male)
                {
                    output += "<em>" + GetString("om.contact.parentvalue") + "</em> <strong>" + GetString("general.male") + "</strong>";
                }
                else if (gender == (int)UserGenderEnum.Female)
                {
                    output += "<em>" + GetString("om.contact.parentvalue") + "</em> <strong>" + GetString("general.female") + "</strong>";
                }
                else
                {
                    output += "<em>" + GetString("om.contact.parentvalue") + "</em> <strong>" + GetString("general.unknown") + "</strong>";
                }
            }
                // Datetime values
            else if (dataType == FieldDataType.DateTime)
            {
                output += "<em>" + GetString("om.contact.parentvalue") + "</em> <strong>" + ValidationHelper.GetDateTime(fieldValue, DateTimeHelper.ZERO_TIME) + "</strong>";
            }
                // Get all contacts which have same string value
            else
            {
                output += "<em>" + GetString("om.contact.parentvalue") + "</em> ";
                if (fieldName == "ContactCountryID")
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
                else if (fieldName == "ContactStateID")
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
                else if (fieldName == "ContactStatusID")
                {
                    ContactStatusInfo status = ContactStatusInfoProvider.GetContactStatusInfo(ValidationHelper.GetInteger(fieldValue, 0));
                    if (status != null)
                    {
                        output += "<strong>" + HTMLHelper.HTMLEncode(status.ContactStatusDisplayName) + "</strong>";
                    }
                    else
                    {
                        output += GetString("general.na");
                    }
                }
                else
                {
                    output += "<strong>" + HTMLHelper.HTMLEncode(fieldValue) + "</strong>";
                }
            }
        }
        else
        {
            output += "<em>" + GetString("om.contact.parentvalue") + "</em> " + GetString("general.na");
        }
        output += "<div><em>" + GetString("om.contact.mergedvalues") + "</em></div>";

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

                // Sort contacts by full name
                var contactsView = mMergedContacts.Tables[0].DefaultView;
                contactsView.Sort = "ContactFullNameJoined";

                mMergedContacts.CaseSensitive = true;

                var value = dr[fieldName];

                if (fieldName == "ContactBirthday")
                {
                    // Get all contacts which have same ContactBirthday value
                    output += "<div><strong>" + HTMLHelper.HTMLEncode(ValidationHelper.GetDateTime(value, DateTimeHelper.ZERO_TIME).ToShortDateString()) + "</strong></div>";
                    contactsView.RowFilter = fieldName + " = '" + value + "'";
                }
                    // Display gender
                else if (fieldName == "ContactGender")
                {
                    int gender = ValidationHelper.GetInteger(value, 0);
                    if (gender == (int)UserGenderEnum.Male)
                    {
                        output += GetTooltipItem(GetString("general.male"));
                    }
                    else if (gender == (int)UserGenderEnum.Female)
                    {
                        output += GetTooltipItem(GetString("general.female"));
                    }
                    else
                    {
                        output += GetTooltipItem(GetString("general.unknown"));
                    }

                    if (String.IsNullOrEmpty(ValidationHelper.GetString(value, null)))
                    {
                        contactsView.RowFilter = fieldName + " IS NULL";
                    }
                    else
                    {
                        contactsView.RowFilter = fieldName + " = " + value;
                    }
                }
                    // Need to transform status ID to displayname
                else if (fieldName == "ContactStatusID")
                {
                    ContactStatusInfo status = ContactStatusInfoProvider.GetContactStatusInfo((int)value);
                    output += GetTooltipItem(ResHelper.LocalizeString(status.ContactStatusDisplayName));
                    contactsView.RowFilter = fieldName + " = '" + status.ContactStatusID + "'";
                }
                    // Need to transform country ID to displayname
                else if (fieldName == "ContactCountryID")
                {
                    CountryInfo country = CountryInfoProvider.GetCountryInfo((int)value);
                    output += GetTooltipItem(country.CountryDisplayName);
                    contactsView.RowFilter = fieldName + " = '" + country.CountryID + "'";
                }
                    // Need to transform state ID to displayname
                else if (fieldName == "ContactStateID")
                {
                    StateInfo state = StateInfoProvider.GetStateInfo((int)value);
                    output += GetTooltipItem(state.StateDisplayName);
                    contactsView.RowFilter = fieldName + " = '" + state.StateID + "'";
                }
                    // Other fields, process based on field type
                else
                {
                    output += GetTooltipItem(ValidationHelper.GetString(value, null));
                    contactsView.RowFilter = fieldName + " = '" + ContactHelper.EscapeString(ValidationHelper.GetString(value, "")) + "'";
                }

                // Display all contact 
                DataTable contacts = contactsView.ToTable(false, "ContactFullNameJoined");
                foreach (DataRow contactRow in contacts.Rows)
                {
                    output += "<div>&nbsp;-&nbsp;" + HTMLHelper.HTMLEncode(ValidationHelper.GetString(contactRow["ContactFullNameJoined"], "").Trim()) + "</div>";
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
    /// Initializes last name with tooltip.
    /// </summary>
    private void InitLastName()
    {
        // Get grouped dataset
        DataTable dt = SortGroupContactsByColumn("ContactLastName ASC", "ContactLastName NOT LIKE '' AND ContactLastName NOT LIKE '" + ContactHelper.MERGED + "%'", "ContactLastName");

        // Bind control with data
        cmbContactLastName.DropDownList.DataSource = dt;
        cmbContactLastName.DropDownList.DataTextField = "ContactLastName";
        cmbContactLastName.DropDownList.DataValueField = "ContactLastName";
        cmbContactLastName.DataBind();

        // Insert parent value to first position
        if (!String.IsNullOrEmpty(mParentContact.ContactLastName) && !mParentContact.ContactLastName.StartsWithCSafe(ContactHelper.MERGED) && !cmbContactLastName.DropDownList.Items.Contains(new ListItem(mParentContact.ContactLastName)))
        {
            cmbContactLastName.DropDownList.Items.Insert(0, mParentContact.ContactLastName);
        }

        cmbContactLastName.SelectedValue = mParentContact.ContactLastName;

        foreach (ListItem item in cmbContactLastName.DropDownList.Items)
        {
            item.Text = HTMLHelper.HTMLEncode(item.Text);
        }

        // Display appropriate icon
        DisplayTooltip(imgContactLastName, SortGroupContactsByColumn("ContactLastName ASC", "ContactLastName NOT LIKE ''", "ContactLastName"), "ContactLastName", mParentContact.ContactLastName, FieldDataType.Unknown);
    }


    /// <summary>
    /// Initializes country selector with tooltip.
    /// </summary>
    private void InitCountrySelector()
    {
        // Init countries
        DataTable table = SortGroupContactsByColumn("ContactCountryID", "ContactCountryID > 0", "ContactCountryID");
        countrySelector.CountryID = PreselectCountryState(mParentContact.ContactCountryID, table);
        bool display = DisplayTooltip(imgContactCountry, table, "ContactCountryID", mParentContact.ContactCountryID.ToString(), FieldDataType.Unknown);

        // Init states
        table = SortGroupContactsByColumn("ContactStateID", "ContactStateID > 0", "ContactStateID");
        countrySelector.StateID = PreselectCountryState(mParentContact.ContactStateID, table);
        display &= DisplayTooltip(imgContactState, table, "ContactStateID", mParentContact.ContactStateID.ToString(), FieldDataType.Unknown);
    }


    /// <summary>
    /// Initializes calendar with birthday data.
    /// </summary>
    private void InitBirthday()
    {
        DataTable table = SortGroupContactsByColumn("ContactBirthday", "ContactBirthday IS NOT NULL", "ContactBirthday");

        // Preselect calendar with birthday data from parent         
        if (mParentContact.ContactBirthday != DateTime.MinValue)
        {
            calendarControl.Value = mParentContact.ContactBirthday;
        }
            // Preselect calendar with birthday data from merged contacts only if single value exists in merged items
        else if ((table.Rows != null) && (table.Rows.Count > 0))
        {
            calendarControl.Value = ValidationHelper.GetDateTime(table.Rows[0][0], DateTimeHelper.ZERO_TIME);
        }

        DisplayTooltip(imgContactBirthday, table, "ContactBirthday", mParentContact.ContactBirthday == DateTimeHelper.ZERO_TIME ? null : mParentContact.ContactBirthday.ToString(), FieldDataType.Unknown);
    }


    /// <summary>
    /// Initializes gender selector.
    /// </summary>
    private void InitGender()
    {
        DataTable table = SortGroupContactsByColumn("ContactGender", null, "ContactGender");

        // Preselect gender with data from parent
        if (mParentContact.ContactGender != (int)UserGenderEnum.Unknown)
        {
            genderSelector.Value = mParentContact.ContactGender;
        }
            // Preselect gender with data from merged contacts only if single value exists in merged items
        else if ((table.Rows != null) && (table.Rows.Count > 0))
        {
            genderSelector.Value = ValidationHelper.GetInteger(table.Rows[0][0], 0);
        }

        DisplayTooltip(imgContactGender, table, "ContactGender", mParentContact.ContactGender == (int)UserGenderEnum.Unknown ? null : mParentContact.ContactGender.ToString(), FieldDataType.Unknown);
    }


    /// <summary>
    /// Initializes contact status selector.
    /// </summary>
    private void InitContactStatus()
    {
        DataTable table = SortGroupContactsByColumn("ContactStatusID", "ContactStatusID > 0", "ContactStatusID");

        // Preselect contact status with data from parent
        if (mParentContact.ContactStatusID > 0)
        {
            contactStatusSelector.Value = mParentContact.ContactStatusID;
        }
            // Preselect contact status with data from merged contacts only if single value exists in merged items
        else if ((table.Rows != null) && (table.Rows.Count > 0))
        {
            contactStatusSelector.Value = ValidationHelper.GetInteger(table.Rows[0][0], 0);
        }

        DisplayTooltip(imgContactStatus, table, "ContactStatusID", mParentContact.ContactStatusID.ToString(), FieldDataType.Unknown);
    }


    /// <summary>
    /// Initializes monitored checkbox with tooltip.
    /// </summary>
    private void InitMonitored()
    {
        DataTable tableTrueValues = SortGroupContactsByColumn("ContactMonitored", "ContactMonitored = 1", "ContactMonitored");
        DataTable tableFalseValues = SortGroupContactsByColumn("ContactMonitored", "(ContactMonitored = 0) OR (ContactMonitored IS NULL)", "ContactMonitored");
        chkContactMonitored.Checked = mParentContact.ContactMonitored;
        DisplayTooltipForMonitored(imgContactMonitored, tableTrueValues, tableFalseValues, mParentContact.ContactMonitored);
    }


    /// <summary>
    /// Initializes campaign selector with tooltip.
    /// </summary>
    private void InitCampaign()
    {
        DataTable table = SortGroupContactsByColumn("ContactCampaign", "ContactCampaign IS NOT NULL AND ContactCampaign <> ''", "ContactCampaign");

        // Preselect contact campaign with data from parent
        if (mParentContact.ContactCampaign != String.Empty)
        {
            cCampaign.Value = mParentContact.ContactCampaign;
        }
            // Preselect contact campaign with data from merged contacts only if single value exists in merged items
        else if ((table.Rows != null) && (table.Rows.Count > 0))
        {
            cCampaign.Value = ValidationHelper.GetString(table.Rows[0][0], String.Empty);
        }

        DisplayTooltip(imgContactCampaign, table, "ContactCampaign", mParentContact.ContactCampaign, FieldDataType.Text);
    }


    /// <summary>
    /// Initializes notes.
    /// </summary>
    private void InitNotes()
    {
        // Merge contact notes
        htmlNotes.Value = mParentContact.ContactNotes;
        DataTable table = SortGroupContactsByColumn(null, "ContactNotes NOT LIKE ''", "ContactNotes");

        // Preselect value only if single value exists in merged items
        if ((table.Rows != null) && (table.Rows.Count > 0))
        {
            foreach (DataRow dr in table.Rows)
            {
                htmlNotes.Value += ValidationHelper.GetString(dr[0], "");
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
        if (ContactHelper.AuthorizedModifyContact(mParentContact.ContactSiteID, true))
        {
            // Validate form
            if (ValidateForm())
            {
                // Change parent contact values
                SaveChanges();
                // Update hash table with account-contact roles
                UpdateRoles();

                // Merge contacts
                ContactHelper.Merge(mParentContact, mMergedContacts, mRoles, GetContactGroups());

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
        mParentContact.ContactFirstName = cmbContactFirstName.Text.Trim();
        mParentContact.ContactMiddleName = cmbContactMiddleName.Text.Trim();
        mParentContact.ContactLastName = cmbContactLastName.Text.Trim();
        mParentContact.ContactSalutation = cmbContactSalutation.Text.Trim();
        mParentContact.ContactTitleBefore = cmbContactTitleBefore.Text.Trim();
        mParentContact.ContactTitleAfter = cmbContactTitleAfter.Text.Trim();
        mParentContact.ContactCompanyName = cmbContactCompanyName.Text.Trim();
        mParentContact.ContactJobTitle = cmbContactJobTitle.Text.Trim();
        mParentContact.ContactAddress1 = cmbContactAddress1.Text.Trim();
        mParentContact.ContactAddress2 = cmbContactAddress2.Text.Trim();
        mParentContact.ContactCity = cmbContactCity.Text.Trim();
        mParentContact.ContactZIP = cmbContactZIP.Text.Trim();
        mParentContact.ContactStateID = countrySelector.StateID;
        mParentContact.ContactCountryID = countrySelector.CountryID;
        mParentContact.ContactMobilePhone = cmbContactMobilePhone.Text.Trim();
        mParentContact.ContactHomePhone = cmbContactHomePhone.Text.Trim();
        mParentContact.ContactBusinessPhone = cmbContactBusinessPhone.Text.Trim();
        mParentContact.ContactEmail = cmbContactEmail.Text.Trim();
        mParentContact.ContactWebSite = cmbContactWebSite.Text.Trim();
        mParentContact.ContactBirthday = ValidationHelper.GetDateTime(calendarControl.Value, DateTimeHelper.ZERO_TIME);
        mParentContact.ContactGender = genderSelector.Gender;
        mParentContact.ContactStatusID = ValidationHelper.GetInteger(contactStatusSelector.Value, 0);
        mParentContact.ContactCampaign = (string)cCampaign.Value;
        mParentContact.ContactNotes = (string)htmlNotes.Value;
        mParentContact.ContactOwnerUserID = MembershipContext.AuthenticatedUser.UserID;
        mParentContact.ContactMonitored = chkContactMonitored.Checked;

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
                mParentContact.SetValue(key, convertedValue);
            }
        }

        ContactInfoProvider.SetContactInfo(mParentContact, false);
    }


    /// <summary>
    /// Sorts, filters and groups contacts by specified rules
    /// </summary>
    /// <param name="sortRule">Sorting rule by specified column</param>
    /// <param name="rowFilter">Filtering rule by specified column</param>
    /// <param name="groupByColumn">Grouping by specified column</param>
    /// <returns>Returns sorted DataTable</returns>
    private DataTable SortGroupContactsByColumn(string sortRule, string rowFilter, string groupByColumn)
    {
        mMergedContacts.Tables[0].DefaultView.Sort = sortRule;
        mMergedContacts.Tables[0].DefaultView.RowFilter = rowFilter;
        return mMergedContacts.Tables[0].DefaultView.ToTable(true, groupByColumn);
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
        else if ((table.Rows != null) && (table.Rows.Count >= 1))
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
        string errorMessage = new Validator().NotEmpty(cmbContactLastName.Text.Trim(), GetString("om.contact.enterlastname")).Result;
        if (!String.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
            return false;
        }

        // Validates birthday
        if (!calendarControl.IsValid())
        {
            ShowError(String.Format(GetString("om.contact.enterbirthday"), DateTime.Now.ToString("d")));

            return false;
        }

        // Validates email
        if (!String.IsNullOrEmpty(cmbContactEmail.Text) && !ValidationHelper.IsEmail(cmbContactEmail.Text))
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


    protected void Page_PreRender(object sender, EventArgs e)
    {
        imgContactState.Visible = lblState.Visible = countrySelector.StateIsVisible();
    }

    #endregion
}