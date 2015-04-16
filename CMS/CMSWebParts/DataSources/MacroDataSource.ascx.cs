using CMS.Helpers;
using CMS.PortalControls;

public partial class CMSWebParts_DataSources_MacroDataSource : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Macro expression returning data to be provided by the data source..
    /// </summary>
    public string Expression
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Expression"), "");
        }
        set
        {
            this.SetValue("Expression", value);
        }
    }


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
            srcMacro.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, srcMacro.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            srcMacro.CacheDependencies = value;
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
            srcMacro.CacheMinutes = value;
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
            srcMacro.FilterName = ValidationHelper.GetString(GetValue("WebPartControlID"), ID);
            srcMacro.Expression = Expression;
            srcMacro.CacheItemName = CacheItemName;
            srcMacro.CacheDependencies = CacheDependencies;
            srcMacro.CacheMinutes = CacheMinutes;
        }
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        srcMacro.ClearCache();
    }

    #endregion
}