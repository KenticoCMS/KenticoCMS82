using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Principal;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.EventLog;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

// Edited object
[Breadcrumb(1, "com.discount.generatecoupons")]
public partial class CMSModules_Ecommerce_Pages_Tools_Discount_Discount_Codes_Generator : CMSEcommercePage
{
    #region "Variables and constants"

    private static readonly Hashtable mWarnings = new Hashtable();
    private int mDiscountId;
    private int count;
    private string prefix = "";
    private int numberOfUses;
    private string redirectUrl;
    private Discount mDiscount;
    private string mElementName;
    private const string pattern = "*****";

    #endregion


    #region "Properties"

    /// <summary>
    /// DiscountId query string parameter.
    /// </summary>
    private int DiscountID
    {
        get
        {
            if (mDiscountId == 0)
            {
                mDiscountId = QueryHelper.GetInteger("discountId", 0);
            }
            return mDiscountId;
        }
    }


    /// <summary>
    /// Gets the right discount info from DB based on Discount type and Discount ID.
    /// Returns DiscountWrapper object filled with the discount values.
    /// </summary>
    private Discount Discount
    {
        get
        {
            if (mDiscount == null)
            {
                if (QueryHelper.GetBoolean("isMultiBuy", false))
                {
                    mDiscount = MultiBuyDiscountInfoProvider.GetMultiBuyDiscountInfo(DiscountID);
                }
                else
                {
                    mDiscount = DiscountInfoProvider.GetDiscountInfo(DiscountID);
                }
            }

            return mDiscount;
        }
    }


    /// <summary>
    /// Gets the name of the page element.
    /// </summary>    
    private string ElementName
    {
        get
        {
            if (string.IsNullOrEmpty(mElementName))
            {
                switch (Discount.DiscountType)
                {
                    case DiscountTypeEnum.MultibuyDiscount:
                        mElementName = "MultiBuyCouponsCodes";
                        break;

                    case DiscountTypeEnum.OrderDiscount:
                        mElementName = "OrderCouponCodes";
                        break;

                    case DiscountTypeEnum.ShippingDiscount:
                        mElementName = "ShippingCouponCodes";
                        break;
                }
            }

            return mElementName;
        }
    }


    /// <summary>
    /// Current log context.
    /// </summary>
    public LogContext CurrentLog
    {
        get
        {
            return EnsureLog();
        }
    }


    /// <summary>
    /// Current Error.
    /// </summary>
    private string CurrentError
    {
        get
        {
            return ValidationHelper.GetString(mWarnings["DefineError_" + ctlAsyncLog.ProcessGUID], string.Empty);
        }
        set
        {
            mWarnings["DefineError_" + ctlAsyncLog.ProcessGUID] = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Parent discount does not exist or it is a catalog discount where coupons are not allowed
        if (Discount == null || Discount.DiscountType == DiscountTypeEnum.CatalogDiscount)
        {
            EditedObjectParent = null;
            return;
        }

        // Check if object from current site is edited
        CheckEditedObjectSiteID(Discount.DiscountSiteID);

        // Check UI permissions
        CheckUIPermissions();

        CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("com.couponcode.generate"),
            CommandName = "generate"
        });

        redirectUrl = GetRedirectUrl();
        SetBreadcrumb(0, GetString("com.discount.coupons"), redirectUrl, null, null);

        if (!URLHelper.IsPostback())
        {
            // Show error message
            if (QueryHelper.Contains("error"))
            {
                ShowError(HTMLHelper.HTMLEncode(QueryHelper.GetString("error", string.Empty)));
            }
        }

        // Setup and configure asynchronous control
        SetupAsyncControl();
    }

    #endregion


    #region "Event handlers"

    void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        if (e.CommandName == "generate")
        {
            if (!DiscountInfoProvider.IsUserAuthorizedToModifyDiscount(SiteContext.CurrentSiteName, CurrentUser))
            {
                RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyDiscounts");
            }

            // Collect data from form
            count = ValidationHelper.GetInteger(txtNumberOfCodes.Text, 0);
            numberOfUses = ValidationHelper.GetInteger(txtTimesToUse.Text, int.MinValue);
            prefix = txtPrefix.Text.Trim();

            // Validate inputs
            if (count < 1)
            {
                ShowError(GetString("com.couponcode.invalidcount"));
                return;
            }

            if (!string.IsNullOrEmpty(txtTimesToUse.Text) && ((numberOfUses <= 0) || (numberOfUses > 999999)))
            {
                ShowError(GetString("com.couponcode.invalidnumberOfUses"));
                return;
            }

            if (!string.IsNullOrEmpty(prefix) && !ValidationHelper.IsCodeName(prefix))
            {
                ShowError(GetString("com.couponcode.invalidprefix"));
                return;
            }
            // Set numberOfUses to 0 for empty
            if (string.IsNullOrEmpty(txtTimesToUse.Text))
            {
                numberOfUses = 0;
            }

            // Run action in asynchronous control
            EnsureAsyncLog();
            RunAsync(GenerateCodes);
        }
    }


    /// <summary>
    /// Checks page UI permissions based on parent discount type.
    /// </summary>
    private void CheckUIPermissions()
    {
        // Check UI personalization
        CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, ElementName);
    }

    #endregion


    #region "Methods"

    private string GetRedirectUrl()
    {
        if (Discount != null)
        {
            if (!string.IsNullOrEmpty(ElementName))
            {
                var url = UIContextHelper.GetElementUrl("CMS.Ecommerce", ElementName);
                url = URLHelper.AddParameterToUrl(url, "parentobjectid", Discount.DiscountID.ToString());
                url = URLHelper.AddParameterToUrl(url, "displaytitle", "false");
                return url;
            }
        }

        return RequestContext.CurrentURL;
    }


    private HashSet<string> GetExistingCodes()
    {
        // Prepare query for codes cache
        var existingQuery = ECommerceHelper.GetAllCouponCodesQuery(SiteContext.CurrentSiteID);

        // Restrict cache if prefix specified
        if (!string.IsNullOrEmpty(prefix))
        {
            existingQuery.Where("CouponCodeCode", QueryOperator.Like, prefix + "%");
        }

        // Create cache of this site coupon codes
        var existingCodes = new HashSet<string>();
        using (DbDataReader reader = existingQuery.ExecuteReader())
        {
            while (reader.Read())
            {
                existingCodes.Add(reader.GetString(0));
            }
        }

        return existingCodes;
    }


    private void GenerateCodes(object parameter)
    {
        try
        {
            // Construct cache for code uniqueness check
            var existingCodes = GetExistingCodes();
            BaseInfo coupon = null;

            using (CMSActionContext context = new CMSActionContext())
            {
                // Do not touch parent for all codes
                context.TouchParent = false;
                context.LogEvents = false;

                // Create generator
                var generator = new RandomCodeGenerator(pattern, prefix);
                // Use cache for checking code uniqueness
                generator.CodeChecker = code => !existingCodes.Contains(code);

                for (int i = 0; i < count; i++)
                {
                    // Get new code
                    string code = generator.GenerateCode();

                    coupon = Discount.CreateCoupon(code, numberOfUses);

                    // Log that coupon was created
                    AddLog(string.Format(GetString("com.couponcode.generated"), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(code))));
                }
            }

            // Touch parent (one for all)
            if (coupon != null)
            {
                coupon.Generalized.TouchParent();
            }

            // Log information that coupons were generated
            EventLogProvider.LogEvent(EventType.INFORMATION, "Discounts", "GENERATECOUPONCODES",
                                      string.Format("{0} coupon codes for discount '{1}' successfully generated.", count, Discount.DiscountDisplayName),
                                      userId: CurrentUser.UserID,
                                      userName: CurrentUser.UserName,
                                      siteId: SiteContext.CurrentSiteID
                                     );
        }
        catch (Exception ex)
        {
            CurrentError = GetString("com.couponcode.generateerror");
            EventLogProvider.LogException("Discounts", "GENERATECOUPONCODES", ex);
        }
    }


    /// <summary>
    /// Adds parameter to current URL and Redirects to it.
    /// </summary>
    /// <param name="parameter">Parameter to be added.</param>
    /// <param name="value">Value of parameter to be added.</param>
    private void RedirectTo(string parameter, string value)
    {
        string urlToRedirect = URLHelper.AddParameterToUrl(RequestContext.CurrentURL, parameter, value);
        URLHelper.Redirect(urlToRedirect);
    }

    #endregion


    #region "Handling asynchronous thread"

    private void ctlAsyncLog_OnCancel(object sender, EventArgs e)
    {
        CurrentLog.Close();
        RedirectTo("error", GetString("com.couponcode.generationterminated"));
    }


    private void ctlAsyncLog_OnFinished(object sender, EventArgs e)
    {
        CurrentLog.Close();

        if (!String.IsNullOrEmpty(CurrentError))
        {
            RedirectTo("error", CurrentError);
        }

        URLHelper.Redirect(redirectUrl);
    }


    /// <summary>
    /// Adds the log information
    /// </summary>
    /// <param name="newLog">New log information</param>
    protected void AddLog(string newLog)
    {
        EnsureLog();
        LogContext.AppendLine(newLog);
    }


    /// <summary>
    /// Ensures the logging context
    /// </summary>
    protected LogContext EnsureLog()
    {
        LogContext currentLog = LogContext.EnsureLog(ctlAsyncLog.ProcessGUID);

        return currentLog;
    }


    /// <summary>
    /// Ensures log for asynchronous control
    /// </summary>
    private void EnsureAsyncLog()
    {
        pnlLog.Visible = true;
        pnlGeneral.Visible = false;
        CurrentError = string.Empty;

        CurrentLog.Close();
        EnsureLog();
    }


    /// <summary>
    /// Runs asynchronous thread
    /// </summary>
    /// <param name="action">Method to run</param>
    protected void RunAsync(AsyncAction action)
    {
        ctlAsyncLog.RunAsync(action, WindowsIdentity.GetCurrent());
    }


    /// <summary>
    /// Prepare asynchronous control
    /// </summary>
    private void SetupAsyncControl()
    {
        ctlAsyncLog.OnFinished += ctlAsyncLog_OnFinished;
        ctlAsyncLog.OnError += (s, e) => CurrentLog.Close();
        ctlAsyncLog.OnRequestLog += (sender, args) => { ctlAsyncLog.LogContext = CurrentLog; };
        ctlAsyncLog.OnCancel += ctlAsyncLog_OnCancel;

        ctlAsyncLog.MaxLogLines = 1000;

        // Asynchronous content configuration
        ctlAsyncLog.TitleText = GetString("com.couponcode.generating");
        if (!RequestHelper.IsCallback())
        {
            // Set visibility of panels
            pnlGeneral.Visible = true;
            pnlLog.Visible = false;
        }
    }

    #endregion
}