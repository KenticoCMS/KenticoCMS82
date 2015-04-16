<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Properties_CreateWireframe"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
    EnableEventValidation="false" CodeFile="CreateWireframe.aspx.cs" %>

<%@ Register Src="~/CMSModules/Content/CMSDesk/New/TemplateSelection.ascx" TagName="TemplateSelection" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/EditMenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="new-page-dialog">
        <asp:Panel ID="pnlContent" runat="server" CssClass="page-content-frame">
            <div class="PTSelection white-wizard-body">
                <cms:CMSPanel ID="pnlMenu" runat="server" FixedPosition="true">
                    <cms:editmenu ID="menuElem" ShortID="m" runat="server" ShowProperties="false" ShowSpellCheck="true" IsLiveSite="false" />
                </cms:CMSPanel>
                <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" EnableViewState="false" />
                <cms:LocalizedHeading ID="lblPageName" runat="server" ResourceString="Wireframe.SelectTemplate" Level="3" />
                <cms:MessagesPlaceHolder ID="plcMess" runat="server" IsLiveSite="false" OffsetX="16" />
                <cms:TemplateSelection ID="selTemplate" ShortID="s" runat="server" />
                <div class="Footer">
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
