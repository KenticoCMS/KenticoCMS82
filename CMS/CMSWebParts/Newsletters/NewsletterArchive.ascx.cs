using System;

using CMS.Helpers;
using CMS.Newsletters;
using CMS.PortalControls;
using CMS.SiteProvider;

public partial class CMSWebParts_Newsletters_NewsletterArchive : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the newsletter code name.
    /// </summary>
    public string NewsletterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NewsletterName"), "");
        }
        set
        {
            SetValue("NewsletterName", value);
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the results.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TransformationName"), "cms.root.newsletter_archive");
        }
        set
        {
            SetValue("TransformationName", value);
            repNewsArchive.TransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether only issues where mailout time is bigger than current time can be selected.
    /// </summary>
    public bool SelectOnlySendedIssues
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlySendedIssues"), false);
        }
        set
        {
            SetValue("SelectOnlySendedIssues", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether value 'IgnoreShowInNewsletterArchive' for select issues will be ignored.
    /// </summary>
    public bool IgnoreShowInNewsletterArchive
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("IgnoreShowInNewsletterArchive"), false);
        }
        set
        {
            SetValue("IgnoreShowInNewsletterArchive", value);
        }
    }


    /// <summary>
    /// Gets or sets the number which indicates how many issues should be displayed.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), -1);
        }
        set
        {
            SetValue("SelectTopN", value);
            repNewsArchive.TopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the Order By clause.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderBy"), "IssueMailoutTime");
        }
        set
        {
            SetValue("OrderBy", value);
            repNewsArchive.OrderBy = value;
        }
    }

    #endregion


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
            repNewsArchive.StopProcessing = true;
        }
        else
        {
            // Get the newsletter
            NewsletterInfo news = NewsletterInfoProvider.GetNewsletterInfo(NewsletterName, SiteContext.CurrentSiteID);

            if (news != null)
            {
                repNewsArchive.ControlContext = ControlContext;
                repNewsArchive.QueryName = "newsletter.issue.selectall";

                // Get newsletter's issues that are not A/B tests or finished A/B test winners
                string where = string.Format("(IssueNewsletterID = {0}) AND (IssueIsABTest=0 OR IssueIsABTest IS NULL OR (IssueIsABTest=1 AND IssueVariantOfIssueID IS NULL AND (IssueStatus={1} OR IssueStatus={2})))",
                    news.NewsletterID, (int)IssueStatusEnum.Sending, (int)IssueStatusEnum.Finished);

                if (!IgnoreShowInNewsletterArchive)
                {
                    where += " AND (IssueShowInNewsletterArchive = 1)";
                }

                if (SelectOnlySendedIssues)
                {
                    where += string.Format(" AND (IssueMailoutTime IS NOT NULL) AND (IssueMailoutTime < getDate()) AND (IssueStatus={0} OR IssueStatus={1})", (int)IssueStatusEnum.ReadyForSending, (int)IssueStatusEnum.Finished);
                }

                repNewsArchive.WhereCondition = where;
                repNewsArchive.OrderBy = OrderBy;
                repNewsArchive.TopN = SelectTopN;
                repNewsArchive.TransformationName = TransformationName;
                
                // Set caching properties
                repNewsArchive.CacheMinutes = CacheMinutes;
                repNewsArchive.CacheItemName = CacheItemName;
                repNewsArchive.CacheDependencies = CacheDependencies;
            }
        }
    }


    /// <summary>
    /// Reloads the data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
        repNewsArchive.ReloadData(true);
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        repNewsArchive.ClearCache();
    }


    /// <summary>
    /// OnPreRender override.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        Visible = !StopProcessing;

        if (!repNewsArchive.HasData())
        {
            Visible = false;
        }
        base.OnPreRender(e);
    }
}