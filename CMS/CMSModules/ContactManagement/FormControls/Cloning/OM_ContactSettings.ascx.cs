using System;
using System.Collections;

using CMS.FormControls;
using CMS.Base;
using CMS.OnlineMarketing;
using CMS.Helpers;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSModules_ContactManagement_FormControls_Cloning_OM_ContactSettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Gets properties hashtable.
    /// </summary>
    public override Hashtable CustomParameters
    {
        get
        {
            return GetProperties();
        }
    }


    /// <summary>
    /// Exclueded child types.
    /// </summary>
    public override string ExcludedChildTypes
    {
        get
        {
            return ActivityInfo.OBJECT_TYPE + ";" + UserAgentInfo.OBJECT_TYPE + ";" + IPInfo.OBJECT_TYPE;
        }
    }


    /// <summary>
    /// Exclueded binding types.
    /// </summary>
    public override string ExcludedBindingTypes
    {
        get
        {
            return ContactGroupMemberInfo.OBJECT_TYPE_CONTACT;
        }
    }


    /// <summary>
    /// Hide display name.
    /// </summary>
    public override bool HideDisplayName
    {
        get
        {
            return true;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblMerged.ToolTip = GetString("clonning.settings.contact.merged.tooltip");
        lblAddressesAgents.ToolTip = GetString("clonning.settings.contact.ipaddressesuseragents.tooltip");

        if (!RequestHelper.IsPostBack())
        {
            ContactInfo ci = (ContactInfo)InfoToClone;
            txtFirstName.Text = ci.ContactFirstName;
            txtLastName.Text = ci.ContactLastName;
        }
    }


    /// <summary>
    /// Returns true if custom settings are valid against given clone setting.
    /// </summary>
    /// <param name="settings">Clone settings</param>
    public override bool IsValid(CloneSettings settings)
    {
        if (string.IsNullOrEmpty(txtLastName.Text))
        {
            ShowError(GetString("om.contact.enterlastname"));
            return false;
        }
        return true;
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[ContactInfo.OBJECT_TYPE + ".merged"] = chkMerged.Checked;
        result[ContactInfo.OBJECT_TYPE + ".addressesagents"] = chkAddressesAgents.Checked;
        result[ContactInfo.OBJECT_TYPE + ".activity"] = chkActivity.Checked;
        result[ContactInfo.OBJECT_TYPE + ".contactgroup"] = chkContactGroup.Checked;
        result[ContactInfo.OBJECT_TYPE + ".firstname"] = txtFirstName.Text;
        result[ContactInfo.OBJECT_TYPE + ".lastname"] = txtLastName.Text;
        return result;
    }

    #endregion
}