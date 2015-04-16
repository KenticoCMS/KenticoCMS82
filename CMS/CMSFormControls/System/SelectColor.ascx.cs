using CMS.FormControls;
using CMS.Helpers;

public partial class CMSFormControls_System_SelectColor : FormEngineUserControl
{
    #region "Constants"

    private const string WHITE_HEX = "#FFFFFF";

    #endregion


    #region "Public properties"

    public override object Value
    {
        get
        {
            return ValidationHelper.GetString(clrPicker.SelectedColor, WHITE_HEX);
        }
        set
        {
            clrPicker.SelectedColor = ValidationHelper.GetString(value, WHITE_HEX);
        }
    }

    #endregion
}