using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Helpers;
using CMS.UIControls;
using CMS.Base;

public partial class CMSPages_GetCMSVersion : CMSPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Show version only if right key is inserted
        string versionKey = QueryHelper.GetString("versionkey", string.Empty);
        if (EncryptionHelper.VerifyVersionRSA(versionKey))
        {
            Version v = CMSVersion.Version;
            if (v != null)
            {
                // Write the version to the response
                Response.Clear();
                Response.Write(v.ToString(3));

                RequestHelper.EndResponse();
            }
        }
    }
}