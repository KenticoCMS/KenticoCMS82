<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/CMSModules/AdminControls/Controls/UIControls/GeneralTree.ascx.cs" Inherits="CMSModules_AdminControls_Controls_UIControls_GeneralTree" %>
<%@ Register Src="~/CMSAdminControls/UI/Trees/UniTree.ascx" TagName="UniTree" TagPrefix="cms" %>
<script type="text/javascript" language="javascript">
    //<![CDATA[

    var selectedItemId = 0;
    var selectedItemType = '';
    var selectedItemParent = 0;
    var isExportable = true;

    function updateMenu() {
        var isItem = ((selectedItemId > 0) && (selectedItemType == 'item')),
            isParent = selectedItemParent > 0;

        updateMenuItem('#' + '<%=AddButon.ClientID %>' + ' button:not(".dropdown-toggle")', isItem || !isParent);
        updateMenuItem('#' + '<%=AddButon.ClientID %>' + ' li', isItem, 'disabled');
        updateMenuItem('#' + '<%=DeleteItemButton.ClientID %>', !isParent);
        updateMenuItem('#' + '<%=ExportItemButton.ClientID %>', !isItem || (isExportable != 1));
        updateMenuItem('#' + '<%=CloneItemButton.ClientID %>', !isItem);
    }

    function updateMenuItem(selector, disabled, className) {
        var item = $cmsj(selector);
        if (item.length > 0) {
            if (className) {
                item.toggleClass(className, disabled);
            }

            if (disabled) {
                item.attr('disabled', 'disabled');
            }
            else {
                item.removeAttr('disabled');
            }
        }
    }

    function ExportObject() {
        if ((selectedItemId > 0) && (selectedItemType == 'item')) {
            OpenExportObject('<%=objectType %>', selectedItemId);
        }
    }

    function SelectNode(elementId, type, parentId, updateContent, suffix, itemIsExportable) {
        selectedItemId = elementId;
        selectedItemType = type;
        selectedItemParent = parentId;
        isExportable = itemIsExportable;

        // Set selected item in tree
        $cmsj('span[name=treeNode]').each(function () {
            var jThis = $cmsj(this);

            jThis.removeClass('ContentTreeSelectedItem');

            if (this.id == type + '_' + elementId) {
                jThis.addClass('ContentTreeSelectedItem');
            }
        });

        suffix = (typeof suffix === "undefined") ? "" : suffix;

        // Update frames URLs
        if (updateContent) {
            if (doNotReloadContent) {
                doNotReloadContent = false;
            } else {
                selectStartPage(elementId, type, parentId, suffix);
            }
        }

        updateMenu();
    }

    function selectStartPage(elementId, type, parentId, suffix) {
        var contentFrame = frames['paneContentTMain'];
        contentFrame.location = createFrameUrl(elementId, type, parentId) + suffix;
    }

    function createFrameUrl(elementId, type, parentId) {
        var par = (type == 'item') ? '&parentobjectid=' + parentId : "";
        return ((type == 'item') ? "<%=itemLocation %>" : "<%=categoryLocation %>") + '&action=edit&objectid=' + elementId + par;
    }

    //]]>
</script>
<asp:Panel runat="server" ID="pnlMain">
    <cms:UILayout runat="server" ID="layoutElem">
        <Panes>
            <cms:UILayoutPane ID="paneMenu" runat="server" Direction="North" RenderAs="Div" SpacingOpen="0" PaneClass="ui-layout-pane-visible">
                <Template>
                    <div class="tree-buttons-panel">
                        <cms:CMSMoreOptionsButton ID="btnAdd" runat="server" />
                        <cms:CMSAccessibleButton runat="server" ID="btnDelete" IconCssClass="icon-bin" IconOnly="true" />
                        <cms:CMSAccessibleButton runat="server" ID="btnExport" IconCssClass="icon-arrow-right-rect" IconOnly="true" />
                        <cms:CMSAccessibleButton runat="server" ID="btnClone" IconCssClass="icon-doc-copy" IconOnly="true" />
                    </div>
                </Template>
            </cms:UILayoutPane>
            <cms:UILayoutPane ID="paneTree" runat="server" Direction="Center" RenderAs="Div" PaneClass="ContentTreeArea" SpacingOpen="0">
                <Template>
                    <div class="TreeAreaTree">
                        <cms:MessagesPlaceHolder runat="server" ID="pnlMessages" />
                        <cms:UniTree runat="server" ShortID="t" ID="treeElem" CollapseAll="false" GeneralIDs="true" MultipleRoots="true" />
                    </div>
                </Template>
            </cms:UILayoutPane>
        </Panes>
    </cms:UILayout>
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Panel>
