using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using CMS.FormControls;
using CMS.Helpers;

using TreeNode = CMS.DocumentEngine.TreeNode;

/// <summary>
/// Form control for displaying and storing pinged trackback URLs.
/// </summary>
public partial class CMSModules_Blogs_FormControls_PingedUrls : FormEngineUserControl
{
    #region "Variables"

    private string mValue = String.Empty;
    private XmlDocument xmlPinged = null;
    private XmlNodeList pingedList = null;
    private XmlNodeList readyList = null;
    private XmlNodeList waitingList = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            TreeNode node = (TreeNode)Form.EditedObject;
            mValue = ValidationHelper.GetString(node.GetValue("BlogPostPingedUrls"), "");

            return mValue;
        }
        set
        {
            // Set XML for controls
            mValue = Convert.ToString(value);
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Hide "Already pinged" label
        Label lblNotPinged = null;
        if (Form != null)
        {
            lblNotPinged = Form.FieldLabels["BlogPostPingedUrls"] as Label;
            if (lblNotPinged != null)
            {
                lblNotPinged.Visible = false;
            }
        }

        // Load data in prerender so that not-pinged urls control has time to save new values
        LoadFromXML();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Loads XML from given string.
    /// </summary>
    private void LoadXML(string value)
    {
        if (!String.IsNullOrEmpty(value))
        {
            xmlPinged = new XmlDocument();
            xmlPinged.LoadXml(value);

            if (xmlPinged.DocumentElement != null)
            {
                pingedList = xmlPinged.DocumentElement.SelectNodes("url[(@status!='ready') and (@status!='waiting')]");
                readyList = xmlPinged.DocumentElement.SelectNodes("url[@status='ready']");
                waitingList = xmlPinged.DocumentElement.SelectNodes("url[@status='waiting']");
            }
        }
    }


    /// <summary>
    /// Loads labels with data from XML.
    /// </summary>
    protected void LoadFromXML()
    {
        LoadXML(Convert.ToString(Value));

        // Check if is there anything to display
        if (((pingedList != null) && (pingedList.Count > 0)) || ((readyList != null) && (readyList.Count > 0)) || ((waitingList != null) && (waitingList.Count > 0)))
        {
            // Setup panel
            pnlPingedValues.Visible = true;
            pnlPingedValues.Controls.Add(new LiteralControl("<table class=\"blog-trackbacks\">"));

            // Go through all not pinged URLs
            if ((waitingList != null) && (waitingList.Count > 0))
            {
                foreach (XmlNode node in waitingList)
                {
                    Label lbl = null;
                    pnlPingedValues.Controls.Add(new LiteralControl("<tr>"));
                    pnlPingedValues.Controls.Add(new LiteralControl("<td>"));

                    // Add image
                    string accessibleIcon = UIHelper.GetAccessibleIconTag("icon-i-circle", GetString("blog.trackbacks.waiting"), FontIconSizeEnum.Standard);
                    pnlPingedValues.Controls.Add(new LiteralControl(accessibleIcon));

                    pnlPingedValues.Controls.Add(new LiteralControl("</td>"));
                    pnlPingedValues.Controls.Add(new LiteralControl("<td>"));

                    // Add URL label
                    lbl = new Label();
                    lbl.Text = node.Attributes["value"].Value;
                    pnlPingedValues.Controls.Add(lbl);
                    pnlPingedValues.Controls.Add(new LiteralControl("</td>"));
                    pnlPingedValues.Controls.Add(new LiteralControl("</tr>"));
                }
            }

            // Go through all URLs which are ready for ping
            if ((readyList != null) && (readyList.Count > 0))
            {
                foreach (XmlNode node in readyList)
                {
                    Label lbl = null;
                    pnlPingedValues.Controls.Add(new LiteralControl("<tr>"));
                    pnlPingedValues.Controls.Add(new LiteralControl("<td>"));

                    // Add image
                    string accessibleIcon = UIHelper.GetAccessibleIconTag("icon-clock", GetString("blog.trackbacks.ready"), FontIconSizeEnum.Standard);
                    pnlPingedValues.Controls.Add(new LiteralControl(accessibleIcon));

                    pnlPingedValues.Controls.Add(new LiteralControl("</td>"));
                    pnlPingedValues.Controls.Add(new LiteralControl("<td>"));

                    // Add URL label
                    lbl = new Label();
                    lbl.Text = node.Attributes["value"].Value;
                    pnlPingedValues.Controls.Add(lbl);
                    pnlPingedValues.Controls.Add(new LiteralControl("</td>"));
                    pnlPingedValues.Controls.Add(new LiteralControl("</tr>"));
                }
            }

            // Go through all pinged URLs
            if ((pingedList != null) && (pingedList.Count > 0))
            {
                foreach (XmlNode node in pingedList)
                {
                    bool error = ValidationHelper.GetBoolean(node.Attributes["error"].Value, false);
                    Label lbl = null;

                    pnlPingedValues.Controls.Add(new LiteralControl("<tr>"));
                    pnlPingedValues.Controls.Add(new LiteralControl("<td>"));

                    // Add error image
                    if (error)
                    {
                        string tooltip = null;
                        if ((node != null) && (node.Attributes["message"] != null))
                        {
                            tooltip = GetString("blogs.trackbacks.failed") + ": " + node.Attributes["message"].Value;
                        }
                        string accessibleIcon = UIHelper.GetAccessibleIconTag("icon-times-circle", tooltip, FontIconSizeEnum.Standard);
                        pnlPingedValues.Controls.Add(new LiteralControl(accessibleIcon));
                    }
                    // Add error image
                    else
                    {
                        
                        string tooltip = null;
                        if ((node != null) && (node.Attributes["message"] != null))
                        {
                            tooltip = GetString("blogs.trackbacks.success");
                        }
                        string accessibleIcon = UIHelper.GetAccessibleIconTag("icon-check-circle", tooltip, FontIconSizeEnum.Standard);
                        pnlPingedValues.Controls.Add(new LiteralControl(accessibleIcon));
                    }

                    pnlPingedValues.Controls.Add(new LiteralControl("</td>"));
                    pnlPingedValues.Controls.Add(new LiteralControl("<td>"));

                    // Add URL label
                    lbl = new Label();
                    lbl.Text = node.Attributes["value"].Value;
                    pnlPingedValues.Controls.Add(lbl);
                    pnlPingedValues.Controls.Add(new LiteralControl("</td>"));
                    pnlPingedValues.Controls.Add(new LiteralControl("</tr>"));
                }
            }

            pnlPingedValues.Controls.Add(new LiteralControl("</table>"));
        }
    }

    #endregion
}