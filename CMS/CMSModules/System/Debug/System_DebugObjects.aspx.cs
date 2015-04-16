using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_System_Debug_System_DebugObjects : CMSDebugPage
{
    protected int index = 0;

    protected int totalObjects = 0;
    protected int totalTableObjects = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        gridObjects.Columns[0].HeaderText = GetString("General.ObjectType");
        gridObjects.Columns[1].HeaderText = GetString("General.Count");

        gridHashtables.Columns[0].HeaderText = GetString("Administration-System.CacheName");
        gridHashtables.Columns[1].HeaderText = GetString("General.Count");

        btnClear.Text = GetString("Administration-System.ClearHashtables");

        ReloadData();
    }


    protected void ReloadData()
    {
        DataTable dt;

        try
        {
            // Load the dictionaries
            dt = LoadDictionaries();
        }
        catch
        {
            // Load the dictionaries again (in case of collection was modified during the first load)
            dt = LoadDictionaries();
        }

        dt.DefaultView.Sort = "TableName ASC";

        gridHashtables.DataSource = dt.DefaultView;
        gridHashtables.DataBind();

        // Objects
        if (ObjectTypeInfo.TrackObjectInstances)
        {
            dt = new DataTable();
            dt.Columns.Add(new DataColumn("ObjectType", typeof(string)));
            dt.Columns.Add(new DataColumn("ObjectCount", typeof(int)));

            foreach (var typeInfo in ObjectTypeManager.RegisteredTypes)
            {
                var instances = typeInfo.InstanceCount;
                if (instances > 0)
                {
                    DataRow dr = dt.NewRow();
                    dr["ObjectType"] = typeInfo.ObjectType;

                    // Get the instances
                    dr["ObjectCount"] = instances;

                    dt.Rows.Add(dr);
                }
            }

            dt.DefaultView.Sort = "ObjectType ASC";

            gridObjects.DataSource = dt.DefaultView;
            gridObjects.DataBind();
        }
    }


    /// <summary>
    /// Loads the dictionaries to the data table
    /// </summary>
    private DataTable LoadDictionaries()
    {
        DataTable dt = new DataTable();

        dt.Columns.Add(new DataColumn("TableName", typeof(string)));
        dt.Columns.Add(new DataColumn("ObjectCount", typeof(int)));

        lock (AbstractProviderDictionary.Dictionaries)
        {
            // Hashtables
            foreach (DictionaryEntry item in AbstractProviderDictionary.Dictionaries)
            {
                var dict = item.Value as IProviderDictionary;
                if (dict != null)
                {
                    var count = dict.GetRealCount();
                    if (count > 0)
                    {
                        DataRow dr = dt.NewRow();

                        dr["TableName"] = AbstractProviderDictionary.GetDictionaryDisplayName(dict);
                        dr["ObjectCount"] = count;

                        dt.Rows.Add(dr);
                    }
                }
            }
        }

        return dt;
    }


    protected string GetCount(object count)
    {
        int cnt = ValidationHelper.GetInteger(count, 0);
        totalObjects += cnt;

        return cnt.ToString();
    }


    protected string GetTableCount(object count)
    {
        int cnt = ValidationHelper.GetInteger(count, 0);
        totalTableObjects += cnt;

        return cnt.ToString();
    }


    protected void btnClear_Click(object sender, EventArgs e)
    {
        Functions.ClearHashtables();

        // Collect the memory
        GC.Collect();
        GC.WaitForPendingFinalizers();

        ReloadData();
    }
}