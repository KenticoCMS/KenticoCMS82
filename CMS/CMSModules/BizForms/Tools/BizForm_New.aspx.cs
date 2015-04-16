using System;
using System.Linq;

using CMS.DataEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.Membership;
using CMS.Base;
using CMS.OnlineForms;
using CMS.PortalEngine;
using CMS.Search;
using CMS.SiteProvider;
using CMS.UIControls;

[Title("BizForm_New.HeaderCaption")]
[UIElement("CMS.Form", "Form.AddForm")]
public partial class CMSModules_BizForms_Tools_BizForm_New : CMSBizFormPage
{
    private const string bizFormNamespace = "BizForm";
    private string mFormTablePrefix = null;


    /// <summary>
    /// Returns prefix for bizform table name.
    /// </summary>
    private string FormTablePrefix
    {
        get
        {
            if (string.IsNullOrEmpty(mFormTablePrefix))
            {
                mFormTablePrefix = String.Format("Form_{0}_", ValidationHelper.GetIdentifier(SiteContext.CurrentSiteName));
            }

            return mFormTablePrefix;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check 'CreateForm' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.form", "CreateForm"))
        {
            RedirectToAccessDenied("cms.form", "CreateForm");
        }

        // Validator initializations
        rfvFormDisplayName.ErrorMessage = GetString("BizForm_Edit.ErrorEmptyFormDispalyName");
        rfvFormName.ErrorMessage = GetString("BizForm_Edit.ErrorEmptyFormName");
        rfvTableName.ErrorMessage = GetString("BizForm_Edit.ErrorEmptyFormTableName");

        // Control initializations
        lblFormDisplayName.Text = GetString("BizForm_Edit.FormDisplayNameLabel");
        lblFormName.Text = GetString("BizForm_Edit.FormNameLabel");
        lblTableName.Text = GetString("BizForm_Edit.TableNameLabel");
        lblPrefix.Text = FormTablePrefix + "&nbsp;";
        // Remove prefix length from maximum allowed length of table name
        txtTableName.MaxLength = 100 - FormTablePrefix.Length;
        btnOk.Text = GetString("General.OK");

        // Page title control initialization
        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("BizForm_Edit.ItemListLink"),
            RedirectUrl = "~/CMSModules/BizForms/Tools/BizForm_List.aspx"
        });

        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("BizForm_Edit.NewItemCaption")
        });
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        if (!BizFormInfoProvider.LicenseVersionCheck(RequestContext.CurrentDomain, FeatureEnum.BizForms, ObjectActionEnum.Insert))
        {
            ShowError(GetString("LicenseVersion.BizForm"));
            return;
        }

        DataClassInfo dci = null;
        BizFormInfo bizFormObj = null;

        string errorMessage = new Validator().NotEmpty(txtFormDisplayName.Text, rfvFormDisplayName.ErrorMessage).
            NotEmpty(txtFormName.Text, rfvFormName.ErrorMessage).
            NotEmpty(txtTableName.Text, rfvTableName.ErrorMessage).
            IsIdentifier(txtFormName.Text, GetString("bizform_edit.errorformnameinidentifierformat")).
            IsIdentifier(txtTableName.Text, GetString("BizForm_Edit.ErrorFormTableNameInIdentifierFormat")).Result;

        if (String.IsNullOrEmpty(errorMessage))
        {
            using (var tr = new CMSTransactionScope())
            {
                // Prepare the values
                string formDisplayName = txtFormDisplayName.Text.Trim();

                bizFormObj = new BizFormInfo();
                bizFormObj.FormDisplayName = formDisplayName;
                bizFormObj.FormName = txtFormName.Text.Trim();
                bizFormObj.FormSiteID = SiteContext.CurrentSiteID;
                bizFormObj.FormEmailAttachUploadedDocs = true;
                bizFormObj.FormItems = 0;
                bizFormObj.FormClearAfterSave = false;
                bizFormObj.FormLogActivity = true;

                // Ensure the code name
                bizFormObj.Generalized.EnsureCodeName();

                // Table name is combined from prefix ('BizForm_<sitename>_') and custom table name
                string safeFormName = ValidationHelper.GetIdentifier(bizFormObj.FormName);
                bizFormObj.FormName = safeFormName;

                string className = bizFormNamespace + "." + safeFormName;

                // Generate the table name
                string tableName = txtTableName.Text.Trim();
                if (String.IsNullOrEmpty(tableName) || (tableName == InfoHelper.CODENAME_AUTOMATIC))
                {
                    tableName = safeFormName;
                }
                tableName = FormTablePrefix + tableName;

                TableManager tm = new TableManager(null);

                // TableName wont be longer than 60 letters and will be unique
                if (tableName.Length > 60)
                {
                    int x = 1;

                    while (tm.TableExists(tableName.Substring(0, 59) + x.ToString()))
                    {
                        x++;
                    }

                    tableName = tableName.Substring(0, 59) + x.ToString();
                }

                // If first letter of safeFormName is digit, add "PK" to beginning
                string primaryKey = BizFormInfoProvider.GenerateFormPrimaryKeyName(bizFormObj.FormName);

                try
                {
                    // Create new table in DB
                    tm.CreateTable(tableName, primaryKey);
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;

                    // Table with the same name already exists
                    ShowError(string.Format(GetString("bizform_edit.errortableexists"), tableName));
                    return;
                }

                // Change table owner
                try
                {
                    string owner = SqlHelper.GetDBSchema(SiteContext.CurrentSiteName);
                    if ((!String.IsNullOrEmpty(owner)) && (owner.ToLowerCSafe() != "dbo"))
                    {
                        tm.ChangeDBObjectOwner(tableName, owner);
                        tableName = owner + "." + tableName;
                    }
                }
                catch (Exception ex)
                {
                    EventLogProvider.LogException("BIZFORM_NEW", "E", ex);
                }

                // Convert default datetime to string in english format
                string defaultDateTime = DateTime.Now.ToString(CultureHelper.EnglishCulture.DateTimeFormat);

                try
                {
                    // Add FormInserted and FormUpdated columns to the table
                    tm.AddTableColumn(tableName, "FormInserted", "datetime", false, defaultDateTime);
                    tm.AddTableColumn(tableName, "FormUpdated", "datetime", false, defaultDateTime);
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;

                    // Column wasn't added successfully
                    ShowError(errorMessage);

                    return;
                }

                // Create the BizForm class
                dci = BizFormInfoProvider.CreateBizFormDataClass(className, formDisplayName, tableName, primaryKey);

                try
                {
                    // Create new bizform dataclass
                    using (CMSActionContext context = new CMSActionContext())
                    {
                        // Disable logging of tasks
                        context.DisableLogging();

                        // Set default search settings
                        dci.ClassSearchEnabled = true;

                        DataClassInfoProvider.SetDataClassInfo(dci);

                        // Create default search settings
                        dci.ClassSearchSettings = SearchHelper.GetDefaultSearchSettings(dci);
                        dci.ClassSearchCreationDateColumn = "FormInserted";
                        DataClassInfoProvider.SetDataClassInfo(dci);
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;

                    // Class with the same name already exists
                    ShowError(errorMessage);

                    return;
                }

                // Create new bizform
                bizFormObj.FormClassID = dci.ClassID;

                try
                {
                    // Create new bizform
                    BizFormInfoProvider.SetBizFormInfo(bizFormObj);
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;

                    ShowError(errorMessage);

                    return;
                }

                tr.Commit();

                if (String.IsNullOrEmpty(errorMessage))
                {
                    // Redirect to Form builder tab
                    string url = UIContextHelper.GetElementUrl("CMS.Form", "Forms.Properties", false, bizFormObj.FormID);
                    url = URLHelper.AddParameterToUrl(url, "tabname", "Forms.FormBuldier");
                    URLHelper.Redirect(url);
                }
            }
        }
        else
        {
            ShowError(errorMessage);
        }
    }
}