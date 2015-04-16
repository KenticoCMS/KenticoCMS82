using System;
using System.Text;
using System.Web;

using CMS.Helpers;
using CMS.WebServices;
using CMS.UIControls;
using CMS.Base;

public partial class CMSModules_REST_FormControls_GenerateHash : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle.TitleText = GetString("rest.generateauthhash");
        btnAuthenticate.Text = GetString("rest.authenticate");
        btnAuthenticate.Click += btnAuthenticate_Click;
    }


    protected void btnAuthenticate_Click(object sender, EventArgs e)
    {
        string[] urls = txtUrls.Text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        txtUrls.Text = "";

        foreach (string url in urls)
        {
            string urlWithoutHash = URLHelper.RemoveParameterFromUrl(url, "hash");
            string newUrl = HttpUtility.UrlDecode(urlWithoutHash);
            string query = URLHelper.GetQuery(newUrl).TrimStart('?');

            int index = newUrl.IndexOfCSafe("/rest");
            if (index >= 0)
            {
                // Extract the domain
                string domain = URLHelper.GetDomain(newUrl);

                // Separate the query
                newUrl = URLHelper.RemoveQuery(newUrl.Substring(index));

                // Rewrite the URL to physical URL
                string[] rewritten = BaseRESTService.RewriteRESTUrl(newUrl, query, domain, "GET");
                newUrl = rewritten[0].TrimStart('~') + "?" + rewritten[1];

                // Get the hash from real URL
                txtUrls.Text += URLHelper.AddParameterToUrl(urlWithoutHash, "hash", RESTService.GetHashForURL(newUrl, domain)) + Environment.NewLine;
            }
            else
            {
                txtUrls.Text += url + Environment.NewLine;
            }
        }
    }
}