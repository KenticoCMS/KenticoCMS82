﻿using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using CMS.Core;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;
using CMS.PortalControls;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.LicenseProvider;
using CMS.DataEngine;
using CMS.Modules;
using CMS.MacroEngine;


public partial class CMSModules_Content_Controls_DocTypeSelection : CMSAbstractNewDocumentControl
{
    #region "Variables"

    private DataSet dsClasses;
    private string lastPriorityClassName;
    private BaseInfo mScope;

    #endregion


    #region "Properties"

    /// <summary>
    /// Unigrid object used for listing document types.
    /// </summary>
    public UniGrid Grid
    {
        get
        {
            return gridClasses;
        }
    }


    /// <summary>
    /// The count of document types found.
    /// </summary>
    public int ClassesCount
    {
        get;
        private set;
    }


    /// <summary>
    /// Gets the best fitting document type scope if exist.
    /// </summary>
    private DocumentTypeScopeInfo Scope
    {
        get
        {
            return (DocumentTypeScopeInfo)InfoHelper.EnsureInfo(ref mScope, () => DocumentTypeScopeInfoProvider.GetScopeInfo(ParentNode));
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        var currentUser = MembershipContext.AuthenticatedUser;

        // Setup unigrid
        gridClasses.GridView.ShowHeader = false;
        gridClasses.GridView.BorderWidth = 0;
        gridClasses.OnExternalDataBound += gridClasses_OnExternalDataBound;
        gridClasses.OnBeforeDataReload += gridClasses_OnBeforeDataReload;
        gridClasses.OnAfterRetrieveData += gridClasses_OnAfterRetrieveData;

        if (ConvertDocumentID > 0)
        {
            // Hide extra options
            plcNewABTestVariant.Visible = false;
            plcNewLink.Visible = false;
        }
        else
        {
            if (Scope != null)
            {
                // Initialize control by scope settings
                AllowNewABTest &= Scope.ScopeAllowABVariant;
                AllowNewLink &= Scope.ScopeAllowLinks;
            }

            lblNewLink.Text = GetString("content.ui.linkexistingdoc");
            lnkNewLink.NavigateUrl = "javascript:modalDialog('" + GetLinkDialogUrl(ParentNodeID) + "', 'contentselectnode', '90%', '85%')";

            plcNewABTestVariant.Visible = false;

            if (ParentNode != null)
            {
                // AB test variant settings
                if (SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSABTestingEnabled")
                    && AllowNewABTest
                    && !IsInDialog
                    && currentUser.IsAuthorizedPerResource("cms.ABTest", "Read")
                    && ModuleEntryManager.IsModuleLoaded(ModuleName.ONLINEMARKETING)
                    && ResourceSiteInfoProvider.IsResourceOnSite("CMS.ABTest", SiteContext.CurrentSiteName)
                    && LicenseHelper.CheckFeature(RequestContext.CurrentDomain, FeatureEnum.ABTesting)
                    && (ParentNode.NodeAliasPath != "/")
                    && (ParentNode.NodeClassName != "CMS.Folder"))
                {
                    string url = URLHelper.AddParameterToUrl(NewVariantUrl, "parentnodeid", ParentNodeID.ToString());
                    url = URLHelper.AddParameterToUrl(url, "parentculture", ParentCulture);

                    plcNewABTestVariant.Visible = true;
                    lblNewVariant.Text = GetString("abtesting.abtestvariant");
                    lnkNewVariant.NavigateUrl = URLHelper.GetAbsoluteUrl(url);
                }
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        gridClasses.GridView.Columns[1].Visible = false;
        gridClasses.GridView.GridLines = GridLines.None;

        // Set info label
        headDocumentTypeSelection.Text = Caption;
        headDocumentTypeSelection.Level = HeadingLevel;

        // Show/hide new linked document panel
        if (!AllowNewLink)
        {
            plcNewLinkNew.Visible = false;
        }

        // Hide separator if no additional actions are visible
        pnlSeparator.Visible = pnlFooter.Visible || pnlABVariant.Visible;
    }

    #endregion


    #region "Methods"

    protected void gridClasses_OnBeforeDataReload()
    {
        if (ParentNode != null)
        {
            var currentUser = MembershipContext.AuthenticatedUser;

            // Check permission to create new document
            if (currentUser.IsAuthorizedToCreateNewDocument(ParentNode, null))
            {
                // Apply document type scope
                string where = DocumentTypeScopeInfoProvider.GetScopeClassWhereCondition(Scope);

                if (!String.IsNullOrEmpty(gridClasses.CompleteWhereCondition))
                {
                    where = SqlHelper.AddWhereCondition(where, gridClasses.CompleteWhereCondition);
                }

                if ((ConvertDocumentID > 0) || !PortalHelper.IsWireframingEnabled(SiteContext.CurrentSiteName))
                {
                    where = SqlHelper.AddWhereCondition(where, "ClassName <> 'CMS.Wireframe'");
                }

                // Add extra where condition
                where = SqlHelper.AddWhereCondition(where, Where);

                var parentClassId = ValidationHelper.GetInteger(ParentNode.GetValue("NodeClassID"), 0);

                // Get the allowed child classes
                DataSet ds = AllowedChildClassInfoProvider.GetAllowedChildClasses(parentClassId, SiteContext.CurrentSiteID)
                    .Where(where)
                    .OrderBy("ClassID")
                    .TopN(gridClasses.TopN)
                    .Columns("ClassName", "ClassDisplayName", "ClassID", "ClassIconClass");

                List<DataRow> priorityRows = new List<DataRow>();

                // Check user permissions for "Create" permission
                bool hasNodeAllowCreate = (currentUser.IsAuthorizedPerTreeNode(ParentNode, NodePermissionsEnum.Create) == AuthorizationResultEnum.Allowed);
                bool isAuthorizedToCreateInContent = currentUser.IsAuthorizedPerResource("CMS.Content", "Create");

                // No data loaded yet
                ClassesCount = 0;

                // If dataSet is not empty
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    List<DataRow> rowsToRemove = new List<DataRow>();

                    DataTable table = ds.Tables[0];
                    table.DefaultView.Sort = "ClassDisplayName";

                    DataTable resultTable = table.DefaultView.ToTable();

                    for (int i = 0; i < resultTable.Rows.Count; ++i)
                    {
                        DataRow dr = resultTable.Rows[i];
                        string doc = ValidationHelper.GetString(DataHelper.GetDataRowValue(dr, "ClassName"), string.Empty);

                        // Document type is not allowed, remove it from the data set (Extra check for 'CreateSpecific' permission)
                        if (!isAuthorizedToCreateInContent && !currentUser.IsAuthorizedPerClassName(doc, "Create") && (!currentUser.IsAuthorizedPerClassName(doc, "CreateSpecific") || !hasNodeAllowCreate))
                        {
                            rowsToRemove.Add(dr);
                        }
                        else
                        {
                            // Priority document types
                            switch (doc.ToLowerCSafe())
                            {
                                case "cms.menuitem":
                                    // Page (Menu item)
                                    {
                                        priorityRows.Add(dr);
                                        lastPriorityClassName = doc;
                                    }
                                    break;

                                case "cms.wireframe":
                                    // Wireframe document
                                    if (currentUser.IsAuthorizedPerResource("CMS.Design", "Wireframing"))
                                    {
                                        priorityRows.Add(dr);
                                        lastPriorityClassName = doc;
                                    }
                                    else
                                    {
                                        rowsToRemove.Add(dr);
                                    }
                                    break;
                            }
                        }
                    }

                    // Remove the document types
                    foreach (DataRow dr in rowsToRemove)
                    {
                        resultTable.Rows.Remove(dr);
                    }

                    if (!DataHelper.DataSourceIsEmpty(resultTable))
                    {
                        int index = 0;

                        // Put priority rows to first position
                        foreach (DataRow priorityRow in priorityRows)
                        {
                            DataRow dr = resultTable.NewRow();
                            dr.ItemArray = priorityRow.ItemArray;

                            resultTable.Rows.Remove(priorityRow);
                            resultTable.Rows.InsertAt(dr, index);

                            index++;
                        }

                        ClassesCount = resultTable.Rows.Count;

                        dsClasses = new DataSet();
                        dsClasses.Tables.Add(resultTable);

                        gridClasses.DataSource = dsClasses;
                    }
                    else
                    {
                        // Show error message
                        SetErrorMessage(GetString(Scope != null ? "Content.ScopeApplied" : "Content.NoPermissions"));

                        gridClasses.Visible = false;

                        ClassesCount = -1;
                    }
                }
                else
                {
                    if (!gridClasses.FilterIsSet && NoDataAsError)
                    {
                        // Show error message
                        SetErrorMessage(NoDataMessage);
                    }
                    else
                    {
                        gridClasses.ZeroRowsText = NoDataMessage;
                    }
                }
            }
            else
            {
                // Show error message
                SetErrorMessage(GetString("Content.NoPermissions"));
            }
        }
    }


    protected DataSet gridClasses_OnAfterRetrieveData(DataSet ds)
    {
        // Check if there are more options
        if (RedirectWhenNoChoice
            && !plcNewABTestVariant.Visible
            && !AllowNewLink
            && !URLHelper.IsPostback()
            && !DataHelper.DataSourceIsEmpty(ds))
        {
            DataTable table = ds.Tables[0];
            if (table.Rows.Count == 1)
            {
                int classId = ValidationHelper.GetInteger(table.Rows[0]["ClassId"], 0);

                // Redirect when only one document type found
                if (!string.IsNullOrEmpty(SelectionUrl))
                {
                    string url = GetSelectionUrl(classId);
                    if (IsInDialog)
                    {
                        url = URLHelper.UpdateParameterInUrl(url, "reloadnewpage", "1");
                    }
                    URLHelper.Redirect(url);
                }
            }
        }

        return ds;
    }


    protected object gridClasses_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        if (sourceName.ToLowerCSafe() == "classname")
        {
            DataRowView drv = (DataRowView)parameter;

            // Get properties
            string className = ValidationHelper.GetString(drv["ClassName"], string.Empty);
            string classDisplayName = ResHelper.LocalizeString(MacroResolver.Resolve(ValidationHelper.GetString(drv["ClassDisplayName"], string.Empty)));
            int classId = ValidationHelper.GetInteger(drv["ClassId"], 0);
            string iconClass = ValidationHelper.GetString(drv["ClassIconClass"], string.Empty);

            string nameFormat = UIHelper.GetDocumentTypeIcon(Page, className, iconClass) + "{0}";

            // Append link if url specified
            if (!string.IsNullOrEmpty(SelectionUrl))
            {
                string url = GetSelectionUrl(classId);
                if (IsInDialog)
                {
                    url = URLHelper.UpdateParameterInUrl(url, "dialog", "1");
                    url = URLHelper.UpdateParameterInUrl(url, "reloadnewpage", "1");
                }

                // Prepare attributes
                string attrs = "";
                if (!string.IsNullOrEmpty(ClientTypeClick))
                {
                    attrs = string.Format("onclick=\"{0}\"", ClientTypeClick);
                }

                nameFormat = string.Format("<a class=\"ContentNewClass cms-icon-link\" href=\"{0}\" {2}>{1}</a>", url, nameFormat, attrs);
            }

            // Format items to output
            return string.Format(nameFormat, HTMLHelper.HTMLEncode(classDisplayName)) + GenerateSpaceAfter(className);
        }

        return HTMLHelper.HTMLEncode(parameter.ToString());
    }


    /// <summary>
    /// Generates empty line after menu item link.
    /// </summary>
    /// <param name="className">Class name</param>
    public string GenerateSpaceAfter(object className)
    {
        string classNameStr = ValidationHelper.GetString(className, string.Empty).ToLowerCSafe();
        if (classNameStr.EqualsCSafe(lastPriorityClassName, true))
        {
            return "<br />";
        }

        return string.Empty;
    }


    private void SetErrorMessage(string message)
    {
        // Show error message
        ShowError(message);

        headDocumentTypeSelection.Visible = false;
        pnlFooter.Visible = false;
        pnlABVariant.Visible = false;
    }

    #endregion
}
