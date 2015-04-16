<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SettingsKeyControlSelector.ascx.cs"
    Inherits="CMSModules_Settings_FormControls_SettingsKeyControlSelector" %>
<%@ Register Src="~/CMSFormControls/Dialogs/FileSystemSelector.ascx" TagPrefix="cms" TagName="FileSystemSelector" %>
<%@ Register Src="~/CMSModules/FormControls/FormControls/FormControlSelector.ascx" TagName="FormControlSelector" TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <%-- Radios --%>
        <asp:Panel ID="pnlChoice" runat="server" CssClass="radio-list-vertical">
            <cms:CMSRadioButton ID="radDefault" GroupName="ControlTypeRadGroup" runat="server" AutoPostBack="true" ResourceString="general.default" />
            <cms:CMSRadioButton ID="radFormControl" GroupName="ControlTypeRadGroup" runat="server" AutoPostBack="true" ResourceString="objecttype.cms_formusercontrol" />
            <%-- Form control selection --%>
            <asp:Panel ID="pnlFormControl" runat="server" Visible="false" CssClass="selector-subitem">
                <cms:FormControlSelector ID="ucFormControlSelector" runat="server" ShowInheritedControls="true" ReturnColumnName="UserControlCodeName" AutoPostBack="true" AllowEmptyValue="True" />
            </asp:Panel>
            <cms:CMSRadioButton ID="radFileSystem" GroupName="ControlTypeRadGroup" runat="server" AutoPostBack="true" ResourceString="settingskey.filesystempath" />
            <%-- File system path selection --%>
            <asp:Panel ID="pnlFileSystem" runat="server" Visible="false" CssClass="selector-subitem">
                <cms:FileSystemSelector ID="ucFileSystemSelector" runat="server" AllowEmptyValue="False" />
            </asp:Panel>
        </asp:Panel>
    </ContentTemplate>
</cms:CMSUpdatePanel>
