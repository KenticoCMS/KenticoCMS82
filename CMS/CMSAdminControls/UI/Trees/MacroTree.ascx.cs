using System;
using System.Web.UI.WebControls;
using System.Linq;

using CMS.UIControls;

public partial class CMSAdminControls_UI_Trees_MacroTree : MacroTree
{
    /// <summary>
    /// Tree view control.
    /// </summary>
    protected override TreeView TreeControl
    {
        get
        {
            return treeElem;
        }
    }


    /// <summary>
    /// Priority (context) items tree control
    /// </summary>
    protected override TreeView PriorityTreeControl
    {
        get
        {
            return treeElemPriority;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        pnlPriorityTree.Visible = treeElemPriority.Visible;
    }
}