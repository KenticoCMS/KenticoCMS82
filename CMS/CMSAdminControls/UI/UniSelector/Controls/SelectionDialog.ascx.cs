using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;

using CMS.Base;
using CMS.Controls;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.UIControls;

public partial class CMSAdminControls_UI_UniSelector_Controls_SelectionDialog : CMSUserControl, IObjectTypeDriven, ICallbackEventHandler
{
    #region "Variables"

    private SelectionModeEnum selectionMode = SelectionModeEnum.SingleButton;
    private string resourcePrefix = "general";
    private string objectType;
    private string returnColumnName;
    private string displayNameFormat;
    private string valuesSeparator = ";";

    private string filterControl;

    private bool useDefaultNameFilter = true;

    private string whereCondition;
    private string orderBy;
    private string siteWhereCondition;

    private int itemsPerPage = 10;

    private GeneralizedInfo iObjectType;

    private string txtClientId;
    private string hdnClientId;
    private string hdnDrpId;
    private string hashId;
    private string callbackValues;

    private CMSAbstractBaseFilterControl searchControl;

    private string emptyReplacement = "&nbsp;";
    private string parentClientId;
    private string dialogGridName = "~/CMSAdminControls/UI/UniSelector/DialogItemList.xml";
    private string additionalColumns;
    private string additionalSearchColumns = String.Empty;
    private string callbackMethod;
    private string disabledItems;
    private string filterMode;

    private bool allowEditTextBox;
    private bool fireOnChanged;
    private bool mLocalizeItems = true;

    private string mGlobalObjectSuffix;

    private Hashtable parameters;
    private readonly Dictionary<string, string> hashItems = new Dictionary<string, string>();

    private string mSearchColumns = String.Empty;

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates whether to remove multiple commas (can happen when DisplayNameFormat is like {%column1%}, {%column2%}, {column3} and column2 is empty.
    /// </summary>
    public bool RemoveMultipleCommas
    {
        get;
        set;
    }


    /// <summary>
    /// If true, the selector uses the type condition to get the data
    /// </summary>
    public bool UseTypeCondition
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or set the suffix which is added to global objects if AddGlobalObjectSuffix is true. Default is "(global)".
    /// </summary>
    public string GlobalObjectSuffix
    {
        get
        {
            if (string.IsNullOrEmpty(mGlobalObjectSuffix))
            {
                mGlobalObjectSuffix = GetString("general.global");
            }
            return mGlobalObjectSuffix;
        }
        set
        {
            mGlobalObjectSuffix = value;
        }
    }


    /// <summary>
    /// Indicates whether global objects have suffix "(global)" in the grid.
    /// </summary>
    public bool AddGlobalObjectSuffix
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether global object name should have prefix '.'
    /// </summary>
    public bool AddGlobalObjectNamePrefix
    {
        get;
        set;
    }


    /// <summary>
    /// Specifies whether the selection dialog should resolve localization macros.
    /// </summary>
    public bool LocalizeItems
    {
        get
        {
            return mLocalizeItems;
        }
        set
        {
            mLocalizeItems = value;
        }
    }


    /// <summary>
    /// Current page index.
    /// </summary>
    public int PageIndex
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["PageIndex"], 0);
        }
        set
        {
            ViewState["PageIndex"] = value;
        }
    }


    /// <summary>
    /// Item prefix.
    /// </summary>
    public string ItemPrefix
    {
        get
        {
            return ValidationHelper.GetString(ViewState["ItemPrefix"], string.Empty);
        }
        set
        {
            ViewState["ItemPrefix"] = value;
        }
    }


    /// <summary>
    /// Item prefix.
    /// </summary>
    public string FilterWhere
    {
        get
        {
            return ValidationHelper.GetString(ViewState["FilterWhere"], string.Empty);
        }
        set
        {
            ViewState["FilterWhere"] = value;
        }
    }


    /// <summary>
    /// Control's unigrid.
    /// </summary>
    public UniGrid UniGrid
    {
        get
        {
            return uniGrid;
        }
    }


    /// <summary>
    /// Gets or sets current object type.
    /// </summary>
    public string ObjectType
    {
        get
        {
            return ValidationHelper.GetString(parameters["ObjectType"], String.Empty);
        }
        set
        {
            parameters["ObjectType"] = value;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Load parameters
        LoadParameters();

        // Load custom filter
        LoadFilter();
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Register event handlers
        uniGrid.OnExternalDataBound += uniGrid_OnExternalDataBound;
        uniGrid.OnPageChanged += uniGrid_OnPageChanged;
        uniGrid.IsLiveSite = IsLiveSite;
        if (!RequestHelper.IsPostBack())
        {
            uniGrid.Pager.DefaultPageSize = 10;
        }

        btnSearch.Click += btnSearch_Click;

        // Load data into the unigrid
        LoadControls();

        // Get control IDs from parent window
        txtClientId = QueryHelper.GetString("txtElem", string.Empty);
        hdnClientId = QueryHelper.GetString("hidElem", string.Empty);
        hdnDrpId = QueryHelper.GetString("selectElem", string.Empty);
        parentClientId = QueryHelper.GetString("clientId", string.Empty);
        hashId = QueryHelper.GetString("hashElem", string.Empty);

        string scriptValuesSeparator = ScriptHelper.GetString(valuesSeparator);
        string regexEscapedValuesSeparator = ScriptHelper.GetString(Regex.Escape(valuesSeparator), false);

        // Buttons scripts
        string buttonsScript = "function US_Cancel(){ Cancel(); return false; }";

        switch (selectionMode)
        {
            // Button modes
            case SelectionModeEnum.SingleButton:
            case SelectionModeEnum.MultipleButton:
                buttonsScript += "function US_Submit(){ SelectItems(escape(ItemsElem().value),HashElem().value); return false; }";
                break;

            // Textbox modes
            case SelectionModeEnum.SingleTextBox:
            case SelectionModeEnum.MultipleTextBox:
                if (allowEditTextBox)
                {
                    buttonsScript += "function US_Submit(){ SelectItems(escape(ItemsElem().value), escape(ItemsElem().value.replace(/^" + regexEscapedValuesSeparator + "+|" + regexEscapedValuesSeparator + "+$/g, '')), " + ScriptHelper.GetString(hdnClientId) + ", " + ScriptHelper.GetString(txtClientId) + ", " + ScriptHelper.GetString(hashId) + ", HashElem().value); return false; }";
                }
                else
                {
                    buttonsScript += "function US_Submit(){ SelectItemsReload(escape(ItemsElem().value), escape(nameElem.value), " + ScriptHelper.GetString(hdnClientId) + ", " + ScriptHelper.GetString(txtClientId) + ", " + ScriptHelper.GetString(hdnDrpId) + ", " + ScriptHelper.GetString(hashId) + ", HashElem().value); return false; }";
                }
                break;

            // Other modes
            default:
                buttonsScript += "function US_Submit(){ SelectItemsReload(escape(ItemsElem().value), escape(nameElem.value), " + ScriptHelper.GetString(hdnClientId) + ", " + ScriptHelper.GetString(txtClientId) + ", " + ScriptHelper.GetString(hdnDrpId) + ", " + ScriptHelper.GetString(hashId) + ", HashElem().value); return false; }";
                break;
        }

        string script;

        switch (selectionMode)
        {
            // Button modes
            case SelectionModeEnum.SingleButton:
            case SelectionModeEnum.MultipleButton:
                {
                    // Register javascript code
                    if (callbackMethod == null)
                    {
                        script = string.Format("function SelectItems(items,hash) {{ wopener.US_SelectItems_{0}(items,hash); CloseDialog(); }}", parentClientId);
                    }
                    else
                    {
                        script = string.Format("function SelectItems(items,hash) {{ wopener.{0}(items.replace(/^{1}+|{1}+$/g, ''),hash); CloseDialog(); }}", callbackMethod, regexEscapedValuesSeparator);
                    }
                }
                break;

            // Selector modes
            default:
                {
                    // Register javascript code
                    script = @"
function SelectItems(items, names, hiddenFieldId, txtClientId, hashClientId, hash) {
    if(items.length > 0) {
        wopener.US_SetItems(items, names, hiddenFieldId, txtClientId, null, hashClientId, hash);
    } else {
        wopener.US_SetItems('','', hiddenFieldId, txtClientId);
    }" +
(fireOnChanged ? "wopener.US_SelectionChanged_" + parentClientId + "();" : "")
+ @"
    return CloseDialog(); 
}

function SelectItemsReload(items, names, hiddenFieldId, txtClientId, hidValue, hashClientId, hash) {
    if (items.length > 0) {
        wopener.US_SetItems(items, names, hiddenFieldId, txtClientId, hidValue, hashClientId, hash);
    } else {
        wopener.US_SetItems('','', hiddenFieldId, txtClientId);
    }
    wopener.US_ReloadPage_" + parentClientId + @"();
    return CloseDialog();
}";
                }
                break;
        }

        StringBuilder sb = new StringBuilder();
        sb.Append(@"
var nameElem = document.getElementById('", hidName.ClientID, @"');

function ItemsElem() {
    return document.getElementById('", hidItem.ClientID, @"');
}

function HashElem() {
    return document.getElementById('", hidHash.ClientID, @"');
}

function SetHash(hashvalue) {
    var hashElem = HashElem();
    if (hashElem != null) {
        hashElem.value = hashvalue;
    }
}

function ProcessItem(chkbox, hash, changeChecked, getHash) {
    var itemsElem = ItemsElem();
    var items = itemsElem.value;
    var checkHash = '';
    if (chkbox != null) {
        var item = chkbox.id.substr(3);
        if (changeChecked) {
            chkbox.checked = !chkbox.checked;
        }
        if (chkbox.checked) {
            if (items == '') {
                itemsElem.value = ", scriptValuesSeparator, @" + item + ", scriptValuesSeparator, @";
            }
            else if (items.toLowerCase().indexOf(", scriptValuesSeparator, @" + item.toLowerCase() + ", scriptValuesSeparator, @") < 0)
            {
                itemsElem.value += item + ", scriptValuesSeparator, @";
            }
        }
        else
        {
            var re = new RegExp('", regexEscapedValuesSeparator, @"' + item + '", regexEscapedValuesSeparator, @"', 'i');
            itemsElem.value = items.replace(re, ", scriptValuesSeparator, @");
        }
        checkHash = '|' + item + '#' + hash;
    }
    else
    {
        checkHash = '|' + items + '#' + hash;
    }
    if (getHash) {
        ", Page.ClientScript.GetCallbackEventReference(this, "itemsElem.value + checkHash", "SetHash", null), @";
    }
}

function Cancel() { CloseDialog(); }

function SelectAllItems(checkbox, hash) {
    var itemsElem = ItemsElem();
    itemsElem.value = '';
    SetHash('');
    var checkboxes = document.getElementsByTagName('input');
    for(var i = 0; i < checkboxes.length; i++) {
        var chkbox = checkboxes[i];
        if (chkbox.className == 'chckbox') {
            if(checkbox.checked) { chkbox.checked = true; }
            else { chkbox.checked = false; }

            ProcessItem(chkbox, null, false, false);
        }
    }
    ProcessItem(null, hash, false, true);
}");

        ltlScript.Text = ScriptHelper.GetScript(script + sb + buttonsScript);
    }


    /// <summary>
    /// Change header title for multiple selection.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            ChangeSearchCondition();
        }

        // Load the grid data
        ReloadGrid();

        if (uniGrid.GridView.HeaderRow != null)
        {
            switch (selectionMode)
            {
                // Multiple selection
                case SelectionModeEnum.Multiple:
                case SelectionModeEnum.MultipleTextBox:
                case SelectionModeEnum.MultipleButton:
                    {
                        // Add checkbox for selection of all items on the current page
                        CMSCheckBox chkAll = new CMSCheckBox
                        {
                            ID = "chkAll",
                            ToolTip = GetString("UniSelector.CheckAll")
                        };

                        // Prepare string for hash
                        string values = String.Join(valuesSeparator, hashItems.Keys);
                        chkAll.Attributes.Add("onclick", String.Format("SelectAllItems(this,'{0}')", ValidationHelper.GetHashString(values)));

                        uniGrid.GridView.HeaderRow.Cells[0].Controls.Clear();
                        uniGrid.GridView.HeaderRow.Cells[0].Controls.Add(chkAll);
                        uniGrid.GridView.Columns[0].ItemStyle.CssClass = "unigrid-selection";

                        uniGrid.GridView.HeaderRow.Cells[1].Text = GetString(resourcePrefix + ".itemname|general.itemname");
                    }
                    break;

                // Single selection
                default:
                    {
                        uniGrid.GridView.Columns[0].Visible = false;
                        uniGrid.GridView.HeaderRow.Cells[1].Text = GetString(resourcePrefix + ".itemname|general.itemname");
                    }
                    break;
            }
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Unigrid external data bound handler.
    /// </summary>
    protected object uniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "yesno":
                return UniGridFunctions.ColoredSpanYesNo(parameter);

            case "select":
                {
                    var ti = iObjectType.TypeInfo;

                    DataRowView drv = (parameter as DataRowView);

                    // Get item ID
                    string itemID = drv[returnColumnName].ToString();
                    string hashKey = itemID;

                    // Add global object name prefix if required
                    if (AddGlobalObjectNamePrefix && !String.IsNullOrEmpty(ti.SiteIDColumn) && (ValidationHelper.GetInteger(DataHelper.GetDataRowValue(drv.Row, ti.SiteIDColumn), 0) == 0))
                    {
                        itemID = "." + itemID;
                    }

                    // Store hash codes for grid items
                    if (!hashItems.ContainsKey(hashKey))
                    {
                        hashItems.Add(hashKey, ValidationHelper.GetHashString(itemID));
                    }

                    // Add checkbox for multiple selection
                    switch (selectionMode)
                    {
                        case SelectionModeEnum.Multiple:
                        case SelectionModeEnum.MultipleTextBox:
                        case SelectionModeEnum.MultipleButton:
                            {
                                string checkBox = string.Format("<span class=\"checkbox\"><input id=\"chk{0}\" type=\"checkbox\" onclick=\"ProcessItem(this,'{1}',false,true);\" class=\"chckbox\" ", itemID, hashItems[hashKey]);
                                if (hidItem.Value.IndexOfCSafe(valuesSeparator + itemID + valuesSeparator, true) >= 0)
                                {
                                    checkBox += "checked=\"checked\" ";
                                }
                                if (disabledItems.Contains(string.Format("{0}{1}{0}", valuesSeparator, itemID)))
                                {
                                    checkBox += "disabled=\"disabled\" ";
                                }

                                checkBox += string.Format("/><label for=\"chk{0}\">&nbsp;</label></span>", itemID);

                                return checkBox;
                            }
                    }
                }
                break;

            case "itemname":
                {
                    DataRowView drv = (parameter as DataRowView);

                    // Get item ID
                    string itemID = drv[returnColumnName].ToString();
                    string hashKey = itemID;

                    // Get item name
                    string itemName;

                    // Special formatted user name
                    if (displayNameFormat == UniSelector.USER_DISPLAY_FORMAT)
                    {
                        string userName = ValidationHelper.GetString(DataHelper.GetDataRowValue(drv.Row, "UserName"), String.Empty);
                        string fullName = ValidationHelper.GetString(DataHelper.GetDataRowValue(drv.Row, "FullName"), String.Empty);

                        itemName = Functions.GetFormattedUserName(userName, fullName, IsLiveSite);
                    }
                    else if (displayNameFormat == null)
                    {
                        itemName = drv[iObjectType.DisplayNameColumn].ToString();
                    }
                    else
                    {
                        MacroResolver resolver = MacroResolver.GetInstance();
                        foreach (DataColumn item in drv.Row.Table.Columns)
                        {
                            resolver.SetNamedSourceData(item.ColumnName, drv.Row[item.ColumnName]);
                        }
                        itemName = resolver.ResolveMacros(displayNameFormat);
                    }

                    if (RemoveMultipleCommas)
                    {
                        itemName = TextHelper.RemoveMultipleCommas(itemName);
                    }

                    // Add the prefixes
                    itemName = ItemPrefix + itemName;
                    itemID = ItemPrefix + itemID;

                    var ti = iObjectType.TypeInfo;

                    // Add global object name prefix if required
                    if (AddGlobalObjectNamePrefix && !String.IsNullOrEmpty(ti.SiteIDColumn) && (ValidationHelper.GetInteger(DataHelper.GetDataRowValue(drv.Row, ti.SiteIDColumn), 0) == 0))
                    {
                        itemID = "." + itemID;
                    }

                    if (String.IsNullOrEmpty(itemName))
                    {
                        itemName = emptyReplacement;
                    }

                    if (AddGlobalObjectSuffix)
                    {
                        if ((iObjectType != null) && !string.IsNullOrEmpty(ti.SiteIDColumn))
                        {
                            itemName += (ValidationHelper.GetInteger(DataHelper.GetDataRowValue(drv.Row, ti.SiteIDColumn), 0) > 0 ? string.Empty : " " + GlobalObjectSuffix);
                        }
                    }

                    // Link action
                    string onclick = null;
                    bool disabled = disabledItems.Contains(";" + itemID + ";");
                    if (!disabled)
                    {
                        string safeItemID = GetSafe(itemID);
                        string itemHash = ValidationHelper.GetHashString(itemID);
                        switch (selectionMode)
                        {
                            case SelectionModeEnum.Multiple:
                            case SelectionModeEnum.MultipleTextBox:
                            case SelectionModeEnum.MultipleButton:
                                onclick = string.Format("ProcessItem(document.getElementById('chk{0}'),'{1}',true,true); return false;", ScriptHelper.GetString(itemID).Trim('\''), hashItems[hashKey]);
                                break;

                            case SelectionModeEnum.SingleButton:
                                onclick = string.Format("SelectItems({0},'{1}'); return false;", safeItemID, itemHash);
                                break;

                            case SelectionModeEnum.SingleTextBox:
                                if (allowEditTextBox)
                                {
                                    onclick = string.Format("SelectItems({0},{0},{1},{2},{3},'{4}'); return false;", safeItemID, ScriptHelper.GetString(hdnClientId), ScriptHelper.GetString(txtClientId), ScriptHelper.GetString(hashId), itemHash);
                                }
                                else
                                {
                                    onclick = string.Format("SelectItems({0},{1},{2},{3},{4},'{5}'); return false;", safeItemID, GetSafe(itemName), ScriptHelper.GetString(hdnClientId), ScriptHelper.GetString(txtClientId), ScriptHelper.GetString(hashId), itemHash);
                                }
                                break;

                            default:
                                onclick = string.Format("SelectItemsReload({0},{1},{2},{3},{4},{5},'{6}'); return false;", safeItemID, GetSafe(itemName), ScriptHelper.GetString(hdnClientId), ScriptHelper.GetString(txtClientId), ScriptHelper.GetString(hdnDrpId), ScriptHelper.GetString(hashId), itemHash);
                                break;
                        }

                        onclick = "onclick=\"" + onclick + "\" ";
                    }

                    if (LocalizeItems)
                    {
                        itemName = ResHelper.LocalizeString(itemName);
                    }

                    return "<div " + (!disabled ? "class=\"SelectableItem\" " : null) + onclick + ">" + HTMLHelper.HTMLEncode(TextHelper.LimitLength(itemName, 100)) + "</div>";
                }
        }

        return null;
    }


    protected void uniGrid_OnPageChanged(object sender, EventArgs e)
    {
        // Load the grid data
        ReloadGrid();
    }


    /// <summary>
    /// Button search event handler.
    /// </summary>
    private void btnSearch_Click(object sender, EventArgs e)
    {
        ChangeSearchCondition();
    }


    /// <summary>
    /// On search condition changed.
    /// </summary>
    private void searchControl_OnFilterChanged()
    {
        ChangeSearchCondition();
    }


    protected void lnkSelectAll_Click(object sender, EventArgs e)
    {
        if (iObjectType != null)
        {
            // Get all values
            DataSet ds = GetData(returnColumnName);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                string[] values = hidItem.Value.Split(new[] { valuesSeparator }, StringSplitOptions.RemoveEmptyEntries);

                // Build hashtable of current values
                Hashtable ht = new Hashtable(values.Length);
                foreach (string value in values)
                {
                    ht[value] = true;
                }

                DataTable dt = ds.Tables[0];

                // Process all items
                foreach (DataRow dr in dt.Rows)
                {
                    string add = dr[returnColumnName].ToString();

                    if (!ht.Contains(add))
                    {
                        ht.Add(add, true);
                    }
                }

                // Build the selected values string
                StringBuilder sb = new StringBuilder();
                sb.Append(valuesSeparator);

                foreach (string value in ht.Keys)
                {
                    sb.Append(value, valuesSeparator);
                }

                hidItem.Value = sb.ToString();
                hidHash.Value = ValidationHelper.GetHashString(hidItem.Value);
            }

            pnlHidden.Update();
        }
    }


    protected void lnkDeselectAll_Click(object sender, EventArgs e)
    {
        DataSet ds = GetData(returnColumnName);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            string[] values = hidItem.Value.Split(new[] { valuesSeparator }, StringSplitOptions.RemoveEmptyEntries);

            // Build hashtable of current values
            Hashtable ht = new Hashtable(values.Length);
            foreach (string value in values)
            {
                ht[value] = true;
            }

            // Remove the values from hashtable
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string remove = dr[returnColumnName].ToString();

                if (ht.Contains(remove))
                {
                    ht.Remove(remove);
                }
            }

            // Build the selected values string
            StringBuilder sb = new StringBuilder();
            sb.Append(valuesSeparator);

            foreach (string key in ht.Keys)
            {
                sb.Append(key, valuesSeparator);
            }

            hidItem.Value = sb.ToString();
            hidHash.Value = ValidationHelper.GetHashString(hidItem.Value);
        }

        // Remove the selection from hidden fields
        pnlHidden.Update();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Overridden to get the parameters.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    public override object GetValue(string propertyName)
    {
        if ((parameters != null) && parameters.Contains(propertyName))
        {
            return parameters[propertyName];
        }

        return base.GetValue(propertyName);
    }


    /// <summary>
    /// Overridden set value to collect parameters.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <param name="value">Value</param>
    public override bool SetValue(string propertyName, object value)
    {
        // Handle special properties
        switch (propertyName.ToLowerCSafe())
        {
            case "itemprefix":
                ItemPrefix = ValidationHelper.GetString(value, string.Empty);
                break;
        }

        base.SetValue(propertyName, value);

        // Set parameters for dialog
        parameters[propertyName] = value;

        return true;
    }


    /// <summary>
    /// Loads dynamically custom filter if is defined.
    /// </summary>
    private void LoadFilter()
    {
        // Use user filter
        if (!String.IsNullOrEmpty(filterControl))
        {
            pnlFilter.Controls.Clear();

            searchControl = (CMSAbstractBaseFilterControl)LoadUserControl(filterControl);
            if (searchControl != null)
            {
                searchControl.Parameters = parameters;
                searchControl.FilteredControl = this;
                searchControl.OnFilterChanged += searchControl_OnFilterChanged;
                searchControl.ID = "filterElem";
                searchControl.SelectedValue = hidItem.Value.Replace(valuesSeparator, string.Empty);
                searchControl.FilterMode = filterMode;

                pnlFilter.Controls.Add(searchControl);
                pnlFilter.Visible = true;

                // Get init filter where condition
                FilterWhere = SqlHelper.AddWhereCondition(string.Empty, searchControl.WhereCondition);

                // When both filters are rendered, mark the first as followed by another
                if (useDefaultNameFilter)
                {
                    pnlFilter.CssClass += " header-panel-not-last";
                }
            }
        }
    }


    /// <summary>
    /// Loads control parameters.
    /// </summary>
    private void LoadParameters()
    {
        string identifier = QueryHelper.GetString("params", null);

        parameters = (Hashtable)WindowHelper.GetItem(identifier);
        if (parameters != null)
        {
            // Load values from session
            selectionMode = (SelectionModeEnum)parameters["SelectionMode"];
            resourcePrefix = ValidationHelper.GetString(parameters["ResourcePrefix"], "general");
            objectType = ValidationHelper.GetString(parameters["ObjectType"], null);
            returnColumnName = ValidationHelper.GetString(parameters["ReturnColumnName"], null);
            valuesSeparator = ValidationHelper.GetString(parameters["ValuesSeparator"], ";");
            filterControl = ValidationHelper.GetString(parameters["FilterControl"], null);
            useDefaultNameFilter = ValidationHelper.GetBoolean(parameters["UseDefaultNameFilter"], true);
            whereCondition = ValidationHelper.GetString(parameters["WhereCondition"], null);
            orderBy = ValidationHelper.GetString(parameters["OrderBy"], null);
            itemsPerPage = ValidationHelper.GetInteger(parameters["ItemsPerPage"], 10);
            emptyReplacement = ValidationHelper.GetString(parameters["EmptyReplacement"], "&nbsp;");
            dialogGridName = ValidationHelper.GetString(parameters["DialogGridName"], dialogGridName);
            additionalColumns = ValidationHelper.GetString(parameters["AdditionalColumns"], null);
            callbackMethod = ValidationHelper.GetString(parameters["CallbackMethod"], null);
            allowEditTextBox = ValidationHelper.GetBoolean(parameters["AllowEditTextBox"], false);
            fireOnChanged = ValidationHelper.GetBoolean(parameters["FireOnChanged"], false);
            disabledItems = ";" + ValidationHelper.GetString(parameters["DisabledItems"], String.Empty) + ";";
            GlobalObjectSuffix = ValidationHelper.GetString(parameters["GlobalObjectSuffix"], string.Empty);
            AddGlobalObjectSuffix = ValidationHelper.GetBoolean(parameters["AddGlobalObjectSuffix"], false);
            AddGlobalObjectNamePrefix = ValidationHelper.GetBoolean(parameters["AddGlobalObjectNamePrefix"], false);
            RemoveMultipleCommas = ValidationHelper.GetBoolean(parameters["RemoveMultipleCommas"], false);
            filterMode = ValidationHelper.GetString(parameters["FilterMode"], null);
            displayNameFormat = ValidationHelper.GetString(parameters["DisplayNameFormat"], null);
            additionalSearchColumns = ValidationHelper.GetString(parameters["AdditionalSearchColumns"], String.Empty);
            siteWhereCondition = ValidationHelper.GetString(parameters["SiteWhereCondition"], null);
            UseTypeCondition = ValidationHelper.GetBoolean(parameters["UseTypeCondition"], true);

            // Set item prefix if it was passed by UniSelector's AdditionalUrlParameters
            var itemPrefix = QueryHelper.GetString("ItemPrefix", null);
            if (!String.IsNullOrEmpty(itemPrefix))
            {
                ItemPrefix = itemPrefix;
            }

            // Pre-select unigrid values passed from parent window
            if (!RequestHelper.IsPostBack())
            {
                string values = (string)parameters["Values"];
                if (!String.IsNullOrEmpty(values))
                {
                    hidItem.Value = values;
                    hidHash.Value = ValidationHelper.GetHashString(hidItem.Value);
                    parameters["Values"] = null;
                }
            }
        }
    }


    /// <summary>
    /// Loads variables and objects.
    /// </summary>
    private void LoadControls()
    {
        // Load objects
        if (!String.IsNullOrEmpty(objectType))
        {
            iObjectType = ModuleManager.GetReadOnlyObject(objectType);
            if (iObjectType == null)
            {
                throw new Exception("[UniSelector.SelectionDialog]: Object type '" + objectType + "' not found.");
            }

            if (returnColumnName == null)
            {
                returnColumnName = iObjectType.TypeInfo.IDColumn;
            }
        }

        mSearchColumns = GetSearchColumns();
        // Display default name filter only if search columns are specified
        if (useDefaultNameFilter && (!String.IsNullOrEmpty(mSearchColumns) || !String.IsNullOrEmpty(additionalSearchColumns) || (displayNameFormat == UniSelector.USER_DISPLAY_FORMAT)))
        {
            lblSearch.ResourceString = resourcePrefix + ".entersearch|general.entersearch";
            btnSearch.ResourceString = "general.search";

            pnlSearch.Visible = true;

            if (!URLHelper.IsPostback())
            {
                ScriptHelper.RegisterStartupScript(this, typeof(string), "Focus", ScriptHelper.GetScript("try{document.getElementById('" + txtSearch.ClientID + "').focus();}catch(err){}"));
            }
        }

        if (!URLHelper.IsPostback())
        {
            uniGrid.Pager.DefaultPageSize = itemsPerPage;
        }

        uniGrid.GridName = dialogGridName;
        uniGrid.GridView.EnableViewState = false;

        // Show the OK button if needed
        switch (selectionMode)
        {
            case SelectionModeEnum.Multiple:
            case SelectionModeEnum.MultipleTextBox:
            case SelectionModeEnum.MultipleButton:
                {
                    pnlAll.Visible = true;

                    lnkSelectAll.Text = GetString("UniSelector.SelectAll");
                    lnkDeselectAll.Text = GetString("UniSelector.DeselectAll");
                }
                break;
        }
    }


    /// <summary>
    /// Returns dataset for specified GeneralizedInfo.
    /// </summary>
    /// <param name="returnColumn">Return column</param>
    private DataSet GetData(string returnColumn)
    {
        int totalRecords = 0;
        return GetData(returnColumn, 0, 0, ref totalRecords, true);
    }


    /// <summary>
    /// Returns dataset for specified GeneralizedInfo.
    /// </summary>
    private DataSet GetData(string returnColumn, int offset, int maxRecords, ref int totalRecords, bool selection)
    {
        // If object type is set
        if (iObjectType != null)
        {
            // Init columns
            string columns = null;
            DataSet ds = null;

            if (!selection)
            {
                if (displayNameFormat == UniSelector.USER_DISPLAY_FORMAT)
                {
                    // Ensure columns which are needed for USER_DISPLAY_FORMAT
                    columns = "UserName, FullName";
                }
                else if (displayNameFormat != null)
                {
                    columns = DataHelper.GetNotEmpty(MacroProcessor.GetMacros(displayNameFormat), iObjectType.DisplayNameColumn).Replace(";", ", ");
                }
                else
                {
                    columns = iObjectType.DisplayNameColumn;
                }
            }

            // Add return column name
            columns = SqlHelper.MergeColumns(columns, returnColumn);

            // Add additional columns
            columns = SqlHelper.MergeColumns(columns, additionalColumns);

            var ti = iObjectType.TypeInfo;

            // Get SiteID column if needed
            if (AddGlobalObjectSuffix && !string.IsNullOrEmpty(ti.SiteIDColumn))
            {
                columns = SqlHelper.MergeColumns(columns, ti.SiteIDColumn);
            }

            string where = SqlHelper.AddWhereCondition(whereCondition, FilterWhere);
            if (!String.IsNullOrEmpty(uniGrid.WhereClause))
            {
                where = SqlHelper.AddWhereCondition(where, uniGrid.WhereClause);
            }

            // Apply site restrictions
            if (!string.IsNullOrEmpty(siteWhereCondition))
            {
                where = SqlHelper.AddWhereCondition(where, siteWhereCondition);
            }

            string order = String.Empty;

            // Order by
            if (String.IsNullOrEmpty(orderBy))
            {
                order += iObjectType.DisplayNameColumn;
            }
            else
            {
                order += orderBy;
            }

            try
            {
                // Get the data query
                var q = iObjectType.GetDataQuery(
                    UseTypeCondition,
                    s => s
                        .Where(where)
                        .OrderBy(order)
                        .Columns(columns),
                    true
                );

                q.IncludeBinaryData = false;
                q.Offset = offset;
                q.MaxRecords = maxRecords;

                // Get the data
                ds = q.Result;
                totalRecords = q.TotalRecords;
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("UniSelector", "GETDATA", ex);

                uniGrid.ShowError(ex.Message);
            }

            return ds;
        }
        else
        {
            totalRecords = 0;
            return null;
        }
    }


    /// <summary>
    /// Changes ViewState with search condition for UniGrid.
    /// </summary>
    private void ChangeSearchCondition()
    {
        if (iObjectType != null)
        {
            string where = null;

            // Get default filter where
            if ((useDefaultNameFilter) && (txtSearch.Text != String.Empty))
            {
                // Trim entered text
                string searchText = txtSearch.Text.Trim();
                // Avoid SQL injection
                searchText = SqlHelper.EscapeQuotes(searchText);
                // Escape like patterns
                searchText = SqlHelper.EscapeLikeText(searchText);
                
                if (displayNameFormat == UniSelector.USER_DISPLAY_FORMAT)
                {
                    // Ensure search in columns which are needed for USER_DISPLAY_FORMAT
                    where = String.Format("UserName LIKE N'%{0}%' OR FullName LIKE N'%{0}%'", searchText);
                }

                if (!String.IsNullOrEmpty(mSearchColumns))
                {
                    // Combine main search columns with additional 
                    additionalSearchColumns = additionalSearchColumns.TrimEnd(';') + ";" + mSearchColumns;
                }

                // Append additional columns that should be used for search
                if (!string.IsNullOrEmpty(additionalSearchColumns))
                {
                    string[] columns = additionalSearchColumns.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string column in columns)
                    {
                        where = SqlHelper.AddWhereCondition(where, string.Format("{0} LIKE N'%{1}%'", column.Trim(), searchText), "OR");
                    }
                }
            }

            // Get search filter where
            if (searchControl != null)
            {
                where = SqlHelper.AddWhereCondition(where, searchControl.WhereCondition);
            }

            // Save where condition to the view state
            FilterWhere = where;
        }
    }


    /// <summary>
    /// Returns main search columns to filter.
    /// </summary>
    private string GetSearchColumns()
    {
        var ti = iObjectType.TypeInfo;

        if ((ti.DisplayNameColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN))
        {
            // Get column for display name 
            return ti.DisplayNameColumn;
        }
        
        if ((ti.CodeNameColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN))
        {
            // Get column for code name
            return ti.CodeNameColumn;
        }
        
        if (!String.IsNullOrEmpty(displayNameFormat))
        {
            // Get columns from display name format if empty don't filter anything
            return MacroProcessor.GetMacros(displayNameFormat);
        }

        return String.Empty;
    }


    /// <summary>
    /// Reloads the grid with given page index.
    /// </summary>
    protected void ReloadGrid()
    {
        int totalRecords = 0;
        int offset = uniGrid.Pager.CurrentPageSize * (uniGrid.Pager.CurrentPage - 1);

        // Reload data set with new page index
        if (uniGrid.DataSource == null)
        {
            uniGrid.DataSource = GetData(returnColumnName, offset, uniGrid.Pager.CurrentPageSize, ref totalRecords, false);
            uniGrid.PagerForceNumberOfResults = totalRecords;
        }

        uniGrid.ZeroRowsText = !string.IsNullOrEmpty(FilterWhere) ? GetString(resourcePrefix + ".noitemsfound|general.noitemsfound") : GetString(resourcePrefix + ".nodatafound|general.nodatafound");
        uniGrid.ReloadData();
    }


    /// <summary>
    /// Returns string safe for inserting to javascript as parameter.
    /// </summary>
    /// <param name="param">Parameter</param>    
    private string GetSafe(string param)
    {
        // Replace + char for %20 to make it compatible with client side decodeURIComponent
        return ScriptHelper.GetString(Server.UrlEncode(param).Replace("+", "%20"));
    }

    #endregion


    #region "ICallbackEventHandler Members"

    string ICallbackEventHandler.GetCallbackResult()
    {
        // Prepare the parameters for dialog
        string result = string.Empty;
        if (!string.IsNullOrEmpty(callbackValues))
        {
            // All selected items | newly added item(s) # hash of the new item(s)
            var paramFormat = new Regex(String.Format(@"^(?<items>{0}(.*{0})?)\|(?<values>({0}.*{0})|([^{0}]*))#(?<hash>.*)$", Regex.Escape(valuesSeparator)));
            var paramMatch = paramFormat.Match(callbackValues);
            if (paramMatch.Success)
            {
                string value = paramMatch.Groups["values"].Value.Trim(valuesSeparator.ToCharArray());
                string hash = paramMatch.Groups["hash"].Value;

                if (ValidationHelper.ValidateHash(value, hash, new HashSettings { Redirect = false }))
                {
                    // Get new hash for currently selected items
                    result = ValidationHelper.GetHashString(paramMatch.Groups["items"].Value);
                }
            }
        }

        return result;
    }


    void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
    {
        callbackValues = eventArgument;
    }

    #endregion
}