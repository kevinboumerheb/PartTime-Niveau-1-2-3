var Kev = Kev || {};
Kev.Form = Kev.Form || {};
Kev.Form.Teams = (function () {
    function onLoad(executionContext) {
        // Get the form context
        var formContext = executionContext.getFormContext();

        // Retrieve the Account lookup field
        var accountLookup = formContext.getAttribute("kev_account");

        // Check if the Account is set
        if (accountLookup && accountLookup.getValue()) {
            var accountId = accountLookup.getValue()[0].id.replace("{", "").replace("}", "");

            // Fetch Team records associated with this Account and retrieve 'kev_teamname' values
            var fetchXml = "<fetch mapping='logical'>" +
                "<entity name='kev_teams'>" +
                "<attribute name='kev_teamname' />" +
                "<filter type='and'>" +
                "<condition attribute='kev_account' operator='eq' value='" + accountId + "' />" +
                "</filter>" +
                "</entity>" +
                "</fetch>";

            Xrm.WebApi.retrieveMultipleRecords("kev_teams", "?fetchXml=" + encodeURIComponent(fetchXml)).then(function (result) {
                var selectedValues = [];

                // Collect all 'kev_teamname' option set values that are already used
                result.entities.forEach(function (record) {
                    selectedValues.push(record.kev_teamname);
                });

                // Filter the option set by removing already used options
                var optionSetControl = formContext.getControl("kev_teamname"); // Get control, not attribute
                if (optionSetControl) {
                    selectedValues.forEach(function (value) {
                        optionSetControl.removeOption(value); // Remove each used option from the control
                    });
                }
            }).catch(function (error) {
                console.error("Error retrieving team records: ", error.message);
            });
        }
    }


    function onTeamNameChange(executionContext) {
        var formContext = executionContext.getFormContext();

        // Get the team name option set attribute
        var teamNameAttribute = formContext.getAttribute("kev_teamname");
        var nameAttribute = formContext.getAttribute("kev_name");

        if (teamNameAttribute && nameAttribute) {
            // Get the selected value
            var selectedValue = teamNameAttribute.getValue();

            if (selectedValue !== null) {
                // Get the text (label) of the selected option
                var optionText = teamNameAttribute.getSelectedOption().text;

                // Set the text to the name field
                nameAttribute.setValue(optionText);
            } else {
                // Clear the name field if no option is selected
                nameAttribute.setValue(null);
            }
        }
    }


    return {
        OnLoad: onLoad,
        OnTeamNameChange: onTeamNameChange,
    };
})();
