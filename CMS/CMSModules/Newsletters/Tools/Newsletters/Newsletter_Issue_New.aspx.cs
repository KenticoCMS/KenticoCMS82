using System;
using System.Web.UI.WebControls;
using System.Linq;

using CMS.Core;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;

[Security(Resource = "CMS.Newsletter", UIElements = "Newsletters;Newsletter;EditNewsletterProperties;Newsletter.Issues;EditIssueProperties")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_New : CMSNewsletterPage
{
    /// <summary>
    /// Messages placeholder.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMessages;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        int newsletterId = QueryHelper.GetInteger("parentobjectid", 0);
        var newsletter = NewsletterInfoProvider.GetNewsletterInfo(newsletterId);
        if (newsletter == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        var siteName = SiteInfoProvider.GetSiteName(newsletter.NewsletterSiteID);
        if (!CurrentUser.IsAuthorizedPerResource(newsletter.TypeInfo.ModuleName, "AuthorIssues", siteName))
        {
            RedirectToAccessDenied(newsletter.TypeInfo.ModuleName, "AuthorIssues");
        }

        // Remove extra padding set by the master page
        CurrentMaster.PanelContent.CssClass = string.Empty;

        // Ensure correct padding
        MessagesPlaceHolder.OffsetX = 16;
        MessagesPlaceHolder.OffsetY = 8;

        editElem.NewsletterID = newsletterId;

        InitHeaderActions();
    }


    /// <summary>
    /// Initializes header action control.
    /// </summary>
    private void InitHeaderActions()
    {
        CurrentMaster.HeaderActions.ActionsList.Clear();

        // Init save button
        CurrentMaster.HeaderActions.ActionsList.Add(new SaveAction(this)
        {
            OnClientClick = "if (GetContent != null) {return GetContent();} else {return false;}"
        });

        // Ensure spell check action
        CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("EditMenu.IconSpellCheck"),
            Tooltip = GetString("EditMenu.SpellCheck"),
            OnClientClick = "var frame = GetFrame(); if ((frame != null) && (frame.contentWindow.SpellCheck_" + ClientID + " != null)) {frame.contentWindow.SpellCheck_" + ClientID + "();} return false;",
            ButtonStyle = ButtonStyle.Default,
        });

        CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        CurrentMaster.HeaderActions.ReloadData();
        CurrentMaster.HeaderActions.Attributes.Add("onmouseover", "if (RememberFocusedRegion) {RememberFocusedRegion();}");
    }


    /// <summary>
    /// Actions handler.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        if (e.CommandName.ToLowerInvariant() == "save")
        {

            if (editElem.Save())
            {
                string url = UIContextHelper.GetElementUrl(ModuleName.NEWSLETTER, "EditIssueProperties");
                url = URLHelper.AddParameterToUrl(url, "tabname", "Newsletter.Issue.Content");
                url = URLHelper.AddParameterToUrl(url, "objectid", editElem.IssueID.ToString());
                url = URLHelper.PropagateUrlParameters(url, "parentobjectid");
                url = UIContextHelper.AppendDialogHash(url);
                URLHelper.Redirect(url);
            }
            else
            {
                MessagesPlaceHolder.ShowError(editElem.ErrorMessage);

                // Refresh content of editable regions in the issue body
                editElem.RefreshEditableRegions();
            }
        }
    }
}