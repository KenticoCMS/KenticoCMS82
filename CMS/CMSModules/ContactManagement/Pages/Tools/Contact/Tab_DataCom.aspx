<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Tab_DataCom.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Data.com" Inherits="CMSModules_ContactManagement_Pages_Tools_Contact_Tab_DataCom" Theme="Default" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/DataCom/ErrorSummary.ascx" TagName="ErrorSummary" TagPrefix="cms" %>
<%@ Register Assembly="CMS.DataCom" Namespace="CMS.DataCom" TagPrefix="cms" %>
<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ErrorSummary ID="ErrorSummary" runat="server" MessagesEnabled="true" />
    <asp:HiddenField ID="ContactHiddenField" runat="server" EnableViewState="false" />
    <asp:Label ID="lblLoggedAs" runat="server" EnableViewState="false" />
    <cms:CMSPanel ID="ContactFormPanel" ShortID="p" runat="server" ViewStateMode="Disabled" >
        <cms:DataComForm ID="ContactForm" runat="server" />
    </cms:CMSPanel>
    <script type="text/javascript">

        function DataCom_SetContact(value) {
            var element = document.getElementById('<%= ContactHiddenField.ClientID %>');
            if (element != null) {
                element.value = value;
                __doPostBack('', 'SetContact');
            }
        }

    </script>
</asp:Content>
