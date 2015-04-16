using System;
using System.Web.UI;

using CMS.Base;
using CMS.Controls;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_WebParts_WebPartProperties_nested : CMSWebPartPropertiesPage
{
    #region "Properties"

    /// <summary>
    /// Nested web part key
    /// </summary>
    protected string NestedKey
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether datasource was changed
    /// </summary>
    private bool DataSourceChanged
    {
        get
        {
            return selWebPart.DropDownListControl.UniqueID.EqualsCSafe(CMSHttpContext.Current.Request.Form[Page.postEventSourceID], true);
        }
    }


    #endregion


    #region "Methods"

    /// <summary>
    /// PreInit event handler.
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        CurrentMaster.BodyClass += " WebpartProperties";
    }


    /// <summary>
    /// Init event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        EnsureChildControls();

        base.OnInit(e);

        // Check permissions for web part properties UI
        if (PortalContext.ViewMode.IsWireframe())
        {
            // Check wireframes permissions
        }
        else
        {
            // Check design mode permissions
            var currentUser = MembershipContext.AuthenticatedUser;
            if (!currentUser.IsAuthorizedPerUIElement("CMS.Design", "WebPartProperties.General"))
            {
                RedirectToUIElementAccessDenied("CMS.Design", "WebPartProperties.General");
            }
        }

        NestedKey = QueryHelper.GetString("nestedkey", "");

        switch (NestedKey.ToLower())
        {
            case "datasource":
                selWebPart.WhereCondition = "ObjectType = 'webpartcategory' OR WebPartType = " + (int)WebPartTypeEnum.DataSource;
                break;

            case "viewer":
                selWebPart.WhereCondition = "ObjectType = 'webpartcategory' OR WebPartType = " + (int)WebPartTypeEnum.BasicViewer;
                break;
        }

        selWebPart.DropDownListControl.AutoPostBack = true;

        lblNested.ResourceString = "WebPartProperties.Nested." + NestedKey;

        CurrentMaster.MessagesPlaceHolder.OffsetY = 10;

        // Initialize the control
        webPartProperties.AliasPath = aliasPath;
        webPartProperties.CultureCode = cultureCode;
        webPartProperties.WebPartID = webpartId;
        webPartProperties.ZoneID = zoneId;
        webPartProperties.InstanceGUID = instanceGuid;
        webPartProperties.PageTemplateID = templateId;
        webPartProperties.IsNewWebPart = isNew;

        webPartProperties.Position = position;
        webPartProperties.PositionLeft = positionLeft;
        webPartProperties.PositionTop = positionTop;

        webPartProperties.IsNewVariant = isNewVariant;
        webPartProperties.VariantID = variantId;
        webPartProperties.ZoneVariantID = zoneVariantId;
        webPartProperties.VariantMode = variantMode;

        if (!RequestHelper.IsPostBack())
        {
            // Preselect the data source type
            if (webpartId != "")
            {
                // Get pageinfo
                var pi = GetPageInfo(aliasPath, templateId, cultureCode);
                if (pi == null)
                {
                    RedirectToInformation(GetString("WebPartProperties.WebPartNotFound"));
                    return;
                }

                // Get page template
                var pti = pi.UsedPageTemplateInfo;
                if ((pti != null) && ((pti.TemplateInstance != null)))
                {
                    var webPart = pti.TemplateInstance.GetWebPart(instanceGuid, zoneVariantId, variantId) ?? pti.TemplateInstance.GetWebPart(webpartId);
                    var nested = webPart.NestedWebParts[NestedKey];
                    if (nested != null)
                    {
                        var wpi = WebPartInfoProvider.GetWebPartInfo(nested.WebPartType);
                        if (wpi != null)
                        {
                            selWebPart.Value = wpi.WebPartID;
                        }
                    }
                }
            }
        }

        // Hide web part properties if no data source selected
        int dsId = ValidationHelper.GetInteger(CMSHttpContext.Current.Request.Form[selWebPart.DropDownListControl.UniqueID], 0);
        if (dsId <= 0)
        {
            webPartProperties.StopProcessing = true;
            webPartProperties.Visible = false;
            webPartProperties.EnableViewState = false;
            ShowInformation(GetString("WebPartProperties.NestedSelect." + NestedKey));
        }
        else
        {
            webPartProperties.NestedWebPartID = dsId;
            webPartProperties.NestedWebPartKey = NestedKey;
            webPartProperties.Visible = true;
        }

        // Avoid mismatch viewstate issue after change
        if (DataSourceChanged)
        {
            webPartProperties.EnableViewState = false;
        }

        webPartProperties.LoadData();
        
    }


    /// <summary>
    /// Load event handler.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Enable viewstate after form change
        if (RequestHelper.IsPostBack() && DataSourceChanged)
        {
            webPartProperties.EnableViewState = true;
        }

        CurrentMaster.DisplaySiteSelectorPanel = true;

        // Register the OnSave event handler
        FramesManager.OnSave += (sender, args) => { return webPartProperties.OnSave(); };
    }

    #endregion
}