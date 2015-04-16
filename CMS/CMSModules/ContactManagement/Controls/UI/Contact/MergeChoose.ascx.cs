using System;
using System.Collections;
using System.Text;
using System.Web.UI;

using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;

using ContactFilter = CMSModules_ContactManagement_Controls_UI_Contact_Filter;

public partial class CMSModules_ContactManagement_Controls_UI_Contact_MergeChoose : CMSAdminListControl
{
    #region "Variables and constants"

    /// <summary>
    /// URL of collision dialog.
    /// </summary>
    private const string MERGE_DIALOG = "~/CMSModules/ContactManagement/Pages/Tools/Contact/CollisionDialog.aspx";


    private ContactInfo mContactInfo;
    private ContactFilter mFilter;

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
    /// Current contact.
    /// </summary>
    public ContactInfo Contact
    {
        get
        {
            if (mContactInfo == null)
            {
                if (UIContext.EditedObject != null)
                {
                    mContactInfo = (ContactInfo)UIContext.EditedObject;
                }
            }
            return mContactInfo;
        }
    }


    /// <summary>
    /// Modal dialog identifier.
    /// </summary>
    private Guid Identifier
    {
        get
        {
            Guid identifier;
            if (!Guid.TryParse(hdnIdentifier.Value, out identifier))
            {
                identifier = Guid.NewGuid();
                hdnIdentifier.Value = identifier.ToString();
            }

            return identifier;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        gridElem.OnFilterFieldCreated += gridElem_OnFilterFieldCreated;
        gridElem.LoadGridDefinition();
        base.OnInit(e);
    }


    private void gridElem_OnFilterFieldCreated(string columnName, UniGridFilterField filterDefinition)
    {
        mFilter = filterDefinition.ValueControl as ContactFilter;
        if (mFilter != null)
        {
            mFilter.NotMerged = true;
            mFilter.HideMergedFilter = true;
            mFilter.IsLiveSite = IsLiveSite;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (Contact != null)
        {
            // Current contact is global object
            if (Contact.ContactSiteID == 0)
            {
                mFilter.SiteID = SiteContext.CurrentSiteID;

                // Display site selector in site manager
                if (ContactHelper.IsSiteManager)
                {
                    mFilter.DisplaySiteSelector = true;
                    mFilter.SiteID = UniSelector.US_GLOBAL_RECORD;
                }

                    // Display 'site or global' selector in CMS desk for global objects
                else if (ContactHelper.AuthorizedReadContact(SiteContext.CurrentSiteID, false) && ContactHelper.AuthorizedModifyContact(SiteContext.CurrentSiteID, false))
                {
                    mFilter.DisplayGlobalOrSiteSelector = true;
                }
                mFilter.HideMergedIntoGlobal = true;
            }
            else
            {
                mFilter.SiteID = Contact.ContactSiteID;
            }
            mFilter.ShowGlobalStatuses =
                ConfigurationHelper.AuthorizedReadConfiguration(UniSelector.US_GLOBAL_RECORD, false) &&
                (SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSCMGlobalConfiguration") || ContactHelper.IsSiteManager);
            gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, "ContactID <> " + Contact.ContactID);
            gridElem.ZeroRowsText = GetString("om.contact.nocontacts");
            btnMergeSelected.Click += btnMerge_Click;
            btnMergeAll.Click += btnMergeAll_Click;

            if (Request[Page.postEventArgumentID] == "saved")
            {
                ShowConfirmation(GetString("om.contact.merging"));

                // Clear selected items
                gridElem.ResetSelection();
            }
        }
        else
        {
            StopProcessing = true;
            Visible = false;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        pnlButton.Visible = !DataHelper.DataSourceIsEmpty(gridElem.GridView.DataSource);
        gridElem.NamedColumns["sitename"].Visible = ((mFilter.SelectedSiteID < 0) && (mFilter.SelectedSiteID != UniSelector.US_GLOBAL_RECORD));
    }


    /// <summary>
    /// Button merge selected click.
    /// </summary>
    private void btnMerge_Click(object sender, EventArgs e)
    {
        if (ContactHelper.AuthorizedModifyContact(Contact.ContactSiteID, true))
        {
            if (gridElem.SelectedItems.Count > 0)
            {
                SetDialogParameters(false);
                OpenWindow();
            }
            else
            {
                ShowError(GetString("om.contact.selectcontacts"));
            }
        }
    }


    /// <summary>
    /// Button merge all click.
    /// </summary>
    private void btnMergeAll_Click(object sender, EventArgs e)
    {
        if (ContactHelper.AuthorizedModifyContact(Contact.ContactSiteID, true))
        {
            if (!DataHelper.DataSourceIsEmpty(gridElem.GridView.DataSource))
            {
                SetDialogParameters(true);
                OpenWindow();
            }
            else
            {
                ShowError(GetString("om.contact.selectcontacts"));
            }
        }
    }


    /// <summary>
    /// Sets the dialog parameters to the context.
    /// </summary>
    private void SetDialogParameters(bool mergeAll)
    {
        string condition = mergeAll ? SqlHelper.AddWhereCondition(gridElem.WhereCondition, gridElem.WhereClause) : SqlHelper.GetWhereCondition("ContactID", gridElem.SelectedItems);

        Hashtable parameters = new Hashtable();
        parameters["MergedContacts"] = new ContactListInfo().Generalized.GetData(null, condition, null, -1, null, false);
        parameters["ParentContact"] = Contact;
        parameters["issitemanager"] = ContactHelper.IsSiteManager;

        WindowHelper.Add(Identifier.ToString(), parameters);
    }


    /// <summary>
    /// Registers JS for opening window.
    /// </summary>
    private void OpenWindow()
    {
        ScriptHelper.RegisterDialogScript(Page);

        string url = MERGE_DIALOG + "?params=" + Identifier;
        url += "&hash=" + QueryHelper.GetHash(url, false);

        StringBuilder script = new StringBuilder();
        script.Append(@"modalDialog('" + ResolveUrl(url) + @"', 'mergeDialog', 850, 700, null, null, true);");

        ScriptHelper.RegisterStartupScript(this, typeof (string), "MergeDialog" + ClientID, ScriptHelper.GetScript(script.ToString()));
        ScriptHelper.RegisterClientScriptBlock(this, typeof (string), "RefreshPageScript", ScriptHelper.GetScript("function RefreshPage() {  __doPostBack('" + ClientID + @"', 'saved'); }"));
    }

    #endregion
}