<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_System_Debug_Log"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="System - SQL"
    CodeFile="Log.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="FloatLeft">
        <cms:CMSCheckBox runat="server" ID="chkCompleteContext" ResourceString="Debug.ShowCompleteContext"
            AutoPostBack="true" />
    </div>
    <div class="FloatRight">
        <cms:LocalizedButton runat="server" ResourceString="Administration-System.btnClearCache" ID="btnClearCache" OnClick="btnClearCache_Click" ButtonStyle="Default"
            EnableViewState="false" />
        <cms:LocalizedButton runat="server" ResourceString="Debug.ClearLog" ID="btnClear" OnClick="btnClear_Click" ButtonStyle="Default"
            EnableViewState="false" />
    </div>
    <br />
    <br />
    <asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" EnableViewState="false" />
    <div class="ClearBoth">
    </div>
    <asp:PlaceHolder runat="server" ID="plcLogs" EnableViewState="false" />
</asp:Content>
