using System;

using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.UIControls;
using CMS.Base;
using CMS.Membership;
using CMS.PortalEngine;

public partial class CMSInstall_Controls_WizardSteps_LicenseDialog : CMSUserControl
{
    private string mFullHostName = null;


    /// <summary>
    /// Host name.
    /// </summary>
    protected string FullHostName
    {
        get
        {
            if (mFullHostName == null)
            {
                mFullHostName = RequestContext.CurrentDomain;
                if (mFullHostName.StartsWithCSafe("www."))
                {
                    mFullHostName = mFullHostName.Substring(4);
                }
            }
            return mFullHostName;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        lnkSkipLicense.Click += new EventHandler(lnkSkipLicense_Click);

        if (!RequestHelper.IsPostBack())
        {
            lblLicenseCaption.Text = string.Format(ResHelper.GetFileString("Install.lblLicenseCaption"), FullHostName);
        }

        // License step strings
        lblLicenseTip.Text = string.Format(ResHelper.GetFileString("Install.lblLicenseTip"), FullHostName);
        lblEnterLicenseKey.Text = ResHelper.GetFileString("Install.lblEnterLicenseKey");
        lnkSkipLicense.Text = ResHelper.GetFileString("Install.lnkSkipLicense");
        lblFreeLicenseInfo.Text = ResHelper.GetFileString("Install.lblFreeLicenseInfo");
    }


    private void lnkSkipLicense_Click(object sender, EventArgs e)
    {
        AuthenticationHelper.AuthenticateUser("administrator", false);
        URLHelper.Redirect(UIContextHelper.GetApplicationUrl("cms", "sites", "action=new"));
    }


    /// <summary>
    /// Try to set new license for actual domain.
    /// </summary>
    public void SetLicenseKey()
    {
        if (txtLicense.Text == "")
        {
            throw new Exception(ResHelper.GetFileString("Install.LicenseEmpty"));
        }

        LicenseKeyInfo lki = new LicenseKeyInfo();
        lki.LoadLicense(txtLicense.Text, FullHostName);

        switch (lki.ValidationResult)
        {
            case LicenseValidationEnum.Expired:
                throw new Exception(ResHelper.GetFileString("Install.LicenseNotValid.Expired"));
            case LicenseValidationEnum.Invalid:
                throw new Exception(ResHelper.GetFileString("Install.LicenseNotValid.Invalid"));
            case LicenseValidationEnum.NotAvailable:
                throw new Exception(ResHelper.GetFileString("Install.LicenseNotValid.NotAvailable"));
            case LicenseValidationEnum.WrongFormat:
                throw new Exception(ResHelper.GetFileString("Install.LicenseNotValid.WrongFormat"));
            case LicenseValidationEnum.Valid:
                // Try to store license into database
                if ((FullHostName == "localhost") || (FullHostName == "127.0.0.1") || (lki.Domain == FullHostName))
                {
                    LicenseKeyInfoProvider.SetLicenseKeyInfo(lki);
                }
                else
                {
                    throw new Exception(ResHelper.GetFileString("Install.LicenseForDifferentDomain"));
                }
                break;
        }
    }


    /// <summary>
    /// Sets license key is expired.
    /// </summary>
    public void SetLicenseExpired()
    {
        lblLicenseCaption.Text = string.Format(ResHelper.GetFileString("Install.lblLicenseCaptionExpired"), FullHostName);
    }
}