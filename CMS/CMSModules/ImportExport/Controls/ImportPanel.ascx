<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ImportExport_Controls_ImportPanel" CodeFile="ImportPanel.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" tagname="PageTitle" tagprefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/Trees/ObjectTree.ascx" TagName="ObjectTree" TagPrefix="cms" %>

<%@ Register Src="ImportGridView.ascx" TagName="ImportGridView" TagPrefix="cms" %>
<%@ Register Src="ImportGridTasks.ascx" TagName="ImportGridTasks" TagPrefix="cms" %>

<script type="text/javascript">
    //<![CDATA[
    function imSetDivPosition() {
        var intY = document.getElementById("imTreeDiv").scrollTop;
        document.getElementById("hdnDivScrollBar").value = intY;
    }

    function imGetDivPosition() {
        var intY = document.getElementById('hdnDivScrollBar').value;
        document.getElementById("imTreeDiv").scrollTop = intY;
    }

    // Hook onload handler
    if (window.onload != null) {
        var oldOnload = window.onload;
        window.onload = function(e) { oldOnload(e); imGetDivPosition(); };
    }
    else {
        window.onload = function(e) { imGetDivPosition(); };
    }
    //]]>
</script>

<asp:Panel ID="pnlGrid" runat="Server">
    <table width="100%" cellpadding="0" cellspacing="0">
        <tr>
            <td style="width: 250px; vertical-align: top;">
                <div id="imTreeDiv" class="ObjectTreeWrap" onclick="imSetDivPosition();" onunload="imSetDivPosition();">
                    <cms:ObjectTree ID="objectTree" ShortID="t" runat="server" UsePostback="true" UseImages="true"
                        EnableViewState="true" />
                </div>
                <input type="hidden" id="hdnDivScrollBar" name="hdnDivScrollBar" value="<%=mScrollPosition%>" />
            </td>
            <td style="vertical-align: top;">
                <div class="ObjectWrap">
                    <div>
                        <asp:Panel runat="server" ID="pnlError" CssClass="WizardHeaderLine" Visible="false"
                            BackColor="transparent">
                            <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" EnableViewState="false" />
                        </asp:Panel>
                        <asp:PlaceHolder ID="plcPanel" runat="server">
                            <asp:PlaceHolder ID="plcControl" runat="Server" />
                            <cms:ImportGridView ID="gvObjects" ShortID="go" runat="server" />
                            <cms:ImportGridTasks ID="gvTasks" ShortID="gt" runat="server" />
                        </asp:PlaceHolder>
                    </div>
                </div>
            </td>
        </tr>
    </table>
</asp:Panel>
