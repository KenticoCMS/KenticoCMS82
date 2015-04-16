using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Helpers;
using CMS.Membership;
using CMS.PortalControls;

public partial class CMSModules_OnlineMarketing_Controls_Content_MenuAddWidgetVariant : CMSAbstractPortalUserControl
{
    #region "Page methods"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Use UI culture for strings
        string culture = MembershipContext.AuthenticatedUser.PreferredUICultureCode;

        // Add MVT variant
        iAddMVTVariant.Text = ResHelper.GetString("mvtvariant.new", culture);
        iAddMVTVariant.Attributes.Add("onclick", "ContextAddWebPartMVTVariant(GetContextMenuParameter('addWidgetVariantMenu'));");

        // Add Content personalization variant
        iAddCPVariant.Text = ResHelper.GetString("contentpersonalizationvariant.new", culture);
        iAddCPVariant.Attributes.Add("onclick", "ContextAddWebPartCPVariant(GetContextMenuParameter('addWidgetVariantMenu'));");

        pnlWebPartMenu.Attributes.Add("onmouseover", "ActivateParentBorder();");
    }

    #endregion
}