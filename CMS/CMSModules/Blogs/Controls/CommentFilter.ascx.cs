using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Blogs;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.FormEngine;

public partial class CMSModules_Blogs_Controls_CommentFilter : CMSUserControl
{
    #region "Variables"

    private SiteInfo currentSite = null;
    private CurrentUserInfo currentUser = null;
    private bool mDisplayAllRecord = true;
    private bool mIsInMydesk = false;

    #endregion


    #region "Events"

    /// <summary>
    /// Event which raises when the search button is clicked.
    /// </summary>
    public event EventHandler SearchPerformed;


    /// <summary>
    /// Raises the action performed event.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    protected void RaiseSearchPerformed(Object sender, EventArgs e)
    {
        if (SearchPerformed != null)
        {
            SearchPerformed(sender, e);
        }
    }

    #endregion


    #region "Private property"

    /// <summary>
    /// Cookie key 
    /// </summary>
    private string mSessionKey = "BlogCommentsFilter" + (MembershipContext.AuthenticatedUser == null ? String.Empty : MembershipContext.AuthenticatedUser.UserID.ToString());

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value which determines whether to display (all) record in Blog dropdown.
    /// </summary>
    public bool DisplayAllRecord
    {
        get
        {
            return mDisplayAllRecord;
        }
        set
        {
            mDisplayAllRecord = value;
        }
    }


    /// <summary>
    /// Gets the Blog part of the WHERE conditon.
    /// </summary>
    public string BlogWhereCondition
    {
        get
        {
            string blogWhere = "";

            string val = ValidationHelper.GetString(uniSelector.Value, "");
            if (val == "")
            {
                val = (DisplayAllRecord ? "##ALL##" : "##MYBLOGS##");
            }

            // Blogs dropdownlist
            switch (val)
            {
                case "##ALL##":
                    // If current user isn't Global admin or user with 'Manage' permissions for blogs
                    if (!currentUser.IsAuthorizedPerResource("cms.blog", "Manage"))
                    {
                        blogWhere = "(NodeOwner=" + currentUser.UserID +
                                    " OR (';' + BlogModerators + ';' LIKE N'%;" + SqlHelper.EscapeQuotes(currentUser.UserName) + ";%'))";
                    }
                    break;

                case "##MYBLOGS##":
                    blogWhere = "NodeOwner = " + currentUser.UserID;
                    break;

                default:
                    blogWhere = "BlogID = " + ValidationHelper.GetInteger(uniSelector.Value, 0);
                    break;
            }

            return blogWhere;
        }
    }


    /// <summary>
    /// Gets the Comment part of the WHERE conditon.
    /// </summary>
    public string CommentWhereCondition
    {
        get
        {
            string where = "";

            // Approved dropdownlist
            if (drpApproved.SelectedIndex > 0)
            {
                switch (drpApproved.SelectedValue)
                {
                    case "YES":
                        where += " CommentApproved = 1 AND";
                        break;

                    case "NO":
                        where += " (CommentApproved = 0 OR CommentApproved IS NULL ) AND";
                        break;
                }
            }
            // Spam dropdownlist
            if (drpSpam.SelectedIndex > 0)
            {
                switch (drpSpam.SelectedValue)
                {
                    case "YES":
                        where += " CommentIsSpam = 1 AND";
                        break;

                    case "NO":
                        where += " (CommentIsSpam = 0 OR CommentIsSpam IS NULL) AND";
                        break;
                }
            }
            if (txtUserName.Text.Trim() != "")
            {
                where += " CommentUserName LIKE '%" + txtUserName.Text.Trim().Replace("'", "''") + "%' AND";
            }
            if (txtComment.Text.Trim() != "")
            {
                where += " CommentText LIKE '%" + txtComment.Text.Trim().Replace("'", "''") + "%' AND";
            }
            if (where != "")
            {
                where = where.Remove(where.Length - 4); // 4 = " AND".Length
            }

            return where;
        }
    }


    /// <summary>
    /// Gets the filter query string.
    /// </summary>
    public string FilterQueryString
    {
        get
        {
            return "&blog=" + URLHelper.URLEncode(ValidationHelper.GetString(uniSelector.Value, String.Empty)) +
                    "&user=" + HTMLHelper.HTMLEncode(txtUserName.Text) +
                   "&comment=" + HTMLHelper.HTMLEncode(txtComment.Text) +
                   "&approved=" + drpApproved.SelectedItem.Value +
                   "&isspam=" + drpSpam.SelectedItem.Value;
        }
    }


    /// <summary>
    /// Indicates if controls is in MyDesk section.
    /// </summary>
    public bool IsInMydesk
    {
        get
        {
            return mIsInMydesk;
        }
        set
        {
            mIsInMydesk = value;
        }
    }

    #endregion


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        currentSite = SiteContext.CurrentSite;
        currentUser = MembershipContext.AuthenticatedUser;

        bool manageBlogs = false;
        // Check 'Manage' permission
        if (currentUser.IsAuthorizedPerResource("cms.blog", "Manage"))
        {
            manageBlogs = true;
        }

        string where = "(NodeSiteID = " + currentSite.SiteID + ")";
        if (!((currentUser.IsGlobalAdministrator) || (manageBlogs)))
        {
            where += " AND " + BlogHelper.GetBlogsWhere(currentUser.UserID, currentUser.UserName, null);
        }

        // Init Blog selector
        uniSelector.DisplayNameFormat = "{%BlogName%}";
        uniSelector.SelectionMode = SelectionModeEnum.SingleDropDownList;
        uniSelector.WhereCondition = where;
        uniSelector.ReturnColumnName = "BlogID";
        uniSelector.ObjectType = "cms.blog";
        uniSelector.ResourcePrefix = "unisiteselector";
        uniSelector.AllowEmpty = false;
        uniSelector.AllowAll = false;

        // Preselect my blogs
        if (IsInMydesk && !RequestHelper.IsPostBack())
        {
            uniSelector.Value = "##MYBLOGS##";
        }
    }


    protected void uniSelector_OnSpecialFieldsLoaded(object sender, EventArgs e)
    {
        if (DisplayAllRecord)
        {
            uniSelector.SpecialFields.Add(new SpecialField() { Text = GetString("general.selectall"), Value = "##ALL##" });
        }

        uniSelector.SpecialFields.Add(new SpecialField() { Text = GetString("myblogs.comments.selectmyblogs"), Value = "##MYBLOGS##" });
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            Visible = false;
            uniSelector.StopProcessing = true;
        }
        else
        {
            btnFilter.Text = GetString("General.Search");

            if (!RequestHelper.IsPostBack())
            {
                // Fill dropdowns
                HandleDropdowns();

                // Preselect filter data
                PreselectFilter();
            }
        }
    }


    protected void btnFilter_Click(object sender, EventArgs e)
    {
        SessionHelper.SetValue(mSessionKey, FilterQueryString);
        RaiseSearchPerformed(null, null);
    }


    protected void HandleDropdowns()
    {
        // Filter approved dropdown
        drpApproved.Items.Add(new ListItem(GetString("general.selectall"), "ALL"));
        drpApproved.Items.Add(new ListItem(GetString("general.yes"), "YES"));
        drpApproved.Items.Add(new ListItem(GetString("general.no"), "NO"));

        drpApproved.SelectedValue = QueryHelper.GetString("approved", (IsInMydesk ? "NO" : "ALL"));

        // Filter spam dropdown
        drpSpam.Items.Add(new ListItem(GetString("general.selectall"), "ALL"));
        drpSpam.Items.Add(new ListItem(GetString("general.yes"), "YES"));
        drpSpam.Items.Add(new ListItem(GetString("general.no"), "NO"));
    }


    /// <summary>
    /// Gets the information on last selected filter configuration and pre-selects the actual values.
    /// </summary>
    private void PreselectFilter()
    {
        string queryString = ValidationHelper.GetString(SessionHelper.GetValue(mSessionKey), "");
        string username = QueryHelper.GetString("user", "");
        if (String.IsNullOrEmpty(username))
        {
            username = URLHelper.GetQueryValue(queryString, "user");
        }
        string comment = QueryHelper.GetString("comment", "");
        if (String.IsNullOrEmpty(comment))
        {
            comment = URLHelper.GetQueryValue(queryString, "comment");
        }
        string approved = QueryHelper.GetString("approved", "");
        if (String.IsNullOrEmpty(approved))
        {
            approved = URLHelper.GetQueryValue(queryString, "approved");
        }
        string isspam = QueryHelper.GetString("isspam", "");
        if (String.IsNullOrEmpty(isspam))
        {
            isspam = URLHelper.GetQueryValue(queryString, "isspam");
        }
        string blog = QueryHelper.GetString("blog", "");
        if (String.IsNullOrEmpty(blog))
        {
            blog = URLHelper.GetQueryValue(queryString, "blog");
        }

        if (username != "")
        {
            txtUserName.Text = username;
        }

        if (comment != "")
        {
            txtComment.Text = comment;
        }

        if (approved != "")
        {
            if (drpApproved.Items.Count > 0)
            {
                drpApproved.SelectedValue = approved;
            }
        }

        if (isspam != "")
        {
            if (drpSpam.Items.Count > 0)
            {
                drpSpam.SelectedValue = isspam;
            }
        }

        if (!String.IsNullOrEmpty(blog))
        {
            uniSelector.Value = URLHelper.URLDecode(blog);
        }
    }
}