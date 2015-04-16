using System;

using CMS.UIControls;

[Action(0, "conversion.conversion.new", "Edit.aspx")]
[Security(Resource = "CMS.WebAnalytics", UIElements = "Conversions;Conversions.List")]
public partial class CMSModules_WebAnalytics_Pages_Tools_Conversion_List : CMSConversionPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    #endregion
}