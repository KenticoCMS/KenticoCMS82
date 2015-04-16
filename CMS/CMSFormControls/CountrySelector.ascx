<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSFormControls_CountrySelector" CodeFile="CountrySelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector" TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" ViewStateMode="Enabled">
    <ContentTemplate>
        <div class="country-selector">
            <cms:UniSelector ID="uniSelectorCountry" runat="server" DisplayNameFormat="{%CountryDisplayName%}"
                ObjectType="cms.country" ResourcePrefix="countryselector" AllowAll="false" AllowEmpty="false" />
            <asp:PlaceHolder runat="server" ID="plcStates">
                <cms:UniSelector ID="uniSelectorState" runat="server" DisplayNameFormat="{%StateDisplayName%}"
                    ObjectType="cms.state" ResourcePrefix="stateselector" />
            </asp:PlaceHolder>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
