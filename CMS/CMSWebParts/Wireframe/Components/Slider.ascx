<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Wireframe_Components_Slider"
    CodeFile="~/CMSWebParts/Wireframe/Components/Slider.ascx.cs" %>
<asp:Panel runat="server" ID="pnlBox" CssClass="WireframeSliderBox">
    <div class="WireframeSliderInside">
        <asp:Panel runat="server" ID="pnlSlider" CssClass="WireframeSliderElem">
            <div class="WireframeSliderElemInside">
            </div>
            <cms:WebPartResizer runat="server" ID="resSlider" RenderEnvelope="true" />
        </asp:Panel>
    </div>
    <cms:WebPartResizer runat="server" ID="resBox" RenderEnvelope="true" />
</asp:Panel>
