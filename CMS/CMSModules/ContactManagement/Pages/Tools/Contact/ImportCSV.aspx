<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ImportCSV.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default"
    Title="Contact list" Inherits="CMSModules_ContactManagement_Pages_Tools_Contact_ImportCSV" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div data-ng-controller="CMS.OnlineMarketing.ContactImport.Controller">       
        <div data-cms-file-upload-directive
            data-on-file-loaded="model.onFileLoaded"
            data-file-stream="model.fileStream"
            data-ng-show="model.fileUploaderIsVisible">
        </div>

        <div data-cms-attribute-mapping-directive
            data-contact-groups="model.contactGroups"
            data-selected-contact-group="model.selectedContactGroup"
            data-contact-fields="model.contactFields"
            data-contact-fields-mapping="model.contactFieldsMapping"
            data-parsed-lines="model.parsedLines"
            data-file-stream="model.fileStream"
            data-ng-show="model.attributeMapperIsVisible">
        </div>

        <div data-cms-import-process-directive
            data-contact-fields-mapping="model.contactFieldsMapping"
            data-file-stream="model.fileStream"
            data-contact-group="model.selectedContactGroup"
            data-site-guid="model.siteGuid"
            data-ng-show="model.importProcessIsVisible">
        </div>
    </div>
    
    <script type="text/ng-template" id="importProcessTemplate.html">
        <div class="UIContent scroll-area om-import-csv-process-container">
            <div class="PageContent">
                <div data-cms-messages-placeholder></div>
                <h4 data-ng-show="!finished">{{"om.contact.importcsv.importingstepmessage"|resolve}}</h4>
                <h4 data-ng-show="finished">{{"om.contact.importcsv.finishedstepmessage"|resolve}}</h4>

                <p class="lead">{{"om.contact.importcsv.importing"|resolve|stringFormat:result.processed}}</p>
                    
                <ul>
                    <li>{{"om.contact.importcsv.importedcount"|resolve|stringFormat:result.imported}}</li>
                    <li>{{"om.contact.importcsv.duplicatescount"|resolve|stringFormat:result.duplicities}}</li>
                    <li>{{"om.contact.importcsv.notimported"|resolve|stringFormat:result.failures}}</li>
                </ul>
            </div>
        </div>
    </script>

    <script type="text/ng-template" id="fileUploadTemplate.html">
        <div class="UIHeader">
            <div class="header-container">
                <div class="cms-edit-menu">
                    <div class="header-actions-main">
                        <button data-ng-click="onClick()" type="button" class="btn btn-primary">{{"om.contact.importcsv.selectfilebuttontext"|resolve}}</button>
                    </div>
                </div>
            </div>
        </div>
        <input type='file' class='js-file-input hidden' accept='.csv,.txt'></input>
        <div class="UIContent scroll-area om-import-csv-content-container">
            <div class="PageContent">    
                <div data-cms-messages-placeholder></div> 
                <h4>{{"om.contact.importcsv.selectfilestepmessage.main"|resolve}}</h4>
                <div class="content-block-50">
                    <p class="lead">{{"om.contact.importcsv.selectfilestepmessage.message"|resolve}}</p>
                    <ul>
                        <li>{{"om.contact.importcsv.selectfilestepmessage.header"|resolve}}</li>
                        <li>{{"om.contact.importcsv.selectfilestepmessage.encoding"|resolve}}</li>
                        <li>{{"om.contact.importcsv.selectfilestepmessage.comma"|resolve}}</li>
                    </ul>
                </div>        
                <div class="content-block-50">
                    <p class="lead">{{"om.contact.importcsv.selectfilestepmessage.note"|resolve}}</p>
                    <ul>
                        <li>{{"om.contact.importcsv.selectfilestepmessage.duplicates"|resolve}}</li>
                        <li>{{"om.contact.importcsv.selectfilestepmessage.columns"|resolve}}</li>
                    </ul>
                </div>
            </div>
        </div>
    </script>

    <script type="text/ng-template" id="attributeMappingTemplate.html">
        <div class="UIHeader">
            <div class="header-container">
                <div class="cms-edit-menu">
                    <div class="header-actions-main">
                        <button type="button" data-ng-click="onMappingFinished()" class="btn btn-primary">{{"om.contact.importcsv.importcontactsbuttontext"|resolve}}</button>
                    </div>
                </div>
            </div>
        </div>
        <div class="UIContent scroll-area om-import-csv-content-container">
            <div class="PageContent">
                <div data-cms-messages-placeholder></div>
                <h4>{{message|resolve}}</h4>
                <div class="form-horizontal" ng-hide="contactGroups.length === 0">            
                    <div class="form-group">
                        <div class="control-group-inline">         
                            <div class="editing-form-label-cell">
                                <label for="cgSelect" class="control-label">{{"om.contact.importcsv.selectcg"|resolve}}:</label>
                            </div>            
                            <div class="editing-form-value-cell">
                                <select id="cgSelect" class="DropDownField form-control" 
                                    data-ng-model="selectedContactGroup" 
                                    data-ng-options="cg.contactGroupGUID as cg.contactGroupDisplayName for cg in contactGroups" >
                                    <option value="">{{"om.contact.importcsv.nocg"|resolve}}</option>
                                </select>
                            </div>
                        </div>
                    </div>  
                </div> 
            </div>    
            <div class="om-import-csv-mapping-tables-container">
                <div data-ng-repeat="csvColumn in csvColumnNamesWithExamples" class="om-import-csv-mapping-table col-xs-12 col-md-6 col-lg-4">
                    <div data-contact-fields-mapping="processingContactFieldsMapping" data-csv-column="csvColumn" data-cms-attribute-mapping-control-directive></div>
                </div>
            </div>
        </div>
    </script>

    <script type="text/ng-template" id="attributeMappingControlTemplate.html">
        <table class="table table-hover _nodivs">
            <thead>
                <tr class="unigrid-head">
                    <th scope="col">{{csvColumn.ColumnName.trim() || "&nbsp;"}}</th>
                </tr>
            </thead>
            <tbody>
                <tr data-ng-repeat="example in csvColumn.ColumnExamples track by $index">
                    <td>{{example.trim() || "&nbsp;"}}</td>
                </tr>
            </tbody>
        </table>
        
        <div class="form-horizontal">            
            <div class="form-group">
                <div class="control-group-inline">         
                    <div class="editing-form-label-cell">
                        <label for="id_{{csvColumn.ColumnName}}" class="control-label">{{"om.contact.importcsv.belongsto"|resolve}}:</label>
                    </div>            
                    <div class="editing-form-value-cell">
                        <select id="id_{{csvColumn.ColumnName}}" data-ng-model="selectedField" data-ng-change="contactFieldUpdate()" class="DropDownField form-control" >
                            <option value="-1">{{"om.contact.importcsv.donotimport"|resolve}}</option>
                            <optgroup data-ng-repeat="category in contactFieldsMapping" label="{{category.categoryName}}" data-ng-if="category.categoryMembers">
                                <option data-ng-repeat="field in category.categoryMembers" value="{{field.name}}" data-ng-disabled="field.mappedIndex !== -1 && field.mappedIndex !== csvColumn.ColumnIndex">
                                    {{field.displayName}}
                                </option>
                            </optgroup>
                        </select>
                    </div>
                </div>
            </div>  
        </div>           
    </script>
</asp:Content>
