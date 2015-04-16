<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAPIExamples_Controls_APIExample"
    CodeFile="APIExample.ascx.cs" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <div>
            <div class="api-examples-action-buttons">
                <asp:Label ID="lblNumber" runat="server" EnableViewState="false" />
                <cms:CMSButton ID="btnAction" runat="server" EnableViewState="false" OnClick="btnAction_Click" ButtonStyle="Default" />
                <cms:CMSAccessibleButton runat="server" ID="btnShowCode" CausesValidation="false" EnableViewState="false" 
                    OnClick="btnShowCode_Click"  IconCssClass="icon-magnifier" ScreenReaderDescription="View code" CssClass="btn-icon" />
            </div>
            <asp:Label ID="lblInfo" runat="server" EnableViewState="false" Visible="false" CssClass="InfoLabel" />
            <asp:Label ID="lblError" runat="server" EnableViewState="false" Visible="false" CssClass="ErrorLabel" />
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
