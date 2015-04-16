<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Newsletters_Controls_Newsletter_Preview" CodeFile="Newsletter_Preview.ascx.cs" %>

<div id="topPanel" class="header-panel header-container">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblSubscriber" runat="server" EnableViewState="false" ResourceString="selectsubscriber.itemname" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <div class="control-group-inline">
                    <cms:LocalizedButton ButtonStyle="Default" ID="lnkPrevious" ResourceString="general.previous" runat="server" OnClientClick="getPreviousSubscriber(); return false;" />
                    <cms:LocalizedButton ButtonStyle="Default" ResourceString="general.next" ID="lnkNext" runat="server" OnClientClick="getNextSubscriber(); return false;" />
                    <asp:Label CssClass="form-control-text" ID="lblEmail" runat="server" EnableViewState="false" />
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblSubject" runat="server" EnableViewState="false" ResourceString="general.subject" DisplayColon="true" AssociatedControlID="lblSubjectValue" />
            </div>
            <div class="editing-form-value-cell">
                <asp:Label CssClass="form-control-text" ID="lblSubjectValue" runat="server" EnableViewState="false" />
            </div>
        </div>
    </div>
</div>

<iframe id="preview" class="scroll-area dialog-content-frame" frameborder="0"></iframe>

<script type="text/javascript">
    //<![CDATA[
    function SetIFrameHeight(frameId) {
        var F = document.getElementById(frameId);
        var T = document.getElementById('topPanel');
        var contentElem = document.getElementById('divContent');
        if ((F != null) && (contentElem != null)) {
            offset = 0;
            if (T != null) {
                offset = T.offsetHeight;
            }
            F.height = (contentElem.offsetHeight - offset);
        }
    }
    window.afterResize = function () { SetIFrameHeight('preview'); };
    //]]>
</script>

<script type="text/javascript">
<!--
    var previewFrame;
    var emailItem;
    var nextLnk;
    var prevLnk;
    var emailItem;
    var subjItem;

    function InitObjects() {
        previewFrame = document.getElementById('preview');
        emailItem = document.getElementById(lblEmail);
        nextLnk = document.getElementById(lnkNext);
        prevLnk = document.getElementById(lnkPrev);
        subjItem = document.getElementById(lblSubj);
    }

    function pageLoad() {
        InitObjects();

        previewFrame.src = "Newsletter_Issue_ShowPreview.aspx?subscriberguid=" + guid[currentSubscriberIndex] + "&objectid=" + newsletterIssueId;

        if ((guid != null) && (guid.length > 0)) {
            if ((email != null) && (email.length > 0)) {
                emailItem.innerHTML = email[currentSubscriberIndex];
            }
            if ((subject != null) && (subject.length > 0)) {
                subjItem.innerHTML = subject[currentSubscriberIndex];
            }
        }
        else {
            // Hide area with preview iframe
            document.getElementById('preview').style.display = 'none';
        }
    }

    function getPreviousSubscriber() {
        if ((currentSubscriberIndex > 0) && (guid.length > 0) && (email.length > 0) && (subject.length > 0)) {
            currentSubscriberIndex--;
            SetPreview();
        }
    }

    function getNextSubscriber() {
        if ((currentSubscriberIndex < email.length - 1) && (currentSubscriberIndex < subject.length - 1) && (currentSubscriberIndex < guid.length - 1) && (guid.length > 0) && (email.length > 0) && (subject.length > 0)) {
            currentSubscriberIndex++;
            SetPreview();
        }
    }

    function SetPreview() {
        emailItem.innerHTML = email[currentSubscriberIndex];
        subjItem.innerHTML = subject[currentSubscriberIndex];
        previewFrame.src = "Newsletter_Issue_ShowPreview.aspx?subscriberguid=" + guid[currentSubscriberIndex] + "&objectid=" + newsletterIssueId;
    }

    -->
</script>
