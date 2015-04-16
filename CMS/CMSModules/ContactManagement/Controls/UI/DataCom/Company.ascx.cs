using System;
using System.Collections.Generic;

using CMS.DataCom;
using CMS.EventLog;
using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;

/// <summary>
/// A control that displays Data.com company details.
/// </summary>
public partial class CMSModules_ContactManagement_Controls_UI_DataCom_Company : CMSAdminControl
{
    #region "Inner classes"

    /// <summary>
    /// Represents a Data.com company attribute name/display value pair.
    /// </summary>
    public class RepeaterDataItem
    {

        /// <summary>
        /// A Data.com company attribute name.
        /// </summary>
        public string AttributeName;


        /// <summary>
        /// A Data.com company attribute display value.
        /// </summary>
        public string AttributeValue;

    }

    #endregion


    #region "Variables"

    /// <summary>
    /// The Data.com company to display.
    /// </summary>
    private Company mCompany;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the Data.com company to display.
    /// </summary>
    public Company Company
    {
        get
        {
            return mCompany;
        }
        set
        {
            mCompany = value;
        }
    }

    #endregion


    #region "Life cycle events"

    protected override void OnPreRender(EventArgs e)
    {
        if (Company != null)
        {
            try
            {
                AttributeRepeater.DataSource = CreateDataSource();
                AttributeRepeater.DataBind();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }
        base.OnPreRender(e);
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Creates a list of localized attribute name/display value pairs for the specified Data.com company, and returns it.
    /// </summary>
    /// <returns>A list of localized attribute name/display value pairs for the specified Data.com company.</returns>
    private List<RepeaterDataItem> CreateDataSource()
    {
        IDataComConfiguration configuration = DataComHelper.GetConfiguration(SiteContext.CurrentSiteID);
        IEntityAttributeFormatter formatter = DataComHelper.GetEntityAttributeFormatter();
        List<RepeaterDataItem> items = new List<RepeaterDataItem>();
        EntityInfo entityInfo = DataComHelper.GetCompanyEntityInfo(configuration);
        IEntityAttributeMapperFactory entityAttributeFactory = DataComHelper.GetCompanyAttributeMapperFactory();
        foreach (EntityAttributeInfo entityAttributeInfo in entityInfo.Items)
        {
            string entityAttributeValue = String.Empty;
            EntityAttributeMapperBase entityAttribute = entityAttributeFactory.CreateEntityAttributeMapper(entityAttributeInfo.Name, entityInfo);
            if (entityAttribute != null)
            {
                entityAttributeValue = entityAttribute.GetEntityAttributeDisplayValue(Company, formatter);
            }
            RepeaterDataItem item = new RepeaterDataItem
            {
                AttributeName = ResHelper.LocalizeString(entityAttributeInfo.DisplayName),
                AttributeValue = entityAttributeValue
            };
            items.Add(item);
        }

        return items;
    }


    /// <summary>
    /// Displays an error message and logs the specified exception to the event log.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    private void HandleException(Exception exception)
    {
        ErrorSummary.Report(exception);
        EventLogProvider.LogException("Data.com Connector", "CompanyControl", exception);
    }

    #endregion
}