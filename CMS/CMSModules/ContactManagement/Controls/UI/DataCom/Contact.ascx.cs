using System;
using System.Collections.Generic;

using CMS.DataCom;
using CMS.EventLog;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;

/// <summary>
/// A control that displays Data.com contact details.
/// </summary>
public partial class CMSModules_ContactManagement_Controls_UI_DataCom_Contact : CMSAdminControl
{
    #region "Inner classes"

    /// <summary>
    /// Represents a Data.com contact attribute name/display value pair.
    /// </summary>
    public class RepeaterDataItem
    {

        /// <summary>
        /// A Data.com contact attribute name.
        /// </summary>
        public string AttributeName;


        /// <summary>
        /// A Data.com contact attribute display value.
        /// </summary>
        public string AttributeValue;

    }

    #endregion


    #region "Variables"

    /// <summary>
    /// The Data.com contact to display.
    /// </summary>
    private Contact mContact;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the Data.com contact to display.
    /// </summary>
    public Contact Contact
    {
        get
        {
            return mContact;
        }
        set
        {
            mContact = value;
        }
    }

    #endregion


    #region "Life cycle methods"

    protected override void OnPreRender(EventArgs e)
    {
        if (Contact != null)
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
    /// Creates a list of localized attribute name/display value pairs for the specified Data.com contact, and returns it.
    /// </summary>
    /// <returns>A list of localized attribute name/display value pairs for the specified Data.com contact.</returns>
    private List<RepeaterDataItem> CreateDataSource()
    {
        IEntityAttributeFormatter formatter = DataComHelper.GetEntityAttributeFormatter();
        List<RepeaterDataItem> items = new List<RepeaterDataItem>();
        EntityInfo entityInfo = DataComHelper.GetContactEntityInfo();
        IEntityAttributeMapperFactory entityAttributeFactory = DataComHelper.GetContactAttributeMapperFactory();
        foreach (EntityAttributeInfo entityAttributeInfo in entityInfo.Items)
        {
            string entityAttributeValue = String.Empty;
            EntityAttributeMapperBase entityAttribute = entityAttributeFactory.CreateEntityAttributeMapper(entityAttributeInfo.Name, entityInfo);
            if (entityAttribute != null)
            {
                entityAttributeValue = entityAttribute.GetEntityAttributeDisplayValue(Contact, formatter);
            }
            RepeaterDataItem item = new RepeaterDataItem()
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
        EventLogProvider.LogException("Data.com Connector", "ContactControl", exception);
    }

    #endregion
}