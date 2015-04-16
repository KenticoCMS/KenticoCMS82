using System;
using System.Data;
using System.Collections;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Core;
using CMS.UIControls;
using CMS.DataEngine;

[UIElement(ModuleName.BLOGS, "MyBlogsComments")]
public partial class CMSModules_Blogs_MyBlogs_MyBlogs_Comments_List : CMSContentManagementPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // No cms.blog doc. type
        if (DataClassInfoProvider.GetDataClassInfo("cms.blog") == null)
        {
            RedirectToInformation(GetString("blog.noblogdoctype"));
        }
    }
}