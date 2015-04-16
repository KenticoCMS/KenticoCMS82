<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Blogs_Controls_NewSubscription" CodeFile="NewSubscription.ascx.cs" %>
<asp:Panel ID="pnlContent" runat="server" CssClass="BoardNewPost">
    <asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" Visible="false" EnableViewState="false" />
    <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" Visible="false" EnableViewState="false" />
    <asp:Panel runat="server" ID="pnlPadding" CssClass="FormPadding" DefaultButton="btnOK">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel ID="lblEmail" CssClass="control-label" runat="server" AssociatedControlID="txtEmail" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtEmail" runat="server" MaxLength="100" /><br />
                    <cms:CMSRequiredFieldValidator ID="rfvEmailRequired" runat="server" ControlToValidate="txtEmail"
                        Display="Dynamic" EnableViewState="false" />
                    <cms:CMSRegularExpressionValidator ID="revEmailValid" runat="server" ControlToValidate="txtEmail"
                        Display="Dynamic" EnableViewState="false" />
                </div>
            </div>
            <div class="form-group form-group-submit">
                <cms:LocalizedButton ID="btnOk" runat="server" ButtonStyle="Primary" OnClick="btnOK_Click"
                    EnableViewState="false" />
            </div>
        </div>
    </asp:Panel>
</asp:Panel>
