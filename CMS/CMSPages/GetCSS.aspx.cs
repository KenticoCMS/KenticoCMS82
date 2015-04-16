using System;

using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;

public partial class CMSPages_GetCSS : GetFilePage
{
    #region "Properties"

    /// <summary>
    /// Implemented abstract property - not used
    /// </summary>
    public override bool AllowCache
    {
        get
        {
            return false;
        }
        set
        {
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Init handler.
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        if (!DebugHelper.DebugResources)
        {
            DisableDebugging();
        }

        // Transfer the execution to the newsletter page if newsletter template
        string newsletterTemplateName = QueryHelper.GetString("newslettertemplatename", String.Empty);
        if (newsletterTemplateName != string.Empty)
        {
            Server.Transfer("~/CMSModules/Newsletters/CMSPages/GetCSS.aspx?newslettertemplatename=" + newsletterTemplateName);
        }

        // Process all other request with resource handler
        ResourceHandler handler = new ResourceHandler();
        handler.ProcessRequest(Context);

        RequestHelper.EndResponse();

        base.OnPreInit(e);
    }

    #endregion
}