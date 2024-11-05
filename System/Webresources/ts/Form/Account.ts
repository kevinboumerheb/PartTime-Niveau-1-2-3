/// <reference path="../../node_modules/@types/xrm/index.d.ts"/>
namespace Javista.Form {
    export class Account {
        private formContext: Xrm.FormContext;

        public onLoad(context: Xrm.Events.EventContext) {
            // Sauvegarde du contexte du formulaire pour réutilisation dans
            // les autres événements
            this.formContext = context.getFormContext();
            this.disableControls();
        }
        public onSave(context: Xrm.Events.SaveEventContext) {
            // Annulation de la sauvegarde
            context.getEventArgs().preventDefault();
        }
        public onNameChange(context: Xrm.Events.EventContext) {
            var name = this.doUpperCase("name");
            this.createLogTask(name);
        }

        /**
        * @summary Créé une tâche de log avec le nom du compte.
        * @param accountName - Le nom du compte
        * @desc Créé une tâche de log avec le nom du compte. Cette méthode est un exemple à supprimer
        */
        private createLogTask(accountName: string) {
            Xrm.WebApi.createRecord("task", { "subject": "Le nom du compte a changé (nouvelle valeur: " + accountName + ")" }).then(
                id => {
                    Xrm.Navigation.openAlertDialog({
                        text: "L'identifiant de la tâche est " + id,
                        confirmButtonLabel: "Compris!"
                    });
                },
                error => {
                    Xrm.Navigation.openErrorDialog({
                        message: "Une erreur est survenue",
                        details: error.message
                    });
                }
            );
        }
        /**
         * @summary Désactive les contrôles
         * @description Désactive les contrôles sur le formulaire en fonction
         * des règles métiers
         */
        private disableControls() {
            if (this.formContext.ui.getFormType() === XrmEnum.FormType.Update) {
                let attrs: Array<string> = ["name", "primarycontactid"];
                attrs.forEach(attribute => {
                    let ctrl = this.formContext.getControl<Xrm.Controls.StandardControl>(attribute);
                    if (ctrl)
                        ctrl.setDisabled(true);
                });
            }
        }
        /**
         * Transforme une chaine en majuscule
         * @param attributeName - Le texte à mettre en majuscule
         */
        private doUpperCase(attributeName: string): string {
            let nameAttr = this.formContext.getAttribute<Xrm.Attributes.StringAttribute>(attributeName);
            if (!nameAttr) {
                Xrm.Navigation.openErrorDialog({
                    message: "L'attribut " + attributeName + " n'est pas présent sur le formulaire"
                });
                return null;
            }

            let value = nameAttr.getValue();

            value = value.toUpperCase();

            return value;
        }
    }
}