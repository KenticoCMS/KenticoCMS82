﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CMS;
using CMS.Base;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Membership;


[assembly: RegisterCustomClass("LicenseNewControlExtender", typeof(LicenseNewControlExtender))]

/// <summary>
/// License new page extender.
/// </summary>
public class LicenseNewControlExtender : ControlExtender<UIForm>
{
    public override void OnInit()
    {
        Control.OnAfterValidate += Control_OnAfterValidate;
    }


    private void Control_OnAfterValidate(object sender, EventArgs e)
    {
        try
        {
            string licenseKey = ValidationHelper.GetString(Control.GetFieldValue("LicenseKey"), String.Empty).Trim();
            LicenseKeyInfo lk = new LicenseKeyInfo();
            lk.LoadLicense(licenseKey, "");

            switch (lk.ValidationResult)
            {
                case LicenseValidationEnum.Valid:
                    using (new CMSActionContext { AllowLicenseRedirect = false })
                    {
                        UserInfoProvider.ClearLicenseValues();
                        Functions.ClearHashtables();
                    }
                    return;

                case LicenseValidationEnum.Expired:
                    Control.ValidationErrorMessage = ResHelper.GetString("Licenses_License_New.LicenseNotValid.Expired");
                    break;

                case LicenseValidationEnum.Invalid:
                    Control.ValidationErrorMessage = ResHelper.GetString("Licenses_License_New.LicenseNotValid.Invalid");
                    break;

                case LicenseValidationEnum.NotAvailable:
                    Control.ValidationErrorMessage = ResHelper.GetString("Licenses_License_New.LicenseNotValid.NotAvailable");
                    break;

                case LicenseValidationEnum.WrongFormat:
                    Control.ValidationErrorMessage = ResHelper.GetString("Licenses_License_New.LicenseNotValid.WrongFormat");
                    break;
            }

            Control.StopProcessing = true;
            Control.ShowValidationErrorMessage = true;
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("License", "NEW", ex);
            Control.ValidationErrorMessage = ResHelper.GetString("general.saveerror");
        }
    }
}