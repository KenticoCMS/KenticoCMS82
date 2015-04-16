using System;
using System.Web;
using System.Web.UI;

using CMS.UIControls;

public partial class CMSTemplates_CorporateSite_Root : TemplateMasterPage
{
    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        PageManager = manPortal;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ltlTags.Text = HeaderTags;
    }
}