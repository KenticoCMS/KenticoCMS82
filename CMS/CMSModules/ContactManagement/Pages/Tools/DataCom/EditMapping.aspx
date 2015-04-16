<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" EnableEventValidation="false" Theme="Default" CodeFile="EditMapping.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_DataCom_EditMapping" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/DataCom/ContactMapping.ascx" TagName="ContactMapping" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/DataCom/CompanyMapping.ascx" TagName="CompanyMapping" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/DataCom/ErrorSummary.ascx" TagName="ErrorSummary" TagPrefix="cms" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:HiddenField ID="MappingHiddenField" runat="server" EnableViewState="false" />
    <asp:Panel ID="MappingPanel" runat="server" EnableViewState="false">
        <cms:ContactMapping ID="ContactMappingControl" runat="server"></cms:ContactMapping>
        <cms:CompanyMapping ID="CompanyMappingControl" runat="server"></cms:CompanyMapping>
    </asp:Panel>
    <asp:Panel runat="server" ID="MainPanel" EnableViewState="false">
        <cms:ErrorSummary ID="ErrorSummary" runat="server" EnableViewState="false" MessagesEnabled="true" />
        <div class="form-horizontal">
            <asp:Repeater ID="MappingItemRepeater" runat="server" OnItemDataBound="MappingItemRepeater_ItemDataBound">
                <ItemTemplate>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel DisplayColon="true" CssClass="control-label" ID="FieldNameLiteral" runat="server" EnableViewState="false" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSDropDownList ID="AttributeNamesDropDownList" runat="server" EnableViewState="false" CssClass="DropDownField" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </asp:Panel>
    <script type="text/javascript">

        $cmsj(document).ready(function ($) {
            var element = document.getElementById('<%= MappingHiddenField.ClientID %>');
            if (element != null && element.value != null && element.value != '') {
                var mapping = element.value;
                var panelElement = document.getElementById('<%= MappingPanel.ClientID %>');
                if (panelElement != null) {
                    var sourceElement = wopener.document.getElementById('<%= SourceMappingHiddenFieldClientId %>');
                    var sourcePanelElement = wopener.document.getElementById('<%= SourceMappingPanelClientId %>');
                    if (sourceElement != null && sourcePanelElement != null) {
                        var html = $cmsj(panelElement).html();
                        sourceElement.value = mapping;
                        $cmsj(sourcePanelElement).html(html);
                        CloseDialog();
                    }
                }
            }
        });

    </script>
</asp:Content>
