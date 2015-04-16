<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Mapping.ascx.cs" Inherits="CMSModules_SocialMedia_Controls_Facebook_Mapping" %>
<%@ Register TagPrefix="cms" TagName="FacebookError" Src="~/CMSModules/SocialMedia/Controls/Facebook/Error.ascx" %>
<%@ Import Namespace="CMS.SocialMedia.Facebook" %>
<cms:FacebookError ID="FacebookError" runat="server" EnableViewState="false" />
<div id="ContainerControl" runat="server" visible="false">
    <table visible="false" style="margin-bottom:1em;border-collapse:collapse;border-spacing:0">
        <tbody>
            <tr>
                <td style="font-weight:bold;color:inherit"><%= HTMLHelper.HTMLEncode(GetString("fb.mapping.fieldheader"))%></td>
                <td style="font-weight:bold;padding-left:2em;color:inherit"><%= HTMLHelper.HTMLEncode(GetString("fb.mapping.attributeheader"))%></td>
            </tr>
            <asp:Repeater ID="UserMappingItemRepeater" runat="server">
                <ItemTemplate>
                    <tr>
                        <td style="color:inherit">
                            <%# HTMLHelper.HTMLEncode(GetUserFieldDisplayName(((EntityMappingItem)Container.DataItem).FieldName)) %>
                        </td>
                        <td style="padding-left:2em;color:inherit"><%# HTMLHelper.HTMLEncode(GetFacebookUserAttributeDisplayName(((EntityMappingItem)Container.DataItem).AttributeName)) %></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Repeater ID="UserSettingsMappingItemRepeater" runat="server">
                <ItemTemplate>
                    <tr>
                        <td style="color:inherit">
                            <%# HTMLHelper.HTMLEncode(GetUserSettingsFieldDisplayName(((EntityMappingItem)Container.DataItem).FieldName)) %>
                            <span style="color:gray;font-size:smaller">(<%= HTMLHelper.HTMLEncode(ResHelper.GetString("objecttype.cms_usersettings")) %>)</span>
                        </td>
                        <td style="padding-left:2em;color:inherit"><%# HTMLHelper.HTMLEncode(GetFacebookUserAttributeDisplayName(((EntityMappingItem)Container.DataItem).AttributeName)) %></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </tbody>
    </table>
</div>
<p id="MessageControl" runat="server" enableviewstate="false" visible="false"></p>
