using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.DataCom;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.ExtendedControls.ActionsConfig;
using CMS.FormEngine;
using CMS.FormControls;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.UIControls;
using CMS.ExtendedControls;


/// <summary>
/// Provides UI elements to display and edit CMS contact and possibly update it from the specified Data.com contact.
/// </summary>
[EditedObject(ContactInfo.OBJECT_TYPE, "contactId")]
[Security(Resource = ModuleName.ONLINEMARKETING, UIElements = "EditContact;ContactDataCom")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Tab_DataCom : CMSDataComPage, ICallbackEventHandler
{
    #region "Variables"

    /// <summary>
    /// New instance of credential provider.
    /// </summary>
    private readonly ICredentialProvider credentialProvider = new UserCredentialProvider(MembershipContext.AuthenticatedUser);

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the Data.com contact to display side-by-side with the CMS contact.
    /// </summary>
    public Contact Contact
    {
        get
        {
            return ViewState["DataComContact"] as Contact;
        }
        set
        {
            ViewState["DataComContact"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the suggested Data.com contact filter when the Data.com search didn't return an exact match.
    /// </summary>
    public ContactFilter Filter
    {
        get;
        set;
    }


    /// <summary>
    /// Gets the site identifier of the edited contact.
    /// </summary>
    public int ContactSiteID
    {
        get
        {
            return ContactHelper.ObjectSiteID(EditedObject);
        }
    }


    /// <summary>
    /// Gets the unique identifier to pass the parameters on to the Data.com contact selection dialog window.
    /// </summary>
    private string ParametersIdentifier
    {
        get
        {
            string identifier = ViewState["PID"] as string;
            if (identifier == null)
            {
                identifier = Guid.NewGuid().ToString("N");
                ViewState["PID"] = identifier;
            }

            return identifier;
        }
    }


    /// <summary>
    /// Gets the unique identifier to pass the parameters on to the Data.com Buy contact dialog window.
    /// </summary>
    private string BuyParametersIdentifier
    {
        get
        {
            string identifier = ViewState["BuyPID"] as string;
            if (identifier == null)
            {
                identifier = Guid.NewGuid().ToString("N");
                ViewState["BuyPID"] = identifier;
            }

            return identifier;
        }
    }


    /// <summary>
    /// Returns redirect url to login page.
    /// </summary>
    private string LoginPageUrl
    {
        get
        {
            var url = "~/CMSModules/ContactManagement/Pages/Tools/DataCom/Login.aspx";
            url = URLHelper.AddParameterToUrl(url, "returnurl", HttpUtility.UrlEncode(RequestContext.CurrentURL));
            return url;
        }
    }

    #endregion


    #region "Life cycle methods"

    /// <summary>
    /// PreInit event handler
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        RequiresDialog = false;
        base.OnPreInit(e);
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        ContactInfo contact = EditedObject as ContactInfo;
        if (contact == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }
        
        AuthorizeReadRequest(contact);

        ScriptHelper.RegisterDialogScript(Page);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Do not check login if it's a CallBack - search button was pressed (see CreateSearchActionClientScript method)
        if (!RequestHelper.IsCallback())
        {
            bool validCredential = false;
            try
            {
                validCredential = CheckCredential();
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return;
            }

            if (!validCredential)
            {
                URLHelper.Redirect(LoginPageUrl);
            }
        }

        try
        {
            if (!String.IsNullOrEmpty(ContactHiddenField.Value))
            {
                JsonSerializer serializer = new JsonSerializer();
                Contact freshContact = serializer.Unserialize<Contact>(ContactHiddenField.Value);
                if (Contact == null)
                {
                    ContactForm.MergeHint = true;
                }
                else
                {
                    if (Contact.ContactId != freshContact.ContactId)
                    {
                        ContactForm.MergeHint = true;
                    }
                    else if (String.IsNullOrEmpty(Contact.Phone) && String.IsNullOrEmpty(Contact.Email) && (!String.IsNullOrEmpty(freshContact.Phone) || !String.IsNullOrEmpty(freshContact.Email)))
                    {
                        ContactForm.MergeHint = true;
                        ContactForm.MergeHintAttributes = new string[] { "Phone", "Email" };
                    }
                }
                Contact = freshContact;
            }
            ContactInfo contactInfo = EditedObject as ContactInfo;
            ContactIdentity identity = DataComHelper.CreateContactIdentity(contactInfo);
            Filter = identity.CreateFilter();
            // Do not search for contact if it's a CallBack - search button was pressed (see CreateSearchActionClientScript method)
            if (Contact == null && !RequestHelper.IsCallback())
            {
                DataComClient client = DataComHelper.CreateClient();
                IContactProvider provider = DataComHelper.CreateContactProvider(client, credentialProvider.GetCredential());
                ContactFinder finder = DataComHelper.CreateContactFinder(provider);
                Contact match = finder.Find(identity);
                if (match != null)
                {
                    ShowInformation(GetString("datacom.contactmatch"));
                    Contact = match;
                    ContactForm.MergeHint = true;
                }
                else
                {
                    ShowInformation(GetString("datacom.nocontactmatch"));
                }
            }
            InitializeHeaderActions();
            InitializeDataComForm();
        }
        catch (Exception exception)
        {
            HandleException(exception);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        if (Contact == null)
        {
            ContactFormPanel.CssClass = "Hidden";
            HeaderAction actionSave = HeaderActions.ActionsList.SingleOrDefault(x => x.CommandName == "save");
            if (actionSave != null)
            {
                actionSave.Visible = false;
                // Make the Search button primary if the Save button is hidden 
                HeaderAction actionSearch = HeaderActions.ActionsList.SingleOrDefault(x => x.CommandName == "search");
                if (actionSearch != null)
                {
                    actionSearch.ButtonStyle = ButtonStyle.Primary;
                }
            }
        }
    }


    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerInvariant())
        {
            case "save":
                ContactInfo contact = EditedObject as ContactInfo;
                AuthorizeModifyRequest(contact);
                try
                {
                    if (ContactForm.Validate() && ContactForm.Store())
                    {
                        ContactForm.Merge();
                        BaseInfo data = ContactForm.Data as BaseInfo;
                        data.Generalized.SetObject();
                        Filter = null;
                        ShowChangesSaved();
                    }
                }
                catch (Exception exception)
                {
                    HandleException(exception);
                }
                break;

            case "logout":
                credentialProvider.SetCredential(new NetworkCredential());
                URLHelper.Redirect(LoginPageUrl);
                break;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes the form with the required dependencies and the specified CMS contact.
    /// </summary>
    protected void InitializeDataComForm()
    {
        IDataComConfiguration configuration = DataComHelper.GetConfiguration(ContactSiteID);
        ContactInfo contactInfo = EditedObject as ContactInfo;
        ContactForm.ParametersIdentifier = BuyParametersIdentifier;
        ContactForm.FormInformation = DataComHelper.GetContactFormInfo();
        ContactForm.EntityInfo = DataComHelper.GetContactEntityInfo();
        ContactForm.EntityMapping = configuration.GetContactMapping();
        ContactForm.EntityAttributeMapperFactory = DataComHelper.GetContactAttributeMapperFactory();
        ContactForm.Entity = Contact;
        ContactForm.EntityAttributeFormatter = DataComHelper.GetEntityAttributeFormatter();
        ContactForm.BuyContactEnabled = (credentialProvider.GetCredential() != null);
        ContactForm.DefaultFieldLayout = FieldLayoutEnum.ThreeColumns;
        ContactForm.DefaultFormLayout = FormLayoutEnum.SingleTable;
        ContactForm.Restore(contactInfo);
    }


    /// <summary>
    /// Adds actions to the page header.
    /// </summary>
    protected void InitializeHeaderActions()
    {
        AddHeaderActions(new HeaderAction
        {
            Text = GetString("general.apply"),
            CommandName = "save",
            RegisterShortcutScript = true
        });

        AddHeaderActions(new HeaderAction
        {
            Text = GetString("general.search"),
            OnClientClick = CreateSearchActionClientScript(),
            CommandName = "search",
            ButtonStyle = ButtonStyle.Default,
        });

        AddHeaderActions(new HeaderAction
        {
            Text = string.Format(GetString("datacom.logout"), credentialProvider.GetCredential().UserName),
            GenerateSeparatorBeforeAction = true,
            CommandName = "logout",
            OnClientClick = "return confirm('" + GetString("datacom.automation.confirmlogout") + "')",
            ButtonStyle = ButtonStyle.Default,
        });

        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }


    /// <summary>
    /// Creates a client script to open the dialog window to select a Data.com contact, and returns it.
    /// </summary>
    /// <returns>A client script to open the dialog window to select a Data.com contact.</returns>
    protected string CreateSearchActionClientScript()
    {
        string baseUrl = URLHelper.ResolveUrl("~/CMSModules/ContactManagement/Pages/Tools/DataCom/SelectContact.aspx");
        string url = String.Format("{0}?pid={1}", baseUrl, ParametersIdentifier);
        string script = String.Format("function DataCom_SelectContact (arg, context) {{ modalDialog('{0}', 'SelectContact', '980', '950', null); return false; }}", URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url)));
        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "DataCom_SelectContact", script, true);

        return String.Format("{0}; return false;", Page.ClientScript.GetCallbackEventReference(this, null, "DataCom_SelectContact", null));
    }


    /// <summary>
    /// Displays an error message and logs the specified exception to the event log.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    private void HandleException(Exception exception)
    {
        ErrorSummary.Report(exception);
        EventLogProvider.LogException("Data.com Connector", "ContactPage", exception);
    }


    /// <summary>
    /// Checks whether is user has provided credential to DataCom or not. If not, user is redirected to login page.
    /// </summary>
    /// <exception cref="Exception">Might be thrown when error occurs in communicating with Data.com</exception>
    private bool CheckCredential()
    {
        var credential = credentialProvider.GetCredential();
        var client = DataComHelper.CreateClient();
        return (credential != null && client.UserIsValid(credential));
    }

    #endregion


    #region ICallbackEventHandler Members

    string ICallbackEventHandler.GetCallbackResult()
    {
        return null;
    }


    void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
    {
        JsonSerializer serializer = new JsonSerializer();
        Hashtable parameters = new Hashtable();
        if (Filter != null)
        {
            parameters["Filter"] = serializer.Serialize(Filter);
        }
        parameters["SiteID"] = ContactSiteID;
        WindowHelper.Add(ParametersIdentifier, parameters);
    }

    #endregion
}