using System;

using CMS.FormControls;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.FormEngine;
using CMS.DataEngine;
using CMS.Helpers;

public partial class CMSModules_Newsletters_FormControls_NewsletterSelector : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets selector value
    /// </summary>
    public override object Value
    {
        get
        {
            return UniSelector.Value;
        }
        set
        {
            UniSelector.Value = value;
        }
    }


    /// <summary>
    /// Gets Value display name.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            return UniSelector.ValueDisplayName;
        }
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            UniSelector.Enabled = value;
        }
    }
    

    /// <summary>
    /// Gets ClientID of the dropdown-list with newsletters.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return UniSelector.ClientID;
        }
    }


    /// <summary>
    /// Uni-selector mode.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            EnsureChildControls();
            return usNewsletters;
        }
    }


    /// <summary>
    /// Gets or sets if site filter should be shown or not.
    /// </summary>
    public bool ShowSiteFilter
    {
        get
        {
            return GetValue("ShowSiteFilter", true);
        }
        set
        {
            SetValue("ShowSiteFilter", value);
        }
    }


    /// <summary>
    /// Gets or sets the resource prefix of uni selector. If not set default values are used.
    /// </summary>
    public string ResourcePrefix
    {
        get
        {
            return UniSelector.ResourcePrefix;
        }
        set
        {
            UniSelector.ResourcePrefix = value;
        }
    }


    /// <summary>
    /// Indicates if selector works in simple mode - without any special fields.
    /// </summary>
    public bool UseSimpleMode
    {
        get
        {
            return GetValue("UseSimpleMode", false);
        }
        set
        {
            SetValue("UseSimpleMode", value);
        }
    }


    /// <summary>
    /// Gets underlying form control.
    /// </summary>
    protected override FormEngineUserControl UnderlyingFormControl
    {
        get
        {
            return usNewsletters;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!StopProcessing)
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Setups control and reloads the data.
    /// </summary>
    protected void ReloadData()
    {
        usNewsletters.WhereCondition = "NewsletterSiteID = " + SiteContext.CurrentSiteID;

        if (!UseSimpleMode)
        {
            usNewsletters.SpecialFields.Add(new SpecialField { Text = GetString("NewsletterSelect.LetUserChoose"), Value = "NWSLetUserChoose" });
        }

        // Return newsletter name or newsletter ID according to type of field (if no field specified newsletter name is returned)
        if ((FieldInfo != null) && DataTypeManager.IsInteger(TypeEnum.Field, FieldInfo.DataType))
        {
            usNewsletters.AllowEmpty = true;
            usNewsletters.ReturnColumnName = "NewsletterID";
        }
    }


    /// <summary>
    /// Returns WHERE condition for selected form.
    /// </summary>
    public override string GetWhereCondition()
    {
        // Return correct WHERE condition for integer if none value is selected
        if ((FieldInfo != null) && DataTypeManager.IsInteger(TypeEnum.Field, FieldInfo.DataType))
        {
            int id = ValidationHelper.GetInteger(usNewsletters.Value, 0);
            if (id <= 0)
            {
                return null;
            }
        }
        return base.GetWhereCondition();
    }

    #endregion
}