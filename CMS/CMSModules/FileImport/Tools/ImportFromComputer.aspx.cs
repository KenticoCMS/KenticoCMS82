using System;

using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;

[Security(Resource = "CMS.FileImport", Permission = "ImportFiles", ResourceSite = true)]
[UIElement("CMS.FileImport", "ImportFromComputer")]
public partial class CMSModules_FileImport_Tools_ImportFromComputer : CMSDeskPage
{
    #region "Page methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.HeaderActionsPlaceHolder.Visible = false;

        // Initialize selectors
        cultureSelector.SiteID = SiteContext.CurrentSiteID;
        pathElem.SiteID = SiteContext.CurrentSiteID;
        pathElem.DisableTextInput = true;
        pathElem.Changed += pathElem_Changed;

        cultureSelector.DropDownCultures.AutoPostBack = true;
        cultureSelector.DropDownCultures.SelectedIndexChanged += cultureSelector_Changed;

        chkIncludeExtension.AutoPostBack = true;
        chkIncludeExtension.CheckedChanged += chkIncludeExtension_CheckedChanged;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        InitializeUploader();
    }

    #endregion


    #region "Selectors methods"

    private void chkIncludeExtension_CheckedChanged(object sender, EventArgs e)
    {
        InitializeUploader();
    }


    private void cultureSelector_Changed(object sender, EventArgs e)
    {
        InitializeUploader();
    }


    private void pathElem_Changed(object sender, EventArgs e)
    {
        InitializeUploader();
    }


    private void InitializeUploader()
    {
        string uploadParams = String.Format("NodeID|{0}|DocumentCulture|{1}|IncludeExtension|{2}|NodeGroupID|0", pathElem.NodeId, cultureSelector.Value, chkIncludeExtension.Checked);
        uploadParams += "|Hash|" + ValidationHelper.GetHashString(uploadParams, new HashSettings { UserSpecific = false });

        string script = String.Format(@"
var hdnUploaderOptions = document.getElementById('{0}_hdnUploaderOptions');
if (hdnUploaderOptions) {{
    hdnUploaderOptions.value = '{1}';
}}", uploaderElem.ClientID, uploadParams);

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "UploaderInit", script, true);
    }

    #endregion
}