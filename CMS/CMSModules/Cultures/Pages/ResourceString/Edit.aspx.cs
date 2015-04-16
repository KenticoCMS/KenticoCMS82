using System;

using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.Localization;
using CMS.UIControls;

[Help("DefaultNewEdit_string")]
[Breadcrumb(0, "culture.strings", "List.aspx?culturecode={%LocalizationContext.CurrentCulture.CultureCode%}", null)]
[Breadcrumb(1, "culture.newstring")]
public partial class CMSModules_Cultures_Pages_ResourceString_Edit : GlobalAdminPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get parameters from query string
        string resStringKey = QueryHelper.GetString("stringkey", String.Empty);
        string cultureCode = QueryHelper.GetString("culturecode", String.Empty);

        ResourceStringInfo ri = ResourceStringInfoProvider.GetResourceStringInfo(resStringKey, cultureCode);
        if (ri != null)
        {
            if (ri.StringID > 0)
            {
                // Modify breadcrumbs
                if (PageBreadcrumbs.Items.Count >= 2)
                {
                    PageBreadcrumbs.Items[0].Text = ri.StringKey;
                }

                // Set actions
                CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction
                {
                    Text = GetString("culture.newstring"),
                    RedirectUrl = ResolveUrl("Edit.aspx?culturecode=" + cultureCode),
                    ButtonStyle = ButtonStyle.Default
                });
            }
            else
            {
                // Create new resource string
                resEditor.RedirectUrlAfterSave = "Edit.aspx?stringkey={0}&culturecode=" + cultureCode;
            }
        }
        else
        {
            ShowError(GetString("localize.doesntexist"));
            resEditor.Visible = false;
        }
    }

    #endregion
}