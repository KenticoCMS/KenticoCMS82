using System;

using CMS;
using CMS.Helpers;
using CMS.DataEngine;
using CMS.DocumentEngine.Types;
using CMS.DocumentEngine;

[assembly: RegisterDocumentType(News.CLASS_NAME, typeof(News))]

namespace CMS.DocumentEngine.Types
{
    /// <summary>
    /// Sample item class.
    /// </summary>
    public partial class News : TreeNode
    {
        #region "Constants"

        /// <summary>
        /// Class name of the item.
        /// </summary>
        public const string CLASS_NAME = "CMS.News";

        #endregion


        #region "Properties"

        /// <summary>
        /// 
        /// </summary>
        [DatabaseField]
        public int NewsID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("NewsID"), 0);
            }
            set
            {
                SetValue("NewsID", value);
            }
        }


        /// <summary>
        /// News Title.
        /// </summary>
        [DatabaseField]
        public string NewsTitle
        {
            get
            {
                return ValidationHelper.GetString(GetValue("NewsTitle"), "");
            }
            set
            {
                SetValue("NewsTitle", value);
            }
        }


        /// <summary>
        /// Release Date.
        /// </summary>
        [DatabaseField]
        public DateTime NewsReleaseDate
        {
            get
            {
                return ValidationHelper.GetDateTime(GetValue("NewsReleaseDate"), DateTimeHelper.ZERO_TIME);
            }
            set
            {
                SetValue("NewsReleaseDate", value);
            }
        }


        /// <summary>
        /// News Summary.
        /// </summary>
        [DatabaseField]
        public string NewsSummary
        {
            get
            {
                return ValidationHelper.GetString(GetValue("NewsSummary"), "");
            }
            set
            {
                SetValue("NewsSummary", value);
            }
        }


        /// <summary>
        /// News Text.
        /// </summary>
        [DatabaseField]
        public string NewsText
        {
            get
            {
                return ValidationHelper.GetString(GetValue("NewsText"), "");
            }
            set
            {
                SetValue("NewsText", value);
            }
        }


        /// <summary>
        /// Teaser.
        /// </summary>
        [DatabaseField]
        public Guid NewsTeaser
        {
            get
            {
                return ValidationHelper.GetGuid(GetValue("NewsTeaser"), Guid.Empty);
            }
            set
            {
                SetValue("NewsTeaser", value);
            }
        }

        #endregion


        #region "Constructors"

        /// <summary>
        /// Constructor.
        /// </summary>
        public News()
            : base(CLASS_NAME)
        {
        }

        #endregion
    }
}
