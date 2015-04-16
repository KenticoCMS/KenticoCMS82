using System;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Personas;
using CMS.UIControls;


[assembly: RegisterCustomClass("PersonasUniGridExtender", typeof (PersonasUniGridExtender))]

/// <summary>
/// Extends Personas Unigrid with additional abilities.
/// </summary>
public class PersonasUniGridExtender : ControlExtender<UniGrid>
{
    private readonly IPersonaPictureImgTagGenerator mPersonaPictureImgTagGenerator = PersonasFactory.GetPersonaPictureImgTagGenerator();


    /// <summary>
    /// Initializes extender
    /// </summary>
    public override void OnInit()
    {
        Control.GridView.AddCssClass("rows-middle-vertical-align");
        Control.OnExternalDataBound += SetPersonaImageAndDisplayName;
    }


    /// <summary>
    /// Gets content for row with persona image and display name.
    /// </summary>
    /// <returns>Modified content of given row</returns>
    private object SetPersonaImageAndDisplayName(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            // Get content for row with persona image and name
            case "PersonaNameAndImage":
            {
                var persona = PersonaInfoProvider.GetPersonaInfoById(ValidationHelper.GetInteger(parameter, 0));

                string imgTag = mPersonaPictureImgTagGenerator.GenerateImgTag(persona, 50);

                string encodedPersonaDisplayName = HTMLHelper.HTMLEncode(persona.PersonaDisplayName);

                // Show only display name of persona when persona does not have image and default image is not specified
                if (imgTag == null)
                {
                    return encodedPersonaDisplayName;
                }

                return imgTag + "<span class=\"personas-table-persona-name\">" + encodedPersonaDisplayName + "</span>";
            }
        }

        return parameter;
    }
}