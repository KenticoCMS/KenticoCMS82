using System;
using System.Text;
using System.Web.UI;

using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_AdminControls_Controls_Class_FormBuilder_FormComponents : CMSUserControl
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        FillPanelWithComponents();
    }


    private void FillPanelWithComponents()
    {
        var controls = FormUserControlInfoProvider.GetFormUserControls("UserControlShowInBizForms = 1 AND UserControlPriority = 100", null);
        StringBuilder content = new StringBuilder();

        foreach (var control in controls)
        {
            string iconUrl;
            if (control.UserControlThumbnailGUID == Guid.Empty)
            {
                iconUrl = GetImageUrl("CMSModules/CMS_FormEngine/custom.png");
            }
            else
            {
                iconUrl = ResolveUrl(MetaFileURLProvider.GetMetaFileUrl(control.UserControlThumbnailGUID, "icon"));
            }

            content.AppendFormat("<div title=\"{0}\"><div class=\"form-component component_{1}\" ondblclick=\"FormBuilder.addNewField('{1}','',-1);FormBuilder.scrollPosition=9999;\"><span class=\"component-label\">{2}</span><img src=\"{3}\" alt=\"{1}\" /></div></div>",
                ResHelper.LocalizeString(control.UserControlDescription), control.UserControlCodeName, control.UserControlDisplayName, iconUrl);
        }

        pnlFormComponents.Controls.Add(new LiteralControl(content.ToString()));
    }
}