using System;

using CMS.Helpers;
using CMS.UIControls;
using CMS.Search;

public partial class CMSModules_SmartSearch_Controls_IndexInfo : CMSUserControl
{
    /// <summary>
    /// Gets or sets the search index.
    /// </summary>
    public SearchIndexInfo SearchIndex
    {
        get;
        set;
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        LoadData();

        ScriptHelper.RegisterDialogScript(Page);
    }


    /// <summary>
    /// Loads the index information.
    /// </summary>
    public void LoadData()
    {
        if (SearchIndex == null)
        {
            return;
        }

        var isInAction = (SearchIndex.IndexFilesStatus == IndexStatusEnum.REBUILDING || SearchIndex.IndexFilesStatus == IndexStatusEnum.OPTIMIZING);
        var isNotReady = (!isInAction && SearchIndex.IndexFilesStatus != IndexStatusEnum.READY);

        // Items count
        lblItemCount.Text = ValidationHelper.GetString(SearchIndex.NumberOfIndexedItems, "0");

        // File size
        lblFileSize.Text = DataHelper.GetSizeString(SearchIndex.IndexFileSize);

        // Status
        string statusName =  GetString("srch.status." + SearchIndex.IndexFilesStatus);

        // Show preloader image and link to thread log in status when in action 
        if (isInAction)
        {
            var statusText = "";
            if (SearchTaskInfoProvider.IndexerThreadGuid != Guid.Empty)
            {
                string url = URLHelper.ResolveUrl("~/CMSModules/System/Debug/System_ViewLog.aspx");
                url = URLHelper.UpdateParameterInUrl(url, "threadGuid", SearchTaskInfoProvider.IndexerThreadGuid.ToString());
                if (WebFarmHelper.WebFarmEnabled)
                {
                    url = URLHelper.UpdateParameterInUrl(url, "serverName", WebFarmHelper.ServerName);
                }
                statusText = "<a href=\"javascript:void(0)\" onclick=\"modalDialog('" + url + "', 'ThreadProgress', '1000', '700');\" >" + statusName + "</a>";
            }

            ltlStatus.Text = ScriptHelper.GetLoaderInlineHtml(Page, statusText, "form-control-text");
            ltlStatus.Visible = true;
            lblStatus.Visible = false;
        }
        else
        {
            lblStatus.Text = statusName; 
        }

        // Show colored status name
        if (isNotReady)
        {
            lblStatus.Text = "<span class=\"StatusDisabled\">" + statusName + "</span>";
        }
        else if (SearchIndex.IndexFilesStatus == IndexStatusEnum.READY)
        {
            lblStatus.Text = "<span class=\"StatusEnabled\">" + statusName + "</span>";
        }

        // Is optimized
        lblIsOptimized.Text = UniGridFunctions.ColoredSpanYesNo(SearchIndex.IsOptimized());

        // Last update
        lblLastUpdate.Text = SearchIndex.IndexFilesLastUpdate.ToString();

        // Last rebuild
        lblLastRebuild.Text = GetString("general.notavailable");

        if (SearchIndex.IndexLastRebuildTime != DateTimeHelper.ZERO_TIME)
        {
            lblLastRebuild.Text = ValidationHelper.GetString(SearchIndex.IndexLastRebuildTime, "");
        }
    }
}
