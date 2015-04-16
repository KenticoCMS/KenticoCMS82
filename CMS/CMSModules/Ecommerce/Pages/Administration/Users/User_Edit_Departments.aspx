<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Administration_Users_User_Edit_Departments"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="User Edit - Departments"
    CodeFile="User_Edit_Departments.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>

<asp:Content ID="cntSiteSelect" runat="server" ContentPlaceHolderID="plcSiteSelector">
    <asp:PlaceHolder ID="plcSites" runat="server">
        <div class="form-horizontal form-filter">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel ID="lblSelectSite" runat="server" CssClass="control-label" ResourceString="general.site"
                        AssociatedControlID="siteSelector" DisplayColon="true" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:PlaceHolder ID="plcTable" runat="server">
                <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" DisplayColon="true" ResourceString="com.departments.userdepartments" CssClass="listing-title" EnableViewState="false" />
                <cms:UniSelector ID="uniSelector" runat="server" IsLiveSite="false" OrderBy="DepartmentName"
                    ObjectType="ecommerce.department" SelectionMode="Multiple" ResourcePrefix="departmentsselector" />
            </asp:PlaceHolder>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>