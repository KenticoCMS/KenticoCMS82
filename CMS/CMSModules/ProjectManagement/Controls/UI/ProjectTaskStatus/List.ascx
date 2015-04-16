<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_ProjectManagement_Controls_UI_Projecttaskstatus_List" CodeFile="List.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>

<script type="text/javascript">
    //<![CDATA[
    function EditProjecttaskstatus(id) {
        window.location.replace('Edit.aspx?projecttaskstatusId=' + id);
    }
    //]]>
</script>

<asp:Literal ID="ltlInfo" runat="server" EnableViewState="false"></asp:Literal>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<cms:UniGrid ID="gridElem" runat="server" GridName="~/CMSModules/ProjectManagement/Controls/UI/Projecttaskstatus/List.xml" OrderBy="TaskStatusOrder"
    Columns="TaskStatusID,TaskStatusDisplayName,TaskStatusIsNotStarted,TaskStatusColor,TaskStatusEnabled,TaskStatusIcon,TaskStatusIsFinished" />
