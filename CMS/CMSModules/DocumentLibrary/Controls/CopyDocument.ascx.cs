using System;
using System.Web.UI.WebControls;

using CMS.FormEngine;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.Base;
using CMS.DocumentEngine;
using CMS.WorkflowEngine;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;
using CMS.DataEngine;

public partial class CMSModules_DocumentLibrary_Controls_CopyDocument : ContentActionsControl
{
    #region "Variables"

    private TreeNode mCopiedNode = null;
    private TreeNode mTargetNode = null;
    private DataClassInfo mDataClassInfo = null;
    private FormInfo mFormInfo = null;
    private FormFieldInfo mNodeNameSourceInfo = null;
    private string mNodeNameSource = null;
    private const string DOCUMENT_NAME = "DocumentName";

    #endregion


    #region "Properties"

    /// <summary>
    /// Identifier of document under which the document should be copied.
    /// </summary>
    public int TargetNodeID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["TargetNodeID"], 0);
        }
        set
        {
            ViewState["TargetNodeID"] = value;
        }
    }


    /// <summary>
    /// Parent document for copied node.
    /// </summary>
    private TreeNode TargetNode
    {
        get
        {
            if (TargetNodeID == 0)
            {
                return null;
            }
            return mTargetNode ?? (mTargetNode = TreeProvider.SelectSingleNode(TargetNodeID));
        }
    }


    /// <summary>
    /// Identifier of node to copy.
    /// </summary>
    public int CopiedNodeID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["CopiedNodeID"], 0);
        }
        set
        {
            ViewState["CopiedNodeID"] = value;
        }
    }


    /// <summary>
    /// Culture of document to copy.
    /// </summary>
    public string CopiedDocumentCulture
    {
        get
        {
            return ValidationHelper.GetString(ViewState["CopiedDocumentCulture"], string.Empty);
        }
        set
        {
            ViewState["CopiedDocumentCulture"] = value;
        }
    }


    /// <summary>
    /// Node to copy.
    /// </summary>
    private TreeNode CopiedNode
    {
        get
        {
            if (CopiedNodeID == 0)
            {
                return null;
            }
            else
            {
                if (mCopiedNode == null)
                {
                    mCopiedNode = DocumentHelper.GetDocument(CopiedNodeID, CopiedDocumentCulture, TreeProvider);
                    // Ensure original document is being copied
                    mCopiedNode = TreeProvider.GetOriginalNode(mCopiedNode);
                }
                return mCopiedNode;
            }
        }
    }


    /// <summary>
    /// Data class information of copied node.
    /// </summary>
    private DataClassInfo DataClassInfo
    {
        get
        {
            if ((mDataClassInfo == null) && (CopiedNode != null))
            {
                string className = ValidationHelper.GetString(CopiedNode.NodeClassName, string.Empty);
                mDataClassInfo = DataClassInfoProvider.GetDataClassInfo(className);
            }
            return mDataClassInfo;
        }
    }


    /// <summary>
    /// Form information of copied node's class.
    /// </summary>
    private FormInfo FormInfo
    {
        get
        {
            if ((mFormInfo == null) && (DataClassInfo != null))
            {
                // Load form information
                mFormInfo = new FormInfo(DataClassInfo.ClassFormDefinition);
            }
            return mFormInfo;
        }
    }


    /// <summary>
    /// Node name source field.
    /// </summary>
    private string NodeNameSource
    {
        get
        {
            if (mNodeNameSource == null)
            {
                if (DataClassInfo != null)
                {
                    mNodeNameSource = ValidationHelper.GetString(DataClassInfo.ClassNodeNameSource, string.Empty);
                }
                if (string.IsNullOrEmpty(mNodeNameSource))
                {
                    mNodeNameSource = DOCUMENT_NAME;
                }
            }
            return mNodeNameSource;
        }
    }


    /// <summary>
    /// Field information of node name source.
    /// </summary>
    private FormFieldInfo NodeNameSourceInfo
    {
        get
        {
            if ((mNodeNameSourceInfo == null) && (FormInfo != null))
            {
                mNodeNameSourceInfo = FormInfo.GetFormField(NodeNameSource);
            }
            return mNodeNameSourceInfo;
        }
    }


    /// <summary>
    /// Maximum length for textbox.
    /// </summary>
    private int MaxLength
    {
        get
        {
            if (NodeNameSourceInfo != null)
            {
                // Size of node name source field
                return NodeNameSourceInfo.Size;
            }
            else
            {
                // Document name size
                return 100;
            }
        }
    }


    /// <summary>
    /// Gets cancel button.
    /// </summary>
    public Button CancelButton
    {
        get
        {
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
            return btnSave;
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
        if (CopiedNode != null)
        {
            if (!RequestHelper.IsPostBack() || force)
            {
                // Suggest new document name
                txtDocumentName.Text = TreePathUtils.GetUniqueDocumentName(CopiedNode.DocumentName, CopiedNode.NodeParentID, 0, CopiedNode.DocumentCulture, CopiedNode.ClassName);
                plcDocumentName.Visible = true;
                plcMessage.Visible = false;
            }
        }
        // Set control properties
        Visible = (CopiedNode != null);
        txtDocumentName.MaxLength = MaxLength;
        btnCancel.Text = ResHelper.GetString("general.cancel");
        rfvDocumentName.ErrorMessage = ResHelper.GetString("basicform.erroremptyvalue");
    }


    protected override void AddError(string errorMessage)
    {
        lblError.Text = errorMessage;
    }

    #endregion


    #region "Button handling"

    protected void btnSave_Click(object sender, EventArgs e)
    {
        // Perform server-side validation
        rfvDocumentName.Validate();
        if ((TargetNode != null) && (CopiedNode != null) && rfvDocumentName.IsValid)
        {
            try
            {
                string newDocumentName = txtDocumentName.Text.Trim();
                TreeNode newNode = CopyNode(CopiedNode, TargetNode, false, TreeProvider, true, newDocumentName);

                // Check if copy was successful
                if (newNode != null)
                {
                    lblInfo.Text = ResHelper.GetString("contentrequest.copyok");
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ResHelper.GetString("contentrequest.copyfailed") + " : " + ex.Message;
            }
            finally
            {
                plcDocumentName.Visible = false;
                btnSave.Visible = false;
                btnCancel.Text = ResHelper.GetString("general.close");
            }
        }
    }

    #endregion
}