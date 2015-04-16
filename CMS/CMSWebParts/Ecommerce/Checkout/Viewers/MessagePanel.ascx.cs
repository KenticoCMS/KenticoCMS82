using System;
using System.Linq;

using CMS.Base;
using CMS.Helpers;


/// <summary>
/// Panel for info/error messages in checkout process
/// </summary>
public partial class CMSWebParts_Ecommerce_Checkout_Viewers_MessagePanel : CMSCheckoutWebPart
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ComponentEvents.RequestEvents.RegisterForEvent(MESSAGE_RAISED, HandleMessage);
    }


    /// <summary>
    /// Handle incoming message.
    /// </summary>
    protected void HandleMessage(object sender, EventArgs e)
    {
        CMSEventArgs<string> args = e as CMSEventArgs<string>;

        if (args != null)
        {
            string message = args.Parameter;
            if (!String.IsNullOrEmpty(message))
            {
                lblMessage.Text = message;
                messageWrapper.Visible = true;
            }
            else
            {
                messageWrapper.Visible = false;
            }
            // Clear parameter to tag this message as handled
            args.Parameter = string.Empty;

        }
    }
}