<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WebPartProperties_layout_menu.aspx.cs"
    Inherits="CMSModules_PortalEngine_UI_WebParts_WebPartProperties_layout_menu"
    MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalSimplePage.master" Theme="Default" %>

<%@ Register Src="~/CMSModules/PortalEngine/FormControls/WebPartLayouts/WebPartLayoutSelector.ascx"
    TagPrefix="cms" TagName="LayoutSelector" %>
<asp:Content ContentPlaceHolderID="plcSiteSelector" ID="cntContent" runat="server">
    <div id="pnlContent" runat="server">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" ID="lblLayouts" runat="server" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:LayoutSelector runat="server" ID="selectLayout" OnChanged="drpLayouts_Changed"
                        IsLiveSite="false" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
