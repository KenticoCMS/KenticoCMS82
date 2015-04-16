using System;
using System.Linq;
using System.Web.UI;

using CMS.PortalEngine;
using CMS.UIControls;
using CMS.Helpers;

public partial class CMSModules_AdminControls_Controls_UIControls_ObjectTreeMenu : CMSAbstractUIWebpart
{
    #region "Properties"

    /// <summary>
    /// Indicates whether use max node limit stored in settings.
    /// </summary>
    public bool UseMaxNodeLimit
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseMaxNodeLimit"), true);
        }
        set
        {
            SetValue("UseMaxNodeLimit", value);
        }
    }


    /// <summary>
    /// If false tree wont show items but just categories.
    /// </summary>
    public bool ShowItems
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowItems"), true);
        }
        set
        {
            SetValue("ShowItems", value);
        }
    }


    /// <summary>
    /// Maximum tree nodes shown under parent node - this value can be ignored if UseMaxNodeLimit set to false.
    /// </summary>
    public int MaxTreeNodes
    {
        get
        {
            return  GetIntContextValue("MaxTreeNodes", -1);
        }
        set
        {
            SetValue("MaxTreeNodes", value);
        }
    }


    /// <summary>
    /// Indicates whether tree displays clone button
    /// </summary>
    public bool DisplayCloneButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayCloneButton"), false);
        }
        set
        {
            SetValue("DisplayCloneButton", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        paneTree.Values.Add(new UILayoutValue("MaxTreeNodes", MaxTreeNodes));
        paneTree.Values.Add(new UILayoutValue("ShowItems", ShowItems));
        paneTree.Values.Add(new UILayoutValue("UseMaxNodeLimit", UseMaxNodeLimit));
        paneTree.Values.Add(new UILayoutValue("DisplayCloneButton", DisplayCloneButton));

        paneContentTMain.Src = ResolveUrl("~/CMSPages/Blank.aspx");

        base.OnContentLoaded();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (PortalContext.ViewMode.IsDesign(true))
        {
            paneContentTMain.RenderAs = HtmlTextWriterTag.Div;
            paneContentTMain.Size = "0";
        }

        // Handle title
        ManagePaneTitle(paneTitle, true);

        // Show dialog footer only when used in a dialog
        layoutElem.DisplayFooter = DisplayFooter;

        ScriptHelper.HideVerticalTabs(Page);
    }

    #endregion
}
