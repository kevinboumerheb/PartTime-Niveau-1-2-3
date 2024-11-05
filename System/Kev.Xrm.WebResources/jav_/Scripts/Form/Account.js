var Kev = Kev || {};
Kev.Form = Kev.Form || {};
Kev.Form.Account = (function () {
    function onLoad(executionContext) {
        var formContext = executionContext.getFormContext();

        // 101: Automatically fill the account name with the user's name if the form is in creation mode
        if (formContext.ui.getFormType() === 1) {
            // FormType 1 = Create form
            var userSettings = Xrm.Utility.getGlobalContext().userSettings;

            // Try to get the full user name directly
            var userName = userSettings.userName; // userName returns the full name (First + Last)

            if (!formContext.getAttribute("name").getValue()) {
                formContext.getAttribute("name").setValue(userName);
            }
        }

        // 101: Retrieve active contacts of the account and display an alert with the count
        var accountId = formContext.data.entity.getId(); // Get current account ID
        if (accountId) {
            getActiveContacts(accountId)
                .then(function (contactCount) {
                    if (contactCount > 0) {
                        Xrm.Navigation.openAlertDialog({
                            text: "Nombre de contacts actifs : " + contactCount,
                        });
                    }
                })
                .catch(function (error) {
                    console.log(error.message);
                });
        }

        // 203 js
        var escalate = formContext.getAttribute("kev_escalate").getValue();
        // Get the "Teams To Escalate" multiselect field
        var teamsField = formContext.getControl("kev_teamstoescalate");
        var teamsAttribute = formContext.getAttribute("kev_teamstoescalate");

        if (escalate === true) {
            // If escalate is "Yes", make the multiselect field visible and required
            teamsField.setVisible(true);
            teamsAttribute.setRequiredLevel("required");
        } else {
            // If escalate is "No", hide the multiselect field, remove requirement, and reset its value
            teamsField.setVisible(false);
            teamsAttribute.setRequiredLevel("none");
            teamsAttribute.setValue(null); // Reset the value to null
        }

        onTeamsToEscalateChange(executionContext);
        //202 js
        exampleUsage();
    }

    // 102: Function to handle the "Contact Principal" field change
    function onContactPrincipalChange(executionContext) {
        var formContext = executionContext.getFormContext();
        var contactPrincipalField = formContext.getAttribute("primarycontactid");

        // Check if the contactPrincipalField exists
        if (contactPrincipalField) {
            var contactLookup = contactPrincipalField.getValue(); // Get the lookup value

            // Check if a contact is selected
            if (contactLookup != null && contactLookup.length > 0) {
                var contactId = contactLookup[0].id;

                // Retrieve the email and mobile fields from the contact
                Xrm.WebApi.retrieveRecord(
                    "contact",
                    contactId,
                    "?$select=emailaddress1,address1_telephone1"
                ).then(
                    function success(contact) {
                        if (contact) {
                            // Show the email and mobile fields in a popup dialog
                            var message =
                                "Email: " +
                                (contact.emailaddress1 || "Not available") +
                                "\nMobile: " +
                                (contact.address1_telephone1 || "Not available");
                            formContext
                                .getAttribute("emailaddress1")
                                .setValue(contact.emailaddress1);
                            Xrm.Navigation.openAlertDialog({ text: message });
                        }
                    },
                    function error(err) {
                        console.log(err.message);
                    }
                );
            } else {
                console.log("No contact selected.");
            }
        } else {
            console.log("Contact Principal field does not exist on the form.");
        }
    }

    // 203: Part 1.c
    function onEscalateChange(executionContext) {
        var formContext = executionContext.getFormContext();
        // Get the value of the Escalate? field
        var escalate = formContext.getAttribute("kev_escalate").getValue();
        // Get the "Teams To Escalate" multiselect field
        var teamsField = formContext.getControl("kev_teamstoescalate");
        var teamsAttribute = formContext.getAttribute("kev_teamstoescalate");

        if (escalate === true) {
            // If escalate is "Yes", make the multiselect field visible and required
            teamsField.setVisible(true);
            teamsAttribute.setRequiredLevel("required");
            teamsAttribute.setValue(null); // Reset the value to null
        } else {
            // If escalate is "No", hide the multiselect field, remove requirement, and reset its value
            teamsField.setVisible(false);
            teamsAttribute.setRequiredLevel("none");
            teamsAttribute.setValue(null); // Reset the value to null
            onTeamsToEscalateChange(executionContext);
        }
    }

    function onTeamsToEscalateChange(executionContext) {
        var formContext = executionContext.getFormContext();

        // Get the values selected in the Teams To Escalate multiselect option set
        var teamsSelected = formContext.getAttribute("kev_teamstoescalate").getValue();

        // Define the team sections
        var sections = {
            "1": "Dev_Details",     // assuming "1" corresponds to "Dev"
            "2": "Sales_Details",   // assuming "2" corresponds to "Sales"
            "3": "Admin_Details",   // assuming "3" corresponds to "Admin"
            "4": "Support_Details"  // assuming "4" corresponds to "Support"
        };

        // Hide all team sections initially
        for (var team in sections) {
            formContext.ui.tabs.get("SUMMARY_TAB").sections.get(sections[team]).setVisible(false);
        }

        // Show selected team sections
        if (teamsSelected) {
            teamsSelected.forEach(function (teamValue) {
                if (sections[teamValue]) {
                    formContext.ui.tabs.get("SUMMARY_TAB").sections.get(sections[teamValue]).setVisible(true);
                }
            });
        }
    }


    //203: Part 2
    function onTypeChange(executionContext) {
        var formContext = executionContext.getFormContext();

        // Get the value of the 'Type' field (Boolean)
        var typeField = formContext.getAttribute("kev_type"); // Change 'type_field' to your actual field name
        var typeValue = typeField.getValue();

        // Get the 'Product' field (Option Set)
        var productField = formContext.getControl("kev_product"); // Change 'product_field' to your actual field name

        if (typeValue === true) {
            // If Type is Client (Boolean is true)
            productField.clearOptions(); // Clear existing options
            productField.addOption({ text: "Client Product 1", value: 1 });
            productField.addOption({ text: "Client Product 2", value: 2 });
        } else if (typeValue === false) {
            // If Type is Partner (Boolean is false)
            productField.clearOptions(); // Clear existing options
            productField.addOption({ text: "Partner Product 1", value: 3 });
            productField.addOption({ text: "Partner Product 2", value: 4 });
            productField.addOption({ text: "Partner Product 3", value: 5 });
        }
    }

    // 101: Retrieve active contacts of the account
    function getActiveContacts(accountId) {
        return Xrm.WebApi.retrieveMultipleRecords(
            "contact",
            "?$filter=_parentcustomerid_value eq " + accountId + " and statecode eq 0"
        ).then(function (result) {
            return result.entities.length; // Return the count of active contacts
        });
    }

    let nextLink = null;
    // 202 Part A: Function to retrieve records with paging
    function retrieveRecordsWithPaging(entitySetName, pageSize, pageNumber = 1) {
        let url;
        if (pageNumber === 1 || !nextLink) {
            url = `${Xrm.Utility.getGlobalContext().getClientUrl()}/api/data/v9.0/${entitySetName}?$top=${pageSize}&$count=true`;
        } else {
            url = nextLink;
        }

        return Xrm.WebApi.retrieveMultipleRecords(
            entitySetName,
            `?$top=${pageSize}&$count=true`
        )
            .then(function (response) {
                console.log("API Response:", response);
                nextLink = response["@odata.nextLink"];
                return {
                    records: response.entities || [],
                    totalCount: response["@odata.count"] || 0,
                    hasNextPage: !!nextLink,
                };
            })
            .catch(function (error) {
                console.error("Error in API call:", error.message);
                return {
                    records: [],
                    totalCount: 0,
                    hasNextPage: false,
                };
            });
    }

    // 202 Part A: Function to retrieve records with paging
    function exampleUsage() {
        const entitySetName = "account"; // Make sure this matches your environment
        const pageSize = 100; // Number of records per page
        let currentPage = 1;

        function getNextPage() {
            retrieveRecordsWithPaging(entitySetName, pageSize, currentPage)
                .then(function (result) {
                    console.log(
                        `Retrieved ${result.records.length} records from page ${currentPage}`
                    );
                    console.log(`Total records: ${result.records.length}`);
                    console.log("Records:", result.records);

                    if (result.hasNextPage) {
                        currentPage++;
                        console.log("Fetching next page...");
                        getNextPage();
                    } else {
                        console.log("No more pages to fetch.");
                    }
                })
                .catch(function (error) {
                    console.error("Error retrieving records:", error);
                });
        }

        getNextPage();
    }

    return {
        OnLoad: onLoad,
        OnContactPrincipalChange: onContactPrincipalChange,
        OnEscalateChange: onEscalateChange,
        OnTeamsToEscalateChange: onTeamsToEscalateChange,
        OnTypeChange: onTypeChange,
    };
})();
