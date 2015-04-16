<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SiteProgress.ascx.cs"
    Inherits="CMSInstall_Controls_WizardSteps_SiteProgress" %>
<asp:Panel ID="pnlProgress" runat="server">
    <div class="install-progress">
        <table class="install-wizard" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td align="left" style="vertical-align: top">
                    <div class="install-progress-site">
                        <div style="margin: 5px 0px 0px 5px;">
                            <asp:Literal ID="ltlProgress" runat="server" />
                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
