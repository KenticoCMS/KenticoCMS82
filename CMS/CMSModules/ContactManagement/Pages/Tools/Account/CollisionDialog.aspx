<%@ Page Language="C#" AutoEventWireup="True" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Inherits="CMSModules_ContactManagement_Pages_Tools_Account_CollisionDialog" CodeFile="CollisionDialog.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/CountrySelector.ascx" TagName="CountrySelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Basic/DropDownListControl.ascx" TagName="DropDownListControl"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ContactManagement/FormControls/AccountStatusSelector.ascx"
    TagName="AccountStatusSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ContactManagement/FormControls/AccountSelector.ascx"
    TagName="AccountSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Basic/HtmlAreaControl.ascx" TagName="HtmlAreaControl"
    TagPrefix="cms" %>
<asp:Content ID="content" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:Panel ID="pnlContent" CssClass="MergeDialog" runat="server">
        <cms:JQueryTabContainer ID="pnlTabs" runat="server" CssClass="Dialog_Tabs">
            <cms:JQueryTab ID="tabFields" runat="server">
                <ContentTemplate>
                    <div class="PageContent">
                        <h4><%=GetString("general.general") %></h4>
                        <div class="form-horizontal form-merge-collisions">
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" ID="lblName" runat="server" EnableViewState="false" ResourceString="om.account.name"
                                        DisplayColon="true" AssociatedControlID="cmbAccountName" ShowRequiredMark="true" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownListControl CssClass="form-control" ID="cmbAccountName" runat="server" EditText="true" IsLiveSite="false" />
                                        <i runat="server" id="imgAccountName" class="icon-exclamation-triangle form-control-icon" visible="false"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" ID="lblAccountStatus" runat="server" ResourceString="om.accountstatus"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="accountStatusSelector" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline-forced">
                                        <cms:AccountStatusSelector ID="accountStatusSelector" runat="server" AllowAllItem="false" DisplaySiteOrGlobal="true"
                                            IsLiveSite="false" />
                                        <i runat="server" id="imgAccountStatus" visible="False" class="icon-exclamation-triangle form-control-icon"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" ID="lblAccountOwner" runat="server" ResourceString="om.account.owner"
                                        DisplayColon="true" EnableViewState="false" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <cms:LocalizedLabel ID="lblOwner" CssClass="form-control-text" runat="server" />
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" ID="lblAccountHeadquarters" runat="server" ResourceString="om.account.subsidiaryof"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="accountSelector" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline-forced">
                                        <cms:AccountSelector ID="accountSelector" runat="server" IsLiveSite="false" />
                                        <i runat="server" id="imgAccountHeadquarters" visible="False" class="icon-exclamation-triangle form-control-icon"></i>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <h4><%=GetString("general.address")%></h4>
                        <div class="form-horizontal form-merge-collisions">
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" ID="lblAccountAddress1" runat="server" ResourceString="om.contact.address1"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cmbAccountAddress1" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownListControl CssClass="form-control" ID="cmbAccountAddress1" runat="server" EditText="true" IsLiveSite="false" />
                                        <i runat="server" id="imgAccountAddress1" visible="False" class="icon-exclamation-triangle form-control-icon"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" ID="lblAccountAddress2" runat="server" ResourceString="om.contact.address2"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cmbAccountAddress2" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownListControl CssClass="form-control" ID="cmbAccountAddress2" runat="server" EditText="true" IsLiveSite="false" />
                                        <i runat="server" id="imgAccountAddress2" visible="False" class="icon-exclamation-triangle form-control-icon"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" ID="lblAccountCity" runat="server" ResourceString="general.city"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cmbAccountCity" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownListControl ID="cmbAccountCity" runat="server" CssClass="form-control" EditText="true"
                                            IsLiveSite="false" />
                                        <i runat="server" id="imgAccountCity" visible="False" class="icon-exclamation-triangle form-control-icon"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" ID="lblAccountZIP" runat="server" ResourceString="general.zip"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cmbAccountZIP" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownListControl ID="cmbAccountZIP" runat="server" CssClass="form-control" EditText="true"
                                            IsLiveSite="false" />
                                        <i runat="server" id="imgAccountZIP" visible="False" class="icon-exclamation-triangle form-control-icon"></i>
                                    </div>
                                </div>
                            </div>
                            <cms:CMSUpdatePanel runat="server">
                                <ContentTemplate>
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel CssClass="control-label" ID="lblCountry" runat="server" ResourceString="general.country"
                                                DisplayColon="true" EnableViewState="false" AssociatedControlID="countrySelector" />
                                            <cms:LocalizedLabel CssClass="control-label label-state-selector" ID="lblState" runat="server" ResourceString="general.state" DisplayColon="true"
                                                EnableViewState="false" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <div class="control-group-inline">
                                                <cms:CountrySelector ID="countrySelector" runat="server" CssClass="merge-country-selector"
                                                    IsLiveSite="false" />
                                                <div class="merge-country-conflict-icons">
                                                    <i runat="server" id="imgAccountCountry" visible="False" class="icon-exclamation-triangle form-control-icon"></i>
                                                    <i runat="server" id="imgAccountState" visible="False" class="icon-exclamation-triangle form-control-icon"></i>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </cms:CMSUpdatePanel>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" ID="lblAccountPhone" runat="server" ResourceString="general.phone"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cmbAccountPhone" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownListControl ID="cmbAccountPhone" runat="server" CssClass="DropDownField" EditText="true"
                                            IsLiveSite="false" />
                                        <i runat="server" id="imgAccountPhone" visible="False" class="icon-exclamation-triangle form-control-icon"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" ID="lblAccountFax" runat="server" ResourceString="general.fax"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cmbAccountFax" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownListControl ID="cmbAccountFax" runat="server" CssClass="DropDownField" EditText="true"
                                            IsLiveSite="false" />
                                        <i runat="server" id="imgAccountFax" visible="False" class="icon-exclamation-triangle form-control-icon"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" ID="lblAccountEmail" runat="server" ResourceString="general.emailaddress"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cmbAccountEmail" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownListControl ID="cmbAccountEmail" runat="server" CssClass="DropDownField" EditText="true"
                                            IsLiveSite="false" />
                                        <i runat="server" id="imgAccountEmail" visible="False" class="icon-exclamation-triangle form-control-icon"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" ID="lblAccountWebSite" runat="server" ResourceString="om.account.url"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cmbAccountWebSite" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownListControl ID="cmbAccountWebSite" runat="server" CssClass="DropDownField"
                                            EditText="true" IsLiveSite="false" />
                                        <i runat="server" id="imgAccountWebSite" visible="False" class="icon-exclamation-triangle form-control-icon"></i>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <h4><%=GetString("om.contact.notes")%></h4>
                        <div class="form-horizontal form-merge-collisions">
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" ID="lblAccountNotes" runat="server" ResourceString="om.contact.notes"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="htmlNotes" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <cms:HtmlAreaControl ID="htmlNotes" runat="server" ToolbarSet="Basic" IsLiveSite="false" />
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-value-cell-offset editing-form-value-cell">
                                    <cms:LocalizedButton ID="btnStamp" runat="server" ResourceString="om.account.stamp"
                                        ButtonStyle="Default" EnableViewState="false" />
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </cms:JQueryTab>
            <cms:JQueryTab ID="tabCustomFields" runat="server">
                <ContentTemplate>
                    <div class="PageContent">
                        <asp:PlaceHolder ID="plcCustomFields" runat="server"></asp:PlaceHolder>
                    </div>
                </ContentTemplate>
            </cms:JQueryTab>
            <cms:JQueryTab ID="tabContacts" runat="server">
                <ContentTemplate>
                    <div class="PageContent">
                        <cms:LocalizedHeading ID="headContactInfo" Level="4" runat="server" EnableViewState="false" CssClass="listing-title"
                            ResourceString="om.account.contactroles" />
                        <asp:PlaceHolder ID="plcAccountContact" runat="server"></asp:PlaceHolder>
                    </div>
                </ContentTemplate>
            </cms:JQueryTab>
            <cms:JQueryTab ID="tabContactGroups" runat="server">
                <ContentTemplate>
                    <div class="PageContent">
                        <cms:LocalizedHeading ID="headContactGroupsInfo" Level="4" runat="server" CssClass="listing-title"
                            ResourceString="om.contactgroup.selectmerge" />
                        <cms:CMSCheckBoxList ID="chkContactGroups" runat="server" />
                    </div>
                </ContentTemplate>
            </cms:JQueryTab>
        </cms:JQueryTabContainer>
    </asp:Panel>
</asp:Content>
<asp:Content ID="footer" ContentPlaceHolderID="plcFooter" runat="server">
    <cms:LocalizedButton ID="btnMerge" runat="server" ButtonStyle="Primary" ResourceString="om.contact.merge"
        EnableViewState="false" />
</asp:Content>
