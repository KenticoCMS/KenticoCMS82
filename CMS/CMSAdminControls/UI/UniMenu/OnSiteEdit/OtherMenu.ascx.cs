using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;

using MenuItem = CMS.UIControls.UniMenuConfig.Item;
using SubMenuItem = CMS.UIControls.UniMenuConfig.SubItem;

public partial class CMSAdminControls_UI_UniMenu_OnSiteEdit_OtherMenu : CMSUserControl
{
    #region "Methods"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        var cui = CurrentUser;

        if (cui != null)
        {
            if (cui.IsAuthorizedPerUIElement("CMS.OnSiteEdit", "OnSiteHighlight"))
            {
                // Highlight button
                MenuItem highlightItem = new MenuItem();
                highlightItem.CssClass = "BigButton";
                highlightItem.ImageAlign = ImageAlign.Top;
                highlightItem.IconClass = "icon-square-dashed-line";
                highlightItem.OnClientClick = "OEHighlightToggle(event, this);";
                highlightItem.Text = GetString("onsiteedit.highlight");
                highlightItem.Tooltip = GetString("onsiteedit.highlighttooltip");
                highlightItem.ImageAltText = GetString("onsiteedit.highlight");

                otherMenu.Buttons.Add(highlightItem);
            }

            if (cui.IsAuthorizedPerUIElement("CMS.OnSiteEdit", "OnSiteHidden"))
            {
                // Hidden button
                MenuItem hiddenItem = new MenuItem();
                hiddenItem.CssClass = "BigButton OnSiteHiddenButton";
                hiddenItem.ImageAlign = ImageAlign.Top;
                hiddenItem.IconClass = "icon-eye-slash";
                hiddenItem.Text = GetString("general.hidden");
                hiddenItem.Tooltip = GetString("onsiteedit.hiddentooltip");
                hiddenItem.ImageAltText = GetString("general.hidden");

                // Add temporary empty sub menu item to ensure generating of the sub menu functions
                SubMenuItem epmtyItem = new SubMenuItem();
                hiddenItem.SubItems.Add(epmtyItem);

                otherMenu.Buttons.Add(hiddenItem);
            }
        }
    }

    #endregion
}
