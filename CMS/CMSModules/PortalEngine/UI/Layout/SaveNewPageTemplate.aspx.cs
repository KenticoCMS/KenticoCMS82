using System;

using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_Layout_SaveNewPageTemplate : CMSModalDesignPage
{
    protected int pageTemplateId = 0;
    protected PageTemplateInfo pt = null;


    protected void Page_Load(object sender, EventArgs e)
    {
        pageTemplateId = QueryHelper.GetInteger("templateid", 0);
        if (pageTemplateId > 0)
        {
            pt = PageTemplateInfoProvider.GetPageTemplateInfo(pageTemplateId);
        }

        categorySelector.StartingPath = QueryHelper.GetString("startingpath", String.Empty);

        bool keep = QueryHelper.GetBoolean("assign", true);
        if (!keep)
        {
            chkKeep.Enabled = false;
            chkKeep.Checked = false;
        }

        // Check the authorization per UI element
        var currentUser = MembershipContext.AuthenticatedUser;
        if (!currentUser.IsAuthorizedPerUIElement("CMS.Content", new[] { "Properties", "Properties.Template", "Template.SaveAsNew" }, SiteContext.CurrentSiteName))
        {
            RedirectToUIElementAccessDenied("CMS.Content", "Properties;Properties.Template;Template.SaveAsNew");
        }

        PageTitle.TitleText = GetString("PortalEngine.SaveNewPageTemplate.PageTitle");
        // Preset category
        if (!RequestHelper.IsPostBack())
        {
            if (pt != null)
            {
                categorySelector.Value = pt.CategoryID.ToString();
                if (pt.IsReusable)
                {
                    plcKeep.Visible = false;
                }
            }
        }
    }


    protected void btnOK_Click(object sender, EventArgs e)
    {
        if (pt != null)
        {
            // Limit text length
            txtTemplateDisplayName.Text = TextHelper.LimitLength(txtTemplateDisplayName.Text.Trim(), 200, "");

            // finds whether required fields are not empty
            string result = new Validator()
                .NotEmpty(txtTemplateDisplayName.Text, GetString("Administration-PageTemplate_General.ErrorEmptyTemplateDisplayName"))
                .NotEmpty(txtTemplateCodeName.Text, GetString("Administration-PageTemplate_General.ErrorEmptyTemplateCodeName"))
                .IsCodeName(txtTemplateCodeName.Text, GetString("general.invalidcodename"))
                .Result;

            if (String.IsNullOrEmpty(result))
            {
                // Check if template with given name already exists            
                if (PageTemplateInfoProvider.PageTemplateNameExists(txtTemplateCodeName.Text))
                {
                    ShowError(GetString("general.codenameexists"));
                }

                bool templateCloned = false;

                if (pt.IsReusable || !chkKeep.Checked)
                {
                    // Clone template with clear
                    pt = pt.Clone(true);
                    templateCloned = true;
                }

                // Moving an ad-hoc template to a reusable template
                if (!pt.IsReusable)
                {
                    // Transfer template layout from file system to the database object to ensure that the new layout file (created afterwards) will contain the correct content
                    pt.PageTemplateLayout = pt.PageTemplateLayout;
                }

                pt.CodeName = txtTemplateCodeName.Text;
                pt.DisplayName = txtTemplateDisplayName.Text;
                pt.Description = txtTemplateDescription.Text;

                pt.CategoryID = Convert.ToInt32(categorySelector.Value);

                // Reset the Ad-hoc status
                pt.IsReusable = true;
                pt.PageTemplateNodeGUID = Guid.Empty;

                pt.PageTemplateSiteID = 0;

                if (templateCloned)
                {
                    // After all properties were set, reset object original values in order to behave as a new object. This ensures that a new layout file is created and the original one is not deleted.
                    pt.ResetChanges();
                }

                try
                {
                    PageTemplateInfoProvider.SetPageTemplateInfo(pt);
                    int siteId = QueryHelper.GetInteger("siteid", 0);
                    if (siteId > 0)
                    {
                        PageTemplateInfoProvider.AddPageTemplateToSite(pt.PageTemplateId, siteId);
                    }

                    if (!chkKeep.Checked)
                    {
                        ShowInformation(GetString("PortalEngine.SaveNewPageTemplate.Saved"));

                        txtTemplateCodeName.Text = pt.CodeName;

                        pnlContent.Enabled = false;
                        btnOk.Visible = false;
                    }
                    else
                    {
                        string script;

                        bool refresh = QueryHelper.GetBoolean("refresh", false);
                        if (refresh)
                        {
                            script = "wopener.location.replace(wopener.location); CloseDialog();";
                        }
                        else
                        {
                            String selId = QueryHelper.GetString("selectorID", String.Empty);
                            script = "SelectTemplate(" + pt.PageTemplateId + "," + ScriptHelper.GetString(selId) + ");";
                        }

                        ltlScript.Text = ScriptHelper.GetScript(script);
                    }
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
            else
            {
                ShowError(result);
            }
        }
    }
}