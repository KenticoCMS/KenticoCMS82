using System;
using System.Linq;

using CMS.DataCom;
using CMS.UIControls;

/// <summary>
/// A control that provides the UI elements to display and edit the Data.com company filter.
/// </summary>
public partial class CMSModules_ContactManagement_Controls_UI_DataCom_CompanyFilter : CMSAdminControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the Data.com company filter.
    /// </summary>
    public CompanyFilter Filter
    {
        get
        {
            return GetFilter();
        }
        set
        {
            SetFilter(value);
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Occurs when the Search button is clicked.
    /// </summary>
    public event EventHandler Search;

    #endregion


    #region "Life cycle methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        SearchButton.Click += new EventHandler(SearchButton_Click);
    }


    protected void SearchButton_Click(object sender, EventArgs e)
    {
        if (Search != null)
        {
            Search.Invoke(this, EventArgs.Empty);
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Creates and initializes a new instance of the CompanyFilter class with the UI element values, and returns it.
    /// </summary>
    /// <returns>A new instance of the CompanyFilter class initialized with the UI element values.</returns>
    private CompanyFilter GetFilter()
    {
        CompanyFilter filter = new CompanyFilter();
        filter.Name.Add(TrimFilterValue(NameTextBox.Text));

        return filter;
    }


    /// <summary>
    /// Updates the UI element values with the specified Data.com company filter values.
    /// </summary>
    /// <param name="filter">A Data.com company filter.</param>
    private void SetFilter(CompanyFilter filter)
    {
        NameTextBox.Text = filter.Name.FirstOrDefault();
    }


    /// <summary>
    /// Removes all leading and trailing white-space characters from the specified string, and returns it.
    /// </summary>
    /// <param name="value">A string to trim.</param>
    /// <returns>The specified string without all leading and trailing white-space characters, if the result is not an empty string; otherwise, null.</returns>
    private string TrimFilterValue(string value)
    {
        if (String.IsNullOrEmpty(value))
        {
            return null;
        }
        value = value.Trim();
        if (String.Empty.Equals(value))
        {
            return null;
        }

        return value;
    }

    #endregion
}