<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSFormControls_Images_ImageDialogSelector" CodeFile="ImageDialogSelector.ascx.cs" %>
<cms:CMSRadioButton id="radImageNo" runat="server" resourcestring="general.no"
    groupname="EnableImage" />
<cms:CMSRadioButton id="radImageSimple" runat="server" resourcestring="forum.settings.simpledialog"
    groupname="EnableImage" />
<cms:CMSRadioButton id="radImageAdvanced" runat="server" resourcestring="forum.settings.advanceddialog"
    groupname="EnableImage" />
