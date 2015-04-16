<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BizForm_Edit_OnlineMarketing.aspx.cs" Inherits="CMSModules_BizForms_Tools_BizForm_Edit_OnlineMarketing"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Form properties - On-line marketing" Theme="Default" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="form-horizontal">
        <div class="form-group">
            <cms:CMSCheckBox ID="chkLogActivity" runat="server" ResourceString="bizformgeneral.lbllogactivity" CssClass="ContentCheckbox" AutoPostBack="true"
                OnCheckedChanged="chkLogActivity_CheckedChanged" />
        </div>
    </div>
    <asp:PlaceHolder ID="plcMapping" runat="server" />
</asp:Content>