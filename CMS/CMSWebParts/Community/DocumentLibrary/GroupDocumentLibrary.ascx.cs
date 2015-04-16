using CMS.Community;
using CMS.Helpers;
using CMS.PortalControls;
using CMS.SiteProvider;

public partial class CMSWebParts_Community_DocumentLibrary_GroupDocumentLibrary : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Alias path determining parent document of library.
    /// </summary>
    public string LibraryPath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LibraryPath"), libraryElem.LibraryPath);
        }
        set
        {
            SetValue("LibraryPath", value);
            libraryElem.LibraryPath = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether documents are combined with default culture version.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), libraryElem.CombineWithDefaultCulture);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            libraryElem.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Number of displayed documents.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), libraryElem.PageSize);
        }
        set
        {
            SetValue("PageSize", value);
            libraryElem.PageSize = value;
        }
    }


    /// <summary>
    /// Order by clause used for ordering documents.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderBy"), libraryElem.OrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
            libraryElem.OrderBy = value;
        }
    }


    /// <summary>
    /// Specifies the form used for editing document properties.
    /// </summary>
    public string DocumentForm
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DocumentForm"), libraryElem.DocumentForm);
        }
        set
        {
            SetValue("DocumentForm", value);
            libraryElem.DocumentForm = value;
        }
    }


    /// <summary>
    /// Gets or sets group name.
    /// </summary>
    public string GroupName
    {
        get
        {
            string groupName = ValidationHelper.GetString(GetValue("GroupName"), null);

            if ((string.IsNullOrEmpty(groupName) || groupName == GroupInfoProvider.CURRENT_GROUP) && (CommunityContext.CurrentGroup != null))
            {
                return CommunityContext.CurrentGroup.GroupName;
            }

            if (string.IsNullOrEmpty(groupName))
            {
                GroupInfo gi = GroupInfoProvider.GetGroupInfo(libraryElem.GroupID);
                if (gi != null)
                {
                    groupName = gi.GroupName;
                }
            }

            return groupName;
        }
        set
        {
            SetValue("GroupName", value);
            libraryElem.GroupID = GroupID;
        }
    }


    /// <summary>
    /// Gets a group identifier based on group name.
    /// </summary>
    private int GroupID
    {
        get
        {
            GroupInfo gi = GroupInfoProvider.GetGroupInfo(GroupName, SiteContext.CurrentSiteName);
            return (gi != null) ? gi.GroupID : 0;
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
            libraryElem.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, libraryElem.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            libraryElem.CacheDependencies = value;
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
            libraryElem.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed for zero rows result.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ZeroRowsText"), libraryElem.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            libraryElem.ZeroRowsText = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether permissions should be checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), libraryElem.CheckPermissions);
        }
        set
        {
            SetValue("CheckPermissions", value);
        }
    }

    #endregion


    #region "Stop processing"

    /// <summary>
    /// Returns true if the control processing should be stopped.
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
            libraryElem.StopProcessing = value;
        }
    }

    #endregion


    #region "Page events"

    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }

    #endregion


    #region "Methods"

    private void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing and tell inner controls to stop processing
            libraryElem.StopProcessing = true;
        }
        else
        {
            // Initialize document library control
            libraryElem.LibraryPath = LibraryPath;
            libraryElem.CombineWithDefaultCulture = CombineWithDefaultCulture;
            libraryElem.PageSize = PageSize;
            libraryElem.OrderBy = OrderBy;
            libraryElem.DocumentForm = DocumentForm;
            libraryElem.GroupID = GroupID;
            libraryElem.CacheDependencies = CacheDependencies;
            libraryElem.CacheItemName = CacheItemName;
            libraryElem.CacheMinutes = CacheMinutes;
            libraryElem.ZeroRowsText = ZeroRowsText;
            libraryElem.CheckPermissions = CheckPermissions;
            libraryElem.ComponentName = WebPartID;
        }
    }

    #endregion
}