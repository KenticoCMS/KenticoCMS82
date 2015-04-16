using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;

using CMS.Controls;
using CMS.EventLog;
using CMS.Helpers;
using CMS.Base;
using CMS.SharePoint.Imaging;
using CMS.SharePoint.Lists;


public partial class CMSWebParts_SharePoint_SharePointDataSource_files_SharePointDatasource : CMSBaseDataSource
{
    #region "Public constants"

    // SharePoint modes
    public const string MODE_LIST_ITEMS = "listitems";
    public const string MODE_SITE_LISTS = "sitelists";
    public const string MODE_PICTURE_LIBRARIES = "piclibs";
    public const string MODE_PICTURE_LIBRARY_ITEMS = "piclibitems";

    #endregion


    #region "Private variables"

    private bool useBase64Encoding = false;
    private bool retrieved = false;
    private XmlNode camlData = null;
    private object dataSource = null;
    private bool? mHasData = null;

    // Properties
    private string mUsername;
    private string mPassword;
    private string mListName;
    private string mSPServiceURL;
    private bool mShowReturnedCAML;
    private int mRowLimit;
    private string mQuery;
    private string mViewFields;
    private bool mUseClassicDataset = true;
    private string mFields;
    private string mMode = MODE_LIST_ITEMS;
    private string mItemIDField;
    private string mSelectedItemFieldName = null;
    private string mItemIDFieldType = "Text";
    private bool? mIsSelected = null;
    private string mErrorMessage = null;

    private static Regex mSpecialCharsRegex = null;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Regex used for replacing special characters from Sharepoint xml.
    /// </summary>
    private static Regex SpecialCharsRegex
    {
        get
        {
            if (mSpecialCharsRegex == null)
            {
                mSpecialCharsRegex = RegexHelper.GetRegex("(_x00[0-9a-z]{2}_)", true);
            }

            return mSpecialCharsRegex;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets Datasource object.
    /// </summary>
    public override object DataSource
    {
        get
        {
            object datasource = GetDataSource();

            // If GetDataSource didn't cause retrieval, it was loaded from cache
            if (!retrieved)
            {
                if (datasource != null)
                {
                    ShowRawResponse(null);

                    mHasData = true;
                }
                // If null is in cache, try retrieve, could be error
                else
                {
                    datasource = GetDataSourceFromDB();

                    // New retrieval successful
                    if (datasource != null)
                    {
                        // Clear old cache item
                        ClearCache();

                        // Causes new dataset to be cached
                        datasource = GetDataSource();
                    }
                }
            }

            return datasource;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether current datasource contains  selected item.
    /// </summary>
    public override bool IsSelected
    {
        get
        {
            if (!mIsSelected.HasValue)
            {
                mIsSelected = ((Mode == MODE_LIST_ITEMS) && (QueryHelper.GetText(ItemIDField, null) != null));
            }

            return mIsSelected.Value;
        }
        set
        {
            mIsSelected = value;
        }
    }


    /// <summary>
    /// Gets or sets value of username.
    /// </summary>
    public string Username
    {
        get
        {
            return mUsername;
        }
        set
        {
            mUsername = value;
        }
    }


    /// <summary>
    /// Gets or sets value of password.
    /// </summary>
    public string Password
    {
        get
        {
            return mPassword;
        }
        set
        {
            mPassword = value;
        }
    }


    /// <summary>
    /// Gets or sets value of SharePoint list name.
    /// </summary>
    public string ListName
    {
        get
        {
            return mListName;
        }
        set
        {
            mListName = value;
        }
    }


    /// <summary>
    /// Gets or sets URL of SharePoint service (Eg. Lists.asmx, Imaging.asmx).
    /// </summary>
    public string SPServiceURL
    {
        get
        {
            return mSPServiceURL;
        }
        set
        {
            mSPServiceURL = value;
        }
    }


    /// <summary>
    /// Enables or disables showing CAML on output.
    /// </summary>
    public bool ShowReturnedCAML
    {
        get
        {
            return mShowReturnedCAML;
        }
        set
        {
            mShowReturnedCAML = value;
        }
    }


    /// <summary>
    /// Gets or set the row limit.
    /// </summary>
    public int RowLimit
    {
        get
        {
            return mRowLimit;
        }
        set
        {
            mRowLimit = value;
        }
    }


    /// <summary>
    /// Gets or sets query to specify which document should be retrieved (like where condition).
    /// </summary>
    public string Query
    {
        get
        {
            return mQuery;
        }
        set
        {
            mQuery = value;
        }
    }


    /// <summary>
    /// Gets or sets document fields which should be retrieved.
    /// </summary>
    public string ViewFields
    {
        get
        {
            return mViewFields;
        }
        set
        {
            mViewFields = value;
        }
    }


    /// <summary>
    /// Enables or disables using of classic dataset as data source for ASCX transformation.
    /// </summary>
    public bool UseClassicDataset
    {
        get
        {
            return mUseClassicDataset;
        }
        set
        {
            mUseClassicDataset = value;
        }
    }


    /// <summary>
    /// Gets or sets fields which should be included in dataset
    /// Note: Only if UseClassicDataset is enabled
    /// </summary>
    public string Fields
    {
        get
        {
            return mFields;
        }
        set
        {
            mFields = value;
        }
    }


    /// <summary>
    /// Gets or sets the mode which specifies what this webpart exactly do.
    /// </summary>
    public string Mode
    {
        get
        {
            return mMode;
        }
        set
        {
            mMode = value;
        }
    }


    /// <summary>
    /// Gets or sets query string key name. Presence of the key in query string indicates, 
    /// that some item should be selected. The item is determined by query string value.        
    /// </summary>
    public string ItemIDField
    {
        get
        {
            return mItemIDField;
        }
        set
        {
            mItemIDField = value;
        }
    }


    /// <summary>
    /// Gets or sets the field name which is used for selecting item. Case sensitive!
    /// </summary>
    public string SelectedItemFieldName
    {
        get
        {
            return mSelectedItemFieldName;
        }
        set
        {
            mSelectedItemFieldName = value;
        }
    }


    /// <summary>
    /// Gets or sets the type of field for selecting item.
    /// </summary>
    public string ItemIDFieldType
    {
        get
        {
            return mItemIDFieldType;
        }
        set
        {
            mItemIDFieldType = value;
        }
    }


    /// <summary>
    /// Indicates whether datasource has some data = is not empty.
    /// </summary>
    public bool HasData
    {
        get
        {
            if (!mHasData.HasValue)
            {
                // Get data records - value of mHasData is set
                GetDataRecords(camlData);
            }

            return mHasData.Value;
        }
    }


    /// <summary>
    /// Returns error message.
    /// </summary>
    public string ErrorMessage
    {
        get
        {
            return mErrorMessage;
        }
    }

    #endregion


    #region "Data source"

    /// <summary>
    /// Gets data from SharePoint.
    /// </summary>
    /// <returns>Dataset or XmlNode as object</returns>
    protected override object GetDataSourceFromDB()
    {
        // Mark loading was tried
        retrieved = true;

        object data = null;

        try
        {
            // Check if URL address of web service is valid
            if (!ValidationHelper.IsURL(SPServiceURL))
            {
                DisplayError(ResHelper.GetString("SharePoint.invalidURL"));
                return null;
            }

            // Get SharePoint data and set datasource
            data = GetSharePointData();
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("SharePointDataSource", "GetSharePointData", ex);
        }

        return data;
    }

    #endregion


    #region "SharePoint methods"

    /// <summary>
    /// Retrieves data from SharePoint server using web services.
    /// </summary>
    /// <returns>Dataset or XmlNode</returns>
    protected object GetSharePointData()
    {
        #region "Prepare credentials for authentication"

        ICredentials credentials = null;

        // If there are data in username or password use it for authentication
        if (String.IsNullOrEmpty(Username) && String.IsNullOrEmpty(Password))
        {
            credentials = SharePointFunctions.GetSharePointCredetials();
        }
        else
        {
            credentials = SharePointFunctions.GetSharePointCredetials(Username, Password, useBase64Encoding);
        }

        #endregion


        #region "Retrieve SharePoint data"

        string serviceURL = SPServiceURL;

        // If not direct web service specified, determine web service URL by mode
        if (!serviceURL.EndsWithCSafe(".asmx", true))
        {
            // Remove trailing slash
            serviceURL = serviceURL.TrimEnd('/');

            switch (Mode.ToLowerCSafe())
            {
                case MODE_LIST_ITEMS:
                case MODE_SITE_LISTS:
                    serviceURL += "/_vti_bin/lists.asmx";
                    break;

                case MODE_PICTURE_LIBRARIES:
                case MODE_PICTURE_LIBRARY_ITEMS:
                    serviceURL += "/_vti_bin/imaging.asmx";
                    break;
            }
        }

        // Query web service
        try
        {
            Lists spLists;
            Imaging spImaging;

            switch (Mode.ToLowerCSafe())
            {
                    // Load list items
                case MODE_LIST_ITEMS:

                    // Instantiate Lists service
                    spLists = new Lists(serviceURL);
                    spLists.Credentials = credentials;

                    camlData = LoadListItems(spLists, ListName);
                    break;

                    // Load site lists
                case MODE_SITE_LISTS:
                    // Instantiate Lists service
                    spLists = new Lists(serviceURL);
                    spLists.Credentials = credentials;

                    // Get all SharePoint lists
                    camlData = spLists.GetListCollection();
                    break;

                    // Load picture libraries
                case MODE_PICTURE_LIBRARIES:
                    // Instantiate imaging service
                    spImaging = new Imaging(serviceURL);
                    spImaging.Credentials = credentials;

                    // Get picture libraries
                    camlData = spImaging.ListPictureLibrary();
                    break;

                    // Load picture library items
                case MODE_PICTURE_LIBRARY_ITEMS:
                    // Instantiate imaging service
                    spImaging = new Imaging(serviceURL);
                    spImaging.Credentials = credentials;

                    // Show error if library name empty
                    if (String.IsNullOrEmpty(ListName))
                    {
                        DisplayError(ResHelper.GetString("SharePoint.picslibunspecified"));
                        break;
                    }

                    // Get pictures in libraries, directly (not in folder)
                    camlData = spImaging.GetListItems(ListName, null);
                    break;
            }
        }
        catch (Exception ex)
        {
            DisplayError(ResHelper.GetString("sharepoint.erroradta") + ResHelper.Colon + " " + ex.Message);
        }


        // No data
        if (camlData == null)
        {
            return null;
        }

        #endregion


        #region "Prepare data"

        // Use of XSLT transformation
        if (!UseClassicDataset)
        {
            dataSource = camlData;
        }
        // Use of classic dataset
        else
        {
            // Prepare dataset
            DataSet ds = new DataSet();

            // If datset fields are specified
            if (!String.IsNullOrEmpty(Fields))
            {
                DataTable dt = ds.Tables.Add();

                string[] fields = Fields.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string field in fields)
                {
                    string currentField = field.Trim();
                    if (!string.IsNullOrEmpty(currentField))
                    {
                        try
                        {
                            dt.Columns.Add(field.Trim());
                        }
                        catch (DuplicateNameException)
                        {
                            mErrorMessage = ResHelper.GetString("sharepoint.duplicateField");
                        }
                    }
                }

                XmlNodeList records = GetDataRecords(camlData);

                List<string> fieldsCheck = new List<string>();
                fieldsCheck.AddRange(fields);

                // Load items to dataset
                foreach (XmlNode record in records)
                {
                    DataRow dr = dt.NewRow();

                    // Add fields
                    foreach (string field in fields)
                    {
                        string currentField = field.Trim();
                        if (!string.IsNullOrEmpty(currentField))
                        {
                            XmlAttribute attr = record.Attributes[currentField];
                            if (attr != null)
                            {
                                dr[currentField] = attr.Value;

                                // At least one record has the field
                                fieldsCheck.Remove(field);
                            }
                        }
                    }

                    dt.Rows.Add(dr);
                }

                // None of retrieved records has fields
                if (fieldsCheck.Count > 0)
                {
                    DisplayError(String.Format(ResHelper.GetString("sharepoint.fieldnotFound"), fieldsCheck[0]));
                    return null;
                }

                // Set daatsource
                dataSource = ds;
            }
            // No fields specified, use all fields
            else
            {
                // Only if CAML contains data record, otherwise dataset would contain wrong values
                if (HasData)
                {
                    camlData.InnerXml = SpecialCharsRegex.Replace(camlData.InnerXml, "_");
                    XmlNodeReader rd = new XmlNodeReader(camlData);
                    ds.ReadXml(rd);
                    rd.Close();

                    switch (Mode.ToLowerCSafe())
                    {
                            // Use last datatable as datasource
                        case MODE_LIST_ITEMS:

                            dataSource = ds.Tables[ds.Tables.Count - 1].DefaultView;
                            break;

                            // Filter hidden lists in dataset
                        case MODE_SITE_LISTS:
                            // Dataset structure changes based on xml, try to use last data table
                            DataTable dt = ds.Tables[ds.Tables.Count - 1];

                            // Show only visible lists
                            dt.DefaultView.RowFilter = "Hidden = 'False'";
                            dataSource = dt.DefaultView;
                            break;

                            // Dateset with picture libraries
                        case MODE_PICTURE_LIBRARIES:
                            dataSource = ds.Tables[ds.Tables.Count - 1].DefaultView;
                            break;

                            // Dataset with pictures
                        case MODE_PICTURE_LIBRARY_ITEMS:
                            dataSource = ds.Tables[ds.Tables.Count - 1].DefaultView;
                            break;
                    }
                }
            }
        }

        #endregion


        ShowRawResponse(camlData);

        return dataSource;
    }


    /// <summary>
    /// Returns XmlNodeList with data records depending on retrieve mode.
    /// </summary>
    /// <param name="camlData">SharePoint data</param>    
    protected XmlNodeList GetDataRecords(XmlNode camlData)
    {
        XmlNodeList records = null;

        if (camlData != null)
        {
            // Get items from CAML
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(camlData.OwnerDocument.NameTable);
            nsmgr.AddNamespace("soap", "http://schemas.microsoft.com/sharepoint/soap/");

            switch (Mode.ToLowerCSafe())
            {
                    // Get records of list items
                case MODE_LIST_ITEMS:
                    nsmgr.AddNamespace("rs", "urn:schemas-microsoft-com:rowset");
                    nsmgr.AddNamespace("z", "#RowsetSchema");
                    records = camlData.SelectNodes("/rs:data/z:row", nsmgr);
                    break;

                    // Get records of site lists, do not select hidden lists
                case MODE_SITE_LISTS:
                    records = camlData.SelectNodes("/soap:List[@Hidden = 'False']", nsmgr);
                    break;

                    // Get records of picture libraries
                case MODE_PICTURE_LIBRARIES:
                    // Define whatever prefix for default xml namespace
                    nsmgr.AddNamespace("lib", "http://schemas.microsoft.com/sharepoint/soap/ois/");
                    records = camlData.SelectNodes("/lib:Library", nsmgr);
                    break;

                    // Get records of pictures in library
                case MODE_PICTURE_LIBRARY_ITEMS:
                    nsmgr.AddNamespace("rs", "urn:schemas-microsoft-com:rowset");
                    nsmgr.AddNamespace("z", "#RowsetSchema");
                    records = camlData.SelectNodes("/z:row", nsmgr);
                    break;
            }
        }

        // Set has data flag
        mHasData = ((records != null) && (records.Count > 0));

        return records;
    }


    /// <summary>
    /// Queries SharePoint for list items.
    /// </summary>    
    /// <param name="listService">List service proxy class instance</param>
    /// <param name="listName">List name</param>
    /// <returns>CAML with list items</returns>
    protected XmlNode LoadListItems(Lists listService, string listName)
    {
        // Get all SharePoint lists
        XmlNode listCollectionCaml = listService.GetListCollection();

        XmlNamespaceManager nsmgr = new XmlNamespaceManager(listCollectionCaml.OwnerDocument.NameTable);
        nsmgr.AddNamespace("soap", "http://schemas.microsoft.com/sharepoint/soap/");

        // Check list with given name exists
        XmlNodeList collections = listCollectionCaml.SelectNodes("/soap:List[@Title = '" + listName + "']", nsmgr);

        // If specified list does not exists show error
        if (collections.Count == 0)
        {
            DisplayError(ResHelper.GetString("SharePoint.listdoesnotexists"));
            return null;
        }


        #region "Advanced options"

        //XmlElement query = null;
        XmlDocument viewFields = null;
        XmlDocument query = null;
        XmlDocument queryOptions = null;

        // Specify search by query
        if (!String.IsNullOrEmpty(Query))
        {
            //query = xmlOptions.CreateElement("Query");
            //query.InnerXml = this.Query; //"<Where><Gt><FieldRef Name=\"ID\" /><Value Type=\"Counter\">0</Value></Gt></Where>";

            query = new XmlDocument();
            query.LoadXml(Query);
        }

        // Select item 
        if (!String.IsNullOrEmpty(ItemIDField))
        {
            string queryValue = QueryHelper.GetText(ItemIDField, null);

            if (queryValue != null)
            {
                IsSelected = true;
                // Set field name from ItemIDField if not set
                string fieldName = (String.IsNullOrEmpty(SelectedItemFieldName)) ? ItemIDField : SelectedItemFieldName;

                // No query was set
                if (query == null)
                {
                    query = new XmlDocument();
                }

                // Get or create Query node
                XmlNode queryNode = query.SelectSingleNode("Query");
                if (queryNode == null)
                {
                    queryNode = query.CreateNode(XmlNodeType.Element, "Query", null);
                    query.AppendChild(queryNode);
                }

                // Get or create Where node
                XmlNode whereNode = queryNode.SelectSingleNode("Where");
                if (whereNode == null)
                {
                    whereNode = query.CreateNode(XmlNodeType.Element, "Where", null);
                    queryNode.AppendChild(whereNode);
                }

                // Prepare query for selecting item
                string selectItem = String.Format("<Eq><FieldRef ID='{0}' /><Value Type='{1}'>{2}</Value></Eq>", fieldName, ItemIDFieldType, queryValue);

                // Incorporate with original query
                if (!String.IsNullOrEmpty(whereNode.InnerXml))
                {
                    selectItem = String.Format("<And>{0}{1}</And>", whereNode.InnerXml, selectItem);
                }

                // Update Where node
                whereNode.InnerXml = selectItem;
            }
        }

        // Specify which fields should be retrieved
        if (!String.IsNullOrEmpty(ViewFields))
        {
            viewFields = new XmlDocument();
            string xml = "<ViewFields>";

            string[] fields = ViewFields.Split(';');
            foreach (string field in fields)
            {
                xml += string.Format("<FieldRef Name=\"{0}\" />", field.Trim());
            }

            xml += "</ViewFields>";
            viewFields.LoadXml(xml);
        }

        //xmlDoc.CreateElement("QueryOptions");
        //queryOptions.InnerXml = "";

        // If set limit maximum count of rows
        string rowLimit = (RowLimit > 0) ? RowLimit.ToString() : null;

        #endregion


        // Get documents in the list
        XmlNode documentsXml = listService.GetListItems(ListName, null, query, viewFields, rowLimit, queryOptions, null);

        return documentsXml;
    }


    /// <summary>
    /// Shows error with message.
    /// </summary>
    /// <param name="message">Message</param>
    protected void DisplayError(string message)
    {
        Label lblError = new Label();
        lblError.Text = message;
        lblError.CssClass = "ErrorLabel";

        Controls.Add(lblError);
    }


    /// <summary>
    /// Displays raw SharePoint(CAML) response or message about loading from cache.
    /// Depends on ShowReturnedCAML property.
    /// </summary>
    /// <param name="camlData">Raw response or null</param>
    protected void ShowRawResponse(XmlNode camlData)
    {
        // If enabled, show raw CAML = SharePoint response
        if (ShowReturnedCAML)
        {
            ltlRawXml.Visible = true;
            string value;
            if (camlData != null)
            {
                value = HTMLHelper.HTMLEncode(HTMLHelper.ReformatHTML(camlData.InnerXml));
            }
            else
            {
                value = "loaded from cache";
            }

            ltlRawXml.Text = "<pre>" + value + "</pre>";
        }
    }

    #endregion
}