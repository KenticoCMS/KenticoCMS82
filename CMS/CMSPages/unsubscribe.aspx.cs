using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Helpers;
using CMS.UIControls;

public partial class CMSPages_unsubscribe : CMSPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string datetime = QueryHelper.GetString("datetime", string.Empty);

        // Forums
        Guid subGuid = QueryHelper.GetGuid("forumsubguid", Guid.Empty);
        int forumId = QueryHelper.GetInteger("forumid", 0);
        string forumSubscriptionHash = QueryHelper.GetString("forumsubscriptionhash", string.Empty);

        if (subGuid != Guid.Empty)
        {
            Server.Transfer(ResolveUrl("~/CMSModules/Forums/CMSPages/Unsubscribe.aspx?forumsubguid=") + subGuid.ToString() + "&forumid=" + forumId);
        }
        else if (!string.IsNullOrEmpty(forumSubscriptionHash))
        {
            Server.Transfer(ResolveUrl("~/CMSModules/Forums/CMSPages/Unsubscribe.aspx?forumsubscriptionhash=") + forumSubscriptionHash + "&datetime=" + datetime);
        }

        // Newsletters
        Guid subscriberGuid = QueryHelper.GetGuid("subscriberguid", Guid.Empty);
        Guid newsletterGuid = QueryHelper.GetGuid("newsletterguid", Guid.Empty);
        Guid issueGuid = QueryHelper.GetGuid("issueguid", Guid.Empty);
        string subscriptionHash = QueryHelper.GetString("subscriptionhash", string.Empty);
        int issueID = QueryHelper.GetInteger("issueid", 0);
        int contactID = QueryHelper.GetInteger("contactid", 0);

        // Prepare contact attribute if specified
        string contactStr = (contactID > 0 ? "&contactid=" + contactID.ToString() : string.Empty);

        if ((subscriberGuid != Guid.Empty) && (newsletterGuid != Guid.Empty))
        {
            if (issueGuid != Guid.Empty)
            {
                Server.Transfer(ResolveUrl(string.Format("~/CMSModules/Newsletters/CMSPages/Unsubscribe.aspx?subscriberguid={0}&newsletterGuid={1}&issueguid={2}{3}", subscriberGuid, newsletterGuid, issueGuid, contactStr)));
            }
            else if (issueID != 0)
            {
                Server.Transfer(ResolveUrl(string.Format("~/CMSModules/Newsletters/CMSPages/Unsubscribe.aspx?subscriberguid={0}&newsletterGuid={1}&issueid={2}{3}", subscriberGuid, newsletterGuid, issueID, contactStr)));
            }
            else
            {
                Server.Transfer(ResolveUrl(string.Format("~/CMSModules/Newsletters/CMSPages/Unsubscribe.aspx?subscriberguid={0}&newsletterGuid={1}{2}", subscriberGuid, newsletterGuid, contactStr)));
            }
        }
        else if (!string.IsNullOrEmpty(subscriptionHash))
        {
            Server.Transfer(ResolveUrl(string.Format("~/CMSModules/Newsletters/CMSPages/Unsubscribe.aspx?subscriptionhash={0}&datetime={1}{2}", subscriptionHash, datetime, contactStr)));
        }
    }
}