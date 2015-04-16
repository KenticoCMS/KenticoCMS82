<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/CMSModules/Blogs/Controls/Blogs_Comments.ascx.cs"
    Inherits="CMSModules_Blogs_Controls_Blogs_Comments" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Blogs/Controls/CommentFilter.ascx" TagName="CommentFilter"
    TagPrefix="cms" %>
<cms:CommentFilter runat="server" ID="filterElem" DisplayAllRecord="true" />
<cms:UniGrid ID="gridComments" runat="server" GridName="~/CMSModules/Blogs/Tools/Comments_List.xml"
    OrderBy="CommentDate DESC" IsLiveSite="false" ExportFileName="cms_blogcomment" />
<br />
<asp:Panel ID="pnlActions" runat="server" CssClass="control-group-inline">
    <cms:LocalizedLabel ID="lblAction" runat="server" EnableViewState="false" DisplayColon="true" CssClass="form-control-text"
        ResourceString="blog.comments.action" />
    <cms:CMSDropDownList ID="drpAction" runat="server" CssClass="DropDownFieldSmall" />
    <cms:CMSButton ID="btnAction" runat="server" ButtonStyle="Default" OnClick="btnAction_Click"
        EnableViewState="false" />
</asp:Panel>
<script type="text/javascript"> 
    <!--
    // Refreshes current page when comment properties are changed
    function RefreshBlogCommentPage(filterParams, usePostback) {
        url = window.location.href;

        index = window.location.href.indexOf('?');
        if (index > 0) {
            url = window.location.href.substring(0, index);
        }
        if (usePostback) {
            postBack();
        }
        else {
            window.location.replace(url + filterParams);
        }
    }

    // Confirm mass delete
    function MassConfirm(dropdown, msg) {
        var drop = document.getElementById(dropdown);
        if (drop != null) {
            if (drop.value == "delete") {
                return confirm(msg);
            }
            return true;
        }
        return true;
    }       
    -->
</script>
