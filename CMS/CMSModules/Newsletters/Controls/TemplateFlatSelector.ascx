<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TemplateFlatSelector.ascx.cs" Inherits="CMSModules_Newsletters_Controls_TemplateFlatSelector" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniFlatSelector.ascx" TagName="UniFlatSelector"
    TagPrefix="cms" %>

<script type="text/javascript">
    //<![CDATA[
    // Javacript after async postback
    function pageLoad() {
        // Resizes area
        if ($cmsj.isFunction(window.resizearea)) {
            resizearea();
        }

        // Uniflat search
        setTimeout('Focus()', 100);
        var timer = null;
        SetupSearch();
    }

</script>

<asp:Panel ID="pnlSelector" runat="server" CssClass="ItemSelector">
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="LayoutSelector">
                <cms:UniFlatSelector ID="flatElem" runat="server">
                    <HeaderTemplate>
                        <div class="SelectorFlatItems">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="SelectorEnvelope" style="overflow: hidden">
                            <div class="SelectorFlatImage">
                                <img alt="Layout image" src="<%#flatElem.GetFlatImageUrl(Eval("TemplateThumbnailGUID"))%>" />
                            </div>
                            <span class="SelectorFlatText">
                                <%#HTMLHelper.HTMLEncode(ResHelper.LocalizeString(Convert.ToString(Eval("TemplateDisplayName"))))%></span>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                        <div style="clear: both">
                        </div>
                        </div>
                    </FooterTemplate>
                </cms:UniFlatSelector>
                <div class="selector-flat-description">
                    <asp:Literal runat="server" ID="litCategory" EnableViewState="false"></asp:Literal>
                </div>
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Panel>
