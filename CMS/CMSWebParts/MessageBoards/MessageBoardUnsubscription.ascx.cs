using System;

using CMS.Helpers;
using CMS.PortalControls;

public partial class CMSWebParts_MessageBoards_MessageBoardUnsubscription : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Information text
    /// </summary>
    public string UnsubscriptionInfoText
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("UnsubscriptionInfoText"), "");
        }
        set
        {
            this.SetValue("UnsubscriptionInfoText", value);
        }
    }


    /// <summary>
    /// Information CSS class
    /// </summary>
    public string UnsubscriptionInfoCssClass
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("UnsubscriptionInfoCssClass"), "");
        }
        set
        {
            this.SetValue("UnsubscriptionInfoCssClass", value);
        }
    }


    /// <summary>
    /// Unsubscription button text
    /// </summary>
    public string UnsubscriptionButtonText
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("UnsubscriptionButtonText"), "");
        }
        set
        {
            this.SetValue("UnsubscriptionButtonText", value);
        }
    }


    /// <summary>
    /// Unsubscription button CSS class
    /// </summary>
    public string UnsubscriptionButtonCssClass
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("UnsubscriptionButtonCssClass"), "");
        }
        set
        {
            this.SetValue("UnsubscriptionButtonCssClass", value);
        }
    }


    /// <summary>
    /// Confirmation text
    /// </summary>
    public string UnsubscriptionText
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("UnsubscriptionText"), "You have been successfully unsubscribed.");
        }
        set
        {
            this.SetValue("UnsubscriptionText", value);
        }
    }


    /// <summary>
    /// Unsuccessful unsubscription text
    /// </summary>
    public string UnsuccessfulUnsubscriptionText
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("UnsuccessfulUnsubscriptionText"), "Unsubscription was unsuccessfull.");
        }
        set
        {
            this.SetValue("UnsuccessfulUnsubscriptionText", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties
    /// </summary>
    protected void SetupControl()
    {
        if (this.StopProcessing)
        {
            // Do not process
            unsubscription.StopProcessing = true;
        }
        else
        {
            string subscription = QueryHelper.GetString("boardsubscriptionhash", null);
            Guid subGuid = QueryHelper.GetGuid("boardsubguid", Guid.Empty);

            if (!string.IsNullOrEmpty(subscription) || (subGuid != Guid.Empty))
            {
                unsubscription.SuccessfulUnsubscriptionText = UnsubscriptionText;
                unsubscription.UnsuccessfulUnsubscriptionText = UnsuccessfulUnsubscriptionText;
                unsubscription.UnsubscriptionInfoText = UnsubscriptionInfoText;
                unsubscription.UnsubscriptionInfoCssClass = UnsubscriptionInfoCssClass;
                unsubscription.UnsubscriptionButtonText = UnsubscriptionButtonText;
                unsubscription.UnsubscriptionButtonCssClass = UnsubscriptionButtonCssClass;
            }
            else
            {
                Visible = false;
            }
        }
    }


    /// <summary>
    /// Reloads the control data
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }

    #endregion
}