<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_WebAnalytics_Controls_UI_Conversion_Edit" CodeFile="Edit.ascx.cs" %>
    
<cms:UIForm runat="server" ID="EditForm" ObjectType="analytics.conversion" RefreshHeader="True">
    <SecurityCheck Resource="CMS.WebAnalytics" Permission="ManageConversions" />
</cms:UIForm>