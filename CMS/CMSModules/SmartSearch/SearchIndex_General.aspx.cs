using System;

using CMS.UIControls;
using CMS.Search;

[EditedObject(SearchIndexInfo.OBJECT_TYPE, "indexId")]
public partial class CMSModules_SmartSearch_SearchIndex_General : GlobalAdminPage
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        var searchIndex = EditedObject as SearchIndexInfo;

        ucIndexInfo.SearchIndex = searchIndex;

        ucSearchIndexEdit.AsyncIndexTaskStarted += (sender, args) => ucIndexInfo.LoadData();
    }
}
