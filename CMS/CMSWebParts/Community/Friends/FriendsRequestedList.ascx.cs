using System;
using CMS.Membership;
using CMS.PortalControls;

public partial class CMSWebParts_Community_Friends_FriendsRequestedList : CMSAbstractWebPart
{
    #region "Stop processing"

    /// <summary>
    /// Returns true if the control processing should be stopped.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            lstRequested.StopProcessing = value;
        }
    }

    #endregion


    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
            lstRequested.StopProcessing = true;
        }
        else
        {
            lstRequested.RedirectToAccessDeniedPage = false;
            lstRequested.UserID = MembershipContext.AuthenticatedUser.UserID;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = (AuthenticationHelper.IsAuthenticated() && lstRequested.HasData());
    }
}