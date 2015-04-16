<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ActivityTypeSelector.ascx.cs"
    Inherits="CMSModules_ContactManagement_FormControls_ActivityTypeSelector" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:UniSelector ID="ucType" ShortID="us" ObjectType="om.activitytype" runat="server"
    ReturnColumnName="ActivityTypeName" SelectionMode="SingleDropDownList" OrderBy="ActivityTypeDisplayName"
    LocalizeItems="true" AllowEmpty="false" IsLiveSite="false" ResourcePrefix="activitytypeselect" />
