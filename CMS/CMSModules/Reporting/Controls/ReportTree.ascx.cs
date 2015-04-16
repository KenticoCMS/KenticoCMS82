using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.Helpers;
using CMS.Reporting;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSModules_Reporting_Controls_ReportTree : CMSAdminControl
{
    #region "Variables"

    private int mMaxTreeNodes = -1;
    private bool mShowReports = true;
    private bool mUseMaxNodeLimit = true;

    /// <summary>
    /// Index used for item count under one node.
    /// </summary>
    private int mIndex = -1;

    #endregion


    #region "Public Properties"

    /// <summary>
    /// Indicates whether use max node limit stored in settings.
    /// </summary>
    public bool UseMaxNodeLimit
    {
        get
        {
            return mUseMaxNodeLimit;
        }
        set
        {
            mUseMaxNodeLimit = value;
        }
    }

    /// <summary>
    /// If false tree wont show reports but just categories.
    /// </summary>
    public bool ShowReports
    {
        set
        {
            mShowReports = value;
        }
        get
        {
            return mShowReports;
        }
    }

    /// <summary>
    /// Gets or sets select path.
    /// </summary>
    public string SelectPath
    {
        get
        {
            return treeElem.SelectPath;
        }
        set
        {
            treeElem.SelectPath = value;
            treeElem.ExpandPath = value;
        }
    }


    /// <summary>
    /// Maximum tree nodes shown under parent node - this value can be ignored if UseMaxNodeLimit set to false.
    /// </summary>
    public int MaxTreeNodes
    {
        get
        {
            if (mMaxTreeNodes < 0)
            {
                mMaxTreeNodes = SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSMaxUITreeNodes");
            }
            return mMaxTreeNodes;
        }
        set
        {
            mMaxTreeNodes = value;
        }
    }

    #endregion


    #region "Custom events"

    /// <summary>
    /// On selected item event handler.
    /// </summary>    
    public delegate void ItemSelectedEventHandler(string selectedValue);

    /// <summary>
    /// On selected item event handler.
    /// </summary>
    public event ItemSelectedEventHandler OnItemSelected;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        ScriptHelper.RegisterJQuery(Page);

        // Create and set category provider
        UniTreeProvider categoryProvider = new UniTreeProvider();
        categoryProvider.DisplayNameColumn = "DisplayName";
        categoryProvider.IDColumn = "ObjectID";
        categoryProvider.LevelColumn = "ObjectLevel";
        categoryProvider.OrderColumn = "CategoryOrder";
        categoryProvider.ParentIDColumn = "ParentID";
        categoryProvider.PathColumn = "ObjectPath";
        categoryProvider.ValueColumn = "ObjectID";
        categoryProvider.ChildCountColumn = "CompleteChildCount";
        categoryProvider.QueryName = "Reporting.ReportCategory.selectallview";
        categoryProvider.ObjectTypeColumn = "ObjectType";
        categoryProvider.Columns = "DisplayName, ObjectID, ObjectLevel,CategoryOrder,ParentID, ObjectPath, CompleteChildCount,ObjectType,CategoryChildCount, CategoryImagePath";
        categoryProvider.ImageColumn = "CategoryImagePath";

        if (!ShowReports)
        {
            categoryProvider.WhereCondition = "ObjectType = 'reportcategory'";
            categoryProvider.ChildCountColumn = "CategoryChildCount";
            categoryProvider.ObjectTypeColumn = "";

            treeElem.DefaultImagePath = GetImageUrl("Objects/CMS_WebPartCategory/list.png");
            treeElem.NodeTemplate = "<span id=\"##OBJECTTYPE##_##NODEID##\" onclick=\"SelectReportNode(##NODEID##,'##OBJECTTYPE##', ##PARENTNODEID##,true);\" name=\"treeNode\" class=\"ContentTreeItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
            treeElem.SelectedNodeTemplate = "<span id=\"##OBJECTTYPE##_##NODEID##\" onclick=\"SelectReportNode(##NODEID##,'##OBJECTTYPE##', ##PARENTNODEID##,true);\" name=\"treeNode\" class=\"ContentTreeItem ContentTreeSelectedItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
        }
        else
        {
            categoryProvider.OrderBy = "ObjectType DESC, DisplayName ASC";

            treeElem.OnGetImage.Execute += treeElem_OnGetImage;
            treeElem.NodeTemplate = "<span id=\"##OBJECTTYPE##_##NODEID##\" onclick=\"SelectReportNode(##NODEID##,'##OBJECTTYPE##', ##PARENTNODEID##,true);\" name=\"treeNode\" class=\"ContentTreeItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
            treeElem.SelectedNodeTemplate = "<span id=\"##OBJECTTYPE##_##NODEID##\" onclick=\"SelectReportNode(##NODEID##,'##OBJECTTYPE##', ##PARENTNODEID##,true);\" name=\"treeNode\" class=\"ContentTreeItem ContentTreeSelectedItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
        }

        // Set up tree 
        treeElem.ProviderObject = categoryProvider;

        treeElem.IsLiveSite = false;

        // Setup event handler
        treeElem.OnItemSelected += treeElem_OnItemSelected;

        treeElem.UsePostBack = false;
        treeElem.OnNodeCreated += treeElem_OnNodeCreated;
    }


    /// <summary>
    /// Used for maxnodes in collapsed node.
    /// </summary>
    private TreeNode treeElem_OnNodeCreated(DataRow itemData, TreeNode defaultNode)
    {
        if (UseMaxNodeLimit && (MaxTreeNodes > 0))
        {
            //Get parentID from data row
            int parentID = ValidationHelper.GetInteger(itemData.ItemArray[4], 0);
            string objectType = ValidationHelper.GetString(itemData.ItemArray[7], String.Empty);

            //Dont use maxnodes limitation for categories
            if (objectType.ToLowerCSafe() == "reportcategory")
            {
                return defaultNode;
            }

            //Increment index count in collapsing
            mIndex++;
            if (mIndex == MaxTreeNodes)
            {
                //Load parentid
                int parentParentID = 0;
                ReportCategoryInfo parentParent = ReportCategoryInfoProvider.GetReportCategoryInfo(parentID);
                if (parentParent != null)
                {
                    parentParentID = parentParent.CategoryParentID;
                }

                TreeNode node = new TreeNode();
                node.Text = "<span class=\"ContentTreeItem\" onclick=\"SelectReportNode(" + parentID + " ,'reportcategory'," + parentParentID + ",true ); return false;\"><span class=\"Name\">" + GetString("general.seelisting") + "</span></span>";
                return node;
            }
            if (mIndex > MaxTreeNodes)
            {
                return null;
            }
        }
        return defaultNode;
    }


    /// <summary>
    /// Page PreRender.
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        treeElem.ReloadData();
    }


    /// <summary>
    ///  On selected item event.
    /// </summary>
    /// <param name="selectedValue">Selected value</param>
    protected void treeElem_OnItemSelected(string selectedValue)
    {
        if (OnItemSelected != null)
        {
            OnItemSelected(selectedValue);
        }
    }


    /// <summary>
    /// On get image event.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="args">Event arguments wrapper</param>
    protected void treeElem_OnGetImage(object sender, UniTreeImageArgs args)
    {
        if ((args.TreeNode != null) && (args.TreeNode.ItemData != null))
        {
            string objectType = string.Empty;

            DataRow dr = (DataRow)args.TreeNode.ItemData;
            if (dr != null)
            {
                objectType = ValidationHelper.GetString(dr["ObjectType"], "").ToLowerCSafe();
            }

            // Return image path
            if (objectType == "report")
            {
                args.ImagePath = GetImageUrl("Objects/Reporting_Report/object.png");
            }
            else if (objectType == "reportcategory")
            {
                args.ImagePath = GetImageUrl("Objects/Reporting_ReportCategory/list_tree.png");
            }
        }
    }

    #endregion
}