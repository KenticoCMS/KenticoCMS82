<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ProjectManagement_Controls_UI_ProjectTask_List"
    CodeFile="List.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<cms:UniGrid ID="gridElem" runat="server" GridName="~/CMSModules/ProjectManagement/Controls/UI/ProjectTask/List.xml" />
<cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:ModalPopupDialog runat="server" ID="reminderPopupDialog" CancelControlID="btnReminderCancel"
            BackgroundCssClass="ModalBackground" CssClass="ModalPopupDialog">
            <asp:Panel ID="pnlDialogBody" runat="server" CssClass="DialogPageBody">
                <asp:Panel ID="pnlHeader" runat="server" CssClass="PageHeader" EnableViewState="false">
                    <cms:PageTitle ID="titleElem" runat="server" />
                </asp:Panel>
                <asp:Panel ID="pnlContent" runat="server" CssClass="DialogPageContent DialogScrollableContent">
                    <div class="form-horizontal project-management-task-send-reminder">
                        <cms:LocalizedLabel runat="server" ID="lblReminderError" CssClass="ErrorLabel" EnableViewState="false"
                            Visible="false" />
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel ID="lblProjectDescription" runat="server" EnableViewState="false"
                                    DisplayColon="true" ResourceString="pm.projecttask.remindertext" AssociatedControlID="txtReminderText" CssClass="control-label editing-form-label" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextArea ID="txtReminderText" runat="server" Rows="19" EnableViewState="false" CssClass="form-control" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlFooterContainer">
                    <div class="PageFooterLine">
                        <asp:Panel runat="server" ID="pnlFooter" CssClass="Buttons">
                            <cms:LocalizedButton runat="server" ID="btnReminderOK" ResourceString="general.send"
                                OnClick="btnReminderOK_onClick" ButtonStyle="Primary" />
                            <cms:LocalizedButton runat="server" OnClientClick="return false;" ID="btnReminderCancel"
                                ResourceString="general.cancel" ButtonStyle="Primary" />
                        </asp:Panel>
                    </div>
                </asp:Panel>
            </asp:Panel>
        </cms:ModalPopupDialog>
    </ContentTemplate>
</cms:CMSUpdatePanel>
