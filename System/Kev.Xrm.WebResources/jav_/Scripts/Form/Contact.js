var Kev = Kev || {};
Kev.Form = Kev.Form || {};
Kev.Form.Contact = (function () {
    function onLoad(executionContext) {
        var formContext = executionContext.getFormContext();

        // Check if kev_previousversionid is empty
        var previousVersionId = formContext.getAttribute("kev_previousversionid").getValue();
        if (!previousVersionId) { // if empty or null
            // Hide the kev_previousversionid field
            formContext.getControl("kev_previousversionid").setVisible(false);

            // Hide the Subgrid_new_1 subgrid
            formContext.getControl("Subgrid_new_1").setVisible(false);
        }
    }

    return {
        OnLoad: onLoad,
    };
})();
