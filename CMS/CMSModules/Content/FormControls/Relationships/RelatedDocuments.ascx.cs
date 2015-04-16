using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DocumentEngine;
using CMS.DataEngine;
using CMS.Relationships;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_Content_FormControls_Relationships_RelatedDocuments : ReadOnlyFormEngineUserControl
{
    #region "Variables"

    private TreeProvider mTreeProvider;
    private bool mShowAddRelation = true;
    private DialogConfiguration mConfig;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets the configuration for Copy and Move dialog.
    /// </summary>
    private DialogConfiguration Config
    {
        get
        {
            if (mConfig == null)
            {
                mConfig = new DialogConfiguration();
                mConfig.HideLibraries = true;
                mConfig.ContentSelectedSite = SiteContext.CurrentSiteName;
                mConfig.HideAnchor = true;
                mConfig.HideAttachments = true;
                mConfig.HideContent = false;
                mConfig.HideEmail = true;
                mConfig.HideLibraries = true;
                mConfig.HideWeb = true;
                mConfig.ContentSelectedSite = SiteContext.CurrentSiteName;
                mConfig.OutputFormat = OutputFormatEnum.Custom;
                mConfig.CustomFormatCode = "relationship";
                mConfig.ContentSites = AvailableSitesEnum.OnlyCurrentSite;
            }
            return mConfig;
        }
    }

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets relationship name.
    /// </summary>
    public string RelationshipName
    {
        get
        {
            return GetValue("RelationshipName", String.Empty);
        }
        set
        {
            SetValue("RelationshipName", value);
        }
    }


    /// <summary>
    /// Indicates if allow switch sides.
    /// </summary>
    public bool AllowSwitchSides
    {
        get
        {
            return GetValue("AllowSwitchSides", true);
        }
        set
        {
            SetValue("AllowSwitchSides", value);
        }
    }


    /// <summary>
    /// Default side (False - left, True - right).
    /// </summary>
    public bool DefaultSide
    {
        get
        {
            return GetValue("DefaultSide", true);
        }
        set
        {
            SetValue("DefaultSide", value);
        }
    }


    /// <summary>
    /// Default page size.
    /// </summary>
    public int DefaultPageSize
    {
        get
        {
            return GetValue("DefaultPageSize", 5);
        }
        set
        {
            SetValue("DefaultPageSize", value);
        }
    }


    /// <summary>
    /// Page size values separated with comma.
    /// </summary>
    public string PageSize
    {
        get
        {
            return GetValue("PageSize", "5,10,25,50,100,##ALL##");
        }
        set
        {
            SetValue("PageSize", value);
        }
    }


    /// <summary>
    /// Indicates id show link 'Add relation'.
    /// </summary>
    public bool ShowAddRelation
    {
        get
        {
            return mShowAddRelation;
        }
        set
        {
            mShowAddRelation = value;
        }
    }


    /// <summary>
    /// Path where content tree in document selection dialog will start.
    /// </summary>
    public string StartingPath
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the document;.
    /// </summary>
    public TreeNode TreeNode
    {
        get;
        set;
    }


    /// <summary>
    /// Gets tree provider.
    /// </summary>
    public TreeProvider TreeProvider
    {
        get
        {
            if (mTreeProvider == null)
            {
                mTreeProvider = new TreeProvider(MembershipContext.AuthenticatedUser);
            }

            return mTreeProvider;
        }
    }


    /// <summary>
    /// Enables or disables control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            UniGridRelationship.GridView.Enabled = value;
            btnNewRelationship.Enabled = value;
        }
    }


    /// <summary>
    /// Messages placeholder.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// OnInit event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Paging
        UniGridRelationship.PageSize = PageSize;
        UniGridRelationship.Pager.DefaultPageSize = DefaultPageSize;
    }


    /// <summary>
    /// PageLoad event handler.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            UniGridRelationship.StopProcessing = StopProcessing;
        }
        else
        {
            // Set tree node from Form object
            if ((TreeNode == null) && (Form != null) && (Form.EditedObject != null))
            {
                TreeNode node = Form.EditedObject as TreeNode;
                if ((node != null) && (Form.Mode == FormModeEnum.Update))
                {
                    TreeNode = node;
                }
                else
                {
                    ShowError(GetString("relationship.editdocumenterror"));
                }
            }

            if (TreeNode != null)
            {
                // Set unigrid
                UniGridRelationship.OnExternalDataBound += UniGridRelationship_OnExternalDataBound;
                UniGridRelationship.OnBeforeDataReload += UniGridRelationship_OnBeforeDataReload;
                UniGridRelationship.OnAction += UniGridRelationship_OnAction;
                UniGridRelationship.ZeroRowsText = GetString("relationship.nodatafound");
                UniGridRelationship.ShowActionsMenu = !IsLiveSite;

                int nodeId = TreeNode.NodeID;
                bool oneRelationshipName = !string.IsNullOrEmpty(RelationshipName);

                WhereCondition condition = new WhereCondition();

                if (oneRelationshipName)
                {
                    condition.WhereIn("RelationshipNameID", new IDQuery<RelationshipNameInfo>().WhereEquals("RelationshipName", RelationshipName));
                }

                // Switch sides is disabled
                if (!AllowSwitchSides)
                {
                    condition.WhereEquals(DefaultSide ? "RightNodeID" : "LeftNodeID", nodeId);
                }
                else
                {
                    condition.Where(new WhereCondition().WhereEquals("RightNodeID", nodeId).Or().WhereEquals("LeftNodeID", nodeId));
                }

                UniGridRelationship.WhereCondition = condition.ToString(true);

                if (ShowAddRelation)
                {
                    btnNewRelationship.OnClientClick = GetAddRelatedDocumentScript() + " return false;";
                }
                else
                {
                    pnlNewLink.Visible = false;
                }
            }
            else
            {
                UniGridRelationship.StopProcessing = true;
                UniGridRelationship.Visible = false;
                pnlNewLink.Visible = false;
            }

            if (RequestHelper.IsPostBack())
            {
                string target = Request[Page.postEventSourceID];
                if ((target == pnlUpdate.ClientID) || (target == pnlUpdate.UniqueID))
                {
                    string action = Request[Page.postEventArgumentID];

                    if (!string.IsNullOrEmpty(action))
                    {
                        switch (action.ToLowerCSafe())
                        {
                            // Insert from 'Select document' dialog
                            case "insertfromselectdocument":
                                SaveRelationship();
                                break;
                        }
                    }
                }
            }
            else
            {
                bool inserted = QueryHelper.GetBoolean("inserted", false);
                if (inserted)
                {
                    ShowConfirmation(GetString("relationship.wasadded"));
                }    
            }
        }
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Performs actions before reload grid.
    /// </summary>
    private void UniGridRelationship_OnBeforeDataReload()
    {
        DataControlField rightTypeColumn = UniGridRelationship.NamedColumns["RightClassID"];

        // Hide columns
        if (!string.IsNullOrEmpty(RelationshipName) && !AllowSwitchSides)
        {
            string headerText = GetString("relationship.relateddocument");
            DataControlField leftColumn = UniGridRelationship.NamedColumns["LeftNodeName"];
            DataControlField relationshipNameColumn = UniGridRelationship.NamedColumns["RelationshipDisplayName"];
            DataControlField rightColumn = UniGridRelationship.NamedColumns["RightNodeName"];

            if (DefaultSide)
            {
                leftColumn.HeaderText = headerText;
                leftColumn.HeaderStyle.Width = new Unit("100%");
                rightColumn.Visible = false;
                rightTypeColumn.Visible = false;
            }
            else
            {
                DataControlField leftColumnType = UniGridRelationship.NamedColumns["LeftClassID"];

                rightColumn.HeaderText = headerText;
                rightTypeColumn.HeaderStyle.Width = new Unit("100%");
                leftColumn.Visible = false;
                leftColumnType.Visible = false;
            }

            // Hide relationship name column
            relationshipNameColumn.Visible = false;
        }
        else
        {
            rightTypeColumn.HeaderStyle.Width = new Unit("100%");
        }
    }


    /// <summary>
    /// Fires on the grid action.
    /// </summary>
    /// <param name="actionName">Action name</param>
    /// <param name="actionArgument">Action argument</param>
    private void UniGridRelationship_OnAction(string actionName, object actionArgument)
    {
        // Check modify permissions
        if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(TreeNode, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
        {
            return;
        }

        if (actionName == "delete")
        {
            string[] parameters = ((string)actionArgument).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (parameters.Length == 3)
            {
                // Parse parameters
                int leftNodeId = ValidationHelper.GetInteger(parameters[0], 0);
                int rightNodeId = ValidationHelper.GetInteger(parameters[1], 0);
                int relationshipNameId = ValidationHelper.GetInteger(parameters[2], 0);

                // If parameters are valid
                if ((leftNodeId > 0) && (rightNodeId > 0) && (relationshipNameId > 0))
                {
                    // Remove relationship
                    RelationshipInfoProvider.RemoveRelationship(leftNodeId, rightNodeId, relationshipNameId);

                    // Log synchronization
                    DocumentSynchronizationHelper.LogDocumentChange(TreeNode.NodeSiteName, TreeNode.NodeAliasPath, TaskTypeEnum.UpdateDocument, TreeProvider);

                    ShowConfirmation(GetString("relationship.wasdeleted"));
                }
            }
        }
    }


    /// <summary>
    /// Binds the grid columns.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="sourceName">Source name</param>
    /// <param name="parameter">Parameter</param>
    private object UniGridRelationship_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "lefnodename":
            case "rightnodename":
                var tr = new ObjectTransformation(PredefinedObjectType.NODE, ValidationHelper.GetInteger(parameter, 0));
                tr.EncodeOutput = false;
                tr.Transformation = "{%Object.GetIcon()%} {%NodeName|(default)" + GetString("general.root") + "|(encode)%}";
                return tr;

            case "delete":
                CMSGridActionButton btn = ((CMSGridActionButton)sender);
                btn.PreRender += imgDelete_PreRender;
                break;
        }

        return parameter;
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected void imgDelete_PreRender(object sender, EventArgs e)
    {
        CMSGridActionButton imgDelete = (CMSGridActionButton)sender;
        if (!Enabled)
        {
            // Disable delete icon in case that editing is not allowed
            imgDelete.Enabled = false;
            imgDelete.Style.Add("cursor", "default");
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Saves relationship.
    /// </summary>
    public void SaveRelationship()
    {
        if (TreeNode != null)
        {
            // Check modify permissions
            if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(TreeNode, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
            {
                return;
            }

            bool currentNodeIsOnLeftSide = !DefaultSide;
            // Selected node Id
            int selectedNodeId = ValidationHelper.GetInteger(hdnSelectedNodeId.Value, 0);

            // Get relationshipname
            RelationshipNameInfo relationshipNameInfo = RelationshipNameInfoProvider.GetRelationshipNameInfo(RelationshipName);

            int relationshipNameId = 0;
            if (relationshipNameInfo != null)
            {
                relationshipNameId = relationshipNameInfo.RelationshipNameId;
            }

            if ((selectedNodeId > 0) && (relationshipNameId > 0))
            {
                try
                {
                    // Left side
                    if (currentNodeIsOnLeftSide)
                    {
                        RelationshipInfoProvider.AddRelationship(TreeNode.NodeID, selectedNodeId, relationshipNameId);
                    }
                    // Right side
                    else
                    {
                        RelationshipInfoProvider.AddRelationship(selectedNodeId, TreeNode.NodeID, relationshipNameId);
                    }

                    // Log synchronization
                    DocumentSynchronizationHelper.LogDocumentChange(TreeNode.NodeSiteName, TreeNode.NodeAliasPath, TaskTypeEnum.UpdateDocument, TreeProvider);

                    ShowConfirmation(GetString("relationship.wasadded"));
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }
    }


    /// <summary>
    /// Returns Javascript used for invoking 'add related document' dialog.
    /// </summary>
    public string GetAddRelatedDocumentScript()
    {
        string postbackArgument;
        if (!AllowSwitchSides && !string.IsNullOrEmpty(RelationshipName))
        {
            postbackArgument = "insertfromselectdocument";

            // Register javascript 'postback' function
            string script = "function RefreshRelatedPanel(elementId) { if (elementId != null) { __doPostBack(elementId, '" + postbackArgument + "'); } } \n";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshRelatedPanel", ScriptHelper.GetScript(script));

            // Dialog 'Select document'
            Config.EditorClientID = pnlUpdate.ClientID + ";" + hdnSelectedNodeId.ClientID;

            // Set dialog starting path
            if (!string.IsNullOrEmpty(StartingPath))
            {
                Config.ContentStartingPath = StartingPath;
            }

            string url = CMSDialogHelper.GetDialogUrl(Config, IsLiveSite, false, null, false);

            return string.Format("modalDialog('{0}', 'contentselectnode', '90%', '85%');", url);
        }
        else
        {
            postbackArgument = "insert";

            // Register javascript 'postback' function
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshUpdatePanel_" + ClientID, ScriptHelper.GetScript(
                "function RefreshUpdatePanel_" + ClientID + "(){ " + Page.ClientScript.GetPostBackEventReference(pnlUpdate, postbackArgument) + "; } \n"));

            // Dialog 'Add related document'
            string query = "?nodeid=" + TreeNode.NodeID;
            query = URLHelper.AddUrlParameter(query, "defaultside", DefaultSide.ToString());
            query = URLHelper.AddUrlParameter(query, "allowswitchsides", AllowSwitchSides.ToString());
            query = URLHelper.AddUrlParameter(query, "relationshipname", RelationshipName);
            query = URLHelper.AddUrlParameter(query, "externalControlID", ClientID);
            query = URLHelper.AddUrlParameter(query, "startingpath", StartingPath ?? "");

            query = query.Replace("%", "%25").Replace("/", "%2F");

            query = URLHelper.AddUrlParameter(query, "hash", QueryHelper.GetHash(query));

            string url;
            if (IsLiveSite)
            {
                url = ResolveUrl("~/CMSFormControls/LiveSelectors/RelatedDocuments.aspx" + query);
            }
            else
            {
                url = ResolveUrl("~/CMSFormControls/Selectors/RelatedDocuments.aspx" + query);
            }

            return string.Format("modalDialog('{0}', 'AddRelatedDocument', '900', '315');", url);
        }
    }

    #endregion
}