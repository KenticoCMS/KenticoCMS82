using System;
using System.Web.UI.WebControls;
using System.Data;

using CMS.Core;
using CMS.Ecommerce;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.ExtendedControls;
using CMS.PortalEngine;

[Title("TaxClass_List.HeaderCaption")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_TaxClasses_TaxClass_List : CMSTaxClassesPage
{
    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        UniGrid.OnAction += uniGrid_OnAction;

        if (UseGlobalObjects && ExchangeTableInfoProvider.IsExchangeRateFromGlobalMainCurrencyMissing(SiteContext.CurrentSiteID))
        {
            ShowError(GetString("com.NeedExchangeRateFromGlobal"));
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Init header actions
        HeaderActions actions = CurrentMaster.HeaderActions;
        actions.ActionPerformed += HeaderActions_ActionPerformed;

        string newElementName;
        if (IsMultiStoreConfiguration)
        {
            newElementName = "new.configuration.globaltaxclass";
            CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, "Tools.Ecommerce.TaxClasses");
        }
        else
        {
            newElementName = "new.configuration.taxclass";
            CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, "Configuration.TaxClasses");
        }

        // Prepare the new tax class header element
        actions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("TaxClass_List.NewItemCaption"),
            RedirectUrl = GetRedirectURL(newElementName),
        });

        // Show copy from global link when not configuring global tax classes
        if (ConfiguredSiteID != 0)
        {
            // Show "Copy from global" link only if there is at least one global tax class
            DataSet ds = TaxClassInfoProvider.GetTaxClasses().TopN(1).OnSite(0).Column("TaxClassID");
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                actions.ActionsList.Add(new HeaderAction
                {
                    Text = GetString("general.copyfromglobal"),
                    OnClientClick = "return ConfirmCopyFromGlobal();",
                    CommandName = "copyFromGlobal",
                    ButtonStyle = ButtonStyle.Default
                });

                // Register javascript to confirm action 
                string script = "function ConfirmCopyFromGlobal() {return confirm(" + ScriptHelper.GetString(GetString("com.ConfirmTaxClassFromGlobal")) + ");}";
                ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ConfirmCopyFromGlobal", ScriptHelper.GetScript(script));
            }
        }

        // Show information about usage of global objects when used on site
        HandleGlobalObjectInformation(UniGrid.ObjectType);

        // Filter records by site
        UniGrid.WhereCondition = InitSiteWhereCondition("TaxClassSiteID").ToString(true);
    }

    #endregion


    #region "Event Handlers"

    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "copyfromglobal":
                CopyFromGlobal();
                UniGrid.ReloadData();
                break;
        }
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
            var editElementName = IsMultiStoreConfiguration ? "edit.Tools.Ecommerce.TaxProperties" : "edit.Configuration.TaxClasses";
            string url = GetRedirectURL(editElementName);
            url = URLHelper.AddParameterToUrl(url, "objectid", ValidationHelper.GetInteger(actionArgument, 0).ToString("0"));

            URLHelper.Redirect(url);
        }
        else if (actionName == "delete")
        {
            // Check permissions
            CheckConfigurationModification();

            TaxClassInfoProvider.DeleteTaxClassInfo(ValidationHelper.GetInteger(actionArgument, 0));
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    ///  Copies site-specific tax classes from global list.
    /// </summary>
    protected void CopyFromGlobal()
    {
        CheckConfigurationModification();
        TaxClassInfoProvider.CopyFromGlobal(ConfiguredSiteID);
    }


    /// <summary>
    /// Generates redirection url with query string parameters (siteid working with site selector values and IsMultiStoreConfiguration).
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

        url = URLHelper.AddParameterToUrl(url, "issitemanager", IsMultiStoreConfiguration.ToString());

        return url;
    }

    #endregion
}