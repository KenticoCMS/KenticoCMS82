<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Controls_VariantSlider"
    CodeFile="VariantSlider.ascx.cs" %>
<%@ Register Src="~/CMSModules/Newsletters/Controls/VariantDialog.ascx" TagPrefix="cms"
    TagName="VariantDialog" %>
<cms:CMSUpdatePanel ID="pnlu" runat="server" EnableViewState="false">
    <ContentTemplate>
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel ID="lblABVar" runat="server" ResourceString="newsletter.lblabvariant"
                            DisplayColon="true" CssClass="control-label" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:PlaceHolder ID="plcSliderPanel" runat="server">
                        <div class="FloatLeft">
                            <asp:Panel ID="pnlSlider" runat="server" CssClass="SliderBarPanel">
                                <cms:CMSTextBox ID="txtSlider" runat="server" EnableViewState="false" />
                                <ajaxToolkit:SliderExtender ID="sliderExtender" runat="server" TargetControlID="txtSlider"
                                    BoundControlID="hdnSliderPosition" EnableHandleAnimation="true" Length="150"
                                    EnableViewState="false" HandleCssClass="slider-horizontal-handle" RailCssClass="slider-horizontal-rail">
                                </ajaxToolkit:SliderExtender>
                                <cms:CMSTextBox ID="hdnSliderPosition" runat="server" EnableViewState="false" />
                            </asp:Panel>
                        </div>
                        <div class="SliderItem">
                            <div class="SliderPartLabel">
                                <div class="FloatLeft">
                                    <asp:Label ID="lblPart" runat="server" Text="1" />
                                </div>
                                <div class="FloatLeft">
                                    /
                                </div>
                                <div class="FloatLeft">
                                    <asp:Label ID="lblTotal" runat="server" Text="1" />
                                </div>
                            </div>
                        </div>
                    </asp:PlaceHolder>
                    <div class="SliderItem">
                        <asp:ImageButton ID="btnDummy" runat="server" Style="position: absolute; left: -1000px; top: -1000px; width: 0px; height: 0px;" OnClientClick="return false;" />
                    </div>
                    <asp:PlaceHolder ID="plcAddVariant" runat="server">
                        <div class="SliderItem">
                            <i id="imgAddVariant" class="icon-plus" aria-hidden="true" runat="server"></i>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plcRemoveVariant" runat="server">
                        <div class="SliderItem">
                            <i id="imgRemoveVariant" class="icon-bin" aria-hidden="true" runat="server"></i>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plcVariantProperties" runat="server">
                        <div class="SliderItem">
                            <i id="imgVariantProperties" class="icon-edit" aria-hidden="true" runat="server"></i>
                        </div>
                    </asp:PlaceHolder>
                    <div class="SliderItemSeparator">
                    </div>
                    <div class="SliderItem">
                        <div class="SliderPartLabel">
                            <div class="FloatLeft">
                                <asp:Label ID="lblVariantName" runat="server" />
                            </div>
                        </div>
                    </div>
                    <cms:VariantDialog ID="variantDailog" runat="server" />
                    <asp:Button ID="btnSubmit" runat="server" CssClass="HiddenButton" />
                </div>
            </div>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
<div class="ClearBoth">
</div>
