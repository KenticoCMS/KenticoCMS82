using System;

using CMS.Core;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Base;
using CMS.PortalEngine;
using CMS.UIControls;

[Title("Department_List.HeaderCaption")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_Departments_Department_List : CMSDepartmentsPage
{
    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Init Unigrid
        UniGrid.OnAction += uniGrid_OnAction;
        UniGrid.WhereCondition = InitSiteWhereCondition("DepartmentSiteID").ToString(true);
        HandleGridsSiteIDColumn(UniGrid);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        string newElementName;

        if (IsMultiStoreConfiguration)
        {
            newElementName = "Ecommerce.GlobalDepartments.New";
            CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, "Ecommerce.GlobalDepartments");
        }
        else
        {
            newElementName = "new.configuration.Departments";
            CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, "Configuration.Departments");
        }

        // Header actions
        HeaderActions actions = CurrentMaster.HeaderActions;
        actions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("Department_List.newitemcaption"),
            RedirectUrl = GetRedirectURL(newElementName),
        });
    }

    #endregion


    #region "Event Handlers"

    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            var editElementName = IsMultiStoreConfiguration ? "Edit.Ecommerce.GlobalDepartments.Properties" : "Edit.DepartmentsProperties";
            URLHelper.Redirect(UIContextHelper.GetElementUrl("CMS.Ecommerce", editElementName, false, actionArgument.ToInteger(0)));
        }
        else if (actionName == "delete")
        {
            DepartmentInfo deptInfoObj = DepartmentInfoProvider.GetDepartmentInfo(ValidationHelper.GetInteger(actionArgument, 0));
            // Nothing to delete
            if (deptInfoObj == null)
            {
                return;
            }

            // Check permissions
            CheckConfigurationModification(deptInfoObj.DepartmentSiteID);

            if (deptInfoObj.Generalized.CheckDependencies())
            {
                // Show error message
                ShowError(GetString("Ecommerce.DeleteDisabledWithoutEnable"));

                return;
            }

            // Delete DepartmentInfo object from database
            DepartmentInfoProvider.DeleteDepartmentInfo(deptInfoObj);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Generates redirection url with query string parameters.
    /// </summary>
    /// <param name="uiElementName">Name of ui element to redirect to.</param>
    private string GetRedirectURL(string uiElementName)
    {
        string url = UIContextHelper.GetElementUrl("cms.ecommerce", uiElementName, false);
        // Only global object can be created from site manager       
        if (IsMultiStoreConfiguration)
        {
            url = URLHelper.AddParameterToUrl(url, "siteid", SpecialFieldValue.GLOBAL.ToString());
        }

        return url;
    }

    #endregion
}