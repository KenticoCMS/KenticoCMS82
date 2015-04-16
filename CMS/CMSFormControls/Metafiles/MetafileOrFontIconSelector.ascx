<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MetafileOrFontIconSelector.ascx.cs"
    Inherits="CMSFormControls_Metafiles_MetafileOrFontIconSelector" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/MetaFiles/File.ascx" TagName="MetafileUploader"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <div class="form-group">
            <cms:CMSDropDownList ID="lstOptions" runat="server" RepeatDirection="Horizontal" UseResourceStrings="true" AutoPostBack="True">
                <asp:ListItem Selected="True" Text="Image" Value="metafile" />
                <asp:ListItem Text="Font icon class" Value="cssclass" />
            </cms:CMSDropDownList>
        </div>
        <asp:PlaceHolder ID="plcMetaFile" runat="server">
            <cms:MetafileUploader ID="fileUploader" runat="server" />
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcCssClass" runat="server" Visible="false">
            <cms:CMSTextBox ID="txtCssClass" runat="server" ></cms:CMSTextBox>
        </asp:PlaceHolder>
    </ContentTemplate>
</cms:CMSUpdatePanel>
