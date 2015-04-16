using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;

using CMS.Helpers;
using CMS.PortalControls;


public partial class CMSWebParts_SharePoint_SharePointDataSource : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets value of username.
    /// </summary>
    public string Username
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Username"), SPDataSource.Username);
        }
        set
        {
            SetValue("Username", value);
            SPDataSource.Username = value;
        }
    }


    /// <summary>
    /// Gets or sets value of password.
    /// </summary>
    public string Password
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Password"), SPDataSource.Password);
        }
        set
        {
            SetValue("Password", value);
            SPDataSource.Password = value;
        }
    }


    /// <summary>
    /// Gets or sets value of SharePoint list name.
    /// </summary>
    public string ListName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ListName"), SPDataSource.ListName);
        }
        set
        {
            SetValue("ListName", value);
            SPDataSource.ListName = value;
        }
    }


    /// <summary>
    /// Gets or sets URL of SharePoint service (Eg. Lists.asmx, Imaging.asmx).
    /// </summary>
    public string SPServiceURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SPServiceURL"), SPDataSource.SPServiceURL);
        }
        set
        {
            SetValue("SPServiceURL", value);
            SPDataSource.SPServiceURL = value;
        }
    }


    /// <summary>
    /// Enables or disables showing CAML on output.
    /// </summary>
    public bool ShowReturnedCAML
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowReturnedCAML"), SPDataSource.ShowReturnedCAML);
        }
        set
        {
            SetValue("ShowReturnedCAML", value);
            SPDataSource.ShowReturnedCAML = value;
        }
    }


    /// <summary>
    /// Gets or set the row limit.
    /// </summary>
    public int RowLimit
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("RowLimit"), SPDataSource.RowLimit);
        }
        set
        {
            SetValue("RowLimit", value);
            SPDataSource.RowLimit = value;
        }
    }


    /// <summary>
    /// Gets or sets query to specify which document should be retrieved (like where condition).
    /// </summary>
    public string Query
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Query"), SPDataSource.Query);
        }
        set
        {
            SetValue("Query", value);
            SPDataSource.Query = value;
        }
    }


    /// <summary>
    /// Gets or sets document fields which should be retrieved.
    /// </summary>
    public string ViewFields
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ViewFields"), SPDataSource.ViewFields);
        }
        set
        {
            SetValue("ViewFields", value);
            SPDataSource.ViewFields = value;
        }
    }


    /// <summary>
    /// Enables or disables using of classic dataset as data source for ASCX transformation.
    /// </summary>
    public bool UseClassicDataset
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseClassicDataset"), SPDataSource.UseClassicDataset);
        }
        set
        {
            SetValue("UseClassicDataset", value);
            SPDataSource.UseClassicDataset = value;
        }
    }


    /// <summary>
    /// Gets or sets fields which should be included in dataset
    /// Note: Only if UseClassicDataset is enabled
    /// </summary>
    public string Fields
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Fields"), SPDataSource.Fields);
        }
        set
        {
            SetValue("Fields", value);
            SPDataSource.Fields = value;
        }
    }


    /// <summary>
    /// Gets or sets the mode which specifies what this webpart exactly do.
    /// </summary>
    public string Mode
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Mode"), SPDataSource.Mode);
        }
        set
        {
            SetValue("Mode", value);
            SPDataSource.Mode = value;
        }
    }


    /// <summary>
    /// Gets or sets query string key name. Presence of the key in query string indicates, 
    /// that some item should be selected. The item is determined by query string value.        
    /// </summary>
    public string ItemIDField
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ItemIDField"), SPDataSource.ItemIDField);
        }
        set
        {
            SetValue("ItemIDField", value);
            SPDataSource.ItemIDField = value;
        }
    }


    /// <summary>
    /// Gets or sets the field name which is used for selecting item. Case sensitive!
    /// </summary>
    public string SelectedItemFieldName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedItemFieldName"), SPDataSource.SelectedItemFieldName);
        }
        set
        {
            SetValue("SelectedItemFieldName", value);
            SPDataSource.SelectedItemFieldName = value;
        }
    }


    /// <summary>
    /// Gets or sets the type of field for selecting item.
    /// </summary>
    public string ItemIDFieldType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ItemIDFieldType"), SPDataSource.ItemIDFieldType);
        }
        set
        {
            SetValue("ItemIDFieldType", value);
            SPDataSource.ItemIDFieldType = value;
        }
    }


    /// <summary>
    /// Gets the value that indicates whether current datasource contains selected item.
    /// </summary>
    public bool IsSelected
    {
        get
        {
            return SPDataSource.IsSelected;
        }
    }

    #endregion


    #region "Overridden properties"

    /// <summary>
    /// Gets or sets the cache item name.
    /// </summary>
    public override string CacheItemName
    {
        get
        {
            return base.CacheItemName;
        }
        set
        {
            base.CacheItemName = value;
            SPDataSource.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, SPDataSource.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            SPDataSource.CacheDependencies = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache minutes.
    /// </summary>
    public override int CacheMinutes
    {
        get
        {
            return base.CacheMinutes;
        }
        set
        {
            base.CacheMinutes = value;
            SPDataSource.CacheMinutes = value;
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
            SPDataSource.StopProcessing = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            // Set datasource properties            
            SPDataSource.Username = Username;
            SPDataSource.Password = EncryptionHelper.DecryptData(Password);
            SPDataSource.ListName = ListName;
            SPDataSource.SPServiceURL = SPServiceURL;
            SPDataSource.ShowReturnedCAML = ShowReturnedCAML;
            SPDataSource.RowLimit = RowLimit;
            SPDataSource.Query = Query;
            SPDataSource.ViewFields = ViewFields;
            SPDataSource.UseClassicDataset = UseClassicDataset;
            SPDataSource.Fields = Fields;
            SPDataSource.Mode = Mode;
            SPDataSource.ItemIDField = ItemIDField;
            SPDataSource.SelectedItemFieldName = SelectedItemFieldName;
            SPDataSource.ItemIDFieldType = ItemIDFieldType;
            SPDataSource.CacheItemName = CacheItemName;
            SPDataSource.CacheDependencies = CacheDependencies;
            SPDataSource.CacheMinutes = CacheMinutes;

            // Set datasource control filter name = this webpart name
            SPDataSource.FilterName = ValidationHelper.GetString(GetValue("WebPartControlID"), ID);
        }
    }

    #endregion
}