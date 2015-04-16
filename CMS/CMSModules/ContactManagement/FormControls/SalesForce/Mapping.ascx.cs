using System;
using System.Collections;
using System.Web.UI;

using CMS.EventLog;
using CMS.FormControls;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.SalesForce;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DataEngine;

/// <summary>
/// Displays the mapping setting, and allows the user to edit it.
/// </summary>
public partial class CMSModules_ContactManagement_FormControls_SalesForce_Mapping : FormEngineUserControl, ICallbackEventHandler
{

    #region "Private members"

    private bool mEnabled = true;

    #endregion

    #region "Public properties"

    /// <summary>
    /// Gets or sets the mapping in serialized form.
    /// </summary>
    public override object Value
    {
        get
        {
            return MappingHiddenField.Value;
        }
        set
        {
            MappingHiddenField.Value = value as string;
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

    #region "Life-cycle methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterJQuery(Page);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        string baseUrl = URLHelper.ResolveUrl("~/CMSModules/ContactManagement/Pages/Tools/SalesForce/MappingEditor.aspx");
        string url = String.Format("{0}?pid={1}", baseUrl, ParametersId);
        string script = String.Format("function SalesForce_EditLeadMapping (arg, context) {{ modalDialog('{0}', 'EditLeadMapping', '900', '600', null); return false; }}", URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url)));
        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "SalesForce_EditLeadMapping", script, true);
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
                MappingSerializer serializer = new MappingSerializer();
                Mapping mapping = serializer.DeserializeMapping(content);
                MappingControl.Mapping = mapping;
                MappingControl.Enabled = Enabled;
            }
            if (Enabled)
            {
                string credentials = GetCredentials();
                if (!String.IsNullOrEmpty(credentials))
                {
                    InitializeEditButton();
                }
                else
                {
                    MessageLabel.InnerHtml = GetString("sf.credentialsrequiredformapping");
                    MessageLabel.Attributes.Add("class", "Red");
                    MessageLabel.Visible = true;
                    EditMappingButton.Visible = false;
                }
            }
            else
            {
                EditMappingButton.Visible = false;
            }
        }
        catch (Exception exception)
        {
            HandleError(exception);
        }
    }

    private void InitializeEditButton()
    {
        EditMappingButton.OnClientClick = String.Format("{0}; return false;", Page.ClientScript.GetCallbackEventReference(this, null, "SalesForce_EditLeadMapping", null));
    }

    private string GetCredentials()
    {
        string settingName = "CMSSalesForceCredentials";
        SiteInfo site = GetCurrentSiteForSettings();
        if (site != null)
        {
            settingName = String.Format("{0}.{1}", site.SiteName, settingName);
        }

        return SettingsKeyInfoProvider.GetValue(settingName);
    }

    private SiteInfo GetCurrentSiteForSettings()
    {
        int siteId = QueryHelper.GetInteger("SiteID", 0);
        SiteInfo site = SiteInfoProvider.GetSiteInfo(siteId);

        return site;
    }

    private Hashtable CreateParameters(string credentials)
    {
        Hashtable parameters = new Hashtable();
        parameters["EntityModelName"] = "Lead";
        parameters["Mapping"] = Value;
        parameters["MappingHiddenFieldClientId"] = MappingHiddenField.ClientID;
        parameters["MappingPanelClientId"] = MappingPanel.ClientID;
        parameters["Credentials"] = credentials;

        return parameters;
    }

    private void HandleError(Exception exception)
    {
        SalesForceError.Report(exception);
        EventLogProvider.LogException("Salesforce.com Connector", "MappingFormControl", exception);
    }

    #endregion
    
    #region ICallbackEventHandler Members

    string ICallbackEventHandler.GetCallbackResult()
    {
        return null;
    }

    void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
    {
        string credentials = GetCredentials();
        if (!String.IsNullOrEmpty(credentials))
        {
            Hashtable parameters = WindowHelper.GetItem(ParametersId) as Hashtable;
            if (parameters == null)
            {
                parameters = CreateParameters(credentials);
                WindowHelper.Add(ParametersId, parameters);
            }
        }
    }

    #endregion

}