using System;
using System.Data;
using System.Collections;

using CMS.FormControls;
using CMS.Base;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.DocumentEngine;

public partial class CMSModules_Objects_FormControls_Cloning_CMS_SiteDomainAliasSettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Hide the control.
    /// </summary>
    public override bool DisplayControl
    {
        get
        {
            return false;
        }
    }


    /// <summary>
    /// Turn off codename validation.
    /// </summary>
    public override bool ValidateCodeName
    {
        get
        {
            return false;
        }
    }

    #endregion
}