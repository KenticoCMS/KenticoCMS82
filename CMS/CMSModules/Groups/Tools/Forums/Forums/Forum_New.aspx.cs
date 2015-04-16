using System;

using CMS.Forums;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;

[EditedObject(ForumInfo.OBJECT_TYPE_GROUP, "forumId")]
public partial class CMSModules_Groups_Tools_Forums_Forums_Forum_New : CMSGroupForumPage
{
    protected int forumGroupId = 0;
    protected int groupId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        groupId = QueryHelper.GetInteger("groupid", 0);
        forumGroupId = QueryHelper.GetInteger("forumgroupid", 0);
        forumNew.GroupID = forumGroupId;
        forumNew.OnSaved += forumNew_OnSaved;
        forumNew.OnCheckPermissions += forumNew_OnCheckPermissions;
        forumNew.IsLiveSite = false;

        InitializeMasterPage();
    }


    private void forumNew_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        int lGroupId = 0;

        ForumGroupInfo fgi = ForumGroupInfoProvider.GetForumGroupInfo(forumNew.GroupID);
        if (fgi != null)
        {
            lGroupId = fgi.GroupGroupID;
        }

        CheckPermissions(lGroupId, CMSAdminControl.PERMISSION_MANAGE);
    }


    private void forumNew_OnSaved(object sender, EventArgs e)
    {
        string url = UIContextHelper.GetElementUrl("cms.forums", "EditGroupForum");
        url = URLHelper.AddParameterToUrl(url, "forumid", forumNew.ForumID.ToString());
        url = URLHelper.AddParameterToUrl(url, "parentobjectid", forumNew.ForumGroup.GroupID.ToString());
        URLHelper.Redirect(URLHelper.AddParameterToUrl(URLHelper.AddParameterToUrl(URLHelper.AddParameterToUrl(UIContextHelper.GetElementUrl("cms.forums", "EditGroupForum", false), "forumid", Convert.ToString(forumNew.ForumID)), "parentobjectid", Convert.ToString(forumNew.ForumGroup.GroupID)), "saved", "1"));
    }


    /// <summary>
    /// Initializes Master Page.
    /// </summary>
    protected void InitializeMasterPage()
    {
        // Initialize help 
        Title = "Forums - New forum";

        // Initialize breadcrumbs
        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("forum_list.headercaption"),
            RedirectUrl = URLHelper.AddParameterToUrl(UIContextHelper.GetElementUrl("cms.forums","GroupForumGroupEditTab_Forums", false), "parentobjectid", forumGroupId.ToString()),
        });
        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("Forum_Edit.NewForum"),
        });
    }
}