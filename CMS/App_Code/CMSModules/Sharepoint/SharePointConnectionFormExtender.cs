using System;

using CMS;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.SharePoint;


[assembly: RegisterCustomClass("SharePointConnectionFormExtender", typeof(SharePointConnectionFormExtender))]

/// <summary>
/// Extends UI form used for SharePoint Connection with additional abilities.
/// </summary>
public class SharePointConnectionFormExtender : ControlExtender<UIForm>
{

    #region "Public methods"

    public override void OnInit()
    {
        Control.OnBeforeValidate += Control_OnBeforeValidate;
        Control.OnAfterDataLoad += Control_OnAfterDataLoad;
        Control.OnBeforeSave += Control_OnBeforeSave;
    }

    #endregion


    #region "Private methods"

    private void Control_OnAfterDataLoad(object sender, EventArgs eventArgs)
    {
        var userNameLabel = Control.FieldLabels["SharePointConnectionUserName"];
        if (userNameLabel != null)
        {
            userNameLabel.ShowRequiredMark = true;
        }
    }


    private void Control_OnBeforeValidate(object sender, EventArgs eventArgs)
    {
        if (Control.GetFieldValue("SharePointConnectionAuthMode") as string == SharePointAuthMode.DEFAULT)
        {
            var userNameField = Control.FormInformation.GetFormField("SharePointConnectionUserName");
            if (userNameField != null)
            {
                userNameField.AllowEmpty = false;
            }
        }
    }

    private void Control_OnBeforeSave(object sender, EventArgs eventArgs)
    {
        var connection = Control.EditedObject as SharePointConnectionInfo;
        if ((connection != null) && (connection.SharePointConnectionAuthMode == SharePointAuthMode.ANONYMOUS))
        {
            connection.SharePointConnectionUserName = null;
            connection.SharePointConnectionDomain = null;
            connection.SharePointConnectionPassword = null;
        }
    }

    #endregion

}