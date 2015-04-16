using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_Properties_HTMLMediaProperties : ItemProperties
{
    #region "Private properties"

    /// <summary>
    /// Gets or sets the original URL (the one which come when properties are loaded).
    /// </summary>
    private string OriginalUrl
    {
        get
        {
            return ValidationHelper.GetString(ViewState["OriginalUrl"], "");
        }
        set
        {
            ViewState["OriginalUrl"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the permanent URL.
    /// </summary>
    private string PermanentUrl
    {
        get
        {
            return ValidationHelper.GetString(ViewState["PermanentUrl"], "");
        }
        set
        {
            ViewState["PermanentUrl"] = value;
        }
    }


    /// <summary>
    /// Gets or sets current url (depending on the settings).
    /// </summary>
    private string CurrentUrl
    {
        get
        {
            if (DisplayUrlTextbox)
            {
                switch (ViewMode)
                {
                    case MediaTypeEnum.Flash:
                        return txtFlashUrl.Text;
                    case MediaTypeEnum.AudioVideo:
                        return txtVidUrl.Text;
                    default:
                        return txtUrl.Text;
                }
            }
            else
            {
                return ValidationHelper.GetString(ViewState["CurrentUrl"], "");
            }
        }
        set
        {
            if (DisplayUrlTextbox)
            {
                switch (ViewMode)
                {
                    case MediaTypeEnum.Flash:
                        string ext = URLHelper.GetUrlParameter(txtFlashUrl.Text, "ext");
                        if (!string.IsNullOrEmpty(ext))
                        {
                            value = URLHelper.UpdateParameterInUrl(value, "ext", ext);
                        }
                        txtFlashUrl.Text = value;
                        break;
                    case MediaTypeEnum.AudioVideo:
                        txtVidUrl.Text = value;
                        break;
                    default:
                        txtUrl.Text = value;
                        break;
                }
            }
            else
            {
                ViewState["CurrentUrl"] = value;
            }

            // For other sources than MediaLibraries OriginalUrl and PermanentUrl is the same
            if (SourceType != MediaSourceEnum.MediaLibraries)
            {
                OriginalUrl = value;
                PermanentUrl = value;
            }
        }
    }

    /// <summary>
    /// Returns the default width of the width height selector.
    /// </summary>
    private int DefaultWidth
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["DefaultHeight"], 0);
        }
        set
        {
            ViewState["DefaultHeight"] = value;
        }
    }


    /// <summary>
    /// Returns the default height of the width height selector.
    /// </summary>
    private int DefaultHeight
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["DefaultWidth"], 0);
        }
        set
        {
            ViewState["DefaultWidth"] = value;
        }
    }


    /// <summary>
    /// Returns original width of the image.
    /// </summary>
    private int OriginalWidth
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["OriginalWidth"], 0);
        }
        set
        {
            ViewState["OriginalWidth"] = value;
        }
    }


    /// <summary>
    /// Returns original height of the image.
    /// </summary>
    private int OriginalHeight
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["OriginalHeight"], 0);
        }
        set
        {
            ViewState["OriginalHeight"] = value;
        }
    }

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the View mode of the control (it determines which tabs are displayed).
    /// </summary>
    public MediaTypeEnum ViewMode
    {
        get
        {
            return CMSDialogHelper.GetMediaType(ValidationHelper.GetString(ViewState["ViewMode"], ""));
        }
        set
        {
            ViewState["ViewMode"] = value;
            ShowProperTabs();
        }
    }


    /// <summary>
    /// Indicates whether the URL textbox is displayed at top of the properties panel.
    /// </summary>
    public bool DisplayUrlTextbox
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["DisplayUrlTextbox"], true);
        }
        set
        {
            ViewState["DisplayUrlTextbox"] = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            // Refresh buttons
            string refreshTooltip = GetString("dialogs.web.refresh");

            imgRefresh.ScreenReaderDescription = imgRefresh.ToolTip = refreshTooltip;
            imgVidRefresh.ToolTip = imgVidRefresh.ScreenReaderDescription = refreshTooltip;
            imgFlashRefresh.ToolTip = imgFlashRefresh.ScreenReaderDescription = refreshTooltip;

            btnVideoPreview.Click += (s, ea) => LoadVideoPreview();
            btnFlashPreview.Click += (s, ea) => LoadFlashPreview();
            btnImagePreview.Click += btnImagePreview_Click;
            btnImageTxtPreview.Click += (s, ea) => UpdateFromUrl();
            imgRefresh.Click += (s, ea) => UpdateFromUrl();
            imgFlashRefresh.Click += (s, ea) => LoadFlashPreview();
            imgVidRefresh.Click += (s, ea) => LoadVideoPreview();

            btnSizeRefreshHidden.Click += btnSizeRefreshHidden_Click;
            btnBehaviorSizeRefreshHidden.Click += btnBehaviorSizeRefreshHidden_Click;

            imgWidthHeightElem.CustomRefreshCode = ControlsHelper.GetPostBackEventReference(btnBehaviorSizeRefreshHidden, "") + ";return false;";

            // Display URL txt box if required
            plcUrlTxt.Visible = DisplayUrlTextbox;
            plcFlashUrl.Visible = DisplayUrlTextbox;
            plcVidUrl.Visible = DisplayUrlTextbox;
            colorElem.IsLiveSite = IsLiveSite;

            lblEmpty.Text = NoSelectionText;

            var className = (!DisplayUrlTextbox) ? "large-preview" : "small-preview";
            pnlPreviewType.CssClass = className;
        }
        else
        {
            lblEmpty.Text = NoSelectionText;
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Becuse of loss of color after postback in update panel
        colorElem.SelectedColor = colorElem.ColorTextBox.Text;

        ShowProperTabs();
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!StopProcessing)
        {
            // Load align dropdown with values
            drpAlign.Items.Add(new ListItem(GetString("general.selectnone"), ""));
            drpAlign.Items.Add(new ListItem(GetString("dialogs.image.align.left"), "left"));
            drpAlign.Items.Add(new ListItem(GetString("dialogs.image.align.right"), "right"));
            drpAlign.Items.Add(new ListItem(GetString("dialogs.image.align.bottom"), "bottom"));
            drpAlign.Items.Add(new ListItem(GetString("dialogs.image.align.middle"), "middle"));
            drpAlign.Items.Add(new ListItem(GetString("dialogs.image.align.top"), "top"));
            drpAlign.Items.Add(new ListItem(GetString("dialogs.image.align.baseline"), "baseline"));
            drpAlign.Items.Add(new ListItem(GetString("dialogs.image.align.textbottom"), "text-bottom"));
            drpAlign.Items.Add(new ListItem(GetString("dialogs.image.align.texttop"), "text-top"));

            // Load target dropdown with values
            drpLinkTarget.Items.Add(new ListItem(GetString("general.selectnone"), "none"));
            drpLinkTarget.Items.Add(new ListItem(GetString("dialogs.target.blank"), "_blank"));
            drpLinkTarget.Items.Add(new ListItem(GetString("dialogs.target.self"), "_self"));
            drpLinkTarget.Items.Add(new ListItem(GetString("dialogs.target.parent"), "_parent"));
            drpLinkTarget.Items.Add(new ListItem(GetString("dialogs.target.top"), "_top"));

            // Load target dropdown with values
            drpFlashScale.Items.Add(new ListItem(GetString("general.selectnone"), ""));
            drpFlashScale.Items.Add(new ListItem(GetString("dialogs.flash.scale.showall"), "showall"));
            drpFlashScale.Items.Add(new ListItem(GetString("dialogs.flash.scale.noborder"), "noborder"));
            drpFlashScale.Items.Add(new ListItem(GetString("dialogs.flash.scale.exactfit"), "exactfit"));
        }
    }


    protected void btnSizeRefreshHidden_Click(object sender, EventArgs e)
    {
        // Remove width & height parameters from url
        string url = URLHelper.RemoveParameterFromUrl(URLHelper.RemoveParameterFromUrl(CurrentUrl, "width"), "height");
        CurrentUrl = url;

        switch (ViewMode)
        {
            case MediaTypeEnum.AudioVideo:
                vidWidthHeightElem.Width = DefaultWidth;
                vidWidthHeightElem.Height = DefaultHeight;
                LoadVideoPreview();

                break;
            case MediaTypeEnum.Flash:
                flashWidthHeightElem.Width = DefaultWidth;
                flashWidthHeightElem.Height = DefaultHeight;
                LoadFlashPreview();

                break;

            default:
                widthHeightElem.Width = DefaultWidth;
                widthHeightElem.Height = DefaultHeight;
                LoadImagePreview();
                break;
        }
    }


    protected void btnBehaviorSizeRefreshHidden_Click(object sender, EventArgs e)
    {
        imgWidthHeightElem.Width = DefaultWidth;
        imgWidthHeightElem.Height = DefaultHeight;
    }

    #endregion


    #region "Private general methods"

    /// <summary>
    /// Display or hides the tabs according to the ViewMode setting.
    /// </summary>
    private void ShowProperTabs()
    {
        pnlTabs.SelectedTabIndex = 0;

        switch (ViewMode)
        {
            case MediaTypeEnum.AudioVideo:

                tabFlashAdvanced.Visible = false;
                tabFlashAdvanced.HeaderText = "";

                tabFlashGeneral.Visible = false;
                tabFlashGeneral.HeaderText = "";

                tabImageAdvanced.Visible = false;
                tabImageAdvanced.HeaderText = "";

                tabImageBehavior.Visible = false;
                tabImageBehavior.HeaderText = "";

                tabImageGeneral.Visible = false;
                tabImageGeneral.HeaderText = "";

                tabImageLink.Visible = false;
                tabImageLink.HeaderText = "";

                tabVideoGeneral.Visible = true;
                tabVideoGeneral.HeaderText = GetString("general.general");

                InitAVMode();
                LoadVideoPreview(false);

                break;
            case MediaTypeEnum.Flash:

                tabFlashAdvanced.Visible = true;
                tabFlashAdvanced.HeaderText = GetString("dialogs.tab.advanced");

                tabFlashGeneral.Visible = true;
                tabFlashGeneral.HeaderText = GetString("general.general");

                tabImageAdvanced.Visible = false;
                tabImageAdvanced.HeaderText = "";

                tabImageBehavior.Visible = false;
                tabImageBehavior.HeaderText = "";

                tabImageGeneral.Visible = false;
                tabImageGeneral.HeaderText = "";

                tabImageLink.Visible = false;
                tabImageLink.HeaderText = "";

                tabVideoGeneral.Visible = false;
                tabVideoGeneral.HeaderText = "";

                InitFlashMode();
                LoadFlashPreview(false);

                break;

            default:

                tabFlashAdvanced.Visible = false;
                tabFlashAdvanced.HeaderText = "";

                tabFlashGeneral.Visible = false;
                tabFlashGeneral.HeaderText = "";

                tabImageAdvanced.Visible = true;
                tabImageAdvanced.HeaderText = GetString("dialogs.tab.advanced");

                tabImageBehavior.Visible = true;
                tabImageBehavior.HeaderText = GetString("dialogs.tab.behavior");

                tabImageGeneral.Visible = true;
                tabImageGeneral.HeaderText = GetString("general.general");

                tabImageLink.Visible = true;
                tabImageLink.HeaderText = GetString("general.link");

                tabVideoGeneral.Visible = false;
                tabVideoGeneral.HeaderText = "";

                InitImageMode();
                LoadImagePreview(false);

                break;
        }
    }

    #endregion


    #region "Private methods for Image mode"

    private void InitImageMode()
    {
        string postBackRef = ControlsHelper.GetPostBackEventReference(btnImagePreview, "");
        string postBackKeyDownRef = "var keynum;if(window.event){keynum = event.keyCode;}else if(event.which){keynum = event.which;}if(keynum == 13){" + postBackRef + "; return false;}";
        string postBackTxtRef = ControlsHelper.GetPostBackEventReference(btnImageTxtPreview, "");
        string postBackKeyDownTxtRef = "var keynum;if(window.event){keynum = event.keyCode;}else if(event.which){keynum = event.which;}if(keynum == 13){" + postBackTxtRef + "; return false;}";

        // Assign javascript change event to all fields (to refresh the preview)
        // General tab
        txtAlt.Attributes["onchange"] = postBackRef;
        txtAlt.Attributes["onkeydown"] = postBackKeyDownRef;
        txtBorderWidth.Attributes["onchange"] = postBackRef;
        txtBorderWidth.Attributes["onkeydown"] = postBackKeyDownRef;
        txtHSpace.Attributes["onchange"] = postBackRef;
        txtHSpace.Attributes["onkeydown"] = postBackKeyDownRef;
        txtVSpace.Attributes["onchange"] = postBackRef;
        txtVSpace.Attributes["onkeydown"] = postBackKeyDownRef;
        drpAlign.Attributes["onchange"] = postBackRef;
        txtUrl.Attributes["onchange"] = postBackTxtRef;
        txtUrl.Attributes["onkeydown"] = postBackKeyDownTxtRef;
        widthHeightElem.HeightTextBox.Attributes["onkeydown"] = postBackKeyDownRef;
        widthHeightElem.WidthTextBox.Attributes["onkeydown"] = postBackKeyDownRef;
        widthHeightElem.ScriptAfterChange = postBackRef;
        colorElem.ColorTextBox.Attributes["onchange"] = postBackRef;
        colorElem.ColorTextBox.Attributes["onkeydown"] = postBackKeyDownRef;
        
        // Link tab
        txtLinkUrl.Attributes["onchange"] = postBackRef + "; ensureBehaviorTab(false);";
        txtLinkUrl.Attributes["onkeydown"] = postBackKeyDownRef;
        drpLinkTarget.Attributes["onchange"] = postBackRef;
        
        // Advanced tab
        txtImageAdvId.Attributes["onchange"] = postBackRef;
        txtImageAdvId.Attributes["onkeydown"] = postBackKeyDownRef;
        txtImageAdvTooltip.Attributes["onchange"] = postBackRef;
        txtImageAdvTooltip.Attributes["onkeydown"] = postBackKeyDownRef;
        txtImageAdvClass.Attributes["onchange"] = postBackRef;
        txtImageAdvClass.Attributes["onkeydown"] = postBackKeyDownRef;
        txtImageAdvStyle.Attributes["onchange"] = postBackRef;
        
        // Behavior
        radImageNone.InputAttributes["onchange"] = postBackRef + "; ensureBehaviorTab(false);";
        radImageSame.InputAttributes["onchange"] = postBackRef + "; ensureBehaviorTab(false);";
        radImageNew.InputAttributes["onchange"] = postBackRef + "; ensureBehaviorTab(false);";
        radImageLarger.InputAttributes["onchange"] = postBackRef + "; ensureBehaviorTab(false);";
        imgWidthHeightElem.HeightTextBox.Attributes["onchange"] = postBackRef;
        imgWidthHeightElem.HeightTextBox.Attributes["onkeydown"] = postBackKeyDownRef;
        imgWidthHeightElem.WidthTextBox.Attributes["onchange"] = postBackRef;
        imgWidthHeightElem.WidthTextBox.Attributes["onkeydown"] = postBackKeyDownRef;

        string postBackRefWidthHeight = ControlsHelper.GetPostBackEventReference(btnSizeRefreshHidden, "") + ";return false;";
        widthHeightElem.CustomRefreshCode = postBackRefWidthHeight;

        // Browse server (Link tab) scripts
        btnLinkBrowseServer.OnClientClick = "browseServer(); return false;";
        ScriptHelper.RegisterDialogScript(Page);

        // Get the correct url of the dialog
        bool isLiveSite = RequestContext.CurrentURL.Contains("LiveSelectors");
        string url = ResolveUrl("~/CMSFormControls/" + (isLiveSite ? "Live" : "") + "Selectors/InsertImageOrMedia/default.aspx") + URLHelper.GetQuery(RequestContext.CurrentURL);
        url = URLHelper.RemoveParameterFromUrl(URLHelper.RemoveParameterFromUrl(URLHelper.AddParameterToUrl(url, DialogParameters.EDITOR_CLIENTID, txtLinkUrl.ClientID), "hash"), "output");
        url = URLHelper.AddParameterToUrl(url, "output", "url");
        url = URLHelper.UpdateParameterInUrl(url, "link", "1");
        url = URLHelper.UpdateParameterInUrl(url, "extension_to_url", "0");
        url = URLHelper.UpdateParameterInUrl(url, "size_to_url", "0");
        url = URLHelper.RemoveParameterFromUrl(url, "content");
        url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url, false));

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ImageLinkBrowseServer", ScriptHelper.GetScript(
            "function browseServer() { \n" +
            "   modalDialog('" + url + "', 'SelectLinkDialog', '90%', '85%', null, true); \n" +
            "} \n" +
            "function SetUrl(text) { \n" +
            "    var urlElem = document.getElementById('" + txtLinkUrl.ClientID + "'); \n" +
            "    if (urlElem != null) { \n" +
            "        urlElem.value = text;\n" +
            "    } \n" +
            "} \n"));

        // Behavior tab script
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ImageLinkBehavior", ScriptHelper.GetScript(
            "function ensureBehaviorTab(clearUrl) {\n" +
            "  var txtUrl = document.getElementById('" + txtLinkUrl.ClientID + "'); \n" +
            "  var radNone = document.getElementById('" + radImageNone.ClientID + "'); \n" +
            "  var rad1 = document.getElementById('" + radImageSame.ClientID + "'); \n" +
            "  var rad2 = document.getElementById('" + radImageNew.ClientID + "'); \n" +
            "  var removeLink = document.getElementById('" + pnlRemoveLink.ClientID + "'); \n" +
            "  var removeLink2 = document.getElementById('" + pnlRemoveLink2.ClientID + "'); \n" +
            "  var urlInfo = document.getElementById('js-link-url-info'); \n" +
            "  if ((txtUrl != null) && (rad1 != null) && (rad2 != null) \n" +
            "       && (removeLink != null) && (removeLink2 != null) && (urlInfo != null)) { \n" +
            "    if (clearUrl) {txtUrl.value = '';} \n" +
            "    var disableRadios = (txtUrl.value.length != 0); \n" +
            "    rad1.disabled = disableRadios;\n" +
            "    rad2.disabled = disableRadios;\n" +
            "    removeLink.style.display = (disableRadios ? 'inline' : 'none');\n" +
            "    removeLink2.style.display = (disableRadios ? 'inline' : 'none');\n" +
            "    if (rad1.checked || rad2.checked) { \n" +
            "      if (disableRadios) { \n" +
            "        radNone.checked = true; \n" +
            "        urlInfo.style.display = 'none'; \n" +
            "      } else {  \n" +
            "        urlInfo.style.display = '';  \n" +
            "      } \n" +
            "    } else { \n" +
            "      urlInfo.style.display = 'none'; \n" +
            "    } \n" +
            "  } \n" +
            "} function TabSwitch() { ensureBehaviorTab(false); }\n"));

        ScriptHelper.RegisterStartupScript(Page, typeof(Page), "ImageLinkBehaviorDefault", ScriptHelper.GetScript("ensureBehaviorTab(false);"));

        pnlTabs.OnClientTabClick = "TabSwitch();";
        btnRemoveLink.OnClientClick = "ensureBehaviorTab(true); return false;";
        btnRemoveLink2.OnClientClick = "ensureBehaviorTab(true); return false;";
    }


    /// <summary>
    /// Loads the image preview.
    /// </summary>
    /// <param name="saveSession">Determines whether to save data to session or not</param>
    private void LoadImagePreview(bool saveSession = true)
    {
        if (saveSession)
        {
            SaveImageSession();
        }

        // Don't allow to resize image over the original dimensions
        int width = (widthHeightElem.Width > OriginalWidth) && (OriginalWidth > 0) ? OriginalWidth : widthHeightElem.Width;
        int height = (widthHeightElem.Height > OriginalHeight) && (OriginalHeight > 0) ? OriginalHeight : widthHeightElem.Height;

        string url = CurrentUrl;
        if (!string.IsNullOrEmpty(url) && !ItemNotSystem)
        {
            string chset = Guid.NewGuid().ToString();
            url = URLHelper.UpdateParameterInUrl(url, "chset", chset);

            // Add latest version requirement for live site
            int versionHistoryId = HistoryID;
            if (IsLiveSite && (versionHistoryId > 0))
            {
                // Add requirement for latest version of files for current document
                string newparams = "latestforhistoryid=" + versionHistoryId;
                newparams += "&hash=" + ValidationHelper.GetHashString("h" + versionHistoryId);

                url = URLHelper.AppendQuery(url, newparams);
            }
        }

        imagePreview.URL = url;
        imagePreview.SizeToURL = ValidationHelper.GetBoolean(ViewState[DialogParameters.IMG_SIZETOURL], false);
        imagePreview.Align = drpAlign.SelectedValue;
        imagePreview.Width = width;
        imagePreview.Height = height;
        imagePreview.HSpace = ValidationHelper.GetInteger(txtHSpace.Text, -1);
        imagePreview.VSpace = ValidationHelper.GetInteger(txtVSpace.Text, -1);
        imagePreview.Alt = txtAlt.Text;
        imagePreview.BorderColor = colorElem.SelectedColor;
        imagePreview.BorderWidth = ValidationHelper.GetInteger(txtBorderWidth.Text, -1);
        imagePreview.Style = txtImageAdvStyle.Text.Trim().TrimEnd(';') + ";";
        imagePreview.Tooltip = txtImageAdvTooltip.Text.Trim();
    }


    /// <summary>
    /// Save current image settings into session.
    /// </summary>
    private void SaveImageSession()
    {
        // Get hashtable from session
        Hashtable properties = SessionHelper.GetValue("DialogSelectedParameters") as Hashtable;
        if (properties == null)
        {
            // Create new if necessary
            properties = new Hashtable();
        }
        string url = CurrentUrl.Trim();
        bool sizeToUrl = ValidationHelper.GetBoolean(ViewState[DialogParameters.IMG_SIZETOURL], false);

        // Change size to URL flag if URL contains width/height definition
        if (url.ToLowerCSafe().Contains("width=") || url.ToLowerCSafe().Contains("height="))
        {
            if (!sizeToUrl)
            {
                ViewState[DialogParameters.IMG_SIZETOURL] = sizeToUrl = true;
            }
        }

        if ((widthHeightElem.Width != DefaultWidth) && sizeToUrl)
        {
            url = URLHelper.UpdateParameterInUrl(url, "width", widthHeightElem.Width.ToString());
        }
        if ((widthHeightElem.Height != DefaultHeight) && sizeToUrl)
        {
            url = URLHelper.UpdateParameterInUrl(url, "height", widthHeightElem.Height.ToString());
        }
        string style = txtImageAdvStyle.Text.Trim().TrimEnd(';') + ";";

        properties[DialogParameters.IMG_ORIGINALWIDTH] = OriginalWidth;
        properties[DialogParameters.IMG_ORIGINALHEIGHT] = OriginalHeight;
        properties[DialogParameters.IMG_HEIGHT] = widthHeightElem.Height;
        properties[DialogParameters.IMG_WIDTH] = widthHeightElem.Width;
        properties[DialogParameters.IMG_URL] = URLHelper.ResolveUrl(url);
        properties[DialogParameters.IMG_SIZETOURL] = sizeToUrl;
        properties[DialogParameters.IMG_BORDERCOLOR] = colorElem.SelectedColor;
        properties[DialogParameters.IMG_ALT] = HTMLHelper.HTMLEncode(txtAlt.Text.Trim());
        properties[DialogParameters.IMG_ALIGN] = drpAlign.SelectedValue;
        properties[DialogParameters.IMG_BORDERWIDTH] = ValidationHelper.GetInteger(txtBorderWidth.Text, -1);
        properties[DialogParameters.IMG_HSPACE] = ValidationHelper.GetInteger(txtHSpace.Text, -1);
        properties[DialogParameters.IMG_VSPACE] = ValidationHelper.GetInteger(txtVSpace.Text, -1);
        properties[DialogParameters.IMG_EXT] = ValidationHelper.GetString(ViewState[DialogParameters.IMG_EXT], "");
        properties[DialogParameters.IMG_DIR] = ValidationHelper.GetString(ViewState[DialogParameters.IMG_DIR], "");
        properties[DialogParameters.IMG_USEMAP] = ValidationHelper.GetString(ViewState[DialogParameters.IMG_USEMAP], "");
        properties[DialogParameters.IMG_LANG] = ValidationHelper.GetString(ViewState[DialogParameters.IMG_LANG], "");
        properties[DialogParameters.IMG_LONGDESCRIPTION] = ValidationHelper.GetString(ViewState[DialogParameters.IMG_LONGDESCRIPTION], "");
        properties[DialogParameters.IMG_LINK] = URLHelper.ResolveUrl(txtLinkUrl.Text.Trim());

        string imgLinkUrl = txtLinkUrl.Text.Trim();
        properties[DialogParameters.IMG_LINK] = URLHelper.IsAnchor(imgLinkUrl) ? imgLinkUrl : URLHelper.ResolveUrl(imgLinkUrl);

        if (drpLinkTarget.SelectedIndex > 0)
        {
            properties[DialogParameters.IMG_TARGET] = drpLinkTarget.SelectedValue;
        }
        else
        {
            properties[DialogParameters.IMG_TARGET] = "";
        }
        properties[DialogParameters.IMG_ID] = HTMLHelper.HTMLEncode(txtImageAdvId.Text.Trim());
        properties[DialogParameters.IMG_TOOLTIP] = HTMLHelper.HTMLEncode(txtImageAdvTooltip.Text.Trim());
        properties[DialogParameters.IMG_STYLE] = (style != ";" ? HTMLHelper.HTMLEncode(style) : "");
        properties[DialogParameters.IMG_CLASS] = HTMLHelper.HTMLEncode(txtImageAdvClass.Text.Trim());
        properties[DialogParameters.IMG_MOUSEOVERWIDTH] = "";
        properties[DialogParameters.IMG_MOUSEOVERHEIGHT] = "";

        if (radImageNone.Checked)
        {
            properties[DialogParameters.IMG_BEHAVIOR] = "";
        }
        else if (radImageNew.Checked)
        {
            properties[DialogParameters.IMG_BEHAVIOR] = "_blank";
        }
        else if (radImageSame.Checked)
        {
            properties[DialogParameters.IMG_BEHAVIOR] = "_self";
        }
        else if (radImageLarger.Checked)
        {
            properties[DialogParameters.IMG_BEHAVIOR] = "hover";
            properties[DialogParameters.IMG_MOUSEOVERWIDTH] = imgWidthHeightElem.Width;
            properties[DialogParameters.IMG_MOUSEOVERHEIGHT] = imgWidthHeightElem.Height;
        }
        properties[DialogParameters.LAST_TYPE] = MediaTypeEnum.Image;

        // Save image properties into session
        SessionHelper.SetValue("DialogSelectedParameters", properties);
    }


    /// <summary>
    /// Returns URL updated according specified properties.
    /// </summary>
    /// <param name="height">Height of the item</param>
    /// <param name="width">Width of the item</param>
    /// <param name="url">URL to update</param>
    private string UpdateUrl(int width, int height, string url)
    {
        if (plcUrlTxt.Visible)
        {
            return CMSDialogHelper.UpdateUrl(width, height, OriginalWidth, OriginalHeight, url, SourceType);
        }
        return url;
    }


    /// <summary>
    /// Update parameters and preview from URL textbox.
    /// </summary>
    private void UpdateFromUrl()
    {
        int width = ValidationHelper.GetInteger(URLHelper.GetUrlParameter(CurrentUrl, "width"), 0);
        int height = ValidationHelper.GetInteger(URLHelper.GetUrlParameter(CurrentUrl, "height"), 0);

        // Update WIDTH and HEIGHT information according URL
        if (width > 0)
        {
            widthHeightElem.Width = width;
        }
        if (height > 0)
        {
            widthHeightElem.Height = height;
        }

        pnlUpdateWidthHeight.Update();
        LoadImagePreview();
    }


    protected void btnImagePreview_Click(object sender, EventArgs e)
    {
        // Update item URL
        bool getPermanent = ((widthHeightElem.Width < OriginalWidth) ||
                             (widthHeightElem.Height < OriginalHeight)) &&
                            (SourceType == MediaSourceEnum.MediaLibraries);

        CurrentUrl = UpdateUrl(widthHeightElem.Width, widthHeightElem.Height, (getPermanent ? PermanentUrl : OriginalUrl));

        pnlUpdateImgUrl.Update();
        LoadImagePreview();
    }

    #endregion


    #region "Private methods for AudioVideo mode"

    private void InitAVMode()
    {
        string postBackRef = ControlsHelper.GetPostBackEventReference(btnVideoPreview, "");
        string postBackKeyDownRef = "var keynum;if(window.event){keynum = event.keyCode;}else if(event.which){keynum = event.which;}if(keynum == 13){" + postBackRef + "; return false;}";

        // Assign javascript change event to all fields (to refresh the preview)
        vidWidthHeightElem.HeightTextBox.Attributes["onkeydown"] = postBackKeyDownRef;
        vidWidthHeightElem.WidthTextBox.Attributes["onkeydown"] = postBackKeyDownRef;
        vidWidthHeightElem.ScriptAfterChange = postBackRef;
        chkVidAutoPlay.InputAttributes["onclick"] = postBackRef;
        chkVidLoop.InputAttributes["onclick"] = postBackRef;
        chkVidShowControls.InputAttributes["onclick"] = postBackRef;
        txtVidUrl.Attributes["onchange"] = postBackRef;
        txtVidUrl.Attributes["onkeydown"] = postBackKeyDownRef;

        vidWidthHeightElem.CustomRefreshCode = ControlsHelper.GetPostBackEventReference(btnSizeRefreshHidden, "") + ";return false;";
    }


    /// <summary>
    /// Loads the video preview.
    /// </summary>
    /// <param name="saveSession">Determines whether to save data to session or not</param>
    private void LoadVideoPreview(bool saveSession = true)
    {
        if (saveSession)
        {
            SaveVideoSession();
        }

        string url = CurrentUrl;
        if (!string.IsNullOrEmpty(url) && !ItemNotSystem)
        {
            // Add latest version requirement for live site
            int versionHistoryId = HistoryID;
            if (IsLiveSite && (versionHistoryId > 0))
            {
                // Add requirement for latest version of files for current document
                string newparams = "latestforhistoryid=" + versionHistoryId;
                newparams += "&hash=" + ValidationHelper.GetHashString("h" + versionHistoryId);

                url = URLHelper.AppendQuery(url, newparams);
            }
        }

        videoPreview.Height = vidWidthHeightElem.Height;
        videoPreview.Width = vidWidthHeightElem.Width;
        videoPreview.AutoPlay = false; // Always ignore autoplay in preview
        videoPreview.AVControls = chkVidShowControls.Checked;
        videoPreview.Loop = chkVidLoop.Checked;
        videoPreview.Url = url;
        videoPreview.Type = ValidationHelper.GetString(ViewState[DialogParameters.AV_EXT], "");
    }


    /// <summary>
    /// Save current audi/video settings into session
    /// </summary>
    private void SaveVideoSession()
    {
        // Get hashtable from session
        Hashtable properties = SessionHelper.GetValue("DialogSelectedParameters") as Hashtable;
        if (properties == null)
        {
            // Create new if necessary
            properties = new Hashtable();
        }

        properties[DialogParameters.AV_WIDTH] = vidWidthHeightElem.Width;
        properties[DialogParameters.AV_HEIGHT] = vidWidthHeightElem.Height;
        properties[DialogParameters.AV_AUTOPLAY] = chkVidAutoPlay.Checked;
        properties[DialogParameters.AV_LOOP] = chkVidLoop.Checked;
        properties[DialogParameters.AV_CONTROLS] = chkVidShowControls.Checked;
        properties[DialogParameters.AV_EXT] = ViewState[DialogParameters.AV_EXT];
        properties[DialogParameters.AV_URL] = URLHelper.ResolveUrl(CurrentUrl);
        properties[DialogParameters.LAST_TYPE] = MediaTypeEnum.AudioVideo;

        // Save image properties into session
        SessionHelper.SetValue("DialogSelectedParameters", properties);
    }

    #endregion


    #region "Private methods for Flash mode"

    private void InitFlashMode()
    {
        string postBackRef = ControlsHelper.GetPostBackEventReference(btnFlashPreview, "");
        string postBackKeyDownRef = "var keynum;if(window.event){keynum = event.keyCode;}else if(event.which){keynum = event.which;}if(keynum == 13){" + postBackRef + "; return false;}";

        // Assign javascript change event to all fields (to refresh the preview)
        // General tab
        flashWidthHeightElem.HeightTextBox.Attributes["onkeydown"] = postBackKeyDownRef;
        flashWidthHeightElem.WidthTextBox.Attributes["onkeydown"] = postBackKeyDownRef;
        flashWidthHeightElem.ScriptAfterChange = postBackRef;
        chkFlashAutoplay.InputAttributes["onclick"] = postBackRef;
        chkFlashLoop.InputAttributes["onclick"] = postBackRef;
        chkFlashEnableMenu.InputAttributes["onclick"] = postBackRef;
        txtFlashUrl.Attributes["onchange"] = postBackRef;
        txtFlashUrl.Attributes["onkeydown"] = postBackKeyDownRef;
        // Advanced tab
        drpFlashScale.Attributes["onchange"] = postBackRef;
        txtFlashId.Attributes["onchange"] = postBackRef;
        txtFlashId.Attributes["onkeydown"] = postBackKeyDownRef;
        txtFlashTitle.Attributes["onchange"] = postBackRef;
        txtFlashTitle.Attributes["onkeydown"] = postBackKeyDownRef;
        txtFlashClass.Attributes["onchange"] = postBackRef;
        txtFlashClass.Attributes["onkeydown"] = postBackKeyDownRef;
        txtFlashStyle.Attributes["onchange"] = postBackRef;
        txtFlashStyle.Attributes["onkeydown"] = postBackKeyDownRef;

        flashWidthHeightElem.CustomRefreshCode = ControlsHelper.GetPostBackEventReference(btnSizeRefreshHidden, "") + ";return false;";
    }


    /// <summary>
    /// Loads the flash preview.
    /// </summary>
    /// <param name="saveSession">Determines whether to save data to session or not</param>
    private void LoadFlashPreview(bool saveSession = true)
    {
        if (saveSession)
        {
            SaveFlashSession();
        }

        string url = CurrentUrl;
        if (!string.IsNullOrEmpty(url) && !ItemNotSystem)
        {
            // Add latest version requirement for live site
            int versionHistoryId = HistoryID;
            if (IsLiveSite && (versionHistoryId > 0))
            {
                // Add requirement for latest version of files for current document
                string newparams = "latestforhistoryid=" + versionHistoryId;
                newparams += "&hash=" + ValidationHelper.GetHashString("h" + versionHistoryId);

                url = URLHelper.AddParameterToUrl(URLHelper.AppendQuery(url, newparams), "ext", ".swf");
            }
        }

        flashPreview.Url = url;
        flashPreview.Type = ValidationHelper.GetString(ViewState[DialogParameters.FLASH_EXT], "");
        flashPreview.Height = flashWidthHeightElem.Height;
        flashPreview.Width = flashWidthHeightElem.Width;
        flashPreview.Menu = chkFlashEnableMenu.Checked;
        flashPreview.AutoPlay = chkFlashAutoplay.Checked;
        flashPreview.Loop = chkFlashLoop.Checked;
        flashPreview.Id = txtFlashId.Text.Trim();
        flashPreview.Title = txtFlashTitle.Text.Trim();
        flashPreview.Class = txtFlashClass.Text.Trim();
        flashPreview.Style = txtFlashStyle.Text.Trim().TrimEnd(';') + ";";
        flashPreview.FlashVars = txtFlashVars.Text;
    }


    /// <summary>
    /// Save current flash settings into session.
    /// </summary>
    private void SaveFlashSession()
    {
        // Get hashtable from session
        Hashtable props = SessionHelper.GetValue("DialogSelectedParameters") as Hashtable ?? new Hashtable();

        props[DialogParameters.FLASH_WIDTH] = flashWidthHeightElem.Width;
        props[DialogParameters.FLASH_HEIGHT] = flashWidthHeightElem.Height;
        props[DialogParameters.FLASH_AUTOPLAY] = chkFlashAutoplay.Checked;
        props[DialogParameters.FLASH_LOOP] = chkFlashLoop.Checked;
        props[DialogParameters.FLASH_MENU] = chkFlashEnableMenu.Checked;
        props[DialogParameters.FLASH_EXT] = ViewState[DialogParameters.FLASH_EXT];
        props[DialogParameters.FLASH_URL] = URLHelper.ResolveUrl(CurrentUrl);
        props[DialogParameters.FLASH_ID] = HTMLHelper.HTMLEncode(txtFlashId.Text.Trim());
        props[DialogParameters.FLASH_TITLE] = HTMLHelper.HTMLEncode(txtFlashTitle.Text.Trim());
        props[DialogParameters.FLASH_CLASS] = HTMLHelper.HTMLEncode(txtFlashClass.Text.Trim());
        props[DialogParameters.FLASH_FLASHVARS] = HTMLHelper.HTMLEncode(txtFlashVars.Text.Trim());
        props[DialogParameters.FLASH_SCALE] = (drpFlashScale.SelectedIndex > 0) ? drpFlashScale.SelectedValue : "";
        props[DialogParameters.LAST_TYPE] = MediaTypeEnum.Flash;

        string style = txtFlashStyle.Text.Trim().TrimEnd(';') + ";";
        props[DialogParameters.FLASH_STYLE] = (style != ";" ? HTMLHelper.HTMLEncode(style) : "");
        
        // Save image properties into session
        SessionHelper.SetValue("DialogSelectedParameters", props);
    }

    #endregion


    #region "Overridden methods"

    /// <summary>
    /// Loads MediaItem into the properties dialog.
    /// </summary>
    /// <param name="item">Media item</param>
    /// <param name="properties">Item properties</param>
    public override void LoadSelectedItems(MediaItem item, Hashtable properties)
    {
        if (item == null)
        {
            return;
        }
        // Display the properties
        pnlEmpty.Visible = false;
        pnlTabs.CssClass = "Dialog_Tabs";

        ViewMode = (item.MediaType == MediaTypeEnum.Unknown) ? ViewMode : item.MediaType;

        CurrentUrl = item.Url;
        OriginalUrl = item.Url;
        PermanentUrl = item.PermanentUrl;
        OriginalWidth = DefaultWidth = item.Width;
        OriginalHeight = DefaultHeight = item.Height;
            
        HistoryID = item.HistoryID;

        // Ensure default dimensions
        if (ViewMode == MediaTypeEnum.AudioVideo)
        {
            // Audio default height = 45, video = 200
            int vidDefaultHeight = (MediaHelper.IsAudio(item.Extension) ? 45 : 200);

            properties[DialogParameters.AV_WIDTH] = 300;
            properties[DialogParameters.AV_HEIGHT] = vidDefaultHeight;
        }
        else if (ViewMode == MediaTypeEnum.Flash)
        {
            properties[DialogParameters.FLASH_WIDTH] = 300;
            properties[DialogParameters.FLASH_HEIGHT] = 200;
        }

        if (properties == null)
        {
            properties = new Hashtable();
        }
        properties[DialogParameters.URL_URL] = item.Url;

        switch (ViewMode)
        {
            case MediaTypeEnum.Image:
                // Image
                properties[DialogParameters.IMG_ORIGINALWIDTH] = item.Width;
                properties[DialogParameters.IMG_ORIGINALHEIGHT] = item.Height;
                properties[DialogParameters.IMG_WIDTH] = item.Width;
                properties[DialogParameters.IMG_HEIGHT] = item.Height;
                properties[DialogParameters.IMG_MOUSEOVERWIDTH] = item.Width;
                properties[DialogParameters.IMG_MOUSEOVERHEIGHT] = item.Height;
                properties[DialogParameters.IMG_URL] = item.Url;
                properties[DialogParameters.IMG_EXT] = item.Extension;
                ViewState[DialogParameters.IMG_EXT] = item.Extension;
                break;

            case MediaTypeEnum.AudioVideo:
                // Media
                properties[DialogParameters.AV_URL] = item.Url;
                properties[DialogParameters.AV_EXT] = item.Extension;
                properties[DialogParameters.AV_CONTROLS] = true;
                ViewState[DialogParameters.AV_EXT] = item.Extension;
                break;

            case MediaTypeEnum.Flash:
                // Flash
                properties[DialogParameters.FLASH_URL] = item.Url;
                properties[DialogParameters.FLASH_EXT] = item.Extension;
                ViewState[DialogParameters.FLASH_EXT] = item.Extension;
                break;
        }

        LoadProperties(properties);
    }


    /// <summary>
    /// Loads the properties into control.
    /// </summary>
    /// <param name="properties">Collection with properties</param>
    public override void LoadItemProperties(Hashtable properties)
    {
        LoadProperties(properties);
    }


    /// <summary>
    /// Loads selected properties into the child controls and ensures displaying correct values.
    /// </summary>
    /// <param name="properties">Properties collection</param>
    public override void LoadProperties(Hashtable properties)
    {
        if (properties != null)
        {
            if (properties[DialogParameters.LAST_TYPE] != null)
            {
                // Setup view mode if necessary
                Hashtable temp = (Hashtable)properties.Clone();
                ViewMode = (MediaTypeEnum)properties[DialogParameters.LAST_TYPE];
                properties = temp;
            }

            // Display the properties
            pnlEmpty.Visible = false;
            pnlTabs.CssClass = "Dialog_Tabs";

            if ((properties[DialogParameters.LAST_TYPE] == null) || ((MediaTypeEnum)properties[DialogParameters.LAST_TYPE] == MediaTypeEnum.Image))
            {
                #region "Image general tab"

                int width = ValidationHelper.GetInteger(properties[DialogParameters.IMG_WIDTH], 0);
                int height = ValidationHelper.GetInteger(properties[DialogParameters.IMG_HEIGHT], 0);

                OriginalWidth = ValidationHelper.GetInteger(properties[DialogParameters.IMG_ORIGINALWIDTH], 0);
                OriginalHeight = ValidationHelper.GetInteger(properties[DialogParameters.IMG_ORIGINALHEIGHT], 0);

                string url = ValidationHelper.GetString(properties[DialogParameters.IMG_URL], "");

                // Image URL is missing, look at A/V/F URLs in case that item was inserted from the Web tab with manually selected media type
                if (url == "")
                {
                    url = ValidationHelper.GetString(properties[DialogParameters.AV_URL], "");
                    if (url == "")
                    {
                        url = ValidationHelper.GetString(properties[DialogParameters.FLASH_URL], "");
                        if (url == "")
                        {
                            url = ValidationHelper.GetString(properties[DialogParameters.URL_URL], "");
                            width = ValidationHelper.GetInteger(properties[DialogParameters.URL_WIDTH], 0);
                            height = ValidationHelper.GetInteger(properties[DialogParameters.URL_HEIGHT], 0);
                        }
                        else
                        {
                            width = ValidationHelper.GetInteger(properties[DialogParameters.FLASH_WIDTH], 0);
                            height = ValidationHelper.GetInteger(properties[DialogParameters.FLASH_HEIGHT], 0);
                        }
                    }
                    else
                    {
                        width = ValidationHelper.GetInteger(properties[DialogParameters.AV_WIDTH], 0);
                        height = ValidationHelper.GetInteger(properties[DialogParameters.AV_HEIGHT], 0);
                    }
                    properties[DialogParameters.IMG_EXT] = ValidationHelper.GetString(properties[DialogParameters.URL_EXT], "");
                }
                else
                {
                    OriginalUrl = url;
                }
                url = CMSDialogHelper.UpdateUrl(width, height, OriginalWidth, OriginalHeight, url, SourceType);

                MediaSource source = CMSDialogHelper.GetMediaData(url, null);
                if ((source == null) || (source.MediaType == MediaTypeEnum.Unknown))
                {
                    ItemNotSystem = true;
                }

                string imgLink = ValidationHelper.GetString(properties[DialogParameters.IMG_LINK], "");
                string imgTarget = ValidationHelper.GetString(properties[DialogParameters.IMG_TARGET], "");
                string imgBehavior = ValidationHelper.GetString(properties[DialogParameters.IMG_BEHAVIOR], "");

                bool isLink = false;
                if (!String.IsNullOrEmpty(imgLink))
                {
                    if (imgBehavior == "hover")
                    {
                        isLink = true;
                    }
                    else
                    {
                        isLink = CMSDialogHelper.IsImageLink(url, imgLink, OriginalWidth, OriginalHeight, imgTarget);
                    }
                }

                if (tabImageGeneral.Visible)
                {
                    string color = ValidationHelper.GetString(properties[DialogParameters.IMG_BORDERCOLOR], "");
                    string alt = ValidationHelper.GetString(properties[DialogParameters.IMG_ALT], "");
                    string align = ValidationHelper.GetString(properties[DialogParameters.IMG_ALIGN], "");
                    int border = -1;
                    if (properties[DialogParameters.IMG_BORDERWIDTH] != null)
                    {
                        // Remove 'px' from definition of border width
                        border = ValidationHelper.GetInteger(properties[DialogParameters.IMG_BORDERWIDTH].ToString().Replace("px", ""), -1);
                    }
                    int hspace = -1;
                    if (properties[DialogParameters.IMG_HSPACE] != null)
                    {
                        // Remove 'px' from definition of hspace
                        hspace = ValidationHelper.GetInteger(properties[DialogParameters.IMG_HSPACE].ToString().Replace("px", ""), -1);
                    }
                    int vspace = -1;
                    if (properties[DialogParameters.IMG_VSPACE] != null)
                    {
                        // Remove 'px' from definition of vspace
                        vspace = ValidationHelper.GetInteger(properties[DialogParameters.IMG_VSPACE].ToString().Replace("px", ""), -1);
                    }

                    DefaultWidth = OriginalWidth;
                    DefaultHeight = OriginalHeight;

                    widthHeightElem.Width = width;
                    widthHeightElem.Height = height;
                    txtAlt.Text = HttpUtility.HtmlDecode(alt);
                    colorElem.SelectedColor = color;
                    txtBorderWidth.Text = (border < 0 ? "" : border.ToString());
                    txtHSpace.Text = (hspace < 0 ? "" : hspace.ToString());
                    txtVSpace.Text = (vspace < 0 ? "" : vspace.ToString());

                    CurrentUrl = URLHelper.ResolveUrl(url);

                    // Initialize media file URLs
                    if (SourceType == MediaSourceEnum.MediaLibraries)
                    {
                        OriginalUrl = (string.IsNullOrEmpty(OriginalUrl) ? ValidationHelper.GetString(properties[DialogParameters.URL_DIRECT], "") : OriginalUrl);
                        PermanentUrl = (string.IsNullOrEmpty(PermanentUrl) ? ValidationHelper.GetString(properties[DialogParameters.URL_PERMANENT], "") : PermanentUrl);
                    }

                    ListItem li = drpAlign.Items.FindByValue(align);
                    if (li != null)
                    {
                        drpAlign.ClearSelection();
                        li.Selected = true;
                    }

                    ViewState[DialogParameters.IMG_EXT] = ValidationHelper.GetString(properties[DialogParameters.IMG_EXT], "");
                    ViewState[DialogParameters.IMG_SIZETOURL] = ValidationHelper.GetBoolean(properties[DialogParameters.IMG_SIZETOURL], false);
                    ViewState[DialogParameters.IMG_DIR] = ValidationHelper.GetString(properties[DialogParameters.IMG_DIR], "");
                    ViewState[DialogParameters.IMG_USEMAP] = ValidationHelper.GetString(properties[DialogParameters.IMG_USEMAP], "");
                    ViewState[DialogParameters.IMG_LANG] = ValidationHelper.GetString(properties[DialogParameters.IMG_LANG], "");
                    ViewState[DialogParameters.IMG_LONGDESCRIPTION] = ValidationHelper.GetString(properties[DialogParameters.IMG_LONGDESCRIPTION], "");
                }

                #endregion


                #region "Image link tab"

                if (tabImageLink.Visible)
                {
                    if (isLink)
                    {
                        txtLinkUrl.Text = imgLink;

                        ListItem liTarget = drpLinkTarget.Items.FindByValue(imgTarget);
                        if (liTarget != null)
                        {
                            drpLinkTarget.ClearSelection();
                            liTarget.Selected = true;
                        }
                    }
                }

                #endregion


                #region "Image advanced tab"

                if (tabImageAdvanced.Visible)
                {
                    string imgId = ValidationHelper.GetString(properties[DialogParameters.IMG_ID], "");
                    string imgTooltip = ValidationHelper.GetString(properties[DialogParameters.IMG_TOOLTIP], "");
                    string imgStyleClass = ValidationHelper.GetString(properties[DialogParameters.IMG_CLASS], "");
                    string imgStyle = ValidationHelper.GetString(properties[DialogParameters.IMG_STYLE], "");

                    txtImageAdvId.Text = HttpUtility.HtmlDecode(imgId);
                    txtImageAdvTooltip.Text = HttpUtility.HtmlDecode(imgTooltip);
                    txtImageAdvClass.Text = HttpUtility.HtmlDecode(imgStyleClass);
                    txtImageAdvStyle.Text = HttpUtility.HtmlDecode(imgStyle);
                }

                #endregion


                #region "Image behavior tab"

                if (tabImageBehavior.Visible)
                {
                    if ((!isLink) || (imgBehavior == "hover"))
                    {
                        // Process target
                        string target = !String.IsNullOrEmpty(imgBehavior) ? imgBehavior : imgTarget;
                        switch (target.ToLowerCSafe())
                        {
                            case "_self":
                                radImageSame.Checked = true;
                                break;

                            case "_blank":
                                radImageNew.Checked = true;
                                break;

                            case "hover":
                                radImageLarger.Checked = true;
                                break;

                            default:
                                radImageNone.Checked = true;
                                break;
                        }
                    }

                    // Process behavior
                    int imgBehaviorWidth = ValidationHelper.GetInteger(properties[DialogParameters.IMG_MOUSEOVERWIDTH], 0);
                    int imgBehaviorHeight = ValidationHelper.GetInteger(properties[DialogParameters.IMG_MOUSEOVERHEIGHT], 0);
                    if (imgBehaviorWidth == 0)
                    {
                        imgBehaviorWidth = width;
                    }
                    if (imgBehaviorHeight == 0)
                    {
                        imgBehaviorHeight = height;
                    }
                    if (imgBehaviorWidth > 0)
                    {
                        imgWidthHeightElem.Width = imgBehaviorWidth;
                    }

                    if (imgBehaviorHeight > 0)
                    {
                        imgWidthHeightElem.Height = imgBehaviorHeight;
                    }

                    // Add image tag for preview
                    LoadImagePreview();
                }

                #endregion
            }

            if ((properties[DialogParameters.LAST_TYPE] == null) || ((MediaTypeEnum)properties[DialogParameters.LAST_TYPE] == MediaTypeEnum.AudioVideo))
            {
                #region "Video general tab"

                if (tabVideoGeneral.Visible)
                {
                    string vidExt = ValidationHelper.GetString(properties[DialogParameters.AV_EXT], "");

                    int vidWidth = ValidationHelper.GetInteger(properties[DialogParameters.AV_WIDTH], 300);
                    int vidHeight = ValidationHelper.GetInteger(properties[DialogParameters.AV_HEIGHT], 200);

                    bool vidAutoplay = ValidationHelper.GetBoolean(properties[DialogParameters.AV_AUTOPLAY], false);
                    bool vidLoop = ValidationHelper.GetBoolean(properties[DialogParameters.AV_LOOP], false);
                    bool vidControls = ValidationHelper.GetBoolean(properties[DialogParameters.AV_CONTROLS], true);
                    string vidUrl = ValidationHelper.GetString(properties[DialogParameters.AV_URL], "");

                    DefaultWidth = vidWidth;
                    DefaultHeight = vidHeight;

                    vidWidthHeightElem.Width = vidWidth;
                    vidWidthHeightElem.Height = vidHeight;
                    chkVidAutoPlay.Checked = vidAutoplay;
                    chkVidLoop.Checked = vidLoop;
                    chkVidShowControls.Checked = vidControls;

                    CurrentUrl = URLHelper.ResolveUrl(vidUrl);

                    // Initialize media file URLs
                    if (SourceType == MediaSourceEnum.MediaLibraries)
                    {
                        OriginalUrl = (string.IsNullOrEmpty(OriginalUrl) ? ValidationHelper.GetString(properties[DialogParameters.URL_DIRECT], "") : OriginalUrl);
                        PermanentUrl = (string.IsNullOrEmpty(PermanentUrl) ? ValidationHelper.GetString(properties[DialogParameters.URL_PERMANENT], "") : PermanentUrl);
                    }

                    ViewState[DialogParameters.AV_EXT] = vidExt;

                    LoadVideoPreview();
                }

                #endregion
            }

            if ((properties[DialogParameters.LAST_TYPE] == null) || ((MediaTypeEnum)properties[DialogParameters.LAST_TYPE] == MediaTypeEnum.Flash))
            {
                #region "Flash general tab"

                if (tabFlashGeneral.Visible)
                {
                    int flashWidth = ValidationHelper.GetInteger(properties[DialogParameters.FLASH_WIDTH], 300);
                    int flashHeight = ValidationHelper.GetInteger(properties[DialogParameters.FLASH_HEIGHT], 200);
                    bool flashAutoplay = ValidationHelper.GetBoolean(properties[DialogParameters.FLASH_AUTOPLAY], false);
                    bool flashLoop = ValidationHelper.GetBoolean(properties[DialogParameters.FLASH_LOOP], false);
                    bool flashEnableMenu = ValidationHelper.GetBoolean(properties[DialogParameters.FLASH_MENU], false);
                    string flashExt = ValidationHelper.GetString(properties[DialogParameters.FLASH_EXT], "");
                    if (String.IsNullOrEmpty(flashExt))
                    {
                        // Try to get flash extension from url if not found
                        flashExt = ValidationHelper.GetString(properties[DialogParameters.URL_EXT], "");
                    }
                    string flashUrl = ValidationHelper.GetString(properties[DialogParameters.FLASH_URL], "");

                    // Update extension in URL
                    flashUrl = URLHelper.UpdateParameterInUrl(flashUrl, "ext", "." + flashExt.TrimStart('.'));

                    DefaultWidth = flashWidth;
                    DefaultHeight = flashHeight;

                    flashWidthHeightElem.Width = flashWidth;
                    flashWidthHeightElem.Height = flashHeight;
                    chkFlashAutoplay.Checked = flashAutoplay;
                    chkFlashLoop.Checked = flashLoop;
                    chkFlashEnableMenu.Checked = flashEnableMenu;

                    CurrentUrl = URLHelper.ResolveUrl(flashUrl);

                    // Initialize media file URLs
                    if (SourceType == MediaSourceEnum.MediaLibraries)
                    {
                        OriginalUrl = (string.IsNullOrEmpty(OriginalUrl) ? ValidationHelper.GetString(properties[DialogParameters.URL_DIRECT], "") : OriginalUrl);
                        PermanentUrl = (string.IsNullOrEmpty(PermanentUrl) ? ValidationHelper.GetString(properties[DialogParameters.URL_PERMANENT], "") : PermanentUrl);
                    }

                    ViewState[DialogParameters.FLASH_EXT] = flashExt;
                }

                #endregion


                #region "Flash advanced tab"

                if (tabFlashAdvanced.Visible)
                {
                    string flashId = ValidationHelper.GetString(properties[DialogParameters.FLASH_ID], "");
                    string flashTitle = ValidationHelper.GetString(properties[DialogParameters.FLASH_TITLE], "");
                    string flashClass = ValidationHelper.GetString(properties[DialogParameters.FLASH_CLASS], "");
                    string flashStyle = ValidationHelper.GetString(properties[DialogParameters.FLASH_STYLE], "");
                    string flashScale = ValidationHelper.GetString(properties[DialogParameters.FLASH_SCALE], "");
                    string flashVars = ValidationHelper.GetString(properties[DialogParameters.FLASH_FLASHVARS], "");

                    txtFlashId.Text = HttpUtility.HtmlDecode(flashId);
                    txtFlashTitle.Text = HttpUtility.HtmlDecode(flashTitle);
                    txtFlashClass.Text = HttpUtility.HtmlDecode(flashClass);
                    txtFlashStyle.Text = HttpUtility.HtmlDecode(flashStyle);
                    txtFlashVars.Text = HttpUtility.HtmlDecode(flashVars);

                    ListItem liFlashScale = drpFlashScale.Items.FindByValue(flashScale);
                    if (liFlashScale != null)
                    {
                        drpFlashScale.ClearSelection();
                        liFlashScale.Selected = true;
                    }
                    LoadFlashPreview();
                }

                #endregion
            }


            #region "General items"

            EditorClientID = ValidationHelper.GetString(properties[DialogParameters.EDITOR_CLIENTID], "");

            #endregion
        }
    }


    /// <summary>
    /// Returns all parameters of the selected item as name – value collection.
    /// </summary>
    public override Hashtable GetItemProperties()
    {
        Hashtable retval = new Hashtable();


        #region "Image general tab"

        if (tabImageGeneral.Visible)
        {
            string url = CurrentUrl.Trim();
            bool sizeToUrl = ValidationHelper.GetBoolean(ViewState[DialogParameters.IMG_SIZETOURL], false);

            if ((widthHeightElem.Width < DefaultWidth) || (DefaultWidth == 0))
            {
                retval[DialogParameters.IMG_WIDTH] = widthHeightElem.Width;
                if (sizeToUrl)
                {
                    url = URLHelper.AddParameterToUrl(url, "width", widthHeightElem.Width.ToString());
                }
            }
            if ((widthHeightElem.Height < DefaultHeight) || (DefaultWidth == 0))
            {
                retval[DialogParameters.IMG_HEIGHT] = widthHeightElem.Height;
                if (sizeToUrl)
                {
                    url = URLHelper.AddParameterToUrl(url, "height", widthHeightElem.Height.ToString());
                }
            }

            retval[DialogParameters.IMG_URL] = URLHelper.ResolveUrl(url);
            retval[DialogParameters.IMG_SIZETOURL] = sizeToUrl;
            retval[DialogParameters.IMG_BORDERCOLOR] = colorElem.SelectedColor;
            retval[DialogParameters.IMG_ALT] = txtAlt.Text.Trim().Replace("%", "%25");
            retval[DialogParameters.IMG_ALIGN] = drpAlign.SelectedValue;
            retval[DialogParameters.IMG_BORDERWIDTH] = ValidationHelper.GetInteger(txtBorderWidth.Text, -1);
            retval[DialogParameters.IMG_HSPACE] = ValidationHelper.GetInteger(txtHSpace.Text, -1);
            retval[DialogParameters.IMG_VSPACE] = ValidationHelper.GetInteger(txtVSpace.Text, -1);
            retval[DialogParameters.IMG_EXT] = ValidationHelper.GetString(ViewState[DialogParameters.IMG_EXT], "");
            retval[DialogParameters.IMG_DIR] = ValidationHelper.GetString(ViewState[DialogParameters.IMG_DIR], "");
            retval[DialogParameters.IMG_USEMAP] = ValidationHelper.GetString(ViewState[DialogParameters.IMG_USEMAP], "");
            retval[DialogParameters.IMG_LANG] = ValidationHelper.GetString(ViewState[DialogParameters.IMG_LANG], "");
            retval[DialogParameters.IMG_LONGDESCRIPTION] = ValidationHelper.GetString(ViewState[DialogParameters.IMG_LONGDESCRIPTION], "");
            retval[DialogParameters.IMG_ORIGINALWIDTH] = OriginalWidth;
            retval[DialogParameters.IMG_ORIGINALHEIGHT] = OriginalHeight;
        }

        #endregion


        #region "Image link tab"

        if (tabImageLink.Visible)
        {
            string url = txtLinkUrl.Text.Trim();
            retval[DialogParameters.IMG_LINK] = URLHelper.IsAnchor(url) ? url : URLHelper.ResolveUrl(url);

            if (drpLinkTarget.SelectedIndex > 0)
            {
                retval[DialogParameters.IMG_TARGET] = drpLinkTarget.SelectedValue;
            }
            else
            {
                retval[DialogParameters.IMG_TARGET] = "";
            }
        }

        #endregion


        #region "Image advanced tab"

        if (tabImageAdvanced.Visible)
        {
            string style = txtImageAdvStyle.Text.Trim().TrimEnd(';').Replace("%", "%25") + ";";
            retval[DialogParameters.IMG_ID] = HTMLHelper.HTMLEncode(txtImageAdvId.Text.Trim().Replace("%", "%25"));
            retval[DialogParameters.IMG_TOOLTIP] = txtImageAdvTooltip.Text.Trim().Replace("%", "%25");
            retval[DialogParameters.IMG_STYLE] = (style != ";" ? HTMLHelper.HTMLEncode(style) : "");
            retval[DialogParameters.IMG_CLASS] = HTMLHelper.HTMLEncode(txtImageAdvClass.Text.Trim().Replace("%", "%25"));
        }

        #endregion


        #region "Image behavior tab"

        if (tabImageBehavior.Visible)
        {
            // Only if link url is empty
            if (String.IsNullOrEmpty(txtLinkUrl.Text.Trim()))
            {
                string url = CurrentUrl.Trim();
                url = URLHelper.RemoveParameterFromUrl(url, "width");
                url = URLHelper.RemoveParameterFromUrl(url, "height");
                retval[DialogParameters.IMG_MOUSEOVERWIDTH] = "";
                retval[DialogParameters.IMG_MOUSEOVERHEIGHT] = "";
                if (radImageNone.Checked)
                {
                    retval[DialogParameters.IMG_BEHAVIOR] = "";
                }
                else if (radImageNew.Checked)
                {
                    retval[DialogParameters.IMG_BEHAVIOR] = "_blank";
                    retval[DialogParameters.IMG_LINK] = url;
                }
                else if (radImageSame.Checked)
                {
                    retval[DialogParameters.IMG_BEHAVIOR] = "_self";
                    retval[DialogParameters.IMG_LINK] = url;
                }
            }
            if (radImageLarger.Checked)
            {
                // Unresolve URL for Inline control
                retval[DialogParameters.IMG_URL] = URLHelper.UnResolveUrl(retval[DialogParameters.IMG_URL].ToString(), SystemContext.ApplicationPath);
                retval[DialogParameters.IMG_BEHAVIOR] = "hover";
                retval[DialogParameters.IMG_MOUSEOVERWIDTH] = imgWidthHeightElem.Width;
                retval[DialogParameters.IMG_MOUSEOVERHEIGHT] = imgWidthHeightElem.Height;
                retval[DialogParameters.OBJECT_TYPE] = "image";
            }
        }

        #endregion


        #region "Video general tab"

        if (tabVideoGeneral.Visible)
        {
            retval[DialogParameters.AV_WIDTH] = vidWidthHeightElem.Width;
            retval[DialogParameters.AV_HEIGHT] = vidWidthHeightElem.Height;
            retval[DialogParameters.AV_AUTOPLAY] = chkVidAutoPlay.Checked;
            retval[DialogParameters.AV_LOOP] = chkVidLoop.Checked;
            retval[DialogParameters.AV_CONTROLS] = chkVidShowControls.Checked;
            retval[DialogParameters.AV_EXT] = ViewState[DialogParameters.AV_EXT];
            retval[DialogParameters.AV_URL] = URLHelper.ResolveUrl(CurrentUrl);
            retval[DialogParameters.OBJECT_TYPE] = "audiovideo";
        }

        #endregion


        #region "Flash general tab"

        if (tabFlashGeneral.Visible)
        {
            retval[DialogParameters.FLASH_WIDTH] = flashWidthHeightElem.Width;
            retval[DialogParameters.FLASH_HEIGHT] = flashWidthHeightElem.Height;
            retval[DialogParameters.FLASH_AUTOPLAY] = chkFlashAutoplay.Checked;
            retval[DialogParameters.FLASH_LOOP] = chkFlashLoop.Checked;
            retval[DialogParameters.FLASH_MENU] = chkFlashEnableMenu.Checked;
            retval[DialogParameters.FLASH_EXT] = ViewState[DialogParameters.FLASH_EXT];
            retval[DialogParameters.FLASH_URL] = URLHelper.ResolveUrl(CurrentUrl);
            retval[DialogParameters.OBJECT_TYPE] = "flash";
        }

        #endregion


        #region "Flash advanced tab"

        if (tabFlashAdvanced.Visible)
        {
            string style = txtFlashStyle.Text.Trim().TrimEnd(';').Replace("%", "%25") + ";";
            retval[DialogParameters.FLASH_ID] = HTMLHelper.HTMLEncode(txtFlashId.Text.Trim().Replace("%", "%25"));
            retval[DialogParameters.FLASH_TITLE] = HTMLHelper.HTMLEncode(txtFlashTitle.Text.Trim().Replace("%", "%25"));
            retval[DialogParameters.FLASH_STYLE] = (style != ";" ? HTMLHelper.HTMLEncode(style) : "");
            retval[DialogParameters.FLASH_CLASS] = HTMLHelper.HTMLEncode(txtFlashClass.Text.Trim().Replace("%", "%25"));
            retval[DialogParameters.FLASH_FLASHVARS] = HTMLHelper.HTMLEncode(txtFlashVars.Text.Trim().Replace("%", "%25"));
            if (drpFlashScale.SelectedIndex > 0)
            {
                retval[DialogParameters.FLASH_SCALE] = drpFlashScale.SelectedValue;
            }
            else
            {
                retval[DialogParameters.FLASH_SCALE] = "";
            }
        }

        #endregion


        #region "General items"

        retval[DialogParameters.EDITOR_CLIENTID] = (String.IsNullOrEmpty(EditorClientID) ? "" : EditorClientID.Replace("%", "%25"));

        #endregion


        #region "Unresolve URL for in-line controls"

        bool isAudioVideo = (ValidationHelper.GetString(retval[DialogParameters.AV_URL], "") != "");
        bool isFlash = (ValidationHelper.GetString(retval[DialogParameters.FLASH_URL], "") != "");
        bool isInlineImage = ((ValidationHelper.GetString(retval[DialogParameters.IMG_LINK], "") != "") &&
                              (ValidationHelper.GetString(retval[DialogParameters.IMG_BEHAVIOR], "") == "hover"));

        if (isAudioVideo || isFlash || isInlineImage)
        {
            CurrentUrl = URLHelper.UnResolveUrl(CurrentUrl, SystemContext.ApplicationPath);

            if (isAudioVideo)
            {
                retval[DialogParameters.AV_URL] = CurrentUrl;
            }
            else if (isFlash)
            {
                retval[DialogParameters.FLASH_URL] = CurrentUrl;
            }
            else
            {
                retval[DialogParameters.IMG_URL] = CurrentUrl;
            }
        }

        #endregion


        return retval;
    }


    /// <summary>
    /// Validates all the user input.
    /// </summary>
    public override bool Validate()
    {
        string errorMessage = "";

        switch (ViewMode)
        {
            case MediaTypeEnum.AudioVideo:

                if (!vidWidthHeightElem.Validate())
                {
                    errorMessage += " " + GetString("dialogs.image.invalidsize");
                }

                errorMessage = errorMessage.Trim();
                break;

            case MediaTypeEnum.Flash:

                if (!flashWidthHeightElem.Validate())
                {
                    errorMessage += " " + GetString("dialogs.image.invalidsize");
                }
                errorMessage = errorMessage.Trim();
                break;

            case MediaTypeEnum.Image:

                errorMessage += ValidateInt(txtBorderWidth.Text, "dialogs.image.invalidborder");
                errorMessage += ValidateInt(txtHSpace.Text, "dialogs.image.invalidhspace");
                errorMessage += ValidateInt(txtVSpace.Text, "dialogs.image.invalidvspace");
                if (!widthHeightElem.Validate())
                {
                    errorMessage += " " + GetString("dialogs.image.invalidsize");
                }
                if ((colorElem.ColorTextBox.Text.Trim() != "") && !ValidationHelper.IsColor(colorElem.ColorTextBox.Text.Trim()))
                {
                    errorMessage += " " + GetString("dialogs.image.invalidcolor");
                }

                errorMessage = errorMessage.Trim();
                break;
        }

        if (errorMessage != "")
        {
            switch (ViewMode)
            {
                case MediaTypeEnum.Image:
                    LoadImagePreview();
                    break;

                case MediaTypeEnum.AudioVideo:
                    LoadVideoPreview();
                    break;

                case MediaTypeEnum.Flash:
                    LoadFlashPreview();
                    break;
            }

            ScriptHelper.RegisterStartupScript(Page, typeof(Page), "ImagePropertiesError", ScriptHelper.GetAlertScript(errorMessage));

            return false;
        }

        return true;
    }


    /// <summary>
    /// Validates given filed as Int.
    /// </summary>
    private string ValidateInt(string text, string errorMessage)
    {
        string trimmedText = text.Trim();
        if ((trimmedText != "") && !ValidationHelper.IsInteger(trimmedText))
        {
            return " " + GetString(errorMessage);
        }
        return "";
    }


    /// <summary>
    /// Clears the properties form.
    /// </summary>
    public override void ClearProperties(bool hideProperties)
    {
        // Hide the properties
        pnlEmpty.Visible = hideProperties;
        pnlTabs.CssClass = (hideProperties ? "DialogElementHidden" : "Dialog_Tabs");

        pnlTabs.SelectedTabIndex = 0;

        imgWidthHeightElem.Height = 0;
        imgWidthHeightElem.Width = 0;
        flashWidthHeightElem.Height = 0;
        flashWidthHeightElem.Width = 0;
        vidWidthHeightElem.Height = 0;
        vidWidthHeightElem.Width = 0;
        widthHeightElem.Height = 0;
        widthHeightElem.Width = 0;

        videoPreview.Url = "";
        imagePreview.URL = "";
        flashPreview.Url = "";

        txtAlt.Text = "";
        txtBorderWidth.Text = "";
        txtFlashClass.Text = "";
        txtFlashId.Text = "";
        txtFlashStyle.Text = "";
        txtFlashTitle.Text = "";
        txtHSpace.Text = "";
        txtImageAdvClass.Text = "";
        txtImageAdvId.Text = "";
        txtImageAdvStyle.Text = "";
        txtImageAdvTooltip.Text = "";
        txtLinkUrl.Text = "";
        txtVSpace.Text = "";
        txtUrl.Text = "";
        txtFlashUrl.Text = "";
        txtVidUrl.Text = "";

        drpAlign.SelectedIndex = 0;
        drpFlashScale.SelectedIndex = 0;
        drpLinkTarget.SelectedIndex = 0;
    }

    #endregion
}