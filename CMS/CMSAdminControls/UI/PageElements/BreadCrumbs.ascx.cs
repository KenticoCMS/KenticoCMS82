using System;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.UIControls;

public partial class CMSAdminControls_UI_PageElements_BreadCrumbs : Breadcrumbs
{
    #region "Variables"

    private bool mPropagateToMainNavigation = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Help control.
    /// </summary>
    public HelpControl Help
    {
        get
        {
            return helpBreadcrumbs;
        }
    }


    /// <summary>
    /// Indicates whether breadcrumbs items are propagated to main breadcrumbs navigation in administration.
    /// </summary>
    public bool PropagateToMainNavigation
    {
        get
        {
            return mPropagateToMainNavigation;
        }
        set
        {
            mPropagateToMainNavigation = value;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// OnPreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Create the breadcrumbs
        if (ItemsInternal.Count > 0)
        {
            if (!HideBreadcrumbs)
            {
                pnlBreadCrumbs.Visible = true;

                // Generate the breadcrumbs controls
                int breadCrumbsLength = ItemsInternal.Count;
                for (int i = 0; i < breadCrumbsLength; i++)
                {
                    CreateBreadCrumbsItem(ItemsInternal[i], (i + 1 == breadCrumbsLength));
                }
            }
            
            var last = ItemsInternal.Last();
            if (String.IsNullOrEmpty(last.RedirectUrl))
            {
                last.RedirectUrl = URLHelper.UrlEncodeQueryString(RequestContext.CurrentURL);
            }

            if (PropagateToMainNavigation)
            {
                // Register breadcrumbs data for client code and start BreadcrumbsDataSource JS module
                RequestContext.ClientApplication.Add("breadcrumbs", new
                {
                    Reframe = ChangeTargetFrame,
                    Data = ItemsInternal
                });
            }
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Creates the internal breadcrumb item representation
    /// </summary>
    /// <param name="breadcrumb">Breadcrumb item from which internal breadcrumb will be created</param>
    /// <param name="isLast">Indicates whether the breadcrumb is the last one</param>
    private void CreateBreadCrumbsItem(BreadcrumbItem breadcrumb, bool isLast)
    {
        // Make link if URL specified
        string text = ResHelper.LocalizeString(breadcrumb.Text);

        var li = new HtmlGenericControl("li");
        if (!(string.IsNullOrEmpty(breadcrumb.RedirectUrl) && string.IsNullOrEmpty(breadcrumb.OnClientClick)) && !isLast)
        {
            HyperLink newLink = new HyperLink();
            newLink.Text = EncodeBreadcrumbs ? HTMLHelper.HTMLEncode(text) : text;
            newLink.NavigateUrl = breadcrumb.RedirectUrl;
            newLink.Target = breadcrumb.Target;
            // JavaScript is specified add on click
            if (!string.IsNullOrEmpty(breadcrumb.OnClientClick))
            {
                newLink.Attributes.Add("onclick", breadcrumb.OnClientClick);
                newLink.Attributes.Add("href", "javascript:void(0)");                
            }
            li.Controls.Add(newLink);
        }
        else // Make label if last item or URL not specified
        {
            li.InnerText = text;
        }

        plcBreadcrumbs.Controls.Add(li);
    }

    #endregion
}