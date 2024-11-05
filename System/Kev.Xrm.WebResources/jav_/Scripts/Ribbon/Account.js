var Kev = Kev || {};
Kev.Ribbon = Kev.Form || {};
Kev.Ribbon.Account = (function () {
    // 202 Part B-a: Function to handle the "Request Approval" button click
    function onRequestApproval() {
        // Prompt to get the task description from the user
        var taskDescription = prompt("Please enter the description for the task:");

        // Check if the user canceled the prompt or left it empty
        if (taskDescription === null || taskDescription.trim() === "") {
            Xrm.Navigation.openAlertDialog({ text: "Task description is required." });
            return;
        }

        // Get the current record ID (e.g., account, contact, etc.)
        var recordId = Xrm.Page.data.entity.getId().replace(/[{}]/g, "");

        // Proceed with requesting approval, passing the task description and record ID
        requestApproval(taskDescription, recordId);
    }

    // 202 Part B-a: Function to request approval from the user's manager
    function requestApproval(taskDescription, recordId) {
        // Get the current user's ID directly from the global context
        var userId = Xrm.Utility.getGlobalContext().userSettings.userId.replace(
            /[{}]/g,
            ""
        );

        // Fetch the current user's manager
        var userRequest = {
            method: "GET",
            url:
                Xrm.Utility.getGlobalContext().getClientUrl() +
                "/api/data/v9.1/systemusers(" +
                userId +
                ")?$select=fullname,_parentsystemuserid_value",
            headers: {
                "OData-MaxVersion": "4.0",
                "OData-Version": "4.0",
                Accept: "application/json",
                "Content-Type": "application/json; charset=utf-8",
            },
        };

        // Call to retrieve user data
        fetch(userRequest.url, {
            method: userRequest.method,
            headers: userRequest.headers,
        })
            .then((response) => response.json())
            .then((data) => {
                var managerId = data._parentsystemuserid_value; // Manager's system user ID
                var userName = data.fullname; // Current user's full name

                if (!managerId) {
                    Xrm.Navigation.openAlertDialog({
                        text: "No manager found for the user!",
                    });
                    return;
                }

                // Fetch manager's name and create a task for them
                getManagerAndCreateTask(managerId, userName, taskDescription, recordId);
            })
            .catch((error) => {
                console.error("Error fetching user or manager: " + error.message);
            });
    }

    // 202 Part B-a: Function to get the manager's details and create a task
    function getManagerAndCreateTask(
        managerId,
        userName,
        taskDescription,
        recordId
    ) {
        // Fetch the manager's details (name)
        var managerRequest = {
            method: "GET",
            url:
                Xrm.Utility.getGlobalContext().getClientUrl() +
                "/api/data/v9.1/systemusers(" +
                managerId +
                ")?$select=fullname",
            headers: {
                "OData-MaxVersion": "4.0",
                "OData-Version": "4.0",
                Accept: "application/json",
                "Content-Type": "application/json; charset=utf-8",
            },
        };

        fetch(managerRequest.url, {
            method: managerRequest.method,
            headers: managerRequest.headers,
        })
            .then((response) => response.json())
            .then((data) => {
                var managerName = data.fullname;

                // Create a task for the manager
                createTaskForManager(
                    managerId,
                    managerName,
                    userName,
                    taskDescription,
                    recordId
                );
            })
            .catch((error) => {
                console.error("Error fetching manager details: " + error.message);
            });
    }

    // 202 Part B-a: Function to create a task for the manager
    function createTaskForManager(
        managerId,
        managerName,
        userName,
        taskDescription,
        recordId
    ) {
        // Data for the task creation
        var taskData = {
            subject: "Approval Request for " + userName,
            description:
                "A new approval task for " + userName + ".\n" + taskDescription + ".",
            scheduledend: new Date().toISOString(), // Due date
            "ownerid@odata.bind": "/systemusers(" + managerId + ")", // Bind the task to the manager
            "regardingobjectid_account@odata.bind": "/accounts(" + recordId + ")", // Use the record ID for "regarding"
        };

        // API request to create the task
        fetch(
            Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/tasks",
            {
                method: "POST",
                headers: {
                    "OData-MaxVersion": "4.0",
                    "OData-Version": "4.0",
                    Accept: "application/json",
                    "Content-Type": "application/json; charset=utf-8",
                },
                body: JSON.stringify(taskData),
            }
        )
            .then((response) => {
                if (response.ok) {
                    // Task created successfully (HTTP status 201 or 204)
                    Xrm.Navigation.openAlertDialog({
                        text: "Task created for manager: " + managerName,
                    });
                } else {
                    // Handle error response
                    return response.json().then((error) => {
                        console.error("Error creating task:", error);
                    });
                }
            })
            .catch((error) => {
                console.error("Error creating task: " + error.message);
            });
    }

    function sendEmailToManager() {
        var recordId = Xrm.Page.data.entity.getId(); // Get the current record's ID
        runWorkflow(recordId);
    }

    function runWorkflow(recordId) {
        var workflowId = "4778a1f1-068e-ef11-ac20-6045bd0401a8"; // Workflow ID without curly braces
        var req = new XMLHttpRequest();

        // Construct the API URL with correct syntax
        var url =
            Xrm.Utility.getGlobalContext().getClientUrl() +
            "/api/data/v9.0/workflows(" +
            workflowId +
            ")/Microsoft.Dynamics.CRM.ExecuteWorkflow";

        req.open("POST", url, true);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("Prefer", "odata.include-annotations=*");

        // Prepare the data payload
        var data = {
            EntityId: recordId, // Ensure this is the ID of the record you want to run the workflow on
        };

        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    Xrm.Navigation.openAlertDialog({
                        text: "Send Email To Manager Workflow Executed",
                    });
                } else {
                    var error = JSON.parse(this.response).error.message;
                    console.error("Error executing workflow: " + error);
                }
            }
        };

        // Send the request with the data
        req.send(JSON.stringify(data));
    }

    function onClickPagaging() {
        exampleUsage();
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
                alert("API Response: " + JSON.stringify(response));
                nextLink = response["@odata.nextLink"];
                return {
                    records: response.entities || [],
                    totalCount: response["@odata.count"] || 0,
                    hasNextPage: !!nextLink,
                };
            })
            .catch(function (error) {
                alert("Error in API call: " + error.message);
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
                    alert(
                        `Retrieved ${result.records.length} records from page ${currentPage}`
                    );
                    alert(`Total records: ${result.records.length}`);
                    alert("Records: " + JSON.stringify(result.records));

                    if (result.hasNextPage) {
                        currentPage++;
                        alert("Fetching next page...");
                        getNextPage();
                    } else {
                        alert("No more pages to fetch.");
                    }
                })
                .catch(function (error) {
                    alert("Error retrieving records: " + error);
                });
        }
        getNextPage();
    }
    return {
        OnRequestApproval: onRequestApproval,
        SendEmailToManager: sendEmailToManager,
        OnClickPagaging: onClickPagaging,
    };
})();
