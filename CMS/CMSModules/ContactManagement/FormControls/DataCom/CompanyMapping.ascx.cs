using System;
using System.Collections;
using System.Web.UI;

using CMS.DataCom;
using CMS.EventLog;
using CMS.FormControls;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Base;

/// <summary>
/// A control that provides the UI elements to display and edit the Data.com company mapping.
/// </summary>
public partial class CMSModules_ContactManagement_FormControls_DataCom_CompanyMapping : FormEngineUserControl, ICallbackEventHandler
{
    #region "Private members"

    private bool mEnabled = true;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the serialized Data.com company mapping.
    /// </summary>
    public override object Value
    {
        get
        {
            return MappingHiddenField.Value;
        }
        set
        {
            MappingHiddenField.Value = ValidationHelper.GetString(value, String.Empty);
        }
    }


    /// <summary>
    /// Gets or sets the value indicating whether this control is enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
        }
    }


    /// <summary>
    /// Gets the client identifier of the control holding the setting value.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return MappingHiddenField.ClientID;
        }
    }

    #endregion


    #region "Protected properties"

    protected string ParametersId
    {
        get
        {
            string parametersId = ViewState["PID"] as string;
            if (String.IsNullOrEmpty(parametersId))
            {
                parametersId = Guid.NewGuid().ToString("N");
                ViewState["PID"] = parametersId;
            }

            return parametersId;
        }
    }

    #endregion


    #region "Life cycle methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterJQuery(Page);
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        string baseUrl = URLHelper.ResolveUrl("~/CMSModules/ContactManagement/Pages/Tools/DataCom/EditMapping.aspx");
        string url = String.Format("{0}?pid={1}", baseUrl, ParametersId);
        string script = String.Format("function DataCom_EditCompanyMapping (arg, context) {{ modalDialog('{0}', 'EditCompanyMapping', '770', '700', null); return false; }}", URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url)));
        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "DataCom_EditCompanyMapping", script, true);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        InitializeControls();
    }

    #endregion


    #region "Private methods"

    private void InitializeControls()
    {
        try
        {
            string content = Value as string;
            if (!String.IsNullOrEmpty(content))
            {
                EntityMappingSerializer serializer = new EntityMappingSerializer();
                EntityMapping mapping = serializer.UnserializeEntityMapping(content);
                CompanyMappingControl.Mapping = mapping;
                CompanyMappingControl.Enabled = Enabled;
            }
            if (Enabled)
            {
                InitializeEditButton();
            }
            else
            {
                EditMappingButton.Visible = false;
            }
        }
        catch (Exception exception)
        {
            HandleException(exception);
        }
    }


    /// <summary>
    /// Configures the edit button to open the dialog window to edit the specified Data.com company mapping.
    /// </summary>
    private void InitializeEditButton()
    {
        EditMappingButton.OnClientClick = String.Format("{0}; return false;", Page.ClientScript.GetCallbackEventReference(this, null, "DataCom_EditCompanyMapping", null));
    }


    private Hashtable CreateParameters()
    {
        Hashtable parameters = new Hashtable();
        parameters["EntityName"] = "Company";
        parameters["Mapping"] = Value;
        parameters["MappingHiddenFieldClientId"] = MappingHiddenField.ClientID;
        parameters["MappingPanelClientId"] = MappingPanel.ClientID;

        return parameters;
    }


    /// <summary>
    /// Displays an error message and logs the specified exception to the event log.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    private void HandleException(Exception exception)
    {
        ErrorSummary.Report(exception);
        EventLogProvider.LogException("Data.com Connector", "CompanyMappingFormControl", exception);
    }

    #endregion


    #region ICallbackEventHandler Members

    string ICallbackEventHandler.GetCallbackResult()
    {
        return null;
    }


    void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
    {
        Hashtable parameters = CreateParameters();
        WindowHelper.Add(ParametersId, parameters);
    }

    #endregion
}