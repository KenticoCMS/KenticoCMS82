<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MappingDialog.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_Contact_MappingDialog" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/ClassFields.ascx" TagName="ClassFields" TagPrefix="uc" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<div class="form-horizontal">
    <div class="form-group">
        <cms:CMSCheckBox ID="chkOverwrite" runat="server" ResourceString="class.allowoverwritecontactinfo" CssClass="ContentCheckbox" />
    </div>
</div>
<asp:Panel ID="pnlGeneral" runat="server" CssClass="ContentPanel">
    <div class="form-horizontal">
        <cms:LocalizedHeading ID="hdnGeneral" runat="server" Level="4" ResourceString="general.general" />
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblFirstName" runat="server" EnableViewState="false" ResourceString="om.contact.firstname"
                    DisplayColon="true" AssociatedControlID="fldFirstName" />
            </div>
            <div class="editing-form-value-cell">
                <uc:ClassFields ID="fldFirstName" runat="server" FieldDataType="Text" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblLastName" runat="server" CssClass="control-label" EnableViewState="false" ResourceString="om.contact.lastname"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <uc:ClassFields ID="fldLastName" runat="server" FieldDataType="Text" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblMiddleName" runat="server" EnableViewState="false" ResourceString="om.contact.middlename"
                    DisplayColon="true" AssociatedControlID="fldMiddleName" />
            </div>
            <div class="editing-form-value-cell">
                <uc:ClassFields ID="fldMiddleName" runat="server" FieldDataType="Text" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblSalutation" runat="server" CssClass="control-label" EnableViewState="false" ResourceString="om.contact.salutation"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <uc:ClassFields ID="fldSalutation" runat="server" FieldDataType="Text" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblTitleBefore" runat="server" EnableViewState="false" ResourceString="om.contact.titlebefore"
                    DisplayColon="true" AssociatedControlID="fldTitleBefore" />
            </div>
            <div class="editing-form-value-cell">
                <uc:ClassFields ID="fldTitleBefore" runat="server" FieldDataType="Text" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblTitleAfter" runat="server" CssClass="control-label" EnableViewState="false" ResourceString="om.contact.titleafter"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <uc:ClassFields ID="fldTitleAfter" runat="server" FieldDataType="Text" />
            </div>
        </div>
    </div>
</asp:Panel>
<asp:Panel ID="pnlPersonal" runat="server" CssClass="ContentPanel">
    <div class="form-horizontal">
        <cms:LocalizedHeading ID="hdnPersonal" runat="server" Level="4" ResourceString="om.contact.personal" />
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblBirthday" runat="server" EnableViewState="false" ResourceString="om.contact.birthday"
                    DisplayColon="true" AssociatedControlID="fldBirthday" />
            </div>
            <div class="editing-form-value-cell">
                <uc:ClassFields ID="fldBirthday" runat="server" FieldDataType="DateTime" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblJobTitle" runat="server" CssClass="control-label" EnableViewState="false" ResourceString="om.contact.jobtitle"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <uc:ClassFields ID="fldJobTitle" runat="server" FieldDataType="Text" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblGender" runat="server" EnableViewState="false" ResourceString="om.contact.gender"
                    DisplayColon="true" AssociatedControlID="fldGender" />
            </div>
            <div class="editing-form-value-cell">
                <uc:ClassFields ID="fldGender" runat="server" FieldDataType="Integer" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblCompanyName" runat="server" CssClass="control-label" EnableViewState="false" ResourceString="om.contact.companyname"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <uc:ClassFields ID="fldCompanyName" runat="server" FieldDataType="Text" />
            </div>
        </div>
    </div>
</asp:Panel>
<asp:Panel ID="pnlAddress" runat="server" CssClass="ContentPanel">
    <div class="form-horizontal">
        <cms:LocalizedHeading ID="hdnAddress" runat="server" Level="4" ResourceString="general.address" />
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblAddress1" runat="server" EnableViewState="false" ResourceString="om.contact.address1"
                    DisplayColon="true" AssociatedControlID="fldAddress1" />
            </div>
            <div class="editing-form-value-cell">
                <uc:ClassFields ID="fldAddress1" runat="server" FieldDataType="Text" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblMobilePhone" runat="server" CssClass="control-label" EnableViewState="false" ResourceString="om.contact.mobilephone"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <uc:ClassFields ID="fldMobilePhone" runat="server" FieldDataType="Text" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblAddress2" runat="server" EnableViewState="false" ResourceString="om.contact.address2"
                    DisplayColon="true" AssociatedControlID="fldAddress2" />
            </div>
            <div class="editing-form-value-cell">
                <uc:ClassFields ID="fldAddress2" runat="server" FieldDataType="Text" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblHomePhone" runat="server" CssClass="control-label" EnableViewState="false" ResourceString="om.contact.homephone"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <uc:ClassFields ID="fldHomePhone" runat="server" FieldDataType="Text" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblCity" runat="server" EnableViewState="false" ResourceString="general.city"
                    DisplayColon="true" AssociatedControlID="fldCity" />
            </div>
            <div class="editing-form-value-cell">
                <uc:ClassFields ID="fldCity" runat="server" FieldDataType="Text" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblBusinessPhone" CssClass="control-label" runat="server" EnableViewState="false"
                    ResourceString="om.contact.businessphone" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <uc:ClassFields ID="fldBusinessPhone" runat="server" FieldDataType="Text" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblZip" runat="server" EnableViewState="false" ResourceString="general.zip"
                    DisplayColon="true" AssociatedControlID="fldZip" />
            </div>
            <div class="editing-form-value-cell">
                <uc:ClassFields ID="fldZip" runat="server" FieldDataType="Text" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblEmail" CssClass="control-label" runat="server" EnableViewState="false" ResourceString="general.emailaddress"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <uc:ClassFields ID="fldEmail" runat="server" FieldDataType="Text" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblCountry" runat="server" EnableViewState="false" ResourceString="general.country"
                    DisplayColon="true" AssociatedControlID="fldCountry" />
            </div>
            <div class="editing-form-value-cell">
                <uc:ClassFields ID="fldCountry" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblURL" runat="server" EnableViewState="false" ResourceString="om.contact.website"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <uc:ClassFields ID="fldURL" runat="server" FieldDataType="Text" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblState" runat="server" EnableViewState="false" ResourceString="general.state"
                    DisplayColon="true" AssociatedControlID="fldState" />
            </div>
            <div class="editing-form-value-cell">
                <uc:ClassFields ID="fldState" runat="server" />
            </div>
        </div>
    </div>
</asp:Panel>
<asp:Panel ID="pnlCustom" runat="server" CssClass="ContentPanel" Visible="false">
    <cms:LocalizedHeading ID="hdnCustom" runat="server" Level="4" ResourceString="general.customfields" />
    <asp:PlaceHolder ID="plcCustom" runat="server" />
</asp:Panel>