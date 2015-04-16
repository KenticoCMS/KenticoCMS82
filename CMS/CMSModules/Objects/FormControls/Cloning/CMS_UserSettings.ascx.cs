using System;
using System.Collections;
using System.Data;

using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DataEngine;
using CMS.WorkflowEngine;

public partial class CMSModules_Objects_FormControls_Cloning_CMS_UserSettings : CloneSettingsControl
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
    /// Excluded child types.
    /// </summary>
    public override string ExcludedChildTypes
    {
        get
        {
            return UserSettingsInfo.OBJECT_TYPE + ";" + PredefinedObjectType.FRIEND + ";" + PredefinedObjectType.PROJECTADHOCTASK + ";" + PersonalizationInfo.OBJECT_TYPE + ";" + PersonalizationInfo.OBJECT_TYPE_DASHBOARD + ";" + OpenIDUserInfo.OBJECT_TYPE;
        }
    }


    /// <summary>
    /// Excluded binding types.
    /// </summary>
    public override string ExcludedBindingTypes
    {
        get
        {
            return PredefinedObjectType.IGNORELIST + ";" + PredefinedObjectType.CONTACTLIST;
        }
    }


    /// <summary>
    /// Excluded other binding types.
    /// </summary>
    public override string ExcludedOtherBindingTypes
    {
        get
        {
            return PredefinedObjectType.REPORTSUBSCRIPTION + ";" + UserRoleInfo.OBJECT_TYPE + ";" + PredefinedObjectType.BOARDMODERATOR + ";" + PredefinedObjectType.FORUMMODERATOR + ";" + MembershipUserInfo.OBJECT_TYPE + ";" + WorkflowStepUserInfo.OBJECT_TYPE + ";" + WorkflowUserInfo.OBJECT_TYPE;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblEmail.ToolTip = GetString("clonning.settings.user.email.tooltip");
        lblGeneratePassword.ToolTip = GetString("clonning.settings.user.generatepassword.tooltip");
        lblPassword.ToolTip = GetString("clonning.settings.user.password.tooltip");

        string script = 
@"function EnableDisablePassword() {
  var elem = document.getElementById('" + txtPassword.ValueElementID + @"');
  var chk = document.getElementById('" + chkGeneratePassword.ClientID + @"');
  if ((chk!= null) && (elem != null)) {
    if (chk.checked) {
      elem.disabled = true;
    } else {
      elem.disabled = false;
    }
  }
}
EnableDisablePassword();
";
        ScriptHelper.RegisterStartupScript(this.Page, typeof(string), "EnableDisablePassword", script, true);
        chkGeneratePassword.Attributes.Add("onclick", "EnableDisablePassword();");
    }


    /// <summary>
    /// Returns true if custom settings are valid against given clone setting.
    /// </summary>
    /// <param name="settings">Clone settings</param>
    public override bool IsValid(CloneSettings settings)
    {
        if (!chkGeneratePassword.Checked)
        {
            if (!txtPassword.IsValid())
            {
                ShowError(AuthenticationHelper.GetPolicyViolationMessage(SiteContext.CurrentSiteName));
                return false;
            }
        }

        // Get sites of original user
        var user = (UserInfo)InfoToClone;
        DataTable siteTable = UserInfoProvider.GetUserSites(user.UserID, null, null, 0, "SiteName");
        var sites = siteTable.AsEnumerable().Select(s => s["SiteName"].ToString());

        // Check that e-mail is unique
        if (!UserInfoProvider.IsEmailUnique(txtEmail.Text.Trim(), sites, 0))
        {
            throw new Exception(GetString("cloneUser.uniqueEmailRequired"));
        }

        return true;
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        
        var objectType = UserInfo.OBJECT_TYPE;

        result[objectType + ".email"] = txtEmail.Text;
        result[objectType + ".generatepassword"] = chkGeneratePassword.Checked;
        result[objectType + ".password"] = txtPassword.Value;
        result[objectType + ".permissions"] = chkPermissions.Checked;
        result[objectType + ".personalization"] = chkPersonalization.Checked;

        return result;
    }

    #endregion
}