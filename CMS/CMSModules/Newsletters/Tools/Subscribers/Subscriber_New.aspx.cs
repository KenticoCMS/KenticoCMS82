using System;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.DataEngine;

[Title("newsletters.subscribers")]
[Breadcrumb(0, ResourceString = "Subscriber_Edit.ItemListLink", TargetUrl = "~/CMSModules/Newsletters/Tools/Subscribers/Subscriber_List.aspx")]
[Breadcrumb(1, ResourceString = "Subscriber_Edit.NewItemCaption")]
[EditedObject("newsletter.subscriber", "objectid")]
[UIElement("cms.newsletter", "Newsletters.SubscriberProperties")]
public partial class CMSModules_Newsletters_Tools_Subscribers_Subscriber_New : CMSNewsletterPage
{
    #region "Properties"

    protected SubscriberInfo TypedEditedObject
    {
        get
        {
            return EditedObject as SubscriberInfo;
        }
    }

    #endregion


    #region "Life-cycle methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        SubscriberInfo sub = EditedObject as SubscriberInfo;

        // Render subscriber email as required
        LocalizedLabel subscriberEmailLabel;
        if ((sub != null) && ((subscriberEmailLabel = NewForm.FieldLabels["SubscriberEmail"]) != null))
        {
            subscriberEmailLabel.ShowRequiredMark = true;
        }
    }

    #endregion


    #region "Events"

    protected void OnBeforeSave(object sender, EventArgs e)
    {
        // Check license
        if (!SubscriberInfoProvider.LicenseVersionCheck(RequestContext.CurrentDomain, FeatureEnum.Subscribers, ObjectActionEnum.Insert))
        {
            NewForm.StopProcessing = true;

            ShowError(GetString("LicenseVersionCheck.Subscribers"));
        }

        // Set SiteID
        NewForm.Data.SetValue("SubscriberSiteID", SiteContext.CurrentSiteID);

        string firstName = ValidationHelper.GetString(NewForm.GetFieldValue("SubscriberFirstName"), string.Empty);
        string lastName = ValidationHelper.GetString(NewForm.GetFieldValue("SubscriberLastName"), string.Empty);

        // Set full name
        NewForm.Data.SetValue("SubscriberFullName", string.Concat(firstName, " ", lastName));

        string email = ValidationHelper.GetString(NewForm.GetFieldValue("SubscriberEmail"), string.Empty);

        // Check email
        if (email == string.Empty)
        {
            NewForm.StopProcessing = true;

            ShowError(GetString("Subscriber_Edit.ErrorEmptyEmail"));
        }
        else if (SubscriberInfoProvider.EmailExists(email))
        {
            NewForm.StopProcessing = true;

            ShowError(GetString("Subscriber_Edit.EmailAlreadyExists"));
        }
    }


    protected void OnAfterSave(object sender, EventArgs e)
    {
        if (TypedEditedObject != null)
        {
            var url = UIContextHelper.GetElementUrl("cms.newsletter", "Newsletters.SubscriberProperties", false, ValidationHelper.GetInteger(TypedEditedObject.SubscriberID, 0));
            url = URLHelper.AddParameterToUrl(url, "saved", "1");

            URLHelper.Redirect(url);
        }
    }

    #endregion
}