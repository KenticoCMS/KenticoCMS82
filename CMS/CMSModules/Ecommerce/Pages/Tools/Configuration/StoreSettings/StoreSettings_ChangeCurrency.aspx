<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_StoreSettings_StoreSettings_ChangeCurrency"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    CodeFile="StoreSettings_ChangeCurrency.aspx.cs" %>

<%@ Register Src="~/CMSModules/ECommerce/FormControls/CurrencySelector.ascx" TagName="CurrencySelector"
    TagPrefix="cms" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <div class="form-horizontal">
        <asp:PlaceHolder runat="server" ID="plcOldCurrency">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblOldMainLabel" runat="server" EnableViewState="false" ResourceString="StoreSettings_ChangeCurrency.OldMainCurrency" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label CssClass="form-control-text" ID="lblOldMainCurrency" runat="server" EnableViewState="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblNewMainCurrency" runat="server" EnableViewState="false"
                    ResourceString="StoreSettings_ChangeCurrency.NewMainCurrency" ShowRequiredMark="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CurrencySelector runat="server" ID="currencyElem" UseNameForSelection="false" AddAllItemsRecord="false" AddSiteDefaultCurrency="false"
                    ExcludeSiteDefaultCurrency="true" IsLiveSite="false" ShowAllItems="true" EnsureSelectedItem="true" />
            </div>
        </div>
        <asp:PlaceHolder runat="server" ID="plcRecalculationDetails">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblExchangeRate" runat="server" EnableViewState="false" ResourceString="StoreSettings_ChangeCurrency.ExchangeRate" ShowRequiredMark="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtEchangeRate" runat="server" MaxLength="10" EnableViewState="false" />
                    <span class="info-icon">
                        <cms:LocalizedLabel runat="server" ResourceString="StoreSettings_ChangeCurrency.ExchangeRateHelp" CssClass="sr-only"></cms:LocalizedLabel>
                        <cms:CMSIcon ID="imgHelp" runat="server" EnableViewState="false" CssClass="icon-question-circle" />
                    </span>
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblRound" runat="server" EnableViewState="false" ResourceString="StoreSettings_ChangeCurrency.Round" ShowRequiredMark="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtRound" runat="server" EnableViewState="false"
                        MaxLength="1" Text="2" />
                    <span class="info-icon">
                        <cms:LocalizedLabel runat="server" ResourceString="StoreSettings_ChangeCurrency.ExchangeRateRoundHelp" CssClass="sr-only"></cms:LocalizedLabel>
                        <cms:CMSIcon ID="imgRoundHelp" runat="server" EnableViewState="false" CssClass="icon-question-circle" />
                    </span>
                </div>
            </div>
        </asp:PlaceHolder>
    </div>
    <asp:PlaceHolder runat="server" ID="plcObjectsSelection">
        <div id="recalculationDetail" class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblRecalculate" runat="server" EnableViewState="false" ResourceString="Recalculate" />
                </div>
                <div class="editing-form-value-cell">
                    <div class="checkbox-list-vertical">
                        <asp:PlaceHolder ID="plcRecalculateFromGlobal" runat="server">
                            <div class="checkbox">
                                <cms:CMSCheckBox ID="chkRecalculateFromGlobal" runat="server" ResourceString="StoreSettings_ChangeCurrency.ChangeSiteFromGlobal"
                                    Checked="true" EnableViewState="false" />
                            </div>
                        </asp:PlaceHolder>
                        <div class="checkbox">
                            <cms:CMSCheckBox ID="chkExchangeRates" runat="server" ResourceString="StoreSettings_ChangeCurrency.ChangeExchangeRates"
                                Checked="true" EnableViewState="false" />
                        </div>
                        <div class="checkbox">
                            <cms:CMSCheckBox ID="chkProductPrices" runat="server" ResourceString="StoreSettings_ChangeCurrency.ChangeProductPrices"
                                Checked="true" EnableViewState="false" />
                        </div>
                        <div class="checkbox">
                            <cms:CMSCheckBox ID="chkFlatTaxes" runat="server" ResourceString="StoreSettings_ChangeCurrency.ChangeTaxes"
                                Checked="true" EnableViewState="false" />
                        </div>
                        <div class="checkbox">
                            <cms:CMSCheckBox ID="chkDiscounts" runat="server" ResourceString="StoreSettings_ChangeCurrency.ChangeDiscounts"
                                Checked="true" EnableViewState="false" />
                        </div>
                        <div class="checkbox">
                            <cms:CMSCheckBox ID="chkCredit" runat="server" ResourceString="StoreSettings_ChangeCurrency.ChangeCredit"
                                Checked="true" EnableViewState="false" />
                        </div>
                        <div class="checkbox">
                            <cms:CMSCheckBox ID="chkShipping" runat="server" ResourceString="StoreSettings_ChangeCurrency.ChangeShipping"
                                Checked="true" EnableViewState="false" />
                        </div>
                        <asp:PlaceHolder ID="plcRecountDocuments" runat="server">
                            <div class="checkbox">
                                <cms:CMSCheckBox ID="chkDocuments" runat="server" ResourceString="StoreSettings_ChangeCurrency.ChangeDocuments"
                                    Checked="true" EnableViewState="false" />
                            </div>
                        </asp:PlaceHolder>
                    </div>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
    <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
</asp:Content>
<asp:Content ID="cntFooter" runat="server" ContentPlaceHolderID="plcFooter">
    <cms:CMSButton ID="btnOk" runat="server" ButtonStyle="Primary" OnClick="btnOk_Click"
        EnableViewState="false" />
</asp:Content>
