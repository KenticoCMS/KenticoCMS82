<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Tools - File import - Import from server" Inherits="CMSModules_FileImport_Tools_ImportFromServer"
    Theme="Default" CodeFile="ImportFromServer.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/FormControls/Documents/SelectPath.ascx"
    TagName="SelectPath" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Cultures/SiteCultureSelector.ascx" TagName="SiteCultureSelector"
    TagPrefix="cms" %>

<%@ Register Src="~/CMSAdminControls/AsyncLogDialog.ascx" TagName="AsyncLog"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TextSimpleFilter.ascx" TagName="TextFilter"
    TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcBeforeBody" runat="server" ID="cntBeforeBody">
    <asp:Panel runat="server" ID="pnlLog" Visible="false">
        <cms:AsyncLog ID="ctlAsyncLog" runat="server" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlContent">
        <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
        <input type="hidden" id="targetNodeId" name="targetNodeId" />
        <asp:PlaceHolder ID="plcImportContent" runat="server">
            <asp:Panel ID="pnlTitle" runat="server">
                <asp:Label ID="lblTitle" runat="server" EnableViewState="false" /><br />
                <br />
            </asp:Panel>
            <asp:PlaceHolder ID="plcImportList" runat="server">
                <asp:HiddenField ID="hdnSelected" runat="server" />
                <asp:HiddenField ID="hdnValue" runat="server" />
                <asp:Panel ID="pnlGrid" runat="server">
                    <div class="form-horizontal form-filter">
                        <div class="form-group">
                            <div class="filter-form-label-cell">
                                <cms:LocalizedLabel ID="lblFilter" ResourceString="general.name" CssClass="control-label" AssociatedControlID="ucFilter"
                                    runat="server" DisplayColon="true" EnableViewState="false" />
                            </div>
                            <cms:TextFilter runat="server" ID="ucFilter" />
                        </div>
                        <div class="form-group form-group-buttons">
                            <div class="filter-form-buttons-cell-wide">
                                <cms:CMSButton ID="btnFilter" runat="server" ButtonStyle="Primary" EnableViewState="false" />
                            </div>
                        </div>
                    </div>
                    <cms:UniGrid ID="gridImport" runat="server" GridName="FileImport.xml" DelayedReload="true" RememberState="false" />
                </asp:Panel>
                <br />
                <asp:Panel ID="pnlCount" runat="server">
                    <b>
                        <asp:Label ID="lblTotal" runat="server" EnableViewState="false" />
                        &nbsp;
                        <asp:Label ID="lblSelected" runat="server" EnableViewState="false" />&nbsp;<asp:Label
                            ID="lblSelectedValue" runat="server" EnableViewState="false" />
                    </b>
                </asp:Panel>
                <cms:FileSystemDataSource ID="fileSystemDataSource" runat="server" IncludeSubDirs="true" />
            </asp:PlaceHolder>
            <br />
            <br />
            <asp:Panel ID="pnlImportControls" runat="server">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel ID="lblTargetAliasPath" runat="server" CssClass="control-label" ResourceString="Tools.FileImport.TargetAliasPath"
                                DisplayColon="true" EnableViewState="false" AssociatedControlID="pathElem" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:SelectPath runat="server" ID="pathElem" IsLiveSite="false" SinglePathMode="true" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel ID="lblSelectCulture" runat="server" CssClass="control-label" ResourceString="general.culture"
                                DisplayColon="true" EnableViewState="false" AssociatedControlID="cultureSelector" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:SiteCultureSelector runat="server" ID="cultureSelector" AllowDefault="false"
                                IsLiveSite="false" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel ID="lblDeleteImported" runat="server" CssClass="control-label" ResourceString="Tools.FileImport.DeleteImported"
                                DisplayColon="true" EnableViewState="false" AssociatedControlID="chkDeleteImported" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSCheckBox ID="chkDeleteImported" runat="server" />
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
                    <div class="form-group">
                        <div class="editing-form-value-cell-offset editing-form-value-cell">
                            <cms:LocalizedButton ID="btnStartImport" runat="server" ResourceString="Tools.FileImport.StartImport"
                                ButtonStyle="Primary" OnClick="btnStartImport_Click" EnableViewState="false" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </asp:PlaceHolder>
    </asp:Panel>
</asp:Content>