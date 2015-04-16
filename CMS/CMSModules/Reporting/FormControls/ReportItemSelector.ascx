<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Reporting_FormControls_ReportItemSelector"
    CodeFile="ReportItemSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:HiddenField ID="hdnGuid" runat="server" />
        <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
        <div>
            <asp:Panel runat="server" ID="pnlReports">
                <cms:UniSelector ID="usReports" runat="server" ObjectType="reporting.report" SelectionMode="SingleDropDownList"
                    AllowEditTextBox="false" IsLiveSite="false" DisplayNameFormat="{%ReportDisplayName%}"
                    ReturnColumnName="ReportName" />
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlItems">
                <cms:UniSelector ID="usItems" runat="server" SelectionMode="SingleDropDownList" AllowEditTextBox="false"
                    Enabled="false" />
            </asp:Panel>
            <asp:Panel CssClass="ReportParametersWebPartPanel" runat="server" ID="pnlParameters"
                Visible="false">
                <div class="ReportParametersWebPartTable">
                    <asp:Panel runat="server" Width="297px">
                        <br />
                        <cms:UniGrid ID="ugParameters" runat="server" IsLiveSite="false" />
                    </asp:Panel>
                </div>
            </asp:Panel>
            <div id="plcParametersButtons" runat="server" style="margin-left: -2px" visible="false">
                <br />
                <cms:LocalizedButton ID="btnSet" runat="server" ResourceString="rep.webparts.setparameters"
                    OnClick="btnSet_Click" ButtonStyle="Default" />
                <cms:LocalizedButton ID="btnClear" runat="server" ResourceString="rep.webparts.clearparameters"
                    OnClick="btnClear_Click" ButtonStyle="Default" />
            </div>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
