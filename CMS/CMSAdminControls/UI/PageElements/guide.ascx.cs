using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;

public partial class CMSAdminControls_UI_PageElements_guide : CMSUserControl
{
    #region "Variables"

    private List<List<string>> mParameters;
    private int mColumns = 1;

    #endregion


    #region "Properties"

    /// <summary>
    /// List of Lists containing ImageURL, Title, PageURL, Description.
    /// </summary>
    public List<List<string>> Parameters
    {
        get
        {
            return mParameters;
        }
        set
        {
            mParameters = value;
        }
    }


    /// <summary>
    /// Number of columns (default 1).
    /// </summary>
    public int Columns
    {
        get
        {
            return mColumns;
        }
        set
        {
            mColumns = value;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (mParameters != null)
        {
            // Register JavaScript
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "detectTreeFrame", ScriptHelper.GetScript(InitScript()));

            Panel PanelModules = new Panel();
            Table tbl = new Table();
            tbl.CellSpacing = 10;
            TableRow tr = new TableRow();
            int actualRowIndex = 0;
            int relativeWidth = Convert.ToInt32(100 / mColumns);
            for (int i = 0; i < mParameters.Count; i++)
            {
                List<string> row = mParameters[i];
                if (row != null)
                {
                    actualRowIndex++;

                    // Initialize Image
                    Image img = new Image();
                    img.ImageUrl = UIHelper.ResolveImageUrl(row[0]);
                    img.CssClass = "PageTitleImage";

                    // Initialize Title
                    LocalizedHeading heading = new LocalizedHeading();
                    heading.Text = " " + HTMLHelper.HTMLEncode(row[1]);
                    heading.EnableViewState = false;
                    heading.Level = 4;


                    // Initialize Hyperlink
                    HyperLink h = new HyperLink();
                    h.Controls.Add(img);
                    h.Controls.Add(heading);

                    if (row.Count > 4)
                    {
                        string fullContent = "false";
                        if (row.Count > 5)
                        {
                            fullContent = row[5].ToLowerCSafe() == "true" ? "true" : "false";
                        }

                        // Ensure not-null help key
                        row[4] = row[4] ?? "";

                        // For personalized guide use code name
                        h.Attributes.Add("onclick", "ShowDesktopContent(" + ScriptHelper.GetString(row[2]) + ", " + fullContent + ", " + ScriptHelper.GetString("node_" + row[4].Replace(".", String.Empty).ToLowerCSafe()) + ");");
                    }
                    else
                    {
                        // Else use display name
                        h.Attributes.Add("onclick", "ShowDesktopContent(" + ScriptHelper.GetString(row[2]) + ", " + ScriptHelper.GetString(row[1]) + ");");
                    }
                    h.Attributes.Add("href", "#");

                    // Resolve description
                    string description = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(row[3]));

                    // Initialize description
                    Label desc = new Label();
                    desc.Text = "<div>" + description + "</div>";

                    // Initialize wrapping panel
                    Panel p = new Panel();
                    p.Controls.Add(h);
                    p.Controls.Add(desc);

                    // Add style
                    p.CssClass = "Guide";

                    // Add to the table
                    TableCell td = new TableCell();

                    // Align all cells to top
                    td.VerticalAlign = VerticalAlign.Top;

                    // Add single description to table-cell
                    td.Controls.Add(p);

                    if (actualRowIndex == mColumns || (i == mParameters.Count - 1))
                    {
                        tr.Cells.Add(td);

                        // Ensure right column number for validity
                        if (i == mParameters.Count - 1)
                        {
                            for (int d = 0; d < (mColumns - (mParameters.Count % mColumns)); d++)
                            {
                                tr.Cells.Add(new TableCell());
                            }
                        }

                        // Add to table
                        tbl.Rows.Add(tr);

                        // Reset index counter
                        actualRowIndex = 0;

                        // Create new row
                        tr = new TableRow();
                    }
                    else
                    {
                        // Set relative width
                        td.Attributes.Add("style", "width:" + relativeWidth + "%;");

                        // Add to table-row
                        tr.Cells.Add(td);
                    }
                }
            }

            // Add single module description to PanelModules
            PanelModules.Controls.Add(tbl);

            // Render whole description
            plcGuide.Controls.Add(PanelModules);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns JavaScript to detect tree frame.
    /// </summary>
    private static string InitScript()
    {
        const string script = @"
var leftMenuFrame;
for(var f = 0;f < parent.frames.length;f++)
{
    if(parent.frames[f].name.toLowerCase().indexOf('tree') != -1)
    leftMenuFrame = parent.frames[f];
}";
        return script;
    }

    #endregion
}