<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductFilter.ascx.cs"
    Inherits="CMSModules_Ecommerce_Controls_UI_ProductFilter" %>
<%@ Register Src="~/CMSModules/Ecommerce/FormControls/PublicStatusSelector.ascx"
    TagName="PublicStatusSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Ecommerce/FormControls/InternalStatusSelector.ascx"
    TagName="InternalStatusSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Ecommerce/FormControls/ManufacturerSelector.ascx"
    TagName="ManufacturerSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Ecommerce/FormControls/SupplierSelector.ascx" TagName="SupplierSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Ecommerce/FormControls/DepartmentSelector.ascx" TagName="DepartmentSelector"
    TagPrefix="cms" %>
<%@ Register TagPrefix="cms" TagName="SelectProductType" Src="~/CMSModules/Ecommerce/FormControls/SelectProductType.ascx" %>
<asp:Panel ID="pnlFilter" runat="server" DefaultButton="btnFilter">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblName" runat="server" EnableViewState="false"
                    ResourceString="com.sku.ProductNameOrNumber" DisplayColon="true" />
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSTextBox ID="txtNameOrNumber" runat="server" MaxLength="450" EnableViewState="false" />
            </div>
        </div>
        <asp:PlaceHolder ID="plcAdvancedFilterType" runat="server">
            <asp:PlaceHolder ID="plcSite" runat="server">
                <div class="form-group">
                    <div class="filter-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblSite" runat="server" EnableViewState="false"
                            ResourceString="com.sku.ProductsSite" DisplayColon="true" />
                    </div>
                    <div class="filter-form-value-cell">
                        <cms:SiteOrGlobalSelector runat="server" ID="siteElem" IsLiveSite="false" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblDepartment" runat="server" EnableViewState="false"
                        ResourceString="com.sku.departmentid" DisplayColon="true" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:DepartmentSelector runat="server" ID="departmentElem" AddAllMyRecord="true"
                        AddAllItemsRecord="false" IsLiveSite="false" AddNoneRecord="false" AddWithoutDepartmentRecord="true"
                        UseNameForSelection="false" />
                </div>
            </div>
            <asp:PlaceHolder ID="plcAdvancedDocumentType" runat="server">
                <div class="form-group">
                    <div class="filter-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblProperties" runat="server" ResourceString="com.sku.properties"
                            DisplayColon="true" />
                    </div>
                    <div class="filter-form-value-cell">
                        <cms:CMSDropDownList runat="server" ID="drpDocTypes" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblType" runat="server" ResourceString="com.sku.producttype"
                        DisplayColon="true" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:SelectProductType runat="server" ID="selectProductTypeElem" AllowAll="true"
                        AllItemResourceString="General.SelectAnything" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcAdvancedFilterGeneral" runat="server">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblManufacturer" runat="server" EnableViewState="false"
                        ResourceString="com.sku.manufacturerid" DisplayColon="true" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:ManufacturerSelector ID="manufacturerElem" runat="server" AddAllItemsRecord="true"
                        AddNoneRecord="true" UseNameForSelection="false" DisplayOnlyEnabled="false" EnsureSelectedItem="true" />
                </div>
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblSupplier" runat="server" EnableViewState="false"
                        ResourceString="com.sku.supplierid" DisplayColon="true" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:SupplierSelector ID="supplierElem" runat="server" AddAllItemsRecord="true" AddNoneRecord="true"
                        UseNameForSelection="false" DisplayOnlyEnabled="false" EnsureSelectedItem="true" />
                </div>
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblStoreStatus" runat="server" EnableViewState="false"
                        ResourceString="com.sku.publicstatusid" DisplayColon="true" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:PublicStatusSelector runat="server" ID="publicStatusElem" AddAllItemsRecord="true"
                        AddNoneRecord="true" IsLiveSite="false" UseNameForSelection="false" DisplayOnlyEnabled="false" EnsureSelectedItem="true" />
                </div>
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblInternalStatus" runat="server"
                        EnableViewState="false" ResourceString="com.sku.internalstatusid" DisplayColon="true" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:InternalStatusSelector runat="server" ID="internalStatusElem" AddAllItemsRecord="true"
                        AddNoneRecord="true" UseNameForSelection="false" DisplayOnlyEnabled="false" EnsureSelectedItem="true" />
                </div>
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblAllowForSale" runat="server" EnableViewState="false"
                        ResourceString="com.sku.enabled" DisplayColon="true" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:CMSDropDownList runat="server" ID="ddlAllowForSale" />
                </div>
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblNeedsShipping" runat="server"
                        EnableViewState="false" ResourceString="com.sku.needsshipping" DisplayColon="true" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:CMSDropDownList runat="server" ID="ddlNeedsShipping" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group form-group-buttons">
            <div class="filter-form-label-cell">
                <asp:PlaceHolder ID="plcAdvancedFilter" runat="server">
                    <div>
                        <asp:LinkButton ID="lnkShowSimpleFilter" runat="server" OnClick="lnkShowSimpleFilter_Click" CssClass="simple-advanced-link" />
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plcSimpleFilter" runat="server">
                    <div>
                        <asp:LinkButton ID="lnkShowAdvancedFilter" runat="server" OnClick="lnkShowAdvancedFilter_Click" CssClass="simple-advanced-link" />
                    </div>
                </asp:PlaceHolder>
            </div>
            <div class="filter-form-buttons-cell-narrow">
                <cms:LocalizedButton ID="btnReset" runat="server" ButtonStyle="Default" EnableViewState="false" />
                <cms:CMSButton ID="btnFilter" runat="server" ButtonStyle="Primary" EnableViewState="false" />
            </div>
        </div>
    </div>
</asp:Panel>
