<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Sites_Pages_Site_Delete"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Delete Site"
    CodeFile="Site_Delete.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/Wizard/Header.ascx" TagName="WizardHeader" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/System/ActivityBar.ascx" TagName="ActivityBar"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <script type="text/javascript" language="javascript">
        //<![CDATA[
        var timerId = 0;
        var i = 0;

        function GetStateAction() {
            if (window.Activity) {
                window.Activity();
            }
            if (i++ >= 10) {
                i = 0;
                try {
                    GetState(false);
                }
                catch (ex) {
                }
            }
        }

        // End timer function
        function StopStateTimer() {
            if (timerId) {
                clearInterval(timerId);
                timerId = 0;
            }
        }

        // Start timer function
        function StartStateTimer() {
            timerId = setInterval("GetStateAction()", 100);
        }
        //]]>
    </script>
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
    <asp:Panel ID="pnlConfirmation" runat="server" CssClass="PageContent">
        <div class="GlobalWizard">
            <table>
                <tr class="Top">
                    <td class="Left">&nbsp;
                    </td>
                    <td class="Center" style="width: 840px;">
                        <cms:WizardHeader ID="ucHeaderConfirm" runat="server" TitleVisible="false" DescriptionVisible="false" />
                    </td>
                    <td class="Right">&nbsp;
                    </td>
                </tr>
                <tr class="Middle">
                    <td class="Center" colspan="3">
                        <asp:Panel runat="server" ID="Panel1" CssClass="GlobalWizardStep" Height="280">
                            <div>
                                <div>
                                    <cms:LocalizedHeading runat="server" ID="headConfirmation" Level="3" EnableViewState="false" />
                                    <div class="checkbox-list-vertical">
                                        <cms:CMSCheckBox ID="chkDeleteDocumentAttachments" runat="server" ResourceString="DeleteSite.DocumentAttachments"
                                            EnableViewState="false" Checked="true" />
                                        <cms:CMSCheckBox ID="chkDeleteMetaFiles" runat="Server" ResourceString="DeleteSite.MetaFiles"
                                            EnableViewState="false" Checked="true" />
                                        <cms:CMSCheckBox ID="chkDeleteMediaFiles" runat="Server" ResourceString="DeleteSite.MediaFiles"
                                            EnableViewState="false" Checked="true" />
                                    </div>
                                </div>
                                <br />
                            </div>
                        </asp:Panel>
                        <div class="WizardButtons">
                            <cms:CMSButton ID="btnYes" runat="server" ButtonStyle="Primary" />
                            <cms:CMSButton
                                ID="btnNo" runat="server" ButtonStyle="Primary" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlDeleteSite" runat="server" CssClass="PageContent" Visible="false">
        <div class="GlobalWizard">
            <table>
                <tr class="Top">
                    <td class="Left">&nbsp;
                    </td>
                    <td class="Center" style="width: 840px;">
                        <cms:WizardHeader ID="ucHeader" runat="server" TitleVisible="false" DescriptionVisible="false" />
                    </td>
                    <td class="Right">&nbsp;
                    </td>
                </tr>
                <tr class="Middle">
                    <td class="Center" colspan="3">
                        <asp:Panel ID="pnlExportPanel" runat="server">
                            <asp:Panel runat="server" ID="pnlContent" CssClass="GlobalWizardStep" Height="300">
                                <div style="height: 280px;">
                                    <asp:Label runat="server" ID="lblFinish" CssClass="InfoLabel" EnableViewState="true" />
                                    <asp:Label runat="server" ID="lblLog" CssClass="InfoLabel" EnableViewState="true" />
                                </div>
                            </asp:Panel>
                        </asp:Panel>
                        <div class="WizardProgress">
                            <div id="actDiv">
                                <div class="WizardProgressLabel">
                                    <cms:LocalizedLabel ID="lblActivityInfo" runat="server" ResourceString="site_delete.footer"
                                        EnableViewState="false" />
                                </div>
                                <cms:ActivityBar runat="server" ID="barActivity" Visible="true" />
                            </div>
                        </div>
                        <div class="WizardButtons">
                            <cms:CMSButton ID="btnOk" runat="server" ButtonStyle="Primary" Enabled="false"
                                RenderScript="true" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <asp:Panel ID="pnlError" runat="server" CssClass="GlobalWizard">
            <div class="alert-error alert">
                <span class="alert-icon">
                    <i class="icon-times-circle"></i>
                    <span class="sr-only"><%= GetString("general.error") %></span>
                </span>
                <asp:Label ID="lblError" runat="server" CssClass="alert-label" EnableViewState="false" />
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlWarning" runat="server" CssClass="GlobalWizard">
            <div class="alert-warning alert" style="display:block; ">
                <span class="alert-icon">
                    <i class="icon-exclamation-triangle"></i>
                    <span class="sr-only"><%= GetString("general.warning") %></span>
                </span>
                <asp:Label ID="lblWarning" runat="server" CssClass="alert-label" EnableViewState="false" />
            </div>
        </asp:Panel>
    </asp:Panel>
    <asp:HiddenField ID="hdnLog" runat="server" EnableViewState="false" />
</asp:Content>
