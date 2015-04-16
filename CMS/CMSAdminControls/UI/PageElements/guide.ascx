<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_PageElements_guide"
    CodeFile="guide.ascx.cs" %>
<script type="text/javascript">
    //<![CDATA[
    function ShowDesktopContent(contentUrl, fullContent, nodeCodeName) {
        var useCurrent = false;
        if (leftMenuFrame === undefined) {
            leftMenuFrame = parent;
            useCurrent = true;
        }
        if (fullContent) {
            window.top.location.href = contentUrl;
        } else {
            if ((leftMenuFrame != null) && (leftMenuFrame.SelectNode != null) && (nodeCodeName)) {
                leftMenuFrame.SelectNode("#" + nodeCodeName + "");
            }
            else {
                // Handle marking selected menu items
                var allElems = leftMenuFrame.self.document.getElementsByTagName('span');
                for (var i = 0; i < allElems.length; i++) {
                    if (allElems[i].className == 'ContentTreeItem') {
                        if (allElems[i].id == nodeCodeName) {
                            allElems[i].className = 'ContentTreeSelectedItem';
                        }
                    }
                    else {
                        if (allElems[i].className == 'ContentTreeSelectedItem') {
                            allElems[i].className = 'ContentTreeItem';
                        }
                    }
                }
            }
            if (useCurrent) {
                location.href = contentUrl;
            }
            else {
                parent.frames['frameMain'].location.href = contentUrl; // Redirect to menu item       
            }            
        }
    }
    //]]>    
</script>
<asp:Panel ID="plcGuide" runat="server" />
