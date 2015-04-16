<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSFormControls_Dialogs_LinkDialogSelector" CodeFile="LinkDialogSelector.ascx.cs" %>
<cms:CMSRadioButton id="radUrlNo" runat="server" resourcestring="general.no"
    groupname="EnableURL" />
<cms:CMSRadioButton id="radUrlSimple" runat="server" resourcestring="forum.settings.simpledialog"
    groupname="EnableURL" />
<cms:CMSRadioButton id="radUrlAdvanced" runat="server" resourcestring="forum.settings.advanceddialog"
    groupname="EnableURL" />
