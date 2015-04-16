using System;
using System.Collections;

using CMS.Newsletters;
using CMS.UIControls;
using CMS.Helpers;

public partial class CMSModules_Newsletters_FormControls_Cloning_Newsletter_SubscriberSettings : CloneSettingsControl
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
    /// Excluded other binding tasks.
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
        if (!RequestHelper.IsPostBack())
        {
            SubscriberInfo info = (SubscriberInfo)InfoToClone;
            txtEmail.Text = info.SubscriberEmail;
            txtFirstName.Text = info.SubscriberFirstName;
            txtLastName.Text = info.SubscriberLastName;
        }
    }

    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[SubscriberInfo.OBJECT_TYPE + ".email"] = txtEmail.Text;
        result[SubscriberInfo.OBJECT_TYPE + ".firstname"] = txtFirstName.Text;
        result[SubscriberInfo.OBJECT_TYPE + ".lastname"] = txtLastName.Text;
        return result;
    }

    #endregion
}