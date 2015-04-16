<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_WebAnalytics_Controls_UI_Campaign_Edit"
    CodeFile="Edit.ascx.cs" %>

<cms:UIForm runat="server" id="EditForm" objecttype="analytics.campaign"
    ononaftervalidate="EditForm_OnAfterValidate" ononbeforesave="EditForm_OnBeforeSave" RefreshHeader="true" ononaftersave="EditForm_OnAfterSave">
    <SecurityCheck Resource="CMS.WebAnalytics" Permission="ManageCampaigns" />
</cms:UIForm>