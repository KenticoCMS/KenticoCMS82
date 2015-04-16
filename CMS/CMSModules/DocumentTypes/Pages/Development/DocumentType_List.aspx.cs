using System;

using CMS.DataEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.IO;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.Modules;

// New document type action
[Action(0, "DocumentType_List.NewDoctype", "DocumentType_New.aspx")]
public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_List : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterExportScript();

        // Unigrid initialization
        uniGrid.OnAction += uniGrid_OnAction;
        uniGrid.ZeroRowsText = GetString("general.nodatafound");
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            URLHelper.Redirect(GetEditUrl(actionArgument.ToString()));
        }
        else if (actionName == "delete")
        {
            int classId = ValidationHelper.GetInteger(actionArgument, 0);

            DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(classId);

            if (dci != null)
            {
                // Check unerasable dependences
                if (DataClassInfoProvider.CheckDependencies(dci.ClassID))
                {
                    ShowError(String.Format(GetString("DocumentType_List.Dependences"), HTMLHelper.HTMLEncode(dci.ClassDisplayName)));
                }
                else
                {
                    // Delete dataclass and its dependencies
                    try
                    {
                        // Delete view
                        string viewName = SqlHelper.GetViewName(dci.ClassTableName, null);

                        TableManager tm = new TableManager(null);
                        tm.DropView(viewName);

                        string className = dci.ClassName;
                        DataClassInfoProvider.DeleteDataClassInfo(dci);
                        
                        // Delete icons
                        string iconFile = UIHelper.GetDocumentTypeIconPath(this, className, "", false);
                        string iconLargeFile = UIHelper.GetDocumentTypeIconPath(this, className, "48x48", false);
                        iconFile = Server.MapPath(iconFile);
                        iconLargeFile = Server.MapPath(iconLargeFile);

                        if (File.Exists(iconFile))
                        {
                            File.Delete(iconFile);
                        }
                        // Ensure that ".gif" file will be deleted
                        iconFile = iconFile.Replace(".png", ".gif");

                        if (File.Exists(iconFile))
                        {
                            File.Delete(iconFile);
                        }

                        if (File.Exists(iconLargeFile))
                        {
                            File.Delete(iconLargeFile);
                        }
                        // Ensure that ".gif" file will be deleted
                        iconLargeFile = iconLargeFile.Replace(".png", ".gif");
                        if (File.Exists(iconLargeFile))
                        {
                            File.Delete(iconLargeFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        EventLogProvider.LogException("Development", "DeleteDocType", ex);
                        ShowError(GetString("DocumentType_List.DeleteFailed") + " " + ex.Message);
                    }
                }
            }
        }
    }

    #region "Private methods"

    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    /// <param name="documentTypeId">Document type identifier</param>
    private String GetEditUrl(string documentTypeId)
    {
        UIElementInfo uiChild = UIElementInfoProvider.GetUIElementInfo("CMS.DocumentEngine", "EditDocumentType");
        if (uiChild != null)
        {
            return URLHelper.AppendQuery(UIContextHelper.GetElementUrl(uiChild, UIContext), "displaytitle=false&objectid=" + documentTypeId);
        }

        return String.Empty;
    }

    #endregion
}