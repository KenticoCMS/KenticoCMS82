using System;

using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_Newsletters_Controls_Newsletter_ContentEditor : CMSUserControl
{
    #region "Variables"

    protected string frameSrc = string.Empty;
    private bool loaded = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Issue ID.
    /// </summary>
    public int IssueID
    {
        get;
        set;
    }


    /// <summary>
    /// Newsletter ID.
    /// </summary>
    public int NewsletterID
    {
        get;
        set;
    }


    /// <summary>
    /// Template ID, optional.
    /// If not set, template from newsletter configuration is used.
    /// </summary>
    public int TemplateID
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates that the control is used in new dialog.
    /// </summary>
    public bool IsDialogMode
    {
        get;
        set;
    }


    /// <summary>
    /// Enable/disable control.
    /// </summary>
    public bool Enabled
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing || ((NewsletterID <= 0) && (IssueID <= 0)))
        {
            return;
        }

        ReloadData(false);
    }


    public void ReloadData(bool forceReload)
    {
        if (!loaded || forceReload)
        {
            frameSrc = URLHelper.ResolveUrl(String.Format("Newsletter_Iframe_Edit.aspx?parentobjectid={0}{1}{2}{3}",
                NewsletterID,
                (IssueID > 0 ? "&objectid=" + IssueID : string.Empty),
                (TemplateID > 0 ? "&templateid=" + TemplateID : string.Empty),
                (!Enabled? "&readonly=1" : string.Empty)));

            loaded = true;
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            RegisterScript();
        }
    }


    protected void RegisterScript()
    {
        // Get master page if present
        ICMSMasterPage master = Page.Master as ICMSMasterPage;

        string script = string.Format(
@"// Editor frameset
var F = null;
// Editor top panel, it is declared in EditIssue control
var T = null;
// Master page content area
var C = null;
// Master page header area
var H = null;

function InitItems() {{
    if (F == null) {{ F = document.getElementById('iframeContent'); }}
    if (T == null) {{ T = document.getElementById('topPanel'); }}
    if (C == null) {{ C = document.getElementById('{0}'); }}
    if (H == null) {{ H = {1}; }}
}}
function SetIFrameHeight() {{
    InitItems();
    if ((F != null) && (C != null)) {{
        var offset = 0;
        // Test for IE browser
        var isIE = /MSIE/.test(navigator.userAgent);
        if (T != null) {{
            offset = T.offsetHeight;

            // Add 3px to hide scrollbar in IE
            if (isIE) {{
                offset += 3;
            }}
        }}
        if (H != null) {{
            offset += H.offsetHeight;

            // Add 1px if not in IE
            if (!isIE) {{
                offset += 1;
            }}
        }}
        F.height = (C.offsetHeight - offset);
    }}
}}", IsDialogMode ? "divContent" : master.Body.ClientID,
  IsDialogMode ? "null" : "document.getElementById('" + master.PanelHeader.ClientID + "')");

        // Register script for content resizing
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "ResizeScript_" + ClientID, script, true);

        // Register startup script for content resizing
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "StartupResizeScript_" + ClientID,
            ScriptHelper.GetScript(string.Format("InitItems(); setTimeout(SetIFrameHeight,10); {0} = function () {{ SetIFrameHeight(); }};", IsDialogMode ? "window.afterResize" : "C.onresize")));
    }

    #endregion
}