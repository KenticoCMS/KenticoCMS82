<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Development_DocumentTypes_DocumentType_Edit_Ecommerce"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Page Type Edit - Ecommerce"
    CodeFile="DocumentType_Edit_Ecommerce.aspx.cs" %>
<%@ Import Namespace="CMS.FormEngine" %>

<%@ Register Src="~/CMSModules/Ecommerce/FormControls/DepartmentSelector.ascx" TagName="DepartmentSelector"
    TagPrefix="cms" %>
<%@ Register TagPrefix="cms" TagName="SelectProductType" Src="~/CMSModules/Ecommerce/FormControls/SelectProductType.ascx" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">    
    <cms:CMSPanel runat="server" ID="pnlProductSection" ShortID="pp">
        <cms:LocalizedHeading runat="server" ID="headParelSection" Level="4" EnableViewState="false" ResourceString="com.documentproductrelation" />
        <cms:CMSCheckBox runat="server" ID="chkIsProduct" ResourceString="DocType.Ecommerce.IsProduct"
            CssClass="InfoLabel" AutoPostBack="true" />
        <cms:CMSCheckBox runat="server" ID="chkIsProductSection" ResourceString="DocType.Ecommerce.IsProductSection"
            CssClass="InfoLabel" />
    </cms:CMSPanel>
    <cms:CMSPanel runat="server" ID="pnlProductTypeProperties" ShortID="pr">
        <cms:CMSPanel runat="server" ID="pnlFieldsMappings" ShortID="p">
            <cms:LocalizedHeading runat="server" ID="headFieldsMapping" Level="4" EnableViewState="false" ResourceString="com.fieldsmapping" />               
            <cms:LocalizedLabel runat="server" ID="lblMappingWarning" CssClass="InfoLabel" EnableViewState="false"
                ResourceString="DocType.Ecommerce.mappingwarning" />                        
            <cms:LocalizedLabel runat="server" ID="lblTitle" CssClass="InfoLabel" EnableViewState="false"
                ResourceString="DocType.Ecommerce.lblTitle" />                        
                                    
            <div class="form-horizontal">
                <div class="editing-form-category">
                    <cms:LocalizedHeading runat="server" ID="headGeneral" Level="5" EnableViewState="false" ResourceString="general.general" class="editing-form-category-caption" />                            					
                    <div class="editing-form-category-fields">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblName" EnableViewState="false" ResourceString="DocType.Ecommerce.lblName" DisplayColon="true"/>
                            </div>
                            <div class="editing-form-value-cell">
                            <cms:CMSDropDownList ID="drpName" runat="server" CssClass="DropDownField" />
                            </div>
                        </div>
                                
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblPrice" EnableViewState="false" ResourceString="DocType.Ecommerce.lblPrice" DisplayColon="true" />
                            </div>
                            <div class="editing-form-value-cell">
                            <cms:CMSDropDownList ID="drpPrice" runat="server" CssClass="DropDownField" />
                            </div>
                        </div>
                                
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblShortDescription" EnableViewState="false" ResourceString="DocType.Ecommerce.lblShortDescription" DisplayColon="true" />
                            </div>
                            <div class="editing-form-value-cell">
                            <cms:CMSDropDownList ID="drpShortDescription" runat="server" CssClass="DropDownField" />
                            </div>
                        </div>
                                
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblDescription" EnableViewState="false" ResourceString="DocType.Ecommerce.lblDescription" DisplayColon="true" />
                            </div>
                            <div class="editing-form-value-cell">
                            <cms:CMSDropDownList ID="drpDescription" runat="server" CssClass="DropDownField" />
                            </div>
                        </div>
                                
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblImage" EnableViewState="false" ResourceString="DocType.Ecommerce.lblImage" DisplayColon="true" />
                            </div>
                            <div class="editing-form-value-cell">
                            <cms:CMSDropDownList ID="drpImage" runat="server" CssClass="DropDownField" />
                            </div>
                        </div>                                
                    </div>                        
                </div>

                <div class="editing-form-category">
                    <cms:LocalizedHeading runat="server" ID="LocalizedHeading1" Level="5" EnableViewState="false" ResourceString="com.product.dimensions" class="editing-form-category-caption" />
                    <div class="editing-form-category-fields">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblWeight" EnableViewState="false" ResourceString="DocType.Ecommerce.lblWeight" DisplayColon="true" />
                            </div>
                            <div class="editing-form-value-cell">
                            <cms:CMSDropDownList ID="drpWeight" runat="server" CssClass="DropDownField" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblHeight" EnableViewState="false" ResourceString="DocType.Ecommerce.lblHeight" DisplayColon="true" />
                            </div>
                            <div class="editing-form-value-cell">
                            <cms:CMSDropDownList ID="drpHeight" runat="server" CssClass="DropDownField" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblWidth" EnableViewState="false" ResourceString="DocType.Ecommerce.lblWidth" DisplayColon="true" />
                            </div>
                            <div class="editing-form-value-cell">
                            <cms:CMSDropDownList ID="drpWidth" runat="server" CssClass="DropDownField" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblDepth" EnableViewState="false" ResourceString="DocType.Ecommerce.lblDepth" DisplayColon="true" />
                            </div>
                            <div class="editing-form-value-cell">
                            <cms:CMSDropDownList ID="drpDepth" runat="server" CssClass="DropDownField" />
                            </div>
                        </div>
                    </div>                        
                </div>            
                
                <asp:PlaceHolder runat="server" ID="plcCustom">
                    <asp:Repeater runat="server" ID="repCustomProperties" OnItemCreated="repCustomPropsItemCreated">
                        <HeaderTemplate>
                            <div class="editing-form-category">
                            <cms:LocalizedHeading runat="server" Level="5" EnableViewState="false" ResourceString="com.productedit.customproperties"/>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" runat="server" Text="<%# PrepareCaption((FormFieldInfo)Container.DataItem) %>" EnableViewState="false" DisplayColon="true" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <cms:CMSDropDownList ID="drpCustomProps" runat="server" CssClass="DropDownField" />
                                </div>
                            </div>
                        </ItemTemplate>
                        <FooterTemplate>
                            </div>
                        </FooterTemplate>
                    </asp:Repeater>
                </asp:PlaceHolder>
            </div>
         </cms:CMSPanel>
     
            
        <cms:CMSPanel runat="server" ID="pnlAutoCreation" ShortID="pa">
            <cms:LocalizedHeading runat="server" ID="headCustom" Level="4" EnableViewState="false" ResourceString="com.productautocreation" />
                            
            <div class="form-horizontal">
                <div class="editing-form-category">
                    <div class="editing-form-category-fields">
                                 
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblDepartments" EnableViewState="false" ResourceString="DocType.Ecommerce.lblDepartments" />     
                            </div>
                            <div class="editing-form-value-cell">
                            <cms:DepartmentSelector runat="server" ID="departmentElem" DropDownListMode="false"
                                UseNameForSelection="false" ShowAllSites="true" AddNoneRecord="true" IsLiveSite="false" />
                            </div>
                        </div>         
                        <%-- Default product type --%>
                        <asp:PlaceHolder runat="server" ID="plcDefaultProductType">                        
                            <div class="form-group">
                                <div class="editing-form-label-cell">             
                                    <cms:LocalizedLabel CssClass="control-label" ID="LocalizedLabel1" runat="server" ResourceString="doctype.ecommerce.defaultproducttype" EnableViewState="false" DisplayColon="true" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <cms:SelectProductType runat="server" ID="productTypeElem" />
                                </div>
                            </div>                       
                        </asp:PlaceHolder>
                                 
                    </div>
                </div>
            </div>            
                         
            <cms:CMSCheckBox runat="server" ID="chkGenerateSKU" ResourceString="DocType.Ecommerce.lblGenerateSKU"
                CssClass="InfoLabel" />
        </cms:CMSPanel>
    </cms:CMSPanel>        
    <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false" />    
    <asp:Literal ID="ltlScrpt" runat="server" EnableViewState="false" />
</asp:Content>
