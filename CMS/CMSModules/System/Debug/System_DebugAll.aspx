<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_System_Debug_System_DebugAll"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="System - All"
    CodeFile="System_DebugAll.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="FloatLeft">
        <cms:CMSCheckBox runat="server" ID="chkCompleteContext" ResourceString="Debug.ShowCompleteContext"
            AutoPostBack="true" />
    </div>
    <div class="FloatRight">
        <cms:CMSButton runat="server" ID="btnClear" OnClick="btnClear_Click" ButtonStyle="Default"
            EnableViewState="false" />
    </div>
    <br />
    <br />
    <cms:CMSPlaceHolder runat="server" ID="plcLogs" EnableViewState="false" />
</asp:Content>
