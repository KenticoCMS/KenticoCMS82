using System;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

using CMS.Base;
using CMS.Core;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.DataEngine;


public partial class CMSModules_ContactManagement_Controls_UI_Contact_MergeSplit : CMSAdminListControl, ICallbackEventHandler
{
    #region "Constants"

    private CMSModules_ContactManagement_Controls_UI_Contact_Filter filter = null;

    #endregion


    #region "Variables"

    private ContactInfo ci;
    private Hashtable mParameters;

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


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
            plcMess.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Current contact site ID.
    /// </summary>
    public ContactInfo Contact
    {
        get
        {
            if (ci == null)
            {
                if (UIContext.EditedObject != null)
                {
                    ci = (ContactInfo)UIContext.EditedObject;
                }
            }
            return ci;
        }
    }


    /// <summary>
    /// True if "select all children" should be visible.
    /// </summary>
    public bool ShowChildrenOption
    {
        get;
        set;
    }


    /// <summary>
    /// Dialog control identifier.
    /// </summary>
    private string Identifier
    {
        get
        {
            string identifier = hdnValue.Value;
            if (string.IsNullOrEmpty(identifier))
            {
                identifier = Guid.NewGuid().ToString();
                hdnValue.Value = identifier;
            }

            return identifier;
        }
    }


    /// <summary>
    /// Gets or sets the callback argument.
    /// </summary>
    private string CallbackArgument
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        gridElem.OnFilterFieldCreated += gridElem_OnFilterFieldCreated;
        gridElem.LoadGridDefinition();
        base.OnInit(e);
    }


    void gridElem_OnFilterFieldCreated(string columnName, UniGridFilterField filterDefinition)
    {
        filter = filterDefinition.ValueControl as CMSModules_ContactManagement_Controls_UI_Contact_Filter;
        if (filter != null)
        {
            filter.HideMergedFilter = true;
            filter.IsLiveSite = IsLiveSite;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // Current contact is global object
        if (Contact.ContactSiteID == 0)
        {
            filter.SiteID = SiteContext.CurrentSiteID;
            // Display site selector in site manager
            if (ContactHelper.IsSiteManager)
            {
                filter.SiteID = UniSelector.US_GLOBAL_RECORD;
                filter.DisplaySiteSelector = true;
            }
            // Display 'site or global' selector in CMS desk for global objects
            else if (ContactHelper.AuthorizedReadContact(SiteContext.CurrentSiteID, false) && ContactHelper.AuthorizedModifyContact(SiteContext.CurrentSiteID, false))
            {
                filter.DisplayGlobalOrSiteSelector = true;
            }
        }
        else
        {
            filter.SiteID = Contact.ContactSiteID;
        }
        filter.ShowGlobalStatuses =
                ConfigurationHelper.AuthorizedReadConfiguration(UniSelector.US_GLOBAL_RECORD, false) &&
                (SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSCMGlobalConfiguration") || ContactHelper.IsSiteManager);

        filter.ShowChildren = ShowChildrenOption;

        string where = String.Empty;

        if (!filter.ChildrenSelected)
        {
            // Display only direct children ("first level")
            if (Contact.ContactSiteID == 0)
            {
                where = SqlHelper.AddWhereCondition(where, "ContactMergedWithContactID IS NULL AND ContactGlobalContactID = " + Contact.ContactID);
            }
            else
            {
                where = SqlHelper.AddWhereCondition(where, "ContactMergedWithContactID = " + Contact.ContactID);
            }
        }
        else
        {
            // Get children for site contact
            where = SqlHelper.AddWhereCondition(where, "ContactID IN (SELECT * FROM Func_OM_Contact_GetChildren(" + Contact.ContactID + ", 0))");
        }
        gridElem.WhereCondition = where;
        gridElem.ZeroRowsText = GetString("om.contact.nocontacts");
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        btnSplit.Click += btnSplit_Click;

        // Register JS scripts
        RegisterScripts();
    }


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "edit":
                var btn = (CMSGridActionButton)sender;
                // Ensure accountID parameter value;
                var objectID = ValidationHelper.GetInteger(btn.CommandArgument, 0);
                // Contact detail URL
                string contactURL = UIContextHelper.GetElementDialogUrl(ModuleName.ONLINEMARKETING, "EditContact", objectID, "isSiteManager=" + ContactHelper.IsSiteManager);
                // Add modal dialog script to onClick action
                btn.OnClientClick = ScriptHelper.GetModalDialogScript(contactURL, "ContactDetail");
                break;
        }

        return null;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        pnlFooter.Visible = !DataHelper.DataSourceIsEmpty(gridElem.GridView.DataSource);
        gridElem.NamedColumns["sitename"].Visible = ((filter.SelectedSiteID < 0) && (filter.SelectedSiteID != UniSelector.US_GLOBAL_RECORD));
        gridElem.NamedColumns["mergedwhen"].Visible = Contact.ContactSiteID > 0;
    }


    private void btnSplit_Click(object sender, EventArgs e)
    {
        if (ContactHelper.AuthorizedModifyContact(Contact.ContactSiteID, true))
        {
            var items = gridElem.SelectedItems;
            if (items.Count > 0)
            {
                var contacts = ContactInfoProvider.GetContacts().WhereIn("ContactID", items.Select(c => c.ToInteger(0)).ToList());

                ContactHelper.SplitContacts(Contact, contacts.ToList(), chkCopyMissingFields.Checked, chkCopyActivities.Checked, chkRemoveAccounts.Checked);
                gridElem.ReloadData();
                gridElem.ResetSelection();
                ShowConfirmation(GetString("om.contact.splitting"));
                pnlUpdate.Update();
            }
            else
            {
                ShowError(GetString("om.contact.selectcontactssplit"));
            }
        }
    }


    /// <summary>
    /// Registers JS.
    /// </summary>
    private void RegisterScripts()
    {
        ScriptHelper.RegisterDialogScript(Page);
        StringBuilder script = new StringBuilder();

        // Register script to open dialogs for role selection and for contact editing
        script.Append(@"var dialogParams_" + ClientID + @" = '';");

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "Actions", ScriptHelper.GetScript(script.ToString()));
    }

    #endregion


    #region "ICallbackEventHandler Members"

    /// <summary>
    /// Gets callback result.
    /// </summary>
    public string GetCallbackResult()
    {
        string queryString = string.Empty;

        if (!string.IsNullOrEmpty(CallbackArgument))
        {
            // Prepare parameters
            mParameters = new Hashtable();
            mParameters["accountcontactid"] = CallbackArgument;
            mParameters["allownone"] = true;

            WindowHelper.Add(Identifier, mParameters);

            queryString = "?params=" + Identifier;
            queryString = URLHelper.AddParameterToUrl(queryString, "hash", QueryHelper.GetHash(queryString));
        }

        return queryString;
    }


    /// <summary>
    /// Raise callback method.
    /// </summary>
    public void RaiseCallbackEvent(string eventArgument)
    {
        CallbackArgument = eventArgument;
    }

    #endregion
}