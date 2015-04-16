using System;

using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;

public partial class CMSPages_Ecommerce_GetProductFile : AbstractCMSPage
{
    #region "Variables"

    private Guid token = Guid.Empty;

    #endregion


    #region "Page methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Get download token from URL
        token = QueryHelper.GetGuid("token", Guid.Empty);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        string errorMessage = null;

        // Get order item SKU file
        OrderItemSKUFileInfo oiskufi = OrderItemSKUFileInfoProvider.GetOrderItemSKUFileInfo(token);

        if (oiskufi != null)
        {
            // Get parent order item
            OrderItemInfo oii = OrderItemInfoProvider.GetOrderItemInfo(oiskufi.OrderItemID);

            if (oii != null)
            {
                // If download is not expired
                if ((oii.OrderItemValidTo.CompareTo(DateTimeHelper.ZERO_TIME) == 0) || (oii.OrderItemValidTo.CompareTo(DateTime.Now) > 0))
                {
                    // Get SKU file
                    SKUFileInfo skufi = SKUFileInfoProvider.GetSKUFileInfo(oiskufi.FileID);

                    if (skufi != null)
                    {
                        // Decide how to process the file based on file type
                        switch (skufi.FileType.ToLowerCSafe())
                        {
                            case "metafile":
                                // Set parameters to current context
                                Context.Items["fileguid"] = skufi.FileMetaFileGUID;
                                Context.Items["disposition"] = "attachment";

                                // Perform server side redirect to download
                                Response.Clear();
                                Server.Transfer(URLHelper.ResolveUrl("~/CMSPages/GetMetaFile.aspx"));
                                Response.End();
                                return;
                        }
                    }
                }
                else
                {
                    // Set error message
                    errorMessage = ResHelper.GetString("getproductfile.expirederror");
                }
            }
        }

        // If error message not set
        if (String.IsNullOrEmpty(errorMessage))
        {
            // Set default error message
            errorMessage = ResHelper.GetString("getproductfile.existerror");
        }

        // Set error message to current context
        Context.Items["title"] = ResHelper.GetString("getproductfile.error");
        Context.Items["text"] = errorMessage;

        // Perform server side redirect to error page
        Response.Clear();
        Server.Transfer(URLHelper.ResolveUrl("~/CMSMessages/Error.aspx"));
        Response.End();
    }

    #endregion
}