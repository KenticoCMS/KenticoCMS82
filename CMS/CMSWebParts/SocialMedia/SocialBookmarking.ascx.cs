using System;
using System.Collections;
using System.Text;

using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.PortalControls;
using CMS.Base;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSWebParts_SocialMedia_SocialBookmarking : CMSAbstractWebPart
{
    #region "Private variables"

    /// <summary>
    /// Hashtable which contains info about bookmark services.
    /// </summary>
    private static readonly Hashtable bookmarkServices = new Hashtable();

    #endregion


    #region "Services properties"

    /// <summary>
    /// Google bookmarks service.
    /// </summary>
    public bool GoogleBookmarks
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("GoogleBookmarks"), false);
        }
        set
        {
            SetValue("GoogleBookmarks", value);
        }
    }


    /// <summary>
    /// Delicious service.
    /// </summary>
    public bool Delicious
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Delicious"), false);
        }
        set
        {
            SetValue("Delicious", value);
        }
    }


    /// <summary>
    /// Digg service.
    /// </summary>
    public bool Digg
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Digg"), false);
        }
        set
        {
            SetValue("Digg", value);
        }
    }


    /// <summary>
    /// MySpace service.
    /// </summary>
    public bool MySpace
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("MySpace"), false);
        }
        set
        {
            SetValue("MySpace", value);
        }
    }


    /// <summary>
    /// Facebook service.
    /// </summary>
    public bool FaceBook
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("FaceBook"), false);
        }
        set
        {
            SetValue("FaceBook", value);
        }
    }


    /// <summary>
    /// YahooMyWeb service.
    /// </summary>
    public bool YahooMyWeb
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("YahooMyWeb"), false);
        }
        set
        {
            SetValue("YahooMyWeb", value);
        }
    }


    /// <summary>
    /// Stumble upon service.
    /// </summary>
    public bool StumbleUpon
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("StumbleUpon"), false);
        }
        set
        {
            SetValue("StumbleUpon", value);
        }
    }


    /// <summary>
    /// Reddit service.
    /// </summary>
    public bool Reddit
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Reddit"), false);
        }
        set
        {
            SetValue("Reddit", value);
        }
    }


    /// <summary>
    /// Newsvine service.
    /// </summary>
    public bool Newsvine
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Newsvine"), false);
        }
        set
        {
            SetValue("Newsvine", value);
        }
    }


    /// <summary>
    /// Technorati service.
    /// </summary>
    public bool Technorati
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Technorati"), false);
        }
        set
        {
            SetValue("Technorati", value);
        }
    }


    /// <summary>
    /// Twitter service.
    /// </summary>
    public bool Twitter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Twitter"), false);
        }
        set
        {
            SetValue("Twitter", value);
        }
    }


    /// <summary>
    /// YahooBookmarks.
    /// </summary>
    public bool YahooBookmarks
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("YahooBookmarks"), false);
        }
        set
        {
            SetValue("YahooBookmarks", value);
        }
    }


    /// <summary>
    /// MyAOL service.
    /// </summary>
    public bool MyAOL
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("MyAOL"), false);
        }
        set
        {
            SetValue("MyAOL", value);
        }
    }


    /// <summary>
    /// Blogmarks service.
    /// </summary>
    public bool Blogmarks
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Blogmarks"), false);
        }
        set
        {
            SetValue("Blogmarks", value);
        }
    }


    /// <summary>
    /// Diigo service.
    /// </summary>
    public bool Diigo
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Diigo"), false);
        }
        set
        {
            SetValue("Diigo", value);
        }
    }


    /// <summary>
    /// Link-a-gogo service.
    /// </summary>
    public bool LinkAGogo
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("LinkAGogo"), false);
        }
        set
        {
            SetValue("LinkAGogo", value);
        }
    }


    /// <summary>
    /// Magnolia service.
    /// </summary>
    public bool Magnolia
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Magnolia"), false);
        }
        set
        {
            SetValue("Magnolia", value);
        }
    }


    /// <summary>
    /// Seganlo service.
    /// </summary>
    public bool Segnalo
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Segnalo"), false);
        }
        set
        {
            SetValue("Segnalo", value);
        }
    }


    /// <summary>
    /// LinkedIn service.
    /// </summary>
    public bool LinkedIn
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("LinkedIn"), false);
        }
        set
        {
            SetValue("LinkedIn", value);
        }
    }

    #endregion


    #region "Other properties"

    /// <summary>
    /// Title (text before bookmarks).
    /// </summary>
    public string Title
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Title"), "");
        }
        set
        {
            SetValue("Title", value);
        }
    }


    /// <summary>
    /// Separator.
    /// </summary>
    public string Separator
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Separator"), "");
        }
        set
        {
            SetValue("Separator", value);
        }
    }


    /// <summary>
    /// Title CSS class.
    /// </summary>
    public string TitleClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TitleClass"), "");
        }
        set
        {
            SetValue("TitleClass", value);
        }
    }


    /// <summary>
    /// If true, links are opened in new window.
    /// </summary>
    public bool ShowInNewWindow
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowInNewWindow"), false);
        }
        set
        {
            SetValue("ShowInNewWindow", value);
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Page load.
    /// </summary>    
    protected void Page_Load(object sender, EventArgs e)
    {
    }


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Fill hashtables.
    /// </summary>
    private static void FillHashTable()
    {
        if (bookmarkServices.Count == 0)
        {
            lock (bookmarkServices)
            {
                // Each record has display name, url, url to book and title
                bookmarkServices["GoogleBookmarks"] = new string[] { "Google Bookmarks", "http://www.google.com/bookmarks/mark?op=add&amp;bkmk=", "&amp;title=" };
                bookmarkServices["Delicious"] = new string[] { "Delicious", "http://delicious.com/save?jump=yes&amp;noui&amp;v=4&amp;url=", "&amp;title=" };
                bookmarkServices["Digg"] = new string[] { "Digg", "http://digg.com/submit/?url=", "&amp;title=" };
                bookmarkServices["MySpace"] = new string[] { "MySpace", "http://www.myspace.com/Modules/PostTo/Pages/?u=", "&amp;t=" };
                bookmarkServices["FaceBook"] = new string[] { "Facebook", "http://www.facebook.com/sharer.php?u=", "&amp;t=" };
                bookmarkServices["YahooMyWeb"] = new string[] { "Yahoo MyWeb", "http://myweb2.search.yahoo.com/myresults/bookmarklet?u=", "&amp;t=" };
                bookmarkServices["StumbleUpon"] = new string[] { "StumbleUpon", "http://www.stumbleupon.com/submit?url=", "&amp;title=" };
                bookmarkServices["Reddit"] = new string[] { "Reddit", "http://www.reddit.com/submit?url=", "&amp;title=" };
                bookmarkServices["Newsvine"] = new string[] { "Newsvine", "http://www.newsvine.com/_tools/seed&amp;save?u=", "&amp;h=" };
                bookmarkServices["Technorati"] = new string[] { "Terchnorati", "http://technorati.com/faves/?add=", "&amp;title=" };
                bookmarkServices["Twitter"] = new string[] { "Twitter", "http://twitter.com/share?url=", "&text=" };
                bookmarkServices["YahooBookmarks"] = new string[] { "Yahoo Bookmarks", "http://bookmarks.yahoo.com/toolbar/savebm?u=", "&amp;t=" };
                bookmarkServices["MyAOL"] = new string[] { "MyAOL", "http://favorites.my.aol.com/ffclient/webroot/0.4.5/src/html/addBookmarkDialog.html?url=", "&amp;title=" };
                bookmarkServices["Blogmarks"] = new string[] { "Blogmarks", "http://blogmarks.net/my/new.php?url=", "&amp;title=" };
                bookmarkServices["Diigo"] = new string[] { "Diigo", "http://www.diigo.com/post?url=", "&amp;title=" };
                bookmarkServices["LinkAGogo"] = new string[] { "Link-a-Gogo", "http://www.linkagogo.com/go/AddNoPopup?url=", "&amp;title=" };
                bookmarkServices["Segnalo"] = new string[] { "Segnalo", "http://segnalo.alice.it/post.html.php?url=", "&amp;title=" };
                bookmarkServices["LinkedIn"] = new string[] { "LinkedIn", "http://www.linkedin.com/shareArticle?mini=true&url=", "&amp;title=" };
            }
        }
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    public void SetupControl()
    {
        ltlIcons.Text = "";

        // Fill hashtable with info about all bookmark services
        FillHashTable();

        // Resolve path of images
        string imagesPath = ResolveUrl("~/CMSWebParts/SocialMedia/SocialBookmarking_files");

        // Get current document
        TreeNode node = DocumentContext.CurrentDocument;
        if (node != null)
        {
            // Get url of current document
            string liveUrl = RequestContext.CurrentURL;
            liveUrl = URLHelper.GetAbsoluteUrl(liveUrl);

            // Encode url
            liveUrl = Server.UrlEncode(liveUrl);

            // Prepare target
            string target = ShowInNewWindow ? "target=\"_blank\"" : "";

            string[] bookmarkInfo = new string[4];

            // Get all keys from hashtable
            object[] keys = new object[bookmarkServices.Count];
            bookmarkServices.Keys.CopyTo(keys, 0);
            StringBuilder sb = new StringBuilder();

            // Loop thru all items in hashtable
            for (int i = 0; i != keys.Length; i++)
            {
                // Get structure
                bookmarkInfo = (string[])bookmarkServices[keys[i]];
                string currentService = keys[i].ToString();
                if (bookmarkInfo.Length != 0)
                {
                    // If current service is enabled generate html code
                    if (ValidationHelper.GetBoolean(GetValue(currentService), false))
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append(Separator);
                        }
                        sb.Append("<a href=\"", bookmarkInfo[1], liveUrl, bookmarkInfo[2], Server.UrlEncode(node.GetDocumentName()), "\" title=\"", GetString("addtobook.addto"), " ", bookmarkInfo[0], "\" ", target, "><img src=\"", imagesPath, "/", currentService.ToLowerCSafe(), ".gif", "\" alt=\"", GetString("addtobook.addto"), " ", bookmarkInfo[0], "\" style=\"border-style:none;\" /></a>");
                    }
                }
            }


            // If at least one bookmarking service was checked show title
            if (sb.Length > 0)
            {
                if (!string.IsNullOrEmpty(TitleClass))
                {
                    sb.Insert(0, "<span class=\"" + TitleClass + "\">" + Title + "</span>");
                }
                else
                {
                    sb.Insert(0, Title);
                }

                // Wrap with span with class
                sb.Insert(0, "<span class=\"SocialBookmarking\">");
                sb.Append("</span>");
            }

            ltlIcons.Text = sb.ToString();
        }
    }

    #endregion
}