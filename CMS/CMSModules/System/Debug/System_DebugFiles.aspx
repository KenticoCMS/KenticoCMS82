<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_System_Debug_System_DebugFiles"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="System - Files"
    CodeFile="System_DebugFiles.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:DisabledModule runat="server" ID="ucDisabled" SettingsKeys="CMSDebugFiles;CMSDebugEverything;CMSDebugEverythingEverywhere" ConfigKeys="CMSDebugFiles;CMSDebugEverything;CMSDebugEverythingEverywhere" InfoText="{$DebugFiles.NotConfigured$}" AtLeastOne="True" />
    <div class="clearfix">
        <div class="form-horizontal form-filter pull-left">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel runat="server" ID="lblOperationType" ResourceString="FilesLog.OperationType"
                        DisplayColon="true" EnableViewState="false" Visible="false" CssClass="control-label" AssociatedControlID="drpOperationType" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:CMSDropDownList runat="server" ID="drpOperationType" AutoPostBack="true" CssClass="input-width-60" />
                </div>
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel runat="server" CssClass="control-label" 
                        DisplayColon="True" ResourceString="Debug.ShowCompleteContext" AssociatedControlID="chkCompleteContext" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:CMSCheckBox runat="server" ID="chkCompleteContext"
                        AutoPostBack="true" />
                </div>
            </div>
        </div>

        <div class="pull-right">
            <cms:CMSButton runat="server" ID="btnClear" OnClick="btnClear_Click" ButtonStyle="Default"
                EnableViewState="false" />
        </div>
    </div>
    <asp:PlaceHolder runat="server" ID="plcLogs" />
</asp:Content>