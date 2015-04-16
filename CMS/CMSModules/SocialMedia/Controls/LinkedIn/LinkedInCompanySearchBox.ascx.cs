using System;

using CMS.Controls;
using CMS.DataEngine;
using CMS.UIControls;

public partial class CMSModules_SocialMedia_Controls_LinkedIn_LinkedInCompanySearchBox : CMSAbstractBaseFilterControl
{
    #region "State management"

    /// <summary>
    /// Applies filter on associated UniGrid control.
    /// </summary>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string text = txtSearch.Text.Trim();
        if (!string.IsNullOrEmpty(text))
        {
            WhereCondition = new WhereCondition()
                .WhereContains("CompanyName", text)
                .ToString(true);
        }
        else
        {
            WhereCondition = null;
        }

        //Raise OnFilterChange event
        RaiseOnFilterChanged();
    }

    #endregion
}