<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RelatedDocuments.ascx.cs"
    Inherits="CMSModules_Content_FormControls_Relationships_RelatedDocuments" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<div class="RelatedDocuments">
    <asp:Panel ID="pnlNewLink" runat="server" Style="margin-bottom: 5px;">
        <cms:LocalizedButton runat="server" ID="btnNewRelationship" ButtonStyle="Default" ResourceString="relationship.addrelateddocs" EnableViewState="false"/>
    </asp:Panel>
    <div>
        <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
            <ContentTemplate>
                <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
                <cms:UniGrid ID="UniGridRelationship" runat="server" GridName="~/CMSModules/Content/FormControls/Relationships/RelatedDocuments_List.xml"
                    OrderBy="RelationshipNameID" IsLiveSite="false" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </div>
    <asp:HiddenField ID="hdnSelectedNodeId" runat="server" Value="" />
</div>
