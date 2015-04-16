using System;

using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.UIControls;

[Security(Resource = "CMS.ABTest", UIElements = "ABTestListing")]
[UIElement("CMS.ABTest", "ABTestListing")]
public partial class CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABTest_List : CMSABTestPage
{
    /// <summary>
    /// If true, the items are edited in dialog
    /// </summary>
    private bool EditInDialog
    {
        get
        {
            return listElem.Grid.EditInDialog;
        }
        set
        {
            listElem.Grid.EditInDialog = value;
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EditInDialog = QueryHelper.GetBoolean("editindialog", false);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;CMSABTestingEnabled;CMSAnalyticsTrackConversions";
        ucDisabledModule.ParentPanel = pnlDisabled;

        InitHeaderActions();
        InitTitle();
    }


    /// <summary>
    /// Initializes header actions.
    /// </summary>
    private void InitHeaderActions()
    {
        if (MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.ABTest", "New"))
        {
            string url = UIContextHelper.GetElementUrl("CMS.ABTest", "New", EditInDialog);

            // Get the alias path of the current node, if in content
            if (Node != null)
            {
                listElem.NodeID = Node.NodeID;
                listElem.ShowOriginalPageColumn = false;
                string aliasPath = Node.NodeAliasPath;
                listElem.AliasPath = aliasPath;

                url = URLHelper.AddParameterToUrl(url, "NodeID", Node.NodeID.ToString());
                url = URLHelper.AddParameterToUrl(url, "AliasPath", aliasPath);
            }

            url = ResolveUrl(url);

            // Set header action
            var action = new HeaderAction
            {
                ResourceName = "CMS.ABTest",
                Permission = "Manage",
                Text = GetString("abtesting.abtest.new"),
                RedirectUrl = url,
                OpenInDialog = EditInDialog
            };

            CurrentMaster.HeaderActions.AddAction(action);
        }
    }


    /// <summary>
    /// Sets title if not in content.
    /// </summary>
    private void InitTitle()
    {
        if (NodeID <= 0)
        {
            SetTitle(GetString("analytics_codename.abtests"));
        }
    }
}