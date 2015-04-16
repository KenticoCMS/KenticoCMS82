using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;

public partial class CMSModules_AdminControls_Pages_ObjectRelationships : CMSModalGlobalAdminPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        // Setup the control
        relElem.ObjectID = QueryHelper.GetInteger("objectid", 0);
        relElem.ObjectType = QueryHelper.GetString("objecttype", "");

        GeneralizedInfo obj = relElem.Object;
        if (obj != null)
        {
            // Set the master page title
            Title = String.Format(GetString("ObjectRelationships.Title"), GetString("objecttype." + obj.TypeInfo.ObjectType.Replace(".", "_").Replace("#", "_")), HTMLHelper.HTMLEncode(obj.ObjectDisplayName));

            PageTitle.TitleText = Title;
            CurrentMaster.PanelContent.RemoveCssClass("dialog-content");
            CurrentMaster.MessagesPlaceHolder.OffsetX = 8;
            CurrentMaster.MessagesPlaceHolder.OffsetY = 8;

            btnAnother.Click += (s, ea) => relElem.Save();
            var master = CurrentMaster as ICMSModalMasterPage;
            if (master != null)
            {
                master.Save += (s, ea) => relElem.SaveAndClose();
                master.ShowSaveAndCloseButton();
                // The button shouldn't have save&close label, because it doesn't seem like it closes - it redirects to a listing of all relationships
                master.SetSaveResourceString("general.save");
            }
        }
    }
}