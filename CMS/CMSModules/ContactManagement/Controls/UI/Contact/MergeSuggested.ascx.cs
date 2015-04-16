using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_ContactManagement_Controls_UI_Contact_MergeSuggested : CMSAdminListControl
{
    #region "Variables"

    protected int mSiteId = -1;
    protected ContactInfo ci;

    /// <summary>
    /// URL of collision dialog.
    /// </summary>
    private const string MERGE_DIALOG = "~/CMSModules/ContactManagement/Pages/Tools/Contact/CollisionDialog.aspx";

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
            filter.IsLiveSite = value;
            plcMess.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets the site id.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteId;
        }
        set
        {
            mSiteId = value;
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

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get edited object
        if (Contact != null)
        {
            SiteID = ci.ContactSiteID;

            gridElem.WhereCondition = filter.GetWhereCondition();
            gridElem.ZeroRowsText = GetString("om.contact.nocontacts");
            gridElem.OnBeforeDataReload += new OnBeforeDataReload(gridElem_OnBeforeDataReload);
            gridElem.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridElem_OnExternalDataBound);
            btnMergeSelected.Click += new EventHandler(btnMerge_Click);
            btnMergeAll.Click += new EventHandler(btnMergeAll_Click);
        }
        else
        {
            StopProcessing = true;
            Visible = false;
        }
    }


    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        if (sourceName == "birthday")
        {
            return ((DateTime)parameter).ToShortDateString();
        }
        return null;
    }


    private void gridElem_OnBeforeDataReload()
    {
        // Hide email column
        gridElem.NamedColumns["email"].Visible = filter.EmailChecked;
        gridElem.NamedColumns["sitename"].Visible = ((filter.SelectedSiteID < 0) && (filter.SelectedSiteID != UniSelector.US_GLOBAL_RECORD));

        // Hide phone columns
        if (!filter.PhoneChecked)
        {
            gridElem.NamedColumns["mobilephone"].Visible
                = gridElem.NamedColumns["homephone"].Visible
                  = gridElem.NamedColumns["businessphone"].Visible = false;
        }

        // Hide birthday column
        gridElem.NamedColumns["birthday"].Visible = filter.BirthdayChecked;

        // Hide address columns
        if (!filter.PostAddressChecked)
        {
            gridElem.NamedColumns["address1"].Visible
                = gridElem.NamedColumns["address2"].Visible
                  = gridElem.NamedColumns["city"].Visible
                    = gridElem.NamedColumns["zip"].Visible = false;
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        pnlFooter.Visible = !DataHelper.DataSourceIsEmpty(gridElem.GridView.DataSource);
    }


    /// <summary>
    /// Button merge click.
    /// </summary>
    private void btnMerge_Click(object sender, EventArgs e)
    {
        if (ContactHelper.AuthorizedModifyContact(SiteID, true))
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
        if (ContactHelper.AuthorizedModifyContact(SiteID, true))
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
        Hashtable parameters = new Hashtable();
        DataSet ds;

        if (mergeAll)
        {
            ds = new ContactListInfo().Generalized.GetData(null, gridElem.WhereCondition, null, -1, null, false);
        }
        else
        {
            ds = new ContactListInfo().Generalized.GetData(null, SqlHelper.GetWhereCondition("ContactID", gridElem.SelectedItems), null, -1, null, false);
        }


        parameters["MergedContacts"] = ds;
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
        script.Append(@"modalDialog('" + ResolveUrl(url) + @"', 'mergeDialog', 700, 700, null, null, true);");

        ScriptHelper.RegisterStartupScript(this, typeof(string), "MergeDialog" + ClientID, ScriptHelper.GetScript(script.ToString()));
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshPageScript", ScriptHelper.GetScript("function RefreshPage() { window.location.replace('" + URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "saved", "true") + "'); }"));
    }

    #endregion
}