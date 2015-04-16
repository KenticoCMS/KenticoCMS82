using System;
using System.Collections;
using System.Data;

using CMS.DataCom;
using CMS.EventLog;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.UIControls;
using CMS.Membership;

/// <summary>
/// A dialog page where user can search Data.com for a contact, view contact details and select it.
/// </summary>
public partial class CMSModules_ContactManagement_Pages_Tools_DataCom_SelectContact : CMSDataComDialogPage
{
    #region "Constants"

    /// <summary>
    /// Maximum number of pages that the grid control can display.
    /// </summary>
    private readonly int MAX_PAGE_COUNT = 5;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the Data.com contact identifier that was selected in a datagrid.
    /// </summary>
    private long CurrentContactId
    {
        get
        {
            return ValidationHelper.GetLong(ViewState["CurrentContactId"], 0);
        }
        set
        {
            ViewState["CurrentContactId"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the selected Data.com contact.
    /// </summary>
    private Contact CurrentContact
    {
        get
        {
            return ViewState["CurrentContact"] as Contact;
        }
        set
        {
            ViewState["CurrentContact"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the current contact filter.
    /// </summary>
    private ContactFilter CurrentContactFilter
    {
        get
        {
            return ViewState["CurrentFilter"] as ContactFilter;
        }
        set
        {
            ViewState["CurrentFilter"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the filter parameter.
    /// </summary>
    private ContactFilter FilterParameter { get; set; }


    /// <summary>
    /// Gets or sets the site identifier parameter.
    /// </summary>
    private int SiteIdentifierParameter { get; set; }

    #endregion


    #region "Life cycle methods"

    protected override void OnInit(EventArgs e)
    {
        SetSaveJavascript("DataCom_ConfirmSelection(); return false;");
        base.OnInit(e);
        ScriptHelper.RegisterWOpenerScript(Page);
        ScriptHelper.RegisterJQuery(Page);
        ContactGrid.OnDataReload += ContactGrid_OnDataReload;
        ContactGrid.OnAction += ContactGrid_OnAction;
        ContactFilterControl.Search += ContactFilterControl_Search;
        PageTitle.TitleText = GetString("datacom.contacts.title");
        AuthorizeRequest();
    }


    protected override void OnLoad(EventArgs e)
    {
        try
        {
            RestoreParameters();
            if (!RequestHelper.IsPostBack())
            {
                CurrentContactFilter = FilterParameter;
                ContactFilterControl.Filter = FilterParameter;
            }
        }
        catch (Exception exception)
        {
            HandleException(exception);
        }
        base.OnLoad(e);
    }


    protected void ContactFilterControl_Search(object sender, EventArgs e)
    {
        CurrentContactFilter = ContactFilterControl.Filter;
        ContactGrid.Pager.Reset();
    }


    protected void ContactGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "select")
        {
            CurrentContactId = ValidationHelper.GetLong(actionArgument, 0);
        }
    }


    protected DataSet ContactGrid_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        try
        {
            ICredentialProvider credentialProvider = new UserCredentialProvider(MembershipContext.AuthenticatedUser);

            DataComClient client = DataComHelper.CreateClient();
            IContactProvider provider = DataComHelper.CreateContactProvider(client, credentialProvider.GetCredential());
            ContactSearchResults response = provider.SearchContacts(CurrentContactFilter, currentOffset / currentPageSize, currentPageSize);
            DataTable table = new DataTable("Contacts");
            table.Columns.Add("ContactId", typeof(long));
            table.Columns.Add("FirstName", typeof(string));
            table.Columns.Add("LastName", typeof(string));
            table.Columns.Add("CompanyName", typeof(string));
            foreach (Contact contact in response.Contacts)
            {
                DataRow row = table.NewRow();
                row["ContactId"] = contact.ContactId;
                row["FirstName"] = contact.FirstName;
                row["LastName"] = contact.LastName;
                row["CompanyName"] = contact.CompanyName;
                table.Rows.Add(row);
                if (contact.ContactId == CurrentContactId)
                {
                    CurrentContact = contact;
                }
            }
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(table);
            int maxHitCount = currentPageSize * MAX_PAGE_COUNT;
            int hitCount = (int)response.TotalHits;
            if (hitCount > maxHitCount)
            {
                hitCount = maxHitCount;
                ShowWarning(GetString("datacom.toomanycontacts"), null, null);
            }
            totalRecords = hitCount;
            return dataSet;
        }
        catch (Exception exception)
        {
            HandleException(exception);
        }
        totalRecords = 0;

        return null;
    }


    protected override void OnPreRender(EventArgs e)
    {
        try
        {
            ContactGrid.ReloadData();
            ContactControl.Contact = CurrentContact;
            if (CurrentContact != null)
            {
                JsonSerializer serializer = new JsonSerializer();
                ContactHiddenField.Value = serializer.Serialize(CurrentContact);
                headTitle.Visible = true;
            }
            else
            {
                EmptySelectionControl.Visible = true;
            }
        }
        catch (Exception exception)
        {
            HandleException(exception);
        }
        base.OnPreRender(e);
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Restores parameters that were passed on to this dialog.
    /// </summary>
    private void RestoreParameters()
    {
        // Validate parameters
        if (!QueryHelper.ValidateHash("hash"))
        {
            throw new Exception("[DataComSelectContactPage.RestoreParameters]: Invalid query hash.");
        }
        Hashtable parameters = WindowHelper.GetItem(QueryHelper.GetString("pid", null)) as Hashtable;
        if (parameters == null)
        {
            throw new Exception("[DataComSelectContactPage.RestoreParameters]: The dialog page parameters are missing, the session might have been lost.");
        }

        // Restore filter
        string content = parameters["Filter"] as string;
        if (String.IsNullOrEmpty(content))
        {
            FilterParameter = new ContactFilter();
        }
        else
        {
            JsonSerializer serializer = new JsonSerializer();
            FilterParameter = serializer.Unserialize<ContactFilter>(content);
        }

        // Restore site identifier
        SiteIdentifierParameter = ValidationHelper.GetInteger(parameters["SiteID"], 0);
    }


    /// <summary>
    /// Displays an error message and logs the specified exception to the event log.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    private void HandleException(Exception exception)
    {
        ErrorSummary.Report(exception);
        EventLogProvider.LogException("Data.com Connector", "SelectContactPage", exception);
    }

    #endregion
}