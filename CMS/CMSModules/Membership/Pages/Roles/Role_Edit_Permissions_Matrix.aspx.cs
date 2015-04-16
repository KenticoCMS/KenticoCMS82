using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSModules_Membership_Pages_Roles_Role_Edit_Permissions_Matrix : CMSRolesPage
{
    #region "Page Events"

    protected override void OnPreInit(EventArgs e)
    {
        ((Panel)CurrentMaster.PanelBody.FindControl("pnlContent")).CssClass = "";
        base.OnPreInit(e);
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EnsureScriptManager();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check "read" permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Permissions", "Read"))
        {
            RedirectToAccessDenied("CMS.Permissions", "Read");
        }

        prmMatrix.SelectedID = QueryHelper.GetString("id", string.Empty);
        if (prmMatrix.SelectedID != "0")
        {
            prmMatrix.SelectedType = QueryHelper.GetString("type", string.Empty);
        }

        prmMatrix.GlobalRoles = ((SiteID <= 0) && MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin));
        prmMatrix.SiteID = SiteID;
        prmMatrix.RoleID = QueryHelper.GetInteger("roleid", 0);
        prmMatrix.OnDataLoaded += new CMSModules_Permissions_Controls_PermissionsMatrix.OnMatrixDataLoaded(prmMatrix_DataLoaded);
        prmMatrix.CornerText = GetString("Administration.Roles.Permission");
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Replaces text in column header with localized string.
    /// </summary>
    /// <param name="ds">Data set to be processed</param>
    protected void prmMatrix_DataLoaded(DataSet ds)
    {
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                dr["RoleDisplayName"] = GetString("Administration.Roles.AllowPermission");
            }
        }
    }

    #endregion
}