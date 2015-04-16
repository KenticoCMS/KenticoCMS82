using System;

using CMS.ExtendedControls;
using CMS.UIControls;

public partial class CMSFormControls_Basic_TextBoxControl : TextBoxControl
{
    /// <summary>
    /// Textbox control
    /// </summary>
    protected override CMSTextBox TextBox
    {
        get
        {
            return txtText;
        }
    }
}
