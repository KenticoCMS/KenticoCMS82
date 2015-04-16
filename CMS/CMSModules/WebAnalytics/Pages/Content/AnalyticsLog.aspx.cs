using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

public partial class CMSModules_WebAnalytics_Pages_Content_AnalyticsLog : CMSPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // If analytics not enabled - no log
        if (!AnalyticsHelper.IsLoggingEnabled(SiteContext.CurrentSiteName, String.Empty))
        {
            return;
        }

        string data = QueryHelper.GetString("data", String.Empty);
        string guid = ValidationHelper.GetString(SessionHelper.GetValue("BrowserCapatibilities"), String.Empty);
        string urlGuid = QueryHelper.GetString("guid", String.Empty);

        // Compares GUIDs to prevent false data
        if ((String.IsNullOrEmpty(guid) || String.IsNullOrEmpty(urlGuid) || (guid != urlGuid)))
        {
            return;
        }

        if (!String.IsNullOrEmpty(data))
        {
            String siteName = SiteContext.CurrentSiteName;
            String cultureCode = CultureHelper.GetPreferredCulture();
            string[] values = data.Split(';');
            if (values.Length == 7)
            {
                // Resolution
                if (!String.IsNullOrEmpty(values[0]) && !String.IsNullOrEmpty(values[1])
                    && ValidationHelper.IsInteger(values[0]) && ValidationHelper.IsInteger(values[1]))
                {
                    string res = values[0] + "x" + values[1];
                    HitLogProvider.LogHit(HitLogProvider.SCREENRESOLUTION, siteName, cultureCode, res, 0);
                    CMSDataContext.Current.BrowserHelper.ScreenResolution = res;
                }

                // Color depth
                if (!String.IsNullOrEmpty(values[2]) && ValidationHelper.IsInteger(values[2]))
                {
                    string depth = values[2] + "-bit";
                    HitLogProvider.LogHit(HitLogProvider.SCREENCOLOR, siteName, cultureCode, depth, 0);
                    CMSDataContext.Current.BrowserHelper.ScreenColorDepth = depth;
                }

                // OS                
                if (!String.IsNullOrEmpty(values[3]))
                {
                    string name = String.Empty;
                    switch (values[3])
                    {
                        case "0":
                            name = "Uknown OS";
                            break;

                        case "1":
                            name = "Windows";
                            break;

                        case "2":
                            name = "Mac OS";
                            break;

                        case "3":
                            name = "UNIX";
                            break;

                        case "4":
                            name = "Linux";
                            break;

                        case "5":
                            name = "Solaris";
                            break;
                    }

                    if (name != String.Empty)
                    {
                        HitLogProvider.LogHit(HitLogProvider.OPERATINGSYSTEM, siteName, cultureCode, name, 0);
                        CMSDataContext.Current.BrowserHelper.OperatingSystem = name;
                    }
                }

                // Silverlight
                if (!String.IsNullOrEmpty(values[4]) && ValidationHelper.IsInteger(values[4]))
                {
                    bool hasSilverlight = (values[4] != "0");
                    string value = hasSilverlight ? "hs" : "ns";
                    HitLogProvider.LogHit(HitLogProvider.SILVERLIGHT, siteName, cultureCode, value, 0);
                    CMSDataContext.Current.BrowserHelper.IsSilverlightInstalled = hasSilverlight;
                }

                // Java
                if (!String.IsNullOrEmpty(values[5]) && ValidationHelper.IsBoolean(values[5]))
                {
                    bool hasJava = (values[5].ToLowerCSafe() != "false");
                    string value = hasJava ? "hj" : "nj";
                    HitLogProvider.LogHit(HitLogProvider.JAVA, siteName, cultureCode, value, 0);
                    CMSDataContext.Current.BrowserHelper.IsSilverlightInstalled = hasJava;
                }

                // Flash                
                if (!String.IsNullOrEmpty(values[6]) && ValidationHelper.IsInteger(values[6]))
                {
                    bool hasFlash = (values[6] != "0");
                    string value = hasFlash ? "hf" : "nf";
                    HitLogProvider.LogHit(HitLogProvider.FLASH, siteName, cultureCode, value, 0);
                    CMSDataContext.Current.BrowserHelper.IsSilverlightInstalled = hasFlash;
                }
            }
        }
    }
}