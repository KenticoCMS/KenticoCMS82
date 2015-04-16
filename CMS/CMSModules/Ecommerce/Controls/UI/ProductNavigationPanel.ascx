<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductNavigationPanel.ascx.cs"
    Inherits="CMSModules_Ecommerce_Controls_UI_ProductNavigationPanel" %>
<cms:UILayout runat="server" ID="layoutElem">
    <Panes>
        <cms:UILayoutPane ID="menu" runat="server" Direction="North" Closable="false" Resizable="false"
            SpacingOpen="0" ControlPath="~/CMSModules/Content/Controls/TreeActionsPanel.ascx" />
        <cms:UILayoutPane ID="tree" runat="server" Direction="Center" PaneClass="TreeBody"
            ControlPath="~/CMSModules/Content/Controls/CMSDeskTree.ascx" />
        <cms:UILayoutPane ID="language" runat="server" Direction="South" Closable="false" Resizable="false"
            SpacingOpen="0" ControlPath="~/CMSModules/Content/Controls/TreeLanguageMenu.ascx" PaneClass="ui-layout-pane-visible" />
    </Panes>
</cms:UILayout>
