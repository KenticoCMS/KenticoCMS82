<%@ Page Title="Tools - File import - Import from computer" Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    AutoEventWireup="true" CodeFile="ImportFromComputer.aspx.cs" Inherits="CMSModules_FileImport_Tools_ImportFromComputer"
    Theme="Default" %>

<%@ Register Src="~/CMSAdminControls/Silverlight/MultiFileUploader/MultiFileUploader.ascx"
    TagName="MultiFileUploader" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/FormControls/Documents/SelectPath.ascx"
    TagName="SelectPath" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Cultures/SiteCultureSelector.ascx" TagName="SiteCultureSelector"
    TagPrefix="cms" %>

<asp:Content ID="Content5" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Panel ID="pnlImportControls" runat="server">
        <cms:CMSUpdatePanel ID="pnlSelectors" runat="server">
            <ContentTemplate>
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel ID="LocalizedLabel1" runat="server" CssClass="control-label" ResourceString="Tools.FileImport.TargetAliasPath"
                                DisplayColon="true" EnableViewState="false" AssociatedControlID="pathElem" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:SelectPath runat="server" ID="pathElem" IsLiveSite="false" SinglePathMode="true"/>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel ID="lblSelectCulture" runat="server" CssClass="control-label" ResourceString="general.culture"
                                DisplayColon="true" EnableViewState="false" AssociatedControlID="cultureSelector" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:SiteCultureSelector runat="server" ID="cultureSelector" AllowDefault="false" IsLiveSite="false" PostbackOnChange="false" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel ID="lblIncludeExtension" runat="server" CssClass="control-label" ResourceString="Tools.FileImport.RemoveExtension"
                                DisplayColon="true" EnableViewState="false" AssociatedControlID="chkIncludeExtension" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSCheckBox ID="chkIncludeExtension" runat="server" />
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:Panel>
    <cms:MultiFileUploader ID="uploaderElem" runat="server" UploadMode="Grid" Multiselect="true"
        SourceType="Content" Width="100%">
        <AlternateContent>
            <cms:LocalizedLabel ID="lblNoSilverlight" runat="server" ResourceString="fileimport.nosilverlight"
                CssClass="InfoLabel" />
        </AlternateContent>
    </cms:MultiFileUploader>
</asp:Content>
