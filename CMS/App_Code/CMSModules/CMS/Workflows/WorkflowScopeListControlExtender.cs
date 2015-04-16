using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using CMS;
using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Localization;
using CMS.Modules;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.WorkflowEngine;


[assembly: RegisterCustomClass("WorkflowScopeListControlExtender", typeof(WorkflowScopeListControlExtender))]

/// <summary>
/// Permission edit control extender
/// </summary>
public class WorkflowScopeListControlExtender : ControlExtender<UniGrid>
{
    /// <summary>
    /// OnInit event handler
    /// </summary>
    public override void OnInit()
    {
        Control.OnExternalDataBound += OnExternalDataBound;
        Control.OnBeforeDataReload += () => Control.NamedColumns["culture"].Visible = LicenseHelper.CheckFeature(RequestContext.CurrentDomain, FeatureEnum.Multilingual);
    }


    /// <summary>
    /// OnExternalDataBound event handler
    /// </summary>
    protected object OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "aliaspath":
                return TreePathUtils.EnsureSingleNodePath((string)parameter);

            case "classdisplayname":
                string docType = ValidationHelper.GetString(parameter, "");
                if (docType == "")
                {
                    return Control.GetString("general.selectall");
                }
                return HTMLHelper.HTMLEncode(docType);

            case "scopecultureid":
                int cultureId = ValidationHelper.GetInteger(parameter, 0);
                if (cultureId > 0)
                {
                    return CultureInfoProvider.GetCultureInfo(cultureId).CultureName;
                }
                else
                {
                    return Control.GetString("general.selectall");
                }

            case "scopeexcluded":
                {
                    bool allowed = !ValidationHelper.GetBoolean(parameter, false);
                    return UniGridFunctions.ColoredSpanAllowedExcluded(allowed);
                }

            case "coverage":
                {
                    DataRowView drv = (DataRowView)parameter;
                    string alias = ValidationHelper.GetString(drv.Row["ScopeStartingPath"], "");
                    bool children = !ValidationHelper.GetBoolean(drv.Row["ScopeExcludeChildren"], false);

                    // Only child documents
                    if (alias.EndsWithCSafe("/%"))
                    {
                        return Control.GetString("workflowscope.children");
                    }
                    else
                    {
                        // Only document
                        if (!children)
                        {
                            return Control.GetString("workflowscope.doc");
                        }
                        // Document including children
                        else
                        {
                            return Control.GetString("workflowscope.docandchildren");
                        }
                    }
                }

            default:
                return parameter;
        }
    }
}