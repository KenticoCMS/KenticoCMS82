using System;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Reporting;


// Edited object
[EditedObject(ReportSubscriptionInfo.OBJECT_TYPE, "subscriptionID")]

// Breadcrumbs
[Breadcrumbs]
[Breadcrumb(0, "Report_Header.Subscriptions", "list.aspx?reportID={?reportID?}", null)]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
[Breadcrumb(1, ResourceString = "reportsubscription.new", NewObject = true)]
public partial class CMSModules_Reporting_Tools_Subscription_Edit : CMSReportingPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.HeaderActions.AddAction(new SaveAction(Page));
        CurrentMaster.HeaderActions.ActionPerformed += new CommandEventHandler(HeaderActions_ActionPerformed);
    }


    void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case ComponentEvents.SAVE:
                editElem.Save();
                break;
        }
    }
}
