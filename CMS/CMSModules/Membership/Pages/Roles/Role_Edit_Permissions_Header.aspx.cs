using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSModules_Membership_Pages_Roles_Role_Edit_Permissions_Header : CMSRolesPage
{
    #region "Page Events"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        ((Panel)CurrentMaster.PanelBody.FindControl("pnlContent")).CssClass = "";
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get parameters
        prmhdrHeader.SiteID = (SiteID <= 0) ? 0 : SiteID;
        prmhdrHeader.RoleID = QueryHelper.GetInteger("roleid", 0);
        prmhdrHeader.HideSiteSelector = ((SiteID <= 0) && MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin));
        prmhdrHeader.ShowUserSelector = false;
        prmhdrHeader.UseUniSelectorAutocomplete = false;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Register script for load permission matrix to other frame
        string scriptText = ScriptHelper.GetScript(string.Format(@"parent.frames['content'].location = 'Role_Edit_Permissions_Matrix.aspx?roleid={0}&id={1}&type={2}&siteid={3}';",
                                                                 prmhdrHeader.RoleID, prmhdrHeader.SelectedID, prmhdrHeader.SelectedType, prmhdrHeader.SiteID));
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "InitMatrix", scriptText);
    }

    #endregion
}