<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ImportExport_Controls_ImportGridView"
    CodeFile="ImportGridView.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/Pager/UIPager.ascx" TagName="UIPager"
    TagPrefix="cms" %>
<asp:PlaceHolder ID="plcGrid" runat="server">
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
        <ContentTemplate>
            <asp:Panel ID="pnlGrid" runat="server" CssClass="ObjectContent">
                <asp:PlaceHolder runat="server" ID="plcObjects">
                    <div class="content-block-50">
                        <asp:Label ID="lblInfo" runat="server" EnableViewState="false" />
                    </div>
                    <asp:Panel ID="pnlLinks" runat="server" CssClass="content-block-50 control-group-inline">
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
                                <ItemTemplate>
                                    <asp:Label ID="lblName" runat="server" ToolTip='<%#HttpUtility.HtmlEncode(ValidationHelper.GetString(Eval(codeNameColumnName), ""))%>'
                                        Text='<%#HttpUtility.HtmlEncode(GetName(Eval(codeNameColumnName), Eval(displayNameColumnName)))%>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </cms:UIGridView>
                    <cms:UIPager ID="pagerElem" ShortID="pg" runat="server" DefaultPageSize="10"
                        DisplayPager="true" VisiblePages="5" PagerMode="Postback"/>
                    <asp:HiddenField runat="server" ID="hdnAvailableItems" Value="" EnableViewState="false" />
                </asp:PlaceHolder>
                <asp:Label ID="lblNoData" runat="Server" />
            </asp:Panel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:PlaceHolder>
