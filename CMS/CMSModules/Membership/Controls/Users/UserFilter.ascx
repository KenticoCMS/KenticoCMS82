<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_Controls_Users_UserFilter"
    CodeFile="UserFilter.ascx.cs" %>

<%@ Register Src="~/CMSModules/Membership/FormControls/Roles/selectrole.ascx" TagName="SelectRole"
    TagPrefix="uc1" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TextSimpleFilter.ascx" TagName="TextSimpleFilter"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>

<cms:DisabledModule runat="server" ID="ucDisabledModule" Visible="false" />
<div class="form-horizontal form-filter">
    <asp:Panel ID="pnlSimpleFilter" runat="server" DefaultButton="btnSearch">
        <div class="form-group">
            <div class="filter-form-value-cell-wide-200 form-search-container">
                <asp:Label AssociatedControlID="txtSearch" runat="server" CssClass="sr-only">
                    <%= GetString("general.search") %>
                </asp:Label>
                <cms:CMSTextBox ID="txtSearch" runat="server" MaxLength="450" />
                <cms:CMSIcon ID="iconSearch" runat="server" CssClass="icon-magnifier" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group form-group-buttons">
            <div class="filter-form-label-cell">
                <asp:LinkButton ID="lnkShowAdvancedFilter" runat="server" OnClick="lnkShowAdvancedFilter_Click" CssClass="simple-advanced-link" />
            </div>
            <div class="filter-form-buttons-cell-wide-with-link">
                <cms:CMSButton ID="btnReset" runat="server" ButtonStyle="Default" EnableViewState="false" />
                <cms:CMSButton ID="btnSearch" runat="server" OnClick="btnSearch_Click" />
            </div>
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlAdvancedFilter" runat="server" DefaultButton="btnAdvancedSearch">
        <asp:PlaceHolder ID="plcUserName" runat="server">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel ID="lblUserName" runat="server" CssClass="control-label" ResourceString="general.username"
                        DisplayColon="true" AssociatedControlID="fltUserName" />
                </div>
                <div>
                    <cms:TextSimpleFilter ID="fltUserName" runat="server" Column="UserName" Size="100" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <asp:Label ID="lblFullName" runat="server" CssClass="control-label" AssociatedControlID="fltFullName" />
            </div>
            <div>
                <cms:TextSimpleFilter ID="fltFullName" runat="server" Column="FullName" Size="450" />
            </div>
        </div>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <asp:Label ID="lblEmail" runat="server" CssClass="control-label" AssociatedControlID="fltEmail" />
            </div>
            <div>
                <cms:TextSimpleFilter ID="fltEmail" runat="server" Column="Email" Size="100" />
            </div>
        </div>
        <asp:PlaceHolder ID="plcNickName" runat="server">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <asp:Label ID="lblNickName" runat="server" CssClass="control-label" AssociatedControlID="fltNickName" />
                </div>
                <div>
                    <cms:TextSimpleFilter ID="fltNickName" runat="server" Column="UserNickName" Size="200" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <asp:Label ID="lblInRoles" runat="server" CssClass="control-label" AssociatedControlID="drpTypeSelectInRoles" />
            </div>
            <div class="filter-form-condition-cell">
                <cms:CMSDropDownList CssClass="ExtraSmallDropDown" runat="server" ID="drpTypeSelectInRoles" />
            </div>
            <div class="filter-form-value-cell control-group-inline">
                <uc1:SelectRole UserFriendlyMode="true" IsLiveSite="false" ID="selectRoleElem" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <asp:Label ID="lblNotInRoles" runat="server" CssClass="control-label" AssociatedControlID="drpTypeSelectNotInRoles" />
            </div>
            <div class="filter-form-condition-cell">
                <cms:CMSDropDownList CssClass="ExtraSmallDropDown" runat="server" ID="drpTypeSelectNotInRoles" />
            </div>
            <div class="filter-form-value-cell control-group-inline">
                <uc1:SelectRole UserFriendlyMode="true" IsLiveSite="false" ID="selectNotInRole" runat="server" />
            </div>
        </div>
        <asp:PlaceHolder ID="plcGroups" runat="server" Visible="false">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <asp:Label ID="lblInGroups" runat="server" CssClass="control-label" AssociatedControlID="drpTypeSelectInGroups" />
                </div>
                <div class="filter-form-condition-cell">
                    <cms:CMSDropDownList CssClass="ExtraSmallDropDown" runat="server" ID="drpTypeSelectInGroups" />
                </div>
                <div class="filter-form-value-cell control-group-inline">
                    <asp:PlaceHolder runat="server" ID="plcSelectInGroups" />
                </div>
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <asp:Label ID="lblNotInGroups" runat="server" CssClass="control-label" AssociatedControlID="drpTypeSelectNotInGroups" />
                </div>
                <div class="filter-form-condition-cell">
                    <cms:CMSDropDownList CssClass="ExtraSmallDropDown" runat="server" ID="drpTypeSelectNotInGroups" />
                </div>
                <div class="filter-form-value-cell control-group-inline">
                    <asp:PlaceHolder runat="server" ID="plcSelectNotInGroups" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcUserEnabled" runat="server">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <asp:Label ID="lblEnabled" runat="server" CssClass="control-label" AssociatedControlID="chkEnabled" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:CMSCheckBox runat="server" ID="chkEnabled" />
                </div>
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <asp:Label ID="lblLockReason" runat="server" CssClass="control-label" AssociatedControlID="drpLockReason" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:CMSDropDownList ID="drpLockReason" runat="server" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcDisplayAnonymous" runat="server" Visible="False">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel ID="lblDisplayAnonymous" runat="server" CssClass="control-label"
                        EnableViewState="False" AssociatedControlID="chkDisplayAnonymous" ResourceString="userlist.displayguests"
                        DisplayColon="True" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:CMSCheckBox ID="chkDisplayAnonymous" runat="server" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcHidden" runat="server" Visible="False">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel ID="lblHidden" runat="server" CssClass="control-label" EnableViewState="False"
                        AssociatedControlID="chkDisplayHidden" ResourceString="user.showhidden" DisplayColon="True" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:CMSCheckBox ID="chkDisplayHidden" runat="server" Checked="True" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcScore" runat="server" Visible="False">
            <asp:PlaceHolder ID="plcSite" runat="server" Visible="False">
                <div class="form-group">
                    <div class="filter-form-label-cell">
                        <cms:LocalizedLabel ID="lblSite" runat="server" CssClass="control-label" EnableViewState="False"
                            AssociatedControlID="siteSelector" ResourceString="general.site" DisplayColon="True" />
                    </div>
                    <div class="filter-form-value-cell control-group-inline">
                        <cms:SiteSelector ID="siteSelector" runat="server" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel ID="lblScore" runat="server" CssClass="control-label" EnableViewState="False"
                        AssociatedControlID="pnlUpdateScore" ResourceString="om.score" DisplayColon="True" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:CMSUpdatePanel ID="pnlUpdateScore" runat="server" UpdateMode="Always">
                        <ContentTemplate>
                            <asp:PlaceHolder ID="plcUpdateContent" runat="server"></asp:PlaceHolder>
                        </ContentTemplate>
                    </cms:CMSUpdatePanel>
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group form-group-buttons">
            <div class="filter-form-label-cell">
                <asp:LinkButton ID="lnkShowSimpleFilter" runat="server" OnClick="lnkShowSimpleFilter_Click" CssClass="simple-advanced-link" />
            </div>
            <div class="filter-form-buttons-cell-wide-with-link">
                <cms:CMSButton ID="btnAdvancedReset" runat="server" ButtonStyle="Default" EnableViewState="false" />
                <cms:CMSButton ID="btnAdvancedSearch" runat="server" OnClick="btnSearch_Click" />
            </div>
        </div>
    </asp:Panel>

</div>