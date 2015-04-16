<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Debug_ViewState"
    EnableViewState="false" CodeFile="ViewState.ascx.cs" %>
<cms:UIGridView runat="server" ID="gridStates" ShowFooter="true" AutoGenerateColumns="false">
    <Columns>
        <asp:TemplateField>
            <ItemTemplate>
                <strong><%#GetIndex()%></strong>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <%#TextHelper.EnsureHTMLLineEndings(ValidationHelper.GetString(Eval("ID"), ""))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <%#ColourYesNo(Eval("IsDirty"))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <%#TextHelper.EnsureHTMLLineEndings(ValidationHelper.GetString(Eval("ViewState"), ""))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <%#DataHelper.GetSizeString(ValidationHelper.GetInteger(Eval("ViewStateSize"), 0))%><br />
                <%#GetSizeChart(Eval("ViewStateSize"), 0, 0, 0)%>
            </ItemTemplate>
            <FooterTemplate>
                <asp:PlaceHolder runat="server" ID="plcTotalSize" EnableViewState="false" Visible="<%#DisplayTotalSize%>">
                    <cms:LocalizedLabel runat="server" ID="lblTotal" EnableViewState="false" ResourceString="ViewStateLog.Total" />
                    <strong>
                        <script type="text/javascript">
                            //<![CDATA[
                            var stateElem = document.getElementById("__VIEWSTATE");
                            if (stateElem != null) {
                                document.write(stateElem.value.length);
                            }
                            //]]>
                        </script>
                    </strong>
                    <cms:LocalizedLabel runat="server" ID="lblTotalBytes" EnableViewState="false" ResourceString="ViewStateLog.TotalBytes" />
                </asp:PlaceHolder>
            </FooterTemplate>
        </asp:TemplateField>
    </Columns>
</cms:UIGridView>
