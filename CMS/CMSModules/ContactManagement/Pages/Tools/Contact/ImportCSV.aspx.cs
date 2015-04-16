using System;
using System.Collections.Generic;
using System.Linq;

using CMS.Core;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;

[UIElement(ModuleName.ONLINEMARKETING, "ContactImport")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_ImportCSV : CMSContactManagementPage
{
    private SiteInfo mCurrentSite;


    private SiteInfo CurrentSite
    {
        get
        {
            // SiteContext.CurrentSite cannot be used, because it recognizes site based on domain name, so even if global is selected in site selector, SiteContext.CurrentSite never shows global site
            return mCurrentSite ?? (mCurrentSite = SiteInfoProvider.GetSiteInfo(QueryHelper.GetInteger("siteId", SiteContext.CurrentSiteID)));
        }
    }


    private object DataFromServer
    {
        get
        {
            return new
            {
                ResourceStrings = new Dictionary<string, string>
                {
                    {"om.contact.importcsv.notsupportedbrowser", GetString("om.contact.importcsv.notsupportedbrowser")},
                    {"om.contact.importcsv.selectfilestepmessage.main", GetString("om.contact.importcsv.selectfilestepmessage.main")},
                    {"om.contact.importcsv.selectfilestepmessage.message", GetString("om.contact.importcsv.selectfilestepmessage.message")},
                    {"om.contact.importcsv.selectfilestepmessage.note", GetString("om.contact.importcsv.selectfilestepmessage.note")},
                    {"om.contact.importcsv.selectfilestepmessage.header", GetString("om.contact.importcsv.selectfilestepmessage.header")},
                    {"om.contact.importcsv.selectfilestepmessage.comma", GetString("om.contact.importcsv.selectfilestepmessage.comma")},
                    {"om.contact.importcsv.selectfilestepmessage.encoding", GetString("om.contact.importcsv.selectfilestepmessage.encoding")},
                    {"om.contact.importcsv.selectfilestepmessage.duplicates", GetString("om.contact.importcsv.selectfilestepmessage.duplicates")},
                    {"om.contact.importcsv.selectfilestepmessage.columns", GetString("om.contact.importcsv.selectfilestepmessage.columns")},
                    {"om.contact.importcsv.selectfilebuttontext", GetString("om.contact.importcsv.selectfilebuttontext")},
                    {"om.contact.importcsv.badfileformat", GetString("om.contact.importcsv.badfileformat")},
                    {"om.contact.importcsv.mapstepmessage", GetString("om.contact.importcsv.mapstepmessage")},
                    {"om.contact.importcsv.noemailmapping", GetString("om.contact.importcsv.noemailmapping")},
                    {"om.contact.importcsv.belongsto", GetString("om.contact.importcsv.belongsto")},
                    {"om.contact.importcsv.importingstepmessage", GetString("om.contact.importcsv.importingstepmessage")},
                    {"om.contact.importcsv.importcontactsbuttontext", GetString("om.contact.importcsv.importcontactsbuttontext")},
                    {"om.contact.importcsv.firstrowslengthdoesnotmatch", GetString("om.contact.importcsv.firstrowslengthdoesnotmatch")},
                    {"om.contact.importcsv.donotimport", GetString("om.contact.importcsv.donotimport")},
                    {"om.contact.importcsv.importfinished", GetString("om.contact.importcsv.importfinished")},
                    {"om.contact.importcsv.selectcg", GetString("om.contact.importcsv.selectcg")},
                    {"om.contact.importcsv.nocg", GetString("om.contact.importcsv.nocg")},
                    {"om.contact.importcsv.finishedstepmessage", GetString("om.contact.importcsv.finishedstepmessage")},
                    {"om.contact.importcsv.duplicatescount", GetString("om.contact.importcsv.duplicatescount")},
                    {"om.contact.importcsv.notimported", GetString("om.contact.importcsv.notimported")},
                    {"om.contact.importcsv.importing", GetString("om.contact.importcsv.importing")},
                    {"om.contact.importcsv.emptyfile", GetString("om.contact.importcsv.emptyfile")},
                    {"om.contact.importcsv.importerror", GetString("om.contact.importcsv.importerror")},
                    {"om.contact.importcsv.importedcount", GetString("om.contact.importcsv.importedcount")},
                    {"om.contact.importcsv.invalidcsv", GetString("om.contact.importcsv.invalidcsv")},
                    {"om.contact.importcsv.confirmleave", GetString("om.contact.importcsv.confirmleave")},
                    {"om.contact.importcsv.badmimetype", GetString("om.contact.importcsv.badmimetype")},
                    {"om.contact.importcsv.unknownerrorclientside", GetString("om.contact.importcsv.unknownerrorclientside")},
                },
                ContactFields = GetCategoriesAndFields(),
                ContactGroups = ContactGroupInfoProvider.GetContactGroups()
                                                        .OnSite(CurrentSite.SiteID)
                                                        .Select(g => new { g.ContactGroupGUID, g.ContactGroupDisplayName }),
                SiteGuid = CurrentSite.SiteGUID
            };
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (CheckSiteAndPermissions())
        {
            ScriptHelper.RegisterAngularModule("CMS.OnlineMarketing/ContactImport/Module", DataFromServer);
            
            base.OnPreRender(e);

            CurrentMaster.PanelContent.CssClass = "";
        }
    }


    private IEnumerable<object> GetCategoriesAndFields()
    {
        FormInfo filterFieldsForm = FormHelper.GetFormInfo(ContactInfo.OBJECT_TYPE + ".CMSImportContacts", false);
        var categoriesAndFields = filterFieldsForm.GetHierarchicalFormElements(x => x.Visible);

        var categoriesList = new List<object>();

        foreach (var category in categoriesAndFields)
        {
            var categoryMembers = category.Value.Select(x => new
            {
                x.Name,
                DisplayName = ResHelper.LocalizeString(x.GetDisplayName(MacroResolver.GetInstance()))
            });

            string categoryName = string.IsNullOrEmpty(category.Key.CategoryName) ? GetString("om.contact.importcsv.nofieldcategory") : ResHelper.LocalizeString(category.Key.GetPropertyValue(FormCategoryPropertyEnum.Caption, MacroResolver.GetInstance()));

            categoriesList.Add(new
            {
                CategoryName = categoryName,
                CategoryMembers = categoryMembers
            });
        }
        return categoriesList;
    }


    private bool CheckSiteAndPermissions()
    {
        if (CurrentSite == null)
        {
            ShowInformation(GetString("om.contact.importcsv.selectsite"));
            return false;
        }

        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.OnlineMarketing", new[] { "ContactsFrameset", "ContactImport" }, CurrentSite.SiteName))
        {
            RedirectToUIElementAccessDenied("CMS.OnlineMarketing", "ContactsFrameset;ContactImport");
        }

        string[] requiredPermissions =
            {
                "ReadContacts",
                "ModifyContacts",
                "ReadContactGroups", 
                "ModifyContactGroups"
            };

        foreach (var permission in requiredPermissions)
        {
            if (!CurrentUser.IsAuthorizedPerResource(ModuleName.CONTACTMANAGEMENT, permission, CurrentSite.SiteName))
            {
                RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, permission);
            }
        }

        return true;
    }
}