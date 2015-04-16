using CMS;
using CMS.Base;
using CMS.EmailEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.UIControls;

[assembly: RegisterCustomClass("EmailTemplateListExtender", typeof(EmailTemplateListExtender))]

/// <summary>
/// Email template unigrid extender
/// </summary>
public class EmailTemplateListExtender : ControlExtender<UniGrid>
{
    public override void OnInit()
    {
        Control.OnExternalDataBound += OnExternalDataBound;
    }


    /// <summary>
    /// Handles external databound event of unigrid.
    /// </summary>
    protected object OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "type":
                string type = ValidationHelper.GetString(parameter, string.Empty);
                if (string.IsNullOrEmpty(type))
                {
                    type = EmailTemplateTypeEnum.General.ToStringRepresentation();
                }

                return HTMLHelper.HTMLEncode(ResHelper.GetString("emailtemplate.type." + type));
        }

        return parameter;
    }
}