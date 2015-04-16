<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_ProjectManagement_Controls_UI_Projecttaskpriority_List" CodeFile="List.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>

<script type="text/javascript">
    //<![CDATA[
    function EditProjecttaskpriority(id) {
        window.location.replace('Edit.aspx?projecttaskpriorityId=' + id);
    }
    //]]>
</script>

<asp:Literal ID="ltlInfo" runat="server" EnableViewState="false"></asp:Literal>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<cms:UniGrid ID="gridElem" runat="server" GridName="~/CMSModules/ProjectManagement/Controls/UI/Projecttaskpriority/List.xml" OrderBy="TaskPriorityOrder"
    Columns="TaskPriorityID,TaskPriorityDisplayName,TaskPriorityDefault, TaskPriorityEnabled" />
