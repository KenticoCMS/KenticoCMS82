using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.FormControls;
using CMS.Helpers;
using CMS.IO;
using CMS.Base;

public partial class CMSModules_Forums_FormControls_ForumLayoutSelector : FormEngineUserControl
{
    #region "Variables"

    private string mValue = string.Empty;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
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
            drpLayouts.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return drpLayouts.SelectedValue;
        }
        set
        {
            mValue = ValidationHelper.GetString(value, "").ToLowerCSafe();
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
    }


    protected override void CreateChildControls()
    {
        base.CreateChildControls();
        if (!StopProcessing)
        {
            ReloadData();
        }
    }

    #endregion


    #region "Private methods"

    private void ReloadData()
    {
        FillDropDownList();

        // Try to preselect value
        if (!string.IsNullOrEmpty(mValue))
        {
            if (drpLayouts.Items.FindByValue(mValue) != null)
            {
                drpLayouts.SelectedValue = mValue;
            }
        }
    }


    private void FillDropDownList()
    {
        // Build path
        string path = SystemContext.WebApplicationPhysicalPath + "\\CMSModules\\Forums\\Controls\\Layouts";

        // Check if path exist
        if (Directory.Exists(path))
        {
            bool customDirExist = false;

            // Get all directories
            string[] directories = Directory.GetDirectories(path);
            foreach (string dir in directories)
            {
                // Get only directory name
                string dirName = dir.Substring(path.Length + 1);
                if (dirName.ToLowerCSafe() != "custom")
                {
                    // Add to dropdown list
                    drpLayouts.Items.Add(new ListItem(dirName, dirName.ToLowerCSafe()));
                }
                else
                {
                    // Custom directory exists
                    customDirExist = true;
                }
            }

            // If custom directory exist
            if (customDirExist)
            {
                // Build path
                path = DirectoryHelper.CombinePath(path, "Custom");
                if (Directory.Exists(path))
                {
                    // Get all custom directories
                    string[] customDirectories = Directory.GetDirectories(path);
                    foreach (string dir in customDirectories)
                    {
                        // Add to the dropdown list
                        string dirName = dir.Substring(path.Length + 1);
                        drpLayouts.Items.Add(new ListItem(dirName, "custom/" + dirName.ToLowerCSafe()));
                    }
                }
            }
        }
    }

    #endregion
}