// Initialize buttons visibility
function Get(id) {
    return document.getElementById(id);
}

function ShowButtons(portal, reusable) {
    var inherited = (Get('SelectedTemplateId').value == '0');
    var templateExists = false;

    var txtElem = txtTemplate;
    if ((txtElem != null) && (txtElem.value != "")) {
        templateExists = true;
    }

    if (editTemplatePropertiesElemStyle != null) {
        editTemplatePropertiesElemStyle.display = ((inherited || reusable && !allowEditShared) ? 'none' : 'inline');
    }
    
    var portalAndReusable = (portal && reusable);
    
    if (cloneElemStyle != null) {
        cloneElemStyle.display = (portalAndReusable ? 'inline' : 'none');
    }
    if (saveElemStyle != null) {
        saveElemStyle.display = ((!portal || !templateExists) ? 'none' : 'inline');
    }
    
    Get('isPortal').value = (portal ? 'true' : 'false');
    Get('isReusable').value = (reusable ? 'true' : 'false');
 
    return false;
}

function SelectTemplate(templateId, templateName, portal, reusable) {
    if (templateId != 0) {
        Get('SelectedTemplateId').value = templateId;
        Get('InheritedTemplateId').value = 0;

        if (templateName != null) {
            txtTemplate.value = templateName;
        }

        ShowButtons(portal, reusable);
    }
}

// btnSelect onclick()
function OnSelectPageTemplate(templateId, templateName, selectorId, portal, reusable) {
    if (templateId != 0) {
        SelectTemplate(templateId, templateName, portal, reusable);

        if (inheritElemStyle != null) {
            inheritElemStyle.display = 'inline';
        }
    }
}

function NoTemplateSelected() {
    if (cloneElemStyle != null) {
        cloneElemStyle.display = 'none';
    }
    if (layoutElemStyle != null) {
        layoutElemStyle.display = 'none';
    }
    if (inheritElemStyle != null) {
        inheritElemStyle.display = 'inline';
    }
    if (editTemplatePropertiesElemStyle != null) {
        editTemplatePropertiesElemStyle.display = 'none';
    }
}

// btnInherit onclick()
function pressedInherit(inheritedTemplateId) {
    // ShowButtons() is called in code behind
    Get('SelectedTemplateId').value = 0;
    Get('InheritedTemplateId').value = inheritedTemplateId;
    Get('TextTemplate').value = txtTemplate.value;
    
    if (inheritElemStyle != null) {
        inheritElemStyle.display = 'none';
    }
    if (editTemplatePropertiesElemStyle != null) {
        editTemplatePropertiesElemStyle.display = 'none';
    }
    return false;
}

function hideInherit() {
    if (inheritElemStyle != null) {
        inheritElemStyle.display = 'none';
    }
    if (editTemplatePropertiesElemStyle != null) {
        editTemplatePropertiesElemStyle.display = 'inline';
    }
}

// btnClone onclick()
function pressedClone(selectedTemplateId) {
    // ShowButtons() is called in code behind 
    Get('SelectedTemplateId').value = selectedTemplateId;
    Get('InheritedTemplateId').value = 0;
    Get('TextTemplate').value = txtTemplate.value;

    if (inheritElemStyle != null) {
        inheritElemStyle.display = 'inline';
    }
    
    ShowButtons(true, false);
}

// btnSave onclick()
function ReceiveNewTemplateData(DisplayName, Category, Description, pageTemplateId, lIsPortal, lIsReusable) {
    if ((DisplayName != "") && (Category != 0))  // description can be empty
    {
        Get('TemplateDisplayName').value = DisplayName;
        Get('TemplateDescription').value = Description;
        Get('TemplateCategory').value = Category;

        txtTemplate.value = DisplayName;
        Get('SelectedTemplateId').value = pageTemplateId;

        ShowButtons(true, true);
    }
}

function SetTemplateName(name) {
    txtTemplate.value = name;
}

// Remembers template name.
function RememberTemplate(templateName) {
    Get('TextTemplate').value = templateName;
}
