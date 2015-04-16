using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_AdminControls_Controls_UIControls_ObjectEditPanel : CMSAbstractUIWebpart
{
    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return EditPanel.MessagesPlaceHolder;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        // Set control's placeholder and actions for use of non-children controls
        ICMSPage page = Page as ICMSPage;
        if (page != null)
        {
            page.HeaderActions = EditPanel.HeaderActions;
        }

        // Css class for header actions
        EditPanel.PreviewMode = true;

        base.OnInit(e);
    }


    protected override void OnPreRender(EventArgs e)
    {
        ManageTexts();

        // Hide edit panel if there is no visible header action or header action with visible base button
        if (!EditPanel.HeaderActions.ActionsList.Any(m => m.Visible && (m.BaseButton == null || m.BaseButton.Visible)))
        {
            Visible = false;
        }

        base.OnPreRender(e);
    }


    #endregion
}
