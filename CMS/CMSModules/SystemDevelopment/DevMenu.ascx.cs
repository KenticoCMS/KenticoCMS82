using System;
using System.Web.UI;
using System.Data;
using System.Collections.Specialized;

using CMS.Core;
using CMS.ExtendedControls;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.Base;
using CMS.ExtendedControls.ActionsConfig;
using CMS.EventLog;
using CMS.SiteProvider;
using CMS.Helpers;

public partial class CMSModules_SystemDevelopment_DevMenu : CMSUserControl, ICallbackEventHandler
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (SystemContext.DevelopmentMode)
        {
            // Restart application
            menu.AddAction(new HeaderAction
            {
                Text = GetString("administration-system.btnrestart"),
                ButtonStyle = ButtonStyle.Default,
                Tooltip = GetString("administration-system.btnrestart"),
                OnClientClick = "function RestartPerformed() {return alert('" + GetString("administration-system.restartsuccess") + "');} if (confirm('" + GetString("system.restartconfirmation") + "')) {" + Page.ClientScript.GetCallbackEventReference(this, "'restart'", "RestartPerformed", String.Empty, true) + "}"
            });

            // Clear cache
            menu.AddAction(new HeaderAction
            {
                Text = GetString("administration-system.btnclearcache"),
                ButtonStyle = ButtonStyle.Default,
                Tooltip = GetString("administration-system.btnclearcache"),
                OnClientClick = "function ClearCachePerformed() {return alert('" + GetString("administration-system.clearcachesuccess") + "');} if (confirm('" + GetString("system.clearcacheconfirmation") + "')) {" + Page.ClientScript.GetCallbackEventReference(this, "'clearcache'", "ClearCachePerformed", String.Empty, true) + "}"
            });

            // Event log
            HeaderAction eventLog = new HeaderAction
            {
                Text = GetString("administration.ui.eventlog"),
                ButtonStyle = ButtonStyle.Default,
                Tooltip = GetString("administration.ui.eventlog"),
                RedirectUrl = "~/CMSModules/EventLog/EventLog.aspx",
                Target = "_blank"
            };

            // Event log items
            DataSet ds = EventLogProvider.GetAllEvents(null, "EventTime DESC", 10, "EventTime, EventType, Source, EventCode");
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                HeaderAction ev = new HeaderAction
                {
                    Text = string.Format("{0} {1} {2} {3}", row["EventTime"], row["EventType"], row["Source"], row["EventCode"]),
                    ButtonStyle = ButtonStyle.Default
                };
                eventLog.AlternativeActions.Add(ev);
            }

            menu.AddAction(eventLog);

            // Debug
            menu.AddAction(new HeaderAction
            {
                Text = GetString("Administration-System.Debug"),
                ButtonStyle = ButtonStyle.Default,
                Tooltip = GetString("Administration-System.Debug"),
                RedirectUrl = URLHelper.AppendQuery(UIContextHelper.GetElementUrl(ModuleName.CMS, "Debug"), "displaytitle=true"),
                Target = "_blank"
            });

            // Submit defect
            menu.AddAction(new HeaderAction
            {
                Text = "Submit defect",
                ButtonStyle = ButtonStyle.Default,
                Tooltip = "Submit defect",
                RedirectUrl = "https://kentico.atlassian.net/secure/CreateIssue!default.jspa",
                Target = "_blank"
            });

            // Virtual site
            HeaderAction sites = new HeaderAction
            {
                Text = GetString("devmenu.sites"),
                ButtonStyle = ButtonStyle.Default,
                Tooltip = GetString("devmenu.sites"),
                Inactive = true
            };

            // Site items
            var sitesDs = SiteInfoProvider.GetSites().Columns("SiteName", "SiteDisplayName").OrderBy("SiteDisplayName");

            foreach (SiteInfo s in sitesDs)
            {
                // Prepare the parameters
                NameValueCollection values = new NameValueCollection();
                values.Add(VirtualContext.PARAM_SITENAME, s.SiteName);

                HeaderAction site = new HeaderAction
                {
                    Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(s.DisplayName)),
                    ButtonStyle = ButtonStyle.Default,
                    RedirectUrl = VirtualContext.GetVirtualContextPath(Request.Path, values),
                    Target = "_blank"
                };

                sites.AlternativeActions.Add(site);
            }

            menu.AddAction(sites);
        }
        else
        {
            Visible = false;
        }
    }


    /// <summary>
    /// Returns callback result.
    /// </summary>
    public string GetCallbackResult()
    {
        return String.Empty;
    }


    /// <summary>
    /// Raises callback event.
    /// </summary>
    /// <param name="eventArgument">Event argument</param>
    public void RaiseCallbackEvent(string eventArgument)
    {
        switch (eventArgument)
        {
            case "restart":
                SystemHelper.RestartApplication(Request.PhysicalApplicationPath);
                break;

            case "clearcache":
                CacheHelper.ClearCache();
                break;
        }
    }
}
