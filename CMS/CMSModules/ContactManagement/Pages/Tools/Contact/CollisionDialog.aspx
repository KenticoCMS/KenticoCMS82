<%@ Page Language="C#" AutoEventWireup="True" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Inherits="CMSModules_ContactManagement_Pages_Tools_Contact_CollisionDialog" CodeFile="CollisionDialog.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/CountrySelector.ascx" TagName="CountrySelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Basic/CalendarControl.ascx" TagName="CalendarControl"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Basic/HtmlAreaControl.ascx" TagName="HtmlAreaControl"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Basic/DropDownListControl.ascx" TagName="DropDownList"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/FormControls/SelectCampaign.ascx" TagName="CampaignSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ContactManagement/FormControls/ContactStatusSelector.ascx"
    TagName="ContactStatusSelector" TagPrefix="cms" %>

<asp:Content ID="content" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:Panel ID="pnlContent" CssClass="MergeDialog" runat="server">
        <cms:JQueryTabContainer ID="pnlTabs" runat="server" CssClass="Dialog_Tabs ">
            <cms:JQueryTab ID="tabFields" runat="server">
                <ContentTemplate>
                    <div class="PageContent">
                        <cms:LocalizedHeading runat="server" Level="4" ResourceString="general.general"></cms:LocalizedHeading>
                        <div class="form-horizontal form-merge-collisions">
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel class="control-label" ID="lblContactFirstName" runat="server" ResourceString="om.contact.firstname"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cmbContactFirstName" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownList ID="cmbContactFirstName" runat="server" EditText="true" IsLiveSite="false" />
                                        <i runat="server" id="imgContactFirstName" class="icon-exclamation-triangle form-control-icon" visible="false"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel class="control-label" ID="lblContactMiddleName" runat="server" ResourceString="om.contact.middlename"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cmbContactMiddleName" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownList ID="cmbContactMiddleName" runat="server" EditText="true" IsLiveSite="false" />
                                        <i runat="server" id="imgContactMiddleName" class="icon-exclamation-triangle form-control-icon" visible="false"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel class="control-label" ID="lblContactLastName" runat="server" ResourceString="om.contact.lastname"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cmbContactLastName" ShowRequiredMark="true" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownList ID="cmbContactLastName" runat="server" EditText="true" IsLiveSite="false" />
                                        <i runat="server" id="imgContactLastName" class="icon-exclamation-triangle form-control-icon" visible="false"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel class="control-label" ID="lblContactSalutation" runat="server" ResourceString="om.contact.salutation"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cmbContactSalutation" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownList ID="cmbContactSalutation" runat="server" CssClass="DropDownField"
                                            EditText="true" IsLiveSite="false" />
                                        <i runat="server" id="imgContactSalutation" class="icon-exclamation-triangle form-control-icon" visible="false"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel class="control-label" ID="lblContactTitleBefore" runat="server" ResourceString="om.contact.titlebefore"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cmbContactTitleBefore" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownList ID="cmbContactTitleBefore" runat="server" CssClass="DropDownField"
                                            EditText="true" IsLiveSite="false" />
                                        <i runat="server" id="imgContactTitleBefore" class="icon-exclamation-triangle form-control-icon" visible="false"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel class="control-label" ID="lblContactTitleAfter" runat="server" ResourceString="om.contact.titleafter"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cmbContactTitleAfter" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownList ID="cmbContactTitleAfter" runat="server" CssClass="DropDownField"
                                            EditText="true" IsLiveSite="false" />
                                        <i runat="server" id="imgContactTitleAfter" class="icon-exclamation-triangle form-control-icon" visible="false"></i>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <cms:LocalizedHeading runat="server" Level="4" ResourceString="om.contact.personal"></cms:LocalizedHeading>
                        <div class="form-horizontal form-merge-collisions">
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel class="control-label" ID="lblContactBirthday" runat="server" ResourceString="om.contact.birthday"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="calendarControl" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:CalendarControl ID="calendarControl" runat="server" IsLiveSite="false" EditTime="false" />
                                        <i runat="server" id="imgContactBirthday" class="icon-exclamation-triangle form-control-icon" visible="false"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel class="control-label" ID="lblContactGender" runat="server" ResourceString="om.contact.gender"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="genderSelector" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline merge-gender-selector">
                                        <cms:GenderSelector ID="genderSelector" runat="server" CssClass="control-group-inline" IsLiveSite="false" />
                                        <i runat="server" id="imgContactGender" class="icon-exclamation-triangle form-control-icon" visible="false"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel class="control-label" ID="lblCompanyName" runat="server" ResourceString="om.contact.companyname"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cmbContactCompanyName" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownList ID="cmbContactCompanyName" runat="server" CssClass="DropDownField"
                                            EditText="true" IsLiveSite="false" />
                                        <i runat="server" id="imgContactCompanyName" class="icon-exclamation-triangle form-control-icon" visible="false"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel class="control-label" ID="lblContactJobTitle" runat="server" ResourceString="om.contact.jobtitle"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cmbContactJobTitle" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownList ID="cmbContactJobTitle" runat="server" CssClass="DropDownField"
                                            EditText="true" IsLiveSite="false" />
                                        <i runat="server" id="imgContactJobTitle" class="icon-exclamation-triangle form-control-icon" visible="false"></i>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <cms:LocalizedHeading runat="server" Level="4" ResourceString="om.contact.settings"></cms:LocalizedHeading>
                        <div class="form-horizontal form-merge-collisions">
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel class="control-label" ID="lblContactStatusID" runat="server" ResourceString="om.contactstatus"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="contactStatusSelector" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline-forced">
                                        <cms:ContactStatusSelector ID="contactStatusSelector" runat="server" IsLiveSite="false"
                                            AllowAllItem="false" />
                                        <i runat="server" id="imgContactStatus" class="icon-exclamation-triangle form-control-icon" visible="false"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel class="control-label" ID="lblContactOwnerUserID" runat="server" ResourceString="om.contact.owner"
                                        DisplayColon="true" EnableViewState="false" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <cms:LocalizedLabel class="form-control-text" ID="lblOwner" runat="server" />
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel class="control-label" ID="lblContactMonitored" runat="server" ResourceString="om.contact.tracking"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="chkContactMonitored" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:CMSCheckBox CssClass="checkbox-no-label" ID="chkContactMonitored" runat="server" IsLiveSite="false" />
                                        <i runat="server" id="imgContactMonitored" class="icon-exclamation-triangle form-control-icon" visible="false"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel class="control-label" ID="lblContactCampaign" runat="server" ResourceString="analytics.campaign"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cCampaign" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline-forced">
                                        <cms:CampaignSelector ID="cCampaign" runat="server" IsLiveSite="false" AllowEmpty="true"
                                            NoneRecordValue="" />
                                        <i runat="server" id="imgContactCampaign" class="icon-exclamation-triangle form-control-icon" visible="false"></i>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <cms:LocalizedHeading runat="server" Level="4" ResourceString="general.address"></cms:LocalizedHeading>
                        <div class="form-horizontal form-merge-collisions">
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel class="control-label" ID="lblContactAddress1" runat="server" ResourceString="om.contact.address1"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cmbContactAddress1" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownList ID="cmbContactAddress1" runat="server" CssClass="DropDownField"
                                            EditText="true" IsLiveSite="false" />
                                        <i runat="server" id="imgContactAddress1" class="icon-exclamation-triangle form-control-icon" visible="false"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel class="control-label" ID="lblContactAddress2" runat="server" ResourceString="om.contact.address2"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cmbContactAddress2" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownList ID="cmbContactAddress2" runat="server" CssClass="DropDownField"
                                            EditText="true" IsLiveSite="false" />
                                        <i runat="server" id="imgContactAddress2" class="icon-exclamation-triangle form-control-icon" visible="false"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel class="control-label" ID="lblContactCity" runat="server" ResourceString="general.city"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cmbContactCity" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownList ID="cmbContactCity" runat="server" CssClass="DropDownField" EditText="true"
                                            IsLiveSite="false" />
                                        <i runat="server" id="imgContactCity" class="icon-exclamation-triangle form-control-icon" visible="false"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel class="control-label" ID="lblContactZIP" runat="server" ResourceString="general.zip"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cmbContactZIP" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownList ID="cmbContactZIP" runat="server" CssClass="DropDownField" EditText="true"
                                            IsLiveSite="false" />
                                        <i runat="server" id="imgContactZIP" class="icon-exclamation-triangle form-control-icon" visible="false"></i>
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
                                                    <i runat="server" id="imgContactCountry" visible="False" class="icon-exclamation-triangle form-control-icon"></i>
                                                    <i runat="server" id="imgContactState" visible="False" class="icon-exclamation-triangle form-control-icon"></i>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </cms:CMSUpdatePanel>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel class="control-label" ID="lblContactMobilePhone" runat="server" ResourceString="om.contact.mobilephone"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cmbContactMobilePhone" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownList ID="cmbContactMobilePhone" runat="server" CssClass="DropDownField"
                                            EditText="true" IsLiveSite="false" />
                                        <i runat="server" id="imgContactMobilePhone" class="icon-exclamation-triangle form-control-icon" visible="false"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel class="control-label" ID="lblContactHomePhone" runat="server" ResourceString="om.contact.homephone"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cmbContactHomePhone" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownList ID="cmbContactHomePhone" runat="server" CssClass="DropDownField"
                                            EditText="true" IsLiveSite="false" />
                                        <i runat="server" id="imgContactHomePhone" class="icon-exclamation-triangle form-control-icon" visible="false"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel class="control-label" ID="lblContactBusinessPhone" runat="server" ResourceString="om.contact.businessphone"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cmbContactBusinessPhone" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownList ID="cmbContactBusinessPhone" runat="server" CssClass="DropDownField"
                                            EditText="true" IsLiveSite="false" />
                                        <i runat="server" id="imgContactBusinessPhone" class="icon-exclamation-triangle form-control-icon" visible="false"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel class="control-label" ID="lblContactEmail" runat="server" ResourceString="general.emailaddress"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cmbContactEmail" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownList ID="cmbContactEmail" runat="server" CssClass="DropDownField" EditText="true"
                                            IsLiveSite="false" />
                                        <i runat="server" id="imgContactEmail" class="icon-exclamation-triangle form-control-icon" visible="false"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel class="control-label" ID="lblContactWebSite" runat="server" ResourceString="om.contact.website"
                                        DisplayColon="true" EnableViewState="false" AssociatedControlID="cmbContactWebSite" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="control-group-inline">
                                        <cms:DropDownList ID="cmbContactWebSite" runat="server" CssClass="DropDownField"
                                            EditText="true" IsLiveSite="false" />
                                        <i runat="server" id="imgContactWebSite" class="icon-exclamation-triangle form-control-icon" visible="false"></i>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <cms:LocalizedHeading runat="server" Level="4" ResourceString="om.contact.notes"></cms:LocalizedHeading>
                        <div class="form-horizontal form-merge-collisions">
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" ID="lblContactNotes" runat="server" ResourceString="om.contact.notes"
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
                            ResourceString="om.contact.accountroles" />
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
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnMerge" runat="server" ButtonStyle="Primary" ResourceString="om.contact.merge"
            EnableViewState="false" />
    </div>
</asp:Content>
