using System;

using CMS.FormControls;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

public partial class CMSModules_WebAnalytics_Controls_UI_Conversion_Edit : CMSAdminEditControl
{
    #region "Variables"

    private String oldConversionName = String.Empty;

    #endregion


    #region "Properties"

    /// <summary>
    /// UIForm control used for editing objects properties.
    /// </summary>
    public UIForm UIFormControl
    {
        get
        {
            return EditForm;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            EditForm.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            EditForm.IsLiveSite = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        EditForm.OnBeforeSave += EditForm_OnBeforeSave;
        EditForm.OnAfterSave += EditForm_OnAfterSave;
        bool modalDialog = QueryHelper.GetBoolean("modaldialog", false);

        if (modalDialog)
        {
            EditForm.SubmitButton.Visible = false;
            EditForm.RedirectUrlAfterCreate = "";
        }
        else
        {
            string editUrl = UIContextHelper.GetElementUrl("CMS.WebAnalytics", "ConversionProperties");
            editUrl = URLHelper.AddParameterToUrl(editUrl, "objectId", "{%EditedObject.ID%}");
            editUrl = URLHelper.AddParameterToUrl(editUrl, "saved", "1");
            editUrl = URLHelper.AddParameterToUrl(editUrl, "displayTitle", "0");
            EditForm.RedirectUrlAfterCreate = editUrl;
        }
    }


    private void EditForm_OnAfterSave(object sender, EventArgs e)
    {
        ConversionInfo ci = EditForm.EditedObject as ConversionInfo;
        // If code name has changed (on existing object) => Rename all analytics statistics data.
        if ((ci != null) && (ci.ConversionName != oldConversionName) && (oldConversionName != String.Empty))
        {
            ConversionInfoProvider.RenameConversionStatistics(oldConversionName, ci.ConversionName, SiteContext.CurrentSiteID);
        }
    }


    private void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        ConversionInfo ci = EditForm.EditedObject as ConversionInfo;
        if (ci != null)
        {
            ci.ConversionSiteID = SiteContext.CurrentSiteID;
            oldConversionName = ci.ConversionName;
        }
    }


    /// <summary>
    /// Saves the data
    /// </summary>
    /// <param name="redirect">If true, use server redirect after successful save</param>
    public bool Save(bool redirect)
    {
        string selectorID = QueryHelper.GetString("selectorID", String.Empty);

        bool ret = EditForm.SaveData("");

        // If saved - redirect with ConversionID parameter
        if ((ret) && (redirect))
        {
            ConversionInfo ci = (ConversionInfo)EditForm.EditedObject;
            if (ci != null)
            {
                URLHelper.Redirect("edit.aspx?conversionid=" + ci.ConversionID + "&saved=1&modaldialog=true&selectorID=" + selectorID);
            }
        }

        return ret;
    }

    #endregion
}