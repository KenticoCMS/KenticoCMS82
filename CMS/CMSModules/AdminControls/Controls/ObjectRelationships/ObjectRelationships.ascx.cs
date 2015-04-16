using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using System.Collections;

using CMS.ExtendedControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.DataEngine;
using CMS.Relationships;

public partial class CMSModules_AdminControls_Controls_ObjectRelationships_ObjectRelationships : CMSUserControl
{
    #region "Private fields"

    /// <summary>
    /// Table of the registered object types [objectType -> true]
    /// </summary>
    private Hashtable registeredObjects = new Hashtable();

    private TranslationHelper th = null;

    private bool loaded = false;

    private GeneralizedInfo mObject = null;
    private GeneralizedInfo mRelatedObject = null;

    private int mObjectID = -1;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Returns true if the left side is active.
    /// </summary>
    private bool ActiveLeft
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["ActiveLeft"], false);
        }
        set
        {
            ViewState["ActiveLeft"] = value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Current object.
    /// </summary>
    public GeneralizedInfo Object
    {
        get
        {
            if (mObject == null)
            {
                mObject = BaseAbstractInfoProvider.GetInfoById(ObjectType, ObjectID);
            }

            return mObject;
        }
    }


    /// <summary>
    /// Related object type.
    /// </summary>
    public GeneralizedInfo RelatedObject
    {
        get
        {
            if (mRelatedObject == null)
            {
                string selected = drpRelatedObjType.SelectedValue;
                if (!String.IsNullOrEmpty(selected))
                {
                    mRelatedObject = ModuleManager.GetReadOnlyObject(selected);
                }
            }

            return mRelatedObject;
        }
    }


    /// <summary>
    /// Type of the current object.
    /// </summary>
    public string ObjectType
    {
        get;
        set;
    }


    /// <summary>
    /// ID of the current object.
    /// </summary>
    public int ObjectID
    {
        get
        {
            return mObjectID;
        }
        set
        {
            mObjectID = value;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        LoadData();
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            DisplayAvailableObjects();
        }

        int selectedSiteId = ValidationHelper.GetInteger(siteSelector.Value, 0);

        pnlNew.Visible = (!pnlSite.Visible || (selectedSiteId >= 0)) && (RelatedObject != null);

        string safeObjectType;

        string activeType = null;
        string currentType = null;

        // Initialize form labels
        var obj = Object;
        if (obj != null)
        {
            safeObjectType = obj.TypeInfo.ObjectType.Replace(".", "_");
            currentType = " (" + GetString("ObjectType." + safeObjectType) + ")";
        }

        if (RelatedObject != null)
        {
            safeObjectType = RelatedObject.TypeInfo.ObjectType.Replace(".", "_");
            activeType = " (" + GetString("ObjectType." + safeObjectType) + ")";
        }

        if (pnlAddNew.Visible)
        {
            btnSwitchSides.Text = GetString("Relationship.SwitchSides");

            leftCell.Text = GetString("ObjRelationship.LeftSide");
            rightCell.Text = GetString("ObjRelationship.RightSide");
            middleCell.Text = GetString("Relationship.RelationshipName");

            // Handle the active items
            if (ActiveLeft)
            {
                leftCell.Text += activeType;
                rightCell.Text += currentType;

                selLeftObj.Visible = true;
                lblLeftObj.Visible = false;

                selRightObj.Visible = false;
                lblRightObj.Visible = true;
            }
            else
            {
                leftCell.Text += currentType;
                rightCell.Text += activeType;

                selLeftObj.Visible = false;
                lblLeftObj.Visible = true;

                selRightObj.Visible = true;
                lblRightObj.Visible = false;
            }
        }
        else
        {
            if (!loaded)
            {
                gridItems.ReloadData();
            }
        }

        base.OnPreRender(e);
    }


    public void LoadData()
    {
        if (!RequestHelper.IsPostBack())
        {
            // Initialize controls
            SetupControl();
        }

        drpRelatedObjType.DropDownList.AutoPostBack = true;
        drpRelatedObjType.DropDownList.SelectedIndexChanged += drpRelatedObjType_SelectedIndexChanged;

        // Init site selector
        siteSelector.OnSelectionChanged += siteSelector_OnSelectionChanged;
        siteSelector.DropDownSingleSelect.AutoPostBack = true;

        // Init events
        gridItems.OnAction += gridItems_OnAction;
        gridItems.OnBeforeDataReload += gridItems_OnBeforeDataReload;
        gridItems.OnExternalDataBound += GridItemsOnOnExternalDataBound;

        // Setup the site selector
        string selectedObjectType = (drpRelatedObjType.DropDownList.SelectedItem != null ? drpRelatedObjType.DropDownList.SelectedItem.Value : null);
        gridItems.WhereCondition = GetWhereCondition(selectedObjectType);

        // Display items that are available
        if (pnlAddNew.Visible)
        {
            DisplayAvailableItems();
        }
    }


    private object GridItemsOnOnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "left":
                return GetDisplayTransformation(parameter, true);

            case "right":
                return GetDisplayTransformation(parameter, false);
        }

        return parameter;
    }

    #endregion


    #region "Grid view handling"

    protected void gridItems_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "delete":
                // Look for info on relationship being removed
                int leftObjId = ValidationHelper.GetInteger(Request.Params["leftObjId"], -1);
                string leftObjType = ValidationHelper.GetString(Request.Params["leftObjType"], "");
                int relationshipId = ValidationHelper.GetInteger(Request.Params["relationshipId"], -1);
                int rightObjId = ValidationHelper.GetInteger(Request.Params["rightObjId"], -1);
                string rightObjType = ValidationHelper.GetString(Request.Params["rightObjType"], "");

                // Remove the relationship if all the necessary information available
                if ((leftObjId > -1) && (leftObjType.Trim() != "") && (relationshipId > -1) && (rightObjId > -1) && (rightObjType.Trim() != ""))
                {
                    ObjectRelationshipInfoProvider.RemoveRelationship(leftObjId, leftObjType, rightObjId, rightObjType, relationshipId);
                }

                // Reload the data
                gridItems.ReloadData();
                break;
        }
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Refreshes the selection of site.
    /// </summary>
    protected void RefreshNewSiteSelection()
    {
        var relatedObject = RelatedObject;
        if (relatedObject != null)
        {
            if (relatedObject.TypeInfo.SiteIDColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN)
            {
                pnlSite.Visible = true;

                // Setup the site selector
                bool allowGlobalObjects = true;
                string selectedObjectType = (drpRelatedObjType.DropDownList.SelectedItem != null ? drpRelatedObjType.DropDownList.SelectedItem.Value : null);
                if (!String.IsNullOrEmpty(selectedObjectType))
                {
                    var obj = ModuleManager.GetReadOnlyObject(selectedObjectType);
                    if (obj != null)
                    {
                        allowGlobalObjects = obj.TypeInfo.SupportsGlobalObjects;
                    }
                }

                siteSelector.SpecialFields.Add(new SpecialField { Text = GetString("general.selectall"), Value = "-1" });
                if (allowGlobalObjects)
                {
                    siteSelector.SpecialFields.Add(new SpecialField { Text = GetString("general.globalobjects"), Value = "0" });
                }
                if (siteSelector.DropDownSingleSelect.Items.Count > 0)
                {
                    siteSelector.Reload(true);
                }

                siteSelector_OnSelectionChanged(null, null);

                return;
            }
        }

        pnlSite.Visible = false;
    }


    protected void btnNewRelationship_Click(object sender, EventArgs e)
    {
        // Hide and disable unused controls
        pnlEditList.Visible = false;
        pnlAddNew.Visible = true;

        RefreshNewSiteSelection();

        // Initialize drop-down list with available relationship types
        DisplayAvailableRelationships();

        // Initialize drop=down list with available relationship items
        DisplayAvailableItems();

        // Supply the current object name
        lblLeftObj.Text = HTMLHelper.HTMLEncode(Object.ObjectDisplayName);
        lblRightObj.Text = HTMLHelper.HTMLEncode(Object.ObjectDisplayName);
    }


    protected void btnSwitchSides_Click(object sender, EventArgs e)
    {
        bool newActiveLeft = !ActiveLeft;
        if (newActiveLeft)
        {
            selLeftObj.Value = selRightObj.Value;
        }
        else
        {
            selRightObj.Value = selLeftObj.Value;
        }

        ActiveLeft = newActiveLeft;

        // Display the items that are available
        DisplayAvailableItems();
    }


    protected void drpRelatedObjType_SelectedIndexChanged(object sender, EventArgs e)
    {
        RefreshNewSiteSelection();
    }


    public void Save()
    {
        // Add the relationship between objects
        if (AddRelationship())
        {
            string safeObjectType = RelatedObject.TypeInfo.ObjectType.Replace(".", "_");
            string activeType = GetString("ObjectType." + safeObjectType);

            string name = ActiveLeft ? selLeftObj.DropDownSingleSelect.SelectedItem.Text : selRightObj.DropDownSingleSelect.SelectedItem.Text;

            ShowInformation(String.Format(ResHelper.GetString("Relationship.Saved"), activeType, HTMLHelper.HTMLEncode(name)));
        }
    }


    public void SaveAndClose()
    {
        // Add the relationship between objects
        if (AddRelationship())
        {
            // Load the list dialog
            pnlEditList.Visible = true;
            pnlAddNew.Visible = false;
            drpRelatedObjType.Enabled = true;

            // Reload the data
            gridItems.ReloadData();
        }
    }


    /// <summary>
    /// Handles site selection change event.
    /// </summary>
    protected void siteSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        // If the new relationship is being added
        if (pnlAddNew.Visible)
        {
            DisplayAvailableItems();
        }
        else
        {
            // Reload the data
            gridItems.ReloadData();
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Reloads the data for the UniGrid control displaying the objects related to the current object.
    /// </summary>
    private void gridItems_OnBeforeDataReload()
    {
        loaded = true;

        registeredObjects.Clear();

        // Prepare the translations table
        th = new TranslationHelper();
        th.UseDisplayNameAsCodeName = true;
    }


    /// <summary>
    /// Inserts the new relationship according the selected values.
    /// </summary>
    private bool AddRelationship()
    {
        if (ObjectID > 0)
        {
            if (drpRelationship.SelectedItem == null)
            {
                ShowError(GetString("ObjRelationship.MustSelect"));
                return false;
            }

            // Get information on type of the selected relationship
            int selectedRelationshipId = ValidationHelper.GetInteger(drpRelationship.SelectedItem.Value, -1);
            string selectedObjType = null;

            // If the main objectis on the left side selected object is taken from rifht drop-down list
            bool currentOnLeft = !ActiveLeft;
            int selectedObjId = currentOnLeft ? ValidationHelper.GetInteger(selRightObj.Value, -1) : ValidationHelper.GetInteger(selLeftObj.Value, -1);

            // Get information on type of the selected object
            selectedObjType = drpRelatedObjType.DropDownList.SelectedItem.Value;

            // If all the necessary information are present
            if ((selectedObjId <= 0) || (selectedRelationshipId <= 0) || (selectedObjType == null))
            {
                ShowError(GetString("ObjRelationship.MustSelect"));
                return false;
            }

            if (currentOnLeft)
            {
                ObjectRelationshipInfoProvider.AddRelationship(ObjectID, ObjectType, selectedObjId, selectedObjType, selectedRelationshipId);
                return true;
            }
            else
            {
                ObjectRelationshipInfoProvider.AddRelationship(selectedObjId, selectedObjType, ObjectID, ObjectType, selectedRelationshipId);
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    private void SetupControl()
    {
        // Information on current object supplied
        if ((ObjectID == 0) || string.IsNullOrEmpty(ObjectType))
        {
            ShowError("[ObjectRelationships.ascx]: ObjectID or ObjectType wasn't initialized.");

            gridItems.StopProcessing = true;
        }
    }


    /// <summary>
    /// Gets the where condition for the selected object type.
    /// </summary>
    /// <param name="selectedObjectType">Selected object type</param>
    public string GetWhereCondition(string selectedObjectType)
    {
        if (Object != null)
        {
            string where = null;

            var relatedObject = RelatedObject;
            if (relatedObject != null)
            {
                // Get the site name
                var relatedTypeInfo = relatedObject.TypeInfo;
                if (relatedTypeInfo.SiteIDColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN)
                {
                    if (siteSelector.DropDownSingleSelect.Items.Count == 0)
                    {
                        siteSelector.Value = SiteContext.CurrentSiteID;
                    }

                    if (siteSelector.HasData)
                    {
                        // Set the site name for registration
                        int selectedSiteId = ValidationHelper.GetInteger(siteSelector.Value, 0);
                        if (selectedSiteId >= 0)
                        {
                            string siteQuery = new DataQuery(relatedTypeInfo.ObjectType, null)
                                                    .Column(relatedTypeInfo.IDColumn)
                                                    .Where(SqlHelper.GetSiteIDWhereCondition(relatedTypeInfo.SiteIDColumn, selectedSiteId))
                                                    .QueryText;

                            // Where condition for the left object
                            string rightWhere = ObjectRelationshipInfoProvider.GetWhereCondition(ObjectID, ObjectType, 0, false, true, selectedObjectType);
                            rightWhere += " AND RelationshipLeftObjectID IN (" + siteQuery + ")";

                            // Where condition for the left object
                            string leftWhere = ObjectRelationshipInfoProvider.GetWhereCondition(ObjectID, ObjectType, 0, true, false, selectedObjectType);
                            leftWhere += " AND RelationshipRightObjectID IN (" + siteQuery + ")";

                            // --- Add site conditions here

                            where = SqlHelper.AddWhereCondition(leftWhere, rightWhere, "OR");
                        }
                    }
                }
            }

            if (String.IsNullOrEmpty(where))
            {
                // Get using regular where
                where = ObjectRelationshipInfoProvider.GetWhereCondition(ObjectID, ObjectType, 0, true, true, selectedObjectType);
            }

            return where;
        }

        return null;
    }


    /// <summary>
    /// Fills the given drop-down list with the available relationship types.
    /// </summary>
    private void DisplayAvailableRelationships()
    {
        if (drpRelationship.Items.Count == 0)
        {
            // Get the relationships from DB
            DataSet ds = RelationshipNameInfoProvider.GetRelationshipNames(SiteContext.CurrentSiteID, "RelationshipAllowedObjects LIKE '%;##OBJECTS##;%'");
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    drpRelationship.Items.Add(new ListItem(dr["RelationshipDisplayName"].ToString(), dr["RelationshipNameID"].ToString()));
                }

                drpRelationship.Enabled = true;
            }
            else
            {
                drpRelationship.Items.Add(new ListItem(GetString("General.NoneAvailable"), ""));
                drpRelationship.Enabled = false;
            }
        }
    }


    /// <summary>
    /// Fills given drop-down list with the items of particular type.
    /// </summary>
    private void DisplayAvailableItems()
    {
        var relatedObject = RelatedObject;
        if (relatedObject != null)
        {
            // Prepare the site where
            string where = null;

            var relatedTypeInfo = relatedObject.TypeInfo;
            if (relatedTypeInfo.SiteIDColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN)
            {
                int selectedSiteId = ValidationHelper.GetInteger(siteSelector.Value, -1);

                switch (selectedSiteId)
                {
                    case -1:
                        // Select all objects
                        break;

                    case 0:
                        where = relatedTypeInfo.SiteIDColumn + " IS NULL";
                        break;

                    default:
                        where = relatedTypeInfo.SiteIDColumn + " = " + selectedSiteId;
                        break;
                }
            }

            // Load the object selectors
            if (ActiveLeft)
            {
                // Active left selector
                selLeftObj.Enabled = true;
                selLeftObj.ObjectType = relatedTypeInfo.ObjectType;
                selLeftObj.WhereCondition = where;
                selLeftObj.Reload(true);

                if (!selLeftObj.HasData)
                {
                    selLeftObj.DropDownSingleSelect.Items.Add(new ListItem(GetString("General.NoneAvailable"), ""));
                    selLeftObj.Enabled = false;
                }
            }
            else
            {
                // Active right selector
                selRightObj.Enabled = true;
                selRightObj.ObjectType = relatedTypeInfo.ObjectType;
                selRightObj.WhereCondition = where;
                selRightObj.Reload(true);

                if (!selRightObj.HasData)
                {
                    selRightObj.DropDownSingleSelect.Items.Add(new ListItem(GetString("General.NoneAvailable"), ""));
                    selRightObj.Enabled = false;
                }
            }
        }
    }


    /// <summary>
    /// Fills given drop-down list with the available object types.
    /// </summary>
    private void DisplayAvailableObjects()
    {
        // All selection
        ListItem li = new ListItem(ResHelper.GetString("General.SelectAnything"), "");
        drpRelatedObjType.DropDownList.Items.Insert(0, li);

        // Preselect
        foreach (ListItem item in drpRelatedObjType.DropDownList.Items)
        {
            string objType = item.Value;

            // Preselect the same object type as the main object if available
            if (!RequestHelper.IsPostBack() && objType.EqualsCSafe(ObjectType, true))
            {
                item.Selected = true;
            }
        }
    }



    /// <summary>
    /// Gets the transformation to display object name
    /// </summary>
    /// <param name="parameter">Row parameter</param>
    /// <param name="left">If true, the object is served from the left object</param>
    protected ObjectTransformation GetDisplayTransformation(object parameter, bool left)
    {
        DataRowView data = (DataRowView)parameter;

        var colName = (left ? "Left" : "Right");
        var objectTypeColumn = String.Format("Relationship{0}ObjectType", colName);
        var idColumn = String.Format("Relationship{0}ObjectID", colName);

        string objectType = ValidationHelper.GetString(data[objectTypeColumn], "");
        int objectId = ValidationHelper.GetInteger(data[idColumn], 0);

        var tr = new ObjectTransformation(objectType, objectId);

        bool identity = ((Object.TypeInfo.ObjectType == objectType) && (Object.ObjectID == objectId));
        string format = "{{% Object.GetFullObjectName(true, {0}) %}}";
        if (identity)
        {
            //format += " (this)";
        }

        tr.Transformation = String.Format(format, (pnlSite.Visible && !identity ? "false" : "true"));
        tr.NoDataTransformation = GetString("General.NotFound");

        return tr;
    }

    #endregion
}