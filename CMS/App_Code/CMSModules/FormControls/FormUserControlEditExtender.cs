using System;
using System.Linq;
using System.Collections.Generic;

using CMS;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;

[assembly: RegisterCustomClass("FormUserControlEditExtender", typeof(FormUserControlEditExtender))]

/// <summary>
/// Form User Control UIForm extender
/// </summary>
public class FormUserControlEditExtender : ControlExtender<UIForm>
{
    #region "Constants"

    private const string FOR_TEXT = "text";
    private const string FOR_LONG_TEXT = "ltext";
    private const string FOR_INT = "int";
    private const string FOR_LONG_INT = "lint";
    private const string FOR_DECIMAL = "decimal";
    private const string FOR_DATE = "datetime";
    private const string FOR_BOOL = "bool";
    private const string FOR_GUID = "guid";
    private const string FOR_FILE = "file";
    private const string FOR_ATTACH = "docatt";
    private const string FOR_VISIBILITY = "vis";

    private const string SHOWIN_PAGETYPE = "pagetype";
    private const string SHOWIN_FORM = "form";
    private const string SHOWIN_CUSTOMTABLE = "customtable";
    private const string SHOWIN_SYSTEMTABLE = "systemtable";
    private const string SHOWIN_REPORT = "report";
    private const string SHOWIN_CONTROL = "control";

    private const string FIELD_PRIORITY = "UserControlPriorityBool";
    private const string FIELD_FOR = "UserControlFor";
    private const string FIELD_SHOWIN = "UserControlShowIn";
    private const string FIELD_SHOWIN2 = "UserControlShowIn2";

    #endregion


    #region "Control events"

    public override void OnInit()
    {
        Control.OnAfterDataLoad += Control_OnAfterDataLoad;
        Control.OnBeforeSave += Control_OnBeforeSave;
    }


    private void Control_OnAfterDataLoad(object sender, EventArgs e)
    {
        UIForm form = (UIForm)sender;
        if (form.IsInsertMode)
        {
            return;
        }

        FormUserControlInfo formControl = form.EditedObject as FormUserControlInfo;
        if (formControl != null)
        {
            // Set control's priority
            var priorityControl = form.FieldControls[FIELD_PRIORITY];
            if (priorityControl != null)
            {
                priorityControl.Value = (formControl.UserControlPriority == (int)ObjectPriorityEnum.High);
            }

            // Set which (data) types the control can be used for
            var controlFor = form.FieldControls[FIELD_FOR];
            if (controlFor != null)
            {
                controlFor.Value = GetControlForValue(formControl);
            }

            // Set which resources the control can be shown in
            var controlShowIn = form.FieldControls[FIELD_SHOWIN];
            if (controlShowIn != null)
            {
                controlShowIn.Value = GetControlShowInValue(formControl, true);
            }
            var controlShowIn2 = form.FieldControls[FIELD_SHOWIN2];
            if (controlShowIn2 != null)
            {
                controlShowIn2.Value = GetControlShowInValue(formControl, false);
            }
        }
    }


    private void Control_OnBeforeSave(object sender, EventArgs e)
    {
        UIForm form = (UIForm)sender;
        FormUserControlInfo formControl = form.EditedObject as FormUserControlInfo;
        if (formControl != null)
        {
            if (form.IsInsertMode)
            {
                FormEngineUserControl fileName = form.FieldControls["UserControlFileName"];
                FormEngineUserControl parentId = form.FieldControls["UserControlParentID"];

                if ((fileName != null) && (parentId != null) && fileName.Visible && !parentId.Visible && (ValidationHelper.GetInteger(parentId.Value, 0) > 0))
                {
                    // Reset inheritance setting if it's not visible
                    formControl.SetValue("UserControlParentID", null);
                }
            }
            else
            {
                // Set control's priority
                formControl.UserControlPriority = ValidationHelper.GetBoolean(form.GetFieldValue(FIELD_PRIORITY), false) ? (int)ObjectPriorityEnum.High : (int)ObjectPriorityEnum.Low;

                // Set which (data) types the control can be used for
                List<string> values = GetFieldValues(form, FIELD_FOR);
                formControl.UserControlForText = values.Contains(FOR_TEXT);
                formControl.UserControlForLongText = values.Contains(FOR_LONG_TEXT);
                formControl.UserControlForInteger = values.Contains(FOR_INT);
                formControl.UserControlForLongInteger = values.Contains(FOR_LONG_INT);
                formControl.UserControlForDecimal = values.Contains(FOR_DECIMAL);
                formControl.UserControlForDateTime = values.Contains(FOR_DATE);
                formControl.UserControlForBoolean = values.Contains(FOR_BOOL);
                formControl.UserControlForGUID = values.Contains(FOR_GUID);
                formControl.UserControlForFile = values.Contains(FOR_FILE);
                formControl.UserControlForDocAttachments = values.Contains(FOR_ATTACH);
                formControl.UserControlForVisibility = values.Contains(FOR_VISIBILITY);

                // Set which resources the control can be shown in
                values = GetFieldValues(form, FIELD_SHOWIN);
                formControl.UserControlShowInDocumentTypes = values.Contains(SHOWIN_PAGETYPE);
                formControl.UserControlShowInBizForms = values.Contains(SHOWIN_FORM);
                values = GetFieldValues(form, FIELD_SHOWIN2);
                formControl.UserControlShowInCustomTables = values.Contains(SHOWIN_CUSTOMTABLE);
                formControl.UserControlShowInSystemTables = values.Contains(SHOWIN_SYSTEMTABLE);
                formControl.UserControlShowInReports = values.Contains(SHOWIN_REPORT);
                formControl.UserControlShowInWebParts = values.Contains(SHOWIN_CONTROL);
            }
        }
    }

    #endregion


    #region "Private methods"

    private static string GetControlForValue(FormUserControlInfo formControl)
    {
        List<string> values = new List<string>();
        if (formControl.UserControlForText)
        {
            values.Add(FOR_TEXT);
        }
        if (formControl.UserControlForLongText)
        {
            values.Add(FOR_LONG_TEXT);
        }
        if (formControl.UserControlForInteger)
        {
            values.Add(FOR_INT);
        }
        if (formControl.UserControlForLongInteger)
        {
            values.Add(FOR_LONG_INT);
        }
        if (formControl.UserControlForDecimal)
        {
            values.Add(FOR_DECIMAL);
        }
        if (formControl.UserControlForDateTime)
        {
            values.Add(FOR_DATE);
        }
        if (formControl.UserControlForBoolean)
        {
            values.Add(FOR_BOOL);
        }
        if (formControl.UserControlForGUID)
        {
            values.Add(FOR_GUID);
        }
        if (formControl.UserControlForFile)
        {
            values.Add(FOR_FILE);
        }
        if (formControl.UserControlForDocAttachments)
        {
            values.Add(FOR_ATTACH);
        }
        if (formControl.UserControlForVisibility)
        {
            values.Add(FOR_VISIBILITY);
        }

        return String.Join("|", values);
    }


    private static string GetControlShowInValue(FormUserControlInfo formControl, bool firstSet)
    {
        List<string> values = new List<string>();
        if (firstSet)
        {
            if (formControl.UserControlShowInDocumentTypes)
            {
                values.Add(SHOWIN_PAGETYPE);
            }
            if (formControl.UserControlShowInBizForms)
            {
                values.Add(SHOWIN_FORM);
            }
        }
        else
        {
            if (formControl.UserControlShowInCustomTables)
            {
                values.Add(SHOWIN_CUSTOMTABLE);
            }
            if (formControl.UserControlShowInSystemTables)
            {
                values.Add(SHOWIN_SYSTEMTABLE);
            }
            if (formControl.UserControlShowInReports)
            {
                values.Add(SHOWIN_REPORT);
            }
            if (formControl.UserControlShowInWebParts)
            {
                values.Add(SHOWIN_CONTROL);
            }
        }

        return String.Join("|", values);
    }


    private List<string> GetFieldValues(UIForm form, string fieldName)
    {
        return ValidationHelper.GetString(form.GetFieldValue(fieldName), string.Empty)
                               .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                               .ToList();
    }

    #endregion
}