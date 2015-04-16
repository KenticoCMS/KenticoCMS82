<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ImportExport_Controls_ImportWizard"
    CodeFile="ImportWizard.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/Wizard/Header.ascx" TagName="WizardHeader" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ImportExport/Controls/ImportPanel.ascx" TagName="ImportPanel"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ImportExport/Controls/ImportConfiguration.ascx" TagName="ImportConfiguration"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ImportExport/Controls/ImportSiteDetails.ascx" TagName="ImportSiteDetails"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/System/ActivityBar.ascx" TagName="ActivityBar"
    TagPrefix="cms" %>


<script type="text/javascript">
    //<![CDATA[      
    var timerZipId = 0;
    var importTimerId = 0;

    // Start timer function
    function StartImportStateTimer() {
        importTimerId = setInterval("GetImportState(false)", 500);
    }

    // End timer function
    function StopImportStateTimer() {
        if (importTimerId) {
            clearInterval(importTimerId);
            importTimerId = 0;

            if (window.HideActivity) {
                window.HideActivity();
            }
        }
    }

    // End timer function
    function StopUnzipTimer() {
        if (timerZipId) {
            clearInterval(timerZipId);
            timerZipId = 0;

            if (window.HideActivity) {
                window.HideActivity();
            }
        }
    }

    // Start timer function
    function StartUnzipTimer() {
        if (window.Activity) {
            timerZipId = setInterval("window.Activity()", 500);
        }
    }

    // Cancel import
    function CancelImport() {
        GetImportState(true);
        return false;
    }
    //]]>
</script>

<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
<asp:Panel ID="pnlWrapper" runat="Server">
    <div class="GlobalWizard">
        <table>
            <tr class="Top">
                <td class="Left">&nbsp;
                </td>
                <td class="Center">
                    <cms:WizardHeader ID="ucHeader" runat="server" />
                </td>
                <td class="Right">&nbsp;
                </td>
            </tr>
            <tr class="Middle">
                <td class="Center" colspan="3">
                    <div id="wzdBody">
                        <cms:CMSWizard ID="wzdImport" ShortID="w" runat="server" DisplaySideBar="False" NavigationButtonStyle-Width="100"
                            NavigationStyle-HorizontalAlign="Right" CssClass="Wizard">
                            <StartNavigationTemplate>
                                <div class="WizardProgress">
                                    <div id="actDiv" style="display: none;">
                                        <div class="WizardProgressLabel">
                                            <cms:LocalizedLabel ID="lblActivityInfo" runat="server" Text="{$Install.ActivityInfo$}" />
                                        </div>
                                        <cms:ActivityBar runat="server" ID="barActivity" Visible="true" />
                                    </div>
                                </div>
                                <div id="buttonsDiv" class="WizardButtons">
                                    <cms:LocalizedButton UseSubmitBehavior="True" ID="StepNextButton" runat="server"
                                        CommandName="MoveNext" Text="{$general.next$}" ButtonStyle="Primary" OnClientClick="return NextStepAction();"
                                        RenderScript="true" />
                                </div>
                            </StartNavigationTemplate>
                            <StepNavigationTemplate>
                                <div id="buttonsDiv" class="WizardButtons">
                                    <cms:LocalizedButton UseSubmitBehavior="True" ID="StepPreviousButton" runat="server"
                                        CommandName="MovePrevious" Text="{$ExportSiteSettings.PreviousStep$}" ButtonStyle="Primary"
                                        CausesValidation="false" RenderScript="true" /><cms:LocalizedButton UseSubmitBehavior="True"
                                            ID="StepNextButton" runat="server" CommandName="MoveNext" Text="{$general.next$}"
                                            ButtonStyle="Primary" OnClientClick="return NextStepAction();" RenderScript="true" />
                                </div>
                            </StepNavigationTemplate>
                            <FinishNavigationTemplate>
                                <div class="WizardProgress">
                                    <div id="actDiv">
                                        <div class="WizardProgressLabel">
                                            <cms:LocalizedLabel ID="lblActivityInfo" runat="server" Text="{$Import.Progress$}" />
                                        </div>
                                        <cms:ActivityBar runat="server" ID="barActivity" Visible="true" />
                                    </div>
                                </div>
                                <div id="buttonsDiv" class="WizardButtons">
                                    <cms:LocalizedButton UseSubmitBehavior="False" ID="StepCancelButton" runat="server"
                                        Text="{$general.cancel$}" ButtonStyle="Primary" RenderScript="true" /><cms:LocalizedButton
                                            UseSubmitBehavior="True" ID="StepFinishButton" runat="server" Enabled="false"
                                            CommandName="MoveComplete" ResourceString="general.finish" ButtonStyle="Primary"
                                            RenderScript="true" />
                                </div>
                            </FinishNavigationTemplate>
                            <WizardSteps>
                                <asp:WizardStep ID="wzdStepStart" runat="server" AllowReturn="False" StepType="Start"
                                    EnableViewState="true">
                                    <div class="GlobalWizardStep" style="height: <%=PanelHeight%>px">
                                        <cms:ImportConfiguration ID="stpConfigImport" ShortID="c" runat="server" />
                                    </div>
                                </asp:WizardStep>
                                <asp:WizardStep ID="wzdStepSiteDetails" runat="server" AllowReturn="False" StepType="Step"
                                    EnableViewState="true">
                                    <div class="GlobalWizardStep" style="height: <%=PanelHeight%>px">
                                        <cms:ImportSiteDetails ID="stpSiteDetails" runat="server" />
                                    </div>
                                </asp:WizardStep>
                                <asp:WizardStep ID="wzdStepSelection" runat="server" AllowReturn="False" StepType="Step"
                                    EnableViewState="true">
                                    <div class="GlobalWizardStepPanel" style="height: <%=PanelHeight%>px; padding: 0;">
                                        <div class="WizardBorder">
                                            <cms:ImportPanel ID="stpImport" ShortID="p" runat="server" />
                                        </div>
                                    </div>
                                </asp:WizardStep>
                                <asp:WizardStep ID="wzdStepProgress" runat="server" AllowReturn="False" StepType="Finish"
                                    EnableViewState="true">
                                    <div class="GlobalWizardStep" style="height: <%=PanelHeight%>px">
                                        <asp:Label ID="lblProgress" runat="Server" CssClass="WizardLog" EnableViewState="false" />
                                    </div>
                                </asp:WizardStep>
                            </WizardSteps>
                        </cms:CMSWizard>
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
<br />
<asp:Panel ID="pnlPermissions" runat="server" Visible="false" EnableViewState="false">
    <br />
    <asp:HyperLink ID="lnkPermissions" runat="server" />
</asp:Panel>
<asp:HiddenField ID="hdnState" runat="server" />
<asp:Literal ID="ltlScriptAfter" runat="server" EnableViewState="false" />
<cms:AsyncControl ID="ctrlAsync" runat="server" />
