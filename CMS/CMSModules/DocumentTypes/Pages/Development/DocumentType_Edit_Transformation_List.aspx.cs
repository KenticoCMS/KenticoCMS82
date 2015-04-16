using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Modules;

public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Transformation_List : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int documentTypeId = QueryHelper.GetInteger("parentobjectid", 0);

        // New item links
        CurrentMaster.HeaderActions.AddAction(new HeaderAction
        {
            Text = GetString("DocumentType_Edit_Transformation_List.btnNew"),
            RedirectUrl = ResolveUrl("DocumentType_Edit_Transformation_New.aspx?parentobjectid=" + documentTypeId + "&hash=" + QueryHelper.GetHash("?objectid=" + documentTypeId))
        });
        CurrentMaster.HeaderActions.AddAction(new HeaderAction
        {
            Text = GetString("DocumentType_Edit_Transformation_List.btnHierarchicalNew"),
            RedirectUrl = ResolveUrl("HierarchicalTransformations_New.aspx?parentobjectid=" + documentTypeId)
        });

        // Set the query editor control
        classTransformations.ClassID = documentTypeId;
        classTransformations.EditPageUrl = GetEditUrl();
        classTransformations.IsSiteManager = true;
    }


    #region "Private methods"

    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    private String GetEditUrl()
    {
        UIElementInfo uiChild = UIElementInfoProvider.GetUIElementInfo("CMS.DocumentEngine", "EditTransformation");
        if (uiChild != null)
        {
            return URLHelper.AppendQuery(UIContextHelper.GetElementUrl(uiChild, UIContext), "displaytitle=false");
        }

        return String.Empty;
    }

    #endregion
}