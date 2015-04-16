using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CMS.PortalControls;
using CMS.Localization;
using CMS.Helpers;

/// <summary>
/// Chat functions static class.
/// </summary>
public static class ChatFunctions
{
    /// <summary>
    /// Make webpart's envelope, which is used for styling and hidding.
    /// </summary>
    /// <param name="cssClass">String. Envelope css classes.</param>
    /// <param name="webpart">CMSAbstractWebpart. Reference to webpart object.</param>
    /// <param name="innerContainerTitle">String. Defines title of container (if webpart has no container defined).</param>
    /// <param name="innerContainerName">String. Defines name of container (if webpart has no container defined).</param>
    public static void MakeWebpartEnvelope(string cssClass, CMSAbstractWebPart webpart, string innerContainerTitle, string innerContainerName)
    {
        if (webpart.Container != null)
        {
            webpart.Container = webpart.Container.Clone();
            webpart.Container.ContainerTextBefore = String.Format("<div id=\"envelope_{0}\" class=\"{1}\">{2}", webpart.ClientID, cssClass, webpart.Container.ContainerTextBefore);
            webpart.Container.ContainerTextAfter += "</div>";
        }
        else
        {
            CMS.PortalEngine.WebPartContainerInfo container = CMS.PortalEngine.WebPartContainerInfoProvider.GetWebPartContainerInfo(innerContainerName);
            if (container != null)
            {
                webpart.ContentBefore = container.ContainerTextBefore.Replace("{%ContainerTitle%}", ResHelper.LocalizeString(innerContainerTitle)) + webpart.ContentBefore;
                webpart.ContentAfter = webpart.ContentAfter + container.ContainerTextAfter;
            }

            webpart.ContentBefore = String.Format("<div id=\"envelope_{0}\" class=\"{1}\">{2}", webpart.ClientID, cssClass, webpart.ContentBefore);
            webpart.ContentAfter += "</div>";
        }
    }
}
