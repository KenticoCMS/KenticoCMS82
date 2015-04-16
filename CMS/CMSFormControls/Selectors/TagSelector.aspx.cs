using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;
using CMS.Taxonomy;

public partial class CMSFormControls_Selectors_TagSelector : CMSModalPage
{
    #region "Variables"

    private int groupId;
    private string textBoxId;
    private string oldTags;
    private Hashtable selectedTags;

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        SetSaveResourceString("general.select");

        // Register jQuery
        ScriptHelper.RegisterJQuery(Page);

        // Get group Id
        groupId = QueryHelper.GetInteger("group", 0);

        // Get id of the base selector textbox
        textBoxId = QueryHelper.GetString("textbox", "");

        // Get selected tags
        oldTags = QueryHelper.GetString("tags", "");
        selectedTags = TagHelper.GetTags(oldTags);

        // Setup UniGrid
        gridElem.ZeroRowsText = GetString("tags.tagselector.noold");
        gridElem.GridView.ShowHeader = false;
        gridElem.OnBeforeDataReload += gridElem_OnBeforeDataReload;
        gridElem.OnAfterDataReload += gridElem_OnAfterDataReload;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;

        // Page title
        PageTitle.TitleText = GetString("tags.tagselector.title");

        Save += btnOk_Click;
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        string retval = "";

        // Append selected tags which are already in DB
        var items = gridElem.SelectedItems;
        if (items.Count > 0)
        {
            items.Sort();

            foreach (string tagName in items)
            {
                if (tagName.Contains(" "))
                {
                    retval = (retval + ", \"" + tagName.Trim('"') + "\"");
                }
                else
                {
                    retval = (retval + ", " + tagName);
                }
            }
        }

        // Remove
        if (retval != "")
        {
            retval = retval.Substring(2);
        }

        ltlScript.Text = ScriptHelper.GetScript("wopener.setTagsToTextBox(" + ScriptHelper.GetString(textBoxId) + ", " + ScriptHelper.GetString(retval) + "); CloseDialog();");
    }

    #endregion


    #region "UniGrid Events"

    protected void gridElem_OnBeforeDataReload()
    {
        // Filter records by tag group ID
        string where = "(TagGroupID = " + groupId + ")";
        if (!String.IsNullOrEmpty(gridElem.CompleteWhereCondition))
        {
            where += " AND (" + gridElem.CompleteWhereCondition + ")";
        }

        gridElem.WhereCondition = where;
    }


    protected void gridElem_OnAfterDataReload()
    {
        if (!DataHelper.DataSourceIsEmpty(gridElem.GridView.DataSource))
        {
            // Fill list with tags and trim quotes and spaces
            if (!URLHelper.IsPostback())
            {
                var selection = (from string tag in selectedTags.Values select tag.Trim('"').Trim()).ToList();

                gridElem.SelectedItems = selection;
            }
        }
    }


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        if (sourceName.ToLowerCSafe() == "tagname")
        {
            DataRowView drv = (DataRowView)parameter;
            string tagName = ValidationHelper.GetString(drv["TagName"], "");
            string tagId = ValidationHelper.GetString(drv["TagID"], "");
            if ((tagName != "") && (tagName != tagId))
            {
                string tagCount = ValidationHelper.GetString(drv["TagCount"], "");
                string tagText = HTMLHelper.HTMLEncode(tagName) + " (" + tagCount + ")";

                // Create link with onclick event which call onclick event of checkbox in the same row
                return "<a href=\"#\" onclick=\"var c=$cmsj(this).parents('tr:first').find('input:checkbox'); c.attr('checked', !c.attr('checked')).get(0).onclick(); return false;\">" + tagText + "</a>";
            }
        }
        return "";
    }

    #endregion
}