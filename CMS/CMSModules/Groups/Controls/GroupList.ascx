<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_Controls_GroupList" CodeFile="GroupList.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" tagname="UniGrid" tagprefix="cms" %>
<%@ Register Src="~/CMSModules/Groups/Controls/GroupFilter.ascx" TagName="GroupFilter"
    TagPrefix="cms" %>
<cms:UniGrid runat="server" ID="gridElem" GridName="~/CMSModules/Groups/Controls/Group_List.xml"
    OrderBy="GroupDisplayName" />