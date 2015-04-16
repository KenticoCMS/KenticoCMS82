using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Base;
using CMS.Localization;

public partial class CMSAdminControls_UI_Development_Localize : CMSContextMenuControl
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Register the dialog script
        ScriptHelper.RegisterDialogScript(Page);

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "Localize", ScriptHelper.GetScript(@"
function EditString(key) {
    modalDialog('" + ResolveUrl("~/CMSFormControls/Selectors/LocalizableTextBox/LocalizeString.aspx") + @"?stringKey=' + escape(key), 'localizableString', 900, 635, null, null, true);
}"));
    }


    /// <summary>
    /// Compares two resource string items to each other
    /// </summary>
    /// <param name="first">First item</param>
    /// <param name="second">Second item</param>
    private static int CompareStrings(DictionaryEntry first, DictionaryEntry second)
    {
        bool firstUnknown = ((string)first.Key).EqualsCSafe((string)first.Value, true);
        bool secondUnknown = ((string)second.Key).EqualsCSafe((string)second.Value, true);

        if (firstUnknown == secondUnknown)
        {
            // Order by value
            string firstValue = ValidationHelper.GetString(first.Value, "");
            string secondValue = ValidationHelper.GetString(second.Value, "");

            return firstValue.CompareTo(secondValue);
        }

        // Order by unknown status
        return secondUnknown.CompareTo(firstUnknown);
    }

    
    protected override void Render(HtmlTextWriter writer)
    {
        if (SystemContext.DevelopmentMode)
        {
            // Render the resources
            var resources = LocalizationContext.CurrentResources;
            if (resources != null)
            {
                StringBuilder sb = new StringBuilder();

                // Sort the items by 
                List<DictionaryEntry> list = new List<DictionaryEntry>(resources.Count);
                foreach (DictionaryEntry item in resources)
                {
                    list.Add(item);
                }
                list.Sort(CompareStrings);

                bool lastUnknown = false;

                sb.Append("<div class=\"MenuHeader\">", ResHelper.GetString("localizable.localize"), "</div>");

                var warningIcon = new CMSIcon{ CssClass = "icon-exclamation-triangle color-orange-80 warning-icon" }.GetRenderedHTML();

                foreach (DictionaryEntry item in list)
                {
                    string key = (string)item.Key;
                    string value = ValidationHelper.GetString(item.Value, "");

                    // Handle the unknown status
                    bool unknown = key.EqualsCSafe(value, false);
                    if (!unknown && lastUnknown)
                    {
                        sb.Append("<div class=\"Separator\">&nbsp;</div>");
                    }
                    lastUnknown = unknown;

                    value = HTMLHelper.HTMLEncode(TextHelper.LimitLength(HTMLHelper.StripTags(value), 70));

                    sb.Append("<div class=\"Item\" onclick=\"EditString('", HTMLHelper.HTMLEncode(ScriptHelper.GetString(key, false)) + "'); return false;\"><div class=\"ItemPadding\">", (unknown ? warningIcon : ""), "<span class=\"Name\"><strong>", value, "</strong> (", TextHelper.LimitLength(HTMLHelper.HTMLEncode(key), 50), ")</span></div></div>");
                }

                ltlStrings.Text = sb.ToString();
            }
        }
        
        base.Render(writer);
    }
}
