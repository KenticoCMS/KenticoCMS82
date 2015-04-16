using System;
using System.Collections;

using CMS.FormControls;
using CMS.Base;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSModules_Chat_FormControls_Cloning_Chat_RoomSettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Defines a custom close refresh script which preserves selected item in site drop down list.
    /// </summary>
    public override string CloseScript
    {
        get
        {
            return "wopener.RefreshUsingPostBack(); CloseDialog();";
        }
    }


    /// <summary>
    /// Returns false, so this control won't be displayed, because it only sets settings.
    /// </summary>
    public override bool DisplayControl
    {
        get
        {
            return false;
        }
    }

    #endregion
}