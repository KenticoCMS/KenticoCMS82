<%@ Page Language="C#" Debug="true" CodeFile="Generator.aspx.cs" Inherits="CMSModules_Personas_Pages_SampleDataGenerator_Generator" 
         MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" Title="Online Marketing sample data generator" %>


<asp:Content ID="cntContent" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlMessages"></asp:Panel>

    <div class="form-horizontal">
        <h4>Sample data generator</h4> 
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label runat="server" AssociatedControlID="txtContactsCount" CssClass="control-label">
                    Create contact statuses if there are none:
                </asp:Label>
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox runat="server" ID="chckCreateContactStatuses" Checked="true"/>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label runat="server" AssociatedControlID="chckGenerateContacts" CssClass="control-label">
                    Generate contacts:
                </asp:Label>
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox runat="server" ID="chckGenerateContacts" Checked="true"/>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label runat="server" AssociatedControlID="txtContactsCount" CssClass="control-label">
                    Number of contacts to generate:
                </asp:Label>
            </div>
            <div class="editing-form-value-cell">
                <div class="control-group-inline">
                    <cms:CMSTextBox runat="server" ID="txtContactsCount" Text="10" />
                    <cms:CMSCheckBox runat="server" Text="Contacts with real names" ID="chckContactRealNames" Checked="False"/>
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label runat="server" AssociatedControlID="chckGenerateMergedContacts" CssClass="control-label">
                    Generate merged contacts for all contacts:
                </asp:Label>
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox runat="server" ID="chckGenerateMergedContacts" Checked="true"/>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label runat="server" AssociatedControlID="chckGenerateRelationships" CssClass="control-label">
                    Generate contact relationships for all contacts:
                </asp:Label>
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox runat="server" ID="chckGenerateRelationships" Checked="true"/>
                <span>
                    (generates one relationship between all contacts and a random user. Warning: not optimized for performance)
                </span>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label runat="server" AssociatedControlID="txtGeneratePersonas" CssClass="control-label">
                    Generate personas:
                </asp:Label>
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox runat="server" ID="txtGeneratePersonas" Checked="true"/>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label runat="server" AssociatedControlID="chckGenerateCGs" CssClass="control-label">
                    Generate contact groups:
                </asp:Label>
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox runat="server" ID="chckGenerateCGs" Checked="true"/>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label runat="server"  AssociatedControlID="txtCGsCount" CssClass="control-label">
                    Number of contact groups to generate:
                </asp:Label>
            </div>
            <div class="editing-form-value-cell">
                <div class="control-group-inline">
                    <cms:CMSTextBox runat="server" ID="txtCGsCount" Text="5" />
                    <asp:Label runat="server">All generated contact groups are dynamic</asp:Label>
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label runat="server" AssociatedControlID="chckGenerateScores" CssClass="control-label">
                    Generate scores:
                </asp:Label>
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox runat="server" ID="chckGenerateScores" Checked="true"/>
            </div>
        </div>
         <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label runat="server"  AssociatedControlID="txtScoresCount" CssClass="control-label">
                    Number of scores to generate:
                </asp:Label>
            </div>
            <div class="editing-form-value-cell">
                <div class="control-group-inline">
                    <cms:CMSTextBox runat="server" ID="txtScoresCount" Text="10" />
                    <asp:Label runat="server">Every score will have 15–25 rules</asp:Label>
                </div>
            </div>
        </div>
                <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label runat="server" AssociatedControlID="chckGenerateActivities" CssClass="control-label">
                    Generate activities for ALL existing contacts on the current site:
                </asp:Label>
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox runat="server" ID="chckGenerateActivities" Checked="False"/>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label runat="server" AssociatedControlID="txtActivitiesCount" CssClass="control-label">
                    Number of activities to generate for each existing contact:
                </asp:Label>
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox runat="server" ID="txtActivitiesCount" Text="30"></cms:CMSTextBox>
                <span>(this will be the medium number of activities for each contact)</span>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell-offset editing-form-value-cell">
                <cms:CMSButton runat="server" ID="btnGenerate" Text="Generate data" />
            </div>
        </div>

    </div>
    
    <div class="form-horizontal">
        <h4>Various helpers section</h4> 
            <div class="form-group">
            <div class="editing-form-value-cell-offset editing-form-value-cell">
                <cms:CMSButton runat="server" ID="btnRecalculateCGs" Text="Recalculate Contact Groups" Tooltip="Recalculates all contact groups that needs to be recalculated" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell-offset editing-form-value-cell">
                <cms:CMSButton runat="server" ID="btnRecalculateScoreRules" Text="Recalculate score rules" Tooltip="Recalculates all scoring rules that needs to be recalculated" />
            </div>
        </div>
    </div>
    
    <div class="form-horizontal">
        <h4>Validation section</h4>
        <div class="form-group">
            <div class="editing-form-value-cell-offset editing-form-value-cell">
                <div class="control-group-inline">
                    <cms:CMSButton runat="server" ID="btnGenerateAll" Text="Generate testing data" />
                    <asp:Label runat="server">Generates testing data for the load test (contact groups, scorings, personas).<br />Modifies various settings when development mode is off (disables email sending, enables global contacts, enables merging by email, ...). Also global contact groups are created only when the development mode is off.</asp:Label>
                </div>
            </div>
        </div>
        <div class="form-group">  
            <div class="editing-form-value-cell-offset editing-form-value-cell">
                <div class="control-group-inline">
                    <cms:CMSButton runat="server" ID="btnValidate" Text="Show validation data" />
                    <asp:Label runat="server">Article describing how to interpret results can be found in internal documentation <a href="https://kentico.atlassian.net/wiki/x/5oEEB">https://kentico.atlassian.net/wiki/x/5oEEB</a>.</asp:Label>
                </div>
            </div>
        </div>
    
        <div class="form-group">
            <asp:Table runat="server" ID="cgResultsTable" GridLines="Both" Caption="List of CGs and it's members count" BorderWidth="1px" Visible="False">
                <asp:TableHeaderRow id="cgResultsTableHeader" BackColor="LightBlue" runat="server">
                    <asp:TableHeaderCell Scope="Column" Text="Contact Group" />
                    <asp:TableHeaderCell Scope="Column" Text="# of contacts" />
                    <asp:TableHeaderCell Scope="Column" Text="% of all contacts" />
                    <asp:TableHeaderCell Scope="Column" Text="CG is global" />
                    <asp:TableHeaderCell Scope="Column" Text="Merged contacts check" />
                </asp:TableHeaderRow>  
            </asp:Table>
        </div>
        
        <div class="form-group">
            <asp:Table runat="server" ID="personasResultsTable" GridLines="Both" Caption="List of Personas and it's members count" BorderWidth="1px" Visible="False">
                <asp:TableHeaderRow id="TableHeaderRow1" BackColor="LightBlue" runat="server">
                    <asp:TableHeaderCell Scope="Column" Text="Persona" />
                    <asp:TableHeaderCell Scope="Column" Text="# of contacts" />
                    <asp:TableHeaderCell Scope="Column" Text="% of all contacts" />
                    <asp:TableHeaderCell Scope="Column" Text="Merged contacts check" />
                </asp:TableHeaderRow>  
            </asp:Table>
        </div>
        
        <div class="form-group">
            <asp:Table runat="server" ID="scringResultsTable" GridLines="Both" Caption="List of score rules and it's members count" BorderWidth="1px" Visible="False">
                <asp:TableHeaderRow id="TableHeaderRow2" BackColor="LightBlue" runat="server">
                    <asp:TableHeaderCell Scope="Column" Text="Score rule" />
                    <asp:TableHeaderCell Scope="Column" Text="# of contacts" />
                    <asp:TableHeaderCell Scope="Column" Text="points groups" />
                    <asp:TableHeaderCell Scope="Column" Text="% of all contacts" />
                </asp:TableHeaderRow>  
            </asp:Table>
        </div>
    </div>
</asp:Content>