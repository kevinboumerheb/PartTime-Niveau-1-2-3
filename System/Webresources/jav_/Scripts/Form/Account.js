var Javista;
(function (Javista) {
    var Form;
    (function (Form) {
        var Account = (function () {
            function Account() {
            }
            Account.prototype.onLoad = function (context) {
                this.formContext = context.getFormContext();
                this.disableControls();
            };
            Account.prototype.onSave = function (context) {
                context.getEventArgs().preventDefault();
            };
            Account.prototype.onNameChange = function (context) {
                var name = this.doUpperCase("name");
                this.createLogTask(name);
            };
            Account.prototype.createLogTask = function (accountName) {
                Xrm.WebApi.createRecord("task", { "subject": "Le nom du compte a changé (nouvelle valeur: " + accountName + ")" }).then(function (id) {
                    Xrm.Navigation.openAlertDialog({
                        text: "L'identifiant de la tâche est " + id,
                        confirmButtonLabel: "Compris!"
                    });
                }, function (error) {
                    Xrm.Navigation.openErrorDialog({
                        message: "Une erreur est survenue",
                        details: error.message
                    });
                });
            };
            Account.prototype.disableControls = function () {
                var _this = this;
                if (this.formContext.ui.getFormType() === 2) {
                    var attrs = ["name", "primarycontactid"];
                    attrs.forEach(function (attribute) {
                        var ctrl = _this.formContext.getControl(attribute);
                        if (ctrl)
                            ctrl.setDisabled(true);
                    });
                }
            };
            Account.prototype.doUpperCase = function (attributeName) {
                var nameAttr = this.formContext.getAttribute(attributeName);
                if (!nameAttr) {
                    Xrm.Navigation.openErrorDialog({
                        message: "L'attribut " + attributeName + " n'est pas présent sur le formulaire"
                    });
                    return null;
                }
                var value = nameAttr.getValue();
                value = value.toUpperCase();
                return value;
            };
            return Account;
        }());
        Form.Account = Account;
    })(Form = Javista.Form || (Javista.Form = {}));
})(Javista || (Javista = {}));
//# sourceMappingURL=Account.js.map