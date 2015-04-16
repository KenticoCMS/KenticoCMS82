using System;
using System.Web.UI.WebControls;
using System.Data;

using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.ExtendedControls;

[Title("Currency_List.HeaderCaption")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_Currencies_Currency_List : CMSCurrenciesPage
{
    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Init header actions
        HeaderActions actions = CurrentMaster.HeaderActions;
        actions.ActionPerformed += HeaderActions_ActionPerformed;

        // New item action
        actions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("Currency_List.NewItemCaption"),
            RedirectUrl = ResolveUrl("Currency_Edit.aspx?siteId=" + SiteID)
        });

        // Show copy from global link when not configuring global currencies.
        if (ConfiguredSiteID != 0)
        {
            // Show "Copy from global" link only if there is at least one global currency
            DataSet ds = CurrencyInfoProvider.GetCurrencies().WhereNull("CurrencySiteID").Columns("CurrencyID").TopN(1);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                actions.ActionsList.Add(new HeaderAction
                {
                    Text = GetString("general.copyfromglobal"),
                    OnClientClick = "return ConfirmCopyFromGlobal();",
                    CommandName = "copyFromGlobal",
                    ButtonStyle = ButtonStyle.Default
                });

                // Register javascript to confirm generate 
                string script = "function ConfirmCopyFromGlobal() {return confirm(" + ScriptHelper.GetString(GetString("com.ConfirmCurrencyFromGlobal")) + ");}";
                ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ConfirmCopyFromGlobal", ScriptHelper.GetScript(script));
            }
        }

        gridElem.OnAction += gridElem_OnAction;

        // Show information about usage of global objects when used on site
        HandleGlobalObjectInformation(gridElem.ObjectType);

        // Filter records by site
        gridElem.WhereCondition = InitSiteWhereCondition("CurrencySiteID").ToString(true);
    }

    #endregion


    #region "Event Handlers"

    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "copyfromglobal":
                CopyFromGlobal();
                gridElem.ReloadData();
                break;
        }
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            URLHelper.Redirect("Currency_Edit.aspx?currencyid=" + Convert.ToString(actionArgument) + "&siteId=" + SiteID);
        }
        else if (actionName == "delete")
        {
            // Check permissions
            CheckConfigurationModification();

            int currencyId = ValidationHelper.GetInteger(actionArgument, 0);
            CurrencyInfo currency = CurrencyInfoProvider.GetCurrencyInfo(currencyId);

            if (currency != null)
            {
                if (currency.Generalized.CheckDependencies())
                {
                    // Show error message
                    ShowError(ECommerceHelper.GetDependencyMessage(currency));

                    return;
                }

                // An attempt to delete main currency
                if (currency.CurrencyIsMain)
                {
                    // Show error message
                    ShowError(string.Format(GetString("com.deletemaincurrencyerror"), HTMLHelper.HTMLEncode(currency.CurrencyDisplayName)));

                    return;
                }

                // Delete CurrencyInfo object from database
                CurrencyInfoProvider.DeleteCurrencyInfo(currency);
            }
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    ///  Copies site-specific currencies from global currency list.
    /// </summary>
    protected void CopyFromGlobal()
    {
        CheckConfigurationModification();
        CurrencyInfoProvider.CopyFromGlobal(ConfiguredSiteID);
    }

    #endregion
}