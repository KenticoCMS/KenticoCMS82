<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ImportExport_Controls_Import___objects__"
    CodeFile="__objects__.ascx.cs" %>

<script type="text/javascript">
    //<![CDATA[
    function CheckChange() {
        for (i = 0; i < im_g_childIDs.length; i++) {
            var child = document.getElementById(im_g_childIDs[i]);
            if (child != null) {
                var name = im_g_childIDNames[i];
                if ((name == 'asbl') || (name == 'code')) {
                    child.checked = false;
                    if (im_g_isPrecompiled) {
                        child.disabled = true;
                    } else {
                        child.disabled = !im_g_parent.checked;
                    }
                }
                else {
                    child.checked = im_g_parent.checked;
                    child.disabled = !im_g_parent.checked;
                }
            }
        }
    }

    function InitCheckboxes() {
        if (!im_g_parent.checked) {
            for (i = 0; i < im_g_childIDs.length; i++) {
                var child = document.getElementById(im_g_childIDs[i]);
                if (child != null) {
                    child.disabled = true;
                }
            }
        }
    }
    //]]>
</script>

<asp:Panel runat="server" ID="pnlWarning" CssClass="wizard-section" Visible="false">
    <asp:Label ID="lblWarning" runat="server" EnableViewState="false" />
</asp:Panel>
<asp:Panel runat="server" ID="pnlInfo" CssClass="wizard-section content-block-25">
    <asp:Label ID="lblInfo2" runat="server" EnableViewState="false" />
    <asp:Label ID="lblInfo" runat="server" EnableViewState="false" />
</asp:Panel>
<asp:Panel runat="server" ID="pnlSelection" CssClass="wizard-section content-block-25">
    <cms:LocalizedHeading ID="headSelection" runat="server" EnableViewState="false" Level="4" CssClass="listing-title" ResourceString="ImportObjects.Selection" />
    <div class="control-group-inline control-group-inline-wrap">
        <cms:CMSButton ID="lnkSelectDefault" runat="server" OnClick="lnkSelectDefault_Click" ButtonStyle="Default" />
        <cms:CMSButton ID="lnkSelectAll" runat="server" OnClick="lnkSelectAll_Click" ButtonStyle="Default" />
        <cms:CMSButton ID="lnkSelectNew" runat="server" OnClick="lnkSelectNew_Click" ButtonStyle="Default" />
        <cms:CMSButton ID="lnkSelectNone" runat="server" OnClick="lnkSelectNone_Click" ButtonStyle="Default" />
    </div>
</asp:Panel>
<asp:Panel runat="server" ID="pnlCheck" CssClass="wizard-section content-block-50">
    <cms:LocalizedHeading ID="headSettings" Level="4" runat="server" EnableViewState="false" CssClass="listing-title" ResourceString="ImportObjects.Settings" />
    <div class="form-horizontal">
        <div class="checkbox-list-vertical">
            <asp:PlaceHolder runat="server" ID="plcSite" Visible="false">
                <asp:PlaceHolder ID="plcExistingSite" runat="Server" Visible="false">
                    <cms:CMSCheckBox ID="chkUpdateSite" runat="server" />
                </asp:PlaceHolder>
                <cms:CMSCheckBox ID="chkBindings" runat="server" />
                <cms:CMSCheckBox ID="chkRunSite" runat="server" />
                <cms:CMSCheckBox ID="chkDeleteSite" runat="server" />
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="plcOverwriteQueries" Visible="false">
                <cms:CMSCheckBox ID="chkOverwriteSystemQueries" runat="server" />
            </asp:PlaceHolder>
            <cms:CMSCheckBox ID="chkSkipOrfans" runat="server" />
            <cms:CMSCheckBox ID="chkImportTasks" runat="server" />
            <cms:CMSCheckBox ID="chkLogSync" runat="server" />
            <cms:CMSCheckBox ID="chkLogInt" runat="server" />
            <cms:CMSCheckBox ID="chkCopyFiles" runat="server" />
            <div class="selector-subitem">
                <div class="checkbox-list-vertical">
                    <cms:CMSCheckBox ID="chkCopyCodeFiles" runat="server" />
                    <cms:CMSCheckBox ID="chkCopyAssemblies" runat="server" />
                    <cms:CMSCheckBox ID="chkCopyGlobalFiles" runat="server" />
                    <asp:PlaceHolder ID="plcSiteFiles" runat="server" Visible="false">
                        <cms:CMSCheckBox ID="chkCopySiteFiles" runat="server" />
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>
    </div>

</asp:Panel>
<asp:Literal ID="ltlScript" EnableViewState="false" runat="Server" />
