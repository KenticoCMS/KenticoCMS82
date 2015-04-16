using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.FormControls;
using CMS.Helpers;
using CMS.Base;
using CMS.DocumentEngine;
using CMS.DataEngine;
using CMS.SiteProvider;
using CMS.PortalEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_PortalEngine_FormControls_PageTemplates_PageTemplateLevels : FormEngineUserControl
{
    #region "Variables"

    private TreeNode mNode = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets tree path - if set is created from TreePath.
    /// </summary>
    public string TreePath
    {
        get
        {
            return treeElem.TreePath;
        }
        set
        {

            treeElem.TreePath = value;
        }
    }


    /// <summary>
    /// Gets or sets Level, levels are rendered only if TreePath is not set. 
    /// </summary>
    public int Level
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Level"), 0);
        }
        set
        {
            SetValue("Level", value);
            treeElem.Level = value;
        }
    }


    /// <summary>
    /// Gets or sets value.
    /// </summary>
    public override object Value
    {
        get
        {
            // Inherit all
            if (radInheritAll.Checked)
            {
                return String.Empty;
            }

            // Do not inherit any content
            if (radNonInherit.Checked)
            {
                return "/";
            }

            // Inherit from master
            if (radInheritMaster.Checked)
            {
                return "\\";
            }

            return treeElem.Value;
        }
        set
        {
            if (!RequestHelper.IsPostBack() || String.IsNullOrEmpty((string)treeElem.Value))
            {
                treeElem.Value = value;
                SetRadioButtons();
            }
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the control is used for a classic page template or a wireframe template.
    /// </summary>
    public bool IsWireframeTemplate
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("IsWireframeTemplate"), false);
        }
        set
        {
            SetValue("IsWireframeTemplate", value);
        }
    }


    /// <summary>
    /// Gets or sets the node.
    /// </summary>
    public TreeNode Node
    {
        get
        {
            if (mNode == null)
            {
                mNode = DocumentManager.Node;
            }

            return mNode;
        }
        set
        {
            mNode = value;
        }
    }

    #endregion


    #region "Page methods"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup the radio button texts
        radNonInherit.Text = GetString("InheritLevels.NonInherit");
        radInheritMaster.Text = GetString("InheritLevels.InheritMaster");

        treeElem.Level = Level;
        if (Node != null)
        {
            // Try get info whether exist linked document in path
            DataSet ds = DocumentManager.Tree.SelectNodes(SiteContext.CurrentSiteName, "/%", Node.DocumentCulture, false, null, "NodeLinkedNodeID IS NOT NULL AND (N'" + SqlHelper.EscapeQuotes(Node.NodeAliasPath) + "' LIKE NodeAliasPath + '%')", null, -1, false, 1, "Count(*) AS NumOfDocs");

            // If node is not link or none of parent documents is not linked document use document name path
            if (!Node.IsLink && ValidationHelper.GetInteger(DataHelper.GetDataRowValue(ds.Tables[0].Rows[0], "NumOfDocs"), 0) == 0)
            {
                TreePath = TreePathUtils.GetParentPath("/" + Node.DocumentNamePath);
            }
            else
            {
                // Otherwise use alias path
                TreePath = TreePathUtils.GetParentPath("/" + Node.NodeAliasPath);
            }

            radInheritAll.Text = GetString("InheritLevels.UseTemplateSettigns");
            bool isWireframe = IsWireframeTemplate || Node.IsWireframe();
            PageTemplateInfo template = PageTemplateInfoProvider.GetPageTemplateInfo(isWireframe ? Node.NodeWireframeTemplateID : Node.DocumentPageTemplateID);
            if (template != null)
            {
                string resString = String.Empty;
                string levels = String.Empty;
                switch (template.InheritPageLevels)
                {
                    case "/":
                        resString = "InheritLevels.NoNesting";
                        break;
                    case "\\":
                        resString = "InheritLevels.InheritMaster";
                        break;
                    case "":
                        resString = "InheritLevels.InheritAll";
                        break;
                    default:
                        resString = "InheritLevels.SelectedLevels";

                        // Format page levels
                        levels = template.InheritPageLevels.Trim(new[] { '}', '{', '/' });
                        levels = levels.Replace("}/{", ", ");
                        break;
                }

                // Display page template default setting
                radInheritAll.Text += " (" + GetString(resString) + (!String.IsNullOrEmpty(levels) ? ": " + levels : String.Empty) + ")";
            }

            radSelect.Text = GetString("InheritLevels.Select");
        }
        else
        {
            // Page template setting
            radInheritAll.Text = GetString("InheritLevels.InheritAll");
            radSelect.Text = GetString("InheritLevels.SelectTemplate");
        }
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        plcTree.Visible = radSelect.Checked;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Sets the value to the radio buttons
    /// </summary>
    private void SetRadioButtons()
    {
        string treeValue = ValidationHelper.GetString(treeElem.Value, string.Empty);

        // Do not inherit any content
        if (treeValue == "/")
        {
            radNonInherit.Checked = true;
        }
        // Inherit from master
        else if (treeValue == "\\")
        {
            radInheritMaster.Checked = true;
        }
        //  Inherited levels
        else if (!String.IsNullOrEmpty(treeValue))
        {
            radSelect.Checked = true;
        }
        else
        {
            radInheritAll.Checked = true;
        }
    }

    #endregion
}