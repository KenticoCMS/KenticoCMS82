using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.EventLog;
using CMS.Helpers;
using CMS.IO;
using CMS.Base;

namespace CMS.SharePoint
{
    /// <summary>
    /// Output file class containing data from SharePoint.
    /// </summary>
    public class CMSOutputSharePointFile : IDataContainer
    {
        #region "Variables"

        private DateTime mInstantiated = DateTime.Now;

        private int mWidth = 0;
        private int mHeight = 0;
        private int mMaxSideSize = 0;

        private byte[] mOutputData = null;
        private string mMimeType = null;
        private bool mDataLoaded = false;

        private DateTime mValidFrom = DateTime.MinValue;
        private DateTime mValidTo = DateTime.MaxValue;

        private string mSharePointFilePath = null;
        private string mSharePointServer = null;
        private string mFileExtension = null;

        private bool mResized = false;

        #endregion


        #region "Properties"

        /// <summary>
        /// Output file data.
        /// </summary>
        public byte[] OutputData
        {
            get
            {
                return mOutputData;
            }
            set
            {
                mOutputData = value;
                mDataLoaded = true;
            }
        }


        /// <summary>
        /// Last modified date is not available for CMSOutputSharePointFile, use instantiation DateTime.
        /// </summary>
        public DateTime LastModified
        {
            get
            {
                return mInstantiated;
            }
        }


        /// <summary>
        /// Requested output width.
        /// </summary>
        public int Width
        {
            get
            {
                return mWidth;
            }
            set
            {
                mWidth = value;
            }
        }


        /// <summary>
        /// Requested output Height.
        /// </summary>
        public int Height
        {
            get
            {
                return mHeight;
            }
            set
            {
                mHeight = value;
            }
        }


        /// <summary>
        /// Requested output MaxSideSize.
        /// </summary>
        public int MaxSideSize
        {
            get
            {
                return mMaxSideSize;
            }
            set
            {
                mMaxSideSize = value;
            }
        }


        /// <summary>
        /// Returns true if the data is loaded to the object.
        /// </summary>
        public bool DataLoaded
        {
            get
            {
                return mDataLoaded;
            }
        }


        public string FileExtension
        {
            get
            {
                if ((mFileExtension == null) && (SharePointFilePath) != null)
                {
                    mFileExtension = Path.GetExtension(SharePointFilePath);
                }

                return mFileExtension;
            }
        }


        /// <summary>
        /// Mime type.
        /// </summary>
        public string MimeType
        {
            get
            {
                if ((mMimeType == null) && (SharePointFilePath != null))
                {
                    mMimeType = MimeTypeHelper.GetMimetype(FileExtension);
                }
                return mMimeType;
            }
            set
            {
                mMimeType = value;
            }
        }


        /// <summary>
        /// Time to which the file is valid.
        /// </summary>
        public DateTime ValidTo
        {
            get
            {
                return mValidTo;
            }
            set
            {
                mValidTo = value;
            }
        }


        /// <summary>
        /// Time from which the file is valid.
        /// </summary>
        public DateTime ValidFrom
        {
            get
            {
                return mValidFrom;
            }
            set
            {
                mValidFrom = value;
            }
        }


        /// <summary>
        /// Returns true if the file is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return (DateTime.Now >= ValidFrom) && (DateTime.Now <= ValidTo);
            }
        }


        /// <summary>
        /// If true, the file is resized version of the file.
        /// </summary>
        public bool Resized
        {
            get
            {
                return mResized;
            }
            set
            {
                mResized = value;
            }
        }


        /// <summary>
        /// Path to file on SharePoint server.
        /// </summary>
        public string SharePointFilePath
        {
            get
            {
                return mSharePointFilePath;
            }

            set
            {
                mSharePointFilePath = value;
            }
        }


        /// <summary>
        /// Gets or sets the address of SharePoint server.
        /// </summary>
        public string SharePointServer
        {
            get
            {
                return mSharePointServer;
            }

            set
            {
                mSharePointServer = value;
            }
        }

        #endregion


        #region "Constructors"

        /// <summary>
        /// Constructor.
        /// </summary>
        public CMSOutputSharePointFile()
        {
        }


        /// <summary>
        /// Constructor.
        /// </summary>        
        /// <param name="server">Address of the SharePoint server</param>
        /// <param name="filePath">Path to file on the server</param>
        /// <param name="data">Output file data</param>
        public CMSOutputSharePointFile(string server, string filePath, byte[] data)
        {
            SharePointServer = server;
            mSharePointFilePath = filePath;
            mOutputData = data;
            mDataLoaded = (data != null);
        }

        #endregion


        #region "Methods"

        /// <summary>
        /// Ensures that the object contains the output data.
        /// </summary>
        /// <param name="defaultData">Default data which should be loaded if data required</param>
        /// <returns>Returns true if new data has been loaded</returns>
        public bool EnsureData(byte[] defaultData)
        {
            if (!mDataLoaded)
            {
                if (defaultData != null)
                {
                    // Use default data
                    OutputData = defaultData;
                }
                else
                {
                    // Load the file data
                    if (SharePointFilePath != null)
                    {
                        LoadData();
                    }
                    else
                    {
                        OutputData = null;
                    }
                }

                mDataLoaded = true;
                return true;
            }

            return false;
        }


        /// <summary>
        /// Loads the data to the object.
        /// </summary>
        /// <param name="data">New data</param>
        /// <param name="am">Attachment manager</param>
        public void LoadData()
        {
            if (SharePointFilePath == null)
            {
                throw new Exception("[CMSOutputSharePointFile.LoadData]: Cannot load data to the file object, the SharePoint information missing.");
            }

            // Get data
            mOutputData = GetSPDocument();

            if (mOutputData != null)
            {
                // If is image
                if (ImageHelper.IsMimeImage(MimeType))
                {
                    // Resize image if parameters set
                    if ((MaxSideSize > 0) || (Width > 0) || (Height > 0))
                    {
                        ImageHelper imgHelper = new ImageHelper(mOutputData);

                        // Original dimensions
                        int originalWidth = imgHelper.ImageWidth;
                        int originalHeight = imgHelper.ImageHeight;

                        // Resize
                        int[] dim = imgHelper.EnsureImageDimensions(Width, Height, MaxSideSize);

                        if ((dim[0] != originalWidth) || (dim[1] != originalHeight))
                        {
                            // Get altered data
                            mOutputData = imgHelper.GetResizedImageData(dim[0], dim[1], ImageHelper.DefaultQuality);

                            Resized = true;
                        }
                    }
                }
            }

            mDataLoaded = true;
        }


        /// <summary>
        /// Retrieves file(document or image) content of specified document from SharePoint server
        /// trough Copy web service
        /// </summary>    
        /// <returns>Byte array</returns>
        private byte[] GetSPDocument()
        {
            // Get parameters from URL
            string serverUrl = SharePointServer;

            if ((serverUrl == null) || (SharePointFilePath == null))
            {
                return null;
            }

            // Prepare valid server address
            if (!serverUrl.StartsWithCSafe("http://") && !serverUrl.StartsWithCSafe("https://"))
            {
                serverUrl = "http://" + serverUrl;
            }
            serverUrl = serverUrl.TrimEnd('/');

            // Complete URL to path
            string fileUrl = serverUrl + "/" + SharePointFilePath;

            // Download file from SharePoint
            byte[] fileContents = null;
            WebClient wc = new WebClient();

            try
            {
                // Try download file
                wc.Credentials = SharePointFunctions.GetSharePointCredetials();
                fileContents = wc.DownloadData(fileUrl);
            }
            catch (Exception ex)
            {
                // Log exception to Event log
                EventLogProvider.LogException("GetSharePointFile", "GetItem", ex);
            }

            return fileContents;
        }

        #endregion


        #region "IDataContainer Members"

        /// <summary>
        /// Gets or sets the value of the column.
        /// </summary>
        /// <param name="columnName">Column name</param>
        public object this[string columnName]
        {
            get
            {
                return GetValue(columnName);
            }
            set
            {
                SetValue(columnName, value);
            }
        }


        /// <summary>
        /// Column names.
        /// </summary>
        public List<string> ColumnNames
        {
            get
            {
                return TypeHelper.NewList(
                    "dataloaded",
                    "fileextension",
                    "height",
                    "isvalid",
                    "lastmodified",
                    "maxsidesize",
                    "mimetype",
                    "outputdata",
                    "resized",
                    "sharepointfilepath",
                    "sharepointserver",
                    "validfrom",
                    "validto",
                    "width"
                    );
            }
        }


        /// <summary>
        /// Returns value of column.
        /// </summary>
        /// <param name="columnName">Column name</param>
        /// <param name="value">Returns the value</param>
        /// <returns>Returns true if the operation was successful (the value was present)</returns>
        public bool TryGetValue(string columnName, out object value)
        {
            switch (columnName.ToLowerCSafe())
            {
                case "dataloaded":
                    value = DataLoaded;
                    return true;

                case "fileextension":
                    value = FileExtension;
                    return true;

                case "height":
                    value = Height;
                    return true;

                case "isvalid":
                    value = IsValid;
                    return true;

                case "lastmodified":
                    value = LastModified;
                    return true;

                case "maxsidesize":
                    value = MaxSideSize;
                    return true;

                case "mimetype":
                    value = MimeType;
                    return true;

                case "outputdata":
                    value = OutputData;
                    return true;

                case "resized":
                    value = Resized;
                    return true;

                case "sharepointfilepath":
                    value = SharePointFilePath;
                    return true;

                case "sharepointserver":
                    value = SharePointServer;
                    return true;

                case "validfrom":
                    value = ValidFrom;
                    return true;

                case "validto":
                    value = ValidTo;
                    return true;

                case "width":
                    value = Width;
                    return true;
            }

            value = null;
            return false;
        }


        /// <summary>
        /// Returns value of column.
        /// </summary>
        /// <param name="columnName">Column name</param>
        public object GetValue(string columnName)
        {
            object value = null;
            TryGetValue(columnName, out value);

            return value;
        }


        /// <summary>
        /// Sets value of column.
        /// </summary>
        /// <param name="columnName">Column name</param>
        /// <param name="value">Column value</param> 
        public bool SetValue(string columnName, object value)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns true if the object contains specified column.
        /// </summary>
        /// <param name="columnName">Column name</param>
        public bool ContainsColumn(string columnName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}