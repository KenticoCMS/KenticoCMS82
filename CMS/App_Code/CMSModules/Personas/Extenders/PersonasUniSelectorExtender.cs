using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using CMS;
using CMS.Base;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Personas;
using CMS.UIControls;


[assembly: RegisterCustomClass("PersonasUniSelectorExtender", typeof(PersonasUniSelectorExtender))]

/// <summary>
/// Extends Personas Unigrid with additional abilities.
/// </summary>
public class PersonasUniSelectorExtender : ControlExtender<UniSelector>
{
    private readonly IPersonaPictureImgTagGenerator mPersonaPictureImgTagGenerator = PersonasFactory.GetPersonaPictureImgTagGenerator();


    /// <summary>
    /// Initializes extender
    /// </summary>
    public override void OnInit()
    {
        Control.UniGrid.GridView.AddCssClass("rows-middle-vertical-align");
        Control.OnAdditionalDataBound += SetPersonaImageAndDisplayName;
    }


    /// <summary>
    /// Gets content for row with persona image and display name.
    /// </summary>
    /// <returns>Modified content of given row</returns>
    private object SetPersonaImageAndDisplayName(object sender, string sourceName, object parameter, object value)
    {
        switch (sourceName.ToLowerCSafe())
        {
            // Get content for row with persona name
            case "itemname":
            {
                LinkButton btn = value as LinkButton;
                DataRowView drv = parameter as DataRowView;
                
                if ((btn == null) || (drv == null))
                {
                    return value;
                }

                var persona = PersonaInfoProvider.GetPersonaInfoById(ValidationHelper.GetInteger(drv.Row["PersonaID"], 0));

                string imgTag = mPersonaPictureImgTagGenerator.GenerateImgTag(persona, 50);

                if (imgTag != null)
                {
                    btn.Text = imgTag + "<span class=\"personas-table-persona-name\">" + HTMLHelper.HTMLEncode(persona.PersonaDisplayName) + "</span>";
                }
                return btn;
            }
        }

        return value;
    }

}