using System;
using System.Linq;

using CMS.FormEngine;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.Base;
using CMS.UIControls;

using System.Xml;
using CMS.DataEngine;

public partial class CMSModules_AdminControls_Controls_UIControls_WebPartSystemProperties : CMSUserControl
{
    #region "Variables"

    String defaultValueColumName = String.Empty;
    BaseInfo eObject = null;
    String defaultSet = String.Empty;
    String trimmedSet = String.Empty;
    String webPartProperties = "<form></form>";

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        eObject = UIContext.EditedObject as BaseInfo;

        // If saved is found in query string
        if (!RequestHelper.IsPostBack() && (QueryHelper.GetInteger("saved", 0) == 1))
        {
            ShowChangesSaved();
        }

        string before = String.Empty;
        string after = String.Empty;

        String objectType = UIContextHelper.GetObjectType(UIContext);

        switch (objectType.ToLowerCSafe())
        {
            case "cms.webpart":
                defaultValueColumName = "WebPartDefaultValues";

                before = PortalFormHelper.GetWebPartProperties(WebPartTypeEnum.Standard, PropertiesPosition.Before);
                after = PortalFormHelper.GetWebPartProperties(WebPartTypeEnum.Standard, PropertiesPosition.After);

                defaultSet = FormHelper.CombineFormDefinitions(before, after);

                WebPartInfo wi = eObject as WebPartInfo;

                // If inherited web part load parent properties
                if (wi.WebPartParentID > 0)
                {
                    WebPartInfo parentInfo = WebPartInfoProvider.GetWebPartInfo(wi.WebPartParentID);
                    if (parentInfo != null)
                    {
                        webPartProperties = FormHelper.MergeFormDefinitions(parentInfo.WebPartProperties, wi.WebPartProperties);
                    }
                }
                else
                {
                    webPartProperties = wi.WebPartProperties;
                }

                break;

            case "cms.widget":
                before = PortalFormHelper.LoadProperties("Widget", "Before.xml");
                after = PortalFormHelper.LoadProperties("Widget", "After.xml");

                defaultSet = FormHelper.CombineFormDefinitions(before, after);

                defaultValueColumName = "WidgetDefaultValues";
                WidgetInfo wii = eObject as WidgetInfo;
                if (wii != null)
                {
                    WebPartInfo wiiWp = WebPartInfoProvider.GetWebPartInfo(wii.WidgetWebPartID);
                    if (wiiWp != null)
                    {
                        webPartProperties = FormHelper.MergeFormDefinitions(wiiWp.WebPartProperties, wii.WidgetProperties);
                    }
                }

                break;
        }

        // Get the web part info
        if (eObject != null)
        {
            String defVal = ValidationHelper.GetString(eObject.GetValue(defaultValueColumName), string.Empty);
            defaultSet = LoadDefaultValuesXML(defaultSet);

            fieldEditor.Mode = FieldEditorModeEnum.SystemWebPartProperties;
            fieldEditor.FormDefinition = FormHelper.MergeFormDefinitions(defaultSet, defVal);
            fieldEditor.OnAfterDefinitionUpdate += fieldEditor_OnAfterDefinitionUpdate;
            fieldEditor.OriginalFormDefinition = defaultSet;
            fieldEditor.WebPartId = eObject.Generalized.ObjectID;
        }

        ScriptHelper.HideVerticalTabs(Page);
    }


    /// <summary>
    /// Load XML with default values (remove keys already overridden in properties tab).
    /// </summary>
    /// <param name="wi">Web part info</param>
    /// <param name="formDef">String XML definition of default values of webpart</param>
    private String LoadDefaultValuesXML(string formDef)
    {
        XmlDocument xmlDefault = new XmlDocument();

        // Test if there is any default properties set
        XmlDocument xmlProperties = new XmlDocument();
        xmlProperties.LoadXml(webPartProperties);

        // Load default system xml 
        xmlDefault.LoadXml(formDef);

        // Filter overridden properties - remove properties with same name as in system XML
        XmlNodeList defaultList = xmlDefault.SelectNodes(@"//field");
        foreach (XmlNode node in defaultList)
        {
            string columnName = node.Attributes["column"].Value.ToString();

            XmlNodeList propertiesList = xmlProperties.SelectNodes("//field[@column=\"" + columnName + "\"]");
            //This property already set in properties tab
            if (propertiesList.Count > 0)
            {
                node.ParentNode.RemoveChild(node);
            }
        }

        // Filter empty categories            
        XmlNodeList nodes = xmlDefault.DocumentElement.ChildNodes;
        for (int i = 0; i < nodes.Count; i++)
        {
            XmlNode node = nodes[i];
            if (node.Name.ToLowerCSafe() == "category")
            {
                // Find next category
                if (i < nodes.Count - 1)
                {
                    XmlNode nextNode = nodes[i + 1];
                    if (nextNode.Name.ToLowerCSafe() == "category")
                    {
                        // Delete actual category
                        node.ParentNode.RemoveChild(node);
                        i--;
                    }
                }
            }
        }

        // Remove WebPartControlID 
        XmlNode IDNode = xmlDefault.SelectSingleNode("//field[@column=\"WebPartControlID\"]");
        if (IDNode != null)
        {
            IDNode.ParentNode.RemoveChild(IDNode);
        }

        // Test if last category is not empty           
        nodes = xmlDefault.DocumentElement.ChildNodes;
        if (nodes.Count > 0)
        {
            XmlNode lastNode = nodes[nodes.Count - 1];
            if (lastNode.Name.ToLowerCSafe() == "category")
            {
                lastNode.ParentNode.RemoveChild(lastNode);
            }
        }
        return xmlDefault.OuterXml;
    }


    protected void fieldEditor_OnAfterDefinitionUpdate(object sender, EventArgs e)
    {
        String defVal = FormHelper.GetFormDefinitionDifference(defaultSet, fieldEditor.FormDefinition, true);
        eObject.SetValue(defaultValueColumName, defVal);
        eObject.Update();
    }

    #endregion
}
