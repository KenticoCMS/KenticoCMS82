<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ImportExport_Controls_ExportGridView"
    CodeFile="ExportGridView.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/Pager/UIPager.ascx" TagName="UIPager"
    TagPrefix="cms" %>
<asp:PlaceHolder ID="plcGrid" runat="server">
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
        <ContentTemplate>
            <cms:CMSPanel ID="pnlGrid" ShortID="p" runat="server" CssClass="wizard-section content-block-25">
                <asp:PlaceHolder runat="server" ID="plcObjects">
                    <asp:Panel ID="pnlLinks" runat="server" EnableViewState="false" CssClass="control-group-inline content-block-50">
                        <cms:CMSButton ID="btnAll" runat="server" OnClick="btnAll_Click" ButtonStyle="Default" />
                        <cms:CMSButton ID="btnNone" runat="server" OnClick="btnNone_Click" ButtonStyle="Default" />
                        <cms:CMSButton ID="btnDefault" runat="server" OnClick="btnDefault_Click" ButtonStyle="Default" />
                    </asp:Panel>
                    <asp:Label ID="lblCategoryCaption" runat="Server" />
                    <cms:UIGridView ID="gvObjects" ShortID="go" runat="server" AutoGenerateColumns="False">
                        <HeaderStyle CssClass="unigrid-head" />
                        <Columns>
                            <asp:TemplateField>
                                <HeaderStyle Width="50" />
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderStyle CssClass="main-column-100" />
                                <ItemTemplate>
                                    <asp:Label ID="lblName" runat="server" ToolTip='<%#HttpUtility.HtmlEncode(ValidationHelper.GetString(Eval(codeNameColumnName), ""))%>'
                                        Text='<%#HttpUtility.HtmlEncode(TextHelper.LimitLength(ResHelper.LocalizeString(GetName(Eval(codeNameColumnName), Eval(displayNameColumnName))), 75))%>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </cms:UIGridView>
                    <cms:UIPager ID="pagerElem" ShortID="pg" runat="server" DefaultPageSize="10"
                        DisplayPager="true" VisiblePages="5" PagerMode="Postback" />
                    <asp:HiddenField runat="server" ID="hdnAvailableItems" Value="" EnableViewState="false" />
                </asp:PlaceHolder>
                <asp:Label ID="lblNoData" runat="Server" />
            </cms:CMSPanel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:PlaceHolder>
