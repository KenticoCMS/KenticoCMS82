<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" EnableEventValidation="false" Theme="Default" CodeFile="SelectContact.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_DataCom_SelectContact" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/DataCom/ContactFilter.ascx" TagName="ContactFilter" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/DataCom/Contact.ascx" TagName="Contact" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/DataCom/ErrorSummary.ascx" TagName="ErrorSummary" TagPrefix="cms" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="plcContent" runat="Server">
    <script type="text/javascript">
        //<![CDATA[
        function DataCom_ConfirmSelection() {
            var element = document.getElementById('<%= ContactHiddenField.ClientID %>');
            if (element != null) {
                var content = element.value;
                if (content != null && content != "") {
                    wopener.DataCom_SetContact(content);
                    CloseDialog();
                }
            }
        }
        //]]>
    </script>
    <asp:HiddenField ID="ContactHiddenField" runat="server" EnableViewState="false" />
    <asp:Panel ID="MainPanel" runat="server">
        <cms:ErrorSummary ID="ErrorSummary" runat="server" EnableViewState="false" MessagesEnabled="true"></cms:ErrorSummary>
        <cms:ContactFilter ID="ContactFilterControl" runat="server"></cms:ContactFilter>
        <table style="width: 100%">
            <tbody>
                <tr>
                    <td style="vertical-align: top; width: 50%">
                        <cms:UniGrid ID="ContactGrid" runat="server" ShortID="g" Columns="ContactId,LastName,FirstName,CompanyName" IsLiveSite="false" DelayedReload="true" RememberState="false">
                            <GridActions>
                                <ug:Action Name="select" Caption="$general.select$" FontIconClass="icon-chevron-right" />
                            </GridActions>
                            <GridColumns>
                                <ug:Column Source="LastName" Caption="$datacom.contact.lastname$" Wrap="false" />
                                <ug:Column Source="FirstName" Caption="$datacom.contact.firstname$" Wrap="false" />
                                <ug:Column Source="CompanyName" Caption="$datacom.contact.companyname$" Wrap="false" />
                            </GridColumns>
                            <GridOptions ShowSelection="false" />
                            <PagerConfig DefaultPageSize="10" PageSizeOptions="5,10" ShowDirectPageControl="false" ShowFirstLastButtons="false" />
                        </cms:UniGrid>
                    </td>
                    <td style="vertical-align: top; padding-left: 30px">
                        <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="datacom.selectedcontact" EnableViewState="false" Visible="false" />
                        <p id="EmptySelectionControl" runat="server" enableviewstate="false" visible="false">
                            <cms:LocalizedLabel runat="server" EnableViewState="false" ResourceString="datacom.nocontactselected"></cms:LocalizedLabel>
                        </p>
                        <cms:Contact ID="ContactControl" runat="server" EnableViewState="false"></cms:Contact>
                    </td>
                </tr>
            </tbody>
        </table>
    </asp:Panel>
</asp:Content>