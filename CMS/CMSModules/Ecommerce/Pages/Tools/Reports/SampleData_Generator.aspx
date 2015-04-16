<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SampleData_Generator.aspx.cs"
    Inherits="CMSModules_Ecommerce_Pages_Tools_Reports_SampleData_Generator" Theme="Default"
    EnableEventValidation="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
        <ContentTemplate>
            <cms:LocalizedHeading runat="server" ID="headGenerateData" Level="4" ResourceString="com.generator.generategroup" EnableViewState="false" />
            <cms:LocalizedLabel ResourceString="com.generator.generatepericope" runat="server"
                ID="lblGeneratePericope" />
            <br />
            <br />
            <cms:LocalizedButton runat="server" ID="btnGenerate" OnClick="btnGenerateClick" ButtonStyle="Primary"
                ResourceString="com.generator.generatebutton" />
            <br/>
            <br/>
            <cms:LocalizedHeading runat="server" ID="headDelete" Level="4" ResourceString="com.generator.deletegroup" EnableViewState="false" />
            <cms:LocalizedLabel ResourceString="com.generator.deletepericope" runat="server"
                ID="lblDeletePericope" />
            <br />
            <br />
            <cms:LocalizedButton runat="server" ID="btnDelete" OnClick="btnDeleteClick" ButtonStyle="Primary"
                ResourceString="com.generator.deletebutton" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
