using System;
using System.Linq;

using CMS;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Base;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.Personas;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.PortalEngine;
using CMS.Core;

[assembly: RegisterCustomClass("PersonaContactsListExtender", typeof(PersonaContactsListExtender))]

/// <summary>
/// Persona related contacts list UniGrid extender.
/// </summary>
public class PersonaContactsListExtender : ControlExtender<UniGrid>
{
    #region "Public methods"

    /// <summary>
    /// OnInit event.
    /// </summary>
    public override void OnInit()
    {
        RegisterScripts();

        // Hide default "Show" & "Reset" buttons (since another filter control with these buttons is used)
        Control.HideFilterButton = true;
        Control.ShowObjectMenu = false;
        UpdateGridWithObjectQuery();

        // Use closure to avoid multiple utilization of the same resource.
        bool canReadContact = new ContactInfo() { ContactSiteID = SiteContext.CurrentSiteID }.CheckPermissions(PermissionsEnum.Read, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser);

        Control.OnExternalDataBound += (sender, name, parameter) =>
        {
            // contactdetail is set as external source name in corresponding xml definition 
            if (name.ToLowerCSafe() == "contactdetail")
            {
                CMSGridActionButton actionButton = (CMSGridActionButton)sender;
                if (!canReadContact)
                {
                    actionButton.Enabled = false;
                    actionButton.ToolTip = ResHelper.GetString("general.readNotAllowed");
                }
            }

            return parameter;
        };
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Registers scripts for displaying modal dialog containing Contact details and postback logic.
    /// </summary>
    private void RegisterScripts()
    {
        // Ensure Dialog script is loaded
        ScriptHelper.RegisterDialogScript(Control.Page);

        // File with JavaScript logic related to this class
        ScriptHelper.RegisterScriptFile(Control.Page, "~/CMSModules/Personas/Scripts/PersonaContactsList.js");

        // prepare script for setting contact details dialog url which is later rendered into the page, so it can be used by JS. Url is not hardcoded into JS, so proper hash string can be generated.
        string dialogUrl = UIContextHelper.GetElementDialogUrl(ModuleName.ONLINEMARKETING, "EditContact");
        string script = string.Format("CMS.Personas.PersonaContactsList.setContactDetailDialogBaseUrl({0});", ScriptHelper.GetString(dialogUrl));

        ScriptHelper.RegisterStartupScript(Control, typeof(string), "CMS.setContactDetailDialogBaseUrl", script, true);
    }


    /// <summary>
    /// Updates UniGrid to display the same contacts as the ones in the given ObjectQuery (filters out contacts not related to current persona).
    /// </summary>
    private void UpdateGridWithObjectQuery()
    {
        var objectQuery = PersonasFactory.GetPersonaService().GetContactsForPersona(GetCurrentPersonaInfo());

        if (Control.QueryParameters == null)
        {
            Control.QueryParameters = objectQuery.Parameters;
        }
        else
        {
            foreach (var param in objectQuery.Parameters)
            {
                Control.QueryParameters.Add(param);
            }
        }

        Control.WhereCondition = new WhereCondition(Control.GetFilter()).Where(objectQuery.WhereCondition).WhereCondition;
    }


    /// <summary>
    /// Obtains current PersonaInfo object.
    /// </summary>
    /// <returns>Current PersonaInfo object</returns>
    private PersonaInfo GetCurrentPersonaInfo()
    {
        var personaID = ((PersonaInfo)((CMSPage)Control.Page).EditedObjectParent).PersonaID;
        return PersonaInfoProvider.GetPersonaInfoById(personaID);
    }

    #endregion
}
