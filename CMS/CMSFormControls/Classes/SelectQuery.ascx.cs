using System;
using CMS.DataEngine;
using CMS.FormControls;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.Base;

public partial class CMSFormControls_Classes_SelectQuery : FormEngineUserControl
{
    #region "Properties"

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
            if (uniSelector != null)
            {
                uniSelector.Enabled = value;
            }
        }
    }


    /// <summary>
    /// Returns ClientID of the textbox with query.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return uniSelector.TextBoxSelect.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return uniSelector.Value;
        }
        set
        {
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }
            uniSelector.Value = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            uniSelector.StopProcessing = true;
        }
        else
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        uniSelector.IsLiveSite = IsLiveSite;
        uniSelector.AllowEmpty = false;
        uniSelector.SetValue("FilterMode", QueryInfo.OBJECT_TYPE);

        // Check if user can edit the transformation
        bool editAuthorized = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Design", "EditSQLCode");
        bool createAuthorized = editAuthorized;

        SetDialog(editAuthorized, createAuthorized);
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        string value = uniSelector.TextBoxSelect.Text.Trim();

        // If macro or special value, do not validate
        if (MacroProcessor.ContainsMacro(value) || value == string.Empty)
        {
            return true;
        }

        try
        {
            // Check if query exists
            QueryInfoProvider.GetQueryInfo(value);
        }
        catch
        {
            ValidationError = GetString("query.queryorclassnotexist").Replace("%%code%%", value);
            return false;
        }

        return true;
    }


    /// <summary>
    /// Sets edit and new dialog URLs depending on whether the user is authorized.
    /// </summary>
    /// <param name="allowEdit">True, if edit button should be active, otherwise false</param>
    /// <param name="allowNew">True, if new button should be active, otherwise false</param>
    private void SetDialog(bool allowEdit, bool allowNew)
    {
        string baseUrl = "~/CMSModules/DocumentTypes/Pages/Development/DocumentType_Edit_Query_Edit.aspx?editonlycode=true";

        if (allowEdit)
        {
            uniSelector.EditItemPageUrl = URLHelper.AddParameterToUrl(baseUrl, "name", "##ITEMID##");
            uniSelector.EditDialogWindowHeight = 780;
        }

        if (allowNew)
        {
            uniSelector.NewItemPageUrl = URLHelper.AddParameterToUrl(baseUrl, "selectedvalue", "##ITEMID##");
        }
    }

    #endregion
}