using System;

using CMS.Core;
using CMS.OnlineMarketing;
using CMS.UIControls;

[EditedObject(ScoreInfo.OBJECT_TYPE, "scoreid")]
[UIElement(ModuleName.SCORING, "Scoring.Contacts")]
public partial class CMSModules_Scoring_Pages_Tab_Contacts : CMSScorePage
{
    protected override void OnPreInit(EventArgs e)
    {
        RequiresDialog = false;
        base.OnPreInit(e);
    }
}