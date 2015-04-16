<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Admin_accessdenied" Theme="Default"
    EnableEventValidation="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Access denied" CodeFile="accessdenied.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/Debug/SecurityLog.ascx" TagName="SecurityLog"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <cms:LocalizedHeading Level="4" ClientIDMode="Static" ID="hdnPermission" runat="server" />
    <asp:Label ID="lblMessage" runat="server" Text="Label" EnableViewState="false" />
    <asp:HyperLink ID="lnkGoBack" runat="server" NavigateUrl="~/default.aspx" EnableViewState="false" /><br />
    <br />
    <br />
    <cms:LocalizedButton ID="btnSignOut" runat="server" ButtonStyle="Primary" OnClick="btnSignOut_Click"
        EnableViewState="false" ResourceString="signoutbutton.signout" />
    <cms:SecurityLog ID="logSec" runat="server" InitFromRequest="true" />
    <script type="text/javascript">
        // <![CDATA[
        if (window == window.top) {
            var signOut = document.getElementById('<%= btnSignOut.ClientID %>');
            if (signOut !== undefined) {
                signOut.style.display = '';
            }
        }
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
