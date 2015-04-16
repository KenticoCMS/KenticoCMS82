using System;

using CMS.Core;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.SiteProvider;
using CMS.DataEngine;

[EditedObject(ConversionInfo.OBJECT_TYPE, "conversionid")]
[CheckLicence(FeatureEnum.CampaignAndConversions)]
[UIElement(ModuleName.WEBANALYTICS, "Conversions.General", false, true)]
public partial class CMSModules_WebAnalytics_Pages_Tools_Conversion_Edit : CMSDeskPage
{
    #region "Variables"

    private bool modalDialog;

    // Help variable for set info label of UI form
    private string infoText = String.Empty;

    #endregion


    #region "Page events"


    protected void Page_PreInit(object sender, EventArgs e)
    {
        modalDialog = QueryHelper.GetBoolean("modalDialog", false);
        if (modalDialog)
        {
            // Display in selector dialog
            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master";

            var master = CurrentMaster as ICMSModalMasterPage;
            if (master != null)
            {
                master.Save += (s, ea) => editElem.Save(true);
                master.ShowSaveAndCloseButton();
            }
        }
        IsDialog = modalDialog;
    }


    protected void Page_Init(object sender, EventArgs e)
    {
        ConversionInfo ci = EditedObject as ConversionInfo;
        
        string conversionName = QueryHelper.GetString("conversionName", String.Empty);
        if (conversionName != String.Empty)
        {
            // Try to check dialog mode
            conversionName = conversionName.Trim(';');
            ci = ConversionInfoProvider.GetConversionInfo(conversionName, SiteContext.CurrentSiteName);
        }
        
        // Test whether conversion is in current site, if not - test if user is authorized for conversion's site
        if (ci != null)
        {
            if (!ci.CheckPermissions(PermissionsEnum.Read, CurrentSiteName, CurrentUser))
            {
                RedirectToAccessDenied(ci.TypeInfo.ModuleName, "Read");
            }
        }

        if ((conversionName != String.Empty) && (ci == null))
        {
            // Set warning text
            infoText = String.Format(GetString("conversion.editedobjectnotexits"), HTMLHelper.HTMLEncode(conversionName));

            // Create new conversion info based on conversion name
            ci = new ConversionInfo();
            ci.ConversionName = conversionName;
            ci.ConversionDisplayName = conversionName;
        }

       
        if (modalDialog)
        {
            if (ci != null)
            {
                PageTitle.TitleText = GetString("analytics.conversion");
            }
            else
            {
                PageTitle.TitleText = GetString("conversion.conversion.new");
            }
        }

        if (ci != null)
        {
            EditedObject = ci;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        bool isNew = (EditedObject == null) || (((ConversionInfo)EditedObject).ConversionID <= 0);
        if (isNew && !modalDialog)
        {
            // Set the title
            PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem { Text = GetString("conversion.conversion.list"), RedirectUrl = "~/CMSModules/WebAnalytics/Pages/Tools/Conversion/List.aspx" });
            PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem { Text = GetString("conversion.conversion.new") });
        }

        // Set info label
        editElem.UIFormControl.ShowInformation(infoText);
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (QueryHelper.GetBoolean("saved", false) && !URLHelper.IsPostback())
        {
            UpdateUniSelector(true);
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Adds scripts to update parent's uniselector (used in modal dialogs)
    /// </summary>
    /// <param name="closeOnSave">If true, window close JS is added</param>
    private void UpdateUniSelector(bool closeOnSave)
    {
        string selector = HTMLHelper.HTMLEncode(QueryHelper.GetString("selectorid", string.Empty));
        ConversionInfo info = editElem.UIFormControl.EditedObject as ConversionInfo;
        if (!string.IsNullOrEmpty(selector) && (info != null))
        {
            ScriptHelper.RegisterWOpenerScript(this);
            // Add selector refresh
            string script =
                string.Format(@"if (wopener) {{ wopener.US_SelectNewValue_{0}('{1}'); }}", selector, info.ConversionName);

            if (closeOnSave)
            {
                script += "CloseDialog();";
            }

            ScriptHelper.RegisterStartupScript(this, GetType(), "UpdateSelector", script, true);
        }
    }

    #endregion
}