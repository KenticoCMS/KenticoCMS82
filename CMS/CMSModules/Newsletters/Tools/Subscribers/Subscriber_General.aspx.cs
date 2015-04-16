using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.Base;
using CMS.UIControls;

[EditedObject(SubscriberInfo.OBJECT_TYPE, "objectid")]
[UIElement(ModuleName.NEWSLETTER, "SubscriberProperties.General")]
public partial class CMSModules_Newsletters_Tools_Subscribers_Subscriber_General : CMSNewsletterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var subscriber = EditedObject as SubscriberInfo;
        if (subscriber == null || subscriber.SubscriberID == 0)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        if(!subscriber.CheckPermissions(PermissionsEnum.Read, CurrentSiteName, CurrentUser))
        {
            RedirectToAccessDenied(subscriber.TypeInfo.ModuleName, "ManageSubscribers");
        }
      
        // Render subscriber email as required
        LocalizedLabel subscriberEmailLabel;
        if ((subscriberEmailLabel = EditForm.FieldLabels["SubscriberEmail"]) != null)
        {
            subscriberEmailLabel.ShowRequiredMark = true;
        }

        if (subscriber.SubscriberRelatedID != 0)
        {
            EditForm.Enabled = false;
            EditForm.Visible = false;
        }

        if (!RequestHelper.IsPostBack())
        {
            // Subscriber was created successfully
            if (QueryHelper.GetBoolean("saved", false))
            {
                ShowChangesSaved();
            }
        }
    }


    /// <summary>
    /// UIForm's OnAfterValidate event handler.
    /// </summary>
    protected void EditForm_OnAfterValidate(object sender, EventArgs e)
    {
        SubscriberInfo subscriberObj = EditedObject as SubscriberInfo;

        if (subscriberObj != null)
        {
            string emailNew = ValidationHelper.GetString(EditForm.GetFieldValue("SubscriberEmail"), String.Empty);
            string emailCurrent = subscriberObj.SubscriberEmail;

            // Check email
            if (emailNew == string.Empty)
            {
                EditForm.StopProcessing = true;
                ShowError(GetString("Subscriber_Edit.ErrorEmptyEmail"));
            }
            else if ((emailNew.ToLowerCSafe() != emailCurrent.ToLowerCSafe()) && (SubscriberInfoProvider.EmailExists(emailNew)))
            {
                EditForm.StopProcessing = true;
                ShowError(GetString("Subscriber_Edit.EmailAlreadyExists"));
            }
        }
        else
        {
            EditForm.StopProcessing = true;
            ShowError(GetString("Subscriber_Edit.SubscriberDoesNotExists"));
        }
    }


    /// <summary>
    /// UIForm's OnBeforeSave event handler.
    /// </summary>
    protected void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        string firstName = ValidationHelper.GetString(EditForm.GetFieldValue("SubscriberFirstName"), string.Empty);
        string lastName = ValidationHelper.GetString(EditForm.GetFieldValue("SubscriberLastName"), string.Empty);

        // Set full name
        EditForm.Data.SetValue("SubscriberFullName", string.Concat(firstName, " ", lastName));

        // Reload header if changes were saved
        ScriptHelper.RefreshTabHeader(Page, ValidationHelper.GetString(EditForm.GetFieldValue("SubscriberEmail"), String.Empty));
    }
}