<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Theme="Default"
    Inherits="CMSAPIExamples_Default" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
    Title="CMS API Examples" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <script type="text/javascript">
        //<![CDATA[
        window.top.location.href ='<%= ApiExamplesApplicationUrl %>';
        //]]>
    </script>
</asp:Content>
