var Javista = Javista || {};
Javista.Utils = Javista.Utils || {};
Javista.Utils.Workflows = {
    start: function (workflowId, recordId, successCallback, errorCallback) {
        ///<summary>
        /// Démarre une instance du workflow définit pour l'enregistrement défini
        ///</summary>
        ///<param name="workflowId" type="String">
        /// Une chaine qui représente l'identifiant unique du workflow à exécuter.
        ///</param>
        ///<param name="recordId" type="String">
        /// Une chaine qui représente l'identifiant unique de l'enregistrement sur lequel exécuter le workflow.
        ///</param>
        ///<returns>Une chaine qui représente l'identifiant unique de l'instance de workflow</returns>

        //Define the query to execute the action
        var query = "workflows(" +
            workflowId.replace("}", "").replace("{", "") +
            ")/Microsoft.Dynamics.CRM.ExecuteWorkflow";

        var data = {
            "EntityId": recordId.replace("}", "").replace("{", "")
        };

        var req = new XMLHttpRequest();
        req.open("POST", Xrm.Page.context.getClientUrl() + "/api/data/v8.2/" + query, true);
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");

        req.onreadystatechange = function () {
            if (this.readyState === 4 /* complete */) {
                req.onreadystatechange = null;

                if (this.status === 200) {
                    //success callback this returns null since no return value available.
                    var result = JSON.parse(this.response);
                    successCallback(result.asyncoperationid);
                } else {
                    //error callback
                    var error = JSON.parse(this.response).error;
                    errorCallback(error.message);
                }
            }
        };
        req.send(JSON.stringify(data));
    },
    checkState: function (asyncOperationId, callback) {
        ///<summary>
        /// Vérifie l'avancement d'une tâche système
        ///</summary>
        ///<param name="asyncOperationId" type="String">
        /// Une chaine qui représente l'identifiant unique de la tâche système à vérifier.
        ///</param>
        ///<param name="callback" type="String">
        /// Méthode de retour pour la vérification.
        ///</param>
        var query = "asyncoperations(" + asyncOperationId.replace("}", "").replace("{", "") + ")" +
            "?$select=statecode,statuscode,friendlymessage";

        var req = new XMLHttpRequest();
        req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v8.2/" + query, true);
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");

        req.onreadystatechange = function () {
            if (this.readyState === 4 /* complete */) {
                req.onreadystatechange = null;
                var resultData;
                if (this.status === 200) {
                    //success callback this returns null since no return value available.
                    var result = JSON.parse(this.response);
                    resultData = {
                        State: result.statuscode,
                        Message: "Traitement en attente de démarrage..."
                    };

                    switch (result.statuscode) {
                        case 0:
                            resultData.Message = "Traitement en attente de démarrage...";
                            break;
                        case 10:
                            resultData.Message = "Traitement en attente...";
                            break;
                        case 20:
                            resultData.Message = "Traitement en cours...";
                            break;
                        case 30:
                            resultData.Message = "Traitement terminé!";
                            break;
                        case 31:
                            resultData.Message = "Une erreur s'est produite: " + result.friendlymessage + " (Veuillez consulter la trace d'exécution du processus)";
                            break;
                    }

                    callback(resultData);
                } else {
                    resultData = {
                        State: -1,
                        Message: "Traitement en attente de démarrage..."
                    };
                    callback(resultData);
                }
            }
        };
        req.send();
    },
    activateCustomAction: function (actionName, actionParams) {
        var result = null;
        //set OData end-point
        var oDataEndPoint = Xrm.Page.context.getClientUrl() + "/api/data/v8.1/";

        //define request
        var req = new XMLHttpRequest();
        req.open("POST", oDataEndPoint + actionName, false);
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");

        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    result = JSON.parse(this.response);
                } else {
                    var error = JSON.parse(this.response).error.message;
                    alert(error);
                }
            }
        }

        req.send(window.JSON.stringify(actionParams));
        return result;
    },
    __namespace: true
}