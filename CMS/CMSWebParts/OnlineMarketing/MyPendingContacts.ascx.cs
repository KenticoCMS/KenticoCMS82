using System;

using CMS.Helpers;
using CMS.PortalControls;

public partial class CMSWebParts_OnlineMarketing_MyPendingContacts : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the site name. If is empty, documents from all sites are displayed.
    /// </summary>
    public int SiteID
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Site"), 0);
        }
        set
        {
            SetValue("Site", value);
        }
    }


    /// <summary>
    /// Gets or sets the value of items per page.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), 10);
        }
        set
        {
            SetValue("PageSize", value);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            pendingContacts.StopProcessing = true;
        }
        else
        {
            pendingContacts.SiteID = SiteID;
            pendingContacts.IsWidget = true;
            pendingContacts.PageSize = PageSize;
        }
    }

    #endregion
}