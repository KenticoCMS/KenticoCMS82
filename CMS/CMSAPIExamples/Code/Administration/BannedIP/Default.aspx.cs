using System;
using System.Data;

using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.Protection;
using CMS.DataEngine;

public partial class CMSAPIExamples_Code_Administration_BannedIP_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check license
        LicenseHelper.CheckFeatureAndRedirect(RequestContext.CurrentDomain, FeatureEnum.BannedIP);

        // Banned ip
        apiCreateBannedIp.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateBannedIp);
        apiGetAndUpdateBannedIp.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateBannedIp);
        apiGetAndBulkUpdateBannedIps.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateBannedIps);
        apiDeleteBannedIp.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteBannedIp);
        apiCheckBannedIp.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CheckBannedIp);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Banned ip
        apiCreateBannedIp.Run();
        apiGetAndUpdateBannedIp.Run();
        apiGetAndBulkUpdateBannedIps.Run();
        apiCheckBannedIp.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Banned ip
        apiDeleteBannedIp.Run();
    }

    #endregion


    #region "API examples - Banned IP"

    /// <summary>
    /// Creates banned ip. Called when the "Create ip" button is pressed.
    /// </summary>
    private bool CreateBannedIp()
    {
        // Create new banned ip object
        BannedIPInfo newIp = new BannedIPInfo();

        // Set the properties
        newIp.IPAddress = "MyNewIp";
        newIp.IPAddressBanReason = "Ban reason";
        newIp.IPAddressAllowed = true;
        newIp.IPAddressAllowOverride = true;
        newIp.IPAddressBanType = BannedIPInfoProvider.BanControlEnumString(BanControlEnum.AllNonComplete);
        newIp.IPAddressBanEnabled = true;

        // Save the banned IP
        BannedIPInfoProvider.SetBannedIPInfo(newIp);

        return true;
    }


    /// <summary>
    /// Gets and updates banned IP. Called when the "Get and update IP" button is pressed.
    /// Expects the CreateBannedIp method to be run first.
    /// </summary>
    private bool GetAndUpdateBannedIp()
    {
        // Prepare the parameters
        string where = "IPAddress LIKE N'MyNewIp%'";

        // Get object from database
        BannedIPInfo modifyIp = BannedIPInfoProvider.GetBannedIPs().Where(where).FirstResult().FirstObject;
        if (modifyIp != null)
        {
            // Update the properties
            modifyIp.IPAddress = modifyIp.IPAddress.ToLowerCSafe();

            // Save the changes
            BannedIPInfoProvider.SetBannedIPInfo(modifyIp);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates banned IPs. Called when the "Get and bulk update ips" button is pressed.
    /// Expects the CreateBannedIp method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateBannedIps()
    {
        // Prepare the parameters
        string where = "IPAddress LIKE N'MyNewIp%'";

        // Get objects from database
        var bannedIPs = BannedIPInfoProvider.GetBannedIPs().Where(where);

        // Loop through the individual items
        foreach (BannedIPInfo modifyIp in bannedIPs)
        {
            // Update the properties
            modifyIp.IPAddress = modifyIp.IPAddress.ToUpper();

            // Save the changes
            BannedIPInfoProvider.SetBannedIPInfo(modifyIp);
        }

        // Return TRUE if any object was found and updated, FALSE otherwise
        return (bannedIPs.Count > 0);
    }


    /// <summary>
    /// Deletes banned ip. Called when the "Delete ip" button is pressed.
    /// Expects the CreateBannedIp method to be run first.
    /// </summary>
    private bool DeleteBannedIp()
    {
        // Prepare the parameters
        string where = "IPAddress LIKE N'MyNewIp%'";

        // Get object from database
        BannedIPInfo deleteIp = BannedIPInfoProvider.GetBannedIPs().Where(where).FirstResult().FirstObject;
        if (deleteIp != null)
        {
            // Delete the banned ip
            BannedIPInfoProvider.DeleteBannedIPInfo(deleteIp);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Checks banned ip if current action is allowed. Called when the "Check banned IP for action" button is pressed.
    /// Expects the CreateBannedIp method to be run first.
    /// </summary>
    private bool CheckBannedIp()
    {
        // Prepare the parameters
        string where = "IPAddress LIKE N'MyNewIp%'";

        // Get object from database
        BannedIPInfo checkIp = BannedIPInfoProvider.GetBannedIPs().Where(where).FirstResult().FirstObject;

        // Check if IP is allowed
        if ((checkIp == null) || !BannedIPInfoProvider.IsAllowed(checkIp.IPAddress, SiteContext.CurrentSiteName, BanControlEnum.AllNonComplete))
        {
            return false;
        }

        return true;
    }

    #endregion
}