<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ExportModule.ascx.cs" Inherits="CMSModules_Modules_Controls_NuGetPackages_ExportModule" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:PlaceHolder runat="server" ID="plcStep1">
    <cms:LocalizedHeading runat="server" ID="lblModuleInfoTitle" Level="4" ResourceString="cms.modules.installpackage.dialogtitle"></cms:LocalizedHeading>
    <cms:UIForm runat="server" ID="dataForm" DefaultFieldLayout="TwoColumns" FieldGroupHeadingIsAnchor="False" ObjectType="cms.resource">
        <LayoutTemplate>
            <cms:FormField runat="server" ID="ffModuleDisplayName" FormControl="LabelControl" Field="none" ResourceString="cms.modules.installpackage.title" DisplayColon="true" />
            <cms:FormField runat="server" ID="ffModuleName" FormControl="LabelControl" Field="none" ResourceString="cms.modules.installpackage.id" DisplayColon="true" />
            <cms:FormField runat="server" ID="ffModuleDescription" FormControl="LabelControl" Field="none" ResourceString="cms.modules.installpackage.description" DisplayColon="true" />
            <cms:FormField runat="server" ID="ffModuleVersion" FormControl="LabelControl" Field="none" ResourceString="cms.modules.installpackage.version" DisplayColon="true" />
            <cms:FormField runat="server" ID="ffModuleAuthor" FormControl="LabelControl" Field="none" ResourceString="cms.modules.installpackage.authors" DisplayColon="true" />
        </LayoutTemplate>
    </cms:UIForm>
    <cms:LocalizedHeading runat="server" ID="lblFilesTitle" Level="5" ResourceString="general.files" DisplayColon="True"></cms:LocalizedHeading>
    <div class="form-horizontal">
        <cms:UniGrid runat="server" ID="gridFiles">
            <GridColumns>
                <ug:Column runat="server" Source="Path" Caption="$general.path$" Wrap="false" AllowSorting="false" />
                <ug:Column runat="server" CssClass="filling-column" />
            </GridColumns>
        </cms:UniGrid>
    </div>

    <cms:LocalizedHeading runat="server" ID="lblObjectsTitle" Level="5" ResourceString="general.objects" DisplayColon="True"></cms:LocalizedHeading>

    <div class="form-horizontal">
        <cms:UniGrid runat="server" ID="gridObjects">
            <GridColumns>
                <ug:Column runat="server" Source="ObjectType" Caption="$general.objecttype$" Wrap="false" AllowSorting="false"></ug:Column>
                <ug:Column runat="server" Source="Name" Caption="$general.name$" Wrap="false" AllowSorting="false"></ug:Column>
                <ug:Column runat="server" CssClass="filling-column" />
            </GridColumns>
        </cms:UniGrid>
    </div>

    <cms:LocalizedLabel runat="server" ResourceString="cms.modules.installpackage.additionalobjects" CssClass="additional-objects-label"></cms:LocalizedLabel>
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="plcStep2" Visible="False">
    <strong>
        <asp:Literal runat="server" ID="ltlExportResult"></asp:Literal>
    </strong>
</asp:PlaceHolder>
