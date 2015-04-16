using System;
using System.Data;

using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.OnlineForms;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.Membership;

[UIElement("CMS.Form", "Form")]
public partial class CMSModules_BizForms_Tools_BizForm_List : CMSBizFormPage
{
    private CurrentUserInfo currentUser;


    protected void Page_Load(object sender, EventArgs e)
    {
        currentUser = MembershipContext.AuthenticatedUser;

        if (currentUser == null)
        {
            return;
        }

        // Check 'ReadForm' permission
        if (!currentUser.IsAuthorizedPerResource("cms.form", "ReadForm"))
        {
            RedirectToAccessDenied("cms.form", "ReadForm");
        }

        UniGridBizForms.OnAction += UniGridBizForms_OnAction;
        UniGridBizForms.OnAfterRetrieveData += uniGrid_OnAfterRetrieveData;
        UniGridBizForms.HideControlForZeroRows = false;
        UniGridBizForms.ZeroRowsText = GetString("general.nodatafound");
        UniGridBizForms.WhereCondition = "FormSiteID = " + SiteContext.CurrentSiteID;

        PageTitle.TitleText = GetString("BizFormList.TitleText");

        InitHeaderActions();
    }


    private void InitHeaderActions()
    {
        HeaderActions.AddAction(new HeaderAction
        {
            Text = GetString("BizFormList.lnkNewBizForm"),
            Tooltip = GetString("BizFormList.lnkNewBizForm"),
            RedirectUrl = "BizForm_New.aspx",
            ResourceName = "cms.form",
            Permission = "CreateForm"
        });
    }


    private DataSet uniGrid_OnAfterRetrieveData(DataSet ds)
    {
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            int i = 0;
            while (i < ds.Tables[0].Rows.Count)
            {
                BizFormInfo form = new BizFormInfo(ds.Tables[0].Rows[i]);
                if (!form.IsFormAllowedForUser(currentUser, SiteContext.CurrentSiteName))
                {
                    ds.Tables[0].Rows.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        return ds;
    }


    protected void UniGridBizForms_OnAction(string actionName, object actionArgument)
    {
        if (DataHelper.GetNotEmpty(actionName, String.Empty) == "edit")
        {
            URLHelper.Redirect(UIContextHelper.GetElementUrl("CMS.Form", "Forms.Properties", false, Convert.ToInt32(actionArgument)));
        }
        if (DataHelper.GetNotEmpty(actionName, "") == "delete")
        {
            // Check 'DeleteFormAndData' permission
            if (!currentUser.IsAuthorizedPerResource("cms.form", "DeleteFormAndData"))
            {
                RedirectToAccessDenied("cms.form", "DeleteFormAndData");
            }

            // Delete Bizform
            BizFormInfoProvider.DeleteBizFormInfo(ValidationHelper.GetInteger(actionArgument, 0));
        }
    }
}