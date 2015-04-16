using System;

using CMS.ExtendedControls;
using CMS.FormControls;

public partial class CMSFormControls_SimpleCountrySelector : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets state enable.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return countrySelector.Enabled;
        }
        set
        {
            countrySelector.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which indicates whether the selector should load all data to DDL.
    /// </summary>
    public bool DisplayAllItems
    {
        get
        {
            return countrySelector.DisplayAllItems;
        }
        set
        {
            countrySelector.DisplayAllItems = value;
        }
    }


    /// <summary>
    /// Add '(none)' record to the dropdownlist.
    /// </summary>
    public bool AddNoneRecord
    {
        get
        {
            return countrySelector.AddNoneRecord;
        }
        set
        {
            countrySelector.AddNoneRecord = value;
        }
    }


    /// <summary>
    /// Add '(select country)' record to the dropdownlist.
    /// </summary>
    public bool AddSelectCountryRecord
    {
        get
        {
            return countrySelector.AddSelectCountryRecord;
        }
        set
        {
            countrySelector.AddSelectCountryRecord = value;
        }
    }


    /// <summary>
    /// Set/get Value property in the form 'CountryName;StateName' or 'CountryID;StateID'
    /// </summary>
    public bool UseCodeNameForSelection
    {
        get
        {
            return countrySelector.UseCodeNameForSelection;
        }
        set
        {
            countrySelector.UseCodeNameForSelection = value;
        }
    }


    /// <summary>
    /// Selected country ID.
    /// </summary>
    public int CountryID
    {
        get
        {
            return countrySelector.CountryID;
        }
        set
        {
            countrySelector.CountryID = value;
        }
    }


    /// <summary>
    /// Selected Country name.
    /// </summary>
    public string CountryName
    {
        get
        {
            return countrySelector.CountryName;
        }
        set
        {
            countrySelector.CountryName = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return countrySelector.Value;
        }
        set
        {
            countrySelector.Value = value;
        }
    }


    /// <summary>
    /// Returns the DDL with countries.
    /// </summary>
    public CMSDropDownList CountryDropDown
    {
        get
        {
            return countrySelector.CountryDropDown;
        }
    }


    /// <summary>
    /// Gets client ID of the country drop down list.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return countrySelector.ValueElementID;
        }
    }

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    #endregion
}