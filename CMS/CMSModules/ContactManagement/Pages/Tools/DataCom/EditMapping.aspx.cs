using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.DataCom;
using CMS.EventLog;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.ExtendedControls;

/// <summary>
/// A dialog page where user can edit the mapping of CMS contact (or account) fields and Data.com contact (or company) attributes.
/// </summary>
public partial class CMSModules_ContactManagement_Pages_Tools_DataCom_EditMapping : CMSDataComDialogPage
{
    #region "Inner classes"

    /// <summary>
    /// Represents a set of objects that depend on the mapping context.
    /// </summary>
    private class RequestContext
    {

        /// <summary>
        /// CMS contact (or account) form info.
        /// </summary>
        public FormInfo FormInfo;


        /// <summary>
        /// Data.com contact (or company) entity info.
        /// </summary>
        public EntityInfo EntityInfo;


        /// <summary>
        /// Delegate to initialize the mapping control.
        /// </summary>
        public Action<EntityMapping> InitializeMappingControl;

    }

    #endregion


    #region "Variables"

    private string mEntityName;
    private string mSourceMappingHiddenFieldClientId;
    private string mSourceMappingPanelClientId;
    private EntityMapping mSourceMapping;


    /// <summary>
    /// Current request context.
    /// </summary>
    private RequestContext mContext;


    /// <summary>
    /// A dictionary of form field name/entity attribute dropdown control pairs.
    /// </summary>
    private Dictionary<string, CMSDropDownList> mAttributeNamesDropDownLists = new Dictionary<string, CMSDropDownList>();

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the client identifier of the hidden input field with mapping on the page that opened this dialog.
    /// </summary>
    protected string SourceMappingHiddenFieldClientId
    {
        get
        {
            return mSourceMappingHiddenFieldClientId;
        }
    }


    /// <summary>
    /// Gets the client identifier of the mapping control with mapping on the page that opened this dialog.
    /// </summary>
    protected string SourceMappingPanelClientId
    {
        get
        {
            return mSourceMappingPanelClientId;
        }
    }


    /// <summary>
    /// Gets the last available entity mapping associated with this dialog.
    /// </summary>
    protected EntityMapping SourceMapping
    {
        get
        {
            return mSourceMapping;
        }
    }

    #endregion


    #region "Life cycle methods"

    protected override void OnInit(EventArgs e)
    {
        ShowInformation(GetString("datacom.settings.mappinginfo"));
        base.OnInit(e);
        ScriptHelper.RegisterWOpenerScript(Page);
        ScriptHelper.RegisterJQuery(Page);
        PageTitle.TitleText = GetString("datacom.editmapping.title");
        Save += ConfirmButton_Click;
        MappingPanel.Style.Add("display", "none");
        try
        {
            RestoreParameters();
            mContext = CreateContext();
            InitializeControls();
        }
        catch (Exception exception)
        {
            HandleException(exception);
        }
    }


    protected void ConfirmButton_Click(object sender, EventArgs e)
    {
        if (mContext != null)
        {
            try
            {
                EntityMapping mapping = CreateEntityMappingFromForm();
                EntityMappingSerializer serializer = new EntityMappingSerializer();
                MappingHiddenField.Value = serializer.SerializeEntityMapping(mapping);
                mContext.InitializeMappingControl(mapping);
                string parametersIdentifier = QueryHelper.GetString("pid", null);
                Hashtable parameters = WindowHelper.GetItem(parametersIdentifier) as Hashtable;
                if (parameters != null)
                {
                    parameters["Mapping"] = MappingHiddenField.Value;
                    WindowHelper.Add(parametersIdentifier, parameters);
                }
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }
    }


    protected void MappingItemRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        LocalizedLabel literal = e.Item.FindControl("FieldNameLiteral") as LocalizedLabel;
        CMSDropDownList dropDownList = e.Item.FindControl("AttributeNamesDropDownList") as CMSDropDownList;
        FormFieldInfo fieldInfo = e.Item.DataItem as FormFieldInfo;
        literal.Text = ResHelper.LocalizeString(fieldInfo.GetDisplayName(MacroContext.CurrentResolver));
        dropDownList.Items.Add(new ListItem(String.Empty, String.Empty));
        dropDownList.Items.AddRange(mContext.EntityInfo.Items.Select(x => new ListItem(ResHelper.LocalizeString(x.DisplayName), x.Name)).ToArray());
        mAttributeNamesDropDownLists.Add(fieldInfo.Name, dropDownList);
    }

    #endregion


    #region "Private methods"

    private void InitializeControls()
    {
        MappingItemRepeater.DataSource = mContext.FormInfo.GetFields(true, false);
        MappingItemRepeater.DataBind();
        if (!RequestHelper.IsPostBack())
        {
            foreach (FormFieldInfo fieldInfo in mContext.FormInfo.GetFields(true, false))
            {
                CMSDropDownList dropDownList = mAttributeNamesDropDownLists[fieldInfo.Name];
                EntityMappingItem item = SourceMapping.GetItem(fieldInfo.Name);
                if (item == null)
                {
                    dropDownList.SelectedIndex = 0;
                }
                else
                {
                    dropDownList.SelectedValue = item.EntityAttributeName;
                }
            }
        }
    }


    /// <summary>
    /// Creates a mapping of CMS contact (or account) fields and Data.com contact (or company) attributes from the current form state, and returns it.
    /// </summary>
    /// <returns>A mapping of CMS contact (or account) fields and Data.com contact (or company) attributes.</returns>
    private EntityMapping CreateEntityMappingFromForm()
    {
        EntityMapping mapping = new EntityMapping();
        foreach (FormFieldInfo fieldInfo in mContext.FormInfo.GetFields(true, false))
        {
            CMSDropDownList dropDownList = mAttributeNamesDropDownLists[fieldInfo.Name];
            if (dropDownList.SelectedIndex != 0)
            {
                EntityAttributeInfo attributeInfo = mContext.EntityInfo.GetAttributeInfo(dropDownList.SelectedValue);
                if (attributeInfo != null)
                {
                    mapping.Add(attributeInfo, fieldInfo);
                }
            }
        }

        return mapping;
    }


    /// <summary>
    /// Creates current request context, and returns it.
    /// </summary>
    /// <returns>Current request context.</returns>
    private RequestContext CreateContext()
    {
        if (mEntityName == "Contact")
        {
            return new RequestContext
            {
                FormInfo = DataComHelper.GetContactFormInfo(),
                EntityInfo = DataComHelper.GetContactEntityInfo(),
                InitializeMappingControl = (x => ContactMappingControl.Mapping = x)
            };
        }
        if (mEntityName == "Company")
        {
            IDataComConfiguration configuration = DataComHelper.GetConfiguration(SiteContext.CurrentSiteID);
            return new RequestContext
            {
                FormInfo = DataComHelper.GetAccountFormInfo(),
                EntityInfo = DataComHelper.GetCompanyEntityInfo(configuration),
                InitializeMappingControl = (x => CompanyMappingControl.Mapping = x)
            };
        }
        throw new Exception(String.Format("[DataComEditMappingPage.CreateContext]: Invalid mapping type ({0}).", mEntityName));
    }


    private void RestoreParameters()
    {
        // Validate parameters
        if (!QueryHelper.ValidateHash("hash"))
        {
            throw new Exception("[DataComEditMappingPage.RestoreParameters]: Invalid query hash.");
        }
        Hashtable parameters = WindowHelper.GetItem(QueryHelper.GetString("pid", null)) as Hashtable;
        if (parameters == null)
        {
            throw new Exception("[DataComEditMappingPage.RestoreParameters]: The dialog page parameters are missing, the session might have been lost.");
        }

        // Restore parameters
        mEntityName = ValidationHelper.GetString(parameters["EntityName"], null);
        mSourceMappingHiddenFieldClientId = ValidationHelper.GetString(parameters["MappingHiddenFieldClientId"], null);
        mSourceMappingPanelClientId = ValidationHelper.GetString(parameters["MappingPanelClientId"], null);

        // Restore mapping
        string content = ValidationHelper.GetString(parameters["Mapping"], null);
        if (String.IsNullOrEmpty(content))
        {
            mSourceMapping = new EntityMapping();
        }
        else
        {
            EntityMappingSerializer serializer = new EntityMappingSerializer();
            mSourceMapping = serializer.UnserializeEntityMapping(content);
        }
    }


    /// <summary>
    /// Displays a default error message and logs the specified exception to the event log.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    private void HandleException(Exception exception)
    {
        ErrorSummary.Report(exception);
        EventLogProvider.LogException("Data.com Connector", "EditMappingPage", exception);
    }

    #endregion
}