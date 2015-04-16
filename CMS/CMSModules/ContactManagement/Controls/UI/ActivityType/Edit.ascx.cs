using System;

using CMS.FormEngine;
using CMS.Base;
using CMS.UIControls;

using UniSelector = CMSAdminControls_UI_UniSelector_UniSelector;

public partial class CMSModules_ContactManagement_Controls_UI_ActivityType_Edit : CMSAdminEditControl
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!SystemContext.DevelopmentMode)
        {
            EditForm.FieldsToHide.Add("ActivityTypeIsCustom");
        }
        InitControlSelectors();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Init UniSelectors for selecting controls.
    /// </summary>
    private void InitControlSelectors()
    {
        var detailsControlSelector = (UniSelector)EditForm.FieldControls["activitytypedetailformcontrol"];
        var mainControlSelector = (UniSelector)EditForm.FieldControls["activitytypemainformcontrol"];

        // Add additional item for selecting default control - currently used only for 'Custom Activity'
        SpecialFieldsDefinition specialFields = new SpecialFieldsDefinition();
        specialFields.Add(new SpecialField()
        {
            Text = GetString("general.defaultchoice"),
            Value = "##default##"
        });

        detailsControlSelector.SpecialFields = mainControlSelector.SpecialFields = specialFields;

        // Set (None) values to save to database
        detailsControlSelector.NoneRecordValue = "";
        mainControlSelector.NoneRecordValue = "";
    }

    #endregion
}