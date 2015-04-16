using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.OnlineMarketing;
using CMS.Base;

[SalesForceModuleLoader]
public partial class CMSModuleLoader
{
    /// <summary>
    /// Attribute class that ensures loading of custom handlers.
    /// </summary>
    private class SalesForceModuleLoaderAttribute : CMSLoaderAttribute
    {
        #region "Constructors"

        /// <summary>
        /// Initializes a new instance of the SalesForceModuleLoader attribute.
        /// </summary>
        public SalesForceModuleLoaderAttribute()
        {
            // Require Online marketing module to load properly
            RequiredModules = new string[] { ModuleName.ONLINEMARKETING };
        }

        #endregion


        #region "Public methods"

        /// <summary>
        /// Called automatically when the application starts.
        /// </summary>
        public override void Init()
        {
            SettingsKeyInfo.TYPEINFO.Events.Update.After += new EventHandler<ObjectEventArgs>(SettingsKeyUpdate_After);
            ContactInfo.TYPEINFO.Events.Update.Before += new EventHandler<ObjectEventArgs>(ContactUpdate_Before);
        }

        #endregion


        #region "Private methods"

        private void SettingsKeyUpdate_After(object sender, ObjectEventArgs e)
        {
            SettingsKeyInfo setting = e.Object as SettingsKeyInfo;
            if (setting.KeyName == "CMSSalesForceLeadReplicationMapping")
            {
                SettingsKeyInfoProvider.SetValue("CMSSalesForceLeadReplicationMappingDateTime", DateTime.Now.ToString("s"));
            }
        }


        private void ContactUpdate_Before(object sender, ObjectEventArgs e)
        {
            ContactInfo contact = e.Object as ContactInfo;
            if (!DetectChange(contact, "ContactSalesForceLeadID", "ContactSalesForceLeadReplicationSuspensionDateTime", "ContactSalesForceLeadReplicationDisabled", "ContactSalesForceLeadReplicationDateTime"))
            {
                contact.SetValue("ContactSalesForceLeadReplicationSuspensionDateTime", null);
            }
        }


        private bool DetectChange(ContactInfo contact, params string[] columnNames)
        {
            foreach (string columnName in columnNames)
            {
                object current = contact.GetValue(columnName);
                object original = contact.GetOriginalValue(columnName);
                if (current != original)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }

}