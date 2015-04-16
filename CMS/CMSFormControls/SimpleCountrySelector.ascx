<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SimpleCountrySelector.ascx.cs" Inherits="CMSFormControls_SimpleCountrySelector" %>

<%@ Register TagPrefix="cms" TagName="CountrySelector" Src="~/CMSFormControls/CountrySelector.ascx" %>

<cms:CountrySelector ID="countrySelector" runat="server" EnableStateSelection="false" />