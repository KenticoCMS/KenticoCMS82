<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_ProjectManagement_MyProjects" CodeFile="~/CMSWebParts/ProjectManagement/MyProjects.ascx.cs" %>
<%@ Register Src="~/CMSModules/ProjectManagement/Controls/UI/Project/List.ascx" TagName="ProjectList"
    TagPrefix="cms" %>
<cms:ProjectList runat="server" ID="ucProjectList" />
