<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Blogs_Controls_BlogCommentView" CodeFile="BlogCommentView.ascx.cs" %>

<%@ Register Src="BlogCommentEdit.ascx" TagName="BlogCommentEdit" TagPrefix="cms" %>
<%@ Register Src="NewSubscription.ascx" TagName="NewSubscription" TagPrefix="cms" %>

<asp:Panel ID="pnlTrackbackURL" runat="server" Visible="false" EnableViewState="false" CssClass="TrackbackPanel">
    <cms:LocalizedLabel ID="lblURLTitle" runat="server" EnableViewState="false" ResourceString="blog.commentview.trackbackurlentry"
        DisplayColon="true" CssClass="TrackbackLabel" />
    <asp:Label ID="lblURLValue" runat="server" EnableViewState="false" CssClass="TrackbackURL" /><br />
    <br />
</asp:Panel>
<a id="comments"></a>
<asp:Label ID="lblTitle" runat="server" EnableViewState="false" CssClass="BlogCommentsTitle" />
<div>
    <cms:LocalizedLabel ID="lblInfo" runat="server" EnableViewState="false" CssClass="InfoLabel" ResourceString="blog.commentview.nocomments" />
</div>
<div>
    <asp:Repeater ID="rptComments" runat="server" />
</div>
<asp:Panel ID="pnlComment" runat="server">
    <cms:LocalizedLabel CssClass="control-label blog-leave-comment" ID="lblLeaveCommentLnk" runat="server" EnableViewState="false"
        ResourceString="blog.commentview.lnkleavecomment" AssociatedControlID="plcBtnSubscribe" />
    <asp:PlaceHolder ID="plcBtnSubscribe" runat="server">
        <cms:LocalizedLinkButton ID="btnSubscribe" runat="server" EnableViewState="false"
            ResourceString="blog.commentview.lnksubscription" />
    </asp:PlaceHolder>
    <cms:BlogCommentEdit ID="ctrlCommentEdit" runat="server" />
</asp:Panel>
<asp:Panel ID="pnlSubscription" runat="server">
    <div class="form-horizontal">
        <div class="form-group">
            <cms:LocalizedLabel CssClass="control-label blog-subscribe" ID="lblNewSubscription" runat="server" EnableViewState="false"
                ResourceString="blog.commentview.lnksubscription" AssociatedControlID="btnLeaveMessage" />
            <cms:LocalizedLinkButton ID="btnLeaveMessage" runat="server" EnableViewState="false"
                ResourceString="blog.commentview.lnkleavemessage" />
        </div>
    </div>
    <cms:NewSubscription ID="elemSubscription" runat="server" />
</asp:Panel>
<asp:HiddenField ID="hdnSelSubsTab" runat="server" />

<script type="text/javascript"> 
<!--
    // Refreshes current page when comment properties are changed in modal dialog window
    function RefreshPage() 
    {         
    
        var url = window.location.href;
        
        // String "#comments" found in url -> trim it
        var charIndex = window.location.href.indexOf('#');
        if (charIndex != -1)
        {
            url = url.substring(0, charIndex);
        }
        
        // Refresh page content
        window.location.replace(url);       
    }
    
    // Switches between edit control and subscription control
    function ShowSubscription(subs, hdnField, elemEdit, elemSubscr) {
        if (hdnField && elemEdit && elemSubscr) 
        {
            var hdnFieldElem = document.getElementById(hdnField);
            var elemEditElem = document.getElementById(elemEdit);
            var elemSubscrElem = document.getElementById(elemSubscr);
            if((hdnFieldElem!=null)&&(elemEditElem!=null)&&(elemSubscrElem!=null))
            {
                if (subs == 1) { // Show subscriber control
                    elemEditElem.style.display = 'none';
                    elemSubscrElem.style.display = 'block';
                }
                else
                {                // Show edit control
                    elemEditElem.style.display = 'block';
                    elemSubscrElem.style.display = 'none';
                }
                hdnFieldElem.value = subs;
            }
        }      
    }    
    -->
</script>

