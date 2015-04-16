using System;

using CMS.UIControls;

// Actions
[Action(0, "translationservice.translationservice.new", "Edit.aspx")]

// Title
[Title("translationservice.translationservice.list")]
[UIElementAttribute("CMS.TranslationServices", "Development.TranslationServices")]
public partial class CMSModules_Translations_Pages_Tools_TranslationService_List : CMSTranslationsPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #endregion
}
