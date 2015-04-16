<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_Preview" CodeFile="Newsletter_Issue_Preview.aspx.cs"
     Title="Newsletter issue - Preview" Theme="Default" %>

<%@ Register Src="~/CMSModules/Newsletters/Controls/Newsletter_Preview.ascx" TagName="Preview" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:Preview ID="preview" runat="server" ShortID="p" />
</asp:Content>