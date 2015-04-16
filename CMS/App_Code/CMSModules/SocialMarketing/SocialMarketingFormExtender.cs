using System;

using CMS;
using CMS.Base;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.SocialMarketing;

[assembly: RegisterCustomClass("SocialMarketingFormExtender", typeof(SocialMarketingFormExtender))]

/// <summary>
/// Extends UI forms from Social marketing module with additional abilities.
/// </summary>
public class SocialMarketingFormExtender : ControlExtender<UIForm>
{

    #region "Page life-cycle methods and event handlers"
    
    public override void OnInit()
    {
        Control.OnItemValidation += Control_OnItemValidation;
        Control.OnBeforeSave += Control_OnBeforeSave;
    }


    private void Control_OnBeforeSave(object sender, EventArgs e)
    {
        TwitterAccountInfo account = Control.EditedObject as TwitterAccountInfo;
        if (account != null)
        {
            TwitterApplicationInfo application = TwitterApplicationInfoProvider.GetTwitterApplicationInfo(account.TwitterAccountTwitterApplicationID);
            try
            {
                account.TwitterAccountUserID = TwitterHelper.GetTwitterUserId(application.TwitterApplicationConsumerKey, application.TwitterApplicationConsumerSecret, account.TwitterAccountAccessToken, account.TwitterAccountAccessTokenSecret);
                string errorMessage = null;
                Validate<TwitterAccountInfo>("TwitterAccountUserID", account.TwitterAccountUserID, "sm.twitter.account.msg.useridexists", ref errorMessage);
                if (!String.IsNullOrEmpty(errorMessage))
                {
                    CancelPendingSave(errorMessage);
                }
            }
            catch
            {
                string errorMessage = ResHelper.GetString("sm.twitter.account.msg.getuseridfail");
                CancelPendingSave(errorMessage);
            }
        }
    }


    private void Control_OnItemValidation(object sender, ref string errorMessage)
    {
        FormEngineUserControl control = sender as FormEngineUserControl;
        if ((control != null) && String.IsNullOrEmpty(errorMessage))
        {
            // Check special field's value is unique on site
            switch (control.FieldInfo.Name)
            {
                case "FacebookApplicationConsumerKey":
                    Validate<FacebookApplicationInfo>(control, "sm.facebook.application.msg.consumerkeyexists", ref errorMessage);
                    break;

                case "FacebookAccountPageID":
                    Validate<FacebookAccountInfo>(control, "sm.facebook.account.msg.pageidexists", ref errorMessage);
                    break;

                case "TwitterApplicationConsumerKey":
                    Validate<TwitterApplicationInfo>(control, "sm.twitter.application.msg.consumerkeyexists", ref errorMessage);
                    break;

                case "LinkedInApplicationConsumerKey":
                    Validate<LinkedInApplicationInfo>(control, "sm.linkedin.application.msg.consumerkeyexists", ref errorMessage);
                    break;

                case "LinkedInAccountProfileID":
                    Validate<LinkedInAccountInfo>(control, "sm.linkedin.profile.msg.profileidexists", ref errorMessage);
                    break;
            }
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Perevnts the form from saving.
    /// </summary>
    /// <param name="errorMessage">The error message to display.</param>
    private void CancelPendingSave(string errorMessage)
    {
        Control.StopProcessing = true;
        Control.ShowError(errorMessage);
    }


    /// <summary>
    /// Validates whether the object property value is unique, and provides an optional error message.
    /// </summary>
    /// <typeparam name="T">The type of object to validate.</typeparam>
    /// <param name="control">The control that corresponds to the property to validate.</param>
    /// <param name="resourceKey">The resource key of the error message.</param>
    /// <param name="errorMessage">The error message.</param>
    private void Validate<T>(FormEngineUserControl control, string resourceKey, ref string errorMessage) where T : BaseInfo, new()
    {
        string uniqueText = control.Value as string;
        Validate<T>(control.FieldInfo.Name, uniqueText, resourceKey, ref errorMessage);
    }


    /// <summary>
    /// Validates whether the column value is unique, and provides an optional error message.
    /// </summary>
    /// <typeparam name="T">The type of object to validate.</typeparam>
    /// <param name="columnName">The name of the column to validate.</param>
    /// <param name="uniqueText">The column text.</param>
    /// <param name="resourceKey">The resource key of the error message.</param>
    /// <param name="errorMessage">The error message.</param>
    private void Validate<T>(string columnName, string uniqueText, string resourceKey, ref string errorMessage) where T : BaseInfo, new()
    {
        int siteId = SiteContext.CurrentSiteID;
        BaseInfo info = Control.EditedObject as BaseInfo;
        if (info != null && siteId > 0 && !String.IsNullOrWhiteSpace(uniqueText))
        {
            int identifier = info.GetIntegerValue(info.TypeInfo.IDColumn, 0);
            ObjectQuery<T> query = new ObjectQuery<T>().Where(columnName, QueryOperator.Equals, uniqueText).OnSite(siteId);
            if (identifier > 0)
            {
                query.Where(info.TypeInfo.IDColumn, QueryOperator.NotEquals, identifier);
            }
            if (query.Count > 0)
            {
                errorMessage = ResHelper.GetString(resourceKey);
            }
        }
    }

    #endregion

}