using System;
using System.Data;

using CMS.CustomTables;
using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSAPIExamples_Code_Development_CustomTables_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // CustomTableItem
        apiCreateCustomTableItem.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateCustomTableItem);
        apiGetAndUpdateCustomTableItem.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateCustomTableItem);
        apiGetAndMoveCustomTableItemDown.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndMoveCustomTableItemDown);
        apiGetAndMoveCustomTableItemUp.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndMoveCustomTableItemUp);
        apiGetAndBulkUpdateCustomTableItems.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateCustomTableItems);
        apiDeleteCustomTableItem.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteCustomTableItem);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Custom table item
        apiCreateCustomTableItem.Run();
        apiGetAndUpdateCustomTableItem.Run();
        apiGetAndMoveCustomTableItemDown.Run();
        apiGetAndMoveCustomTableItemUp.Run();
        apiGetAndBulkUpdateCustomTableItems.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Custom table item
        apiDeleteCustomTableItem.Run();
    }

    #endregion


    #region "API examples - CustomTableItem"

    /// <summary>
    /// Creates custom table item. Called when the "Create item" button is pressed.
    /// </summary>
    private bool CreateCustomTableItem()
    {
        // Prepare the parameters
        string customTableClassName = "customtable.sampletable";

        // Check if Custom table 'Sample table' exists
        DataClassInfo customTable = DataClassInfoProvider.GetDataClassInfo(customTableClassName);
        if (customTable != null)
        {
            // Create new custom table item 
            CustomTableItem newCustomTableItem = CustomTableItem.New(customTableClassName);

            // Set the ItemText field value
            newCustomTableItem.SetValue("ItemText", "New text");

            // Insert the custom table item into database
            newCustomTableItem.Insert();

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates custom table item. Called when the "Get and update item" button is pressed.
    /// Expects the CreateCustomTableItem method to be run first.
    /// </summary>
    private bool GetAndUpdateCustomTableItem()
    {
        string customTableClassName = "customtable.sampletable";

        // Check if Custom table 'Sample table' exists
        DataClassInfo customTable = DataClassInfoProvider.GetDataClassInfo(customTableClassName);
        if (customTable != null)
        {
            // Prepare the parameters 
            string where = "ItemText LIKE N'New text'";
            int topN = 1;
            string columns = "ItemID";

            // Get the data set according to the parameters 
            DataSet dataSet = CustomTableItemProvider.GetItems(customTableClassName, where, null, topN, columns);

            if (!DataHelper.DataSourceIsEmpty(dataSet))
            {
                // Get the custom table item ID
                int itemID = ValidationHelper.GetInteger(dataSet.Tables[0].Rows[0][0], 0);

                // Get the custom table item
                CustomTableItem updateCustomTableItem = CustomTableItemProvider.GetItem(itemID, customTableClassName);

                if (updateCustomTableItem != null)
                {
                    string itemText = ValidationHelper.GetString(updateCustomTableItem.GetValue("ItemText"), "");

                    // Set new values
                    updateCustomTableItem.SetValue("ItemText", itemText.ToLowerCSafe());

                    // Save the changes
                    updateCustomTableItem.Update();

                    return true;
                }
            }
        }

        return false;
    }


    /// <summary>
    /// Gets the custom table item and moves it down. Called when the "Get and move item down" button is pressed.
    /// Expects the CreateCustomTableItem method to be run first.
    /// </summary>
    private bool GetAndMoveCustomTableItemDown()
    {
        string customTableClassName = "customtable.sampletable";

        // Check if Custom table 'Sample table' exists
        DataClassInfo customTable = DataClassInfoProvider.GetDataClassInfo(customTableClassName);
        if (customTable != null)
        {
            // Prepare the parameters
            string where = "ItemText LIKE N'New text'";
            int topN = 1;
            string columns = "ItemID";

            // Get the data set according to the parameters 
            DataSet dataSet = CustomTableItemProvider.GetItems(customTableClassName, where, null, topN, columns);

            if (!DataHelper.DataSourceIsEmpty(dataSet))
            {
                // Get the custom table item ID
                int itemID = ValidationHelper.GetInteger(dataSet.Tables[0].Rows[0][0], 0);
                var item = CustomTableItemProvider.GetItem(itemID, customTableClassName);
                
                // Move the item down
                item.Generalized.MoveObjectDown();

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Gets the custom table item and moves it up. Called when the "Get and move item up" button is pressed.
    /// Expects the CreateCustomTableItem method to be run first.
    /// </summary>
    private bool GetAndMoveCustomTableItemUp()
    {
        string customTableClassName = "customtable.sampletable";

        // Check if Custom table 'Sample table' exists
        DataClassInfo customTable = DataClassInfoProvider.GetDataClassInfo(customTableClassName);
        if (customTable != null)
        {
            // Prepare the parameters
            string where = "ItemText LIKE N'New text'";
            int topN = 1;
            string columns = "ItemID";

            // Get the data set according to the parameters 
            DataSet dataSet = CustomTableItemProvider.GetItems(customTableClassName, where, null, topN, columns);

            if (!DataHelper.DataSourceIsEmpty(dataSet))
            {
                // Get the custom table item ID
                int itemID = ValidationHelper.GetInteger(dataSet.Tables[0].Rows[0][0], 0);
                var item = CustomTableItemProvider.GetItem(itemID, customTableClassName);
                
                // Move the item up
                item.Generalized.MoveObjectUp();

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates custom table items. Called when the "Get and bulk update items" button is pressed.
    /// Expects the CreateCustomTableItem method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateCustomTableItems()
    {
        string customTableClassName = "customtable.sampletable";

        // Check if Custom table 'Sample table' exists
        DataClassInfo customTable = DataClassInfoProvider.GetDataClassInfo(customTableClassName);
        if (customTable != null)
        {
            // Prepare the parameters

            string where = "ItemText LIKE N'New text%'";

            // Get the data
            DataSet customTableItems = CustomTableItemProvider.GetItems(customTableClassName, where);
            if (!DataHelper.DataSourceIsEmpty(customTableItems))
            {
                // Loop through the individual items
                foreach (DataRow customTableItemDr in customTableItems.Tables[0].Rows)
                {
                    // Create object from DataRow
                    CustomTableItem modifyCustomTableItem = CustomTableItem.New(customTableClassName, customTableItemDr);

                    string itemText = ValidationHelper.GetString(modifyCustomTableItem.GetValue("ItemText"), "");

                    // Set new values
                    modifyCustomTableItem.SetValue("ItemText", itemText.ToUpper());

                    // Save the changes
                    modifyCustomTableItem.Update();
                }

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Deletes customTableItem. Called when the "Delete item" button is pressed.
    /// Expects the CreateCustomTableItem method to be run first.
    /// </summary>
    private bool DeleteCustomTableItem()
    {
        string customTableClassName = "customtable.sampletable";

        // Check if Custom table 'Sample table' exists
        DataClassInfo customTable = DataClassInfoProvider.GetDataClassInfo(customTableClassName);
        if (customTable != null)
        {
            // Prepare the parameters
            string where = "ItemText LIKE N'New text%'";

            // Get the data
            DataSet customTableItems = CustomTableItemProvider.GetItems(customTableClassName, where);
            if (!DataHelper.DataSourceIsEmpty(customTableItems))
            {
                // Loop through the individual items
                foreach (DataRow customTableItemDr in customTableItems.Tables[0].Rows)
                {
                    // Create object from DataRow
                    CustomTableItem deleteCustomTableItem = CustomTableItem.New(customTableClassName, customTableItemDr);

                    // Delete custom table item from database
                    deleteCustomTableItem.Delete();
                }

                return true;
            }
        }

        return false;
    }

    #endregion
}