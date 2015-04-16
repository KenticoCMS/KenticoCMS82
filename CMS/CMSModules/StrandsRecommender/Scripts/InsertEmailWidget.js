var STRANDS = (function (my, $) {
    "use strict";

    var root = null;
    my.selectedItem = null;

    /**
     * Inserts selected item into CKeditor text area. 
     */
    my.insertItem = function() {
        if ((window.wopener != null) && (window.wopener.CMSPlugin.insertHtml)) {
            window.wopener.CMSPlugin.insertHtml(my.selectedItem.find(".table").html());
        }
        return CloseDialog();
    };


    /**
     * Asynchronously loads templates and handles visual effects.
     */
    my.initInsertEmailWidget = function (templateNames, loadingImagePath) {
        adjustListPosition();
        $(window).resize(function () {
            adjustListPosition();
        });

        $("#btnInsert").click(function(e) {
            e.preventDefault();
            my.insertItem();
        });
        
        if (templateNames === null || templateNames.length === 0) {
            return;
        }
        
        // Root of unordered list
        root = $("#EmailTemplatesList");

        $.each(templateNames, function (index, templateName) { // Iterate through collection of templates
            var templateID = "EmailTemplate_" + templateName;

            // Show loading icon
            root.append("<li id=\"" + templateID + "\" class=\"FloatLeft\"><div><div class=\"shadow\"><div class=\"inner\"><span class=\"label\">" + templateName + "</span><div class=\"table\"><img alt=\"Loading&hellip;\" src=\"" + loadingImagePath + "\"/></div></div></div></div></li>");
            my.webMethodCall("LoadSpecificEmailTemplate",
                { name: templateName /* Loading template HTML body for current template name */ })
                .done(function(result) { // Success
                    $("#" + templateID)
                        .find(".table")
                            .html(result.d)
                        .end() // Set current template HTML body as most inner div content
                        .click(function() { // Selecting recommendation on mouse click
                            selectItem($(this));
                        })
                        .dblclick(function() { // Insert item on double click
                            selectItem($(this));
                            my.insertItem();
                        })
                        .find("a") // Prevent from clicking on Strands links
                        .click(function(e) {
                            e.preventDefault();
                        });

                    if (my.selectedItem.attr("id") === templateID) // If loading item is selected and successfully loaded, enable insert button
                    {
                        enableInsertButton();
                    }
                });
        });
        
        var firstTemplate = $("#EmailTemplate_" + templateNames[0]);
        selectItem(firstTemplate);
    };


    /**
     * Recomputes position of list with recommendations. Absolute position has to be used in design, therefore this cannot be achieved with CSS.
     */
    function adjustListPosition() {
        var headerHeight = $("*[id*='pnlContainerHeader']").outerHeight(true);

        $(".body").css("top", headerHeight);
    }


    /**
     * Disables insert button
     */
    function disableInsertButton() {
        $("#btnInsert").attr("disabled", "disabled").addClass("btn-disabled");
    }


    /**
     * Enables insert button
     */
    function enableInsertButton() {
        $("#btnInsert").removeAttr("disabled").removeClass("btn-disabled");
    }


    /**
     * Selects template by given item, unselects the rest.
     * @param {string} item Identifier of item to be selected.
     */
    function selectItem(item) {
        root.find("li").removeClass("selected"); // Visually "unselect" all
        item.addClass("selected"); // And select given item
        my.selectedItem = item; // Store item as selected
        item.has("table").length ? enableInsertButton() : disableInsertButton();
    }
    
    return my;
}(STRANDS || {}, $cmsj));