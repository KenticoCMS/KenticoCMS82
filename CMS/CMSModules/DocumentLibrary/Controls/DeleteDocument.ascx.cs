using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DocumentEngine;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_DocumentLibrary_Controls_DeleteDocument : CMSUserControl
{
    #region "Variables"

    private TreeNode mDeletedNode = null;
    private TreeProvider mTreeProvider = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Identifier of document to be deleted.
    /// </summary>
    public int DeletedNodeID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["DeletedNodeID"], 0);
        }
        set
        {
            ViewState["DeletedNodeID"] = value;
        }
    }


    /// <summary>
    /// Culture of document to delete.
    /// </summary>
    public string DeletedDocumentCulture
    {
        get
        {
            return ValidationHelper.GetString(ViewState["DeletedDocumentCulture"], string.Empty);
        }
        set
        {
            ViewState["DeletedDocumentCulture"] = value;
        }
    }


    /// <summary>
    /// Document to be deleted.
    /// </summary>
    private TreeNode DeletedNode
    {
        get
        {
            if (DeletedNodeID == 0)
            {
                return null;
            }
            return mDeletedNode ?? (mDeletedNode = DocumentHelper.GetDocument(DeletedNodeID, DeletedDocumentCulture, TreeProvider));
        }
    }


    /// <summary>
    /// Tree provider instance.
    /// </summary>
    public TreeProvider TreeProvider
    {
        get
        {
            return mTreeProvider ?? (mTreeProvider = new TreeProvider(MembershipContext.AuthenticatedUser));
        }
    }


    /// <summary>
    /// Gets cancel button.
    /// </summary>
    public Button CancelButton
    {
        get
        {
            if (btnCancel == null)
            {
                EnsureChildControls();
            }
            return btnCancel;
        }
    }


    /// <summary>
    /// Gets submit button.
    /// </summary>
    public Button SubmitButton
    {
        get
        {
            if (btnDelete == null)
            {
                EnsureChildControls();
            }
            return btnDelete;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            ReloadData();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Render the styles link in live site mode
        if (Visible && PortalContext.ViewMode.IsLiveSite())
        {
            CSSHelper.RegisterDesignMode(Page);
        }
        bool infoMessageVisible = !string.IsNullOrEmpty(lblInfo.Text);
        bool errorMessageVisible = !string.IsNullOrEmpty(lblError.Text);
        lblInfo.Visible = infoMessageVisible;
        lblError.Visible = errorMessageVisible;
        plcMessage.Visible = infoMessageVisible || errorMessageVisible;
    }

    #endregion


    #region "Methods"

    public void ReloadData()
    {
        ReloadData(false);
    }


    public void ReloadData(bool force)
    {
        if (DeletedNode != null)
        {
            if (!RequestHelper.IsPostBack() || force)
            {
                plcConfirmation.Visible = true;
            }
            // Display confirmation
            lblConfirmation.Text = string.Format(GetString("contentdelete.questionspecific"), DeletedNode.NodeName);

            // Set visibility of 'delete all cultures' checkbox
            string currentSiteName = SiteContext.CurrentSiteName;
            chkAllCultures.Visible = CultureSiteInfoProvider.IsSiteMultilingual(currentSiteName);

            if (MembershipContext.AuthenticatedUser.UserHasAllowedCultures)
            {
                DataSet siteCultures = CultureSiteInfoProvider.GetSiteCultures(currentSiteName);
                foreach (DataRow culture in siteCultures.Tables[0].Rows)
                {
                    string cultureCode = ValidationHelper.GetString(DataHelper.GetDataRowValue(culture, "CultureCode"), string.Empty);
                    if (!MembershipContext.AuthenticatedUser.IsCultureAllowed(cultureCode, currentSiteName))
                    {
                        chkAllCultures.Visible = false;
                        break;
                    }
                }
            }
        }

        Visible = (DeletedNode != null);
        btnCancel.Text = GetString("general.cancel");
    }

    #endregion


    #region "Button handling"

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (DeletedNode != null)
        {
            try
            {
                if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(DeletedNode, NodePermissionsEnum.Delete) == AuthorizationResultEnum.Allowed)
                {
                    DocumentHelper.DeleteDocument(DeletedNode, TreeProvider, chkAllCultures.Checked, false, false);
                    lblInfo.Text = GetString("contentrequest.deleteok");
                }
                else
                {
                    lblError.Text = GetString("accessdenied.notallowedtodeletedocument");
                }
            }
            catch (Exception ex)
            {
                lblError.Text = GetString("contentrequest.deletefailed") + " : " + ex.Message;
            }
            finally
            {
                plcConfirmation.Visible = false;
                plcMessage.Visible = !string.IsNullOrEmpty(lblInfo.Text) || !string.IsNullOrEmpty(lblError.Text);
                btnDelete.Visible = false;
                btnCancel.Text = GetString("general.close");
            }
        }
    }

    #endregion
}