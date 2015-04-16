<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_AdminControls_Controls_ObjectRelationships_ObjectRelationships"
    CodeFile="ObjectRelationships.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Basic/DropDownListControl.ascx" TagName="DropDownListControl"
    TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<script type="text/javascript">
    //<![CDATA[
    function SetVal(id, value) {
        document.getElementById(id).value = value;
    }

    function SetItems(lObj, ltype, relname, rObj, rtype) {
        SetVal('leftObjId', lObj);
        SetVal('leftObjType', ltype);
        SetVal('relationshipId', relname);
        SetVal('rightObjId', rObj);
        SetVal('rightObjType', rtype);
    }
    //]]>
</script>

<asp:Panel ID="pnlHeader" runat="server" Visible="true">
    <asp:Panel runat="server" ID="pnlSelection" CssClass="header-panel">
        <div class="form-horizontal">
            <asp:PlaceHolder runat="server" ID="pnlTypes">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblObjType" EnableViewState="false" ResourceString="ObjectRelationships.RelatedObjType" AssociatedControlID="drpRelatedObjType" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:DropDownListControl ID="drpRelatedObjType" CssClass="DropDownField" runat="server" MacroSource="ObjectTypes.MainObjectTypes" TextFormat="{% GetObjectTypeName(Item) %}" SortItems="True" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="pnlSite" Visible="false">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSite" EnableViewState="false" ResourceString="ObjectRelationships.RelatedObjectSite" AssociatedControlID="siteSelector" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:UniSelector ID="siteSelector" runat="server" ObjectType="cms.site" ResourcePrefix="siteselect"
                            AllowEmpty="false" AllowAll="false" />
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlNew" CssClass="header-panel">
        <cms:LocalizedButton ResourceString="ObjRelationship.New" OnClick="btnNewRelationship_Click" runat="server"/>
    </asp:Panel>
</asp:Panel>
<asp:Panel runat="server" ID="pnlContent" CssClass="PageContent">
    <input type="hidden" id="leftObjId" name="leftObjId" />
    <input type="hidden" id="leftObjType" name="leftObjType" />
    <input type="hidden" id="relationshipId" name="relationshipId" />
    <input type="hidden" id="rightObjId" name="rightObjId" />
    <input type="hidden" id="rightObjType" name="rightObjType" />
    <asp:Panel ID="pnlEditList" runat="server">
        <cms:UniGrid ID="gridItems" runat="server" OrderBy="RelationshipNameID" ObjectType="cms.objectrelationship" DelayedReload="true"
            IsLiveSite="false" Columns="RelationshipLeftObjectType, RelationshipLeftObjectID, RelationshipNameID, RelationshipRightObjectType, RelationshipRightObjectID">
            <GridActions Parameters="RelationshipLeftObjectID;RelationshipLeftObjectType;RelationshipNameID;RelationshipRightObjectID;RelationshipRightObjectType">
                <ug:Action Name="delete" Caption="$Contribution.Actions.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical"
                    OnClick="SetItems({0},'{1}',{2},{3},'{4}');" Confirmation="$General.ConfirmDelete$" />
            </GridActions>
            <GridColumns>
                <ug:Column Source="##ALL##" ExternalSourceName="left" Caption="$ObjRelationship.LeftSide$"
                    Wrap="false" />
                <ug:Column Source="RelationshipNameID" AllowSorting="false" ExternalSourceName="#transform: cms.relationshipname.RelationshipDisplayName" Caption="$Relationship.RelationshipName$" Wrap="false" />
                <ug:Column Source="##ALL##" ExternalSourceName="right" Caption="$ObjRelationship.RightSide$"
                    Wrap="false" CssClass="main-colum-100" />
            </GridColumns>
            <PagerConfig ShowFirstLastButtons="false" ShowDirectPageControl="false" DefaultPageSize="10" />
            <GridOptions DisplayFilter="false" />
        </cms:UniGrid>
    </asp:Panel>
    <asp:Panel ID="pnlAddNew" runat="server" Visible="false">
        <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Table ID="TableRelationship" runat="server" CssClass="UniGridGrid" CellPadding="-1"
                    CellSpacing="-1" Width="100%">
                    <asp:TableHeaderRow CssClass="UniGridHead" EnableViewState="false">
                        <asp:TableHeaderCell ID="leftCell" HorizontalAlign="Left" Wrap="false" />
                        <asp:TableHeaderCell ID="middleCell" HorizontalAlign="Center" Wrap="false" />
                        <asp:TableHeaderCell ID="rightCell" HorizontalAlign="Right" Wrap="false" />
                    </asp:TableHeaderRow>
                    <asp:TableRow>
                        <asp:TableCell HorizontalAlign="Left" Width="40%" VerticalAlign="Top">
                            <asp:Label ID="lblLeftObj" runat="server" />
                            <cms:UniSelector ID="selLeftObj" runat="server" SelectionMode="SingleDropDownList"
                                AllowEmpty="false" IsLiveSite="false" />
                        </asp:TableCell><asp:TableCell HorizontalAlign="center" Width="20%">
                            <cms:CMSDropDownList ID="drpRelationship" runat="server" CssClass="SelectorDropDown" />
                            <div style="padding-top: 5px">
                                <cms:CMSButton ID="btnSwitchSides" runat="server" ButtonStyle="Default" OnClick="btnSwitchSides_Click" />
                            </div>
                        </asp:TableCell><asp:TableCell HorizontalAlign="right" Width="40%" VerticalAlign="Top">
                            <asp:Label ID="lblRightObj" runat="server" />
                            <cms:UniSelector ID="selRightObj" runat="server" SelectionMode="SingleDropDownList"
                                AllowEmpty="false" IsLiveSite="false" />
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
                <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:Panel>
</asp:Panel>
