using System;
using System.Linq;

using CMS.Helpers;
using CMS.Newsletters;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSModules_Newsletters_Controls_TemplateFlatSelector : CMSAdminControl
{
    #region "Variables"

    private int mNewsletterId = 0;
    private int mSiteId = 0;
    private string mTemplateType = EmailTemplateType.Issue;

    #endregion


    #region "Flat selector properties"

    /// <summary>
    /// Gets or sets the newsletter id. Only templates of this newsletter will be diplayed if set.
    /// </summary>
    public int NewsletterId
    {
        get
        {
            return mNewsletterId;
        }
        set
        {
            mNewsletterId = value;
        }
    }


    /// <summary>
    /// Gets or sets selected item (TemplateID) in flat selector.
    /// </summary>
    public string SelectedItem
    {
        get
        {
            return flatElem.SelectedItem;
        }
        set
        {
            flatElem.SelectedItem = value;
        }
    }


    /// <summary>
    /// Gets or sets the site id. Only templates of this site will be diplayed if set.
    /// It is not required if NewsletterId property is set.
    /// </summary>
    public int SiteId
    {
        get
        {
            return mSiteId;
        }
        set
        {
            mSiteId = value;
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
            flatElem.StopProcessing = value;
            EnableViewState = !value;
        }
    }


    /// <summary>
    /// Gets or sets the template type (available in EmailTemplateType class). Issue templates are displayed if not set.
    /// </summary>
    public string TemplateType
    {
        get
        {
            return mTemplateType;
        }
        set
        {
            switch (value)
            {
                case EmailTemplateType.Issue:
                case EmailTemplateType.Subscription:
                case EmailTemplateType.Unsubscription:
                case EmailTemplateType.DoubleOptIn:
                    mTemplateType = value;
                    break;
            }
        }
    }


    /// <summary>
    /// Returns inner instance of UniFlatSelector control.
    /// </summary>
    public UniFlatSelector UniFlatSelector
    {
        get
        {
            return flatElem;
        }
    }


    /// <summary>
    /// Gets or sets name of javascript function which is used for passing selected item out of selector.
    /// </summary>
    public string SelectFunction
    {
        get
        {
            return flatElem.SelectFunction;
        }
        set
        {
            flatElem.SelectFunction = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        ScriptHelper.RegisterJQuery(Page);

        // Setup flat selector
        flatElem.QueryName = "newsletter.emailtemplate.selectall";
        flatElem.ValueColumn = "TemplateID";
        flatElem.SearchLabelResourceString = "newslettertemplate.templatename";
        flatElem.SearchColumn = "TemplateDisplayName";
        flatElem.SelectedColumns = "TemplateID, TemplateDisplayName, TemplateThumbnailGUID";
        flatElem.PageSize = 15;
        flatElem.OrderBy = "TemplateDisplayName";
        flatElem.NotAvailableImageUrl = GetImageUrl("Objects/Newsletter_EmailTemplate/notavailable.png");
        flatElem.NoRecordsMessage = "newslettertemplate.norecords";
        flatElem.NoRecordsSearchMessage = "newslettertemplate.norecordsfound";

        // Select templates of specified type
        flatElem.WhereCondition = "TemplateType='" + TemplateType + "'";
        if (NewsletterId > 0)
        {
            // Select templates that are in default newsletter configuration, assigned to the newsletter or available to all newsletters on specified site
            flatElem.WhereCondition = SqlHelper.AddWhereCondition(flatElem.WhereCondition,
                String.Format("TemplateID IN (SELECT NewsletterTemplateID FROM Newsletter_Newsletter WHERE NewsletterID={0})" +
                " OR TemplateID IN (SELECT TemplateID FROM Newsletter_EmailTemplateNewsletter WHERE NewsletterID={0})", NewsletterId));
        }
        if (SiteId > 0)
        {
            // Select templates from specified site
            flatElem.WhereCondition = SqlHelper.AddWhereCondition(flatElem.WhereCondition, "TemplateSiteID=" + SiteContext.CurrentSiteID);
        }

        flatElem.OnItemSelected += new UniFlatSelector.ItemSelectedEventHandler(flatElem_OnItemSelected);
    }


    /// <summary>
    /// On PreRender.
    /// </summary>    
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (StopProcessing)
        {
            return;
        }

        // Description area
        litCategory.Text = ShowInDescriptionArea(SelectedItem);
    }

    #endregion


    #region "Event handling"

    /// <summary>
    /// Updates description after item is selected in flat selector.
    /// </summary>
    protected string flatElem_OnItemSelected(string selectedValue)
    {
        return ShowInDescriptionArea(selectedValue);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        flatElem.ReloadData();
        pnlUpdate.Update();
    }


    /// <summary>
    /// Add a reload script to the page which will update the page size (items count) according to the window size.
    /// </summary>
    /// <param name="forceResize">Indicates whether to invoke resizing of the page before calculating the items count</param>
    public void RegisterRefreshPageSizeScript(bool forceResize)
    {
        flatElem.RegisterRefreshPageSizeScript(forceResize);
    }


    /// <summary>
    /// Generates HTML text to be used in description area.
    /// </summary>
    ///<param name="selectedValue">Selected item for which generate description</param>
    private string ShowInDescriptionArea(string selectedValue)
    {
        string description = String.Empty;

        if (!String.IsNullOrEmpty(selectedValue))
        {
            int templateId = ValidationHelper.GetInteger(selectedValue, 0);
            var template = EmailTemplateInfoProvider.GetEmailTemplates()
                                                    .WhereEquals("TemplateID", templateId)
                                                    .Columns("TemplateDisplayName", "TemplateSubject")
                                                    .TopN(1)
                                                    .FirstOrDefault();
            if (template != null)
            {
                description = template.TemplateSubject;
            }
        }

        if (!String.IsNullOrEmpty(description))
        {
            return "<div class=\"Description\">" + HTMLHelper.HTMLEncode(description) + "</div>";
        }

        return String.Empty;
    }

    #endregion
}