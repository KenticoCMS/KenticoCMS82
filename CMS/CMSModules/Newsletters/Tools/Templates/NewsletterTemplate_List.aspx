<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NewsletterTemplate_List.aspx.cs"
    Inherits="CMSModules_Newsletters_Tools_Templates_NewsletterTemplate_List" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Tools - Newsletter templates" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:UniGrid runat="server" ID="UniGrid" ShortID="g" OrderBy="TemplateDisplayName"
        IsLiveSite="false" ObjectType="newsletter.emailtemplate" Columns="TemplateID, TemplateDisplayName, TemplateType" RememberStateByParam="">
        <GridActions>
            <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="TemplateDisplayName" Caption="$Unigrid.NewsletterTemplate.Columns.TemplateDisplayName$"
                Wrap="false" Localize="true">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="TemplateType" ExternalSourceName="templatetype" Caption="$general.type$" Wrap="false">
                <Filter Path="~/CMSModules/Newsletters/Controls/TemplateFilter.ascx" />
            </ug:Column>
            <ug:Column CssClass="filling-column" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>
