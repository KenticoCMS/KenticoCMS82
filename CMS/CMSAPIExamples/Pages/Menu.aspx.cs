using System;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;

using CMS.Core;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;

[UIElement(ModuleName.CMS, "APIExamples")]
public partial class CMSAPIExamples_Pages_Menu : CMSAPIExamplePage
{
    private string selectOnStartupScript = "";
    private string reqId = "";


    protected void Page_Load(object sender, EventArgs e)
    {
        reqId = QueryHelper.GetString("module", "").ToLowerCSafe();

        SetupControls();
        LoadMenu();

        if (!string.IsNullOrEmpty(selectOnStartupScript))
        {
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "StartupSelect", ScriptHelper.GetScript(selectOnStartupScript));
        }

    }


    protected void SetupControls()
    {
        ScriptHelper.RegisterJQuery(Page);

        // Initialize TreeView
        treeView.ImageSet = TreeViewImageSet.Custom;
        treeView.ExpandImageToolTip = "Expand";
        treeView.CollapseImageToolTip = "Collapse";
        if (CultureHelper.IsUICultureRTL())
        {
            treeView.LineImagesFolder = GetImageUrl("RTL/Design/Controls/Tree", false, false);
        }
        else
        {
            treeView.LineImagesFolder = GetImageUrl("Design/Controls/Tree", false, false);
        }

        StringBuilder script = new StringBuilder();
        script.AppendLine("function ShowBlank(elem) {");
        script.Append(" var url = '");
        script.Append(ResolveUrl("~/CMSPages/blank.htm"));
        script.AppendLine("';");
        script.AppendLine(" DisplayPage(url, elem);");
        script.AppendLine("}");
        script.AppendLine("function ShowExample(url,elem) {");
        script.Append(" var mainUrl = '");
        script.Append(ResolveUrl("~/CMSAPIExamples/Code/"));
        script.AppendLine("';");
        script.AppendLine(" DisplayPage(mainUrl + url, elem);");
        script.AppendLine("}");

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "MenuScript", ScriptHelper.GetScript(script.ToString()));
    }


    protected void CreateRoot()
    {
        // Create root
        TreeNode rootNode = new TreeNode();
        rootNode.Text = "<span class=\"ContentTreeSelectedItem\" name=\"treeNode\" id=\"rootNode\" onclick=\"ShowBlank(this);\"><span class=\"Name\">Modules</span></span>";
        rootNode.Expanded = true;
        rootNode.SelectAction = TreeNodeSelectAction.None;
        treeView.Nodes.Add(rootNode);
    }


    protected void LoadMenu()
    {
        if (!RequestHelper.IsPostBack())
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(Server.MapPath("menu.xml"));

                treeView.Nodes.Clear();
                CreateRoot();

                addTreeNode(xmlDoc.DocumentElement, treeView.Nodes[0]);
            }
            catch (Exception ex)
            {
                lblError.Visible = true;
                lblError.Text = ex.Message;
                lblError.ToolTip = ex.StackTrace;
            }
        }
    }


    private void addTreeNode(XmlNode xmlNode, TreeNode treeNode)
    {
        if (xmlNode.HasChildNodes)
        {
            XmlNodeList nodeList = xmlNode.ChildNodes;
            //Loop through the child nodes
            foreach (XmlNode node in nodeList)
            {
                TreeNode newTreeNode = CreateNode(node);
                if (newTreeNode != null)
                {
                    treeNode.ChildNodes.Add(newTreeNode);
                    addTreeNode(node, newTreeNode);
                }
            }
        }
    }


    private TreeNode CreateNode(XmlNode xmlNode)
    {
        if (xmlNode.Attributes == null)
        {
            return null;
        }

        string module = xmlNode.Attributes["module"].InnerText;

        // Get node attributes
        string text = xmlNode.Attributes["text"].InnerText;
        string url = (xmlNode.Attributes["url"] != null) ? xmlNode.Attributes["url"].InnerText : null;
        string id = (xmlNode.Attributes["id"] != null) ? xmlNode.Attributes["id"].InnerText : null;

        if (string.IsNullOrEmpty(id))
        {
            id = module;
        }

        string onClickScript = "";

        // Create new node
        TreeNode node = new TreeNode();
        node.Collapse();


        if (!string.IsNullOrEmpty(url))
        {
            string urlStr = ScriptHelper.GetString(url);

            if (reqId.ToLowerCSafe() == id.ToLowerCSafe())
            {
                selectOnStartupScript = string.Format("var node = document.getElementById('node_{0}'); if(node != null){{ShowExample({1}, node);}}", id, urlStr);
            }

            node.SelectAction = TreeNodeSelectAction.None;

            // Prepare selection script
            onClickScript = string.Format("ShowExample({0}, this);", urlStr);
        }
        else
        {
            // Expand category node when clicked
            node.SelectAction = TreeNodeSelectAction.Expand;
        }

        // Create node text
        node.Text = String.Format("<span class=\"ContentTreeItem\" name=\"treeNode\" id=\"node_{2}\" onclick=\"{1}\"><span class=\"Name\">{0}</span></span>", text, onClickScript, id);

        return node;
    }
}