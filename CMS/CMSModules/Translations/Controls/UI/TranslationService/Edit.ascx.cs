using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.TranslationServices;

public partial class CMSModules_Translations_Controls_UI_TranslationService_Edit : CMSAdminEditControl
{
    #region "Properties"

    /// <summary>
    /// UIForm control used for editing objects properties.
    /// </summary>
    public UIForm UIFormControl
    {
        get
        {
            return this.EditForm;
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
            this.EditForm.StopProcessing = value;
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
        EditForm.OnAfterSave += new EventHandler(EditForm_OnAfterSave);

        if (!RequestHelper.IsPostBack())
        {
            ShowWarning();
        }
    }


    protected void EditForm_OnAfterSave(object sender, EventArgs e)
    {
        ShowWarning();
    }


    private void ShowWarning()
    {
        TranslationServiceInfo info = (TranslationServiceInfo)EditForm.EditedObject;
        if ((info != null) && info.TranslationServiceEnabled)
        {
            if (!TranslationServiceHelper.IsServiceAvailable(info.TranslationServiceName, SiteContext.CurrentSiteName))
            {
                ShowWarning(GetString("translationservice.notavailable"), null, null);
            }
        }
    }

    #endregion
}

