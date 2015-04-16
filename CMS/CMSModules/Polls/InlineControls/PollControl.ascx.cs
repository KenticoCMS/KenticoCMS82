using System;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Polls;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DataEngine;

public partial class CMSModules_Polls_InlineControls_PollControl : InlineUserControl
{
    /// <summary>
    /// Poll code name.
    /// </summary>
    public string PollName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PollName"), null);
        }
        set
        {
            SetValue("PollName", value);
            PollView1.PollCodeName = value;
        }
    }


    /// <summary>
    /// Poll site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SiteName"), null);
        }
        set
        {
            SetValue("SiteName", value);
            PollView1.PollSiteID = GetSiteID();
        }
    }


    /// <summary>
    /// Poll group name.
    /// </summary>
    public string GroupName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("GroupName"), null);
        }
        set
        {
            SetValue("GroupName", value);
            PollView1.PollGroupID = GetGroupID();
        }
    }


    /// <summary>
    /// Type of the representation of the answers count in the graph.
    /// </summary>
    public CountTypeEnum CountType
    {
        get
        {
            string value = ValidationHelper.GetString(GetValue("CountType"), "absolute");
            switch (value.ToLowerCSafe())
            {
                case "none":
                case "0":
                    return CountTypeEnum.None;
                case "percentage":
                case "2":
                    return CountTypeEnum.Percentage;
                default:
                    return CountTypeEnum.Absolute;
            }
        }
        set
        {
            SetValue("CountType", value.ToString().ToLowerCSafe());
            PollView1.CountType = value;
        }
    }


    /// <summary>
    /// Control parameter.
    /// </summary>
    public override string Parameter
    {
        get
        {
            return PollName;
        }
        set
        {
            PollName = value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        PollView1.PollCodeName = PollName;
        PollView1.PollSiteID = GetSiteID();
        PollView1.PollGroupID = GetGroupID();
        PollView1.CountType = CountType;
    }


    /// <summary>
    /// Returns site ID according to site name.
    /// </summary>
    protected int GetSiteID()
    {
        if (!string.IsNullOrEmpty(SiteName))
        {
            // Get site object
            SiteInfo site = SiteInfoProvider.GetSiteInfo(SiteName);
            if (site != null)
            {
                return site.SiteID;
            }
        }

        return 0;
    }


    /// <summary>
    /// Returns group ID according to group name.
    /// </summary>
    protected int GetGroupID()
    {
        if (!string.IsNullOrEmpty(GroupName))
        {
            // Get group object
            GeneralizedInfo group = ModuleCommands.CommunityGetGroupInfoByName(GroupName, SiteName);
            if (group != null)
            {
                // Get ID column value
                return ValidationHelper.GetInteger(group.GetValue(group.TypeInfo.IDColumn), 0);
            }
        }

        return 0;
    }
}