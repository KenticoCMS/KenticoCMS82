<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSMessages_Information"
    Theme="Default" EnableEventValidation="false" CodeFile="Information.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSAdminControls/Debug/SecurityLog.ascx" TagName="SecurityLog"
    TagPrefix="cms" %>

<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:LocalizedHeading Level="4" ClientIDMode="Static" ID="hdnPermission" runat="server" />
    <asp:Panel ID="pnlBody" runat="server">
        <asp:Panel ID="pnlContent" runat="server">
            <asp:Label ID="lblMessage" runat="server" EnableViewState="false" CssClass="InfoLabel" />
            <cms:SecurityLog ID="logSec" runat="server" InitFromRequest="true" />
        </asp:Panel>
    </asp:Panel>
    <script type="text/javascript">
        // <![CDATA[
        <%-- 
            All the modal window pages are not closable by clicking on the gray area,
            but this page is different, because there is no close button in the header
            This might get fixed when all the modal pages use dialog=1 GET parameter.
        --%>
        setTimeout(function () {
        	if (top && typeof top.addBackgroundClickHandler === 'function') {
        		top.addBackgroundClickHandler(this);
            }
        }, 500);

        if (wopener && (wopener !== window) && (parent.document.getElementsByClassName('dialog-header').length == 0)) {
            <%-- 
                Hide content title, dialog header will be visible
            --%>
            $cmsj('#hdnPermission').addClass('hidden');
        } else {
             <%-- 
                Hide header if content is not in modal dialog or content is nested in the modal dialog
            --%>
            var headerElem = $cmsj('#<%= CurrentMaster.PanelHeader.ClientID %>');
            headerElem.addClass('hidden');
            <%-- 
                Header is also preceded by an element with fixed height, so this element has to be hidden as well
            --%>
            headerElem.prev().addClass('hidden');
         }
        // ]]>
    </script>
</asp:Content>
