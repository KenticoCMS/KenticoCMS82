using System;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.IO;
using CMS.PortalEngine;
using CMS.Base;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSModules_PortalEngine_UI_WebParts_Development_WebPart_New : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup page title text and image
        PageTitle.TitleText = GetString("Development-WebPart_Edit.TitleNew");
        // Initialize
        btnOk.Text = GetString("general.ok");
        rfvWebPartDisplayName.ErrorMessage = GetString("Development-WebPart_Edit.ErrorDisplayName");
        rfvWebPartName.ErrorMessage = GetString("Development-WebPart_Edit.ErrorWebPartName");

        webpartSelector.ShowInheritedWebparts = false;

        lblWebpartList.Text = GetString("DevelopmentWebPartEdit.InheritedWebPart");

        // Set breadcrumbs
        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("Development-WebPart_Edit.WebParts"),
            RedirectUrl = UIContextHelper.GetElementUrl("CMS.Design", "Development.WebParts", false),
            Target = "_parent"
        });

        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("Development-WebPart_Edit.New"),
        });

        FileSystemDialogConfiguration config = new FileSystemDialogConfiguration();
        config.AllowedExtensions = "ascx";
        config.ShowFolders = false;

        FileSystemSelector.DialogConfig = config;
        FileSystemSelector.AllowEmptyValue = false;
        FileSystemSelector.SelectedPathPrefix = "~/CMSWebParts/";
        FileSystemSelector.DefaultPath = "CMSWebParts";
    }


    /// <summary>
    /// Handles radio buttons change.
    /// </summary>
    protected void radNewWebPart_CheckedChanged(object sender, EventArgs e)
    {
        plcFileName.Visible = radNewWebPart.Checked;
        plcWebparts.Visible = radInherited.Checked;
    }


    /// <summary>
    /// Creates new web part.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Validate the text box fields
        string errorMessage = new Validator().IsCodeName(txtWebPartName.Text, GetString("general.invalidcodename")).Result;

        // Check file name
        if (!chkGenerateFiles.Checked && radNewWebPart.Checked)
        {
            if (errorMessage == String.Empty)
            {
                string webpartPath = WebPartInfoProvider.GetWebPartPhysicalPath(FileSystemSelector.Value.ToString());

                if (!radInherited.Checked)
                {
                    errorMessage = new Validator().IsFileName(Path.GetFileName(webpartPath), GetString("WebPart_Clone.InvalidFileName")).Result;
                }
            }
        }

        if (errorMessage != String.Empty)
        {
            ShowError(HTMLHelper.HTMLEncode(errorMessage));
            return;
        }

        // Run in transaction
        using (var tr = new CMSTransactionScope())
        {
            WebPartInfo wi = new WebPartInfo();

            // Check if new name is unique
            WebPartInfo webpart = WebPartInfoProvider.GetWebPartInfo(txtWebPartName.Text);
            if (webpart != null)
            {
                ShowError(GetString("Development.WebParts.WebPartNameAlreadyExist").Replace("%%name%%", txtWebPartName.Text));
                return;
            }


            string filename = FileSystemSelector.Value.ToString().Trim();
            if (filename.ToLowerCSafe().StartsWithCSafe("~/cmswebparts/"))
            {
                filename = filename.Substring("~/cmswebparts/".Length);
            }

            wi.WebPartDisplayName = txtWebPartDisplayName.Text.Trim();
            wi.WebPartFileName = filename;
            wi.WebPartName = txtWebPartName.Text.Trim();
            wi.WebPartCategoryID = QueryHelper.GetInteger("parentobjectid", 0);
            wi.WebPartDescription = "";
            wi.WebPartDefaultValues = "<form></form>";
            // Initialize WebPartType - fill it with the default value
            wi.WebPartType = wi.WebPartType;

            // Inherited web part
            if (radInherited.Checked)
            {
                // Check if is selected webpart and isn't category item
                if (ValidationHelper.GetInteger(webpartSelector.Value, 0) <= 0)
                {
                    ShowError(GetString("WebPartNew.InheritedCategory"));
                    return;
                }

                int parentId = ValidationHelper.GetInteger(webpartSelector.Value, 0);
                var parent = WebPartInfoProvider.GetWebPartInfo(parentId);
                if (parent != null)
                {
                    wi.WebPartType = parent.WebPartType;
                    wi.WebPartResourceID = parent.WebPartResourceID;
                    wi.WebPartSkipInsertProperties = parent.WebPartSkipInsertProperties;
                }

                wi.WebPartParentID = parentId;

                // Create empty default values definition
                wi.WebPartProperties = "<defaultvalues></defaultvalues>";
            }
            else
            {
                // Check if filename was added
                if (!FileSystemSelector.IsValid())
                {
                    ShowError(FileSystemSelector.ValidationError);

                    return;
                }
                else
                {
                    wi.WebPartProperties = "<form></form>";
                    wi.WebPartParentID = 0;
                }
            }

            // Save the web part
            WebPartInfoProvider.SetWebPartInfo(wi);

            if (chkGenerateFiles.Checked && radNewWebPart.Checked)
            {
                string physicalFile = WebPartInfoProvider.GetFullPhysicalPath(wi);
                if (!File.Exists(physicalFile))
                {
                    string ascx;
                    string code;
                    string designer;

                    // Write the files
                    try
                    {
                        WebPartInfoProvider.GenerateWebPartCode(wi, null, out ascx, out code, out designer);

                        string folder = Path.GetDirectoryName(physicalFile);

                        // Ensure the folder
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }

                        File.WriteAllText(physicalFile, ascx);
                        File.WriteAllText(physicalFile + ".cs", code);

                        // Designer file
                        if (!String.IsNullOrEmpty(designer))
                        {
                            File.WriteAllText(physicalFile + ".designer.cs", designer);
                        }

                    }
                    catch (Exception ex)
                    {
                        LogAndShowError("WebParts", "GENERATEFILES", ex, true);
                        return;
                    }
                }
                else
                {
                    ShowError(String.Format(GetString("General.FileExistsPath"), physicalFile));
                    return;
                }
            }

            // Refresh web part tree
            ScriptHelper.RegisterStartupScript(this, typeof(string), "reloadframee", ScriptHelper.GetScript(
                "parent.location = '" + UIContextHelper.GetElementUrl("cms.design", "Development.Webparts", false, wi.WebPartID) + "';"));

            PageBreadcrumbs.Items[1].Text = HTMLHelper.HTMLEncode(wi.WebPartDisplayName);
            ShowChangesSaved();
            plcTable.Visible = false;

            tr.Commit();
        }
    }
}